using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Blumind.Canvas
{
    interface IFont
    {
        object Raw { get; }

        FontStyle Style { get; }

        float Size { get; }
    }
}
