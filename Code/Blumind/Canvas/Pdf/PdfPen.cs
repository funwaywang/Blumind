using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using PdfSharp.Drawing;

namespace Blumind.Canvas.Pdf
{
    class PdfPen : IPen
    {
        XPen Pen;

        public PdfPen(XPen pen)
        {
            Pen = pen;
        }

        public object Raw
        {
            get { return Pen; }
        }

        public Color Color
        {
            get { return Pen.Color.ToGdiColor(); }
        }

        public float Width
        {
            get { return (float)Pen.Width; }
        }
    }
}
