using System.Drawing;
using System.Xml;
using Blumind.Canvas;
using Blumind.Core;

namespace Blumind.ChartControls.Shapes
{
    class RectangleShape : Shape
    {
        public override void Fill(IGraphics graphics, IBrush brush, Rectangle rect)
        {
            graphics.FillRectangle(brush, rect);
        }

        public override void DrawBorder(IGraphics graphics, IPen pen, Rectangle rect)
        {
            graphics.DrawRectangle(pen, rect.Left, rect.Top, rect.Width, rect.Height);
        }

        //public override XmlElement GenerateSvg(Rectangle rect, XmlElement parentNode, Color borderColor, Color backColor)
        //{
        //    XmlElement shape = parentNode.OwnerDocument.CreateElement("rect");
        //    shape.SetAttribute("x", rect.X.ToString());
        //    shape.SetAttribute("y", rect.Y.ToString());
        //    shape.SetAttribute("width", rect.Width.ToString());
        //    shape.SetAttribute("height", rect.Height.ToString());
        //    shape.SetAttribute("fill", backColor.IsEmpty ? "none" : ST.ToString(backColor));
        //    shape.SetAttribute("stroke", borderColor.IsEmpty ? "none" : ST.ToString(borderColor));
        //    parentNode.AppendChild(shape);

        //    return shape;
        //}
    }
}
