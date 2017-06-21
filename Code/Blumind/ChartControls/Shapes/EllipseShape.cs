using System.Drawing;
using System.Xml;
using Blumind.Canvas;
using Blumind.Core;

namespace Blumind.ChartControls.Shapes
{
    class EllipseShape : Shape
    {
        public override void Fill(IGraphics graphics, IBrush brush, Rectangle rect)
        {
            graphics.FillEllipse(brush, rect);
        }

        public override void DrawBorder(IGraphics graphics, IPen pen, Rectangle rect)
        {
            graphics.DrawEllipse(pen, rect.X, rect.Top, rect.Width, rect.Height);
        }

        //public override XmlElement GenerateSvg(Rectangle rect, XmlElement parentNode, Color borderColor, Color backColor)
        //{
        //    XmlElement shape = parentNode.OwnerDocument.CreateElement("ellipse");
        //    shape.SetAttribute("cx", (rect.X+rect.Width/2).ToString());
        //    shape.SetAttribute("cy", (rect.Y+rect.Height/2).ToString());
        //    shape.SetAttribute("rx", (rect.Width/2).ToString());
        //    shape.SetAttribute("ry", (rect.Height/2).ToString());
        //    shape.SetAttribute("fill", backColor.IsEmpty ? "none" : ST.ToString(backColor));
        //    shape.SetAttribute("stroke", borderColor.IsEmpty ? "none" : ST.ToString(borderColor));
        //    parentNode.AppendChild(shape);

        //    return shape;
        //}
    }
}
