using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Blumind.Controls.OS;

namespace Blumind.Controls
{
    class NavigationButton : Button, IDisposable
    {
        class _NavigationBox : DropDownFrame
        {
            const int Border = 3;
            private NavigationButton OwnerButton;
            private Rectangle FullRectangle = Rectangle.Empty;
            private Rectangle ViewRectangle = Rectangle.Empty;
            private bool IsNavigated = false;

            public event System.EventHandler Navigated;

            public _NavigationBox(NavigationButton ownerButton)
            {
                OwnerButton = ownerButton;
                Cursor = Cursors.SizeNWSE;

                SetStyle(ControlStyles.AllPaintingInWmPaint | 
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw | 
                    ControlStyles.UserPaint, true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                //base.OnPaint(e);
                Graphics grf = e.Graphics;
                grf.FillRectangle(SystemBrushes.InactiveCaption, ClientRectangle);
                grf.DrawRectangle(SystemPens.ActiveCaption, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);

                grf.FillRectangle(Brushes.White, FullRectangle);
                if (BackgroundImage != null)
                    grf.DrawImage(BackgroundImage, FullRectangle, 0, 0, BackgroundImage.Width, BackgroundImage.Height, GraphicsUnit.Pixel);

                Region oldRgn = grf.Clip;
                grf.ExcludeClip(ViewRectangle);
                SolidBrush brush = new SolidBrush(Color.FromArgb(128, SystemColors.Highlight));
                //Pen pen = new Pen(Color.FromArgb(200, SystemColors.Highlight));
                grf.FillRectangle(brush, FullRectangle);// ViewRectangle);
                //grf.DrawRectangle(pen, ViewRectangle.Left, ViewRectangle.Top, ViewRectangle.Width - 1, ViewRectangle.Height - 1);
                grf.Clip = oldRgn;

                grf.DrawRectangle(SystemPens.ActiveCaption, FullRectangle.Left, FullRectangle.Top, FullRectangle.Width - 1, FullRectangle.Height - 1);
            }

            public void SetRanges(Size fullSize, Rectangle viewRange)
            {
                Rectangle rect = new Rectangle(0, 0, MaximumSize.Width, MaximumSize.Height);
                rect.Inflate(-Border, -Border);

                int fw = fullSize.Width > 0 ? fullSize.Width : viewRange.Width;
                int fh = fullSize.Height > 0 ? fullSize.Height : viewRange.Height;

                if (fw > 0 || fh > 0)
                {
                    decimal zoomW = fw > 0 ? ((decimal)rect.Width / fw) : 1;
                    decimal zoomH = fh > 0 ? ((decimal)rect.Height / fh) : 1;
                    decimal zoom = Math.Min(zoomW , zoomH);

                    this.FullRectangle = new Rectangle(rect.Left, rect.Top,
                        (int)Math.Ceiling(fw * zoom),
                        (int)Math.Ceiling(fh * zoom));
                    this.ViewRectangle = new Rectangle(rect.Left + (int)Math.Ceiling(viewRange.X * zoom),
                        rect.Top + (int)Math.Ceiling(viewRange.Y * zoom),
                        (int)Math.Floor(Math.Min(fw, viewRange.Width) * zoom),
                        (int)Math.Floor(Math.Min(fh, viewRange.Height) * zoom));
                }
                else
                {
                    this.FullRectangle = rect;
                }

                BuildBackgroundImage();
                this.Size = new Size(FullRectangle.Width + Border * 2, FullRectangle.Height + Border * 2);
                IsNavigated = false;
            }

            private void BuildBackgroundImage()
            {
                if (BackgroundImage != null)
                {
                    BackgroundImage.Dispose();
                    BackgroundImage = null;
                }

                if (FullRectangle.Width > 0 && FullRectangle.Height > 0)
                {
                    BackgroundImage = new Bitmap(FullRectangle.Width, FullRectangle.Height);
                    using (Graphics grf = Graphics.FromImage(BackgroundImage))
                    {
                        OwnerButton.DrawMap(new PaintEventArgs(grf, new Rectangle(0,0,FullRectangle.Width, FullRectangle.Height)));
                    }
                }
            }

            public void CenterMouse()
            {
                Point pt = new Point(ViewRectangle.Left + ViewRectangle.Width / 2, ViewRectangle.Top + ViewRectangle.Height / 2);
                pt = this.PointToScreen(pt);
                User32.SetCursorPos(pt.X, pt.Y);
            }

            public PointF GetNavigationPosition()
            {
                if (FullRectangle.Width > 0 && FullRectangle.Height > 0)
                {
                    return new PointF((ViewRectangle.Left - FullRectangle.Left) / (float)FullRectangle.Width,
                        (ViewRectangle.Top - FullRectangle.Top) / (float)FullRectangle.Height);
                }
                else
                {
                    return PointF.Empty;
                }
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);

                if (IsNavigated)
                    return;

                Point pt = this.PointToClient(Control.MousePosition);
                pt.X = Math.Max(FullRectangle.X, (pt.X - ViewRectangle.Width / 2));
                pt.Y = Math.Max(FullRectangle.Y, (pt.Y - ViewRectangle.Height / 2));
                pt.X = Math.Min(FullRectangle.Right - ViewRectangle.Width, pt.X);
                pt.Y = Math.Min(FullRectangle.Bottom - ViewRectangle.Height, pt.Y);

                Rectangle rect = new Rectangle(pt, ViewRectangle.Size);
                if (rect != ViewRectangle)
                {
                    ViewRectangle = rect;
                    Invalidate();
                }
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);

                if (Navigated != null)
                {
                    IsNavigated = true;
                    Navigated(this, EventArgs.Empty);
                }
            }
        }

        private _NavigationBox NavigationBox;
        private bool _IsMouseDown = false;
        private bool _IsMouseHover = false;
        private Size FullSize = Size.Empty;
        private Rectangle _ViewRectangle = Rectangle.Empty;
        private Size _NavigationBoxSize = new Size(200, 200);

        public event EventHandler BeforePop;
        public event PaintEventHandler PaintMap;
        public event EventHandler Navigated;

        public NavigationButton()
        {
        }

        private bool IsMouseDown
        {
            get { return _IsMouseDown; }
            set
            {
                if (_IsMouseDown != value)
                {
                    _IsMouseDown = value;
                    OnIsMouseDownChanged();
                }
            }
        }

        private bool IsMouseHover
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

        public Rectangle ViewRectangle
        {
            get { return _ViewRectangle; }
            private set { _ViewRectangle = value; }
        }

        [DefaultValue(typeof(Size), "200, 200")]
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

        private void OnNavigationBoxSizeChanged()
        {
            if (NavigationBox != null)
            {
                NavigationBox.MaximumSize = NavigationBoxSize;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.IsMouseDown = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            this.IsMouseDown = false;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.IsMouseHover = false;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.IsMouseHover = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (VisualStyleInformation.IsEnabledByUser)
            {
                ScrollBarState ss = ScrollBarState.Normal;
                if (!Enabled)
                    ss = ScrollBarState.Disabled;
                else if (IsMouseDown)
                    ss = ScrollBarState.Pressed;
                else if (IsMouseHover)
                    ss = ScrollBarState.Hot;

                ScrollBarRenderer.DrawHorizontalThumb(e.Graphics, ClientRectangle, ss);
            }
            else
            {
                base.OnPaint(e);
            }

            Image image = Properties.Resources.navigation_button;
            e.Graphics.DrawImage(image,
                new Rectangle((ClientSize.Width - image.Width) / 2, (ClientSize.Height - image.Height) / 2, image.Width, image.Height),
                0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (this.NavigationBox != null)
            {
                if (NavigationBox.IsDroped)
                    HideMap();
                else
                    ShowMap();
            }
            else
            {
                ShowMap();
            }
        }

        private void InitNavigationBox()
        {
            NavigationBox = new _NavigationBox(this);
            NavigationBox.MaximumSize = NavigationBoxSize;
            NavigationBox.SetRanges(FullSize, ViewRectangle);
            NavigationBox.Navigated += new EventHandler(NavigationBox_Navigated);
        }

        public void ShowMap()
        {
            if (NavigationBox == null)
            {
                InitNavigationBox();
            }

            OnBeforePop();
            NavigationBox.DropDown(this, DropDownDirection.AboveLeft);
            NavigationBox.CenterMouse();
        }

        public void HideMap()
        {
            if (NavigationBox == null)
                return;
        }

        public PointF GetNavigationPosition()
        {
            if (NavigationBox != null)
                return NavigationBox.GetNavigationPosition();
            else
                return PointF.Empty;
        }

        public void SetRanges(Size fullSize, Rectangle viewRange)
        {
            FullSize = fullSize;
            ViewRectangle = viewRange;
            if (NavigationBox != null)
            {
                NavigationBox.SetRanges(fullSize, viewRange);
            }
        }

        private void OnBeforePop()
        {
            if (BeforePop != null)
                BeforePop(this, EventArgs.Empty);
        }

        private void OnIsMouseDownChanged()
        {
            Invalidate();
        }

        private void OnIsMouseHoverChanged()
        {
            Invalidate();
        }

        #region IDisposable 成员

        void IDisposable.Dispose()
        {
            if (this.NavigationBox != null)
            {
                this.NavigationBox.Dispose();
            }
        }

        #endregion

        private void DrawMap(PaintEventArgs e)
        {
            if (PaintMap != null)
            {
                PaintMap(this, e);
            }
        }

        private void NavigationBox_Navigated(object sender, EventArgs e)
        {
            if (Navigated != null)
                Navigated(this, EventArgs.Empty);
        }
    }
}
