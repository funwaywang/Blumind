using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Xml;
using Blumind.Core;

namespace Blumind.Canvas.Svg
{
    abstract class SvgBrush : IBrush
    {
        public abstract string Render(XmlElement element);

        public object Raw
        {
            get { return this; }
        }
    }

    class SvgSolidBrush : SvgBrush
    {
        public SvgSolidBrush(Color color)
        {
            Color = color;
        }

        public Color Color { get; set; }

        public override string Render(XmlElement element)
        {
            return ST.ToWebColor(Color);
        }
    }

    class SvgLinearGradientBrush : SvgBrush
    {
        public SvgLinearGradientBrush(Rectangle rect, Color color1, Color color2, float angle)
        {
            this.Rectangle = rect;
            this.Color1 = color1;
            this.Color2 = color2;
            this.Angle = angle;
        }

        public SvgLinearGradientBrush(Rectangle rect, Color color1, Color color2, LinearGradientMode linearGradientMode)
        {
            this.Rectangle = rect;
            this.Color1 = color1;
            this.Color2 = color2;
            this.GradientMode = linearGradientMode;
        }

        public Rectangle Rectangle { get; set; }

        public Color Color1 { get; set; }

        public Color Color2 { get; set; }

        public float Angle { get; private set; }

        public LinearGradientMode? GradientMode { get; private set; }

        public override string Render(XmlElement element)
        {
            throw new NotImplementedException();
        }
    }
}
