using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace Blumind.Canvas.GdiPlus
{
    class GdiGraphicsState : IGraphicsState
    {
        GraphicsState State;

        public GdiGraphicsState(GraphicsState state)
        {
            State = state;
        }

        public object Raw
        {
            get { return State; }
        }
    }
}
