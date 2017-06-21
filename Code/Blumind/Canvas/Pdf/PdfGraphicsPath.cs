using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using PdfSharp.Drawing;

namespace Blumind.Canvas.Pdf
{
    class PdfGraphicsPath : IGraphicsPath
    {
        XGraphicsPath Path;

        public PdfGraphicsPath(XGraphicsPath path)
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

        public void AddBezier(Point point1, Point point2, Point point3, Point point4)
        {
            Path.AddBezier(point1, point2, point3, point4);
        }

        public void AddLine(Point point1, Point point2)
        {
            Path.AddLine(point1, point2);
        }
    }
}
