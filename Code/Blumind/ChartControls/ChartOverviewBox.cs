using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Blumind.Controls
{
    class ChartOverviewBox : BaseControl, IIconProvider
    {
        ScrollableControlBase _Chart;
        MouseButtons MouseDownButton;
        Point MouseDownPoint;
        Rectangle MouseDownViewRectangle;
        bool RectangleInvalided;

        public event EventHandler Navigated;

        public ChartOverviewBox()
        {
            SetPaintStyles();

            BackColor = SystemColors.Window;
            ForeColor = SystemColors.WindowText;
            ShadowColor = SystemColors.Highlight;
            BorderColor = SystemColors.ActiveCaption;

            FullRectangle = new Rectangle(10, 10, 200, 200);
            ViewRectangle = new Rectangle(20, 20, 50, 50);
        }

        public Rectangle FullRectangle { get; private set; }

        public Rectangle ViewRectangle { get; private set; }

        private Rectangle TempViewRectangle { get; set; }

        public Color ShadowColor { get; private set; }

        public Color BorderColor { get; private set; }

        [DefaultValue(typeof(Color), "Window")]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        [DefaultValue(typeof(Color), "WindowText")]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
            }
        }

        [Browsable(false), DefaultValue(null)]
        public ScrollableControlBase Chart
        {
            get { return _Chart; }
            set 
            {
                if (_Chart != value)
                {
                    ScrollableControlBase old = _Chart;
                    _Chart = value;
                    OnChartChanged(old);
                }
            }
        }

        protected override Padding DefaultPadding
        {
            get
            {
                return new Padding(2);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (BackgroundImage != null)
                {
                    BackgroundImage.Dispose();
                    BackgroundImage = null;
                }
            }
        }

        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);

            if (theme != null)
            {
                BackColor = theme.Colors.Window;
                ForeColor = theme.Colors.WindowText;
                ShadowColor = theme.Colors.MediumDark;
                BorderColor = theme.Colors.MediumDark;
            }
        }

        void OnChartChanged(ScrollableControlBase old)
        {
            if (old != null)
            {
                old.ContentSizeChanged -= new EventHandler(Chart_ContentSizeChanged);
                old.HScrollValueChanged -= new EventHandler(Chart_HScrollValueChanged);
                old.VScrollValueChanged -= new EventHandler(Chart_VScrollValueChanged);
                old.ViewUpdated -= new EventHandler(Chart_ViewUpdated);
            }

            if (Chart != null)
            {
                Chart.ContentSizeChanged += new EventHandler(Chart_ContentSizeChanged);
                Chart.HScrollValueChanged += new EventHandler(Chart_HScrollValueChanged);
                Chart.VScrollValueChanged += new EventHandler(Chart_VScrollValueChanged);
                Chart.ViewUpdated += new EventHandler(Chart_ViewUpdated);
            }

            InvalidateRanges();
            InvalidateMap();
        }

        void Chart_ViewUpdated(object sender, EventArgs e)
        {
            InvalidateMap();
        }

        void Chart_VScrollValueChanged(object sender, EventArgs e)
        {
            InvalidateRanges();
        }

        void Chart_HScrollValueChanged(object sender, EventArgs e)
        {
            InvalidateRanges();
        }

        void InvalidateRanges()
        {
            RectangleInvalided = true;
            Invalidate(false);
        }

        void Chart_ContentSizeChanged(object sender, EventArgs e)
        {
            ResetRanges();
        }

        public void ResetRanges()
        {
            if (Chart != null)
            {
                Rectangle viewRect = Chart.DisplayRectangle;
                if (Chart.HorizontalScroll.Enabled)
                    viewRect.X += Chart.HorizontalScroll.Value;
                if (Chart.VerticalScroll.Enabled)
                    viewRect.Y += Chart.VerticalScroll.Value;

                //
                SetRanges(Chart.ContentSize, viewRect);
            }

            RectangleInvalided = false;
        }

        void SetRanges(Size contentSize, Rectangle viewRange)
        {
            //
            Rectangle rect = ClientRectangle;
            rect.X += Padding.Left;
            rect.Y += Padding.Top;
            rect.Width -= Padding.Horizontal;
            rect.Height -= Padding.Vertical;

            //
            int fw = contentSize.Width > 0 ? contentSize.Width : viewRange.Width;
            int fh = contentSize.Height > 0 ? contentSize.Height : viewRange.Height;

            if (fw > 0 || fh > 0)
            {
                double zoomW = fw > 0 ? ((double)rect.Width / fw) : 1;
                double zoomH = fh > 0 ? ((double)rect.Height / fh) : 1;
                double zoom = Math.Min(zoomW, zoomH);

                Rectangle fullRectangle = new Rectangle(0, 0, (int)Math.Ceiling(fw * zoom), (int)Math.Ceiling(fh * zoom));
                fullRectangle.X = rect.X + (rect.Width - fullRectangle.Width) / 2;
                fullRectangle.Y = rect.Y + (rect.Height - fullRectangle.Height) / 2;

                Rectangle viewRectangle = new Rectangle(
                    fullRectangle.X + (int)Math.Ceiling(viewRange.X * zoom),
                    fullRectangle.Y + (int)Math.Ceiling(viewRange.Y * zoom),
                    (int)Math.Ceiling(Math.Min(fw, viewRange.Width) * zoom),
                    (int)Math.Ceiling(Math.Min(fh, viewRange.Height) * zoom));
                viewRectangle.X = Math.Max(0, Math.Min(fullRectangle.X + fullRectangle.Width - viewRectangle.Width, viewRectangle.X));
                viewRectangle.Y = Math.Max(0, Math.Min(fullRectangle.Y + fullRectangle.Height - viewRectangle.Height, viewRectangle.Y));

                //
                FullRectangle = fullRectangle;
                ViewRectangle = viewRectangle;
            }
            else
            {
                FullRectangle = rect;
                ViewRectangle = Rectangle.Empty;
            }

            Invalidate();
        }

        void SetViewRectangle(Rectangle viewRect)
        {
            if (ViewRectangle != viewRect)
            {
                if (!ViewRectangle.IsEmpty)
                {
                    Invalidate(new Rectangle(ViewRectangle.X - 2, ViewRectangle.Y - 2, ViewRectangle.Width + 4, ViewRectangle.Height + 4));
                }

                ViewRectangle = viewRect;

                if (!ViewRectangle.IsEmpty)
                {
                    Invalidate(new Rectangle(ViewRectangle.X - 2, ViewRectangle.Y - 2, ViewRectangle.Width + 4, ViewRectangle.Height + 4));
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            ResetRanges();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (RectangleInvalided)
            {
                ResetRanges();
            }

            if (BackgroundImage == null)
            {
                BuildBackgroundImage();
            }

            e.Graphics.Clear(BackColor);
            //e.Graphics.DrawRectangle(SystemPens.ActiveCaption, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);

            e.Graphics.FillRectangle(Brushes.White, FullRectangle);
            if (BackgroundImage != null)
                e.Graphics.DrawImage(BackgroundImage, FullRectangle, 0, 0, BackgroundImage.Width, BackgroundImage.Height, GraphicsUnit.Pixel);

            //Pen pen = new Pen(Color.Red, 2);
            //e.Graphics.DrawRectangle(pen,
            //    ViewRectangle.X,
            //    ViewRectangle.Y,
            //    ViewRectangle.Width - 1,
            //    ViewRectangle.Height - 1);
            Region oldRgn = e.Graphics.Clip;
            e.Graphics.ExcludeClip(ViewRectangle);
            SolidBrush brush = new SolidBrush(Color.FromArgb(160, ShadowColor));
            //Pen pen = new Pen(Color.FromArgb(200, SystemColors.Highlight));
            e.Graphics.FillRectangle(brush, FullRectangle);// ViewRectangle);
            //grf.DrawRectangle(pen, ViewRectangle.Left, ViewRectangle.Top, ViewRectangle.Width - 1, ViewRectangle.Height - 1);
            e.Graphics.Clip = oldRgn;

            e.Graphics.DrawRectangle(new Pen(BorderColor), FullRectangle.Left, FullRectangle.Top, FullRectangle.Width - 1, FullRectangle.Height - 1);
        }

        public void InvalidateMap()
        {
            if (BackgroundImage != null)
            {
                BackgroundImage.Dispose();
                BackgroundImage = null;
            }

            Invalidate();
        }

        void BuildBackgroundImage()
        {
            if (BackgroundImage != null)
            {
                BackgroundImage.Dispose();
                BackgroundImage = null;
            }

            if (Chart != null && FullRectangle.Width > 0 && FullRectangle.Height > 0)
            {
                BackgroundImage = new Bitmap(FullRectangle.Width, FullRectangle.Height);
                using (Graphics grf = Graphics.FromImage(BackgroundImage))
                {
                    PaintEventArgs pe = new PaintEventArgs(grf, new Rectangle(0, 0, FullRectangle.Width, FullRectangle.Height));
                    Chart.DrawNavigationMap(pe);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (MouseDownButton == MouseButtons.Left)
            {
                Rectangle rect = MouseDownViewRectangle;
                rect.Offset(e.X - MouseDownPoint.X, e.Y - MouseDownPoint.Y);
                rect.X = Math.Max(FullRectangle.X, Math.Min(FullRectangle.X + FullRectangle.Width - ViewRectangle.Width, rect.X));
                rect.Y = Math.Max(FullRectangle.Y, Math.Min(FullRectangle.Y + FullRectangle.Height - ViewRectangle.Height, rect.Y));
                SetViewRectangle(rect);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            MouseDownButton = e.Button;
            if (e.Button == MouseButtons.Left)
            {
                TempViewRectangle = ViewRectangle;
                Capture = true;
                MouseDownPoint = new Point(e.X, e.Y);

                if (!ViewRectangle.Contains(e.X, e.Y))
                {
                    int x = Math.Min(FullRectangle.X + FullRectangle.Width - ViewRectangle.Width,
                        Math.Max(FullRectangle.X, (e.X - ViewRectangle.Width / 2)));
                    int y = Math.Min(FullRectangle.Y + FullRectangle.Height - ViewRectangle.Height,
                        Math.Max(FullRectangle.Y, (e.Y - ViewRectangle.Height / 2)));
                    SetViewRectangle(new Rectangle(x, y, ViewRectangle.Width, ViewRectangle.Height));
                }
                MouseDownViewRectangle = ViewRectangle;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (Capture)
                Capture = false;

            if (MouseDownButton != MouseButtons.None)
            {
                MouseDownButton = MouseButtons.None;
                OnNavigated();
            }

            base.OnMouseUp(e);
        }

        protected virtual void OnNavigated()
        {
            if (Navigated != null)
            {
                Navigated(this, EventArgs.Empty);
            }

            //
            if (Chart != null)
            {
                NavigateChart(Chart);
            }
        }

        private void NavigateChart(ScrollableControlBase chart)
        {
            if (chart == null || !chart.Created)
                return;

            Size size = chart.ContentSize;
            Rectangle viewPort = chart.DisplayRectangle;

            if (FullRectangle.Width > ViewRectangle.Width)
            {
                if (FullRectangle.X >= ViewRectangle.X)
                    chart.SetScrollValue(ScrollBars.Horizontal, 0, true);
                else if (FullRectangle.Right <= ViewRectangle.Right)
                    chart.SetScrollValue(ScrollBars.Horizontal, size.Width - viewPort.Width, true);
                else
                    chart.SetScrollValue(ScrollBars.Horizontal,
                        (ViewRectangle.X - FullRectangle.X) * size.Width / FullRectangle.Width, true);
            }

            if (FullRectangle.Height > ViewRectangle.Height)
            {
                if (FullRectangle.Y >= ViewRectangle.Y)
                    chart.SetScrollValue(ScrollBars.Vertical, 0, true);
                else if (FullRectangle.Bottom <= ViewRectangle.Bottom)
                    chart.SetScrollValue(ScrollBars.Vertical, size.Height - viewPort.Height, true);
                else
                    chart.SetScrollValue(ScrollBars.Vertical,
                        (ViewRectangle.Y - FullRectangle.Y) * size.Height / FullRectangle.Height, true);
            }
        }

        #region IIconProvider 成员
        Image _Icon;

        public Image Icon
        {
            get { return _Icon; }
            set { _Icon = value; }
        }

        #endregion
    }
}
