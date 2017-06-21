using System.Drawing;
using System.Xml;
using Blumind.Canvas;
using Blumind.Core;

namespace Blumind.ChartControls.Shapes
{
    class BaseLineShape : Shape
    {
        public override bool IsClosed
        {
            get
            {
                return false;
            }
        }

        public override void Fill(IGraphics graphics, IBrush brush, Rectangle rect)
        {
            //graphics.FillRectangle(brush, rect);
        }

        public override void DrawBorder(IGraphics graphics, IPen pen, Rectangle rect)
        {
            //graphics.DrawLine(pen, rect.Left, rect.Bottom - 1, rect.Right - 1, rect.Bottom - 1);
            graphics.DrawLine(pen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
        }

        public override void DrawOutBox(IGraphics graphics, IPen pen, Rectangle rect, int inflate)
        {
            //base.DrawOutBox(graphics, pen, rect, inflate);

            graphics.DrawLine(pen, rect.Left, rect.Bottom + inflate, rect.Right, rect.Bottom + inflate);
            //graphics.DrawLine(pen, rect.Left, rect.Bottom + inflate - 1, rect.Right - 1, rect.Bottom + inflate - 1);
        }

        //public override XmlElement GenerateSvg(Rectangle rect, XmlElement parentNode, Color borderColor, Color backColor)
        //{
        //    XmlElement shape = parentNode.OwnerDocument.CreateElement("line");
        //    shape.SetAttribute("x1", rect.X.ToString());
        //    shape.SetAttribute("y1", rect.Bottom.ToString());
        //    shape.SetAttribute("x2", rect.Right.ToString());
        //    shape.SetAttribute("y2", rect.Bottom.ToString());
        //    shape.SetAttribute("stroke", borderColor.IsEmpty ? "none" : ST.ToString(borderColor));
        //    parentNode.AppendChild(shape);

        //    return shape;
        //}
    }
}
