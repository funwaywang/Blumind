using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Blumind.Canvas
{
    static class IGraphicsExtensions
    {
        public static void DrawImage(this IGraphics graphics, Image image, Rectangle descRect)
        {
            if (graphics == null)
                throw new NullReferenceException();

            graphics.DrawImage(image, descRect, 0, 0, image.Width, image.Height);
        }

        public static void DrawImage(this IGraphics graphics, Image image, int x, int y)
        {
            if (graphics == null)
                throw new NullReferenceException();

            graphics.DrawImage(image, new Rectangle(x, y, image.Width, image.Height), 0, 0, image.Width, image.Height);
        }

        public static void DrawLine(this IGraphics graphics, IPen pen, Point point1, Point point2)
        {
            if (graphics == null)
                throw new NullReferenceException();

            graphics.DrawLine(pen, point1.X, point1.Y, point2.X, point2.Y, LineAnchor.None, LineAnchor.None);
        }

        public static void DrawLine(this IGraphics graphics, IPen pen, Point point1, Point point2, LineAnchor startAnchor, LineAnchor endAnchor)
        {
            if (graphics == null)
                throw new NullReferenceException();

            graphics.DrawLine(pen, point1.X, point1.Y, point2.X, point2.Y, startAnchor, endAnchor);
        }

        public static void DrawLines(this IGraphics graphics, IPen pen, Point[] points)
        {
            if (graphics == null)
                throw new NullReferenceException();

            graphics.DrawLines(pen, points, LineAnchor.None, LineAnchor.None);
        }

        public static void DrawLine(this IGraphics graphics, IPen pen, int x1, int y1, int x2, int y2)
        {
            if (graphics == null)
                throw new NullReferenceException();

            graphics.DrawLine(pen, x1, y1, x2, y2, LineAnchor.None, LineAnchor.None);
        }

        public static void DrawBezier(this IGraphics graphics, IPen pen, Point startPoint, Point controlPoint1, Point controlPoint2, Point endPoint)
        {
            if (graphics == null)
                throw new NullReferenceException();

            graphics.DrawBezier(pen, startPoint, controlPoint1, controlPoint2, endPoint, LineAnchor.None, LineAnchor.None);
        }

        public static void FillRoundRectangle(this IGraphics graphics, IBrush brush, Rectangle rect, int round)
        {
            if (graphics == null)
                throw new NullReferenceException();

            graphics.DrawRoundRectangle(null, brush, rect.X, rect.Y, rect.Width, rect.Height, round);
        }

        public static void DrawRoundRectangle(this IGraphics graphics, IPen pen, Rectangle rect, int round)
        {
            if (graphics == null)
                throw new NullReferenceException();

            graphics.DrawRoundRectangle(pen, null, rect.X, rect.Y, rect.Width, rect.Height, round);
        }

        public static void FillEllipse(this IGraphics graphics, IBrush brush, Rectangle rect)
        {
            if (graphics == null)
                throw new NullReferenceException();

            graphics.DrawEllipse(null, brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static void FillEllipse(this IGraphics graphics, IBrush brush, int x, int y, int width, int height)
        {
            if (graphics == null)
                throw new NullReferenceException();

            graphics.DrawEllipse(null, brush, x, y, width, height);
        }

        public static void DrawEllipse(this IGraphics graphics, IPen pen, int x, int y, int width, int height)
        {
            if (graphics == null)
                throw new NullReferenceException();

            graphics.DrawEllipse(pen, null, x, y, width, height);
        }

        public static void DrawPath(this IGraphics graphics, IPen pen, IGraphicsPath path)
        {
            if (graphics == null)
                throw new NullReferenceException();

            graphics.DrawPath(pen, null, path);
        }

        public static void FillPath(this IGraphics graphics, IBrush brush, IGraphicsPath path)
        {
            if (graphics == null)
                throw new NullReferenceException();

            graphics.DrawPath(null, brush, path);
        }

        public static void DrawRectangle(this IGraphics graphics, IPen pen, int x, int y, int width, int height)
        {
            if (graphics == null)
                throw new NullReferenceException();

            graphics.DrawRectangle(pen, null, x, y, width, height);
        }

        public static void FillRectangle(this IGraphics graphics, IBrush brush, Rectangle rectangle)
        {
            if (graphics == null)
                throw new NullReferenceException();

            graphics.DrawRectangle(null, brush, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }
    }
}
