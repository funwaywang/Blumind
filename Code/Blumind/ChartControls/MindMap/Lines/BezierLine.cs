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
    class BezierLine : ILine
    {
        const int MaxBezierTurn = 16;
        bool _IsRoot;

        public bool IsRoot
        {
            get { return _IsRoot; }
            set { _IsRoot = value; }
        }

        #region ILine Members

        public void DrawLine(IGraphics graphics, IPen pen, 
            TopicShape shapeFrom, TopicShape shapeTo, 
            Rectangle rectFrom, Rectangle rectTo, 
            Vector4 vectorFrom, Vector4 vectorTo, 
            LineAnchor startAnchor, LineAnchor endAnchor)
        {
            Point[] pts;
            if (IsRoot)
                pts = GetPointsForRoot(shapeFrom, shapeTo, rectFrom, rectTo, vectorFrom, vectorTo);
            else
                pts = GetPoints(rectFrom, rectTo, vectorFrom, vectorTo);
            if (pts.IsNullOrEmpty())
                return;

            graphics.DrawBezier(pen, pts[0], pts[1], pts[2], pts[3], startAnchor, endAnchor);
        }

        Point[] GetPoints(Rectangle rectFrom, Rectangle rectTo, Vector4 vector, Vector4 vectorTo)
        {
            int bezierTurn = 20;

            Point fcp = PaintHelper.CenterPoint(rectFrom);
            Point tcp = PaintHelper.CenterPoint(rectTo);

            int distance = PaintHelper.GetDistance(fcp, tcp);
            //bezierTurn = distance / 5; // 20%

            Point[] pts;

            switch (vector)
            {
                case Vector4.Left:
                    distance = PaintHelper.GetDistance(new Point(rectFrom.Left, fcp.Y), new Point(rectTo.Right, tcp.Y));
                    bezierTurn = Math.Min(MaxBezierTurn, Math.Abs(rectFrom.Left - rectTo.Right));
                    pts = new Point[]{
                        new Point(rectFrom.Left, fcp.Y), 
                        new Point(rectFrom.Left - bezierTurn, fcp.Y),
                        new Point(rectTo.Right + bezierTurn, tcp.Y),
                        new Point(rectTo.Right, tcp.Y)
                    };
                    break;
                case Vector4.Top:
                    distance = PaintHelper.GetDistance(new Point(fcp.X, rectFrom.Top), new Point(tcp.X, rectTo.Bottom));
                    bezierTurn = Math.Min(MaxBezierTurn, Math.Abs(rectTo.Bottom - rectFrom.Top));
                    pts = new Point[]{
                        new Point(fcp.X, rectFrom.Top),
                        new Point(fcp.X, rectFrom.Top - bezierTurn),
                        new Point(tcp.X, rectTo.Bottom + bezierTurn),
                        new Point(tcp.X, rectTo.Bottom)
                    };
                    break;
                case Vector4.Right:
                    distance = PaintHelper.GetDistance(new Point(rectFrom.Right, fcp.Y), new Point(rectTo.Left, tcp.Y));
                    bezierTurn = Math.Min(MaxBezierTurn, Math.Abs(rectTo.Left - rectFrom.Right));//  distance / 5; // 20%
                    pts = new Point[]{
                        new Point(rectFrom.Right, fcp.Y), 
                        new Point(rectFrom.Right + bezierTurn, fcp.Y),
                        new Point(rectTo.Left - bezierTurn, tcp.Y),
                        new Point(rectTo.Left, tcp.Y)
                    };
                    break;
                case Vector4.Bottom:
                    distance = PaintHelper.GetDistance(new Point(fcp.X, rectFrom.Bottom), new Point(tcp.X, rectTo.Top));
                    bezierTurn = Math.Min(MaxBezierTurn, Math.Abs(rectFrom.Bottom - rectTo.Top));
                    pts = new Point[]{
                        new Point(fcp.X, rectFrom.Bottom),
                        new Point(fcp.X, rectFrom.Bottom - bezierTurn),
                        new Point(tcp.X, rectTo.Top + bezierTurn),
                        new Point(tcp.X, rectTo.Top)
                    };
                    break;
                default:
                    pts = null;
                    break;
            }

            return pts;
        }

        Point[] GetPointsForRoot(TopicShape shapeFrom, TopicShape shapeTo, 
            Rectangle rectFrom, Rectangle rectTo,
            Vector4 vector, Vector4 vectorTo)
        {
            var fcp = PaintHelper.CenterPoint(rectFrom);
            int bezierTurn;

            Point[] pts = new Point[4];
            switch (vector)
            {
                case Vector4.Left:
                    bezierTurn = Math.Min(MaxBezierTurn, Math.Abs(rectTo.Right - rectFrom.Left));
                    pts[3] = new Point(rectTo.Right, rectTo.Y + rectTo.Height / 2);
                    pts[2] = new Point(rectTo.Right + bezierTurn, pts[3].Y);
                    break;
                case Vector4.Right:
                    bezierTurn = Math.Min(MaxBezierTurn, Math.Abs(rectTo.Left - rectFrom.Right));
                    pts[3] = new Point(rectTo.X, rectTo.Y + rectTo.Height / 2);
                    pts[2] = new Point(rectTo.X - bezierTurn, pts[3].Y);
                    break;
                case Vector4.Top:
                    bezierTurn = Math.Min(MaxBezierTurn, Math.Abs(rectFrom.Top - rectTo.Bottom));
                    pts[3] = new Point(rectTo.X + rectTo.Width /2, rectTo.Bottom);
                    pts[2] = new Point(rectTo.X + rectTo.Width / 2, rectTo.Bottom + bezierTurn);
                    break;
                case Vector4.Bottom:
                    bezierTurn = Math.Min(MaxBezierTurn, Math.Abs(rectTo.Top - rectFrom.Bottom));
                    pts[3] = new Point(rectTo.X + rectTo.Width / 2, rectTo.Top);
                    pts[2] = new Point(rectTo.X + rectTo.Width / 2, rectTo.Top - bezierTurn);
                    break;
            }

            Point? startPoint;
            if(shapeFrom == TopicShape.Ellipse)
                startPoint = PaintHelper.GetPointForPointIntersectEllipse(rectFrom, pts[3]);
            else
                startPoint = PaintHelper.GetPointForLineIntersectRectangle(rectFrom, pts[3]);
            if (!startPoint.HasValue)
                return null;
            pts[0] = startPoint.Value;
            pts[1] = startPoint.Value;

            return pts;
        }

        #endregion
    }
}
