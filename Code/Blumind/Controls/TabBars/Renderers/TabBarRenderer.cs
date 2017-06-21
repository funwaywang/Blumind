using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace Blumind.Controls
{
    abstract class TabBarRenderer
    {
        public TabBarRenderer(TabBar bar)
        {
            Bar = bar;
        }

        public TabBar Bar { get; private set; }

        public abstract void DrawBackground(PaintEventArgs e);

        public abstract void DrawItem(TabItemPaintEventArgs e);

        protected abstract void DrawButton(PaintEventArgs e, TabBarButton button, UIControlStatus ucs);

        public abstract void DrawBaseLine(PaintEventArgs e);

        protected abstract void DrawCloseButton(TabItemPaintEventArgs e, Rectangle rect);

        public void DrawButtons(PaintEventArgs e, TabBarButton[] buttons)
        {
            if (buttons == null || buttons.Length == 0)
                return;

            foreach (TabBarButton button in buttons)
            {
                if (!button.Visible)
                    continue;

                UIControlStatus ucs = UIControlStatus.Normal;
                if (button.Index > -1)
                {
                    if (button.Index == Bar.DownHitResult.ButtonIndex)
                        ucs = UIControlStatus.Selected;
                    else if (button.Index == Bar.HoverHitResult.ButtonIndex)
                        ucs = UIControlStatus.Hover;
                }

                if (button.OwnerDraw)
                    DrawButton(e, button, ucs);
                else
                    button.Draw(e, ucs);
            }
        }

        public virtual void Initialize(PaintEventArgs e)
        {
            //Bar = bar;
        }

        public virtual void Close()
        {
        }
    }
}
