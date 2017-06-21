using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace Blumind.Canvas.GdiPlus
{
    class GdiGraphicsPath : IGraphicsPath
    {
        public GraphicsPath Path { get; private set; }

        public GdiGraphicsPath(GraphicsPath path)
        {
            Path = path;
        }

        public object Raw
        {
            get { return Path; }
        }

        public void StartFigure()
        {
            Path.StartFigure();
        }

        public void CloseFigure()
        {
            Path.CloseFigure();
        }

        public void AddBezier(Point startPoint, Point controlPoint1, Point controlPoint2, Point endPoint)
        {
            Path.AddBezier(startPoint, controlPoint1, controlPoint2, endPoint);
        }

        public void AddLine(Point point1, Point point2)
        {
            Path.AddLine(point1, point2);
        }
    }
}
