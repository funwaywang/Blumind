using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Xml;
using Blumind.Canvas.Svg;
using Blumind.Configuration;
using Blumind.Controls;
using Blumind.Controls.MapViews;
using Blumind.Model;
using Blumind.Model.Documents;
using Blumind.Model.MindMaps;
using Blumind.Model.Widgets;

namespace Blumind.Core.Exports
{
    class SvgEngine : ChartsExportEngine
    {
        public override string TypeMime
        {
            get { return "image/svg+xml"; }
        }

        public string Version
        {
            get { return "1.1"; }
        }

        /*public override bool Export(MindMapView view, string filename)
        {
            XmlDocument dom = Export(null, view.Map);

            dom.Save(filename);
            return true;
        }*/

        protected override bool ExportChartToFile(Document document, ChartPage chart, string filename)
        {
            if (chart is MindMap)
            {
                /*var dom = Export(document, (MindMap)chart);*/

                Size size = chart.GetContentSize();
                var dom = new SvgDocument(size.Width, size.Height);
                dom.Title = chart.Name;

                //
                if (!chart.BackColor.IsEmpty)
                    dom.BackColor = chart.BackColor;

                //
                var graphics = new SvgGraphics(dom);

                //
                var args = new RenderArgs(graphics, (MindMap)chart, graphics.Font(ChartBox.DefaultChartFont));
                var renderer = new GeneralRender();
                renderer.Paint((MindMap)chart, args);

                //
                dom.Save(filename);
                return true;
            }

            return base.ExportChartToFile(document, chart, filename);
        }

        /*SvgDocument Export(Document document, MindMap map)
        {
            map.EnsureChartLayouted();

            Size size = map.GetContentSize();
            Font defaultFont = map.Font != null ? map.Font : ChartBox.DefaultChartFont;
            Layouter layouter = LayoutManage.GetLayouter(map.LayoutType);
            ILine liner = LayoutManage.GetLiner(map.LayoutType);

            var dom = new SvgDocument(size.Width, size.Height);
            dom.Title = map.Name;
            var svg = dom.DocumentElement;

            // fold buttons
            ExportFoldButtons(dom.Defines);

            // style
            XmlElement style = dom.CreateElement("style");
            ExportStyle(style, map);
            dom.Defines.AppendChild(style);

            // the "root" group
            XmlElement root = dom.CreateElement("g");
            root.SetAttribute("id", "root");
            Font font = map.Font == null ? defaultFont : map.Font;
            SetFontInfo(root, font);
            //if (!map.Style.LineColor.IsEmpty)
            //    root.SetAttribute("stroke", ST.ToString(map.Style.LineColor));

            // desc
            //if (!string.IsNullOrEmpty(map.Description) )
            //{
            //    XmlElement desc = dom.CreateElement("desc");
            //    desc.InnerText = map.Description;
            //    root.AppendChild(desc);
            //}

            svg.AppendChild(root);

            if (map.Root != null)
            {
                ExportTopic(map, map.Root, root, defaultFont, layouter, liner);

                XmlElement links = dom.CreateElement("g");
                links.SetAttribute("id", "links");
                ExportLinks(map, map.Root, links, defaultFont, layouter, liner);
                if (links.ChildNodes.Count > 0)
                    svg.AppendChild(links);
            }

            return dom;
        }

        void ExportTopic(MindMap map, Topic topic, XmlElement parentNode, Font font, Layouter layouter, ILine liner)
        {
            TopicStyle style = topic.Style;

            // g
            XmlElement g = parentNode.OwnerDocument.CreateElement("g");
            if (topic.Font != null)
                SetFontInfo(g, topic.Font);
            parentNode.AppendChild(g);

            // background
            XmlElement node = null;
            Shape shaper = Shape.GetShaper(topic.Style.Shape, topic.Style.RoundRadius);
            if (shaper != null)
            {
                node = shaper.GenerateSvg(topic.Bounds, g,
                    topic.Style.BorderColor.IsEmpty ? map.BorderColor : topic.Style.BorderColor,
                    topic.Style.BackColor.IsEmpty ? map.NodeBackColor : topic.Style.BackColor);
            }

            Rectangle rect = topic.Bounds;
            rect.X += style.Padding.Left;
            rect.Y += style.Padding.Top;
            rect.Width -= style.Padding.Horizontal;
            rect.Height -= style.Padding.Vertical;

            // icon
            //if (topic.Icon != null)
            //PictureWidget[] icons = topic.FindWidgets<PictureWidget>();
            //if(icons.Length > 0)
            //{
            //    ExportIcon(g, topic);
            //    rect.X += style.IconPadding * 2 + icons[0].Data.Width;
            //    rect.Width -= style.IconPadding * 2 + icons[0].Data.Width;
            //}

            // desc
            //if (!string.IsNullOrEmpty(topic.Description) && node != null)
            //{
            //    XmlElement desc = g.OwnerDocument.CreateElement("desc");
            //    desc.InnerText = topic.Description;
            //    node.AppendChild(desc);
            //}

            // widgets
            foreach(Widget widget in topic.Widgets)
            {
                widget.GeneralSvg(g, topic.Location, font);
            }

            // lines
            if (topic.Lines.Count > 0)
            {
                foreach (TopicLine line in topic.Lines)
                {
                    var rectFrom = line.BeginRectangle;// topic.Bounds;
                    var rectTo = line.EndRectangle;// line.Target.Bounds;
                    //if (layouter != null)
                    //    layouter.AdjustLineRect(line, topic, ref rectFrom, ref rectTo);

                    Color color = topic.Style.LineColor.IsEmpty ? map.LineColor : topic.Style.LineColor;
                    liner.GeneralSvg(g, rectFrom, rectTo, line.BeginSide, line.EndSide, color);
                }
            }

            // text
            if (!string.IsNullOrEmpty(topic.Text))
            {
                Rectangle rectText = topic.TextBounds;
                rectText.Offset(topic.Location);

                ExportText(topic.Text, 
                    topic.ForeColor.IsEmpty ? map.NodeForeColor : topic.ForeColor,
                    topic.Font != null ? topic.Font : font, 
                    g, rectText);
            }

            // childrens
            if (!topic.Children.IsEmpty && !topic.Folded)
            {
                foreach (Topic subTopic in topic.Children)
                {
                    ExportTopic(map, subTopic, parentNode, font, layouter, liner);
                }
            }

            // fold button
            if (!topic.IsRoot && !topic.Children.IsEmpty)
            {
                XmlElement use = g.OwnerDocument.CreateElement("use");
                use.SetAttribute("x", topic.FoldingButton.Left.ToString());
                use.SetAttribute("y", topic.FoldingButton.Top.ToString());
                XmlAttribute href = g.OwnerDocument.CreateAttribute("xlink", "href", "http://www.w3.org/1999/xlink");
                href.Value = topic.Folded ? "#btn_plus" : "#btn_minus";
                use.Attributes.Append(href);
                g.AppendChild(use);
            }
        }

        void ExportLinks(MindMap map, Topic topic, XmlElement root, Font defaultFont, Layouter layouter, ILine liner)
        {
            // links
            foreach (Link link in topic.Links)
            {
                if (!link.Visible)
                    continue;

                var layout = link.LayoutData;
                var sb = new StringBuilder();
                sb.AppendFormat("M {0},{1} ", layout.StartPoint.X, layout.StartPoint.Y);
                sb.AppendFormat("C {0},{1} {2},{3} {4},{5}", layout.ControlPoint1.X, layout.ControlPoint1.Y,
                    layout.ControlPoint2.X, layout.ControlPoint2.Y, 
                    layout.EndPoint.X, layout.EndPoint.Y);

                string dasharray = null;
                switch (link.LineStyle)
                {
                    case DashStyle.Dot:
                        dasharray = link.LineWidth.ToString();
                        break;
                    case DashStyle.Dash:
                        dasharray = string.Format("{0},{1}", link.LineWidth * 3, link.LineWidth);
                        break;
                    case DashStyle.DashDot:
                        dasharray = string.Format("{0},{1},{1},{1}", link.LineWidth * 3, link.LineWidth);
                        break;
                    case DashStyle.DashDotDot:
                        dasharray = string.Format("{0},{1},{1},{1},{1},{1}", link.LineWidth * 3, link.LineWidth);
                        break;
                }

                XmlElement node = root.OwnerDocument.CreateElement("path");
                node.SetAttribute("d", sb.ToString());
                node.SetAttribute("fill", "none");
                node.SetAttribute("stroke", ST.ToString(link.Color.IsEmpty ? map.LinkLineColor : link.Color));
                node.SetAttribute("stroke-width", link.LineWidth.ToString());
                if(!string.IsNullOrEmpty(dasharray))
                    node.SetAttribute("stroke-dasharray", dasharray);
                root.AppendChild(node);

                if (!string.IsNullOrEmpty(link.Text) && !layout.TextBounds.IsEmpty)
                {
                    XmlElement nodeText = root.OwnerDocument.CreateElement("text");
                    nodeText.SetAttribute("x", layout.TextBounds.X.ToString());
                    nodeText.SetAttribute("y", (layout.TextBounds.Y + layout.TextBounds.Height / 2).ToString());
                    nodeText.InnerText = link.Text;
                    root.AppendChild(nodeText);
                }
            }

            if (!topic.Children.IsEmpty && !topic.Folded)
            {
                foreach (Topic subTopic in topic.Children)
                {
                    ExportLinks(map, subTopic, root, defaultFont, layouter, liner);
                }
            }
        }

        void ExportIcon(XmlElement g, Topic topic)
        {
            if (topic == null)// || topic.Icon == null)
                return;
            
            PictureWidget[] icons = topic.FindWidgets<PictureWidget>();
            if (icons.Length == 0)
                return;
            
            //TopicStyle style = topic.Style;
            //Rectangle rect = topic.Bounds;
            //rect.X += style.Padding.Left;
            //rect.Y += style.Padding.Top;
            //rect.Width -= style.Padding.Horizontal;
            //rect.Height -= style.Padding.Vertical;
            //Rectangle rectIcon = new Rectangle(rect.Left + style.IconPadding, rect.Top + (rect.Height - topic.Icon.Height) / 2, topic.Icon.Width, topic.Icon.Height);

            //XmlElement node = g.OwnerDocument.CreateElement("image");
            //node.SetAttribute("x", rectIcon.X.ToString());
            //node.SetAttribute("y", rectIcon.Y.ToString());
            //node.SetAttribute("width", rectIcon.Width.ToString());
            //node.SetAttribute("height", rectIcon.Height.ToString());
            //XmlAttribute href = g.OwnerDocument.CreateAttribute("xlink", "href", "http://www.w3.org/1999/xlink");
            //href.Value = "data:image/png;base64," + ST.ImageBase64String(topic.Icon);
            //node.Attributes.Append(href);
            //g.AppendChild(node);
        }

        class TextLine
        {
            public string Text { get; set; }

            public int Width { get; set; }

            public int Height { get; set; }

            public static TextLine[] MeasureLines(Graphics graphics, string text, Font font, Rectangle rectangle)
            {
                return new TextLine[0];
            }
        }

        public static void ExportText(string text, Color fontColor, Font font, XmlElement g, Rectangle rectangle)
        {
            if (string.IsNullOrEmpty(text))
                return;

            XmlElement node = g.OwnerDocument.CreateElement("text");
            node.SetAttribute("fill", ST.ToString(fontColor));
            Graphics grf = Graphics.FromHwnd(IntPtr.Zero);
            if (grf != null)
            {
                if (text.Length > 32)
                {
                    //text = text.Substring(0, 32);
                    Size size = Size.Ceiling(grf.MeasureString(text, font));

                    XmlElement tspan = node.OwnerDocument.CreateElement("tspan");
                    tspan.SetAttribute("x", Math.Max(0, rectangle.X + (rectangle.Width - size.Width) / 2).ToString());
                    tspan.SetAttribute("y", (rectangle.Y + (rectangle.Height - size.Height) / 2 + font.Height * 2 / 3).ToString());
                    tspan.InnerText = text;
                    node.AppendChild(tspan);
                }
                else
                {
                    //TextLine[] lines = TextLine.MeasureLines(grf, text, font, rectangle);
                    StringFormat sf = PaintHelper.SFCenter;
                    CharacterRange[] crs = new CharacterRange[text.Length];
                    for (int i = 0; i < crs.Length; i++)
                    {
                        crs[i] = new CharacterRange(i, 1);
                    }
                    sf.SetMeasurableCharacterRanges(crs);
                    Region[] regions = grf.MeasureCharacterRanges(text, font, rectangle, sf);

                    RectangleF rectLast = RectangleF.Empty;
                    StringBuilder sb = new StringBuilder(text.Length);
                    float xx = -1;
                    float hh = 0;
                    for (int i = 0; i <= text.Length; i++)
                    {
                        //if (text[i] <= 0x20)
                        //    continue;                    

                        RectangleF rect = i < regions.Length ? regions[i].GetBounds(grf) : RectangleF.Empty;
                        //if (rect.Width <= 0 || rect.Height <= 0)
                        //    continue;

                        if (i == text.Length || rectLast.Top != rect.Top)
                        {
                            if (HasAnyVisibleChar(sb))
                            {
                                XmlElement tspan = node.OwnerDocument.CreateElement("tspan");
                                tspan.SetAttribute("x", xx.ToString());
                                tspan.SetAttribute("y", (rectLast.Top + font.Height * 2 / 3).ToString());
                                tspan.InnerText = sb.ToString();
                                node.AppendChild(tspan);
                            }

                            if (i == text.Length)
                            {
                                break;
                            }

                            xx = -1;
                            sb.Length = 0;
                        }
                        if (xx < 0)
                            xx = rect.Left;
                        hh = Math.Max(hh, rect.Height);
                        rectLast = rect;
                        sb.Append(text[i]);
                    }
                }

                grf.Dispose();
            }

            g.AppendChild(node);
        }

        void ExportFoldButtons(XmlElement defs)
        {
            const string strokeColor = "#C0C0C0";
            float fbs = CommonOptions.Charts.FoldingButtonSize - 2;
            float fbs2 = fbs / 2;

            if (fbs2 <= 1)
                return;
            
            // plus button
            XmlElement gPlus = defs.OwnerDocument.CreateElement("g");
            gPlus.SetAttribute("id", "btn_plus");
            gPlus.SetAttribute("stroke", strokeColor);
            defs.AppendChild(gPlus);

            XmlElement c = defs.OwnerDocument.CreateElement("circle");
            c.SetAttribute("cx", fbs2.ToString());
            c.SetAttribute("cy", fbs2.ToString());
            c.SetAttribute("r", fbs2.ToString());
            c.SetAttribute("fill", "#F5F5F5");
            gPlus.AppendChild(c);
            XmlElement cl1 = defs.OwnerDocument.CreateElement("line");
            cl1.SetAttribute("x1", "2");
            cl1.SetAttribute("y1", fbs2.ToString());
            cl1.SetAttribute("x2", (fbs - 2).ToString());
            cl1.SetAttribute("y2", fbs2.ToString());
            gPlus.AppendChild(cl1);
            XmlElement cl2 = defs.OwnerDocument.CreateElement("line");
            cl2.SetAttribute("x1", fbs2.ToString());
            cl2.SetAttribute("y1", "2");
            cl2.SetAttribute("x2", fbs2.ToString());
            cl2.SetAttribute("y2", (fbs - 2).ToString());
            gPlus.AppendChild(cl2);

            // minus button
            XmlElement gMinus = defs.OwnerDocument.CreateElement("g");
            gMinus.SetAttribute("id", "btn_minus");
            gMinus.SetAttribute("stroke", strokeColor);
            defs.AppendChild(gMinus);

            c = defs.OwnerDocument.CreateElement("circle");
            c.SetAttribute("cx", fbs2.ToString());
            c.SetAttribute("cy", fbs2.ToString());
            c.SetAttribute("r", fbs2.ToString());
            c.SetAttribute("fill", "#F5F5F5");
            gMinus.AppendChild(c);
            cl1 = defs.OwnerDocument.CreateElement("line");
            cl1.SetAttribute("x1", "2");
            cl1.SetAttribute("y1", fbs2.ToString());
            cl1.SetAttribute("x2", (fbs - 2).ToString());
            cl1.SetAttribute("y2", fbs2.ToString());
            gMinus.AppendChild(cl1);
        }

        void ExportStyle(XmlElement node, MindMap mindmap)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine();
            sb.Append("svg {");
            if (!mindmap.BackColor.IsEmpty)
                sb.AppendFormat("background-color:{0};", ST.ToString(mindmap.BackColor));
            if (!mindmap.ForeColor.IsEmpty)
                sb.AppendFormat("color:{0}", ST.ToString(mindmap.ForeColor));
            sb.AppendLine("}");

            XmlCDataSection style_data = node.OwnerDocument.CreateCDataSection(sb.ToString());
            node.AppendChild(style_data);
        }

        static bool HasAnyVisibleChar(StringBuilder sb)
        {
            if (sb.Length == 0)
                return false;

            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] <= 0x20)
                    continue;

                return true;
            }
            return false;
        }

        void SetFontInfo(XmlElement node, Font font)
        {
            node.SetAttribute("font-family", font.FontFamily.Name);
            node.SetAttribute("font-size", font.Size.ToString());
        }
        */
    }
}
