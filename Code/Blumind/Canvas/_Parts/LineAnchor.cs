using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace Blumind.Canvas
{
    [Serializable]
    enum LineAnchor
    {
        None = 0,
        Arrow,
        Round,
        Square,
        Diamond,
    }

    static class LineAnchorExtensions
    {
        public static LineAnchor ToLineAnchor(this LineCap cap)
        {
            switch (cap)
            {
                case LineCap.ArrowAnchor:
                    return LineAnchor.Arrow;
                case LineCap.RoundAnchor:
                    return LineAnchor.Round;
                case LineCap.SquareAnchor:
                    return LineAnchor.Square;
                case LineCap.DiamondAnchor:
                    return LineAnchor.Diamond;
                default:
                    return LineAnchor.None;
            }
        }

        public static LineAnchor Parse(string text)
        {
            switch (text.ToLower())
            {
                case "arrow":
                    return LineAnchor.Arrow;
                case "round":
                    return LineAnchor.Round;
                case "square":
                    return LineAnchor.Square;
                case "diamond":
                    return LineAnchor.Diamond;
                default:
                    return LineAnchor.None;
            }
        }

        public static CustomLineCap GetCustomLineCap(this LineAnchor anchor)
        {
            var path = new GraphicsPath();
            switch (anchor)
            {
                case LineAnchor.Arrow:
                    path.AddPolygon(new Point[] { new Point(-2, -1), new Point(2, -1), new Point(0, 2) });
                    break;
                case LineAnchor.Round:
                    path.AddEllipse(-1.5f, -0.5f, 3, 3);
                    break;
                case LineAnchor.Square:
                    path.AddRectangle(new RectangleF(-1.5f, -0.5f, 3, 3));
                    break;
                case LineAnchor.Diamond:
                    path.AddPolygon(new Point[] { new Point(-2, 1), new Point(0, -1), new Point(2, 1), new Point(0, 3) });
                    break;
                default:
                    path.Dispose();
                    return null;
            }

            return new CustomLineCap(path, null);
        }

        public static bool TrySetStart(this LineAnchor anchor, Pen pen)
        {
            if (pen == null)
                throw new ArgumentNullException();

            var cap = GetCustomLineCap(anchor);
            if (cap != null)
            {
                pen.CustomStartCap = cap;
                return true;
            }

            return false;
        }

        public static bool TrySetEnd(this LineAnchor anchor, Pen pen)
        {
            if (pen == null)
                throw new ArgumentNullException();

            var cap = GetCustomLineCap(anchor);
            if (cap != null)
            {
                pen.CustomEndCap = cap;
                return true;
            }

            return false;
        }
    }
}
