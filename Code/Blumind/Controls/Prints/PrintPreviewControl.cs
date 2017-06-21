using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Security.Permissions;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Controls
{
    class PrintPreviewControl : ScrollableControl 
    {
        const int BorderSize = 10;
        const double DefaultZoom = 1.0;
        const int SCROLL_LINE = 5;
        const int SCROLL_PAGE = 100;

        bool _UseAntiAlias = true;
        int _Columns = 1;
        int _Rows = 1;
        PrintDocument _Document;
        int _StartPage;
        Size _VirtualSize;
        double _Zoom = DefaultZoom;
        ZoomType _ZoomType = ZoomType.FitPage;
        Point _Position = Point.Empty;
        Color _PageBackColor = Color.White;
        int _PageCount;
        PreviewPageInfo[] _Pages;

        bool ExceptionPrinting;
        Size ImageSize;
        Point LastOffset;
        bool LayoutOk;
        bool PageInfoCalcPending;
        PointF ScreenDpi;
        double ZoomInternal = DefaultZoom;

        // Events
        public event EventHandler StartPageChanged;
        public event EventHandler PageCountChanged;
        public event EventHandler ZoomChanged;
        public event EventHandler RowsChanged;
        public event EventHandler ColumnsChanged;

        // Methods
        public PrintPreviewControl()
        {
            //SetStyle(ControlStyles.ResizeRedraw, false);
            //SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque, true);

            SetStyle(ControlStyles.UserPaint |
                   ControlStyles.AllPaintingInWmPaint |
                   ControlStyles.OptimizedDoubleBuffer |
                   ControlStyles.ResizeRedraw, true);
        }

        #region Properties

        [DefaultValue(typeof(Color), "White")]
        public Color PageBackColor
        {
            get { return _PageBackColor; }
            set 
            {
                if (_PageBackColor != value)
                {
                    _PageBackColor = value;
                    OnPageBackColorChanged();
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

        [DefaultValue(1)]
        public int Columns
        {
            get
            {
                return this._Columns;
            }
            set
            {
                value = Math.Max(1, value);
                if (_Columns != value)
                {
                    _Columns = value;
                    OnColumnsChanged();
                }
            }
        }

        [DefaultValue(1)]
        public int Rows
        {
            get
            {
                return this._Rows;
            }
            set
            {
                value = Math.Max(1, value);
                if (value != _Rows)
                {
                    _Rows = value;
                    OnRowsChanged();
                }
            }
        }

        [DefaultValue(null)]
        public PrintDocument Document
        {
            get
            {
                return this._Document;
            }
            set
            {
                this._Document = value;
            }
        }

        private Point Position
        {
            get
            {
                return this._Position;
            }

            set
            {
                value = new Point(
                    Math.Max(0, Math.Min(value.X, VirtualSize.Width - ClientSize.Width)),
                    Math.Max(0, Math.Min(value.Y, VirtualSize.Height - ClientSize.Height)));
                if (_Position != value)
                {
                    _Position = value;
                    OnPositionChanged();
                }
            }
        }

        [Localizable(true), AmbientValue(2)]
        public override RightToLeft RightToLeft
        {
            get
            {
                return base.RightToLeft;
            }
            set
            {
                RightToLeft = value;

                InvalidatePreview();
            }
        }

        [DefaultValue(0)]
        public int StartPage
        {
            get
            {
                if (this.Pages != null)
                {
                    return Math.Max(0, Math.Min(_StartPage, Pages.Length - (Rows * Columns)));
                }
                else
                {
                    return _StartPage;
                }
            }
            set
            {
                value = Math.Max(0, value);
                if (value != _StartPage)
                {
                    _StartPage = value;
                    OnStartPageChanged(EventArgs.Empty);
                }
            }
        }

        [DefaultValue(true)]
        public bool UseAntiAlias
        {
            get
            {
                return _UseAntiAlias;
            }
            set
            {
                if (_UseAntiAlias != value)
                {
                    _UseAntiAlias = value;
                    OnUseAntiAliasChanged();
                }
            }
        }

        private Size VirtualSize
        {
            get
            {
                return this._VirtualSize;
            }
            set
            {
                if (_VirtualSize != value)
                {
                    _VirtualSize = value;
                    OnVirtualSizeChanged();
                }
            }
        }

        [DefaultValue(ZoomType.FitPage)]
        public ZoomType ZoomType
        {
            get
            {
                return this._ZoomType;
            }

            set
            {
                if (this._ZoomType != value)
                {
                    this._ZoomType = value;
                    OnZoomTypeChanged();
                }
            }
        }

        [DefaultValue(1.0)]
        public double Zoom
        {
            get
            {
                return this._Zoom;
            }
            set
            {
                value = Math.Max(0.1, value);
                if (value != _Zoom || ZoomType != ZoomType.Custom)
                {
                    _Zoom = value;
                    OnZoomChanged();
                }
            }
        }

        [Browsable(false)]
        public int PageCount
        {
            get { return _PageCount; }
            private set
            {
                if (value != _PageCount)
                {
                    _PageCount = value;
                    OnPageCountChanged();
                }
            }
        }

        private PreviewPageInfo[] Pages
        {
            get { return _Pages; }
            set 
            {
                if (_Pages != value)
                {
                    _Pages = value;
                    OnPagesChanged();
                }
            }
        }

        protected virtual void OnStartPageChanged(EventArgs e)
        {
            InvalidateLayout();

            if (StartPageChanged != null)
                StartPageChanged(this, EventArgs.Empty);
        }

        private void OnUseAntiAliasChanged()
        {
            InvalidateLayout();
        }

        private void OnZoomTypeChanged()
        {
            InvalidateLayout();

            if (ZoomChanged != null)
                ZoomChanged(this, EventArgs.Empty);
        }

        private void OnZoomChanged()
        {
            ZoomType = ZoomType.Custom;

            InvalidateLayout();

            if (ZoomChanged != null)
                ZoomChanged(this, EventArgs.Empty);
        }

        private void OnPositionChanged()
        {
            AutoScrollPosition = Position;

            Invalidate();
        }

        private void OnColumnsChanged()
        {
            InvalidateLayout();

            if (ColumnsChanged != null)
            {
                ColumnsChanged(this, EventArgs.Empty);
            }
        }

        private void OnRowsChanged()
        {
            InvalidateLayout();

            if (RowsChanged != null)
            {
                RowsChanged(this, EventArgs.Empty);
            }
        }

        private void OnPageBackColorChanged()
        {
            Invalidate();
        }

        private void OnVirtualSizeChanged()
        {
            AutoScrollMinSize = new Size(
                Math.Max(ClientSize.Width, VirtualSize.Width),
                Math.Max(ClientSize.Height, VirtualSize.Height));

            Position = new Point(
                Math.Max(0, Math.Min(Position.X, VirtualSize.Width - ClientSize.Width)),
                Math.Max(0, Math.Min(Position.Y, VirtualSize.Height - ClientSize.Height)));

            Invalidate();
        }

        private void OnPageCountChanged()
        {
            StartPage = Math.Max(0, Math.Min(PageCount - 1, StartPage));

            if (PageCountChanged != null)
                PageCountChanged(this, EventArgs.Empty);
        }

        private void OnPagesChanged()
        {
            if (Pages == null)
                PageCount = 0;
            else
                PageCount = Pages.Length;
        }

        #endregion

        void CalculatePageInfo()
        {
            if (!PageInfoCalcPending)
            {
                PageInfoCalcPending = true;
                try
                {
                    if (Pages == null)
                    {
                        try
                        {
                            ComputePreview();
                        }
                        catch(System.Exception ex)
                        {
                            Helper.WriteLog(ex);
                            ExceptionPrinting = true;
                            throw;
                        }
                        finally
                        {
                            Invalidate();
                        }
                    }
                }
                finally
                {
                    PageInfoCalcPending = false;
                }
            }
        }

        void ComputeLayout()
        {
            LayoutOk = true;

            if (Pages.Length == 0)
            {
                VirtualSize = ClientSize;
            }
            else
            {
                using (Graphics wrapper = base.CreateGraphics())
                {
                    ScreenDpi = new PointF(wrapper.DpiX, wrapper.DpiY);
                    wrapper.Dispose();

                    Size physicalSize = Pages[StartPage].PhysicalSize;
                    Size clientPhySize = new Size(PixelsToPhysical(new Point(ClientSize), ScreenDpi));
                    int borders_width = BorderSize * (Columns + 1);
                    int borders_height = BorderSize * (Rows + 1);

                    switch (ZoomType)
                    {
                        case ZoomType.FitPage:
                            double zoom_w = (clientPhySize.Width - borders_width) / ((double)(Columns * physicalSize.Width));
                            double zoom_h = (clientPhySize.Height - borders_height) / ((double)(Rows * physicalSize.Height));
                            ZoomInternal = Math.Min(zoom_w, zoom_h);
                            break;
                        case ZoomType.FitWidth:
                            ZoomInternal = (clientPhySize.Width - borders_width) / ((double)(Columns * physicalSize.Width));
                            break;
                        case ZoomType.FitHeight:
                            ZoomInternal = (clientPhySize.Height - borders_height) / ((double)(Rows * physicalSize.Height));
                            break;
                        case ZoomType.Custom:
                        default:
                            ZoomInternal = Zoom;
                            break;
                    }

                    ImageSize = new Size((int)Math.Ceiling(ZoomInternal * physicalSize.Width), (int)Math.Ceiling(ZoomInternal * physicalSize.Height));
                    int x = (ImageSize.Width * Columns) + borders_width;
                    int y = (ImageSize.Height * Rows) + borders_height;

                    VirtualSize = new Size(PhysicalToPixels(new Point(x, y), ScreenDpi));
                }
            }
        }

        void ComputePreview()
        {
            if (Document == null)
            {
                Pages = new PreviewPageInfo[0];
            }
            else
            {
                //IntSecurity.SafePrinting.Demand();
                var printController = Document.PrintController;
                var underlyingController = new PreviewPrintController();
                underlyingController.UseAntiAlias = UseAntiAlias;
                Document.PrintController = new PrintControllerWithStatusDialog(underlyingController, "PrintControllerWithStatusDialog_DialogTitlePreview");
                Document.Print();
                Pages = underlyingController.GetPreviewPageInfo();
                Document.PrintController = printController;
            }

            StartPage = Math.Min(Pages.Length - 1, StartPage);
        }

        void InvalidateLayout()
        {
            LayoutOk = false;

            Invalidate();
        }

        public void InvalidatePreview()
        {
            Pages = null;

            InvalidateLayout();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            PaintBackground(e);

            PaintHelper.SetHighQualityRender(e.Graphics);

            if (Pages == null)
            {
                BeginInvoke(new MethodInvoker(CalculatePageInfo));
            }
            else if (Pages.Length == 0 || ExceptionPrinting)
            {
                SolidBrush brushText = new SolidBrush(ForeColor);

                string msg;
                if (this.ExceptionPrinting)
                    msg = Lang._("Exception Occurs");
                else
                    msg = Lang._("No Pages");

                e.Graphics.DrawString(msg, Font, brushText, ClientRectangle, PaintHelper.SFCenter);

                brushText.Dispose();
            }
            else
            {
                PaintPages(e);
            }
        }

        void PaintPages(PaintEventArgs e)
        {
            if (!LayoutOk)
            {
                ComputeLayout();
            }

            Size virSize = VirtualSize;
            Size clientSize = ClientSize;
            Point point2 = new Point(Math.Max(0, (clientSize.Width - virSize.Width) / 2), Math.Max(0, (clientSize.Height - virSize.Height) / 2));
            point2.X -= this.Position.X;
            point2.Y -= this.Position.Y;
            this.LastOffset = point2;
            int phy_border_x = PhysicalToPixels(BorderSize, ScreenDpi.X);
            int phy_border_y = PhysicalToPixels(BorderSize, ScreenDpi.Y);

            Region clip = e.Graphics.Clip;
            Rectangle[] pageBounds = new Rectangle[Rows * Columns];
            Point pagePos = Point.Empty;
            int max_page_height = 0;
            int pageIndex = StartPage;

            for (int row = 0; row < Rows; row++)
            {
                pagePos.X = 0;
                pagePos.Y = max_page_height * row;

                for (int col = 0; col < Columns; col++)
                {
                    if (pageIndex < Pages.Length)
                    {
                        Size physicalSize = this.Pages[pageIndex].PhysicalSize;
                        ImageSize = new Size(
                            (int)Math.Ceiling(ZoomInternal * physicalSize.Width),
                            (int)Math.Ceiling(ZoomInternal * physicalSize.Height));
                        Point phy_image_size = PhysicalToPixels(new Point(ImageSize), ScreenDpi);
                        int x = (point2.X + (phy_border_x * (col + 1))) + pagePos.X;
                        int y = (point2.Y + (phy_border_y * (row + 1))) + pagePos.Y;
                        pagePos.X += phy_image_size.X;
                        max_page_height = Math.Max(max_page_height, phy_image_size.Y);
                        pageBounds[pageIndex - StartPage] = new Rectangle(x, y, phy_image_size.X, phy_image_size.Y);
                    }

                    pageIndex++;
                }
            }

            pageIndex = StartPage;
            for (int i = 0; i < pageBounds.Length; i++)
            {
                if (pageIndex >= Pages.Length)
                    break;

                Rectangle rect = pageBounds[i];
                PaintPageBackground(e, rect);

                rect.Inflate(-1, -1);
                if (Pages[pageIndex].Image != null)
                {
                    e.Graphics.DrawImage(Pages[pageIndex].Image, rect);
                }
                //e.Graphics.DrawRectangle(Pens.Black, rect.Left, rect.Top, rect.Width - 1, rect.Height - 1);

                pageIndex++;
            }
        }

        void PaintPageBackground(PaintEventArgs e, Rectangle rect)
        {
            e.Graphics.FillRectangle(new SolidBrush(PageBackColor), rect);
            e.Graphics.DrawRectangle(Pens.Black, rect);
        }

        void PaintBackground(PaintEventArgs e)
        {
            //e.Graphics.Clear(BackColor);

            LinearGradientBrush brush = new LinearGradientBrush(ClientRectangle, PaintHelper.GetDarkColor(BackColor), BackColor, 45.0f);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    return true;
            }

            return base.IsInputKey(keyData);
        }

        protected override void OnResize(EventArgs e)
        {
            InvalidateLayout();

            base.OnResize(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            Point pos = this.Position;
            int num = 0;

            switch (e.KeyCode)
            {
                case Keys.Prior:
                    if (e.Control)
                    {
                        pos.Y = Math.Max(0, pos.Y - SCROLL_PAGE);
                    }
                    else
                    {
                        StartPage = Math.Max(0, StartPage - 1);
                        return;
                    }
                    break;

                case Keys.Next:
                    if (e.Control)
                    {
                        num = Math.Max(ClientSize.Height, _VirtualSize.Height);
                        if (pos.Y >= (num - SCROLL_PAGE))
                            pos.Y = num;
                        else
                            pos.Y += SCROLL_PAGE;
                    }
                    else
                    {
                        StartPage = Math.Max(Pages.Length - 1, StartPage + 1);
                        return;
                    }
                    break;

                case Keys.End:
                    if (e.Control)
                        StartPage = Pages.Length - 1;
                    return;

                case Keys.Home:
                    if (e.Control)
                        StartPage = 0;
                    return;

                case Keys.Left:
                    pos.X = Math.Max(0, pos.X - SCROLL_LINE);
                    break;

                case Keys.Up:
                    pos.Y = Math.Max(0, pos.Y - SCROLL_LINE);
                    break;

                case Keys.Right:
                    num = Math.Max(Width, _VirtualSize.Width);
                    if (pos.X >= (num - SCROLL_LINE))
                        pos.X = num;
                    else
                        pos.X += SCROLL_LINE;
                    break;

                case Keys.Down:
                    num = Math.Max(Height, _VirtualSize.Height);
                    if (pos.Y >= (num - SCROLL_LINE))
                        pos.Y = num;
                    else
                        pos.Y += SCROLL_LINE;
                    break;

                default:
                    base.OnKeyDown(e);
                    return;
            }

            Position = pos;
            e.SuppressKeyPress = true;
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);

            Point position = this.Position;
            if (se.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                position.X = Math.Min(Math.Max(ClientSize.Width, _VirtualSize.Width), se.NewValue);
            }
            else
            {
                position.Y = Math.Min(Math.Max(ClientSize.Height, _VirtualSize.Height), se.NewValue);
            }
            this.Position = position;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            Position = new Point(
                Math.Min(Math.Max(ClientSize.Width, _VirtualSize.Width), HorizontalScroll.Value),
                Math.Min(Math.Max(ClientSize.Height, _VirtualSize.Height), VerticalScroll.Value)
                );
            //if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift || !VerticalScroll.Enabled)
            //{
            //    ScrollSamllChange(ScrollBars.Horizontal, -e.Delta / SystemInformation.MouseWheelScrollDelta * SystemInformation.MouseWheelScrollLines);
            //}
            //else
            //{
            //    ScrollSamllChange(ScrollBars.Vertical, -e.Delta / SystemInformation.MouseWheelScrollDelta * SystemInformation.MouseWheelScrollLines);
            //}
        }

        static Point PhysicalToPixels(Point physical, PointF dpi)
        {
            return new Point(PhysicalToPixels(physical.X, dpi.X), PhysicalToPixels(physical.Y, dpi.Y));
        }

        static Size PhysicalToPixels(Size physicalSize, PointF dpi)
        {
            return new Size(PhysicalToPixels(physicalSize.Width, dpi.X), PhysicalToPixels(physicalSize.Height, dpi.Y));
        }

        static int PhysicalToPixels(int physicalSize, float dpi)
        {
            return (int)((physicalSize * dpi) / 100.0);
        }

        static Point PixelsToPhysical(Point pixels, PointF dpi)
        {
            return new Point(PixelsToPhysical(pixels.X, dpi.X), PixelsToPhysical(pixels.Y, dpi.Y));
        }

        static Size PixelsToPhysical(Size pixels, PointF dpi)
        {
            return new Size(PixelsToPhysical(pixels.Width, dpi.X), PixelsToPhysical(pixels.Height, dpi.Y));
        }

        static int PixelsToPhysical(int pixels, float dpi)
        {
            return (int)((pixels * 100.0) / dpi);
        }

        public void FirstPage()
        {
            StartPage = 0;
        }

        public void PreviousPage()
        {
            StartPage--;
        }

        public void NextPage()
        {
            StartPage++;
        }

        public void LastPage()
        {
            StartPage = PageCount;
        }
    }
}
