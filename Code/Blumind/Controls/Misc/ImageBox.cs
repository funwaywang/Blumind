using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Blumind.Controls
{
    class ImageBox : ScrollableControl
    {
        Image _Image;
        bool _TransparentBackground;

        public event EventHandler ZoomChanged;

        public ImageBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.UserPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw, true);

            ZoomValue = Zoom;
        }

        [DefaultValue(null)]
        public Image Image
        {
            get { return _Image; }
            set 
            {
                if (_Image != value)
                {
                    _Image = value;
                    OnImageChanged();
                }
            }
        }

        [DefaultValue(false)]
        public bool TransparentBackground
        {
            get { return _TransparentBackground; }
            set 
            {
                if (_TransparentBackground != value)
                {
                    _TransparentBackground = value;
                    OnTransparentBackground();
                }
            }
        }

        protected Size ImageViewSize { get; private set; }

        protected virtual void OnZoomTypeChanged()
        {
            if (Image != null)
            {
                CalcaluteImageViewSize();
                Invalidate();
            }

            if (ZoomChanged != null)
                ZoomChanged(this, EventArgs.Empty);
        }

        protected virtual void OnZoomChanged()
        {
            if (Image != null && ZoomType == Blumind.Controls.ZoomType.Custom)
            {
                CalcaluteImageViewSize();
                Invalidate();
            }

            if (ZoomChanged != null)
                ZoomChanged(this, EventArgs.Empty);
        }        

        protected virtual void OnImageChanged()
        {
            CalcaluteImageViewSize();
            Invalidate(false);
        }

        protected virtual void OnTransparentBackground()
        {
            Invalidate();
        }

        bool CalcaluteImageViewSize()
        {
            if (Image != null)
            {
                var size = Image.Size;
                var clientSize = ClientSize;
                var zoom = Zoom;
                switch (ZoomType)
                {
                    case ZoomType.FitHeight:
                        zoom = (float)clientSize.Height / size.Height;
                        if (zoom * size.Width > clientSize.Width)
                            zoom = (float)(clientSize.Height - SystemInformation.HorizontalScrollBarHeight) / size.Height;
                        break;
                    case ZoomType.FitWidth:
                        zoom = (float)clientSize.Width / size.Width;
                        if (zoom * size.Height > clientSize.Height)
                            zoom = (float)(clientSize.Width - SystemInformation.VerticalScrollBarWidth) / size.Width;
                        break;
                    case ZoomType.FitPage:
                        zoom = Math.Min((float)clientSize.Width / size.Width, (float)clientSize.Height / size.Height);
                        break;
                }

                ZoomValue = zoom;
                size =  PaintHelper.Zoom(size, zoom);
                if (ImageViewSize != size)
                {
                    ImageViewSize = size;
                    AutoScrollMinSize = ImageViewSize;
                    return true;
                }
            }

            return false;
        }

        public void RefreshView()
        {
            if (CalcaluteImageViewSize())
            {
                Invalidate();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            RefreshView();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);

            if (TransparentBackground)
            {
                PaintHelper.DrawTransparentBackground(e.Graphics, ClientRectangle);
            }

            if (Image != null)
            {
                var clientSize = ClientSize;
                var rectImage = new Rectangle(-HorizontalScroll.Value, -VerticalScroll.Value, ImageViewSize.Width, ImageViewSize.Height);
                rectImage.X += Math.Max(0, (clientSize.Width - rectImage.Width) / 2);
                rectImage.Y += Math.Max(0, (clientSize.Height - rectImage.Height) / 2);

                e.Graphics.DrawImage(Image,
                    rectImage,
                    0, 0, Image.Width, Image.Height,
                    GraphicsUnit.Pixel);
            }
        }

        #region Mouse
        bool _DragMode;
        Cursor _ScrollCursor;
        Point MouseDownPos;
        Point MouseDownScrollPos;

        bool DragMode
        {
            get { return _DragMode; }
            set
            {
                if (_DragMode != value)
                {
                    _DragMode = value;
                    OnDragModeChanged();
                }
            }
        }

        Cursor ScrollCursor
        {
            get
            {
                if (_ScrollCursor == null)
                {
                    try
                    {
                        _ScrollCursor = Helper.LoadCursor(Properties.Resources.hand_cur);
                    }
                    catch
                    {
                        _ScrollCursor = Blumind.Resources.RS.GetCursor("cur_scroll");
                    }
                }

                return _ScrollCursor;
            }
        }            

        void OnDragModeChanged()
        {
            if (DragMode)
            {
                Cursor.Current = ScrollCursor;
            }
            else
            {
                Cursor.Current = Cursors.Default;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (VerticalScroll.Visible || HorizontalScroll.Visible)
            {
                //MouseDownScrollPos = AutoScrollPosition;
                MouseDownScrollPos = new Point(HorizontalScroll.Value, VerticalScroll.Value);
                MouseDownPos = new Point(e.X, e.Y);
                DragMode = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (DragMode)
            {
                var scroll = new Point(
                    Math.Max(HorizontalScroll.Minimum, Math.Min(HorizontalScroll.Maximum, MouseDownScrollPos.X - (e.X - MouseDownPos.X))),
                    Math.Max(VerticalScroll.Minimum, Math.Min(VerticalScroll.Maximum, MouseDownScrollPos.Y - (e.Y - MouseDownPos.Y))));

                if (scroll.X != HorizontalScroll.Value || scroll.Y != VerticalScroll.Value)
                {
                    if (HorizontalScroll.Visible && scroll.X != HorizontalScroll.Value)
                        HorizontalScroll.Value = scroll.X;
                    if (VerticalScroll.Visible && scroll.Y != VerticalScroll.Value)
                        VerticalScroll.Value = scroll.Y;
                    Invalidate();
                }
                //Invalidate();
                //AutoScrollPosition = new Point(
                //    MouseDownScrollPos.X - (e.X - MouseDownPos.X),
                //    MouseDownScrollPos.Y - (e.Y - MouseDownPos.Y));
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            DragMode = false;
        }

        #endregion

        #region Zoom
        ZoomType _ZoomType = ZoomType.Custom;
        float _Zoom = 1.0f;

        [DefaultValue(ZoomType.Custom)]
        public ZoomType ZoomType
        {
            get { return _ZoomType; }
            set
            {
                if (_ZoomType != value)
                {
                    _ZoomType = value;
                    OnZoomTypeChanged();
                }
            }
        }

        [DefaultValue(1.0f)]
        public float Zoom
        {
            get { return _Zoom; }
            set
            {
                if (_Zoom != value)
                {
                    _Zoom = value;
                    OnZoomChanged();
                }
            }
        }

        [Browsable(false)]
        public float ZoomValue { get; private set; }

        public void ZoomIn()
        {
            Zoom = (int)(ZoomValue / 0.25f + 1) * 0.25f;
            ZoomType = Blumind.Controls.ZoomType.Custom;
        }

        public void ZoomOut()
        {
            Zoom = (int)(ZoomValue / 0.25f - 1) * 0.25f;
            ZoomType = Blumind.Controls.ZoomType.Custom;
        }
        #endregion
    }
}
