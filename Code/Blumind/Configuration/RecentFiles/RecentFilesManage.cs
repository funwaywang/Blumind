using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Blumind.Core;

namespace Blumind.Configuration
{
    class RecentFilesManage : IEnumerable<RecentFileInfo>
    {
        public static RecentFilesManage Default { get; private set; }
        List<RecentFileInfo> Files = new List<RecentFileInfo>();

        public event System.EventHandler FilesChanged;

        static RecentFilesManage()
        {
            Default = new RecentFilesManage();
        }

        public RecentFilesManage()
        {
            Files = new List<RecentFileInfo>();
        }
        
        public RecentFileInfo this[int index]
        {
            get
            {
                if (index < 0 || index >= Files.Count)
                    throw new IndexOutOfRangeException();
                else
                    return Files[index];
            }
        }

        public int Count
        {
            get { return Files.Count; }
        }

        public void Initialize()
        {
            Load();
        }

        public void Push(string filename)
        {
            Image thumbImage = null;
            if (Files.Count > 0)
            {
                var file = Files.Find(f => string.Equals(f.Filename, filename));
                if (file != null)
                    thumbImage = file.ThumbImage;
            }

            Push(filename, thumbImage);
        }

        public void Push(string filename, Image thumbImage)
        {
            _Remove(filename);            
            Files.Add(new RecentFileInfo(filename, thumbImage));

            OnFilesChanged();

            TrySave();
        }

        public bool Remove(string filename)
        {
            if (_Remove(filename) > 0)
            {
                TrySave();
                return true;
            }
            else
            {
                return false;
            }
        }

        int _Remove(string filename)
        {
            var count = Files.RemoveAll(f => String.Equals(f.Filename, filename, StringComparison.InvariantCultureIgnoreCase));
            if (count > 0)
            {
                OnFilesChanged();
            }

            return count;
        }

        void OnFilesChanged()
        {
            if (FilesChanged != null)
                FilesChanged(null, EventArgs.Empty);
        }

        public void Clear()
        {
            if (!Files.IsEmpty())
            {
                Files.Clear();
                TrySave();
            }
        }

        public bool TrySave()
        {
            if (!Options.Current.GetValue(OptionNames.Miscellaneous.SaveRecentFiles, true))
                return false;

            return Save();
        }

        public bool Save()
        {
            XmlDocument dom = new XmlDocument();
            Save(dom);

            string filename = ProgramEnvironment.RecentsFilename;
            if (!string.IsNullOrEmpty(filename))
            {
                var dir = Path.GetDirectoryName(filename);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    try
                    {
                        Directory.CreateDirectory(dir);
                    }
                    catch (System.Exception ex)
                    {
                        Helper.WriteLog(ex);
                        return false;
                    }
                }
            }

            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                try
                {
                    dom.Save(fs);
                }
                catch (System.Exception ex)
                {
                    Helper.WriteLog(ex);
                    return false;
                }
            }

            return true;
        }

        void Save(XmlDocument dom)
        {
            dom.AppendChild(dom.CreateXmlDeclaration("1.0", "utf-8", null));
            var root = dom.CreateElement("recent_files");
            dom.AppendChild(root);

            foreach (var f in Files)
            {
                var node = dom.CreateElement("file");
                root.AppendChild(node);

                ST.WriteTextNode(node, "filename", f.Filename);
                if (f.ThumbImage != null)
                    ST.WriteImageNode(node, "thumb", f.ThumbImage);
            }
        }

        bool Load()
        {
            string filename = ProgramEnvironment.RecentsFilename;
            if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
            {
                XmlDocument dom = new XmlDocument();
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        dom.Load(fs);
                    }
                    catch (System.Exception ex)
                    {
                        Helper.WriteLog(ex);
                        return false;
                    }
                }

                return Load(dom);
            }

            return false;
        }

        bool Load(XmlDocument dom)
        {
            var root = dom.DocumentElement;
            if (root == null)
                return false;
            if (root.Name != "recent_files")
                return false;

            foreach (XmlElement node in root.SelectNodes("file"))
            {
                RecentFileInfo f = new RecentFileInfo();
                f.Filename = ST.ReadTextNode(node, "filename");
                f.ThumbImage = ST.ReadImageNode(node, "thumb");

                Files.Add(f);
            }

            return true;
        }

        public IEnumerator<RecentFileInfo> GetEnumerator()
        {
            return Files.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Files.GetEnumerator();
        }
    }
}
