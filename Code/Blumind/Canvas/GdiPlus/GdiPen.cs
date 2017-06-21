using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Blumind.Canvas.GdiPlus
{
    class GdiPen : IPen
    {
        public Pen Pen { get; private set; }

        public GdiPen(Pen pen)
        {
            Pen = pen;
        }

        public object Raw
        {
            get { return Pen; }
        }

        public Color Color
        {
            get { return Pen.Color; }
        }

        public float Width
        {
            get { return Pen.Width; }
        }
    }
}
