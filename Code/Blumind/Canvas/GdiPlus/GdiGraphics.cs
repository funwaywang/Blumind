using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using Blumind.Controls;

namespace Blumind.Canvas.GdiPlus
{
    class GdiGraphics : IGraphics
    {
        Graphics Graphics;
        Dictionary<LineAnchor, CustomLineCap> LineAnchors = new Dictionary<LineAnchor, CustomLineCap>();

        public GdiGraphics(Graphics graphics)
        {
            Graphics = graphics;
        }

        public Region Clip
        {
            get { return Graphics.Clip; }
            set { Graphics.Clip = value; }
        }

        public SmoothingMode SmoothingMode
        {
            get { return Graphics.SmoothingMode; }
            set { Graphics.SmoothingMode = value; }
        }

        public void TranslateTransform(float dx, float dy)
        {
            Graphics.TranslateTransform(dx, dy);
        }

        public void RotateTransform(float angle)
        {
            Graphics.RotateTransform(angle);
        }

        public IBrush LinearGradientBrush(Rectangle rect, Color color1, Color color2, float angle)
        {
            return new GdiBrush(new LinearGradientBrush(rect, color1, color2, angle));
        }

        public IBrush LinearGradientBrush(Rectangle rect, Color color1, Color color2, LinearGradientMode mode)
        {
            return new GdiBrush(new LinearGradientBrush(rect, color1, color2, mode));
        }

        public IPen Pen(Color color)
        {
            return new GdiPen(new Pen(color));
        }

        public IPen Pen(Color color, float width)
        {
            return new GdiPen(new Pen(color, width));
        }

        public IPen Pen(Color color, float width, DashStyle dashStyle)
        {
            var pen = new Pen(color, width);
            pen.DashStyle = dashStyle;

            return new GdiPen(pen);
        }

        public IPen Pen(Color color, DashStyle dashStyle)
        {
            var pen = new Pen(color);
            pen.DashStyle = dashStyle;

            return new GdiPen(pen);
        }

        public IBrush SolidBrush(Color color)
        {
            return new GdiBrush(new SolidBrush(color));
        }

        public IFont Font(IFont font, FontStyle fontStyle)
        {
            var f = new Font((Font)font.Raw, fontStyle);
            return new GdiFont(f);
        }

        public IFont Font(Font font)
        {
            return new GdiFont(font);
        }

        public IGraphicsPath GraphicsPath()
        {
            return new GdiGraphicsPath(new GraphicsPath());
        }

        public SizeF MeasureString(string text, IFont font)
        {
            return Graphics.MeasureString(text, (Font)font.Raw);
        }

        public void DrawString(string text, IFont font, IBrush brush, Rectangle rect, StringFormat stringFormat)
        {
            Graphics.DrawString(text, (Font)font.Raw, (Brush)brush.Raw, rect, stringFormat);
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight)
        {
            Graphics.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, GraphicsUnit.Pixel);
        }

        public void DrawLines(IPen pen, Point[] points, LineAnchor startAnchor, LineAnchor endAnchor)
        {
            var p = (Pen)pen.Raw;
            if (startAnchor != LineAnchor.None || endAnchor != LineAnchor.None)
                p = GetPenWithAnchor(p, startAnchor, endAnchor);

            Graphics.DrawLines(p, points);
        }

        public void DrawLine(IPen pen, Point point1, Point point2, LineAnchor startAnchor, LineAnchor endAnchor)
        {
            var p = (Pen)pen.Raw;
            if (startAnchor != LineAnchor.None || endAnchor != LineAnchor.None)
                p = GetPenWithAnchor(p, startAnchor, endAnchor);

            Graphics.DrawLine(p, point1, point2);
        }

        public void DrawLine(IPen pen, int x1, int y1, int x2, int y2, LineAnchor startAnchor, LineAnchor endAnchor)
        {
            var p = (Pen)pen.Raw;
            if (startAnchor != LineAnchor.None || endAnchor != LineAnchor.None)
                p = GetPenWithAnchor(p, startAnchor, endAnchor);

            Graphics.DrawLine(p, x1, y1, x2, y2);
        }

        public void DrawBezier(IPen pen, Point startPoint, Point controlPoint1, Point controlPoint2, Point endPoint, LineAnchor startAnchor, LineAnchor endAnchor)
        {
            var p = (Pen)pen.Raw;
            if (startAnchor != LineAnchor.None || endAnchor != LineAnchor.None)
                p = GetPenWithAnchor(p, startAnchor, endAnchor);

            Graphics.DrawBezier(p, startPoint, controlPoint1, controlPoint2, endPoint);
        }

        Pen GetPenWithAnchor(Pen source, LineAnchor startAnchor, LineAnchor endAnchor)
        {
            var sa = GetPenAnchor(startAnchor);
            var ea = GetPenAnchor(endAnchor);
            if (sa != null || ea != null)
            {
                Pen pen = (Pen)source.Clone();

                if(sa != null)
                    pen.CustomStartCap = sa;
                if(ea != null)
                    pen.CustomEndCap = ea;

                return pen;
            }

            return source;
        }

        CustomLineCap GetPenAnchor(LineAnchor anchor)
        {
            if (anchor == LineAnchor.None)
                return null;

            CustomLineCap cap;
            if(LineAnchors.TryGetValue(anchor, out cap))
                return cap;

            cap = anchor.GetCustomLineCap();
            LineAnchors[anchor] = cap;
            return cap;
        }
        
        public void DrawRectangle(IPen pen, IBrush brush, int x, int y, int width, int height)
        {
            if (brush != null)
                Graphics.FillRectangle((Brush)brush.Raw, x, y, width, height);

            if (pen != null)
                Graphics.DrawRectangle((Pen)pen.Raw, x, y, width - 1, height - 1);
        }

        public void DrawEllipse(IPen pen, IBrush brush, int x, int y, int width, int height)
        {
            if (brush is GdiBrush)
                Graphics.FillEllipse((Brush)brush.Raw, x, y, width, height);
            if (pen is GdiPen)
                Graphics.DrawEllipse((Pen)pen.Raw, x, y, width - 1, height - 1);
        }

        public void DrawPath(IPen pen, IBrush brush, IGraphicsPath path)
        {
            if (brush is GdiBrush)
                Graphics.FillPath((Brush)brush.Raw, (GraphicsPath)path.Raw);

            if (pen is GdiPen)
                Graphics.DrawPath((Pen)pen.Raw, (GraphicsPath)path.Raw);
        }

        public void DrawRoundRectangle(IPen pen, IBrush brush, int x, int y, int width, int height, int round)
        {
            if (pen == null && brush == null)
                return;

            var path = PaintHelper.GetRoundRectangle(new Rectangle(x, y, width, height), round);
            if (brush is GdiBrush)
                Graphics.FillPath((Brush)brush.Raw, path);
            if(pen is GdiPen)
                Graphics.DrawPath((Pen)pen.Raw, path);
        }

        public void ClearHighQualityFlag()
        {
            PaintHelper.ClearHighQualityFlag(Graphics);
        }

        public void SetHighQualityRender()
        {
            PaintHelper.SetHighQualityRender(Graphics);
        }

        public void SetClip(Rectangle rect)
        {
            Graphics.SetClip(rect);
        }

        public void Restore(IGraphicsState state)
        {
            Graphics.Restore((GraphicsState)state.Raw);
        }

        public IGraphicsState Save()
        {
            return new GdiGraphicsState(Graphics.Save());
        }
    }
}
