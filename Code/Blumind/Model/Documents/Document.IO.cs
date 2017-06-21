using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Blumind.Core;
using Blumind.Model.MindMaps;

namespace Blumind.Model.Documents
{
    partial class Document
    {
        string _FileName;
        string _Name;

        public event EventHandler FileNameChanged;
        public event EventHandler NameChanged;

        public string FileName
        {
            get { return _FileName; }
            set
            {
                if (_FileName != value)
                {
                    _FileName = value;
                    OnFileNameChanged();
                }
            }
        }

        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnNameChanged();
                }
            }
        }

        void OnNameChanged()
        {
            if (NameChanged != null)
                NameChanged(this, EventArgs.Empty);
        }

        protected virtual void OnFileNameChanged()
        {
            if (!string.IsNullOrEmpty(FileName))
                Name = Path.GetFileNameWithoutExtension(FileName);
            else
                Name = null;

            if (FileNameChanged != null)
                FileNameChanged(this, EventArgs.Empty);
        }

        public static Document Load(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException();

            if (!File.Exists(filename))
                throw new FileNotFoundException();

            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                Document doc = Load(stream);
                if (doc != null)
                {
                    doc.FileName = filename;
                }
                stream.Close();
                return doc;
            }
        }

        public static Document Load(FileStream stream)
        {
            XmlDocument dom = new XmlDocument();
            dom.Load(stream);
            stream.Close();

            return Load(dom);
        }

        public static Document Load(XmlDocument dom)
        {
            if (dom.DocumentElement == null)
                return null;

            // version
            var documentVersion = Document.DocumentVersion;
            if (dom.DocumentElement.HasAttribute("document_version"))
                documentVersion = new Version(dom.DocumentElement.GetAttribute("document_version"));
            else if (dom.DocumentElement.HasAttribute("editor_version"))
            {
                documentVersion = new Version(dom.DocumentElement.GetAttribute("editor_version"));
                if (documentVersion.Major == 3) // 3.0早期的beta版本没有 documentVersion, 而其editor_version 可能略大 DocumentVersion
                    documentVersion = Document.DV_3;
            }
            if (documentVersion > DocumentVersion)
                throw new Exception("Unsupport Document Version");

            // old version document, one chart in a document, 1.5.0 or earlier
            if (dom.DocumentElement.Name == "map")
                return LoadSingleMindMap(documentVersion, dom);

            // new document, multi-charts in a document
            if (dom.DocumentElement.Name != "document")
                return null;

            //
            var doc = new Document();
            
            // information
            var infos = dom.DocumentElement.SelectSingleNode("information") as XmlElement;
            if (infos != null)
            {
                if (documentVersion < Document.DV_3) // 老式的写法
                {
                    doc.Author = ST.ReadTextNode(infos, "Author");
                    doc.Company = ST.ReadTextNode(infos, "Company");
                    doc.Version = ST.ReadTextNode(infos, "Version");
                    doc.Description = ST.ReadTextNode(infos, "Description");
                }
                else
                {
                    doc.Author = ST.ReadTextNode(infos, "author");
                    doc.Company = ST.ReadTextNode(infos, "company");
                    doc.Version = ST.ReadTextNode(infos, "version");
                    doc.Description = ST.ReadTextNode(infos, "description");
                }
            }

            // attributes
            var attrs = dom.DocumentElement.SelectSingleNode("attributes") as XmlElement;
            if (attrs != null)
            {
                foreach (XmlNode attr in attrs.ChildNodes)
                {
                    if (!(attr is XmlElement))
                        continue;
                    XmlElement attr_e = (XmlElement)attr;
                    if (attr_e.Name != "item")
                        continue;

                    doc.Attributes[attr_e.GetAttribute("name")] = attr_e.InnerText;
                }
            }

            // charts
            var charts = dom.DocumentElement.SelectSingleNode("charts") as XmlElement;
            if (charts != null)
            {
                foreach (XmlNode chart in charts)
                {
                    if (!(chart is XmlElement))
                        continue;
                    XmlElement chart_e = (XmlElement)chart;
                    if (chart_e.Name != "chart")
                        continue;

                    ChartPage cdoc = LoadChartDocument(documentVersion, chart_e);
                    if (cdoc != null)
                        doc.Charts.Add(cdoc);
                }

                doc.ActiveChartIndex = ST.GetIntDefault(charts.GetAttribute("active_chart"));
            }

            doc.Modified = false;
            return doc;
        }

        static ChartPage LoadChartDocument(Version documentVersion, XmlElement node)
        {
            ChartPage chart = null;
            ChartType chartType = ST.GetEnumValue(node.GetAttribute("type"), ChartType.MindMap);
            switch (chartType)
            {
                case ChartType.MindMap:
                    chart = new MindMap();// MindMapIO.LoadAsXml(chartElement);
                    break;
                default:
                    return null;
            }

            if (chart != null)
            {
                chart.Deserialize(documentVersion, node);
            }

            return chart;
        }

        static Document LoadSingleMindMap(Version documentVersion, XmlDocument dom)
        {
            MindMap map = MindMap.LoadAsXml(documentVersion, dom.DocumentElement);
            if (map == null)
                return null;

            Document doc = new Document();
            doc.Author = map.Author;
            doc.Company = map.Company;
            doc.Version = map.Version;
            doc.Description = ST.HtmlToText(map.Remark);

            //
            foreach (KeyValuePair<string, string> attr in map.ExtendAttributes)
            {
                doc.Attributes[attr.Key] = attr.Value;
            }

            doc.Charts.Add(map);

            return doc;
        }

        public void Save()
        {
            Save(FileName);
        }

        public void Save(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException();

            XmlDocument dom = SaveAsXml();
            if (dom == null)
                return;

            using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                dom.Save(stream);
                FileName = filename;
                stream.Close();
            }
        }

        public void Save(FileStream stream)
        {
            XmlDocument dom = SaveAsXml();
            if (dom == null)
                return;

            dom.Save(stream);
        }

        XmlDocument SaveAsXml()
        {
            XmlDocument dom = new XmlDocument();
            dom.AppendChild(dom.CreateXmlDeclaration("1.0", "utf-8", null));

            XmlElement root = dom.CreateElement("document");
            root.SetAttribute("type", "BLUMIND");
            root.SetAttribute("editor_version", ProductInfo.Version);
            root.SetAttribute("document_version", Document.DocumentVersion.ToString());
            dom.AppendChild(root);
            
            //
            XmlComment comment = dom.CreateComment("Create by Blumind, you can download it free from http://www.blumind.org");
            root.AppendChild(comment); 

            // information
            XmlElement infos = dom.CreateElement("information");
            ST.WriteTextNode(infos, "author", Author);
            ST.WriteTextNode(infos, "company", Company);
            ST.WriteTextNode(infos, "version", Version);
            ST.WriteTextNode(infos, "description", Description);
            root.AppendChild(infos);

            // attributes
            XmlElement attrs = dom.CreateElement("attributes");
            foreach (KeyValuePair<string, object> attr in Attributes)
            {
                XmlElement attr_e = dom.CreateElement("item");
                attr_e.SetAttribute("name", attr.Key);
                if (attr.Value != null)
                    attr_e.InnerText = attr.Value.ToString();
                attrs.AppendChild(attr_e);
            }
            root.AppendChild(attrs);

            // charts
            XmlElement charts = dom.CreateElement("charts");
            charts.SetAttribute("active_chart", ActiveChartIndex.ToString());
            foreach (ChartPage cdoc in Charts)
            {
                var chart_e = dom.CreateElement("chart");
                cdoc.Serialize(dom, chart_e);

                //switch (cdoc.Type)
                //{
                //    case ChartType.MindMap:
                //        MindMapIO.SaveAsXml((MindMap)cdoc, chart_e);
                //        break;
                //    default:
                //        continue;
                //}
                charts.AppendChild(chart_e);
            }
            root.AppendChild(charts);

            return dom;
        }
    }
}
