using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using Blumind.Canvas;
using Blumind.Controls;
using Blumind.Core;

namespace Blumind.ChartControls.Shapes
{
    class RoundRectangleShape : Shape
    {
        GraphicsPath Path;
        int Round = 5;
        Rectangle Rect;

        public RoundRectangleShape()
        {
        }

        public RoundRectangleShape(int round)
        {
            Round = round;
        }

        void CreatePath(Rectangle rect)
        {
            if (Path != null)
                Path.Dispose();

            Rect = rect;

            Path = PaintHelper.GetRoundRectangle(rect, Round);
        }

        public override void Fill(IGraphics graphics, IBrush brush, Rectangle rect)
        {
            graphics.FillRoundRectangle(brush, rect, Round);
        }

        public override void DrawBorder(IGraphics graphics, IPen pen, Rectangle rect)
        {
            //if (Path == null || Rect != rect)
            //    CreatePath(rect);
            //graphics.DrawPath(pen, Path);
            graphics.DrawRoundRectangle(pen, rect, Round);
        }

        public override void DrawOutBox(IGraphics graphics, IPen pen, Rectangle rect, int inflate)
        {
            rect.Inflate(inflate, inflate);

            //using (GraphicsPath gpOut = PaintHelper.GetRoundRectangle(rect, Round + inflate))
            //{
            //    graphics.DrawPath(pen, gpOut);
            //    gpOut.Dispose();
            //}
            graphics.DrawRoundRectangle(pen, rect, Round + inflate);
        }

        public override void Dispose()
        {
            base.Dispose();

            if (Path != null)
                Path.Dispose();
        }

        //public override XmlElement GenerateSvg(Rectangle rect, XmlElement parentNode, Color borderColor, Color backColor)
        //{
        //    XmlElement shape = parentNode.OwnerDocument.CreateElement("rect");
        //    shape.SetAttribute("x", rect.X.ToString());
        //    shape.SetAttribute("y", rect.Y.ToString());
        //    shape.SetAttribute("width", rect.Width.ToString());
        //    shape.SetAttribute("height", rect.Height.ToString());
        //    shape.SetAttribute("rx", Round.ToString());
        //    shape.SetAttribute("ry", Round.ToString());
        //    shape.SetAttribute("fill", backColor.IsEmpty ? "none" : ST.ToString(backColor));
        //    shape.SetAttribute("stroke", borderColor.IsEmpty ? "none" : ST.ToString(borderColor));
        //    parentNode.AppendChild(shape);

        //    return shape;
        //}
    }
}
