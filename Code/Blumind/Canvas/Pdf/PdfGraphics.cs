using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using PdfSharp.Drawing;

namespace Blumind.Canvas.Pdf
{
    class PdfGraphics : IGraphics
    {
        XGraphics Graphics;

        public PdfGraphics(XGraphics graphics)
        {
            this.Graphics = graphics;
        }

        XPdfFontOptions FontOptions
        {
            get
            {
                return new XPdfFontOptions(PdfSharp.Pdf.PdfFontEncoding.Unicode, PdfSharp.Pdf.PdfFontEmbedding.Default);
            }
        }

        public SmoothingMode SmoothingMode
        {
            get
            {
                switch (Graphics.SmoothingMode)
                {
                    case XSmoothingMode.AntiAlias:
                        return SmoothingMode.AntiAlias;
                    case XSmoothingMode.HighQuality:
                        return SmoothingMode.HighQuality;
                    case XSmoothingMode.HighSpeed:
                        return SmoothingMode.HighSpeed;
                    case XSmoothingMode.Invalid:
                        return SmoothingMode.Invalid;
                    case XSmoothingMode.None:
                        return SmoothingMode.None;
                    default:
                    case XSmoothingMode.Default:
                        return SmoothingMode.Default;
                }
            }
            set
            {
                switch (value)
                {
                    case SmoothingMode.AntiAlias:
                        Graphics.SmoothingMode = XSmoothingMode.AntiAlias;
                        break;
                    case SmoothingMode.HighQuality:
                        Graphics.SmoothingMode = XSmoothingMode.HighQuality;
                        break;
                    case SmoothingMode.HighSpeed:
                        Graphics.SmoothingMode = XSmoothingMode.HighSpeed;
                        break;
                    case SmoothingMode.Invalid:
                        Graphics.SmoothingMode = XSmoothingMode.Invalid;
                        break;
                    case SmoothingMode.None:
                        Graphics.SmoothingMode = XSmoothingMode.None;
                        break;
                    case SmoothingMode.Default:
                    default:
                        Graphics.SmoothingMode = XSmoothingMode.Default;
                        break;
                }
            }
        }

        public void TranslateTransform(float dx, float dy)
        {
            Graphics.TranslateTransform(dx, dy);
        }

        public void RotateTransform(float angle)
        {
            Graphics.RotateTransform(angle);
        }

        public IBrush LinearGradientBrush(Rectangle rect, Color color1, Color color2, LinearGradientMode mode)
        {
            XLinearGradientMode m;
            switch (mode)
            {
                case LinearGradientMode.Vertical:
                    m = XLinearGradientMode.Vertical;
                    break;
                case LinearGradientMode.BackwardDiagonal:
                    m = XLinearGradientMode.BackwardDiagonal;
                    break;
                case LinearGradientMode.ForwardDiagonal:
                    m = XLinearGradientMode.ForwardDiagonal;
                    break;
                case LinearGradientMode.Horizontal:
                default:
                    m = XLinearGradientMode.Horizontal;
                    break;
            }

            return new PdfBrush(new XLinearGradientBrush(rect, color1, color2, m));
        }

        public IBrush LinearGradientBrush(Rectangle rect, Color color1, Color color2, float angle)
        {
            var xbrush = new XLinearGradientBrush(rect, color1, color2, XLinearGradientMode.Horizontal);
            xbrush.RotateTransform(angle);

            return new PdfBrush(xbrush);
        }

        public IBrush SolidBrush(Color color)
        {
            return new PdfBrush(new XSolidBrush(color));
        }

        public IPen Pen(Color color)
        {
            return new PdfPen(new XPen(color));
        }

        public IPen Pen(Color color, float width)
        {
            return new PdfPen(new XPen(color, width));
        }

        public IPen Pen(Color color, float width, DashStyle dashStyle)
        {
            var xpen = new XPen(color, width);
            xpen.DashStyle = (XDashStyle)dashStyle;

            return new PdfPen(xpen);
        }

        public IPen Pen(Color color, DashStyle dashStyle)
        {
            var xpen = new XPen(color);
            xpen.DashStyle = (XDashStyle)dashStyle;

            return new PdfPen(xpen);
        }

        public IFont Font(IFont font, FontStyle fontStyle)
        {
            var xf = (XFont)font.Raw;
            xf = new XFont(xf.GdiFamily, xf.Size, xf.Style | (XFontStyle)fontStyle, FontOptions);
            return new PdfFont(xf);
        }

        public IFont Font(Font font)
        {
            var xfont = new XFont(font.FontFamily, font.Size, (XFontStyle)font.Style, FontOptions);
            return new PdfFont(xfont);
        }

        public IGraphicsPath GraphicsPath()
        {
            return new PdfGraphicsPath(new XGraphicsPath());
        }

        public SizeF MeasureString(string text, IFont font)
        {
            var size = Graphics.MeasureString(text, (XFont)font.Raw);
            return size.ToSizeF();
        }

        public void DrawBezier(IPen pen, Point startPoint, Point controlPoint1, Point controlPoint2, Point endPoint, LineAnchor startAnchor, LineAnchor endAnchor)
        {
            Graphics.DrawBezier((XPen)pen.Raw, startPoint, controlPoint1, controlPoint2, endPoint);
        }

        public void DrawString(string text, IFont font, IBrush brush, Rectangle rect, StringFormat stringFormat)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var sf = new XStringFormat();
            sf.Alignment = (XStringAlignment)stringFormat.Alignment;
            sf.LineAlignment = (XLineAlignment)stringFormat.LineAlignment;
            //sf.FormatFlags = (XStringFormatFlags)stringFormat.FormatFlags;

            if (text.Contains(Environment.NewLine))
            {
                var xfont = (XFont)font.Raw;
                var lines = text.Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.None);
                var lineHeight = (int)Math.Ceiling(xfont.GetHeight(Graphics));
                var totalHeight = lineHeight * lines.Length;

                int y;
                switch (stringFormat.LineAlignment)
                {
                    case StringAlignment.Near:
                        y = rect.Top;
                        break;
                    case StringAlignment.Far:
                        y = rect.Top + rect.Height - totalHeight;
                        break;
                    case StringAlignment.Center:
                    default:
                        y = rect.Top + (rect.Height - totalHeight) / 2;
                        break;
                }

                var rectLine = new Rectangle(rect.X, y, rect.Width, lineHeight);
                foreach (var line in lines)
                {
                    Graphics.DrawString(line, (XFont)font.Raw, (XBrush)brush.Raw, rectLine, sf);
                    rectLine.Y += rectLine.Height;
                }
            }
            else
            {
                Graphics.DrawString(text, (XFont)font.Raw, (XBrush)brush.Raw, rect, sf);
            }
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight)
        {
            Graphics.DrawImage(image, destRect, new Rectangle(srcX, srcY, srcWidth, srcHeight), XGraphicsUnit.Presentation);
        }

        public void DrawLines(IPen pen, Point[] points, LineAnchor startAnchor, LineAnchor endAnchor)
        {
            Graphics.DrawLines((XPen)pen.Raw, points);
        }

        public void DrawLine(IPen pen, Point point1, Point point2, LineAnchor startAnchor, LineAnchor endAnchor)
        {
            Graphics.DrawLine((XPen)pen.Raw, point1, point2);
        }

        public void DrawLine(IPen pen, int x1, int y1, int x2, int y2, LineAnchor startAnchor, LineAnchor endAnchor)
        {
            Graphics.DrawLine((XPen)pen.Raw, x1, y1, x2, y2);
        }

        public void DrawRectangle(IPen pen, IBrush brush, int x, int y, int width, int height)
        {
            if(pen != null && brush != null)
                Graphics.DrawRectangle((XPen)pen.Raw, (XBrush)brush.Raw, x, y, width, height);
            else if (pen == null && brush != null)
                Graphics.DrawRectangle((XBrush)brush.Raw, x, y, width, height);
            else if (pen != null && brush == null)
                Graphics.DrawRectangle((XPen)pen.Raw, x, y, width, height);
        }

        public void Restore(IGraphicsState state)
        {
            Graphics.Restore((XGraphicsState)state.Raw);
        }

        public IGraphicsState Save()
        {
            return new PdfGraphicsState(Graphics.Save());
        }

        public void DrawEllipse(IPen pen, IBrush brush, int x, int y, int width, int height)
        {
            if(brush is PdfBrush)
                Graphics.DrawEllipse((XBrush)brush.Raw, x, y, width, height);

            if(pen is PdfPen)
                Graphics.DrawEllipse((XPen)pen.Raw, x, y, width, height);
        }

        public void DrawPath(IPen pen, IBrush brush, IGraphicsPath path)
        {
            if (brush is PdfBrush)
                Graphics.DrawPath((XBrush)brush, (XGraphicsPath)path.Raw);

            if (pen is PdfPen)
                Graphics.DrawPath((XPen)pen.Raw, (XGraphicsPath)path.Raw);
        }

        public void DrawRoundRectangle(IPen pen, IBrush brush, int x, int y, int width, int height, int round)
        {
            if(brush is PdfBrush)
                Graphics.DrawRoundedRectangle((XBrush)brush.Raw, x, y, width, height, round, round);

            if(brush is PdfPen)
                Graphics.DrawRoundedRectangle((XPen)pen.Raw, x, y, width, height, round, round);
        }

        public void ClearHighQualityFlag()
        {
            Graphics.SmoothingMode = XSmoothingMode.Default;
        }

        public void SetHighQualityRender()
        {
            Graphics.SmoothingMode = XSmoothingMode.HighQuality;
        }

        public void SetClip(Rectangle rect)
        {
            Graphics.IntersectClip(rect);
        }
    }
}
