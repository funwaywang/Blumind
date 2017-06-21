using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Blumind.Controls.MapViews
{
    class ChartToolTip
    {
        enum ChartToolTipStatus
        {
            Standby,
            Show,
            Hide,
        }

        ChartTooltipLayer _Parent;
        string _Text;
        bool _Visible = true;
        bool _ShowAlways;
        object _Tag;
        Rectangle _Bounds;
        int _AutomaticDelay = 500;
        int _InitialDelay = 500;
        int _AutoPopDelay = 5000;
        int _ReshowDelay = 100;
        int TimerTick = 0;
        float _Opacity = 0.0f;
        Padding _Padding = new Padding(8);
        ChartToolTipStatus CurrentStatus = ChartToolTipStatus.Standby;
        Color _BackColor = SystemColors.Info;// Color.FromArgb(0xee, 0xee, 0xff);
        Color _ForeColor = Color.Black;
        bool _ShowCloseButton = false;
        Rectangle CloseButtonBounds;
        bool _IsMouseHover;
        Size _MaximumSize = new Size(400, 400);


        public event EventHandler VisibleChanged;
        public event EventHandler BoundsChanged;

        public ChartToolTip(ChartTooltipLayer parent)
        {
            Parent = parent;
        }

        public string Text
        {
            get { return _Text; }
            set
            {
                if (_Text != value)
                {
                    _Text = value;
                    OnTextChanged();
                }
            }
        }

        public bool Visible
        {
            get { return _Visible; }
            set
            {
                if (_Visible != value)
                {
                    _Visible = value;
                    OnVisibleChanged();
                }
            }
        }

        protected int ButtonSize
        {
            get { return 12; }
        }

        public ChartTooltipLayer Parent
        {
            get { return _Parent; }
            private set { _Parent = value; }
        }

        public bool ShowAlways
        {
            get { return _ShowAlways; }
            set
            {
                if (_ShowAlways != value)
                {
                    _ShowAlways = value;
                    OnShowAlwaysChanged();
                }
            }
        }

        public object Tag
        {
            get { return _Tag; }
            set { _Tag = value; }
        }

        public Rectangle Bounds
        {
            get { return _Bounds; }
            set
            {
                if (_Bounds != value)
                {
                    Rectangle old = _Bounds;
                    _Bounds = value;
                    OnBoundsChanged(old);
                }
            }
        }

        public Point Location
        {
            get { return Bounds.Location; }
            set { Bounds = new Rectangle(value, Bounds.Size); }
        }

        public Size Size
        {
            get { return Bounds.Size; }
            set { Bounds = new Rectangle(Bounds.Location, value); }
        }

        public int InitialDelay
        {
            get { return _InitialDelay; }
            set { _InitialDelay = value; }
        }

        public int AutoPopDelay
        {
            get { return _AutoPopDelay; }
            set { _AutoPopDelay = value; }
        }

        public int ReshowDelay
        {
            get { return _ReshowDelay; }
            set { _ReshowDelay = value; }
        }

        public int AutomaticDelay
        {
            get { return _AutomaticDelay; }
            set
            {
                if (_AutomaticDelay != value)
                {
                    _AutomaticDelay = value;
                    OnAutomaticDelayChanged();
                }
            }
        }

        public float Opacity
        {
            get { return _Opacity; }
            private set
            {
                value = Math.Max(0.0f, Math.Min(1.0f, value));
                if (_Opacity != value)
                {
                    _Opacity = value;
                    OnOpacityChanged();
                }
            }
        }

        public Padding Padding
        {
            get { return _Padding; }
            set { _Padding = value; }
        }

        public Color BackColor
        {
            get { return _BackColor; }
            set { _BackColor = value; }
        }

        public Color ForeColor
        {
            get { return _ForeColor; }
            set { _ForeColor = value; }
        }

        public Font Font { get; set; }

        public bool ShowCloseButton
        {
            get { return _ShowCloseButton; }
            set
            {
                if (_ShowCloseButton != value)
                {
                    _ShowCloseButton = value;
                    Size = CalculateSize();
                    Invalidate();
                }
            }
        }

        public bool IsMouseHover
        {
            get { return _IsMouseHover; }
            set 
            {
                if (_IsMouseHover != value)
                {
                    _IsMouseHover = value;
                    OnIsMouseHoverChanged();
                }
            }
        }

        public Size MaximumSize
        {
            get { return _MaximumSize; }
            set { _MaximumSize = value; }
        }

        private void OnAutomaticDelayChanged()
        {
            InitialDelay = AutomaticDelay;
            AutoPopDelay = AutomaticDelay * 10;
            ReshowDelay = AutomaticDelay / 5;
        }

        private void OnShowAlwaysChanged()
        {
            ShowCloseButton = ShowAlways;
        }

        private void OnTextChanged()
        {
            Size = CalculateSize();
        }

        private void OnOpacityChanged()
        {
            Invalidate(Bounds);
        }

        private void OnIsMouseHoverChanged()
        {
            CurrentStatus = ChartToolTipStatus.Show;
            TimerTick = 0;
        }

        public virtual void Draw(PaintEventArgs e)
        {
            Rectangle rect = Bounds;
            if (rect.Width <= 0 || rect.Height <= 0 || Opacity <= 0)
                return;

            int alpha1 = Math.Min(255, (int)Math.Round(Opacity * 255));
            Color backColor1 = Color.FromArgb(Math.Min(255, alpha1), BackColor);
            LinearGradientBrush brush = new LinearGradientBrush(rect, backColor1, backColor1, 90.0f);
            ColorBlend cb = new ColorBlend(3);
            cb.Colors = new Color[] { PaintHelper.GetLightColor(backColor1, 0.1), backColor1, PaintHelper.GetDarkColor(backColor1, 0.1) };
            cb.Positions = new float[] { 0.0f, 0.5f, 1.0f };
            brush.InterpolationColors = cb;

            // shadow
            if (Opacity > 0.9f)
            {
                rect.Offset(2, 2);
                GraphicsPath path = PaintHelper.GetRoundRectangle(rect, 2);
                e.Graphics.FillPath(new SolidBrush(Color.FromArgb((int)Math.Round(100 * Opacity), Color.Gray)), path);
                rect.Offset(-2, -2);
            }

            //
            GraphicsPath path2 = PaintHelper.GetRoundRectangle(rect, 2);
            e.Graphics.FillPath(brush, path2);
            e.Graphics.DrawPath(new Pen(Color.FromArgb(0x74, 0x74, 0x74)), path2);

            rect.X += Padding.Left;
            rect.Y += Padding.Top;
            rect.Width -= Padding.Horizontal;
            rect.Height -= Padding.Vertical;

            if (ShowCloseButton)
            {
                e.Graphics.DrawImage(Properties.Resources.close_button,
                    CloseButtonBounds, 16, 2, 12, 12, GraphicsUnit.Pixel);

                rect.Width -= ButtonSize;
            }

            // 
            Region old = e.Graphics.Clip;
            e.Graphics.Clip = new Region(rect);
            DrawText(rect, e);
            e.Graphics.Clip = old;
        }

        protected virtual void DrawText(Rectangle rect, PaintEventArgs e)
        {
            if (!string.IsNullOrEmpty(Text))
            {
                e.Graphics.DrawString(Text
                    , (Font == null) ? Parent.Font : Font
                    , new SolidBrush(ForeColor)
                    , rect
                    , PaintHelper.SFLeft);
            }
        }

        private void OnVisibleChanged()
        {
            if (Visible)
            {
                CurrentStatus = ChartToolTipStatus.Standby;
                TimerTick = 0;
            }

            Invalidate();

            if (VisibleChanged != null)
                VisibleChanged(this, EventArgs.Empty);
        }

        protected virtual void OnBoundsChanged(Rectangle old)
        {
            if (old != null && Visible)
                Invalidate(old);

            if (ShowCloseButton)
                CloseButtonBounds = new Rectangle(Bounds.Right - Padding.Right - ButtonSize, Bounds.Top + Padding.Top, ButtonSize, ButtonSize);

            if (BoundsChanged != null)
                BoundsChanged(this, EventArgs.Empty);
        }

        internal void OnTimerTick(int ticks)
        {
            int nt = TimerTick + ticks;
            float opacity = Opacity;

            switch (CurrentStatus)
            {
                case ChartToolTipStatus.Standby:
                    if (nt > InitialDelay)
                    {
                        CurrentStatus = ChartToolTipStatus.Show;
                        nt -= InitialDelay;
                        opacity = 1.0f;
                    }
                    else
                    {
                        opacity = nt / (float)InitialDelay;
                    }
                    break;
                case ChartToolTipStatus.Show:
                    if (ShowAlways || IsMouseHover)
                    {
                        nt = 0;
                        opacity = 1.0f;
                    }
                    else if (nt > AutoPopDelay)
                    {
                        CurrentStatus = ChartToolTipStatus.Hide;
                        nt -= AutoPopDelay;
                    }
                    break;
                case ChartToolTipStatus.Hide:
                    if (nt > ReshowDelay)
                    {
                        nt = 0;
                        opacity = 0.0f;
                        Visible = false;
                    }
                    else
                    {
                        opacity = 1.0f - Math.Min(1.0f, TimerTick / (float)ReshowDelay);
                    }
                    break;
            }

            TimerTick = nt;
            Opacity = opacity;
        }

        public void Hide(bool slow)
        {
            if (slow)
            {
                CurrentStatus = ChartToolTipStatus.Hide;
                TimerTick = (int)Math.Round((1.0f - Opacity) * ReshowDelay);
            }
            else
            {
                Visible = false;
            }
        }

        public void Show()
        {
            Visible = true;
            CurrentStatus = ChartToolTipStatus.Standby;
            TimerTick = 0;

            if (!Parent.ToolTips.Contains(this))
                Parent.ToolTips.Add(this);
        }

        private void Invalidate()
        {
            Invalidate(Bounds);
        }

        private void Invalidate(Rectangle rect)
        {
            rect.Inflate(8, 8);
            Parent.Invalidate(rect);
        }

        protected virtual Size CalculateSize()
        {
            StringFormat sf = PaintHelper.SFLeft;
            sf.Trimming = StringTrimming.EllipsisWord;

            Size maxSize = GetTextMaximumSize();

            Size size = Parent.MeasureString(this.Text, maxSize, sf);
            size.Width += Padding.Horizontal;
            size.Height += Padding.Vertical;

            if (ShowCloseButton)
                size.Width += ButtonSize;

            size.Width = Math.Min(MaximumSize.Width, size.Width);
            size.Height = Math.Min(MaximumSize.Height, size.Height);

            return size;
        }

        public Point CalculateBetterLocation(Size size, Point point)
        {
            const int dis = 16;

            Size ts = CalculateSize();
            int x = Math.Max(0, Math.Min(size.Width - ts.Width, point.X + dis));
            int y = point.Y + dis;
            if (y > size.Height)
                y = point.Y - dis;
            return new Point(x, y);
        }

        protected Size GetTextMaximumSize()
        {
            Size maxSize = MaximumSize;
            maxSize.Width -= Padding.Horizontal;
            maxSize.Height -= Padding.Vertical;
            if (ShowCloseButton)
                maxSize.Width -= ButtonSize;

            return maxSize;
        }

        internal virtual void OnMouseDown(ExMouseEventArgs e)
        {
            if (ShowCloseButton && CloseButtonBounds.Contains(e.X, e.Y) && e.Button == MouseButtons.Left)
            {
                Hide(true);
            }
        }

        internal virtual void OnMouseMove(ExMouseEventArgs e)
        {
            IsMouseHover = true;
        }

        internal virtual void OnMouseUp(ExMouseEventArgs e)
        {
            Hide(false);
        }
    }

    class ChartHyperlinkToolTip : ChartToolTip
    {
        private string[] _Hyperlinks;
        private Rectangle[] HyperlinkBounds;

        public ChartHyperlinkToolTip(ChartTooltipLayer parent)
            : base(parent)
        {

        }

        public string[] Hyperlinks
        {
            get { return _Hyperlinks; }
            set
            {
                if (_Hyperlinks != value)
                {
                    _Hyperlinks = value;
                    OnHyperlinksChanged();
                }
            }
        }

        private void OnHyperlinksChanged()
        {
            Size = CalculateSize();
        }

        protected override Size CalculateSize()
        {
            Size size = base.CalculateSize();
            if (Hyperlinks != null && Hyperlinks.Length > 0)
            {
                HyperlinkBounds = new Rectangle[Hyperlinks.Length];
                int index = 0;
                StringFormat sf = PaintHelper.SFLeft;
                sf.Trimming = StringTrimming.EllipsisPath;
                Size maxSize = GetTextMaximumSize();
                foreach (string link in Hyperlinks)
                {
                    Size size2 = Parent.MeasureString(link, maxSize, sf);
                    size2.Width += Padding.Horizontal;
                    if (ShowCloseButton)
                        size2.Width += ButtonSize;
                    size.Width = Math.Max(size.Width, size2.Width);
                    size.Height += size2.Height;
                    HyperlinkBounds[index++].Size = size2;
                }
            }


            //CalculateHyperlinkBounds();

            return new Size(Math.Min(MaximumSize.Width, size.Width),
                Math.Min(MaximumSize.Height, size.Height));
        }

        protected override void OnBoundsChanged(Rectangle old)
        {
            base.OnBoundsChanged(old);

            CalculateHyperlinkBounds();
        }

        private void CalculateHyperlinkBounds()
        {
            if (Hyperlinks != null && Hyperlinks.Length > 0)
            {
                Rectangle rect = Bounds;
                rect.X += Padding.Left;
                rect.Y = rect.Bottom - Padding.Bottom;
                for (int i = HyperlinkBounds.Length - 1; i >= 0; i--)
                {
                    HyperlinkBounds[i].Location = new Point(rect.X, rect.Y - HyperlinkBounds[i].Height);
                    rect.Y -= HyperlinkBounds[i].Height;
                }
            }
            else
            {
                HyperlinkBounds = null;
            }
        }

        protected override void DrawText(Rectangle rect, PaintEventArgs e)
        {
            StringFormat sf = PaintHelper.SFLeft;
            //sf.FormatFlags |= StringFormatFlags.NoWrap;

            if (Hyperlinks != null && Hyperlinks.Length > 0)
            {
                if (!string.IsNullOrEmpty(Text))
                {
                    sf.Trimming = StringTrimming.EllipsisWord;

                    Size maxSize = GetTextMaximumSize();
                    Size size = Parent.MeasureString(Text, maxSize, sf);
                    Rectangle rectText = rect;
                    rectText.Height = size.Height;
                    e.Graphics.DrawString(Text
                        , Font == null ? Parent.Font : Font
                        , new SolidBrush(ForeColor), rectText, sf);
                }

                //
                sf.Trimming = StringTrimming.EllipsisPath;
                if (HyperlinkBounds != null && HyperlinkBounds.Length > 0)
                {
                    Font font = new Font(Parent.Font, FontStyle.Underline);
                    int index = 0;
                    foreach (string link in Hyperlinks)
                    {
                        if (!string.IsNullOrEmpty(link))
                        {
                            Rectangle rectLink = HyperlinkBounds[index++];
                            e.Graphics.DrawString(link, font, Brushes.Blue, rectLink, sf);
                        }
                    }
                }
            }
            else
            {
                base.DrawText(rect, e);
            }
        }

        internal override void OnMouseMove(ExMouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (HyperlinkBounds != null)
            {
                for (int i = 0; i < HyperlinkBounds.Length; i++)
                {
                    if (HyperlinkBounds[i].Contains(e.X, e.Y))
                    {
                        Parent.Owner.Cursor = Cursors.Hand;
                        return;
                    }
                }

                Parent.Owner.Cursor = Cursors.Default;
            }
        }

        internal override void OnMouseUp(ExMouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (HyperlinkBounds != null && Hyperlinks != null && HyperlinkBounds.Length == Hyperlinks.Length)
            {
                for (int i = 0; i < HyperlinkBounds.Length; i++)
                {
                    if (HyperlinkBounds[i].Contains(e.X, e.Y))
                    {
                        Helper.OpenUrl(Hyperlinks[i]);
                        return;
                    }
                }
            }
        }
    }
}
