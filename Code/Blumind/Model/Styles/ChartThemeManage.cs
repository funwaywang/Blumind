using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Blumind.Configuration;
using Blumind.Core;
using Blumind.Core.Documents;
using Blumind.Globalization;

namespace Blumind.Model.Styles
{
    class ChartThemeManage
    {
        static ChartThemeManage _Default;
        ChartTheme _DefaultTheme;
        ChartThemeFolder _Internals;
        ChartThemeFolder _Extensions;

        public static ChartThemeManage Default
        {
            get 
            {
                if (_Default == null)
                {
                    _Default = new ChartThemeManage();
                    _Default.Refresh();
                }

                return _Default; 
            }
        }

        ChartThemeManage()
        {
        }

        static string ThemesDirectory
        {
            get { return Path.Combine(ProgramEnvironment.ApplicationDataDirectory, "Themes"); }
        }

        public ChartTheme DefaultTheme
        {
            get 
            {
                if (_DefaultTheme != null)
                    return _DefaultTheme;
                else
                    return StandardTheme;
            }
            set
            {
                if (_DefaultTheme != value)
                {
                    _DefaultTheme = value;
                    Options.Current.SetValue(OptionNames.Charts.DefaultChartTheme, value == null ? null : value.Name);
                }
            }
        }

        ChartTheme StandardTheme { get; set; }

        public ChartThemeFolder Internals
        {
            get { return _Internals; }
            private set { _Internals = value; }
        }

        public ChartThemeFolder Extensions
        {
            get { return _Extensions; }
            private set { _Extensions = value; }
        }

        IEnumerable<ChartTheme> AllThemes
        {
            get
            {
                if (Internals != null)
                    foreach (var ct in Internals.AllThemes)
                        yield return ct;

                if (Extensions != null)
                    foreach (var ct in Extensions.AllThemes)
                        yield return ct;
            }
        }

        public void Refresh()
        {
            // Load Internal Themes
            Internals = LoadInternal();

            // Load Extersion Themes
            Extensions = new ChartThemeFolder("Extension");
            LoadExtersionThemes(ThemesDirectory, Extensions);

            //
            if (Options.Current.Contains(OptionNames.Charts.DefaultChartTheme))
            {
                var dct = Options.Current.GetString(OptionNames.Charts.DefaultChartTheme);
                DefaultTheme = AllThemes.Find(t => StringComparer.OrdinalIgnoreCase.Equals(t.Name, dct));
            }
        }

        internal static ChartThemeFolder LoadFromXml(string text)
        {
            XmlDocument dom = new XmlDocument();
            dom.LoadXml(text);

            if (dom.DocumentElement.Name != "themes")
                return null;

            ChartThemeFolder folder = new ChartThemeFolder();
            folder.Name = dom.DocumentElement.GetAttribute("name");

            LoadFromXml(dom.DocumentElement, folder);

            return folder;
        }

        static void LoadFromXml(XmlElement xmlElement, ChartThemeFolder folder)
        {
            // folder
            XmlNodeList foldNodes = xmlElement.SelectNodes("folder");
            foreach (XmlElement foldNode in foldNodes)
            {
                ChartThemeFolder subFolder = new ChartThemeFolder();
                subFolder.Name = foldNode.GetAttribute("name");
                LoadFromXml(foldNode, subFolder);
                if (subFolder.Themes.Count > 0 || subFolder.Folders.Count > 0)
                {
                    folder.Folders.Add(subFolder);
                }
            }

            // theme
            XmlNodeList nodes = xmlElement.SelectNodes("theme");
            if (nodes.Count > 0)
            {
                foreach (XmlNode node in nodes)
                {
                    if (node is XmlElement)
                    {
                        ChartTheme theme = DeserializeTheme(node as XmlElement);
                        if (theme != null)
                        {
                            theme.SetIsInternal(true);
                            folder.Themes.Add(theme);
                        }
                    }
                }
            }
        }

        ChartThemeFolder LoadInternal()
        {
            var folder = LoadFromXml(Properties.Resources.themes);
            if (folder.Themes.Count > 0)
                StandardTheme = folder.Themes[0];
            else
                StandardTheme = null;

            // load windows
            var windowsThemes = WindowsColorSchemes.LoadThemes();
            if (windowsThemes != null)
            {
                folder.Folders.Add(windowsThemes);
            }

            return folder;
        }

        private void LoadExtersionThemes(string direcotry, ChartThemeFolder parentFolder)
        {
            if (!Directory.Exists(direcotry))
                return;

            string[] dirs = Directory.GetDirectories(direcotry);
            foreach (string dir in dirs)
            {
                ChartThemeFolder folder = new ChartThemeFolder(Path.GetFileName(dir));
                LoadExtersionThemes(dir, folder);
                if (folder.Folders.Count > 0 || folder.Themes.Count > 0)
                    parentFolder.Folders.Add(folder);
            }

            string[] files = Directory.GetFiles(direcotry, "*.xml");
            foreach (string filename in files)
            {
                try
                {
                    ChartTheme theme = LoadTheme(filename);
                    if (theme != null)
                        parentFolder.Themes.Add(theme);
                }
                catch(System.Exception ex)
                {
                    Helper.WriteLog(ex);
                }
            }

            //if (folder.Themes.Count > 0 || folder.Folders.Count > 0)
            //{
            //    parentFolder.Folders.Add(folder);
            //}
        }

        private ChartTheme LoadTheme(string filename)
        {
            XmlDocument dom = new XmlDocument();
            dom.Load(filename);
            if (dom.DocumentElement.Name != "theme")
                throw new Exception(Lang._("Invalid file type"));

            ChartTheme theme = DeserializeTheme(dom.DocumentElement);
            if (theme != null)
            {
                theme.Filename = filename;                
            }

            return theme;
        }

        public static void Delete(ChartTheme theme)
        {
            if (theme.Folder != null)
            {
                theme.Folder.Themes.Remove(theme);
            }

            if (!string.IsNullOrEmpty(theme.Filename) && File.Exists(theme.Filename))
            {
                try
                {
                    File.Delete(theme.Filename);
                }
                catch(System.Exception ex)
                {
                    Helper.WriteLog(ex);
                }
            }
        }

        public static string GetThemeFilename(string themeName)
        {
            return GetThemeFilename(ThemesDirectory, themeName);
        }

        public static string GetThemeFilename(string directory, string themeName)
        {
            return Path.Combine(directory, ST.EscapeFileName(string.Format("{0}.xml", themeName)));
        }

        public void Save()
        {
            if (Extensions != null)
            {
                Save(Extensions, ThemesDirectory);
            }
        }

        public void Save(ChartThemeFolder folder, string directory)
        {
            foreach (ChartThemeFolder subFolder in folder.Folders)
            {
                Save(subFolder, Path.Combine(directory, ST.EscapeFileName(subFolder.Name)));
            }

            foreach (ChartTheme theme in folder.Themes)
            {
                string filename = Path.Combine(directory, GetThemeFilename(theme.Name));

                if (!string.IsNullOrEmpty(theme.Filename) && !StringComparer.OrdinalIgnoreCase.Equals(filename, theme.Filename))
                {
                    try
                    {
                        File.Delete(theme.Filename);
                    }
                    catch (System.Exception ex)
                    {
                        Helper.WriteLog(ex);
                    }
                }

                SaveTheme(theme, filename);
            }
        }

        public void SaveTheme(ChartTheme theme, string filename)
        {
            if (theme == null || string.IsNullOrEmpty(filename))
                throw new ArgumentNullException();

            string dir = Path.GetDirectoryName(filename);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            XmlDocument dom = new XmlDocument();
            dom.LoadXml("<?xml version='1.0' encoding='utf-8' ?><theme/>");

            SerializeTheme(dom.DocumentElement, theme);

            dom.Save(filename);
            theme.Filename = filename;
        }

        static void SerializeTheme(XmlElement node, ChartTheme theme)
        {
            if (theme == null || node == null)
                throw new ArgumentNullException();

            //SerializeMapStyle(theme, node);

            node.SetAttribute("name", theme.Name);
            ST.SerializeColor(node, "root_back_color", theme.RootBackColor);
            ST.SerializeColor(node, "root_fore_color", theme.RootForeColor);
            ST.SerializeColor(node, "root_border_color", theme.RootBorderColor);
            ST.SerializeColor(node, "back_color", theme.BackColor);
            ST.SerializeColor(node, "fore_color", theme.ForeColor);
            ST.SerializeColor(node, "line_color", theme.LineColor);
            ST.SerializeColor(node, "border_color", theme.BorderColor);
            ST.SerializeColor(node, "node_back_color", theme.NodeBackColor);
            ST.SerializeColor(node, "node_fore_color", theme.NodeForeColor);
            ST.SerializeColor(node, "select_color", theme.SelectColor);
            ST.SerializeColor(node, "hover_color", theme.HoverColor);
            ST.SerializeColor(node, "link_line_color", theme.LinkLineColor);

            if (!string.IsNullOrEmpty(theme.Description))
            {
                ST.WriteTextNode(node, "description", theme.Description);
            }

            if (theme.LayerSpace != MindMapStyle.DefaultLayerSpace)
                ST.WriteTextNode(node, "layer_space", theme.LayerSpace.ToString());

            if (theme.ItemsSpace != MindMapStyle.DefaultItemsSpace)
                ST.WriteTextNode(node, "items_space", theme.ItemsSpace.ToString());

            if (theme.Font != null)
            {
                XmlElement font = node.OwnerDocument.CreateElement("font");
                ST.WriteFontNode(font, theme.Font);
                node.AppendChild(font);
            }
        }

        static ChartTheme DeserializeTheme(XmlElement node)
        {
            var theme = new ChartTheme();
            //DeserializeMapStyle(node, theme);
            theme.BackColor = ST.DeserializeColor(node, "back_color", theme.BackColor);
            theme.ForeColor = ST.DeserializeColor(node, "fore_color", theme.ForeColor);
            theme.LineColor = ST.DeserializeColor(node, "line_color", theme.LineColor);
            theme.BorderColor = ST.DeserializeColor(node, "border_color", theme.BorderColor);
            theme.NodeBackColor = ST.DeserializeColor(node, "node_back_color", theme.NodeBackColor);
            theme.NodeForeColor = ST.DeserializeColor(node, "node_fore_color", theme.NodeForeColor);
            theme.SelectColor = ST.DeserializeColor(node, "select_color", theme.SelectColor);
            theme.HoverColor = ST.DeserializeColor(node, "hover_color", theme.HoverColor);
            theme.LinkLineColor = ST.DeserializeColor(node, "link_line_color", theme.LinkLineColor);
            theme.LayerSpace = ST.GetInt(ST.ReadTextNode(node, "layer_space"), MindMapStyle.DefaultLayerSpace);
            theme.ItemsSpace = ST.GetInt(ST.ReadTextNode(node, "items_space"), MindMapStyle.DefaultItemsSpace);

            XmlElement fontNode = node.SelectSingleNode("font") as XmlElement;
            if (fontNode != null)
            {
                theme.Font = ST.ReadFontNode(fontNode);
            }

            theme.Name = node.GetAttribute("name");
            theme.RootBackColor = ST.DeserializeColor(node, "root_back_color", theme.RootBackColor);
            theme.RootForeColor = ST.DeserializeColor(node, "root_fore_color", theme.RootForeColor);
            theme.RootBorderColor = ST.DeserializeColor(node, "root_border_color", theme.RootBorderColor);
            theme.Description = ST.ReadCDataNode(node, "description");
            if (theme.Description == string.Empty)
                theme.Description = ST.ReadTextNode(node, "description");


            return theme;
        }
    }
}
