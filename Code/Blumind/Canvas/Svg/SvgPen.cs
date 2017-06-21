using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using Blumind.Core;

namespace Blumind.Canvas.Svg
{
    class SvgPen : IPen
    {
        public SvgPen()
        {
            Color = Color.Black;
            Width = 1.0f;
            DashStyle = DashStyle.Solid;
        }

        public SvgPen(Color color)
            : this()
        {
            Color = color;
        }

        public SvgPen(Color color, float width)
            : this()
        {
            Color = color;
            Width = width;
        }

        public object Raw
        {
            get { return this; }
        }

        public Color Color { get; set; }

        public float Width { get; set; }

        public DashStyle DashStyle { get; set; }

        public float[] DashPattern { get; set; }

        public string Render(System.Xml.XmlElement element)
        {
            if (Width != 1)
                element.SetAttribute("stroke-width", Width.ToString());

            if (DashStyle != DashStyle.Solid)
            {
                var uw = Math.Max(1, (int)Math.Round(Width));
                float[] pattern = null;
                switch (DashStyle)
                {
                    case DashStyle.Custom:
                        pattern = DashPattern;
                        break;
                    case DashStyle.Dash:
                        pattern = new float[] { uw * 3, uw };
                        break;
                    case DashStyle.DashDot:
                        pattern = new float[] { uw * 3, uw, uw, uw };
                        break;
                    case DashStyle.DashDotDot:
                        pattern = new float[] { uw * 3, uw, uw, uw, uw, uw };
                        break;
                    case DashStyle.Dot:
                        pattern = new float[] { uw, uw };
                        break;
                }
                if (!pattern.IsNullOrEmpty())
                {
                    element.SetAttribute("stroke-dasharray", pattern.JoinString(" "));
                }
            }

            return ST.ToWebColor(Color);
        }
    }
}
