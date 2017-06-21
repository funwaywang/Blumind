using System;
using System.Drawing;
using System.Xml;
using Blumind.Canvas;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Model.Styles;

namespace Blumind.ChartControls.MindMap.Lines
{
    class StraightLine : ILine
    {
        #region ILine Members

        public void DrawLine(IGraphics graphics, IPen pen,
            TopicShape shapeFrom, TopicShape shapeTo, 
            Rectangle rectFrom, Rectangle rectTo, 
            Vector4 vectorFrom, Vector4 vectorTo,
            LineAnchor startAnchor, LineAnchor endAnchor)
        {
            Point[] pts = GetPoints(rectFrom, rectTo, vectorFrom, vectorTo);
            if (pts == null)
                return;

            graphics.DrawLine(pen, pts[0], pts[1], startAnchor, endAnchor);
        }

        #endregion

        private Point[] GetPoints(Rectangle rectFrom, Rectangle rectTo, Vector4 vectorFrom, Vector4 vectorTo)
        {
            Point p1 = new Point(rectFrom.X + rectFrom.Width / 2, rectFrom.Y + rectFrom.Height / 2);
            Point p2 = new Point(rectTo.X + rectTo.Width / 2, rectTo.Y + rectTo.Height / 2);

            if (p1 == p2)
                return null;

            Point p3, p4;
            bool inversion = false;
            if (p1.X == p2.X)
            {
                if (p1.Y > p2.Y)
                {
                    p3 = new Point(p1.X, rectFrom.Top - 4);
                    p4 = new Point(p2.X, p2.Y + rectTo.Height / 2 + 4);
                }
                else
                {
                    p3 = new Point(p1.X, p1.Y + rectFrom.Height / 2 + 4);
                    p4 = new Point(p2.X, rectTo.Top - 4);
                }
            }
            else if (p1.Y == p2.Y)
            {
                if (p1.X > p2.X)
                {
                    p3 = new Point(rectFrom.Left - 4, p2.Y);
                    p4 = new Point(p2.X + rectTo.Width / 2 + 4, p2.Y);
                }
                else
                {
                    p3 = new Point(p1.X + rectFrom.Width / 2 + 4, p1.Y);
                    p4 = new Point(rectTo.Left - 4, p2.Y);
                }
            }
            else
            {
                if (p2.Y < p1.Y)
                {
                    Point pt = p2;
                    p2 = p1;
                    p1 = pt;
                    Rectangle rectT = rectTo;
                    rectTo = rectFrom;
                    rectFrom = rectT;
                    inversion = true;
                }

                int w1 = rectFrom.Width / 2;
                int h1 = rectFrom.Height / 2;
                int w2 = rectTo.Width / 2;
                int h2 = rectTo.Height / 2;

                if (p1.X < p2.X) // rect2 is right-bottom of rect1
                {
                    if (w1 > 0 && (decimal)(p2.Y - p1.Y) / h1 >= (decimal)(p2.X - p1.X) / w1) // rect2 is bottom side
                    {
                        p3 = new Point(p1.X + (int)Math.Ceiling((decimal)(p2.X - p1.X) * h1 / (p2.Y - p1.Y)), p1.Y + h1);
                        p4 = new Point(p2.X - (int)Math.Ceiling((decimal)(p2.X - p1.X) * h2 / (p2.Y - p1.Y)), p2.Y - h2);
                    }
                    else // rect2 is right side
                    {
                        p3 = new Point(p1.X + w1, p1.Y + (int)Math.Ceiling((decimal)(p2.Y - p1.Y) * w1 / (p2.X - p1.X)));
                        p4 = new Point(p2.X - w2, p2.Y - (int)Math.Ceiling((decimal)(p2.Y - p1.Y) * w2 / (p2.X - p1.X)));
                    }
                }
                else // rect2 is left-bottom of rect1
                {
                    if (w1 > 0 && (decimal)(p2.Y - p1.Y) / h1 >= (decimal)(p1.X - p2.X) / w1) // rect2 is bottom side
                    {
                        p3 = new Point(p1.X - (int)Math.Ceiling((decimal)(p1.X - p2.X) * h1 / (p2.Y - p1.Y)), p1.Y + h1);
                        p4 = new Point(p2.X + (int)Math.Ceiling((decimal)(p1.X - p2.X) * h2 / (p2.Y - p1.Y)), p2.Y - h2);
                    }
                    else // rect2 is right side
                    {
                        p3 = new Point(p1.X - w1, p1.Y - (int)Math.Ceiling((decimal)(p2.Y - p1.Y) * w1 / (p2.X - p1.X)));
                        p4 = new Point(p2.X + w2, p2.Y + (int)Math.Ceiling((decimal)(p2.Y - p1.Y) * w2 / (p2.X - p1.X)));
                    }
                }
            }

            if (inversion)
                return new Point[] { p4, p3 };
            else
                return new Point[] { p3, p4 };
        }
    }
}
