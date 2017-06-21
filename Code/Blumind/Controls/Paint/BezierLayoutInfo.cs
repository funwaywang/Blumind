using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Blumind.Controls.Paint
{
    class BezierLayoutInfo
    {
        public RectangleF Bounds;
        public Region Region;
        public Rectangle StartBounds;
        public Rectangle EndBounds;
        public Point StartPoint;
        public Point EndPoint;
        public Point ControlPoint1;
        public Point ControlPoint2;
        public Rectangle TextBounds;

        public BezierControlPoint CP1;
        public BezierControlPoint CP2;

        public Rectangle MaxRectangle
        {
            get
            {
                int x1 = Math.Min(Math.Min(StartPoint.X, EndPoint.X), Math.Min(ControlPoint1.X, ControlPoint2.X));
                int y1 = Math.Min(Math.Min(StartPoint.Y, EndPoint.Y), Math.Min(ControlPoint1.Y, ControlPoint2.Y));
                int x2 = Math.Max(Math.Max(StartPoint.X, EndPoint.X), Math.Max(ControlPoint1.X, ControlPoint2.X));
                int y2 = Math.Max(Math.Max(StartPoint.Y, EndPoint.Y), Math.Max(ControlPoint1.Y, ControlPoint2.Y));

                return new Rectangle(x1, y1, x2 - x1, y2 - y1);
            }
        }

        public BezierLayoutInfo Clone()
        {
            return (BezierLayoutInfo)this.MemberwiseClone();
        }

        public Rectangle GetFullBounds()
        {
            var rect = Rectangle.Ceiling(Bounds);
            if (!TextBounds.IsEmpty)
                rect = Rectangle.Union(rect, TextBounds);
            rect = Rectangle.Union(rect, new Rectangle(0, 0, ControlPoint1.X, ControlPoint1.Y));
            rect = Rectangle.Union(rect, new Rectangle(0, 0, ControlPoint2.X, ControlPoint2.Y));

            return rect;
        }

        public Point[] GetPoints()
        {
            return new Point[]{
                StartPoint,
                ControlPoint1,
                ControlPoint2,
                EndPoint
            };
        }
    }
}
