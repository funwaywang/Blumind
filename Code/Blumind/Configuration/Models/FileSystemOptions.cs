using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Blumind.Core;

namespace Blumind.Configuration.Models
{
    class FileSystemOptions : Options
    {
        const string FileName = "Blumind.xml";
        const string ForceRunAsPortable = "-p";

        public FileSystemOptions()
        {
        }

        public FileSystemOptions(Options options)
            : base(options)
        {
        }

        string LocalFilename
        {
            get { return Path.Combine(ProgramEnvironment.GetApplicationDataDirectory(ProgramRunMode.Portable), FileName); }
        }

        string UserFilename
        {
            get 
            {
                return Path.Combine(ProgramEnvironment.GetApplicationDataDirectory(ProgramRunMode.Standard), FileName); 
            }
        }

        public override void Load(string[] args)
        {
            Data.Clear();

            bool forcePortable = args.Contains(ForceRunAsPortable, StringComparer.OrdinalIgnoreCase);

            if (File.Exists(LocalFilename))
            {
                LoadFile(LocalFilename);
                ProgramEnvironment.RunMode = ProgramRunMode.Portable;
            }
            
            if (!forcePortable && File.Exists(UserFilename))
            {
                LoadFile(UserFilename);
                ProgramEnvironment.RunMode = ProgramRunMode.Standard;
            }

            if (forcePortable)
            {
                ProgramEnvironment.RunMode = ProgramRunMode.Portable;
            }
        }

        void LoadFile(string filename)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                return;

            XmlDocument dom = new XmlDocument();
            try
            {
                dom.Load(filename);
            }
            catch (System.Exception ex)
            {
                Helper.WriteLog(ex);
                return;
            }

            XmlElement root = dom.DocumentElement;
            if (root.Name != "options")
                return;

            var nodes = root.SelectNodes("item");
            foreach (XmlElement node in root.ChildNodes)
            {
                var key = node.GetAttribute("key");
                if (string.IsNullOrEmpty(key))
                    continue;

                SetValue(key, DeserializeValue(node));
            }
        }

        public override bool Save()
        {
            var dom = new XmlDocument();
            dom.AppendChild(dom.CreateXmlDeclaration("1.0", "utf-8", null));
            dom.AppendChild(dom.CreateElement("options"));
            var root = dom.DocumentElement;
            foreach (var item in Data)
            {
                var node = dom.CreateElement("item");
                node.SetAttribute("key", item.Key);
                root.AppendChild(node);

                SerializeValue(node, item.Value);
            }

            //
            if (ProgramEnvironment.RunMode == ProgramRunMode.Portable)
            {
                if (TrySaveFile(dom, LocalFilename))
                    return true;
            }

            if (TrySaveFile(dom, UserFilename))
            {
                ProgramEnvironment.RunMode = ProgramRunMode.Standard;
                return true;
            }

            return false;
        }

        bool TrySaveFile(XmlDocument dom, string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return false;

            try
            {
                FileInfo file = new FileInfo(filename);
                using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    dom.Save(stream);
                    stream.Close();
                }

                return File.Exists(filename);
            }
            catch
            {
                return false;
            }
        }
    }
}
