using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;
using Blumind.Controls.Paint;
using Blumind.Model;
using Blumind.Model.Widgets;

namespace Blumind.Core.Documents
{
    /*class XmlSerializer
    {
        public static XmlElement SerializeTopic(Topic topic, XmlDocument dom)
        {
            XmlElement node = dom.CreateElement("node");
            topic.Serialize(dom, node);
            //if (!string.IsNullOrEmpty(topic.Description))
            //    ST.WriteTextNode(topicNode, "description", topic.Description);
            if (!string.IsNullOrEmpty(topic.Hyperlink))
                node.SetAttribute("hyperlink", topic.Hyperlink);
            if (topic.Folded)
                node.SetAttribute("folded", topic.Folded.ToString());

            if (topic.CustomWidth.HasValue)
                node.SetAttribute("custom_width", topic.CustomWidth.Value.ToString());
            if (topic.CustomHeight.HasValue)
                node.SetAttribute("custom_height", topic.CustomHeight.Value.ToString());

            var styleNode = SerializeStyle(topic.Style, dom);
            if (styleNode.Attributes.Count > 0 || styleNode.ChildNodes.Count > 0)
            {
                node.AppendChild(styleNode);
            }

            //parentNode.AppendChild(topicNode);

            // links
            if (!topic.Links.IsEmpty)
            {
                XmlElement links = dom.CreateElement("links");
                foreach (Link link in topic.Links)
                {
                    if (link.Target == null || link.Target == link.From)
                        continue;

                    XmlElement lineNode = SerializeLink(link, dom);
                    links.AppendChild(lineNode);
                }

                if(links.ChildNodes.Count != 0)
                    node.AppendChild(links);
            }

            // widgets
            if (!topic.Widgets.IsEmpty)
            {
                XmlElement widgets = dom.CreateElement("widgets");
                foreach (Widget widget in topic.Widgets)
                {
                    XmlElement widgetNode = dom.CreateElement("widget");
                    widgetNode.SetAttribute("type", widget.GetTypeID());
                    if (!widget.Serialize(widgetNode))
                        continue;
                    widgets.AppendChild(widgetNode);
                }

                if (widgets.ChildNodes.Count != 0)
                    node.AppendChild(widgets);
            }

            // sub-nodes
            if (!topic.Children.IsEmpty)
            {
                XmlElement nodes = dom.CreateElement("nodes");
                node.AppendChild(nodes);
                foreach (Topic subTopic in topic.Children)
                {
                    XmlElement subNode = SerializeTopic(subTopic, dom);
                    if (subNode != null)
                    {
                        nodes.AppendChild(subNode);
                    }
                }
            }

            return node;
        }

        public static Topic DeserializeTopic(XmlElement node)
        {
            Topic topic = new Topic();
            topic.Deserialize(node);
            topic.Hyperlink = node.GetAttribute("hyperlink");

            topic.Folded = ST.GetBoolDefault(node.GetAttribute("folded"));

            //
            if (node.HasAttribute("x")) // 新
            {
                topic.CustomWidth = ST.GetInt(node.GetAttribute("custom_width"));
                topic.CustomHeight = ST.GetInt(node.GetAttribute("custom_height"));
            }
            else
            {
                topic.CustomWidth = ST.GetInt(node.GetAttribute("width"));
                topic.CustomHeight = ST.GetInt(node.GetAttribute("height"));
            }

            XmlElement styleNode = node.SelectSingleNode("style") as XmlElement;
            if (styleNode != null)
            {
                DeserializeStyle(styleNode, topic.Style);
            }

            //XmlElement iconNode = node.SelectSingleNode("icon") as XmlElement;
            //if (iconNode != null)
            //{
            //    topic.SetIcon(ST.ReadImageNode(iconNode));
            //}

            //
            XmlNodeList linkNodes = node.SelectNodes("links/link");
            foreach (XmlElement linkNode in linkNodes)
            {
                Link link = DeserializeLink(linkNode);
                if(link != null)
                    topic.Links.Add(link);
            }

            //
            XmlNodeList widgetNodes = node.SelectNodes("widgets/widget");
            foreach (XmlElement widgetNode in widgetNodes)
            {
                Widget widget = DeserializeWidget(widgetNode);
                if (widget != null)
                    topic.Widgets.Add(widget);
            }

            //
            XmlNodeList nodes = node.SelectNodes("nodes/node");
            foreach (XmlElement subNode in nodes)
            {
                Topic subTopic = DeserializeTopic(subNode);
                if (subTopic != null)
                {
                    topic.Children.Add(subTopic);
                }
            }

            return topic;
        }

        static XmlElement SerializeStyle(TopicStyle style, XmlDocument dom)
        {
            if (style == null)
            {
                throw new ArgumentNullException();
            }

            XmlElement node = dom.CreateElement("style");
            SerializeColor(node, "back_color", style.BackColor);
            SerializeColor(node, "fore_color", style.ForeColor);
            SerializeColor(node, "line_color", style.LineColor);
            SerializeColor(node, "border_color", style.BorderColor);
            if(style.Shape != TopicShape.Default)
                ST.WriteTextNode(node, "shape", ((int)style.Shape).ToString());

            if (style.RoundRadius != TopicStyle.DefaultRoundRadius)
                ST.WriteTextNode(node, "round_radius", style.RoundRadius.ToString());

            if (style.Padding.All != TopicStyle.DefaultNodePadding)
                ST.WriteTextNode(node, "padding", ST.ToString(style.Padding));

            if (style.Font != null)
            {
                XmlElement fontNode = dom.CreateElement("font");
                ST.WriteFontNode(fontNode, style.Font);
                node.AppendChild(fontNode);
            }

            return node;
        }

        private static void DeserializeStyle(XmlElement node, TopicStyle style)
        {
            if (node == null || style == null)
                throw new ArgumentNullException();

            style.BackColor = DeserializeColor(node, "back_color", style.BackColor);
            style.ForeColor = DeserializeColor(node, "fore_color", style.ForeColor);
            style.LineColor = DeserializeColor(node, "line_color", style.LineColor);
            style.BorderColor = DeserializeColor(node, "border_color", style.BorderColor);
            style.Shape = ST.GetTopicShape(ST.ReadTextNode(node, "shape"), TopicShape.Default);
            style.RoundRadius = ST.GetInt(ST.ReadTextNode(node, "round_radius"), TopicStyle.DefaultRoundRadius);

            Padding? padding = ST.GetPadding(ST.ReadTextNode(node, "padding"));
            if (padding.HasValue)
            {
                style.Padding = padding.Value;
            }
            else
            {
                style.Padding = new Padding(TopicStyle.DefaultNodePadding);
            }

            XmlElement fontNode = node.SelectSingleNode("font") as XmlElement;
            if (fontNode != null)
            {
                style.Font = ST.ReadFontNode(fontNode);
            }
        }

        public static XmlElement SerializeMapStyle(XmlDocument dom, MindMap mindmap)
        {
            XmlElement node = dom.CreateElement("style");

            SerializeMapStyle(node, mindmap);

            return node;
        }

        public static void SerializeMapStyle(XmlElement node, MindMap mindmap)
        {
            SerializeColor(node, "back_color", mindmap.BackColor);
            SerializeColor(node, "fore_color", mindmap.ForeColor);
            SerializeColor(node, "line_color", mindmap.LineColor);
            SerializeColor(node, "border_color", mindmap.BorderColor);
            SerializeColor(node, "node_back_color", mindmap.NodeBackColor);
            SerializeColor(node, "node_fore_color", mindmap.NodeForeColor);
            SerializeColor(node, "select_color", mindmap.SelectColor);
            SerializeColor(node, "hover_color", mindmap.HoverColor);
            SerializeColor(node, "link_line_color", mindmap.LinkLineColor);

            ST.WriteTextNode(node, "widget_margin", mindmap.WidgetMargin.ToString());
            ST.WriteTextNode(node, "picture_thumb_size", ST.ToString(mindmap.PictureThumbSize));

            if (mindmap.LayerSpace != MindMapStyle.DefaultLayerSpace)
                ST.WriteTextNode(node, "layer_space", mindmap.LayerSpace.ToString());

            if (mindmap.ItemsSpace != MindMapStyle.DefaultItemsSpace)
                ST.WriteTextNode(node, "items_space", mindmap.ItemsSpace.ToString());

            if (mindmap.Font != null)
            {
                XmlElement font = node.OwnerDocument.CreateElement("font");
                ST.WriteFontNode(font, mindmap.Font);
                node.AppendChild(font);
            }
        }

        public static void DeserializeMapStyle(XmlElement node, MindMap mindmap)
        {
            mindmap.BackColor = DeserializeColor(node, "back_color", mindmap.BackColor);
            mindmap.ForeColor = DeserializeColor(node, "fore_color", mindmap.ForeColor);
            mindmap.LineColor = DeserializeColor(node, "line_color", mindmap.LineColor);
            mindmap.BorderColor = DeserializeColor(node, "border_color", mindmap.BorderColor);
            mindmap.NodeBackColor = DeserializeColor(node, "node_back_color", mindmap.NodeBackColor);
            mindmap.NodeForeColor = DeserializeColor(node, "node_fore_color", mindmap.NodeForeColor);
            mindmap.SelectColor = DeserializeColor(node, "select_color", mindmap.SelectColor);
            mindmap.HoverColor = DeserializeColor(node, "hover_color", mindmap.HoverColor);
            mindmap.LinkLineColor = DeserializeColor(node, "link_line_color", mindmap.LinkLineColor);
            mindmap.LayerSpace = ST.GetInt(ST.ReadTextNode(node, "layer_space"), MindMapStyle.DefaultLayerSpace);
            mindmap.ItemsSpace = ST.GetInt(ST.ReadTextNode(node, "items_space"), MindMapStyle.DefaultItemsSpace);
            mindmap.WidgetMargin = ST.GetInt(ST.ReadTextNode(node, "widget_margin"), mindmap.WidgetMargin);
            mindmap.PictureThumbSize = ST.GetSize(ST.ReadTextNode(node, "picture_thumb_size"), mindmap.PictureThumbSize);

            var fontNode = node.SelectSingleNode("font") as XmlElement;
            if (fontNode != null)
            {
                mindmap.Font = ST.ReadFontNode(fontNode);
            }
        }

        private static void SerializeColor(XmlElement node, string name, Color color)
        {
            if (!color.IsEmpty)
            {
                ST.WriteTextNode(node, name, ST.ToString(color));
            }
        }

        private static Color DeserializeColor(XmlElement node, string name, Color colorDefault)
        {
            return ST.GetColor(ST.ReadTextNode(node, name), colorDefault);
        }

        public static void SerializeTheme(XmlElement node, ChartTheme theme)
        {
            if (theme == null || node == null)
                throw new ArgumentNullException();

            //SerializeMapStyle(theme, node);

            node.SetAttribute("name", theme.Name);
            SerializeColor(node, "root_back_color", theme.RootBackColor);
            SerializeColor(node, "root_fore_color", theme.RootForeColor);
            SerializeColor(node, "root_border_color", theme.RootBorderColor);
            SerializeColor(node, "back_color", theme.BackColor);
            SerializeColor(node, "fore_color", theme.ForeColor);
            SerializeColor(node, "line_color", theme.LineColor);
            SerializeColor(node, "border_color", theme.BorderColor);
            SerializeColor(node, "node_back_color", theme.NodeBackColor);
            SerializeColor(node, "node_fore_color", theme.NodeForeColor);
            SerializeColor(node, "select_color", theme.SelectColor);
            SerializeColor(node, "hover_color", theme.HoverColor);
            SerializeColor(node, "link_line_color", theme.LinkLineColor);
            
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

        public static ChartTheme DeserializeTheme(XmlElement node)
        {
            var theme = new ChartTheme();
            //DeserializeMapStyle(node, theme);
            theme.BackColor = DeserializeColor(node, "back_color", theme.BackColor);
            theme.ForeColor = DeserializeColor(node, "fore_color", theme.ForeColor);
            theme.LineColor = DeserializeColor(node, "line_color", theme.LineColor);
            theme.BorderColor = DeserializeColor(node, "border_color", theme.BorderColor);
            theme.NodeBackColor = DeserializeColor(node, "node_back_color", theme.NodeBackColor);
            theme.NodeForeColor = DeserializeColor(node, "node_fore_color", theme.NodeForeColor);
            theme.SelectColor = DeserializeColor(node, "select_color", theme.SelectColor);
            theme.HoverColor = DeserializeColor(node, "hover_color", theme.HoverColor);
            theme.LinkLineColor = DeserializeColor(node, "link_line_color", theme.LinkLineColor);
            theme.LayerSpace = ST.GetInt(ST.ReadTextNode(node, "layer_space"), MindMapStyle.DefaultLayerSpace);
            theme.ItemsSpace = ST.GetInt(ST.ReadTextNode(node, "items_space"), MindMapStyle.DefaultItemsSpace);

            XmlElement fontNode = node.SelectSingleNode("font") as XmlElement;
            if (fontNode != null)
            {
                theme.Font = ST.ReadFontNode(fontNode);
            }

            theme.Name = node.GetAttribute("name");
            theme.RootBackColor = DeserializeColor(node, "root_back_color", theme.RootBackColor);
            theme.RootForeColor = DeserializeColor(node, "root_fore_color", theme.RootForeColor);
            theme.RootBorderColor = DeserializeColor(node, "root_border_color", theme.RootBorderColor);
            theme.Description = ST.ReadCDataNode(node, "description");
            if (theme.Description == string.Empty)
                theme.Description = ST.ReadTextNode(node, "description");


            return theme;
        }

        public static XmlElement SerializeLink(Link link, XmlDocument dom)
        {
            XmlElement node = dom.CreateElement("link");
            //node.SetAttribute("from", line.From.ID);
            node.SetAttribute("target", link.Target.ID);
            node.SetAttribute("cp1_length", link.LayoutData.CP1.Length.ToString());
            node.SetAttribute("cp1_angle", link.LayoutData.CP1.Angle.ToString());
            node.SetAttribute("cp2_length", link.LayoutData.CP2.Length.ToString());
            node.SetAttribute("cp2_angle", link.LayoutData.CP2.Angle.ToString());
            node.SetAttribute("width", link.LineWidth.ToString());
            if(!link.Color.IsEmpty)
                node.SetAttribute("color", ST.ToString(link.Color));
            node.SetAttribute("line_style", ((int)link.LineStyle).ToString());
            node.SetAttribute("start_cap", ((int)link.StartCap).ToString());
            node.SetAttribute("end_cap", ((int)link.EndCap).ToString());
            node.SetAttribute("text", link.Text);
            node.SetAttribute("hyperlink", link.Hyperlink);

            if (!string.IsNullOrEmpty(link.Remark))
            {
                ST.WriteTextNode(node, "remark", link.Remark);
            }

            return node;
        }

        public static Link DeserializeLink(XmlElement node)
        {
            if (node == null)
                throw new ArgumentNullException();

            Link link = new Link();
            //line.FromID = node.GetAttribute("from");
            link.TargetID = node.GetAttribute("target");
            //link.LayoutData.CPLength1 = ST.GetInt(node.GetAttribute("cp1_length"), 0);
            //link.LayoutData.CPAngle1 = ST.GetInt(node.GetAttribute("cp1_angle"), 0);
            link.LayoutData.CP1 = new BezierControlPoint(
                ST.GetInt(node.GetAttribute("cp1_angle"), 0), 
                ST.GetInt(node.GetAttribute("cp1_length"), 0));
            //link.LayoutData.CPLength2 = ST.GetInt(node.GetAttribute("cp2_length"), 0);
            //link.LayoutData.CPAngle2 = ST.GetInt(node.GetAttribute("cp2_angle"), 0);
            link.LayoutData.CP2 = new BezierControlPoint(
                ST.GetInt(node.GetAttribute("cp2_angle"), 0),
                ST.GetInt(node.GetAttribute("cp2_length"), 0));
            link.Color = ST.GetColor(node.GetAttribute("color"), link.Color);
            link.LineWidth = ST.GetInt(node.GetAttribute("width"), link.LineWidth);
            link.LineStyle = (DashStyle)ST.GetInt(node.GetAttribute("line_style"), (int)link.LineStyle);
            link.StartCap = (LineCap)ST.GetInt(node.GetAttribute("start_cap"), (int)link.StartCap);
            link.EndCap = (LineCap)ST.GetInt(node.GetAttribute("end_cap"), (int)link.EndCap);
            link.Text = node.GetAttribute("text");
            link.Hyperlink = node.GetAttribute("hyperlink");
            link.Remark = ST.ReadTextNode(node, "remark");

            // 向后兼容
            if (string.IsNullOrEmpty(link.Remark))
            {
                link.Remark = ST.ReadTextNode(node, "description");
            }

            return link;
        }

        private static Widget DeserializeWidget(XmlElement node)
        {
            if (node == null)
                throw new ArgumentNullException();

            string typeId = node.GetAttribute("type");
            Widget widget = Widget.Create(typeId);
            if (widget != null && widget.Deserialize(node))
                return widget;
            else
                return null;
        }
    }*/
}
