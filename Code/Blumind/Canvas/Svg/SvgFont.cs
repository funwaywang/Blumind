using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using Blumind.Core;

namespace Blumind.Canvas.Svg
{
    class SvgFont : IFont
    {
        Font Font;

        public SvgFont(Font font)
        {
            Font = font;
        }

        public SvgFont(string fontFamily, float size, FontStyle fontStyle)
        {
            Font = new Font(fontFamily, size, fontStyle);
        }

        public SvgFont(Font font, FontStyle fontStyle)
        {
            Font = new Font(font, fontStyle);
        }

        public object Raw
        {
            get { return Font; }
        }

        public string FontFamily
        {
            get { return Font.FontFamily.Name; }
        }

        public FontStyle Style
        {
            get { return Font.Style; }
        }

        public float Size
        {
            get { return Font.Size; }
        }

        public Font GdiFont
        {
            get { return Font; }
        }

        public void Render(XmlElement element)
        {
            if (Font == null)
                return;

            element.SetAttribute("font-family", FontFamily);
            element.SetAttribute("font-size", string.Format("{0}pt", Font.SizeInPoints.ToString()));

            if ((Style & FontStyle.Italic) == FontStyle.Italic)
                element.SetAttribute("font-style", "italic");

            if ((Style & FontStyle.Bold) == FontStyle.Bold)
                element.SetAttribute("font-weight", "bold");

            string decoration = string.Empty;
            if ((Style & FontStyle.Underline) == FontStyle.Underline)
            {
                decoration += "underline";
            }
            if ((Style & FontStyle.Strikeout) == FontStyle.Strikeout)
            {
                if (decoration != null)
                    decoration += " ";
                decoration += "line-through";
            }
            if (decoration != string.Empty)
            {
                element.SetAttribute("text-decoration", decoration);
            }            
        }
    }
}
