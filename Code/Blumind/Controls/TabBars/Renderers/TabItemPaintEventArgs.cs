using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Blumind.Controls
{
    class TabItemPaintEventArgs : PaintEventArgs
    {
        public TabItemPaintEventArgs(PaintEventArgs e, TabBar tabBar, TabItem item)
            : base(e.Graphics, e.ClipRectangle)
        {
            Initialize(tabBar, item);
        }

        public TabItemPaintEventArgs(Graphics graphics, TabBar tabBar, TabItem item)
            : base(graphics, Rectangle.Empty)
        {
            Initialize(tabBar, item);
        }

        public TabBar Bar { get; private set; }

        public TabItem Item { get; private set; }

        public bool Selected { get; private set; }

        public bool Hover { get; private set; }

        public bool Enabled { get; private set; }

        public UIControlStatus Status { get; private set; }

        public UIControlStatus CloseButtonStatus { get; private set; }

        public Padding Padding { get; private set; }

        public Size IconSize { get; private set; }

        public bool HalfDisplay { get; private set; }

        public Font Font { get; private set; }

        public Rectangle Bounds
        {
            get { return Item.Bounds; }
        }

        public bool IsHorizontal
        {
            get { return Bar.IsHorizontal; }
        }

        public Rectangle ContentRectangle
        {
            get
            {
                Rectangle rect = Item.Bounds;
                if (Padding != Padding.Empty)
                {
                    if (IsHorizontal)
                    {
                        rect.X += Padding.Left;
                        rect.Y += Padding.Top;
                        rect.Width -= Padding.Horizontal;
                        rect.Height -= Padding.Vertical;
                    }
                    else
                    {
                        rect.X += Padding.Top;
                        rect.Y += Padding.Left;
                        rect.Width -= Padding.Vertical;
                        rect.Height -= Padding.Horizontal;
                    }
                }

                return rect;
            }
        }

        void Initialize(TabBar tabBar, TabItem item)
        {
            Selected = tabBar.SelectedItem == item;
            Hover = tabBar.HoverItem == item;
            Enabled = item.Enabled;

            Bar = tabBar;
            Item = item;
            Padding = tabBar.ItemPadding;
            IconSize = tabBar.IconSize;
            HalfDisplay = tabBar.IsHalfDisplay(item);
            Font = tabBar.Font;

            if (!Enabled)
            {
                Status = UIControlStatus.Disabled;
            }
            else if (Selected)
            {
                if (tabBar.Focused)
                {
                    Status = UIControlStatus.Focused;
                }
                else
                {
                    Status = UIControlStatus.Selected;
                }
            }
            else if (Hover)
            {
                Status = UIControlStatus.Hover;
            }

            //
            CloseButtonStatus = UIControlStatus.Normal;
            if (item.CanClose)
            {
                if (tabBar.DownHitResult.Item == item && tabBar.DownHitResult.InCloseButton)
                    CloseButtonStatus = UIControlStatus.Selected;
                else if (tabBar.HoverHitResult.Item == item && tabBar.HoverHitResult.InCloseButton)
                    CloseButtonStatus = UIControlStatus.Hover;
            }
        }

        public T GetBar<T>()
            where T : TabBar
        {
            if (this.Bar == null)
                return null;
            else if (this.Bar is T)
                return (T)this.Bar;
            else
                throw new InvalidCastException();
        }
    }
}
