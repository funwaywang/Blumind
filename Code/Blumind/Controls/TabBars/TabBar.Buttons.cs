using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Blumind.Core;

namespace Blumind.Controls
{
    partial class TabBar
    {
        //const int _ButtonSpace = 4;
        //private const int _ButtonSize = 22;
        float ButtonSpace = 4;
        float StandardButtonSize = 24;
        TabBarButton DropDownButton;
        bool _ShowDropDownButton;

        public void InitializeButtons()
        {
            LeftButtons = new XList<TabBarButton>();
            LeftButtons.ItemAdded += new XListEventHandler<TabBarButton>(Buttons_ItemAdded);

            RightButtons = new XList<TabBarButton>();
            RightButtons.ItemAdded += new XListEventHandler<TabBarButton>(Buttons_ItemAdded);

            DropDownButton = new TabBarButton();
            //DropDownButton.OwnerDraw = false;
            DropDownButton.Icon = Properties.Resources.chevron;
            DropDownButton.Visible = ShowDropDownButton;
            DropDownButton.Click +=new EventHandler(DropDownButton_Click);
            //DropDownButton.Paint += new UIPaintEventHandler(DropDownButton_Paint);
            RightButtons.Add(DropDownButton);

            //
            InitializeSpecialTabs();
        }

        void InitializeSpecialTabs()
        {
            LeftSpecialTabs = new XList<TabItem>();
            RightSpecialTabs = new XList<TabItem>();
        }

        void DropDownButton_Paint(object sender, PaintEventArgs e, UIControlStatus ucs)
        {
            int x = 0;
            switch (ucs)
            {
                case UIControlStatus.Hover:
                    x = 1;
                    break;
                case UIControlStatus.Selected:
                    x = 2;
                    break;
                case UIControlStatus.Disabled:
                    x = 3;
                    break;
            }

            x *= 16;
            Image image = Properties.Resources.tab_button;
            Rectangle rect = DropDownButton.Bounds;
            e.Graphics.DrawImage(image,
                new Rectangle(rect.Left + (rect.Width - 16) / 2, rect.Top + (rect.Height - 16) / 2, 16, 16),
                x, 0, 16, 16, GraphicsUnit.Pixel);
        }

        public XList<TabBarButton> LeftButtons { get; private set; }
        public XList<TabBarButton> RightButtons { get; private set; }
        public XList<TabItem> LeftSpecialTabs { get; private set; }
        public XList<TabItem> RightSpecialTabs { get; private set; }

        public IEnumerable<TabBarButton> AllButtons
        {
            get { return LeftButtons.Union(RightButtons); }
        }

        public IEnumerable<TabItem> AllSpecialTabs
        {
            get { return LeftSpecialTabs.Union(RightSpecialTabs); }
        }

        [DefaultValue(false)]
        public virtual bool ShowDropDownButton
        {
            get { return _ShowDropDownButton; }
            set
            {
                if (_ShowDropDownButton != value)
                {
                    _ShowDropDownButton = value;
                    OnShowDropDownButtonChanged();
                }
            }
        }

        protected virtual void OnShowDropDownButtonChanged()
        {
            DropDownButton.Visible = ShowDropDownButton;
            if (LayoutItems())
            {
                Invalidate();
            }
        }

        int LayoutLeftButtons()
        {
            Rectangle rect = GetWorkArea();

            int pos;
            if (IsHorizontal)
            {
                if (RightToLeft == RightToLeft.Yes)
                    pos = rect.Right;
                else
                    pos = rect.Left;
            }
            else
            {
                pos = rect.Top;
            }

            return LayoutLeftButtons(pos);
        }

        int LayoutLeftButtons(int beginPos)
        {
            Rectangle rect = ClientRectangle;

            if (ShowBaseLine)
                rect.Height -= BaseLineSize;
            int buttonHeight = rect.Height - 4;

            float pos = beginPos;
            int index = 0;
            foreach (TabBarButton button in LeftButtons)
            {
                button.Index = index++;
                if (!button.Visible)
                    continue;
                float size = button.CustomSize > 0 ? button.CustomSize : StandardButtonSize;

                RectangleF bounds;
                if (IsHorizontal)
                {
                    bounds = new RectangleF(pos, rect.Top + (rect.Height - buttonHeight) / 2, size, buttonHeight);
                    pos = bounds.Right;
                }
                else
                {
                    bounds = new RectangleF(rect.Left + (rect.Width - buttonHeight) / 2, pos, buttonHeight, size);
                    pos = bounds.Bottom;
                }

                button.Bounds = Rectangle.Ceiling(bounds);
                pos += button.CustomSpace > 0 ? button.CustomSpace : ButtonSpace;
            }

            if (IsHorizontal && RightToLeft == RightToLeft.Yes)
                return (int)Math.Ceiling(rect.Right - pos);
            else
                return (int)Math.Ceiling(pos);
        }

        int LayoutRightButtons()
        {
            Rectangle rect = GetWorkArea();

            int pos;
            if (IsHorizontal)
            {
                if (RightToLeft == RightToLeft.Yes)
                    pos = rect.Left;
                else
                    pos = rect.Right;
            }
            else
            {
                pos = rect.Bottom;
            }

            return LayoutRightButtons(pos);
        }

        int LayoutRightButtons(int beginPos)
        {
            Rectangle rect = ClientRectangle;

            if (ShowBaseLine)
                rect.Height -= BaseLineSize;
            int buttonHeight = rect.Height - 4;

            float pos = beginPos;
            int index = LeftButtons.Count;
            for (int i = RightButtons.Count - 1; i >= 0; i--)
            {
                TabBarButton button = RightButtons[i];
                button.Index = index + i;
                if (!button.Visible)
                    continue;
                float size = button.CustomSize > 0 ? button.CustomSize : StandardButtonSize;

                RectangleF bounds;
                if (IsHorizontal)
                {
                    bounds = new RectangleF(pos - size, rect.Top + (rect.Height - buttonHeight) / 2, size, buttonHeight);
                    pos = bounds.Left;
                }
                else
                {
                    bounds = new RectangleF(rect.Left + (rect.Width - buttonHeight) / 2, pos - size, buttonHeight, size);
                    pos = bounds.Top;
                }

                button.Bounds = Rectangle.Ceiling(bounds);
                pos -= button.CustomSpace > 0 ? button.CustomSpace : ButtonSpace;
            }

            return (int)Math.Floor(rect.Right - pos);
        }

        TabBarButton GetButton(int index)
        {
            if (index < 0)
                return null;
            else if (index < LeftButtons.Count)
                return LeftButtons[index];
            else
            {
                index -= LeftButtons.Count;
                if (index < RightButtons.Count)
                    return RightButtons[index];
                else
                    return null;
            }
        }

        void DropDownButton_Click(object sender, EventArgs e)
        {
            DropDownMenu();
        }

        void OnButtonClick(int index)
        {
            TabBarButton button = GetButton(index);
            if (button != null)
            {
                button.OnClick();
            }
        }

        void OnButtonMouseDown(int index)
        {
            TabBarButton button = GetButton(index);
            if (button != null)
            {
                button.OnMouseDown();
            }
        }

        void OnButtonMouseUp(int index)
        {
            TabBarButton button = GetButton(index);
            if (button != null)
            {
                button.OnMouseUp();
            }
        }

        void Buttons_ItemAdded(object sender, XListEventArgs<TabBarButton> e)
        {
            if (e.Item != null && e.Item.Owner != this)
            {
                e.Item.Owner = this;
            }
        }

        public void NotifyItemVisibleChanged()
        {
            PerformLayout();
            Invalidate();
        }
    }
}
