using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Blumind.Controls
{
    class MySplitter : Splitter
    {
        const int SplitButtonSize = 30;
        const int SplitButtonSpace = 20;
        bool _ShowSplitButton1;
        bool _ShowSplitButton2;
        Rectangle SplitButton1Rectangle;
        Rectangle SplitButton2Rectangle;
        HitResult _HoverObject;

        public event System.EventHandler SplitButton1Click;
        public event System.EventHandler SplitButton2Click;

        public MySplitter()
        {
            SetStyle(ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw, true);
        }

        [DefaultValue(false)]
        public bool ShowSplitButton1
        {
            get { return _ShowSplitButton1; }
            set
            {
                if (_ShowSplitButton1 != value)
                {
                    _ShowSplitButton1 = value;
                    OnShowSplitButton1Changed();
                }
            }
        }

        [DefaultValue(false)]
        public bool ShowSplitButton2
        {
            get { return _ShowSplitButton2; }
            set
            {
                if (_ShowSplitButton2 != value)
                {
                    _ShowSplitButton2 = value;
                    OnShowSplitButton2Changed();
                }
            }
        }

        private HitResult HoverObject
        {
            get { return _HoverObject; }
            set
            {
                if (_HoverObject != value)
                {
                    _HoverObject = value;
                    OnHoverObjectChanged();
                }
            }
        }

        private Orientation Orientation
        {
            get
            {
                if (Dock == DockStyle.Top || Dock == DockStyle.Bottom)
                    return Orientation.Horizontal;
                else
                    return Orientation.Vertical;
            }
        }

        private void InvalidateSplitButtons()
        {
            if (!SplitButton1Rectangle.IsEmpty)
                Invalidate(SplitButton1Rectangle);

            if (!SplitButton2Rectangle.IsEmpty)
                Invalidate(SplitButton2Rectangle);
        }

        private void LayoutSplitButtons()
        {
            if (!ShowSplitButton1 && !ShowSplitButton2)
                return;

            Rectangle rect = ClientRectangle;

            if (Orientation == Orientation.Horizontal)
            {
                int x = 0;
                if (ShowSplitButton1 && ShowSplitButton2)
                    x = rect.Left + (rect.Width - SplitButtonSpace - SplitButtonSize * 2) / 2;
                else if (ShowSplitButton1 || ShowSplitButton2)
                    x = rect.Left + (rect.Width - SplitButtonSize) / 2;

                if (ShowSplitButton1)
                {
                    SplitButton1Rectangle = new Rectangle(x, rect.Top, SplitButtonSize, rect.Height);
                    x += SplitButtonSize + SplitButtonSpace;
                }
                else
                {
                    SplitButton1Rectangle = Rectangle.Empty;
                }

                if (ShowSplitButton2)
                {
                    SplitButton2Rectangle = new Rectangle(x, rect.Top, SplitButtonSize, rect.Height);
                }
                else
                {
                    SplitButton2Rectangle = Rectangle.Empty;
                }
            }
            else
            {
                int y = 0;
                if (ShowSplitButton1 && ShowSplitButton2)
                    y = rect.Top + (rect.Height - SplitButtonSpace - SplitButtonSize * 2) / 2;
                else if (ShowSplitButton1 || ShowSplitButton2)
                    y = rect.Top + (rect.Height - SplitButtonSize) / 2;

                if (ShowSplitButton1)
                {
                    SplitButton1Rectangle = new Rectangle(rect.Left, y, rect.Width, SplitButtonSize);
                    y += SplitButtonSize + SplitButtonSpace;
                }
                else
                {
                    SplitButton1Rectangle = Rectangle.Empty;
                }

                if (ShowSplitButton2)
                {
                    SplitButton2Rectangle = new Rectangle(rect.Left, y, rect.Width, SplitButtonSize);
                }
                else
                {
                    SplitButton2Rectangle = Rectangle.Empty;
                }
            }
        }

        private void OnShowSplitButton1Changed()
        {
            InvalidateSplitButtons();
            LayoutSplitButtons();
            InvalidateSplitButtons();
        }

        private void OnShowSplitButton2Changed()
        {
            InvalidateSplitButtons();
            LayoutSplitButtons();
            InvalidateSplitButtons();
        }

        private void OnHoverObjectChanged()
        {
            InvalidateSplitButtons();
        }

        private HitResult HitTest(int x, int y)
        {
            if (ShowSplitButton1 && SplitButton1Rectangle.Contains(x, y))
                return HitResult.SplitButton1;
            else if (ShowSplitButton2 && SplitButton2Rectangle.Contains(x, y))
                return HitResult.SplitButton2;
            else
                return HitResult.None;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (ShowSplitButton1 || ShowSplitButton2)
            {
                HoverObject = HitTest(e.X, e.Y);

                if (HoverObject == HitResult.SplitButton1 || HoverObject == HitResult.SplitButton2)
                {
                    Cursor = Cursors.Arrow;
                }
                else
                {
                    Cursor = Orientation == Orientation.Horizontal ? Cursors.HSplit : Cursors.VSplit;
                    base.OnMouseMove(e);
                }
            }
            else
            {
                base.OnMouseMove(e);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            HoverObject = HitResult.None;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (HoverObject == HitResult.SplitButton1 || HoverObject == HitResult.SplitButton2)
            {
                OnSplitButtonClick(HoverObject);
            }
            else
            {
                base.OnMouseDown(e);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            LayoutSplitButtons();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (ShowSplitButton1 && !SplitButton1Rectangle.IsEmpty)
            {
                PaintSplitButton(e, SplitButton1Rectangle, HoverObject == HitResult.SplitButton1, true);
            }

            if (ShowSplitButton2 && !SplitButton2Rectangle.IsEmpty)
            {
                PaintSplitButton(e, SplitButton2Rectangle, HoverObject == HitResult.SplitButton2, false);
            }
        }

        private void PaintSplitButton(PaintEventArgs e, Rectangle rect, bool hover, bool toLeftTop)
        {
            Brush brushBack = null;
            if (!hover)
            {
                brushBack = new SolidBrush(BackColor);
            }
            else
            {
                if (Orientation == Orientation.Horizontal)
                    brushBack = new LinearGradientBrush(rect, PaintHelper.GetLightColor(BackColor), BackColor, 90.0f);
                else
                    brushBack = new LinearGradientBrush(rect, PaintHelper.GetLightColor(BackColor), BackColor, 0.0f);

            }
            e.Graphics.FillRectangle(brushBack, rect);

            Pen penLine = new Pen(PaintHelper.GetDarkColor(BackColor));
            const int iw = 5;
            const int ih = 3;
            if (Orientation == Orientation.Horizontal)
            {
                Image image = Properties.Resources.split_btn_h;
                int x = toLeftTop ? 0 : iw;
                e.Graphics.DrawImage(image,
                    new Rectangle(rect.Left + (rect.Width - iw) / 2, rect.Top + (rect.Height - ih) / 2, iw, ih),
                    x, 0, iw, ih, GraphicsUnit.Pixel);
            }
            else
            {
                Image image = Properties.Resources.split_btn_v;
                int y = toLeftTop ? 0 : iw;
                e.Graphics.DrawImage(image,
                    new Rectangle(rect.Left + (rect.Width - iw) / 2, rect.Top + (rect.Height - ih) / 2, ih, iw),
                    0, y, ih, iw, GraphicsUnit.Pixel);
            }
        }

        private void OnSplitButtonClick(HitResult ho)
        {
            if (ho == HitResult.SplitButton1)
            {
                OnSplitButton1Click();
            }
            else if (ho == HitResult.SplitButton2)
            {
                OnSplitButton2Click();
            }
        }

        private void OnSplitButton2Click()
        {
            if (SplitButton2Click != null)
                SplitButton2Click(this, EventArgs.Empty);
        }

        private void OnSplitButton1Click()
        {
            if (SplitButton1Click != null)
                SplitButton1Click(this, EventArgs.Empty);
        }

        enum HitResult
        {
            None,
            SplitButton1,
            SplitButton2,
        }
    }
}
