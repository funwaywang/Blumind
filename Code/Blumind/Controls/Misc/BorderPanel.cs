using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Controls
{
    class BorderPanel : BaseControl
    {
        int _RoundSize = 0;
        Color _ContentBackColor = Color.White;

        public BorderPanel()
        {
            SetStyle(ControlStyles.UserPaint |
                   ControlStyles.AllPaintingInWmPaint |
                   ControlStyles.OptimizedDoubleBuffer |
                   ControlStyles.ResizeRedraw, true);

            BorderColor = SystemColors.ControlDark;
            ContentBackColor = SystemColors.Window;
            LanguageManage.CurrentChanged += new EventHandler(OnCurrentLanguageChanged);
        }

        [DefaultValue(0)]
        public int RoundSize
        {
            get { return _RoundSize; }
            protected set
            {
                if (_RoundSize != value)
                {
                    _RoundSize = value;
                    OnRoundSizeChanged();
                }
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 100);
            }
        }

        public Color BorderColor { get; private set; }

        [DefaultValue(typeof(Color), "Window")]
        public Color ContentBackColor
        {
            get { return _ContentBackColor; }
            set
            {
                if (_ContentBackColor != value)
                {
                    _ContentBackColor = value;
                    OnInnerBackColorChanged();
                }
            }
        }

        //protected override Padding DefaultPadding
        //{
        //    get
        //    {
        //        return new Padding(4);
        //    }
        //}

        //protected override Padding DefaultMargin
        //{
        //    get
        //    {
        //        return new Padding(1);
        //    }
        //}

        public override Rectangle DisplayRectangle
        {
            get
            {
                var rect = ClientRectangle;
                rect.Inflate(-1, -1);//Border
                if (RoundSize > 0)
                {
                    rect.Inflate(0, -RoundSize);
                }
                return rect;
                //return base.DisplayRectangle;
            }
        }

        private void OnRoundSizeChanged()
        {
            Invalidate(false);
        }

        private void OnInnerBackColorChanged()
        {
            Invalidate(false);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            e.Graphics.Clear(BackColor);

            Rectangle rect = ClientRectangle.Inflate(Padding);
            if (RoundSize == 0)
            {
                e.Graphics.DrawRectangle(new Pen(BorderColor),
                    rect.X,
                    rect.Y,
                    rect.Width - 1,
                    rect.Height - 1);
            }
            else
            {
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                GraphicsPath path = PaintHelper.GetRoundRectangle(rect, RoundSize);
                e.Graphics.FillPath(new SolidBrush(ContentBackColor), path);
                e.Graphics.DrawPath(new Pen(BorderColor), path);
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            if (Controls.Count > 0)
                SelectNextControl(this.Controls[0], true, true, true, true);
        }

        protected virtual void OnCurrentLanguageChanged(object sender, EventArgs e)
        {
        }

        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);

            if (theme != null)
            {
                BorderColor = theme.Colors.BorderColor;
                ContentBackColor = theme.Colors.Window;
            }
        }
    }
}
