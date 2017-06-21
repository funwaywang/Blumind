using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Blumind.Controls
{
    class StartMenuButton : TabBarButton
    {
        public StartMenuButton()
        {
            CustomSize = 80;
            OwnerDraw = false;
        }

        protected override void OnPaint(PaintEventArgs e, UIControlStatus ucs)
        {
            //base.OnPaint(e, ucs);
            Rectangle rect = Bounds;
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            Color backColor;
            Color foreColor;
            if (IsDroppingDown)
            {
                backColor = Owner.HoverItemBackColor;// UITheme.Default.Colors.MenuImageMargin;
                foreColor = Owner.HoverItemForeColor;// Color.Black;
            }
            else
            {
                switch (ucs)
                {
                    case UIControlStatus.Selected:
                    case UIControlStatus.Focused:
                        foreColor = Owner.SelectedItemForeColor;
                        backColor = Owner.SelectedItemBackColor;
                        break;
                    case UIControlStatus.Hover:
                        foreColor = Owner.HoverItemForeColor;
                        backColor = Owner.HoverItemBackColor;
                        break;
                    default:
                        foreColor = UITheme.Default.Colors.SharpText;
                        backColor = UITheme.Default.Colors.Sharp;
                        break;
                }
            }

            //
            e.Graphics.FillRectangle(new SolidBrush(backColor), Bounds);
            //e.Graphics.DrawRectangle(new Pen(UITheme.Default.Colors.MediumDark), rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

            if (!string.IsNullOrEmpty(Text))
            {

                Font font = new Font(Owner.Font, FontStyle.Bold);
                StringFormat sf = PaintHelper.SFCenter;
                sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Hide;
                e.Graphics.DrawString(Text, font, new SolidBrush(foreColor), rect, sf);
            }
        }
    }
}
