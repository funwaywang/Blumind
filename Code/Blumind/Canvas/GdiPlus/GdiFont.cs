using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Blumind.Canvas.GdiPlus
{
    class GdiFont : IFont
    {
        public Font Font { get; private set; }

        public GdiFont(Font font)
        {
            Font = font;
        }

        public GdiFont(FontFamily fontFamily, float size)
            : this(new Font(fontFamily, size))
        {
        }

        public object Raw
        {
            get { return Font; }
        }

        public FontStyle Style
        {
            get
            {
                return Font.Style;
            }
        }

        public float Size
        {
            get { return Font.Size; }
        }
    }
}
