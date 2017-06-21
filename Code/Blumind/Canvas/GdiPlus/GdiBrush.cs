using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Blumind.Canvas.GdiPlus
{
    class GdiBrush : IBrush
    {
        public Brush Brush { get; private set; }

        public GdiBrush(Brush brush)
        {
            Brush = brush;
        }

        public object Raw
        {
            get { return Brush; }
        }
    }
}
