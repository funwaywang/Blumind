using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Blumind.Canvas;
using Blumind.Core;
using Blumind.Model;
using Blumind.Model.MindMaps;
using Blumind.Model.Styles;
using PdfSharp.Drawing;

namespace Blumind.ChartControls.Shapes
{
    abstract class Shape : System.IDisposable
    {
        public virtual bool IsClosed
        {
            get { return true; }
        }

        public abstract void Fill(IGraphics graphics, IBrush brush, Rectangle rect);

        public abstract void DrawBorder(IGraphics graphics, IPen pen, Rectangle rect);

        public virtual void DrawOutBox(IGraphics graphics, IPen pen, Rectangle rect, int inflate)
        {
            rect.Inflate(inflate, inflate);
            DrawBorder(graphics, pen, rect);
        }

        public void DrawIcon(PaintEventArgs e)
        {
            Rectangle rect = e.ClipRectangle;
            rect.Inflate(-2, -2);
            if (rect.Width > 0 && rect.Height > 0)
            {
                var graphics = new Blumind.Canvas.GdiPlus.GdiGraphics(e.Graphics);
                Fill(graphics, graphics.SolidBrush(Color.Gray), rect);
                DrawBorder(graphics, graphics.Pen(Color.Black), rect);
            }
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
        }

        #endregion

        public static Shape GetShaper(Topic topic)
        {
            return GetShaper(topic.Style.Shape, topic.Style.RoundRadius);
        }

        public static Shape GetShaper(TopicShape shape, int round)
        {
            switch (shape)
            {
                case TopicShape.Ellipse:
                    return new EllipseShape();
                case TopicShape.BaseLine:
                    return new BaseLineShape();
                case TopicShape.None:
                    return new NoneShape();
                case TopicShape.Rectangle:
                default:
                    if (round > 0)
                        return new RoundRectangleShape(round);
                    else
                        return new RectangleShape();
            }
        }

        //public virtual XmlElement GenerateSvg(Rectangle rect, XmlElement parentNode, Color borderColor, Color backColor)
        //{
        //    return null;
        //}

        public Point GetBorderPoint(Rectangle rect, Point ptd)
        {
            Point ptc = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

            if (ptd.X == ptc.X)
            {
                if (ptd.Y < ptc.Y)
                    return new Point(ptc.X, ptc.Y - rect.Height / 2);
                else if (ptd.Y > ptc.Y)
                    return new Point(ptc.X, ptc.Y + rect.Height / 2);
                else
                    return ptc;
            }
            else if (ptd.Y == ptc.Y)
            {
                if (ptd.X < ptc.X)
                    return new Point(ptc.X - rect.Width / 2, ptc.Y);
                else if (ptd.X > ptc.X)
                    return new Point(ptc.X + rect.Width / 2, ptc.Y);
                else
                    return ptc;
            }
            else
            {
                Point ptd2 = ptd;
                if (ptd.Y < ptc.Y)
                    ptd2.Y = ptc.Y + (ptc.Y - ptd2.Y);
                if (ptd.X < ptc.X)
                    ptd2.X = ptc.X + (ptc.X - ptd2.X);
                
                float r1 = (float)rect.Height / rect.Width;
                float r2 = (float)Math.Abs(ptd2.Y - ptc.Y) / Math.Abs(ptd2.X - ptc.X);
                Point pt = Point.Empty;
                if (r1 > r2)
                    pt = new Point(rect.Width / 2, (ptd2.Y - ptc.Y) * (rect.Width / 2) / (ptd2.X - ptc.X));
                else if (r1 < r2)
                    pt = new Point((ptd2.X - ptc.X) * (rect.Height / 2) / (ptd2.Y - ptc.Y), rect.Height / 2);
                else
                    pt = new Point(rect.Width / 2, rect.Height / 2);

                if (ptd.X < ptc.X)
                    pt.X = ptc.X - pt.X;
                else
                    pt.X = ptc.X + pt.X;

                if (ptd.Y < ptc.Y)
                    pt.Y = ptc.Y - pt.Y;
                else
                    pt.Y = ptc.Y + pt.Y;

                return pt;
            }
        }
    }
}
