using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Blumind.Canvas;
using Blumind.Controls.Paint;
using Blumind.Core;
using Blumind.Model;
using Blumind.Model.Documents;

namespace Blumind.Model.MindMaps
{
    class FreeMindFile
    {
        public static Document LoadFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            if (!File.Exists(filename))
                throw new FileNotFoundException(null, filename);

            XmlDocument dom = new XmlDocument();
            dom.Load(filename);

            MindMap map = LoadMindMap(dom);

            if (map != null)
            {
                //map.Filename = filename;
                //if (string.IsNullOrEmpty(map.Name))
                //{
                map.Name = Path.GetFileNameWithoutExtension(filename);
                //}
            }

            Document doc = new Document();
            doc.Name = map.Name;
            doc.Charts.Add(map);
            return doc;
        }

        static MindMap LoadMindMap(XmlDocument dom)
        {
            if (dom.DocumentElement.Name != "map")
                throw new ArgumentException();

            MindMap map = new MindMap();

            //
            XmlElement rootNode = dom.DocumentElement.SelectSingleNode("node") as XmlElement;
            if (rootNode != null)
            {
                Topic topic = LoadNodes(rootNode);
                map.Root = topic;
            }

            return map;
        }

        static Topic LoadNodes(XmlElement xmlNode)
        {
            if (xmlNode == null || xmlNode.Name != "node")
                return null;

            // topic
            Topic topic = new Topic();
            topic.Text = xmlNode.GetAttribute("TEXT");
            topic.ID = xmlNode.GetAttribute("ID");
            topic.BackColor = ST.GetColor(xmlNode.GetAttribute("BACKGROUND_COLOR"), topic.BackColor);
            topic.ForeColor = ST.GetColor(xmlNode.GetAttribute("COLOR"), topic.BackColor);
            topic.Folded = ST.GetBool(xmlNode.GetAttribute("FOLDED"), topic.Folded);
            topic.Padding = new Padding(ST.GetInt(xmlNode.GetAttribute("HGAP"), 0),
                ST.GetInt(xmlNode.GetAttribute("VGAP"), 0),
                ST.GetInt(xmlNode.GetAttribute("HGAP"), 0),
                ST.GetInt(xmlNode.GetAttribute("VGAP"), 0));

            // font
            XmlElement node = xmlNode.SelectSingleNode("font") as XmlElement;
            if (node != null)
            {
                FontStyle fs = FontStyle.Regular;
                if(ST.GetBoolDefault(node.GetAttribute("BOLD")))
                    fs |= FontStyle.Bold;
                if( ST.GetBoolDefault(node.GetAttribute("ITALIC")))
                    fs |= FontStyle.Italic;
                topic.Font = new Font(node.GetAttribute("NAME"), ST.GetFloat(node.GetAttribute("SIZE"), 0), fs);
            }

            // note
            XmlElement noteNode = xmlNode.SelectSingleNode("richcontent[@TYPE='NOTE']") as XmlElement;
            if (noteNode != null)
            {
                topic.Remark = DeserializeRemark(noteNode);
            }

            // arrowlink
            XmlNodeList links = xmlNode.SelectNodes("arrowlink");
            foreach (XmlElement linkNode in links)
            {
                Link link = DeserializeLink(linkNode);
                topic.Links.Add(link);
            }

            // children
            XmlNodeList list = xmlNode.SelectNodes("node");
            foreach (XmlElement element in list)
            {
                Topic subTopic = LoadNodes(element);
                if (subTopic != null)
                    topic.Children.Add(subTopic);
            }

            return topic;
        }

        static string DeserializeRemark(XmlElement node)
        {
            if (node == null)
                return null;

            var html = node.InnerText;
            return ST.HtmlToText(html);
            /*var bodyIndex = html.IndexOf("<body>", StringComparison.OrdinalIgnoreCase);
            if (bodyIndex > -1)
            {
                var bodyIndex2 = html.LastIndexOf("</body>", StringComparison.OrdinalIgnoreCase);
                if (bodyIndex2 > -1)
                    return html.Substring(bodyIndex, bodyIndex2 - bodyIndex).Trim();
                else
                    return html.Substring(bodyIndex, html.Length - bodyIndex).Trim();
            }
            else
            {
                return html.Trim();
            }*/
        }

        static Link DeserializeLink(XmlElement node)
        {
            if (node == null)
                throw new ArgumentNullException();

            Link link = new Link();
            link.ID = node.GetAttribute("ID");
            //line.FromID = node.GetAttribute("from");
            link.TargetID = node.GetAttribute("DESTINATION");
            link.Color = ST.GetColor(node.GetAttribute("COLOR"), Color.Black);
            link.LineStyle = DashStyle.Solid;
            link.LineWidth = 1;

            int angle, length;
            ParseInclination(node.GetAttribute("STARTINCLINATION"), out angle, out length);
            link.LayoutData.CP1 = new BezierControlPoint(angle, length);
            ParseInclination(node.GetAttribute("ENDINCLINATION"), out angle, out length);
            link.LayoutData.CP2 = new BezierControlPoint(angle, length);

            link.StartCap = ParseLineCap(node.GetAttribute("STARTARROW"));
            link.EndCap = ParseLineCap(node.GetAttribute("ENDARROW"));

            //link.Layout.CPLength1 = ST.GetInteger(node.GetAttribute("cp1_length"), 0);
            //link.Layout.CPAngle1 = ST.GetInteger(node.GetAttribute("cp1_angle"), 0);
            //link.Layout.CPLength2 = ST.GetInteger(node.GetAttribute("cp2_length"), 0);
            //link.Layout.CPAngle2 = ST.GetInteger(node.GetAttribute("cp2_angle"), 0);
            //link.Width = ST.GetInteger(node.GetAttribute("width"), link.Width);
            //link.LineStyle = (DashStyle)ST.GetInteger(node.GetAttribute("line_style"), (int)link.LineStyle);
            return link;
        }

        static LineAnchor ParseLineCap(string cap)
        {
            switch (cap)
            {
                case "Default":
                    return LineAnchor.Arrow;
                default:
                    return LineAnchor.None;
            }
        }

        static void ParseInclination(string inclination, out int angle, out int length)
        {
            string[] strs = inclination.Split(';');
            if (strs.Length >= 2)
            {
                angle = ST.GetInt(strs[0], 0);
                length = ST.GetInt(strs[1], 0);
            }
            else
            {
                angle = 0;
                length = 0;
            }
        }

        public static void SaveFile(MindMap mindMap, string filename)
        {
            XmlDocument dom = new XmlDocument();

            XmlElement root = dom.CreateElement("map");
            root.SetAttribute("version", "0.9.0");
            root.AppendChild(dom.CreateComment(@"Export from the Blumind, download free mind mapping software Blumind from http://www.blumind.org"));
            root.AppendChild(dom.CreateComment(@"To view this file, download free mind mapping software FreeMind from http://freemind.sourceforge.net"));
            dom.AppendChild(root);
            SaveMindMap(root, mindMap.Root, mindMap);

            dom.Save(filename);
        }

        static void SaveMindMap(XmlElement parent, Topic topic, MindMap mindMap)
        {
            if (parent == null || topic == null)
                return;

            XmlElement node = parent.OwnerDocument.CreateElement("node");
            if(!topic.BackColor.IsEmpty)
                node.SetAttribute("BACKGROUND_COLOR", ST.ToWebColor(topic.BackColor));
            else if(!mindMap.NodeBackColor.IsEmpty)
                node.SetAttribute("BACKGROUND_COLOR", ST.ToWebColor(mindMap.NodeBackColor));

            if (!topic.ForeColor.IsEmpty)
                node.SetAttribute("COLOR", ST.ToWebColor(topic.ForeColor));
            else if(!mindMap.NodeForeColor.IsEmpty)
                node.SetAttribute("COLOR", ST.ToWebColor(mindMap.NodeForeColor));

            if (topic.Folded)
                node.SetAttribute("FOLDED", "true");

            if (!string.IsNullOrEmpty(topic.ID))
                node.SetAttribute("ID", topic.ID);
            
            if (topic.Vector == Blumind.Controls.Vector4.Left)
                node.SetAttribute("POSITION", "left");
            else if (topic.Vector == Blumind.Controls.Vector4.Right)
                node.SetAttribute("POSITION", "right");

            node.SetAttribute("TEXT", topic.Text);

            node.SetAttribute("HGAP", topic.Padding.Left.ToString());
            node.SetAttribute("VGAP", topic.Padding.Right.ToString());

            if (!string.IsNullOrEmpty(topic.Remark))
            {
                SerializeRemark(node, topic.Remark);
            }

            // Links
            if (topic.Links.Count > 0)
            {
                foreach (Link link in topic.Links)
                {
                    SaveLink(node, link, mindMap);
                }
            }

            // Hyperlink
            if (!string.IsNullOrEmpty(topic.Hyperlink))
            {
                node.SetAttribute("LINK", topic.Hyperlink);
            }

            //
            parent.AppendChild(node);

            // children
            if (topic.Children.Count > 0)
            {
                foreach (Topic subTopic in topic.Children)
                {
                    SaveMindMap(node, subTopic, mindMap);
                }
            }
        }

        static void SerializeRemark(XmlElement node, string remark)
        {
            if (node == null || string.IsNullOrEmpty(remark))
                return;

            var rc = node.OwnerDocument.CreateElement("richcontent");
            rc.SetAttribute("TYPE", "NOTE");
            //rc.InnerXml = "<html><head></head><body>" + remark + "</body></html>";
            rc.InnerXml = ST.HtmlToText(remark);
            node.AppendChild(rc);
        }

        static void SaveLink(XmlElement parent, Link link, MindMap mindMap)
        {
            if (parent == null || link == null || string.IsNullOrEmpty(link.TargetID))
                return;

            XmlElement node = parent.OwnerDocument.CreateElement("arrowlink");
            if(!link.Color.IsEmpty)
                node.SetAttribute("COLOR", ST.ToWebColor(link.Color));
            else if(!mindMap.LinkLineColor.IsEmpty)
                node.SetAttribute("COLOR", ST.ToWebColor(mindMap.LinkLineColor));

            node.SetAttribute("DESTINATION", link.TargetID);
            if (link.EndCap != LineAnchor.None)
                node.SetAttribute("ENDARROW", "Default");
            else
                node.SetAttribute("ENDARROW", "None");

            node.SetAttribute("ENDINCLINATION", 
                string.Format("{0};{1};", link.LayoutData.ControlPoint2.X-link.LayoutData.EndPoint.X, link.LayoutData.ControlPoint2.Y - link.LayoutData.EndPoint.Y));

            if (link.StartCap != LineAnchor.None)
                node.SetAttribute("STARTARROW", "Default");
            else
                node.SetAttribute("STARTARROW", "None");

            node.SetAttribute("STARTINCLINATION",
                string.Format("{0};{1};", link.LayoutData.ControlPoint1.X - link.LayoutData.StartPoint.X, link.LayoutData.ControlPoint1.Y - link.LayoutData.StartPoint.Y));

            parent.AppendChild(node);
        }
    }
}
