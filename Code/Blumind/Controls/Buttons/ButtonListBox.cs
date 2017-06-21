using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Blumind.Core;
using System.ComponentModel;

namespace Blumind.Controls
{
    class ButtonListBox : BaseControl
    {
        int _ButtonSize;
        ButtonInfo _HoverObject;
        ButtonInfo _PressedObject;
        const int ButtonSpace = 1;

        public event ButtonEventHandler ButtonClick;

        public ButtonListBox()
        {
            _ButtonSize = 32;
            ReserveIconSpace = true;
            IconSize = new System.Drawing.Size(16, 16);

            Buttons = new XList<ButtonInfo>();
            Buttons.ItemAdded += Buttons_ItemAdded;
            Buttons.ItemRemoved += Buttons_ItemRemoved;

            ButtonBackColor = SystemColors.ControlDark;
            ButtonForeColor = SystemColors.ControlText;
            ButtonHoverBackColor = SystemColors.Highlight;
            ButtonHoverForeColor = SystemColors.HighlightText;
        }

        public XList<ButtonInfo> Buttons { get; private set; }

        public Color ButtonBackColor { get; set; }

        public Color ButtonForeColor { get; set; }

        public Color ButtonHoverBackColor { get; set; }

        public Color ButtonHoverForeColor { get; set; }

        [DefaultValue(32)]
        public int ButtonSize
        {
            get { return _ButtonSize; }
            set 
            {
                if (_ButtonSize != value)
                {
                    _ButtonSize = value;
                    OnButtonSizeChanged();
                }
            }
        }

        ButtonInfo HoverObject
        {
            get { return _HoverObject; }
            set 
            {
                if (_HoverObject != value)
                {
                    var old = _HoverObject;
                    _HoverObject = value;
                    OnHoverObjectChanged(old);
                }
            }
        }

        ButtonInfo PressedObject
        {
            get { return _PressedObject; }
            set 
            {
                if (_PressedObject != value)
                {
                    var old = _PressedObject;
                    _PressedObject = value;
                    OnPressedObjectChanged(old);
                }
            }
        }

        [DefaultValue(true)]
        public bool ReserveIconSpace { get; private set; }

        [DefaultValue(typeof(Size), "16, 16")]
        public Size IconSize { get; set; }

        void OnHoverObjectChanged(ButtonInfo old)
        {
            if (old != null)
                InvalidateButton(old);

            if (HoverObject != null)
                InvalidateButton(HoverObject);
        }

        void OnPressedObjectChanged(ButtonInfo old)
        {
            if (old != null)
                InvalidateButton(old);

            if (PressedObject != null)
                InvalidateButton(PressedObject);
        }

        void InvalidateButton(ButtonInfo button)
        {
            if (button != null)
                Invalidate(button.Bounds);
        }

        void OnButtonSizeChanged()
        {
            PerformLayout();
            Invalidate();
        }

        void Buttons_ItemAdded(object sender, XListEventArgs<ButtonInfo> e)
        {
            PerformLayout();
            Invalidate();
        }

        void Buttons_ItemRemoved(object sender, XListEventArgs<ButtonInfo> e)
        {
            PerformLayout();
            Invalidate();
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            LayoutButtons();
        }

        void LayoutButtons()
        {
            var rect = ClientRectangle;
            rect = rect.Inflate(Padding);

            int y = rect.Y;
            foreach (var btn in Buttons)
            {
                btn.Bounds = new Rectangle(rect.X, y, rect.Width, ButtonSize);
                y += ButtonSize + ButtonSpace;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            InvokePaintBackground(this, e);

            //
            var rect = ClientRectangle;
            int iconSize = (ReserveIconSpace || Buttons.Exists(b => b.Image != null)) ? IconSize.Width : 0;
            int maxTextSize = Buttons.Max(b => (int)Math.Ceiling(e.Graphics.MeasureString(b.Text, Font).Width));
            int iconTextSpace = iconSize > 0 ? 6 : 0;
            int contextSize = Math.Min(rect.Width, iconSize + iconTextSpace + maxTextSize);
            int iconX = rect.X + (rect.Width - contextSize) / 3; // 1/3位置
            int textX = iconX + iconSize + iconTextSpace;

            foreach (var btn in Buttons)
            {
                DrawButton(e, btn, iconX, textX);
            }
        }

        void DrawButton(PaintEventArgs e, ButtonInfo button, int iconX, int textX)
        {
            Color backColor = ButtonBackColor;
            Color foreColor = ButtonForeColor;
            if (button == PressedObject || button == HoverObject)
            {
                backColor = ButtonHoverBackColor;
                foreColor = ButtonHoverForeColor;
            }

            var rect = button.Bounds;
            var font = Font;
            e.Graphics.FillRectangle(new SolidBrush(backColor), rect);

            if (button.Image != null)
            {
                var rectIcon = new Rectangle(iconX, rect.Y, IconSize.Width, rect.Height);
                PaintHelper.DrawImageInRange(e.Graphics, button.Image, rectIcon);
            }

            if (!string.IsNullOrEmpty(button.Text))
            {
                var rectText = new Rectangle(textX, rect.Y, rect.Right - textX, rect.Height);
 
                var sf = PaintHelper.SFLeft;
                sf.FormatFlags |= StringFormatFlags.NoWrap;
                sf.Trimming = StringTrimming.EllipsisCharacter;

                e.Graphics.DrawString(button.Text, Font, new SolidBrush(foreColor), rectText, sf);
            }
        }

        ButtonInfo HitTest(int x, int y)
        {
            foreach (var btn in Buttons)
            {
                if (btn.Bounds.Contains(x, y))
                    return btn;
            }

            return null;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            HoverObject = HitTest(e.X, e.Y);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            HoverObject = null;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            PressedObject = HitTest(e.X, e.Y);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            var po = PressedObject;
            PressedObject = null;
            if (po != null && po == HitTest(e.X, e.Y))
            {
                OnButtonClick(po);
            }
        }

        void OnButtonClick(ButtonInfo button)
        {
            if (button == null)
                throw new ArgumentNullException();

            button.NotifyClick();

            if (ButtonClick != null)
                ButtonClick(this, new ButtonEventArgs(button));
        }
    }
}
