using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blumind.Canvas
{
    interface IGraphicsPath
    {
        object Raw { get; }

        void StartFigure();

        void CloseFigure();

        void AddBezier(System.Drawing.Point point1, System.Drawing.Point point2, System.Drawing.Point point3, System.Drawing.Point point4);

        void AddLine(System.Drawing.Point point1, System.Drawing.Point point2);
    }
}
