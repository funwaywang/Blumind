using System;
using System.Drawing.Drawing2D;
using System.IO;
using System.Xml;
using Blumind.Core.Documents;
using Blumind.Model;

namespace Blumind.Model.MindMaps
{
    class MindMapIO
    {
        /*public static MindMap LoadFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            if (!File.Exists(filename))
                throw new FileNotFoundException(null, filename);

            MindMap map = LoadAsXml(filename);

            if (map != null)
            {
                map.Filename = filename;
                //if (string.IsNullOrEmpty(map.Name))
                //{
                    map.Name = Path.GetFileNameWithoutExtension(filename);
                //}
            }

            return map;
        }

        static MindMap LoadAsXml(string filename)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                throw new FileNotFoundException();

            XmlDocument dom = new XmlDocument();
            dom.Load(filename);

            if (dom.DocumentElement.Name != "map")
                throw new ArgumentException("filename");

            return LoadAsXml(dom.DocumentElement);
        }

        public static MindMap LoadAsXml(XmlElement docElement)
        {
            // doc-root
            MindMap map = new MindMap(docElement.GetAttribute("name"));
            map.LayoutType = ST.GetLayoutType(ST.ReadTextNode(docElement, "layout"));
            map.Description = ST.ReadCDataNode(docElement, "description");
            if (map.Description == string.Empty)
                map.Description = ST.ReadTextNode(docElement, "description");

            // info
            XmlElement infoNode = docElement.SelectSingleNode("info") as XmlElement;
            if (infoNode != null)
            {
                LoadInfomation(map, infoNode);
            }

            // style
            XmlElement styleNode = docElement.SelectSingleNode("style") as XmlElement;
            if (styleNode != null)
            {
                XmlSerializer.DeserializeMapStyle(styleNode, map);
            }

            // extend attributes
            XmlElement attributeNode = docElement.SelectSingleNode("attributes") as XmlElement;
            if (attributeNode != null)
            {
                LoadExtendAttributes(map, attributeNode);
            }

            // nodes
            XmlElement rootNode = docElement.SelectSingleNode("nodes/node") as XmlElement;
            if (rootNode != null)
            {
                var topic = Topic.DeserializeTopic(rootNode);
                map.Root = topic;
            }

            return map;
        }

        static void LoadInfomation(MindMap map, XmlElement info)
        {
            if (map == null || info == null)
                return;

            map.Author = ST.ReadTextNode(info, "author");
            map.Company = ST.ReadTextNode(info, "company");
            map.Version = ST.ReadTextNode(info, "version");
        }

        static void LoadExtendAttributes(MindMap map, XmlElement node)
        {
            if (map == null || node == null)
                return;

            XmlNodeList list = node.SelectNodes("item");
            foreach (XmlElement attNode in list)
            {
                map.ExtendAttributes[attNode.GetAttribute("name")] = attNode.InnerText;
            }
        }*/

        public static MindMap ImportXmlFile(string filename)
        {
            if(string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            if (!File.Exists(filename))
                throw new FileNotFoundException(null, filename);

            MindMap map = new MindMap();

            XmlDocument dom = new XmlDocument();
            dom.Load(filename);

            Topic topic = ImportXmlFile(dom.DocumentElement);
            map.Root = topic;

            return map;
        }

        static Topic ImportXmlFile(XmlElement element)
        {
            Topic topic = new Topic(element.Name);

            foreach (XmlNode subNode in element.ChildNodes)
            {
                if (subNode is XmlElement)
                {
                    topic.Children.Add(ImportXmlFile((XmlElement)subNode));
                }
            }

            return topic;
        }

        /*public static void Save(MindMap map, string filename)
        {
            if (map == null)
                throw new ArgumentNullException("Map");

            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("Filename");

            if (SaveAsXml(map, filename))
            {
                map.Filename = filename;
                map.Name = Path.GetFileNameWithoutExtension(map.Filename);
            }
        }

        static bool SaveAsXml(MindMap map, string filename)
        {
            XmlDocument dom = new XmlDocument();
            dom.LoadXml("<?xml version='1.0' encoding='utf-8' ?><map/>");

            XmlElement document = dom.DocumentElement;
            document.SetAttribute("name", map.Name);
            document.SetAttribute("document_type", DocumentType.Blumind.Name);
            document.SetAttribute("editor_version", ProductInfo.Version);

            // comment
            XmlComment comment = dom.CreateComment("Create by Blumind, you can download it free from http://www.blumind.org ");
            document.AppendChild(comment);

            // info
            XmlElement info = dom.CreateElement("info");
            SaveInfomation(map, info);
            document.AppendChild(info);

            // extend attributes
            if (map.ExtendAttributes.Count > 0)
            {
                XmlElement attributesNode = dom.CreateElement("attributes");
                SaveExtendAttributes(map, attributesNode);
                document.AppendChild(attributesNode);
            }

            SaveAsXml(map, document);

            dom.Save(filename);
            return true;
        }

        public static void SaveAsXml(MindMap map, XmlElement docElement)
        {
            Link[] lines = map.GetLinks(false);
            foreach (Link line in lines)
            {
                if (line.Target != null && string.IsNullOrEmpty(line.Target.ID))
                    line.Target.ID = Topic.NewID();
            }

            XmlDocument dom = docElement.OwnerDocument;

            // layout
            ST.WriteTextNode(docElement, "layout", ST.ToString(map.LayoutType));

            // desc
            if (!string.IsNullOrEmpty(map.Description))
                ST.WriteCDataNode(docElement, "description", map.Description);

            // style
            var styleNode = XmlSerializer.SerializeMapStyle(dom, map);
            if (styleNode.Attributes.Count > 0 || styleNode.ChildNodes.Count > 0)
                docElement.AppendChild(styleNode);

            // topics
            XmlElement topics = dom.CreateElement("nodes");
            topics.AppendChild(Topic.SerializeTopic(dom, map.Root));
            docElement.AppendChild(topics);
        }

        static void SaveInfomation(MindMap map, XmlElement info)
        {
            if (map == null || info == null)
                return;

            ST.WriteTextNode(info, "author", map.Author);
            ST.WriteTextNode(info, "company", map.Company);
            ST.WriteTextNode(info, "version", map.Version);
        }

        static void SaveExtendAttributes(MindMap map, XmlElement node)
        {
            if (map == null || node == null)
                return;

            foreach (string key in map.ExtendAttributes.Keys)
            {
                XmlElement attNode = node.OwnerDocument.CreateElement("item");
                attNode.SetAttribute("name", key);
                attNode.InnerText = map.ExtendAttributes[key];
                node.AppendChild(attNode);
            }
        }*/
    }
}
