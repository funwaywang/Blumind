using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Blumind.Controls
{
    delegate void SetScrollValueProc(ScrollBars scrollBar, int value);

    class ScrollableControlBase : BaseControl
    {
        bool _ShowBorder = true;
        Size _ContentSize = Size.Empty;
        HScrollBar HScroll;
        VScrollBar VScroll;
        ScrollInformation _HorizontalScroll;
        ScrollInformation _VerticalScroll;
        Color _BorderColor = SystemColors.ControlDark;
        bool _ShowNavigationMap = false;
        NavigationButton NavButton = null;
        Size _NavigationBoxSize = new Size(200, 200);

        public event EventHandler ContentSizeChanged;
        public event EventHandler HScrollValueChanged;
        public event EventHandler VScrollValueChanged;

        public ScrollableControlBase()
        {
            InitScrollBars();
        }

        #region property

        [DefaultValue(true)]
        public bool ShowBorder
        {
            get { return _ShowBorder; }
            set
            {
                if (_ShowBorder != value)
                {
                    _ShowBorder = value;
                    OnShowBorderChanged();
                }
            }
        }

        [DefaultValue(typeof(Color), "ControlDark")]
        public Color BorderColor
        {
            get { return _BorderColor; }
            set
            {
                if (_BorderColor != value)
                {
                    _BorderColor = value;
                    OnBorderColorChanged();
                }
            }
        }

        [Browsable(false)]
        public Size ContentSize
        {
            get { return _ContentSize; }
            protected set
            {
                if (_ContentSize != value)
                {
                    _ContentSize = value;
                    OnContentSizeChanged();
                }
            }
        }

        [Browsable(false)]
        public override Rectangle DisplayRectangle
        {
            get
            {
                Rectangle rect = ClientRectangle;
                rect.X += Padding.Left + Margin.Left;
                rect.Y += Padding.Top + Margin.Top;
                rect.Width -= Padding.Horizontal + Margin.Horizontal;
                rect.Height -= Padding.Vertical + Margin.Vertical;

                if (ShowBorder)
                {
                    rect.Inflate(-1, -1);
                }

                if (HScroll != null && HScroll.Visible)
                    rect.Height -= HScroll.Height;

                if (VScroll != null && VScroll.Visible)
                    rect.Width -= VScroll.Width;

                return rect;
            }
        }

        [Browsable(false)]
        public ScrollInformation HorizontalScroll
        {
            get { return _HorizontalScroll; }
        }

        [Browsable(false)]
        public ScrollInformation VerticalScroll
        {
            get { return _VerticalScroll; }
        }

        [DefaultValue(true)]
        public bool ShowNavigationMap
        {
            get { return _ShowNavigationMap; }
            set
            {
                if (_ShowNavigationMap != value)
                {
                    _ShowNavigationMap = value;
                    OnShowNavigationMapChanged();
                }
            }
        }

        protected virtual Rectangle ScrollRectangle
        {
            get
            {
                Rectangle rect = ClientRectangle;
                if (ShowBorder)
                {
                    rect.Inflate(-1, -1);
                }

                return rect;
            }
        }

        protected virtual Size ViewPortSize
        {
            get
            {
                Size size = ClientSize;
                if (ShowBorder)
                {
                    size.Width -= 2;
                    size.Height -= 2;
                }

                size.Width -= Margin.Horizontal;
                size.Height -= Margin.Vertical;

                return size;
            }
        }

        [DefaultValue(typeof(Size), "300, 300")]
        public Size NavigationBoxSize
        {
            get { return _NavigationBoxSize; }
            set 
            {
                if (_NavigationBoxSize != value)
                {
                    _NavigationBoxSize = value;
                    OnNavigationBoxSizeChanged();
                }
            }
        }

        #endregion

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            ResetControlBounds();
        }

        void InitNavButton()
        {
            NavButton = new NavigationButton();
            NavButton.NavigationBoxSize = NavigationBoxSize;
            NavButton.BeforePop += new EventHandler(NavButton_BeforePop);
            NavButton.PaintMap += new PaintEventHandler(NavButton_PaintMap);
            NavButton.Navigated += new EventHandler(NavButton_Navigated);
            Controls.Add(NavButton);
        }

        void InitScrollBars()
        {
            HScroll = new HScrollBar();
            HScroll.Visible = false;
            HScroll.ValueChanged += new EventHandler(HScroll_ValueChanged);
            HScroll.EnabledChanged += new EventHandler(HScroll_EnabledChanged);

            VScroll = new VScrollBar();
            VScroll.Visible = false;
            VScroll.ValueChanged += new EventHandler(VScroll_ValueChanged);
            VScroll.EnabledChanged += new EventHandler(VScroll_EnabledChanged);

            _HorizontalScroll = new ScrollInformation();
            _VerticalScroll = new ScrollInformation();

            Controls.Add(HScroll);
            Controls.Add(VScroll);
        }

        void OnContentSizeChanged()
        {
            if (Created)
            {
                ResetControlBounds();
                Invalidate();
            }

            if (ContentSizeChanged != null)
                ContentSizeChanged(this, EventArgs.Empty);
        }

        void ResetScrollBars()
        {
            if (!Created)
                return;

            Rectangle rect = ScrollRectangle;
            Size sizeViewPort = ViewPortSize;

            bool hv = ContentSize.Width > sizeViewPort.Width;
            bool vv = ContentSize.Height > sizeViewPort.Height;
            if (!vv && hv)
                vv = ContentSize.Height > (sizeViewPort.Height - HScroll.Height);
            if (!hv && vv)
                hv = ContentSize.Width > (sizeViewPort.Width - VScroll.Width);
            if (hv)
                sizeViewPort.Height -= HScroll.Height;
            if (vv)
                sizeViewPort.Width -= VScroll.Width;

            if (hv)
            {
                HScroll.Location = new Point(rect.Left, rect.Bottom - HScroll.Height);
                HScroll.Width = rect.Width - (vv ? VScroll.Width : 0);
                HScroll.Maximum = ContentSize.Width;
                HScroll.Minimum = 0;
                HScroll.LargeChange = ViewPortSize.Width;// Math.Max(HScroll.Maximum / 10, sizeViewPort.Width);
                HScroll.SmallChange = HScroll.LargeChange / 10;
                HScroll.Visible = true;
                HScroll.Enabled = true;
            }
            else
            {
                HScroll.Visible = false;
                HScroll.Enabled = false;
            }

            if (vv)
            {
                VScroll.Location = new Point(rect.Right - VScroll.Width, rect.Top);
                VScroll.Height = rect.Height - (hv ? HScroll.Height : 0);
                VScroll.Maximum = ContentSize.Height;
                VScroll.Minimum = 0;
                VScroll.LargeChange = ViewPortSize.Height;// Math.Max(VScroll.Maximum / 10, sizeViewPort.Height);
                VScroll.SmallChange = VScroll.LargeChange / 10;
                VScroll.Visible = true;
                VScroll.Enabled = true;
            }
            else
            {
                VScroll.Visible = false;
                VScroll.Enabled = false;
            }

            // navigation map button `s bounds
            if (ShowNavigationMap && (hv || vv))
            {
                if (NavButton == null)
                {
                    InitNavButton();
                }
                else
                {
                    NavButton.Visible = true;
                }
                NavButton.Location = new Point(rect.Right - VScroll.Width, rect.Bottom - HScroll.Height);
                NavButton.Size = new Size(VScroll.Width, HScroll.Height);
                if (!hv)
                    VScroll.Height -= HScroll.Height;
                else if (!vv)
                    HScroll.Width -= VScroll.Width;
            }
            else if(NavButton != null)
            {
                NavButton.Visible = false;
            }

            this.HorizontalScroll.GetInformation(HScroll);
            this.VerticalScroll.GetInformation(VScroll);
        }

        protected virtual void ResetControlBounds()
        {
            ResetScrollBars();
        }

        void HScroll_ValueChanged(object sender, EventArgs e)
        {
            int pos = this.HorizontalScroll.Value;
            this.HorizontalScroll.GetInformation(HScroll);
            if (pos != this.HorizontalScroll.Value)
            {
                OnHScrollValueChanged();
            }
        }

        void VScroll_ValueChanged(object sender, EventArgs e)
        {
            int pos = this.VerticalScroll.Value;
            this.VerticalScroll.GetInformation(VScroll);
            if (pos != this.VerticalScroll.Value)
            {
                OnVScrollValueChanged();
            }
        }

        void VScroll_EnabledChanged(object sender, EventArgs e)
        {
            OnVScollEnabledChanged(VScroll.Enabled);
        }

        void HScroll_EnabledChanged(object sender, EventArgs e)
        {
            OnHScrollEnabledChanged(HScroll.Enabled);
        }

        protected virtual void OnVScollEnabledChanged(bool enabled)
        {
        }

        protected virtual void OnHScrollEnabledChanged(bool enabled)
        {
        }

        protected virtual void OnHScrollValueChanged()
        {
            if (HScrollValueChanged != null)
                HScrollValueChanged(this, EventArgs.Empty);
        }

        protected virtual void OnVScrollValueChanged()
        {
            if (VScrollValueChanged != null)
                VScrollValueChanged(this, EventArgs.Empty);
        }

        void OnNavigationBoxSizeChanged()
        {
            if (NavButton != null)
            {
                NavButton.NavigationBoxSize = NavigationBoxSize;
            }
        }

        void NavButton_BeforePop(object sender, EventArgs e)
        {
            if (this.NavButton != null)
            {
                Rectangle rect = DisplayRectangle;
                rect.Offset(HorizontalScroll.Enabled ? HorizontalScroll.Value : 0,
                    VerticalScroll.Enabled ? VerticalScroll.Value : 0);
                NavButton.SetRanges(ContentSize, rect);
            }
        }

        void NavButton_PaintMap(object sender, PaintEventArgs e)
        {
            DrawNavigationMap(e);
        }

        void NavButton_Navigated(object sender, EventArgs e)
        {
            if (NavButton != null)
            {
                PointF pt = NavButton.GetNavigationPosition();
                if (HorizontalScroll.Enabled)
                    SetScrollValue(ScrollBars.Horizontal, (int)Math.Ceiling(HorizontalScroll.Maximum * pt.X));
                if (VerticalScroll.Enabled)
                    SetScrollValue(ScrollBars.Vertical, (int)Math.Ceiling(VerticalScroll.Maximum * pt.Y));
            }
        }

        public void SetScrollValue(ScrollBars bars, int value)
        {
            if (bars == ScrollBars.Horizontal || bars == ScrollBars.Both)
                HScroll.Value = Math.Max(HorizontalScroll.Minimum, Math.Min(HorizontalScroll.Maximum - HorizontalScroll.LargeChange, value));

            if (bars == ScrollBars.Vertical || bars == ScrollBars.Both)
                VScroll.Value = Math.Max(VerticalScroll.Minimum, Math.Min(VerticalScroll.Maximum - VerticalScroll.LargeChange, value));
        }

        public void SetScrollValue(ScrollBars bars, int value, bool asynchronous)
        {
            if (asynchronous)
                Invoke(new SetScrollValueProc(SetScrollValue), bars, value);
            else
                SetScrollValue(bars, value);
        }

        public void ScrollPages(ScrollBars bars, int pages)
        {
            if (bars == ScrollBars.None || pages == 0)
                return;

            if (bars == ScrollBars.Horizontal || bars == ScrollBars.Both)
                SetScrollValue(ScrollBars.Horizontal, HorizontalScroll.Value + HorizontalScroll.LargeChange * pages);

            if (bars == ScrollBars.Vertical || bars == ScrollBars.Both)
                SetScrollValue(ScrollBars.Vertical, VerticalScroll.Value + VerticalScroll.LargeChange * pages);
        }

        public void ScrollSamllChange(ScrollBars bars, int pages)
        {
            if (bars == ScrollBars.None || pages == 0)
                return;

            if (bars == ScrollBars.Horizontal || bars == ScrollBars.Both)
                SetScrollValue(ScrollBars.Horizontal, HorizontalScroll.Value + HorizontalScroll.SmallChange * pages);

            if (bars == ScrollBars.Vertical || bars == ScrollBars.Both)
                SetScrollValue(ScrollBars.Vertical, VerticalScroll.Value + VerticalScroll.SmallChange * pages);
        }

        public virtual void DrawNavigationMap(PaintEventArgs e)
        {
        }

        public void ScrollToCenter()
        {
            if (HorizontalScroll.Enabled)
            {
                SetScrollValue(ScrollBars.Horizontal, HorizontalScroll.Minimum + (HorizontalScroll.Maximum - HorizontalScroll.LargeChange) / 2);
            }

            if (VerticalScroll.Enabled)
            {
                SetScrollValue(ScrollBars.Vertical, VerticalScroll.Minimum + (VerticalScroll.Maximum - VerticalScroll.LargeChange) / 2);
            }
        }

        public void Scroll(int x, int y)
        {
            if (HorizontalScroll.Enabled)
            {
                SetScrollValue(ScrollBars.Horizontal, HorizontalScroll.Value + x);
            }

            if (VerticalScroll.Enabled)
            {
                SetScrollValue(ScrollBars.Vertical, VerticalScroll.Value + y);
            }
        }

        protected virtual void EnsureVisible(Rectangle rect)
        {
            Rectangle display = DisplayRectangle;
            if (HorizontalScroll.Enabled)
            {
                SetScrollValue(ScrollBars.Horizontal,
                    Math.Max(rect.Right - display.Width, Math.Min(rect.Left, HorizontalScroll.Value)));
            }

            if (VerticalScroll.Enabled)
            {
                SetScrollValue(ScrollBars.Vertical,
                    Math.Max(rect.Bottom - display.Height, Math.Min(rect.Top, VerticalScroll.Value)));
            }
        }

        #region method for perperty changed
        private void OnShowBorderChanged()
        {
            Invalidate();
        }

        private void OnBorderColorChanged()
        {
            Invalidate();
        }

        private void OnShowNavigationMapChanged()
        {
            if (Created)
            {
                ResetControlBounds();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!this.Size.IsEmpty && Created)
            {
                ResetControlBounds();
            }
        }
        #endregion
    }
}
