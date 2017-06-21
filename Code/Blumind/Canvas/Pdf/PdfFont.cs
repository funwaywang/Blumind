using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using PdfSharp.Drawing;

namespace Blumind.Canvas.Pdf
{
    class PdfFont : IFont
    {
        XFont Font;

        public PdfFont(XFont font)
        {
            Font = font;
        }

        public object Raw
        {
            get { return Font; }
        }

        public FontStyle Style
        {
            get 
            {
                return (FontStyle)(int)Font.Style; 
            }
        }

        public float Size
        {
            get { return (float)Font.Size; }
        }
    }
}
