using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Blumind.Controls;
using Blumind.Core;

namespace Blumind.Canvas.Svg
{
    class SvgGraphics : IGraphics
    {
        SvgDocument Dom;
        XmlElement CurrentElement;
        SvgGraphicsState CurrentState;

        public SvgGraphics(SvgDocument dom)
        {
            if (dom == null)
                throw new ArgumentNullException();

            Dom = dom;
            CurrentElement = dom.Canvas;
            CurrentState = new SvgGraphicsState();
        }

        public SmoothingMode SmoothingMode { get; set; }

        void AppendElement(XmlElement element)
        {
            if (element == null)
                return;

            CurrentState.Render(element);
            CurrentElement.AppendChild(element);
        }

        public void Clear(Color color)
        {
            Dom.BackColor = color;
        }

        public void TranslateTransform(float dx, float dy)
        {
            CurrentState.Translation = new PointF(CurrentState.Translation.X + dx, CurrentState.Translation.Y + dy);
        }

        public void RotateTransform(float angle)
        {
            CurrentState.Rotation = (CurrentState.Rotation + angle) % 360;
        }

        public IBrush LinearGradientBrush(Rectangle rect, Color color1, Color color2, LinearGradientMode mode)
        {
            return new SvgLinearGradientBrush(rect, color1, color2, mode);
        }

        public IBrush LinearGradientBrush(Rectangle rect, Color color1, Color color2, float angle)
        {
            return new SvgLinearGradientBrush(rect, color1, color2, angle);
        }

        public IBrush SolidBrush(Color color)
        {
            return new SvgSolidBrush(color);
        }

        public IPen Pen(Color color)
        {
            return new SvgPen(color);
        }

        public IPen Pen(Color color, float width)
        {
            return new SvgPen(color, width);
        }

        public IPen Pen(Color color, float width, DashStyle dashStyle)
        {
            var pen = new SvgPen(color, width);
            pen.DashStyle = dashStyle;
            return pen;
        }

        public IPen Pen(Color color, DashStyle dashStyle)
        {
            var pen = new SvgPen(color);
            pen.DashStyle = dashStyle;
            return pen;
        }

        public IFont Font(IFont font, FontStyle fontStyle)
        {
            return new SvgFont((Font)font.Raw, fontStyle);
        }

        public IFont Font(Font font)
        {
            return new SvgFont(font);
        }

        public IFont Font(Font font, FontStyle fontStyle)
        {
            return new SvgFont(font, fontStyle);
        }

        public IGraphicsPath GraphicsPath()
        {
            return new SvgGraphicsPath();
        }

        public SizeF MeasureString(string text, IFont font)
        {
            using (var screen = Graphics.FromHwndInternal(IntPtr.Zero))
            {
                return TextRenderer.MeasureText(text, ((SvgFont)font).GdiFont);
            }
        }

        public void DrawBezier(IPen pen, Point startPoint, Point controlPoint1, Point controlPoint2, Point endPoint, LineAnchor startAnchor, LineAnchor endAnchor)
        {
            var path = new SvgGraphicsPath();
            path.AddBezier(startPoint, controlPoint1, controlPoint2, endPoint);
            var element = DrawPathElement(pen, null, path);

            if (startAnchor != LineAnchor.None || endAnchor != LineAnchor.None)
                SetLineAnchor(element, startAnchor, endAnchor, pen.Color);
        }

        public void DrawString(string text, IFont font, IBrush brush, Rectangle rect, StringFormat stringFormat)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var sfont = (SvgFont)font;
            var gfont = sfont.GdiFont;

            var element = Dom.CreateElement("text");
            element.SetAttribute("fill", ((SvgBrush)brush).Render(element));
            element.SetAttribute("x", rect.X.ToString());
            element.SetAttribute("y", rect.Y.ToString());
            sfont.Render(element);
            AppendElement(element);

            var _lines = text.Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.None);
            var lines = _lines.Select(l => new TextLine(l)).ToArray();

            // measure
            int fontascent = 0;
            using (var screen = Graphics.FromHwndInternal(IntPtr.Zero))
            {
                var gfamily = gfont.FontFamily;
                var gsize = gfont.SizeInPoints * screen.DpiX / 72;
                fontascent = (int)Math.Ceiling(gfamily.GetCellAscent(sfont.Style) * gsize / gfamily.GetEmHeight(sfont.Style));

                foreach (var line in lines)
                {
                    line.Size = Size.Ceiling(screen.MeasureString(line.Text, gfont));
                }
            }

            //
            var fullSize = new Size(lines.Max(line => line.Bounds.Width), lines.Sum(line => line.Bounds.Height));

            // position
            int y;
            switch (stringFormat.LineAlignment)
            {
                case StringAlignment.Near:
                    y = rect.Y;
                    break;
                case StringAlignment.Far:
                    y = rect.Bottom - fullSize.Height;
                    break;
                case StringAlignment.Center:
                default:
                    y = rect.Y + (rect.Height - fullSize.Height) / 2;
                    break;
            }
            y += fontascent;
            foreach (var line in lines)
            {
                switch (stringFormat.Alignment)
                {
                    case StringAlignment.Near:
                        line.Location = new Point(rect.X, y);
                        break;
                    case StringAlignment.Far:
                        line.Location = new Point(rect.Right - line.Width, y);
                        break;
                    case StringAlignment.Center:
                    default:
                        line.Location = new Point(rect.X + (rect.Width - line.Width) / 2, y);
                        break;
                }
                y += line.Height;
            }

            // render
            foreach (var line in lines)
            {
                var span = Dom.CreateElement("tspan");
                span.SetAttribute("x", line.X.ToString());
                span.SetAttribute("y", line.Y.ToString());
                span.InnerText = line.Text;
                element.AppendChild(span);
            }
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight)
        {
            if (image == null)
                return;

            XmlElement element = Dom.CreateElement("image");
            element.SetAttribute("x", destRect.X.ToString());
            element.SetAttribute("y", destRect.Y.ToString());
            element.SetAttribute("width", destRect.Width.ToString());
            element.SetAttribute("height", destRect.Height.ToString());

            XmlAttribute href = Dom.CreateAttribute("xlink", "href", "http://www.w3.org/1999/xlink");
            if (srcX != 0 || srcY != 0 || srcWidth != image.Width || srcHeight != image.Width)
            {
                using (var slice = PaintHelper.CutImage(image, srcX, srcY, srcWidth, srcHeight))
                {
                    href.Value = "data:image/png;base64," + ST.ImageBase64String(slice);
                }

            }
            else
            {
                href.Value = "data:image/png;base64," + ST.ImageBase64String(image);
            }
            element.Attributes.Append(href);
            AppendElement(element);
        }

        public void DrawLines(IPen pen, Point[] points, LineAnchor startAnchor, LineAnchor endAnchor)
        {
            if(pen == null || points == null)
                throw new ArgumentNullException();

            var sb = new StringBuilder();
            foreach(var p in points)
            {
                if(sb.Length > 0)
                    sb.Append(" ");
                sb.AppendFormat("{0},{1}", p.X, p.Y);
            }

            var element = Dom.CreateElement("polyline");
            element.SetAttribute("points", sb.ToString());
            element.SetAttribute("fill", "none");
            element.SetAttribute("stroke", ((SvgPen)pen).Render(element));
            AppendElement(element);

            if (startAnchor != LineAnchor.None || endAnchor != LineAnchor.None)
                SetLineAnchor(element, startAnchor, endAnchor, pen.Color);
        }

        public void DrawLine(IPen pen, int x1, int y1, int x2, int y2, LineAnchor startAnchor, LineAnchor endAnchor)
        {
            var element = Dom.CreateElement("line");
            element.SetAttribute("x1", x1.ToString());
            element.SetAttribute("y1", y1.ToString());
            element.SetAttribute("x2", x2.ToString());
            element.SetAttribute("y2", y2.ToString());
            element.SetAttribute("fill", "none");
            element.SetAttribute("stroke", ((SvgPen)pen).Render(element));
            AppendElement(element);

            if (startAnchor != LineAnchor.None || endAnchor != LineAnchor.None)
                SetLineAnchor(element, startAnchor, endAnchor, pen.Color);
        }

        public void DrawRectangle(IPen pen, IBrush brush, int x, int y, int width, int height)
        {
            var element = Dom.CreateElement("rect");
            element.SetAttribute("x", x.ToString());
            element.SetAttribute("y", y.ToString());
            element.SetAttribute("width", width.ToString());
            element.SetAttribute("height", height.ToString());

            if (brush is SvgBrush)
                element.SetAttribute("fill", ((SvgBrush)brush).Render(element));
            else
                element.SetAttribute("fill", "none");

            if (pen is SvgPen)
                element.SetAttribute("stroke", ((SvgPen)pen).Render(element));
            else
                element.SetAttribute("stroke", "none");

            AppendElement(element);
        }

        public void DrawEllipse(IPen pen, IBrush brush, int x, int y, int width, int height)
        {
            var element = Dom.CreateElement("ellipse");
            element.SetAttribute("cx", (x + width / 2).ToString());
            element.SetAttribute("cy", (y + height / 2).ToString());
            element.SetAttribute("rx", (width / 2).ToString());
            element.SetAttribute("ry", (height / 2).ToString());

            if (brush is SvgBrush)
                element.SetAttribute("fill", ((SvgBrush)brush).Render(element));
            else
                element.SetAttribute("fill", "none");

            if (pen is SvgPen)
                element.SetAttribute("stroke", ((SvgPen)pen).Render(element));
            else
                element.SetAttribute("stroke", "none");

            AppendElement(element);
        }

        public void DrawRoundRectangle(IPen pen, IBrush brush, int x, int y, int width, int height, int round)
        {
            var element = Dom.CreateElement("rect");
            element.SetAttribute("x", x.ToString());
            element.SetAttribute("y", y.ToString());
            element.SetAttribute("width", width.ToString());
            element.SetAttribute("height", height.ToString());
            element.SetAttribute("rx", round.ToString());
            element.SetAttribute("ry", round.ToString());

            if (brush is SvgBrush)
                element.SetAttribute("fill", ((SvgBrush)brush).Render(element));
            else
                element.SetAttribute("fill", "none");

            if (pen is SvgPen)
                element.SetAttribute("stroke", ((SvgPen)pen).Render(element));
            else
                element.SetAttribute("stroke", "none");

            AppendElement(element);
        }

        public void DrawPath(IPen pen, IBrush brush, IGraphicsPath path)
        {
            DrawPathElement(pen, brush, path);
        }

        XmlElement DrawPathElement(IPen pen, IBrush brush, IGraphicsPath path)
        {
            var element = Dom.CreateElement("path");
            element.SetAttribute("d", ((SvgGraphicsPath)path).Render(element));

            if (brush is SvgBrush)
                element.SetAttribute("fill", ((SvgBrush)brush).Render(element));
            else
                element.SetAttribute("fill", "none");

            if (pen is SvgPen)
                element.SetAttribute("stroke", ((SvgPen)pen).Render(element));
            else
                element.SetAttribute("stroke", "none");

            AppendElement(element);
            return element;
        }

        public void ClearHighQualityFlag()
        {
        }

        public void SetHighQualityRender()
        {
        }

        public void SetClip(Rectangle rect)
        {
        }

        public void Restore(IGraphicsState state)
        {
            CurrentState.CopyFrom(state);
        }

        public IGraphicsState Save()
        {
            return CurrentState.Copy();
        }

        void SetLineAnchor(XmlElement element, LineAnchor startAnchor, LineAnchor endAnchor, Color color)
        {
            if(element == null)
                return;

            if (startAnchor != LineAnchor.None)
            {
                element.SetAttribute("marker-start", string.Format("url(#{0})", GetLineAnchorID(startAnchor, color))); 
            }

            if (endAnchor != LineAnchor.None)
            {
                element.SetAttribute("marker-end", string.Format("url(#{0})", GetLineAnchorID(endAnchor, color))); 
            }
        }

        List<string> LineAnchors = new List<string>();
        string GetLineAnchorID(LineAnchor anchor, Color color)
        {
            if (anchor == LineAnchor.None)
                return null;

            var id = string.Format("{0}_{1}", anchor, ST.ToWebColorWithoutSign(color));
            if (LineAnchors.Contains(id))
                return id;

            //
            XmlElement element = Dom.CreateElement("marker");
            element.SetAttribute("id", id);
            element.SetAttribute("viewBox", "0 0 10 10");
            element.SetAttribute("refX", "5");
            element.SetAttribute("refY", "5");
            element.SetAttribute("markerUnits", "strokeWidth");
            element.SetAttribute("orient", "auto");
            element.SetAttribute("markerWidth", "6");
            element.SetAttribute("markerHeight", "5");
            Dom.Defines.AppendChild(element);

            XmlElement path = null;
            switch (anchor)
            {
                case LineAnchor.Arrow:
                    path = Dom.CreateElement("polyline");
                    path.SetAttribute("points", "0,0 9,5 0,9 0,5");
                    break;
                case LineAnchor.Round:
                    path = Dom.CreateElement("ellipse");
                    path.SetAttribute("cx", "4");
                    path.SetAttribute("cy", "5");
                    path.SetAttribute("rx", "4");
                    path.SetAttribute("ry", "4");
                    break;
                case LineAnchor.Square:
                    path = Dom.CreateElement("rect");
                    path.SetAttribute("x", "0");
                    path.SetAttribute("y", "1");
                    path.SetAttribute("width", "8");
                    path.SetAttribute("height", "8");
                    break;
                case LineAnchor.Diamond:
                    path = Dom.CreateElement("polyline");
                    path.SetAttribute("points", "0,5 5,0 10,5 5,10");
                    break;
            }

            if (path != null)
            {
                path.SetAttribute("fill", ST.ToWebColor(color));
                element.AppendChild(path);
            }

            LineAnchors.Add(id);
            return id;
        }

#if DEBUG
        internal static void Test(string filename)
        {
            //
            var svg = new SvgDocument();
            var graphics = new SvgGraphics(svg);
            var rect = new Rectangle(180, 80, 200, 150);
            //graphics.DrawRectangle(graphics.Pen(Color.Red), graphics.SolidBrush(Color.Yellow), rect.X, rect.Y, rect.Width, rect.Height);
            ////graphics.DrawRectangle(graphics.Pen(Color.Red), rect.X, rect.Y, rect.Width, rect.Height);
            //graphics.DrawBezier(graphics.Pen(Color.Green), new Point(100, 100), new Point(100, 200), new Point(200, 200), new Point(200, 100));
            //graphics.DrawString("Hello, World", graphics.Font(SystemFonts.MessageBoxFont), graphics.SolidBrush(Color.Blue), rect, PaintHelper.SFCenter);
            //graphics.DrawString("两个黄鹂鸣翠柳\n一行白鹭上青天\n窗含西岭千秋雪\n门泊东吴万里船",
            //    graphics.Font(SystemFonts.MessageBoxFont, FontStyle.Bold | FontStyle.Italic), graphics.SolidBrush(Color.OrangeRed), rect, PaintHelper.SFCenter);
            //graphics.DrawImage(Properties.Resources.document_128, 10, 10);
            graphics.DrawLine(graphics.Pen(Color.BlueViolet, 5), 240, 150, 350, 260, LineAnchor.Round, LineAnchor.Arrow);
            graphics.DrawLines(graphics.Pen(Color.Brown, 2),
                new Point[] { 
                    new Point(50, 375), 
                    new Point(150, 375), 
                    new Point(150, 325), 
                    new Point(250, 325), 
                    new Point(250, 375),
                    new Point(350, 375) },
                   LineAnchor.Diamond, LineAnchor.Square);

            //graphics.DrawEllipse(graphics.Pen(Color.Green), graphics.SolidBrush(Color.YellowGreen), 10, 10, 100, 50);

            //graphics.DrawRoundRectangle(graphics.Pen(Color.Green), graphics.SolidBrush(Color.YellowGreen), 10, 10, 100, 50, 10);

            svg.Save(filename);
        }
#endif
    }
}
