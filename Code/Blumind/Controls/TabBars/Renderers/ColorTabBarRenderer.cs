using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace Blumind.Controls
{
    class ColorTabBarRenderer : NormalTabBarRenderer
    {
        public ColorTabBarRenderer(TabBar bar)
            : base(bar)
        {
        }

        protected override void DrawItemBackground(TabItemPaintEventArgs e)
        {
            Rectangle rect = e.Bounds;

            //
            Color backColor;
            if (e.Selected)
            {
                backColor = e.Bar.SelectedItemBackColor;

                // 把BaseLineSize加回去
                switch (e.Bar.Alignment)
                {
                    case TabAlignment.Left:
                        rect.Width += e.Bar.BaseLineSize;
                        break;
                    case TabAlignment.Top:
                        rect.Height += e.Bar.BaseLineSize;
                        break;
                    case TabAlignment.Right:
                        rect.X -= e.Bar.BaseLineSize;
                        rect.Width += e.Bar.BaseLineSize;
                        break;
                    case TabAlignment.Bottom:
                        rect.Y -= e.Bar.BaseLineSize;
                        rect.Height += e.Bar.BaseLineSize;
                        break;
                }
            }
            else if (e.Hover)
                backColor = e.Bar.HoverItemBackColor;
            else
                backColor = e.Bar.ItemBackColor;
            e.Graphics.FillRectangle(new SolidBrush(backColor), rect);

            //
            if (e.Item.BackColor.HasValue)
            {
                Rectangle rectC;
                switch (e.Bar.Alignment)
                {
                    case TabAlignment.Left:
                        rectC = new Rectangle(rect.X, rect.Y, Math.Min(3, rect.Width / 3), rect.Height);
                        break;
                    case TabAlignment.Right:
                        rectC = new Rectangle(rect.X, rect.Y, Math.Min(3, rect.Width / 3), rect.Height);
                        rect.X = rect.Right - rectC.Width - 1;
                        break;
                    case TabAlignment.Bottom:
                        rectC = new Rectangle(rect.X, rect.Y, rect.Width, Math.Min(3, rect.Height / 3));
                        rectC.Y = rect.Bottom - rectC.Height - 1;
                        break;
                    case TabAlignment.Top:
                    default:
                        rectC = new Rectangle(rect.X, rect.Y, rect.Width, Math.Min(3, rect.Height / 3));
                        break;
                }

                if (rectC.Height > 0)
                {
                    e.Graphics.FillRectangle(new SolidBrush(e.Item.BackColor.Value), rectC);
                }
            }

            if (!e.Bar.BaseLineColor.IsEmpty)
            {
                DrawItemBorder(e, rect, e.Bar.BaseLineColor);
                //Point[] points = new Point[] { 
                //    new Point(rect.X, rect.Bottom),
                //    new Point(rect.X, rect.Top),
                //    new Point(rect.Right-1, rect.Top),
                //    new Point(rect.Right-1, rect.Bottom),
                //};

                //PixelOffsetMode pom = e.Graphics.PixelOffsetMode;
                //e.Graphics.PixelOffsetMode = PixelOffsetMode.Default;
                //e.Graphics.DrawLines(new Pen(e.Bar.BaseLineColor), points);
                //e.Graphics.PixelOffsetMode = pom;
                ////e.Graphics.DrawPath(new Pen(Bar.BaseLineColor), path);
            }
        }
    }
}
