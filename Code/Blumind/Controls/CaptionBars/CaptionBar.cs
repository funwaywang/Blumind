using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Controls
{
    enum CaptionStyle
    {
        None,
        BaseLine,
        HorizontalGradient,
        VerticalGradient,
    }

    [DefaultProperty("Text")]
    class CaptionBar : BaseControl
    {
        Color _BaseLineColor = SystemColors.ActiveCaption;
        CaptionStyle _BackgroundStyle = CaptionStyle.None;
        Image _Icon;
        int _BaseLineSize = 1;
        int _ButtonSize = 16;
        CaptionButtonInfo _MouseHoverButton;
        CaptionButtonInfo _MouseDownButton;
        ToolTip toolTip1;

        public event System.EventHandler ButtonClick;

        public CaptionBar()
        {
            SetPaintStyles();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        [DefaultValue(16)]
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

        [DefaultValue(CaptionStyle.None)]
        public CaptionStyle BackgroundStyle
        {
            get { return _BackgroundStyle; }
            set
            {
                if (_BackgroundStyle != value)
                {
                    _BackgroundStyle = value;
                    OnBackgroundStyleChanged();
                }
            }
        }

        [DefaultValue(typeof(Color), "ActiveCaption")]
        public Color BaseLineColor
        {
            get { return _BaseLineColor; }
            set
            {
                if (_BaseLineColor != value)
                {
                    _BaseLineColor = value;
                    OnBaseLineColorChanged();
                }
            }
        }

        [DefaultValue(1)]
        public int BaseLineSize
        {
            get { return _BaseLineSize; }
            set
            {
                if (_BaseLineSize != value)
                {
                    _BaseLineSize = value;
                    OnBaseLineSizeChanged();
                }
            }
        }

        [DefaultValue(null)]
        public Image Icon
        {
            get { return _Icon; }
            set
            {
                if (_Icon != value)
                {
                    _Icon = value;
                    OnIconChanged();
                }
            }
        }

        [DefaultValue(null), Browsable(false)]
        public CaptionButtonInfo[] Buttons { get; private set; }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 22);
            }
        }

        protected CaptionButtonInfo MouseHoverButton
        {
            get { return _MouseHoverButton; }
            private set
            {
                if (_MouseHoverButton != value)
                {
                    var old = _MouseHoverButton;
                    _MouseHoverButton = value;
                    OnMouseHoverButtonChanged(old);
                }
            }
        }

        protected CaptionButtonInfo MouseDownButton
        {
            get { return _MouseDownButton; }
            private set
            {
                if (_MouseDownButton != value)
                {
                    var old = _MouseDownButton;
                    _MouseDownButton = value;
                    OnMouseDownButtonChanged(old);
                }
            }
        }

        void OnMouseHoverButtonChanged(CaptionButtonInfo old)
        {
            if (old != null)
                InvalidateButton(old);

            if (MouseHoverButton != null)
                InvalidateButton(MouseHoverButton);

            if (MouseHoverButton != null)
                ShowToolTipText(Lang._(MouseHoverButton.Text));
            else
                ShowToolTipText(null);
        }

        void OnMouseDownButtonChanged(CaptionButtonInfo old)
        {
            if (old != null)
                InvalidateButton(old);

            if (MouseDownButton != null)
                InvalidateButton(MouseDownButton);
        }

        void InvalidateButton(CaptionButtonInfo button)
        {
            if (button == null)
                return;

            Invalidate(button.Bounds);
        }

        void ShowToolTipText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                if (toolTip1 != null)
                    toolTip1.SetToolTip(this, null);
            }
            else
            {
                if (toolTip1 == null)
                    toolTip1 = new ToolTip();
                toolTip1.SetToolTip(this, text);
            }
        }

        void OnBaseLineColorChanged()
        {
            if (BackgroundStyle == CaptionStyle.BaseLine)
                Invalidate();
        }

        void OnBaseLineSizeChanged()
        {
            if (BackgroundStyle == CaptionStyle.BaseLine)
                Invalidate();
        }

        void OnBackgroundStyleChanged()
        {
            Invalidate();
        }

        void OnIconChanged()
        {
            Invalidate();
        }

        void OnButtonSizeChanged()
        {
            ResetControlsBounds();
            Invalidate();
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            ResetControlsBounds();
        }

        public CaptionButtonInfo AddButton(string text, Image image)
        {
            CaptionButtonInfo cbi = new CaptionButtonInfo(text, image);
            AddButton(cbi);
            return cbi;
        }

        public CaptionButtonInfo AddButton(CaptionButtonInfo button)
        {
            if (button == null)
                throw new ArgumentNullException();

            List<CaptionButtonInfo> list = new List<CaptionButtonInfo>();
            if (!Buttons.IsNullOrEmpty())
                list.AddRange(Buttons);
            list.Add(button);
            //list.Sort();
            Buttons = list.ToArray();

            ResetControlsBounds();
            Invalidate();

            return button;
        }

        public bool RemoveButton(CaptionButtonInfo button)
        {
            if (button == null)
                throw new ArgumentNullException();

            if (Buttons.Contains(button))
            {
                Buttons = Buttons.Where(b => b != button).ToArray();
                ResetControlsBounds();
                Invalidate();
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            DrawBackground(e);

            Rectangle rect = ClientRectangle.Inflate(Padding);

            if (!Buttons.IsNullOrEmpty())
            {
                rect.Width = Buttons.Min(b => b.Bounds.Left) - rect.X;
            }

            if (Icon != null && rect.Width > 16)
            {
                e.Graphics.DrawImage(Icon,
                    new Rectangle(rect.Left, rect.Top + (rect.Height - Icon.Height) / 2, Icon.Width, Icon.Height),
                    0, 0, Icon.Width, Icon.Height, GraphicsUnit.Pixel);
                rect.X += Icon.Width + 4;
                rect.Width -= Icon.Width + 4;
            }

            if (!string.IsNullOrEmpty(Text) && rect.Width > 10)
            {
                StringFormat sf = PaintHelper.SFLeft;
                sf.FormatFlags |= StringFormatFlags.NoWrap;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), rect, sf);
            }

            if (!Buttons.IsNullOrEmpty())
            {
                foreach (var btn in Buttons)
                {
                    if (!btn.Visible)
                        continue;
                    DrawButton(e, btn);
                }
            }
        }

        protected virtual void DrawButton(PaintEventArgs e, CaptionButtonInfo button)
        {
            if (button == null)
                throw new ArgumentNullException();

            if (button == MouseHoverButton || button == MouseDownButton)
            {
                VisualStyleElement vse = null;
                ButtonState bs = ButtonState.Normal;
                if (button == MouseDownButton)
                {
                    vse = VisualStyleElement.Button.PushButton.Pressed;
                    bs = ButtonState.Pushed;
                }
                else if (button == MouseHoverButton)
                {
                    vse = VisualStyleElement.Button.PushButton.Normal;
                    bs = ButtonState.Normal;
                }

                if (VisualStyleRenderer.IsSupported && VisualStyleRenderer.IsElementDefined(vse))
                {
                    var renderer = new VisualStyleRenderer(vse);
                    renderer.DrawBackground(e.Graphics, button.Bounds);
                }
                else
                {
                    ControlPaint.DrawButton(e.Graphics, button.Bounds, bs);
                }
            }

            if (button.Image != null)
            {
                var rectangle = button.Bounds;
                e.Graphics.DrawImage(button.Image,
                    new Rectangle(rectangle.X + (rectangle.Width - button.Image.Width) / 2,
                        rectangle.Y + (rectangle.Height - button.Image.Height) / 2,
                        button.Image.Width,
                        button.Image.Height),
                    0, 0, button.Image.Width, button.Image.Height,
                    GraphicsUnit.Pixel);
            }
        }

        protected virtual void DrawBackground(PaintEventArgs e)
        {
            InvokePaintBackground(this, e);

            DrawCaptionBackground(e, ClientRectangle, BackColor, BackgroundStyle, BaseLineSize, BaseLineColor);
        }

        public static void DrawCaptionBackground(PaintEventArgs e
            , Rectangle rect
            , Color backColor
            , CaptionStyle backgroundStyle
            , int baseLineSize
            , Color baseLineColor)
        {
            if (rect.Width == 0 || rect.Height == 0)
                return;

            Brush brush;
            switch (backgroundStyle)
            {
                case CaptionStyle.HorizontalGradient:
                    brush = new LinearGradientBrush(rect, backColor, PaintHelper.GetLightColor(backColor), 0.0f);
                    e.Graphics.FillRectangle(brush, rect);
                    break;
                case CaptionStyle.VerticalGradient:
                    brush = new LinearGradientBrush(rect, backColor, PaintHelper.GetDarkColor(backColor), 90.0f);
                    e.Graphics.FillRectangle(brush, rect);
                    break;
                case CaptionStyle.BaseLine:
                    if (baseLineSize > 0)
                    {
                        //brush = new LinearGradientBrush(rect, baseLineColor, Color.FromArgb(0, baseLineColor), 0.0f);
                        brush = new SolidBrush(baseLineColor);
                        e.Graphics.FillRectangle(brush, new Rectangle(rect.Left, rect.Bottom - baseLineSize, rect.Width, baseLineSize));
                        rect.Height -= baseLineSize;
                    }
                    break;
                default:
                    break;
            }
        }

        protected override void OnTextChanged(System.EventArgs e)
        {
            base.OnTextChanged(e);

            Invalidate();
        }

        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);

            ForeColor = PaintHelper.FarthestColor(BackColor, theme.Colors.Sharp, theme.Colors.Dark);
        }

        void ResetControlsBounds()
        {
            if (Buttons.IsNullOrEmpty())
                return;

            Rectangle rect = ClientRectangle.Inflate(Padding);

            if (BackgroundStyle == CaptionStyle.BaseLine)
            {
                rect.Height -= BaseLineSize;
            }

            var btnRect = new Rectangle(rect.Right - ButtonSize,
                rect.Top + (rect.Height - ButtonSize) / 2,
                ButtonSize,
                ButtonSize);
            foreach (var b in Buttons)
            {
                if (b.Visible)
                {
                    b.Bounds = btnRect;
                    btnRect.X -= ButtonSize + 4;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            MouseHoverButton = HitTest(e.X, e.Y);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            MouseHoverButton = null;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                MouseDownButton = HitTest(e.X, e.Y);
                if (MouseDownButton != null)
                    Capture = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (MouseDownButton != null && MouseDownButton == HitTest(e.X, e.Y))
            {
                MouseDownButton.NotifyClick();

                OnButtonClick(MouseDownButton);
            }

            MouseDownButton = null;
            Capture = false;
        }

        protected virtual void OnButtonClick(CaptionButtonInfo button)
        {
            if (ButtonClick != null)
                ButtonClick(button, EventArgs.Empty);
        }

        CaptionButtonInfo HitTest(int x, int y)
        {
            if (!Buttons.IsNullOrEmpty())
            {
                foreach (var btn in Buttons)
                {
                    if (btn.Bounds.Contains(x, y))
                        return btn;
                }
            }

            return null;
        }
    }
}
