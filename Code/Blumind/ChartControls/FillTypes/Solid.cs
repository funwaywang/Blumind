using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Blumind.Canvas;

namespace Blumind.ChartControls.FillTypes
{
    class Solid : FillType
    {
        public override IBrush CreateBrush(IGraphics graphics, Color backColor, Rectangle rectangle)
        {
            return graphics.SolidBrush(backColor);
        }
    }
}
