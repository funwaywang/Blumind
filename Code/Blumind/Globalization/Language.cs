using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Collections.Generic;
using Blumind.Core;

namespace Blumind.Globalization
{
    class Language
    {
        Hashtable _Words;

        public Language()
        {
            Words = new Hashtable(StringComparer.OrdinalIgnoreCase);
            Authors = new List<AuthorInfo>();
        }

        public Language(string id, string name)
            : this()
        {
            ID = id;
            Name = name;
        }

        public string ID { get; private set; }

        public string Name { get; set; }

        public string NatureName { get; set; }

        public bool Loaded { get; private set; }

        public string Filename { get; set; }

        public bool Stable { get; set; }

        public string[] Compatibilities { get; private set; }

        public Hashtable Words
        {
            get { return _Words; }
            private set { _Words = value; }
        }

        public List<AuthorInfo> Authors { get; private set; }

        public bool CompatibleWith(string cultureId)
        {
            if (Compatibilities != null)
            {
                foreach (string id in Compatibilities)
                    if (StringComparer.OrdinalIgnoreCase.Equals(id, cultureId))
                        return true;
            }

            return false;
        }

        public bool Contains(string name)
        {
            return Words.Contains(name);
        }

        public string this[string name]
        {
            get
            {
                return GetText(name, name);
            }
        }

        public string GetText(string name)
        {
            return GetText(name, name);
        }

        public string GetText(string name, string defaultValue)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            EnsureLoaded();

            if (Words.Contains(name))
            {
                return (string)Words[name];
            }
            else
            {
                return defaultValue;
            }
        }

        public string GetText2(string name1, string name2)
        {
            if (name1 != null && Words.Contains(name1))
                return (string)Words[name1];
            else if (name2 != null && Words.Contains(name2))
                return (string)Words[name2];
            else if (string.IsNullOrEmpty(name1))
                return name2;
            else
                return name1;
        }

        public void Merge(Language lang)
        {
            if (lang == null)
                throw new ArgumentNullException();

            foreach (DictionaryEntry de in lang.Words)
            {
                Words[de.Key] = de.Value;
            }
        }

        public void Apply(Control control)
        {
            LanguageManage.Apply(this, control);
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Name, NatureName);
        }

        #region Load

        public bool LoadInfo(string filename)
        {
            if (File.Exists(filename))
            {
                XmlDocument dom = new XmlDocument();
                try
                {
                    dom.Load(filename);
                }
                catch(System.Exception ex)
                {
                    Helper.WriteLog(ex);
                    return false;
                }

                return LoadInfo(dom);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public bool Load(string filename)
        {
            //if (StringComparer.OrdinalIgnoreCase.Equals(Path.GetFileNameWithoutExtension(filename), "cs.CZ"))
            //    D.Message(string.Format("Load {0}", filename));
            if (File.Exists(filename))
            {
                XmlDocument dom = new XmlDocument();
                bool failed = false;
                using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    try
                    {
                        dom.Load(stream);
                    }
                    catch (System.Exception ex)
                    {
                        //D.Message(string.Format("Load Exception: {0}", ex.Message));
                        Helper.WriteLog(ex);
                        failed = true;
                    }

                    stream.Close();
                }

                if (!failed && LoadXml(dom))
                {
                    Filename = filename;
                    return true;
                }
            }
            else
            {
                //if (StringComparer.OrdinalIgnoreCase.Equals(Path.GetFileNameWithoutExtension(filename), "cs.CZ"))
                //    D.Message("Not Exists");
            }

            return false;
        }

        public static Language LoadFile(string filename)
        {
            //if (StringComparer.OrdinalIgnoreCase.Equals(Path.GetFileNameWithoutExtension(filename), "cs.CZ"))
            //    D.Message(string.Format("LoadFile() {0}", filename));
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                return null;

            var lang = new Language();
            if (lang.Load(filename))
            {
                //if (StringComparer.OrdinalIgnoreCase.Equals(Path.GetFileNameWithoutExtension(filename), "cs.CZ"))
                //{
                //    D.Message(string.Format(">>Lang {0}, Words {1}", lang, lang.Words.Count));
                //    D.Message(string.Format("Test: Options => {0}", Lang._("Options")));
                //}
                return lang;
            }
            else
            {
                //D.Message("LoadFile(filename) Fail");
                return null;
            }
        }

        public static Language LoadXml(string text)
        {
            Language language = null;
            XmlDocument dom = new XmlDocument();
            try
            {
                dom.LoadXml(text);
                language = new Language();
                language.LoadXml(dom);
            }
            catch(System.Exception ex)
            {
                Helper.WriteLog(ex);
                return null;
            }

            return language;
        }

        public bool LoadXml(XmlDocument dom)
        {
            if (dom.DocumentElement == null)
                return false;

            if (!LoadInfo(dom))
                return false;
            
            Words.Clear();
            XmlNode wordsNode = dom.DocumentElement.SelectSingleNode("words");
            if (wordsNode != null)
            {
                LoadNodes(Words, wordsNode);
            }

            Loaded = true;
            return true;
        }

        bool LoadInfo(XmlDocument dom)
        {
            XmlElement node_info = dom.DocumentElement.SelectSingleNode("information") as XmlElement;
            if (node_info != null)
            {
                ID = node_info.GetAttribute("id");
                Name = node_info.GetAttribute("name");
                NatureName = node_info.GetAttribute("nature_name");
                Stable = ST.GetBoolDefault(node_info.GetAttribute("stable"));

                LoadAuthors(node_info);

                LoadCompatibilities(dom);

                return true;
            }
            else
            {
                return false;
            }
        }

        void LoadAuthors(XmlElement node_info)
        {
            Authors.Clear();
            foreach (XmlElement an in node_info.SelectNodes("author"))
            {
                AuthorInfo aif = new AuthorInfo(an.GetAttribute("name"), an.GetAttribute("email"));
                if (!string.IsNullOrEmpty(aif.Name))
                    Authors.Add(aif);
            }
        }

        void LoadCompatibilities(XmlDocument dom)
        {
            XmlElement compatibilities = dom.DocumentElement.SelectSingleNode("compatibility") as XmlElement;
            if (compatibilities != null)
            {
                List<string> cs = new List<string>();
                foreach (XmlElement element in compatibilities.ChildNodes)
                {
                    string id = element.GetAttribute("id");
                    if (!string.IsNullOrEmpty(id))
                        cs.Add(id);
                }

                Compatibilities = cs.ToArray();
            }
            else
            {
                Compatibilities = new string[0];
            }
        }

        void LoadNodes(Hashtable hashtable, XmlNode group)
        {
            XmlNodeList list = group.SelectNodes("item");
            if (list != null)
            {
                foreach (XmlElement node in list)
                {
                    string name = node.GetAttribute("name");
                    if (!hashtable.Contains(name))
                        hashtable.Add(name, node.InnerText);
                }
            }
        }

        public void EnsureLoaded()
        {
            if (!Loaded)
            {
                Load(this.Filename);
            }
        }

        #endregion
    }
}
