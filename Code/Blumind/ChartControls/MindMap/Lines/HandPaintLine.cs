using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Xml;
using Blumind.Canvas;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Model.Styles;

namespace Blumind.ChartControls.MindMap.Lines
{
    class HandPaintLine : ILine
    {
        Random random = new Random();
        double ArrowBase, rand1, rand2;
        bool _IsRoot;

        public bool IsRoot
        {
            get { return _IsRoot; }
            set { _IsRoot = value; }
        }

        public static HandPaintLine Default = new HandPaintLine();

        public HandPaintLine()
        {
            ArrowBase = 7.0;
            rand1 = 0.4;// random.NextDouble();
            rand2 = 0.4;// random.NextDouble();
        }

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

            var path = graphics.GraphicsPath();
            path.StartFigure();
            path.AddBezier(pts[0], pts[1], pts[2], pts[3]);
            path.AddLine(pts[3], pts[4]);
            path.AddBezier(pts[4], pts[5], pts[6], pts[7]);
            path.CloseFigure();

            graphics.FillPath(graphics.SolidBrush(pen.Color), path);
        }

        /*public void GeneralSvg(XmlElement parentNode, Rectangle rectFrom, Rectangle rectTo, Vector4 vectorFrom, Vector4 vectorTo, Color color)
        {
            Point[] pts = GetPoints(rectFrom, rectTo, vectorFrom, vectorTo);
            if (pts == null)
                return;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("M{0},{1} ", pts[0].X, pts[0].Y);
            sb.AppendFormat("C{0},{1} {2},{3} {4},{5}", pts[1].X, pts[1].Y, pts[2].X, pts[2].Y, pts[3].X, pts[3].Y);

            XmlElement path = parentNode.OwnerDocument.CreateElement("path");
            path.SetAttribute("d", sb.ToString());
            path.SetAttribute("fill", "none");
            parentNode.AppendChild(path);

            if (!color.IsEmpty)
                path.SetAttribute("stroke", ST.ToString(color));
        }*/

        private Point[] GetPoints(Rectangle rectFrom, Rectangle rectTo, Vector4 vector, Vector4 vectorTo)
        {
            Point org = PaintHelper.CenterPoint(rectFrom);
            Point to = PaintHelper.CenterPoint(rectTo);

            double dis = IsRoot ? ArrowBase : ArrowBase / 3;

            //
            Point startPoint = GetOffsetPoint(org, to, dis);
            Point point4 = GetRandomPoint(org, to, 0.5, this.rand1);
            Point point5 = GetRandomPoint(to, org, 0.5, this.rand2);
            Point point6 = GetOffsetPoint(org, to, -dis);
            Point point7 = GetOffsetPoint(point4, to, -5.0);
            Point pt = GetOffsetPoint(to, org, -2.0);
            Point pt2 = GetOffsetPoint(to, org, 1.0);

            //
            return new Point[] { 
                startPoint,
                point4,
                point5,
                pt,
                pt2,
                point5,
                point7,
                point6,
            };
        }

        #endregion

        private static Point GetOffsetPoint(Point org, Point to, double dist)
        {
            double d = Math.Atan2(to.Y - org.Y, to.X - org.X) + 1.5707963267948966;
            double num2 = Math.Cos(d) * dist;
            double num3 = Math.Sin(d) * dist;
            return new Point((int)Math.Round(org.X + num2), (int)Math.Round(org.Y + num3));
        }

        private static Point GetRandomPoint(Point from, Point to, double scale, double rand)
        {
            double x = to.X - from.X;
            double y = to.Y - from.Y;
            double num3 = Math.Sqrt((x * x) + (y * y));
            double d = Math.Atan2(y, x) + (0.52359877559829882 * (rand - 0.5));
            double num6 = Math.Cos(d) * num3;
            double num7 = Math.Sin(d) * num3;
            return new Point((int)Math.Round(from.X + (num6 * scale)), (int)Math.Round(from.Y + (num7 * scale)));
        }

        private GraphicsPath CreatePath(Point[] points)
        {
            GraphicsPath path = new GraphicsPath();

            if (points != null && points.Length >= 4)
            {
                Point[] newPoints = new Point[points.Length * 2];
                int pi = 0;
                for (int i = 0; i < points.Length; i++, pi++)
                {
                    newPoints[pi] = points[i];
                }

                for (int i = points.Length - 1; i >= 0; i--, pi++)
                {
                    newPoints[pi] = points[i];
                }

                int ls = 10;
                newPoints[0] = GetNewPoint(newPoints[0], newPoints[3], -ls);
                //newPoints[2] = GetNewPoint(newPoints[2], newPoints[3], -ls / 2);
                newPoints[3] = GetNewPoint(newPoints[3], newPoints[0], -1);
                newPoints[newPoints.Length - 1] = GetNewPoint(newPoints[newPoints.Length - 1], newPoints[newPoints.Length - 4], ls);
                //newPoints[newPoints.Length - 3] = GetNewPoint(newPoints[newPoints.Length - 3], newPoints[newPoints.Length - 4], ls / 2);
                newPoints[newPoints.Length - 4] = GetNewPoint(newPoints[newPoints.Length - 4], newPoints[newPoints.Length - 1], 1);
                //PaintPoints[0] = GetNewPoint(PaintPoints[0], PaintPoints[1], PaintPoints[2], -ls);
                //PaintPoints[PaintPoints.Length - 1] = GetNewPoint(PaintPoints[PaintPoints.Length - 1], PaintPoints[1], PaintPoints[2], ls);

                path.AddBezier(newPoints[0], newPoints[1], newPoints[2], newPoints[3]);
                path.AddBezier(newPoints[4], newPoints[5], newPoints[6], newPoints[7]);
                path.CloseFigure();
            }

            return path;
        }

        private Point GetNewPoint(Point p1, Point p2, int size)
        {
            if (p1.X == p2.X)
            {
                return new Point(p1.X + size, p1.Y);
            }
            else if (p1.Y == p2.Y)
            {
                return new Point(p1.X, p1.Y + size);
            }
            else
            {
                double alpha = Math.Atan((double)(p2.Y - p1.Y) / (double)(p2.X - p1.X));
                return new Point(
                    (int)Math.Round(p1.X + Math.Sin(alpha) * size),
                    (int)Math.Round(p1.Y - Math.Cos(alpha) * size));
            }
            //return new Point(po.X + size, po.Y + size);
        }
    }
}
