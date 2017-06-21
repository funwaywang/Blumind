using System;
using System.Drawing;
using System.IO;
using System.Xml;
using Blumind.Core;

namespace Blumind.Controls
{
    class UIColorTheme
    {
        const string FileType = "blumind-ui-theme";

        public UIColorTheme()
        {
        }

        public UIColorTheme(string name, Color dark, Color light, Color sharp, Color mediumDark, Color mediumLight, Color workSpace, string description)
        {
            Name = name;
            Dark = dark;
            Light = light;
            Sharp = sharp;
            MediumDark = mediumDark;
            MediumLight = mediumLight;
            Workspace = workSpace;
            Description = description;
        }

        public UIColorTheme(string name, Color dark, Color light, Color sharp, Color mediumDark, Color mediumLight)
            : this(name, dark, light, sharp, mediumDark, mediumLight, dark, null)
        {
            Name = name;
        }

        public UIColorTheme(string name, string dark, string light, string sharp, string mediumDark, string mediumLight, string workSpace, string description)
        {
            Name = name;
            Dark = ST.GetColor(dark);
            Light = ST.GetColor(light);
            Sharp = ST.GetColor(sharp);
            MediumDark = ST.GetColor(mediumDark);
            MediumLight = ST.GetColor(mediumLight);
            Workspace = ST.GetColor(workSpace);
            Description = description;
        }

        public string Name { get; set; }

        public Color Dark { get; set; }

        public Color Light { get; set; }

        public Color Sharp { get; set; }

        public Color MediumDark { get; set; }

        public Color MediumLight { get; set; }

        public Color Workspace { get; set; }

        public string Description { get; set; }

        public Color AlternateRowBackColor
        {
            get { return PaintHelper.AdjustColor(MediumLight, 0, 100, 95, 100); }
        }

        public Color SharpText
        {
            get
            {
                return PaintHelper.FarthestColor(Sharp, Light, Dark);
            }
        }

        public Color Control
        {
            get { return MediumLight; }
        }

        public Color ControlText
        {
            get { return PaintHelper.FarthestColor(Control, Dark, Light); }
        }

        public Color MenuImageMargin
        {
            get
            {
                return PaintHelper.Darker(PaintHelper.GetLightColor(MediumLight, 0.5f), Light);
            }
        }

        public Color Window
        {
            get
            {
                return SystemColors.Window;
            }
        }

        public Color WindowText
        {
            get
            {
                return SystemColors.WindowText;
            }
        }

        public Color BorderColor
        {
            get
            {
                return SystemColors.ControlDark;
            }
        }

        public Image CreateThumb(Size size)
        {
            if (size.Width <= 0 || size.Height <= 0)
                return null;

            Bitmap bmp = new Bitmap(size.Width, size.Height);
            using (var grf = Graphics.FromImage(bmp))
            {
                var rect = new Rectangle(0, 0, size.Width, size.Height);
                var colors = new Color[] { Light, Dark, MediumLight, MediumDark, Sharp, Workspace };
                int columns = 2;
                int rows = colors.Length % columns > 0 ? (colors.Length / columns + 1) : (colors.Length / columns);
                var cw = (float)rect.Width / columns;
                var ch = (float)rect.Height / rows;
                float x = rect.X;
                float y = rect.Y;
                for (int i = 0; i < colors.Length; i++)
                {
                    var cr = new RectangleF(x, y, cw, ch);
                    grf.FillRectangle(new SolidBrush(colors[i]), cr);

                    if (i % columns == columns - 1)
                    {
                        y += ch;
                        x = rect.X;
                    }
                    else
                    {
                        x += cw;
                    }
                }
            }

            return bmp;
        }

        public void Save(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException();

            var dom = new XmlDocument();
            Save(dom);

            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                dom.Save(stream);
            }
        }

        public void Save(XmlDocument dom)
        {
            if (dom == null)
                throw new ArgumentNullException();

            dom.AppendChild(dom.CreateXmlDeclaration("1.0", "utf-8", null));
            var root = dom.CreateElement("theme");
            dom.AppendChild(root);

            root.SetAttribute("name", Name);
            root.SetAttribute("type", FileType);
            root.SetAttribute("version", "1.0");

            var colors = dom.CreateElement("colors");
            root.AppendChild(colors);
            colors.AppendChild(CreateColorNode(dom, "Dark", Dark));
            colors.AppendChild(CreateColorNode(dom, "Light", Light));
            colors.AppendChild(CreateColorNode(dom, "Sharp", Sharp));
            colors.AppendChild(CreateColorNode(dom, "MediumDark", MediumDark));
            colors.AppendChild(CreateColorNode(dom, "MediumLight", MediumLight));
            colors.AppendChild(CreateColorNode(dom, "Workspace", Workspace));

            ST.WriteTextNode(root, "description", Description);
        }

        public static UIColorTheme Load(string filename)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                return null;

            var dom = new XmlDocument();
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    dom.Load(stream);
                }
                catch
                {
                    return null;
                }
            }

            return Load(dom);
        }

        public static UIColorTheme Load(XmlDocument dom)
        {
            if (dom == null || dom.DocumentElement == null)
                return null;

            var root = dom.DocumentElement;
            if (!StringComparer.OrdinalIgnoreCase.Equals(root.GetAttribute("type"), FileType))
                return null;

            var t = new UIColorTheme();
            t.Name = root.GetAttribute("name");
            t.Description = ST.ReadTextNode(root, "description");

            var colors = root.SelectSingleNode("colors");
            if (colors != null)
            {
                foreach (XmlElement cn in colors.ChildNodes)
                {
                    switch (cn.GetAttribute("name").ToLower())
                    {
                        case "dark":
                            t.Dark = ST.GetColor(cn.GetAttribute("value"), t.Dark);
                            break;
                        case "light":
                            t.Light = ST.GetColor(cn.GetAttribute("value"), t.Light);
                            break;
                        case "sharp":
                            t.Sharp = ST.GetColor(cn.GetAttribute("value"), t.Sharp);
                            break;
                        case "mediumdark":
                            t.MediumDark = ST.GetColor(cn.GetAttribute("value"), t.MediumDark);
                            break;
                        case "mediumlight":
                            t.MediumLight = ST.GetColor(cn.GetAttribute("value"), t.MediumLight);
                            break;
                        case "workspace":
                            t.Workspace = ST.GetColor(cn.GetAttribute("value"), t.Workspace);
                            break;
                    }
                }
            }

            return t;
        }

        XmlElement CreateColorNode(XmlDocument dom, string name, Color value)
        {
            var node = dom.CreateElement("color");
            node.SetAttribute("name", name);
            node.SetAttribute("value", ST.ToString(value));
            return node;
        }
    }
}
