using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using Blumind.Controls.Charts;
using Blumind.Core;
using Blumind.Model;

namespace Blumind.Controls
{
    abstract class Chart : ScrollableControlBase
    {
        private ChartBox _ChartBox;

        public Chart()
        {
            ChartBox = CreateChartBox();
            ChartBox.Bounds = DisplayRectangle;
            ChartBox.Paint += new PaintEventHandler(ChartBox_Paint);
            ChartBox.MouseDown += new MouseEventHandler(ChartBox_MouseDown);
            ChartBox.MouseUp += new MouseEventHandler(ChartBox_MouseUp);
            ChartBox.MouseMove += new MouseEventHandler(ChartBox_MouseMove);
            ChartBox.MouseLeave += new EventHandler(ChartBox_MouseLeave);
            ChartBox.MouseWheel += new MouseEventHandler(ChartBox_MouseWheel);
            ChartBox.DoubleClick += new EventHandler(ChartBox_DoubleClick);
            ChartBox.KeyDown += new KeyEventHandler(ChartBox_KeyDown);
            ChartBox.KeyUp += new KeyEventHandler(ChartBox_KeyUp);
            ChartBox.KeyPress += new KeyPressEventHandler(ChartBox_KeyPress);
            ChartBox.Resize += new EventHandler(ChartBox_Resize);
            Controls.Add(ChartBox);

            SetPaintStyles();

            Labels = new XList<ChartLabel>();
            Layers = new XList<ChartLayer>();

            Selector = new SelectionLayer(this);
            Layers.Add(Selector);

            MinimumChartSize = DefaultMinimumChartSize;
        }

        #region property
        Color _ChartBackColor = Color.White;
        Color _ChartForeColor = Color.Black;
        bool _HighQualityRender = true;
        Size _MinimumChartSize = Size.Empty;
        Point _TranslatePoint;
        float _Zoom = 1.0f;
        const float MaxZoom = 10.0f;
        const float MinZoom = 0.25f;
        Size _OriginalContentSize;

        public event EventHandler ZoomChanged;
        public event EventHandler ChartBackColorChanged;

        [Browsable(false)]
        public virtual bool CustomDoubleBuffer
        {
            get { return false; }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 100);
            }
        }

        protected override Padding DefaultMargin
        {
            get
            {
                return Padding.Empty;
            }
        }

        protected override Padding DefaultPadding
        {
            get
            {
                return Padding.Empty;
            }
        }

        [DefaultValue(typeof(Color), "White")]
        public Color ChartBackColor
        {
            get { return _ChartBackColor; }
            set 
            {
                if (_ChartBackColor != value)
                {
                    _ChartBackColor = value;
                    OnChartBackColorChanged();
                }
            }
        }

        [DefaultValue(typeof(Color), "Black")]
        public Color ChartForeColor
        {
            get { return _ChartForeColor; }
            set 
            {
                if (_ChartForeColor != value)
                {
                    _ChartForeColor = value;
                    OnChartForeColorChanged();
                }
            }
        }

        protected ChartBox ChartBox
        {
            get { return _ChartBox; }
            private set { _ChartBox = value; }
        }

        [DefaultValue(true)]
        public bool HighQualityRender
        {
            get { return _HighQualityRender; }
            set
            {
                if (_HighQualityRender != value)
                {
                    _HighQualityRender = value;
                    OnHighQualityRenderChanged();
                }
            }
        }

        [DefaultValue(typeof(Size), "0, 0")]
        public Size MinimumChartSize
        {
            get { return _MinimumChartSize; }
            set
            {
                if (_MinimumChartSize != value)
                {
                    _MinimumChartSize = value;
                    OnMinimumChartSizeChanged();
                }
            }
        }

        protected virtual Size DefaultMinimumChartSize
        {
            get { return Size.Empty; }
        }

        protected Rectangle ViewPort
        {
            get
            {
                Rectangle rect = ChartBox.ClientRectangle;

                Point pt = Point.Empty;
                if (HorizontalScroll.Enabled)
                    pt.X = HorizontalScroll.Value;
                if (VerticalScroll.Enabled)
                    pt.Y = VerticalScroll.Value;

                if (Margin.Left != 0)
                    pt.X -= Margin.Left;
                if (Margin.Top != 0)
                    pt.Y -= Margin.Top; 
                
                rect.Offset(pt.X, pt.Y);
                rect.Width -= Margin.Horizontal;
                rect.Height -= Margin.Vertical;

                return rect;
            }
        }

        [Browsable(false)]
        public Point TranslatePoint
        {
            get { return _TranslatePoint; }
            protected set { _TranslatePoint = value; }
        }

        [DefaultValue(1.0f)]
        public float Zoom
        {
            get { return _Zoom; }
            set
            {
                value = Math.Max(MinZoom, Math.Min(MaxZoom, value));
                if (_Zoom != value)
                {
                    _Zoom = value;
                    OnZoomChanged();
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Size OriginalContentSize
        {
            get { return _OriginalContentSize; }
            set 
            {
                if (_OriginalContentSize != value)
                {
                    _OriginalContentSize = value;
                    OnOriginalContentSizeChanged();
                }
            }
        }

        protected virtual void OnChartBackColorChanged()
        {
            InvalidateChart();

            if (ChartBackColorChanged != null)
                ChartBackColorChanged(this, EventArgs.Empty);
        }

        protected virtual void OnChartForeColorChanged()
        {
            InvalidateChart();
        }

        protected virtual void OnHighQualityRenderChanged()
        {
            this.InvalidateChart();
        }

        protected virtual void OnMinimumChartSizeChanged()
        {
            ResetControlBounds();
        }

        private void OnZoomChanged()
        {
            if (ZoomChanged != null)
                ZoomChanged(this, EventArgs.Empty);

            if (!IsUpdating)
            {
                if (OriginalContentSize.IsEmpty)
                {
                    UpdateView(ChangeTypes.All);
                }
                else
                {
                    CalculateContentSize();
                    InvalidateChart();
                }
            }
        }

        protected virtual void OnOriginalContentSizeChanged()
        {
            CalculateContentSize();
        }

        #endregion

        #region methods
        protected virtual ChartBox CreateChartBox()
        {
            return new ChartBox();
        }

        public Bitmap SaveAsImage()
        {
            //Bitmap bmp = new Bitmap(ContentSize.Width, ContentSize.Height);
            Bitmap bmp = new Bitmap(ClientSize.Width, ClientSize.Height);

            using (Graphics grf = Graphics.FromImage(bmp))
            {
                PaintEventArgs pe = new PaintEventArgs(grf, new Rectangle(0, 0, bmp.Width, bmp.Height));
                ChartBox_Paint(null, pe);
            }

            return bmp;
        }

        protected override void ResetControlBounds()
        {
            base.ResetControlBounds();

            ChartBox.Bounds = DisplayRectangle;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (ChartBox != null && ChartBox.CanFocus)
                ChartBox.Focus();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            UpdateView(ChangeTypes.ViewPort);
        }

        //protected override void OnLayout(LayoutEventArgs e)
        //{
        //    base.OnLayout(e);

        //    UpdateView();
        //}
        
        /// <summary>
        /// Point to Chart-Box
        /// </summary>
        /// <param name="pt">Mouse Point of Screen</param>
        /// <returns></returns>
        public Point PointToChartBox(Point pt)
        {
            return ChartBox.PointToClient(pt);
        }

        public Point PointToLogic(Point pt)
        {
            return PointToLogic(pt.X, pt.Y);
        }

        public Point PointToLogic(int x, int y)
        {
            if (HorizontalScroll.Enabled)
                x += HorizontalScroll.Value;

            if (VerticalScroll.Enabled)
                y += VerticalScroll.Value;

            x -= ChartBox.Margin.Left;
            y -= ChartBox.Margin.Top;

            x -= TranslatePoint.X;
            y -= TranslatePoint.Y;

            x = (int)Math.Floor(x / Zoom);
            y = (int)Math.Floor(y / Zoom);

            return new Point(x, y);
        }

        public Point PointToReal(Point pt)
        {
            pt.X = (int)Math.Round(pt.X * Zoom);
            pt.Y = (int)Math.Round(pt.Y * Zoom);

            pt.X += ChartBox.Margin.Left;
            pt.Y += ChartBox.Margin.Top;

            pt.X += TranslatePoint.X;
            pt.Y += TranslatePoint.Y;

            if (HorizontalScroll.Enabled)
                pt.X -= HorizontalScroll.Value;

            if (VerticalScroll.Enabled)
                pt.Y -= VerticalScroll.Value;

            return pt;
        }

        public Rectangle RectangleToReal(Rectangle rect)
        {
            rect.Location = PointToReal(rect.Location);
            rect.Width = (int)Math.Round(rect.Width * Zoom);
            rect.Height = (int)Math.Round(rect.Height * Zoom);
            return rect;
        }

        public Point ChartPointToScreen(Point point)
        {
            return PointToScreen(PointToReal(point));
        }

        public Rectangle ChartRectangleToScreen(Rectangle rect)
        {
            rect.Location = ChartPointToScreen(rect.Location);
            rect.Width = (int)Math.Round(rect.Width * Zoom);
            rect.Height = (int)Math.Round(rect.Height * Zoom);
            return rect;
        }

        private void CalculateContentSize()
        {
            if (OriginalContentSize.IsEmpty)
            {
                ContentSize = Size.Empty;
            }
            else
            {
                Size size = OriginalContentSize;

                // zoom
                size.Width = (int)Math.Ceiling(size.Width * Zoom);
                size.Height = (int)Math.Ceiling(size.Height * Zoom);

                // add margin
                size.Width += Margin.Horizontal;
                size.Height += Margin.Vertical;

                ContentSize = size;
            }
        }
        #endregion

        #region IDataBind Members
        private DataTable _DataSource = null;

        [DefaultValue(null)]
        public DataTable  DataSource
        {
	          get 
	        {
                return _DataSource;
	        }
	          set 
	        {
                if (_DataSource != value)
                {
                    _DataSource = value;
                    OnDataSourceChanged();
                }
	        }
        }

        protected virtual void OnDataSourceChanged()
        {
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (BmpBuffer != null)
                DisposeBmpBuffer();
        }

        #endregion

        #region scroll value

        protected override void OnHScrollValueChanged()
        {
            base.OnHScrollValueChanged();
            this.InvalidateChart();
        }

        protected override void OnVScrollValueChanged()
        {
            base.OnVScrollValueChanged();
            this.InvalidateChart();
        }

        #endregion

        #region Debug
#if DEBUG
        public Graphics GetRawGraphics()
        {
            return ChartBox.CreateGraphics();
        }
#endif
        #endregion

        #region paint
        private Bitmap BmpBuffer;

        [Browsable(false)]
        public Image DoubleBufferImage
        {
            get { return BmpBuffer; }
        }

        public virtual void InvalidateChart()
        {
            InvalidateChart(false);
        }

        public virtual void InvalidateChart(bool realTime)
        {
            if (CustomDoubleBuffer && !realTime)
                DisposeBmpBuffer();

            if (ChartBox != null)
                ChartBox.Invalidate();
        }

        public void InvalidateChart(Rectangle rect)
        {
            InvalidateChart(rect, false);
        }

        public virtual void InvalidateChart(Rectangle rect, bool realTime)
        {
            if (CustomDoubleBuffer && !realTime)
                DisposeBmpBuffer();

            if (ChartBox != null)
                ChartBox.Invalidate(rect);
        }

        public void InvalidateChart(Region region, bool realTime)
        {
            if (CustomDoubleBuffer && !realTime)
                DisposeBmpBuffer();

            if (ChartBox != null)
                ChartBox.Invalidate(region);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            Graphics grf = e.Graphics;            
            grf.Clear(ChartBackColor);

            if (ShowBorder)
            {
                grf.DrawRectangle(new Pen(BorderColor), 0, 0, Width - 1, Height - 1);
            }
        }

        private void DisposeBmpBuffer()
        {
            if (BmpBuffer != null)
            {
                BmpBuffer.Dispose();
                BmpBuffer = null;
            }
        }

        public GraphicsState TranslateGraphics(Graphics graphics)
        {
            Point pt = Point.Empty;
            if (TranslatePoint.X > 0)
                pt.X = (int)Math.Round(TranslatePoint.X / Zoom);
            if (TranslatePoint.Y > 0)
                pt.Y = (int)Math.Round(TranslatePoint.Y / Zoom);
            if (HorizontalScroll.Enabled)
                pt.X -= (int)Math.Round(HorizontalScroll.Value / Zoom);
            if (VerticalScroll.Enabled)
                pt.Y -= (int)Math.Round(VerticalScroll.Value / Zoom);

            if (ChartBox.Margin.Left != 0)
                pt.X += ChartBox.Margin.Left;
            if (ChartBox.Margin.Top != 0)
                pt.Y += ChartBox.Margin.Top;

            GraphicsState gs = graphics.Save();
            if (Zoom != 1.0f)
            {
                graphics.ScaleTransform(Zoom, Zoom);
            }
            graphics.TranslateTransform(pt.X, pt.Y);

            return gs;
        }

        void ChartBox_Paint(object sender, PaintEventArgs e)
        {
            Point pt = Point.Empty;
            if (HorizontalScroll.Enabled)
                pt.X = HorizontalScroll.Value;
            if (VerticalScroll.Enabled)
                pt.Y = VerticalScroll.Value;

            if (Zoom != 1 && Zoom > 0)
            {
                pt.X = (int)Math.Round(pt.X / Zoom);
                pt.Y = (int)Math.Round(pt.Y / Zoom);
            }

            if (ChartBox.Margin.Left != 0)
                pt.X -= ChartBox.Margin.Left;
            if (ChartBox.Margin.Top != 0)
                pt.Y -= ChartBox.Margin.Top;

            if (!CustomDoubleBuffer || BmpBuffer == null)
            {
                ChartPaintEventArgs cpe;
                if (CustomDoubleBuffer)
                {
                    BmpBuffer = new Bitmap(ChartBox.Width, ChartBox.Height);
                    Graphics grf = Graphics.FromImage(BmpBuffer);
                    cpe = new ChartPaintEventArgs(grf, ChartBox.ClientRectangle);
                }
                else
                {
                    cpe = new ChartPaintEventArgs(e);
                }

                cpe.LogicViewPort = ChartBox.ClientRectangle;
                cpe.BackBrush = new SolidBrush(ChartBackColor);
                cpe.ForeBrush = new SolidBrush(ChartForeColor);
                cpe.Font = ChartBox.DefaultChartFont;

                if (Zoom != 1.0f)
                    cpe.Graphics.ScaleTransform(Zoom, Zoom);

                if (HighQualityRender)
                    PaintHelper.SetHighQualityRender(cpe.Graphics);

                if (!pt.IsEmpty)
                {
                    cpe.Graphics.TranslateTransform(-pt.X, -pt.Y);
                    Rectangle rect = cpe.LogicViewPort;
                    rect.Offset(pt.X, pt.Y);
                    cpe.LogicViewPort = rect;
                }

                // Draw Chart
                DrawChart(cpe);

                if (CustomDoubleBuffer)
                {
                    cpe.Graphics.Dispose();
                }
            }

            if (CustomDoubleBuffer && BmpBuffer != null)
            {
                e.Graphics.DrawImage(BmpBuffer,
                    new Rectangle(0, 0, ChartBox.Width, ChartBox.Height),
                    0, 0, BmpBuffer.Width, BmpBuffer.Height, GraphicsUnit.Pixel);
            }

            if (CustomDoubleBuffer)
            {
                //e.Graphics.TranslateTransform(-pt.X, -pt.Y);
                //e.Graphics.ScaleTransform(Zoom, Zoom);
                PaintHelper.SetHighQualityRender(e.Graphics);
            }

            foreach (ChartLayer layer in Layers)
            {
                layer.DrawRealTime(e);
            }

            OnAfterPaint(e);
        }

        protected virtual void DrawChart(ChartPaintEventArgs e)
        {
            e.Graphics.Clear(ChartBackColor);
            DrawLabels(e);

            foreach (ChartLayer layer in Layers)
            {
                layer.Draw(e);
            }
        }

        protected void DrawLabels(ChartPaintEventArgs e)
        {
            foreach (ChartLabel cl in this.Labels)
            {
                if (cl != null)
                {
                    DrawLable(cl, e);
                }
            }
        }

        protected virtual void DrawLable(ChartLabel cl, ChartPaintEventArgs e)
        {
            if (cl == null || !cl.Visible || string.IsNullOrEmpty(cl.Text))
                return;

            //Rectangle rect_chart = new Rectangle(0, 0, this.ContentSize.Width, this.ContentSize.Height);
            Rectangle rect_chart = new Rectangle(0, 0, this.ContentSize.Width, this.ContentSize.Height);
            Rectangle rect = GetLabelBounds(e.Graphics, cl, rect_chart);
            if (rect.Width == 0 || rect.Height == 0)
                return;

            if (cl.BackColor.HasValue)
                e.Graphics.FillRectangle(new SolidBrush(cl.BackColor.Value), rect);

            e.Graphics.DrawString(cl.Text, (cl.Font == null ? e.Font : cl.Font),
                (cl.ForeColor.HasValue ? new SolidBrush(cl.ForeColor.Value) : e.ForeBrush), 
                rect, PaintHelper.SFLeft);
        }

        protected virtual void OnAfterPaint(PaintEventArgs e)
        {
        }

        public override void UpdateView(ChangeTypes ut)
        {
            if ((ut & (ChangeTypes.Layout | ChangeTypes.Visual)) != ChangeTypes.None)
            {
                InvalidateChart();
            }

            base.UpdateView(ut);
        }
        #endregion

        #region ChartLabels
        private XList<ChartLabel> _Labels;

        public XList<ChartLabel> Labels
        {
            get { return _Labels; }
            private set
            {
                if (_Labels != value)
                {
                    _Labels = value;
                    OnLabelsChanged();
                }
            }
        }

        private void OnLabelsChanged()
        {
            if (Labels != null)
            {
                Labels.ItemAdded += new XListEventHandler<ChartLabel>(Labels_ItemAdded);
                Labels.ItemRemoved += new XListEventHandler<ChartLabel>(Labels_ItemRemoved);
                Labels.ItemChanged += new XListValueEventHandler<ChartLabel>(Labels_ItemChanged);
            }
        }

        private void Labels_ItemChanged(object sender, XListValueEventArgs<ChartLabel> e)
        {
            if(e.OldValue != null)
                e.OldValue.NeedPaint -= new EventHandler(Label_NeedPaint);

            if(e.NewValue != null)
                e.NewValue.NeedPaint +=new EventHandler(Label_NeedPaint);

            TryUpdate(ChangeTypes.All);
        }

        private void Labels_ItemRemoved(object sender, XListEventArgs<ChartLabel> e)
        {
            if (e.Item != null)
                e.Item.NeedPaint -= new EventHandler(Label_NeedPaint);

            TryUpdate(ChangeTypes.All);
        }

        private void Labels_ItemAdded(object sender, XListEventArgs<ChartLabel> e)
        {
            if (e.Item != null)
                e.Item.NeedPaint += new EventHandler(Label_NeedPaint);

            TryUpdate(ChangeTypes.All);
        }

        private void Label_NeedPaint(object sender, EventArgs e)
        {
            TryUpdate(ChangeTypes.Visual);
        }

        private Rectangle GetLabelBounds(Graphics grf, ChartLabel cl, Rectangle rectChart)
        {
            if (cl == null || string.IsNullOrEmpty(cl.Text))
                return Rectangle.Empty;

            Rectangle rect;
            if (cl.AutoSize)
            {
                SizeF sizef = grf.MeasureString(cl.Text, (cl.Font != null) ? cl.Font : this.Font);
                rect = new Rectangle(0, 0, (int)Math.Ceiling(sizef.Width), (int)Math.Ceiling(sizef.Height));
            }
            else
            {
                rect = new Rectangle(0, 0, cl.Size.Width, cl.Size.Height);
            }

            switch (cl.Alignment)
            {
                case ContentAlignment.TopLeft:
                    rect.Location = cl.Location;
                    break;
                case ContentAlignment.TopCenter:
                    rect.Location = new Point(rectChart.Left + rectChart.Left + (rectChart.Width-rect.Width )/2, rectChart.Top + cl.Location.Y);
                    break;
                case ContentAlignment.TopRight:
                    rect.Location = new Point(rectChart.Right - cl.Location.X - rect.Width, rectChart.Top + cl.Location.Y);
                    break;
                case ContentAlignment.MiddleLeft:
                    rect.Location = new Point(rectChart.Left + cl.Location.X, rectChart.Top + (rectChart.Height - rect.Height) / 2);
                    break;
                case ContentAlignment.MiddleCenter:
                    rect.Location = new Point(rectChart.Left + (rectChart.Width - rect.Width) / 2, rectChart.Top + (rectChart.Height - rect.Height) / 2);
                    break;
                case ContentAlignment.MiddleRight:
                    rect.Location = new Point(rectChart.Right - cl.Location.X - rect.Width, rectChart.Top + (rectChart.Height - rect.Height) / 2);
                    break;
                case ContentAlignment.BottomLeft:
                    rect.Location = new Point(rectChart.Left + cl.Location.X, rectChart.Bottom - cl.Location.Y - rect.Height);
                    break;
                case ContentAlignment.BottomCenter:
                    rect.Location = new Point(rectChart.Left + (rectChart.Width - rect.Width) / 2, rectChart.Bottom - cl.Location.Y - rect.Height);
                    break;
                case ContentAlignment.BottomRight:
                    rect.Location = new Point(rectChart.Right - cl.Location.X - rect.Width, rectChart.Bottom - cl.Location.Y - rect.Height);
                    break;
            }

            return rect;
        }

        #endregion

        #region chart layers
        private XList<ChartLayer> _Layers = new XList<ChartLayer>();

        public XList<ChartLayer> Layers
        {
            get { return _Layers; }
            private set
            {
                if (_Layers != value)
                {
                    _Layers = value;
                    OnLayersChanged();
                }
            }
        }

        private void OnLayersChanged()
        {
        }

        #endregion

        #region mouses
        Point _ChartMouseDownPoint = Point.Empty;
        MouseButtons _ChartMouseDownButton = MouseButtons.None;
        ChartMouseMethod _MouseMethod = ChartMouseMethod.Select;
        ContextMenuStrip _ChartContextMenuStrip;

        public event EventHandler MouseMethodChanged;

        protected MouseButtons ChartMouseDownButton
        {
            get { return _ChartMouseDownButton; }
            private set { _ChartMouseDownButton = value; }
        }

        protected Point ChartMouseDownPoint
        {
            get { return _ChartMouseDownPoint; }
            private set { _ChartMouseDownPoint = value; }
        }

        [Browsable(false), DefaultValue(ChartMouseMethod.Select)]
        public ChartMouseMethod MouseMethod
        {
            get { return _MouseMethod; }
            set 
            {
                if (_MouseMethod != value)
                {
                    _MouseMethod = value;
                    OnMouseMethodChanged();
                }
            }
        }

        [DefaultValue(null)]
        public ContextMenuStrip ChartContextMenuStrip
        {
            get { return _ChartContextMenuStrip; }
            set { _ChartContextMenuStrip = value; }
        }

        void ChartBox_Resize(object sender, EventArgs e)
        {
            if (CustomDoubleBuffer)
                UpdateView(ChangeTypes.ViewPort | ChangeTypes.Visual);
        }
 
        void ChartBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (Layers.Count > 0)
            {
                ExMouseEventArgs ee = new ExMouseEventArgs(e);
                foreach (ChartLayer layer in Layers)
                {
                    layer.OnMouseMove(ee);
                    if (ee.Suppress)
                        return;
                }
            }

            OnChartMouseMove(e);
        }

        void ChartBox_MouseLeave(object sender, EventArgs e)
        {
            OnChartMouseLeave(e);
        }

        void ChartBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (Layers.Count > 0)
            {
                ExMouseEventArgs ee = new ExMouseEventArgs(e);
                foreach (ChartLayer layer in Layers)
                {
                    layer.OnMouseUp(ee);
                    if (ee.Suppress)
                        goto Exit;
                }
            }

            OnChartMouseUp(e);

            Exit:
            ChartMouseDownButton = MouseButtons.None;
            ChartMouseDownPoint = Point.Empty;
        }

        void ChartBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (!ChartBox.Focused && ChartBox.CanFocus)
                ChartBox.Focus();
            ChartMouseDownButton = e.Button;
            ChartMouseDownPoint = new Point(e.X, e.Y);

            if (Layers.Count > 0)
            {
                ExMouseEventArgs ee = new ExMouseEventArgs(e);
                foreach (ChartLayer layer in Layers)
                {
                    layer.OnMouseDown(ee);
                    if (ee.Suppress)
                        return;
                }
            }

            OnChartMouseDown(e);
        }

        void ChartBox_DoubleClick(object sender, EventArgs e)
        {
            if (Layers.Count > 0)
            {
                HandledEventArgs he = new HandledEventArgs();
                foreach (ChartLayer layer in Layers)
                {
                    layer.OnDoubleClick(he);
                    if (he.Handled)
                        return;
                }
            }

            OnChartDoubleClick(e);
        }

        void ChartBox_MouseWheel(object sender, MouseEventArgs e)
        {
            OnChartMouseWheel(e);
        }

        protected virtual void OnChartMouseDown(MouseEventArgs e)
        {
        }

        protected virtual void OnChartMouseUp(MouseEventArgs e)
        {
        }

        protected virtual void OnChartMouseMove(MouseEventArgs e)
        {
        }

        protected virtual void OnChartMouseLeave(EventArgs e)
        {
        }

        protected virtual void OnChartDoubleClick(EventArgs e)
        {
        }

        protected virtual void OnChartMouseWheel(MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift || !VerticalScroll.Enabled)
            {
                ScrollSamllChange(ScrollBars.Horizontal, -e.Delta / SystemInformation.MouseWheelScrollDelta * SystemInformation.MouseWheelScrollLines);
            }
            else
            {
                ScrollSamllChange(ScrollBars.Vertical, -e.Delta / SystemInformation.MouseWheelScrollDelta * SystemInformation.MouseWheelScrollLines);
            }
        }

        protected virtual void OnMouseMethodChanged()
        {
            if (MouseMethodChanged != null)
                MouseMethodChanged(this, EventArgs.Empty);
        }
        #endregion

        #region Keyboard
        private Keys _ExtentChartInputKey = Keys.None;

        [Browsable(false)]
        protected Keys ExtentChartInputKey
        {
            get { return _ExtentChartInputKey; }
            set 
            {
                if (_ExtentChartInputKey != value)
                {
                    _ExtentChartInputKey = value;
                    OnExtentInputKeyChanged();
                }
            }
        }

        private void OnExtentInputKeyChanged()
        {
            if (ChartBox != null)
            {
                ChartBox.ExtentInputKey = this.ExtentChartInputKey;
            }
        }

        private void ChartBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnChartKeyPress(e);
        }

        private void ChartBox_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (ChartLayer layer in Layers)
            {
                layer.OnKeyUp(e);
                if (e.SuppressKeyPress)
                    return;
            }

            OnChartKeyUp(e);
        }

        private void ChartBox_KeyDown(object sender, KeyEventArgs e)
        {
            foreach (ChartLayer layer in Layers)
            {
                layer.OnKeyDown(e);
                if (e.SuppressKeyPress)
                    return;
            }

            OnChartKeyDown(e);

            if (!e.SuppressKeyPress)
            {
                switch (e.KeyCode)
                {
                    case Keys.PageUp:
                        this.ScrollPages(e.Shift ? ScrollBars.Horizontal : ScrollBars.Vertical, -1);
                        break;
                    case Keys.PageDown:
                        this.ScrollPages(e.Shift ? ScrollBars.Horizontal : ScrollBars.Vertical, 1);
                        break;
                }
            }
        }

        protected virtual void OnChartKeyPress(KeyPressEventArgs e)
        {
        }

        protected virtual void OnChartKeyDown(KeyEventArgs e)
        {
        }

        protected virtual void OnChartKeyUp(KeyEventArgs e)
        {
        }

        #endregion

        #region Select Box
        private SelectionLayer Selector;

        [Browsable(false), DefaultValue(typeof(Color), "Highlight")]
        public Color SelectBoxColor
        {
            get { return Selector.Color; }
            set { Selector.Color = value; }
        }

        protected Rectangle LastSelectionBox
        {
            get { return Selector.Bounds; }//.Visible ? Selector.Bounds : Rectangle.Empty; }
        }

        [Browsable(false)]
        public bool IsSelectMode
        {
            get { return Selector.Active; }
        }

        protected void EnterSelectMode()
        {
            Selector.Active = true;
        }

        protected void ExitSelectMode()
        {
            Selector.Active = false;
        }
        #endregion

        #region ToolTip
        ToolTip toolTip1;

        public void ShowToolTip(string text, IEnumerable<string> links)
        {
            if (links != null)
                links = links.Where(l => !string.IsNullOrEmpty(l.Trim())).ToArray();

            if (string.IsNullOrEmpty(text) && links.IsNullOrEmpty())
            {
                ShowToolTip(null);
            }
            else
            {
                var sb = new StringBuilder();
                if (!links.IsNullOrEmpty())
                {
                    sb.AppendLine("Ctrl+Click to follow link");
                    foreach (var l in links)
                        sb.AppendLine(l);
                    sb.AppendLine(new string('-', 32));
                }

                sb.Append(text);
                ShowToolTip(sb.ToString());
            }
        }

        public void ShowToolTip(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                if (toolTip1 != null)
                    toolTip1.SetToolTip(ChartBox, null);
            }
            else
            {
                if (toolTip1 == null)
                {
                    toolTip1 = new ToolTip();
                }

                toolTip1.SetToolTip(ChartBox, text);
            }
        }

        public void HideToolTip(string text)
        {
            if (toolTip1 != null)
            {
                toolTip1.SetToolTip(ChartBox, null);
            }
        }

        #endregion
    }
}
