using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace Blumind.Canvas
{
    interface IGraphics
    {
        SmoothingMode SmoothingMode { get; set; }

        void Restore(IGraphicsState state);

        IGraphicsState Save();

        void TranslateTransform(float dx, float dy);

        void RotateTransform(float angle);

        void ClearHighQualityFlag();

        void SetHighQualityRender();

        void SetClip(Rectangle rect);

        IBrush LinearGradientBrush(Rectangle rect, Color color1, Color color2, LinearGradientMode mode);

        IBrush LinearGradientBrush(Rectangle rect, Color color1, Color color2, float angle);

        IBrush SolidBrush(Color color);

        IPen Pen(Color color);

        IPen Pen(Color color, float width);

        IPen Pen(Color color, float width, DashStyle dashStyle);

        IPen Pen(Color color, DashStyle dashStyle);

        IFont Font(IFont font, FontStyle fontStyle);

        IFont Font(Font font);

        IGraphicsPath GraphicsPath();

        SizeF MeasureString(string text, IFont font);

        void DrawString(string text, IFont font, IBrush brush, Rectangle rect, StringFormat stringFormat);

        void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight);

        void DrawLines(IPen pen, Point[] points, LineAnchor startAnchor, LineAnchor endAnchor);

        void DrawLine(IPen pen, int x1, int y1, int x2, int y2, LineAnchor startAnchor, LineAnchor endAnchor);

        void DrawBezier(IPen pen, Point startPoint, Point controlPoint1, Point controlPoint2, Point endPoint, LineAnchor startAnchor, LineAnchor endAnchor);

        void DrawEllipse(IPen pen, IBrush brush, int x, int y, int width, int height);

        void DrawPath(IPen pen, IBrush brush, IGraphicsPath path);

        void DrawRectangle(IPen pen, IBrush brush, int x, int y, int width, int height);

        void DrawRoundRectangle(IPen pen, IBrush brush, int x, int y, int width, int height, int round);
    }
}
