using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;

namespace Blumind.Canvas.Svg
{
    class SvgGraphicsState : IGraphicsState
    {
        public SvgGraphicsState()
        {
        }

        public object Raw
        {
            get { return this; }
        }

        public PointF Translation { get; set; }

        public PointF Scaling { get; set; }

        public float Rotation { get; set; }

        public void Render(XmlElement element)
        {
            var sb = new StringBuilder();

            if(Translation != PointF.Empty)
            {
                sb.AppendFormat("translate({0} {1})", Translation.X, Translation.Y);
            }

            if (Scaling != PointF.Empty)
            {
                if (sb.Length > 0)
                    sb.Append(" ");
                sb.AppendFormat("scale({0} {1})", Scaling.X, Scaling.Y);
            }

            if(Rotation != 0.0f)
            {
                if(sb.Length > 0)
                    sb.Append(" ");
                sb.AppendFormat("rotate({0})", Rotation);
            }

            if (sb.Length > 0)
            {
                element.SetAttribute("transform", sb.ToString());
            }
        }

        public SvgGraphicsState Copy()
        {
            return (SvgGraphicsState)this.MemberwiseClone();
        }

        public void CopyFrom(IGraphicsState state)
        {
            if (state is SvgGraphicsState)
            {
                var ss = (SvgGraphicsState)state;
                Translation = ss.Translation;
                Scaling = ss.Scaling;
                Rotation = ss.Rotation;
            }
        }
    }
}
