using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Blumind.Core
{
    public interface ITextObject
    {
        string Text { get; set; }

        Font Font { get; }

        Rectangle Bounds { get; }
    }
}
