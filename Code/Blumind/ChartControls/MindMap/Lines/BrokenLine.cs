using System.Drawing;
using System.Xml;
using System.Text;
using Blumind.Core;
using Blumind.Canvas;
using Blumind.Controls;
using Blumind.Model.Styles;

namespace Blumind.ChartControls.MindMap.Lines
{
    class BrokenLine : ILine
    {
        public void DrawLine(IGraphics graphics, IPen pen,
            TopicShape shapeFrom, TopicShape shapeTo, 
            Rectangle rectFrom, Rectangle rectTo, 
            Vector4 vectorFrom, Vector4 vectorTo,
            LineAnchor startAnchor, LineAnchor endAnchor)
        {
            Point[] pts = GetPoints(rectFrom, rectTo, vectorFrom, vectorTo);
            if (pts == null)
                return;

            graphics.DrawLines(pen, pts, startAnchor, endAnchor);
        }

        Point[] GetPoints(Rectangle rectFrom, Rectangle rectTo, Vector4 vectorFrom, Vector4 vectorTo)
        {
            Point ptF = PaintHelper.CenterPoint(rectFrom);
            Point ptT = PaintHelper.CenterPoint(rectTo);

            Point[] pts = new Point[4];

            switch (vectorFrom)
            {
                case Vector4.Left:
                    pts[0] = new Point(rectFrom.Left, ptF.Y);
                    pts[1] = new Point(rectFrom.Left - (rectFrom.Left - rectTo.Right) / 2, ptF.Y);
                    pts[2] = new Point(rectFrom.Left - (rectFrom.Left - rectTo.Right) / 2, ptT.Y);
                    pts[3] = new Point(rectTo.Right, ptT.Y);
                    break;
                case Vector4.Top:
                    pts[0] = new Point(ptF.X, rectFrom.Top);
                    pts[1] = new Point(ptF.X, rectFrom.Top - (rectFrom.Top - rectTo.Bottom) / 2);
                    pts[2] = new Point(ptT.X, rectFrom.Top - (rectFrom.Top - rectTo.Bottom) / 2);
                    pts[3] = new Point(ptT.X, rectTo.Bottom);
                    break;
                case Vector4.Right:
                    pts[0] = new Point(rectFrom.Right, ptF.Y);
                    pts[1] = new Point(rectFrom.Right + (rectTo.Left - rectFrom.Right) / 2, ptF.Y);
                    pts[2] = new Point(rectFrom.Right + (rectTo.Left - rectFrom.Right) / 2, ptT.Y);
                    pts[3] = new Point(rectTo.Left, ptT.Y);
                    break;
                case Vector4.Bottom:
                    pts[0] = new Point(ptF.X, rectFrom.Bottom);
                    switch (vectorTo)
                    {
                        case Vector4.Left:
                            pts[1] = new Point(ptF.X, ptT.Y);
                            pts[2] = pts[1];
                            pts[3] = new Point(rectTo.Left, ptT.Y);
                            break;
                        case Vector4.Right:
                            pts[1] = new Point(ptF.X, ptT.Y);
                            pts[2] = pts[1];
                            pts[3] = new Point(rectTo.Right, ptT.Y);
                            break;
                        default:
                            pts[1] = new Point(ptF.X, rectFrom.Bottom + (rectTo.Top - rectFrom.Bottom) / 2);
                            pts[2] = new Point(ptT.X, rectFrom.Bottom + (rectTo.Top - rectFrom.Bottom) / 2);
                            pts[3] = new Point(ptT.X, rectTo.Top);
                            break;
                    }
                    break;
                default:
                    break;
            }

            return pts;
        }
    }
}
