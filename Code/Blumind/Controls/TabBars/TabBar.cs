using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Linq;
using Blumind.Core;
using Blumind.Model;
using Blumind.Globalization;

namespace Blumind.Controls
{
    [DefaultEvent("SelectedItemChanged")]
    partial class TabBar : BaseControl
    {
        public event TabItemEventHandler ItemClose;
        public event TabItemCancelEventHandler ItemClosing;
        public event EventHandler ItemMoved;
        public event EventHandler SelectedItemChanged;
        public event EventHandler IsActiveChanged;

        public TabBar()
        {
            SetPaintStyles();
            SetStyle(ControlStyles.Selectable, false);

            InitializeButtons();
            InitializeMouse();
            InitializePaint();

            _Items.ItemAdded += new XListEventHandler<TabItem>(Items_ItemAdded);
            _Items.ItemRemoved += new XListEventHandler<TabItem>(Items_ItemRemoved);
            _Items.ItemChanged += new XListValueEventHandler<TabItem>(Items_ItemChanged);

            MoveDrager = new DragManager(this, DragManager.DragFeedbackType.SubControl);
            //BackColor = Color.FromArgb(183, 200, 226);

            MyToolTip = new ToolTip();
        }

        #region properties
        XList<TabItem> _Items = new XList<TabItem>();
        TabAlignment _Alignment = TabAlignment.Top;
        Padding _ItemPadding = new Padding(2, 0, 2, 0);
        TabItem _SelectedItem = null;
        Size _CloseButtonSize = new Size(16, 16);
        int _ItemMaximumSize;
        int _ItemMinimumSize;
        int _ItemSpace = 1;
        Size _IconSize = new Size(16, 16);
        int _BaseLineSize = 1;
        bool _ShowBaseLine = true;
        List<TabItem> DisplayItems = new List<TabItem>();
        DragManager MoveDrager;
        bool _Movable = false;
        int _ScrollPos = 0;
        TabItem lastMoveNearItem = null;
        int LastMoveGraduatePos = -1;
        bool _IsActive;
        int _TabRounded;
        ToolTip MyToolTip;
        List<TabItem> HalfDisplayItems = new List<TabItem>();//未完全显示的项目
        TabBarRenderer _Renderer;
        int _MaxItemSize = 0;

        [DefaultValue(TabAlignment.Top)]
        public TabAlignment Alignment
        {
            get { return _Alignment; }
            set
            {
                if (_Alignment != value)
                {
                    _Alignment = value;
                    OnAlignmentChanged();
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public XList<TabItem> Items
        {
            get { return _Items; }
            set { _Items = value; }
        }

        [Browsable(false), DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TabItem SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                if (Items.Count > 0 && value == null)
                    return;
                if (_SelectedItem != value)
                {
                    TabItem old = _SelectedItem;
                    _SelectedItem = value;
                    OnSelectedItemChanged(old);
                }
            }
        }

        [Browsable(false), DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TabItem HoverItem
        {
            get { return _HoverItem; }
            set
            {
                if (_HoverItem != value)
                {
                    TabItem old = _HoverItem;
                    _HoverItem = value;
                    OnHoverItemChanged(old);
                }
            }
        }

        [DefaultValue(typeof(Size), "16, 16")]
        public Size CloseButtonSize
        {
            get { return _CloseButtonSize; }
            set
            {
                if (_CloseButtonSize != value)
                {
                    _CloseButtonSize = value;
                    OnCloseButtonSizeChanged();
                }
            }
        }

        [DefaultValue(1)]
        public int ItemSpace
        {
            get { return _ItemSpace; }
            set
            {
                if (_ItemSpace != value)
                {
                    _ItemSpace = value;
                    OnItemSpaceChanged();
                }
            }
        }

        [DefaultValue(typeof(Padding), "2, 0, 2, 0")]
        public virtual Padding ItemPadding
        {
            get { return _ItemPadding; }
            set { _ItemPadding = value; }
        }

        [DefaultValue(0)]
        public int MaxItemSize
        {
            get { return _MaxItemSize; }
            set
            {
                if (_MaxItemSize != value)
                {
                    _MaxItemSize = value;
                    OnMaxItemSizeChanged();
                }
            }
        }

        [DefaultValue(typeof(Size), "16, 16")]
        public Size IconSize
        {
            get { return _IconSize; }
            set
            {
                if (_IconSize != value)
                {
                    _IconSize = value;
                    OnIconSizeChanged();
                }
            }
        }

        [DefaultValue(true)]
        public bool ShowBaseLine
        {
            get { return _ShowBaseLine; }
            set
            {
                if (_ShowBaseLine != value)
                {
                    _ShowBaseLine = value;
                    OnShowBaseLineChanged();
                }
            }
        }

        [DefaultValue(1)]
        public virtual int BaseLineSize
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

        [DefaultValue(false)]
        public bool Movable
        {
            get { return _Movable; }
            set { _Movable = value; }
        }

        protected virtual int HeaderPadding
        {
            get 
            {
                return 0; 
            }
        }

        [Browsable(false)]
        public bool IsHorizontal
        {
            get { return Alignment == TabAlignment.Top || Alignment == TabAlignment.Bottom; }
        }

        [DefaultValue(false)]
        public bool IsActive
        {
            get { return _IsActive; }
            set 
            {
                if (_IsActive != value)
                {
                    _IsActive = value;
                    OnIsActiveChanged();
                }
            }
        }

        [DefaultValue(0)]
        public int TabRounded
        {
            get { return _TabRounded; }
            set
            {
                if (_TabRounded != value)
                {
                    _TabRounded = value;
                    OnTabRoundedChanged();
                }
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 30);
            }
        }

        [DefaultValue(0)]
        public int ItemMinimumSize
        {
            get { return _ItemMinimumSize; }
            set
            {
                if (_ItemMinimumSize != value)
                {
                    _ItemMinimumSize = value;
                    OnItemMinimumSizeChanged();
                }
            }
        }

        [DefaultValue(0)]
        public int ItemMaximumSize
        {
            get { return _ItemMaximumSize; }
            set
            {
                if (_ItemMaximumSize != value)
                {
                    _ItemMaximumSize = value;
                    OnItemMaximumSizeChanged();
                }
            }
        }

        protected virtual int InactivedItemLower
        {
            get { return 1; }
        }

        [DefaultValue(null)]
        public TabBarRenderer Renderer
        {
            get { return _Renderer; }
            set
            {
                if (_Renderer != value)
                {
                    _Renderer = value;
                    OnRendererChanged();
                }
            }
        }

        [Browsable(false)]
        public virtual TabBarRenderer DefaultRenderer
        {
            get
            {
                return new NormalTabBarRenderer(this);
            }
        }

        private void OnRendererChanged()
        {
            Invalidate();
        }

        private void OnSelectedItemChanged(TabItem old)
        {
            //this.LayoutItem(old);
            //this.LayoutItem(SelectedItem);
            PerformLayout();
            EnsureItemVisible(SelectedItem);
            Invalidate();

            if (SelectedItemChanged != null)
                SelectedItemChanged(this, EventArgs.Empty);

            //EnsureItemVisible(SelectedItem);
        }

        private void OnHoverItemChanged(TabItem old)
        {
            InvalidateItem(old);
            InvalidateItem(HoverItem);
            //this.Invalidate();

            if (HoverItem != null)
            {
                if (!string.IsNullOrEmpty(HoverItem.ToolTipText))
                    MyToolTip.SetToolTip(this, HoverItem.ToolTipText);
                else if (HalfDisplayItems.Contains(HoverItem))
                    MyToolTip.SetToolTip(this, HoverItem.Text);
                else
                    MyToolTip.SetToolTip(this, null);
            }
        }

        private void OnCloseButtonSizeChanged()
        {
            if (LayoutItems())
                Invalidate();
        }

        private void OnIconSizeChanged()
        {
            if (LayoutItems())
                Invalidate();
        }

        private void OnItemSpaceChanged()
        {
            if (LayoutItems())
                Invalidate();
        }

        private void OnShowBaseLineChanged()
        {
            Invalidate();
        }

        private void OnBaseLineSizeChanged()
        {
            LayoutItems();

            Invalidate();
        }

        private void OnAlignmentChanged()
        {
            LayoutItems();
            Invalidate();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            if (LayoutItems())
                Invalidate();
        }

        protected virtual void OnIsActiveChanged()
        {
            if (IsActiveChanged != null)
            {
                IsActiveChanged(this, EventArgs.Empty);
            }

            Invalidate();
        }

        private void OnTabRoundedChanged()
        {
            Invalidate();
        }

        protected virtual void OnItemMinimumSizeChanged()
        {
            if (LayoutItems())
                Invalidate();
        }

        protected virtual void OnItemMaximumSizeChanged()
        {
            if (LayoutItems())
                Invalidate();
        }

        protected virtual void OnMaxItemSizeChanged()
        {
            if (LayoutItems())
                Invalidate();
        }

        #endregion

        #region Styles
        Color _ItemBackColor = SystemColors.Control;
        Color _ItemForeColor = SystemColors.ControlText;
        Color _SelectedItemBackColor = SystemColors.Control;
        Color _SelectedItemForeColor = SystemColors.ControlText;
        Color _HoverItemBackColor = SystemColors.Highlight;
        Color _HoverItemForeColor = SystemColors.HighlightText;
        Color _BaseLineColor = SystemColors.ControlDark;

        public event EventHandler ItemBackColorChanged;
        public event EventHandler ItemForeColorChanged;
        public event EventHandler SelectedItemBackColorChanged;
        public event EventHandler SelectedItemForeColorChanged;
        public event EventHandler HoverItemBackColorChanged;
        public event EventHandler HoverItemForeColorChanged;
        public event EventHandler BaseLineColorChanged;

        [DefaultValue(typeof(Color), "Control"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color ItemBackColor
        {
            get { return _ItemBackColor; }
            set 
            {
                if (_ItemBackColor != value)
                {
                    _ItemBackColor = value;
                    OnItemBackColorChanged();
                }
            }
        }

        [DefaultValue(typeof(Color), "ControlText"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color ItemForeColor
        {
            get { return _ItemForeColor; }
            set 
            {
                if (_ItemForeColor != value)
                {
                    _ItemForeColor = value;
                    OnItemForeColorChanged();
                }
            }
        }

        [DefaultValue(typeof(Color), "Control"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color SelectedItemBackColor
        {
            get { return _SelectedItemBackColor; }
            set 
            {
                if (_SelectedItemBackColor != value)
                {
                    _SelectedItemBackColor = value;
                    OnSelectedItemBackColorChanged();
                }
            }
        }

        [DefaultValue(typeof(Color), "ControlText"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color SelectedItemForeColor
        {
            get { return _SelectedItemForeColor; }
            set 
            {
                if (_SelectedItemForeColor != value)
                {
                    _SelectedItemForeColor = value;
                    OnSelectedItemForeColorChanged();
                }
            }
        }

        [DefaultValue(typeof(Color), "Highlight"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color HoverItemBackColor
        {
            get { return _HoverItemBackColor; }
            set
            {
                if (_HoverItemBackColor != value)
                {
                    _HoverItemBackColor = value;
                    OnHoverItemBackColorChanged();
                }
            }
        }

        [DefaultValue(typeof(Color), "HighlightText"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color HoverItemForeColor
        {
            get { return _HoverItemForeColor; }
            set
            {
                if (_HoverItemForeColor != value)
                {
                    _HoverItemForeColor = value;
                    OnHoverItemForeColorChanged();
                }
            }
        }

        [DefaultValue(typeof(Color), "ControlDark"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        protected virtual void OnItemBackColorChanged()
        {
            if (ItemBackColorChanged != null)
            {
                ItemBackColorChanged(this, EventArgs.Empty);
            }

            Invalidate();
        }

        protected virtual void OnItemForeColorChanged()
        {
            if (ItemForeColorChanged != null)
            {
                ItemForeColorChanged(this, EventArgs.Empty);
            }

            Invalidate();
        }

        protected virtual void OnSelectedItemBackColorChanged()
        {
            if (SelectedItemBackColorChanged != null)
            {
                SelectedItemBackColorChanged(this, EventArgs.Empty);
            }

            Invalidate();
        }

        protected virtual void OnSelectedItemForeColorChanged()
        {
            if (SelectedItemForeColorChanged != null)
            {
                SelectedItemForeColorChanged(this, EventArgs.Empty);
            }

            Invalidate();
        }

        protected virtual void OnHoverItemBackColorChanged()
        {
            if (HoverItemBackColorChanged != null)
            {
                HoverItemBackColorChanged(this, EventArgs.Empty);
            }

            Invalidate();
        }

        protected virtual void OnHoverItemForeColorChanged()
        {
            if (HoverItemForeColorChanged != null)
            {
                HoverItemForeColorChanged(this, EventArgs.Empty);
            }

            Invalidate();
        }

        protected virtual void OnBaseLineColorChanged()
        {
            if (BaseLineColorChanged != null)
            {
                BaseLineColorChanged(this, EventArgs.Empty);
            }

            Invalidate();
        }

        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);

            if (theme != null && !DesignMode)
            {
                TabRounded = theme.RoundCorner;

                SelectedItemBackColor = theme.Colors.Window;//.Sharp;
                SelectedItemForeColor = theme.Colors.WindowText;// PaintHelper.FarthestColor(SelectedItemBackColor, theme.Colors.Dark, theme.Colors.Light);// theme.Colors.SharpText;
                ItemBackColor = theme.Colors.MediumLight;
                ItemForeColor = PaintHelper.FarthestColor(ItemBackColor, theme.Colors.Dark, theme.Colors.Light);
                HoverItemBackColor = theme.Colors.Sharp;
                HoverItemForeColor = PaintHelper.FarthestColor(HoverItemBackColor, theme.Colors.Dark, theme.Colors.Light);
                BaseLineColor = theme.Colors.BorderColor;
            }
        }

        #endregion

        #region method of item changed

        void OnItemsChanged(TabItem oldValue, TabItem newVlaue)
        {
            if (newVlaue != null)
            {
                newVlaue.TextChanged += new EventHandler(Item_TextChanged);
                newVlaue.IconChanged += new EventHandler(Item_IconChanged);
                newVlaue.VisibleChanged += new EventHandler(Item_VisibleChanged);
                newVlaue.CanCloseChanged += new EventHandler(Item_CanCloseChanged);
            }

            if (oldValue != null)
            {
                oldValue.TextChanged -= new EventHandler(Item_TextChanged);
                oldValue.IconChanged -= new EventHandler(Item_IconChanged);
                oldValue.VisibleChanged -= new EventHandler(Item_VisibleChanged);
                oldValue.CanCloseChanged -= new EventHandler(Item_CanCloseChanged);
            }

            this.RefreshDisplayItems();
            if (LayoutItems())
            {
                Invalidate();
            }
        }

        protected virtual void OnItemClose(TabItem tabItem)
        {
            if (ItemClose != null)
                ItemClose(this, new TabItemEventArgs(this, tabItem));
        }

        protected virtual bool OnItemClosing(TabItem tabItem)
        {
            TabItemCancelEventArgs ce = new TabItemCancelEventArgs(this, tabItem);
            if (ItemClosing != null)
                ItemClosing(this, ce);

            return !ce.Cancel;
        }

        private void Item_CanCloseChanged(object sender, EventArgs e)
        {
            if (LayoutItem(sender as TabItem))
            {
                Invalidate();
            }
        }

        private void Item_VisibleChanged(object sender, EventArgs e)
        {
            if (LayoutItems())
            {
                Invalidate();
            }
        }

        private void Item_IconChanged(object sender, EventArgs e)
        {
            if (LayoutItem(sender as TabItem))
            {
                Invalidate();
            }
        }

        private void Item_TextChanged(object sender, EventArgs e)
        {
            if (LayoutItem(sender as TabItem))
            {
                Invalidate();
            }
        }

        protected virtual void Items_ItemAdded(object sender, XListEventArgs<TabItem> e)
        {
            if (e.Item != null && e.Item.Bar != this)
                e.Item.Bar = this;

            if (e.Item != null && e.Item.DisplayIndex <= 0)
            {
                if (e.Index > -1 && DisplayItems != null && e.Index < DisplayItems.Count)
                {
                    e.Item.DisplayIndex = DisplayItems[e.Index].DisplayIndex;
                    foreach (var item in DisplayItems)
                    {
                        if (item.DisplayIndex >= e.Item.DisplayIndex)
                            item.DisplayIndex++;
                    }
                }
                else
                {
                    e.Item.DisplayIndex = this.GetMaxDisplayIndex() + 1;
                }
            }

            OnItemsChanged(null, e.Item);
            if (Items.Count == 1)
            {
                SelectedItem = Items[0];
            }
        }

        protected virtual void Items_ItemChanged(object sender, XListValueEventArgs<TabItem> e)
        {
            OnItemsChanged(e.OldValue, e.NewValue);
        }

        protected virtual void Items_ItemRemoved(object sender, XListEventArgs<TabItem> e)
        {
            if (e.Item != null && e.Item.Bar == this)
                e.Item.Bar = null;

            OnItemsChanged(e.Item, null);
            if (e.Item == SelectedItem)
            {
                if (Items.Count > e.Index && e.Index > -1)
                    SelectedItem = Items[e.Index];
                else if (Items.Count > e.Index - 1 && e.Index > 0)
                    SelectedItem = Items[e.Index - 1];
                else if (Items.Count > 0)
                    SelectedItem = Items[0];
                else
                    SelectedItem = null;
            }
        }

        private void OnItemMoved()
        {
            if (ItemMoved != null)
                ItemMoved(this, EventArgs.Empty);
        }

        #endregion

        #region Mouse
        ToolTip ToolTip;
        TabItem _HoverItem = null;
        TabItem MouseDownItem = null;
        HitResult _DownHitResult = HitResult.Empty;
        HitResult _HoverHitResult = HitResult.Empty;
        Dictionary<TabItem, float> TempView = new Dictionary<TabItem, float>();
        Point MouseDownPoint;

        internal HitResult DownHitResult
        {
            get { return _DownHitResult; }
            set
            {
                if (_DownHitResult != value)
                {
                    var old = _DownHitResult;
                    _DownHitResult = value;
                    OnDownHitResultChanged(old);
                }
            }
        }

        internal HitResult HoverHitResult
        {
            get { return _HoverHitResult; }
            set
            {
                if (_HoverHitResult != value)
                {
                    HitResult old = _HoverHitResult;
                    _HoverHitResult = value;
                    OnHoverHitResultChanged(old);
                }
            }
        }

        void InitializeMouse()
        {
            DownHitResult = HitResult.Empty;
        }

        void OnDownHitResultChanged(HitResult old)
        {
            if (!old.IsEmpty)
                Invalidate(old);
            if (!DownHitResult.IsEmpty)
                Invalidate(DownHitResult);
        }

        void OnHoverHitResultChanged(HitResult old)
        {
            HoverItem = HoverHitResult.Item;

            if (!old.IsEmpty)
                Invalidate(old);
            if (!HoverHitResult.IsEmpty)
                Invalidate(HoverHitResult);

            if (HoverHitResult.ButtonIndex > -1)
            {
                TabBarButton button = GetButton(HoverHitResult.ButtonIndex);
                if (button != null)
                    ShowToolTip(button.ToolTipText);
            }
            else if (HoverHitResult.Item != null)
            {
                if (HoverHitResult.InCloseButton)
                {
                    ShowToolTip(Lang._("Close"));
                    //ShowToolTip(LanguageManage.GetText("Close"));
                }
                else
                {
                    ShowToolTip("");
                    //ShowToolTip(HoverHitResult.Item.Text);
                }
            }
            else
            {
                HideToolTip();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (HoverHitResult.ButtonIndex > -1)
            {
                InvalidateButton(HoverHitResult.ButtonIndex);
            }
            HoverHitResult = HitResult.Empty;

            HideToolTip();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                HitResult hr = HitTest(e.X, e.Y);
                if (HitResult.IsTabItem(hr))
                {
                    if (hr.Item.Selectable)
                        SelectedItem = hr.Item;
                    MouseDownItem = hr.Item;
                }
                else
                {
                    MouseDownItem = null;
                }

                if (hr.ButtonIndex != DownHitResult.ButtonIndex)
                {
                    InvalidateButton(hr.ButtonIndex);
                    InvalidateButton(DownHitResult.ButtonIndex);
                    if (hr.ButtonIndex > -1)
                        OnButtonMouseDown(hr.ButtonIndex);
                }
                DownHitResult = hr;
                Capture = true;
            }

            MouseDownPoint = new Point(e.X, e.Y);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (LastMoveGraduatePos > -1)
            {
                DrawMovingGraduate(LastMoveGraduatePos);
                LastMoveGraduatePos = -1;
            }

            var mouseDownItem = MouseDownItem;
            MouseDownItem = null;
            Capture = false;
            if (MoveDrager.IsDraging)
            {
                MoveDrager.EndDrag();
                EndDrag(MoveDrager.DragObject as TabItem, e.X, e.Y);
            }
            else
            {
                HitResult hr = HitTest(e.X, e.Y);
                if (hr.ButtonIndex > -1)
                {
                    InvalidateButton(hr.ButtonIndex);
                    OnButtonMouseUp(hr.ButtonIndex);
                    int buttonIndex = DownHitResult.ButtonIndex;
                    DownHitResult = HitResult.Empty;

                    if (hr.ButtonIndex == buttonIndex)
                    {
                        OnButtonClick(buttonIndex);
                    }
                }
                else if (HitResult.IsTabItem(hr) && hr.Item == mouseDownItem && e.Button == MouseButtons.Left)
                {
                    DownHitResult = HitResult.Empty;
                    if (hr.InCloseButton)
                    {
                        if (OnItemClosing(hr.Item))
                            OnItemClose(hr.Item);
                    }
                    else
                    {
                        OnTabItemClick(hr.Item);
                    }
                }
            }

            DownHitResult = HitResult.Empty;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.Movable && MouseDownItem != null)
            {
                if (MoveDrager.IsDraging)
                {
                    //OnMoveDraging(e.X, e.Y);
                    MoveDrager.Drag(new Point(e.X, e.Y));
                }
                else
                {
                    if (Math.Abs(e.X - MouseDownPoint.X) > SystemInformation.DragSize.Width
                        || Math.Abs(e.Y - MouseDownPoint.Y) > SystemInformation.DragSize.Height)
                    {
                        Rectangle rectMoving;
                        if (IsHorizontal)
                            rectMoving = new Rectangle(MouseDownItem.Bounds.X, 0, MouseDownItem.Bounds.Width, Height);
                        else
                            rectMoving = new Rectangle(0, MouseDownItem.Bounds.Y, Width, MouseDownItem.Bounds.Height);

                        MoveDrager.BeginDrag(MouseDownItem,
                            rectMoving,
                            ClientRectangle,
                            new Point(e.X, e.Y),
                            DraggingImageCreator);
                        if (!Capture)
                            Capture = true;
                    }
                }
            }
            else
            {
                HoverHitResult = HitTest(e.X, e.Y);
            }
        }

        void ShowToolTip(string text)
        {
            Point pt = PointToClient(Control.MousePosition);
            ShowToolTipAt(pt.X + 16, pt.Y + 16, text);
        }

        void ShowToolTipAt(int x, int y, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                HideToolTip();
            }
            else
            {
                if (ToolTip == null)
                {
                    ToolTip = new ToolTip();
                }

                ToolTip.SetToolTip(this, text);
                ToolTip.Active = true;
                //ToolTip.Show(text, this, x, y, 5000);
            }
        }

        void HideToolTip()
        {
            if (ToolTip != null)
            {
                ToolTip.Active = false;
                //ToolTip.Hide(this);
            }
        }

        Bitmap DraggingImageCreator(object dragginObject)
        {
            if (!(dragginObject is TabItem))
                return null;

            TabItem ti = (TabItem)dragginObject;
            Bitmap bmp;
            if (IsHorizontal)
                bmp = new Bitmap(ti.Bounds.Width, Height);
            else
                bmp = new Bitmap(Width, ti.Bounds.Height);
            using (Graphics grf = Graphics.FromImage(bmp))
            {
                PaintHelper.SetHighQualityRender(grf);
                TabBarRenderer renderer = Renderer ?? DefaultRenderer;
                Rectangle clipRect;
                if(IsHorizontal)
                {
                    clipRect = new Rectangle(ti.Bounds.X, 0, ti.Bounds.Width, Height);
                    grf.TranslateTransform(-ti.Bounds.X, 0);
                }
                else
                {
                    clipRect = new Rectangle(0, ti.Bounds.Y, Width, ti.Bounds.Height);
                    grf.TranslateTransform(0, -ti.Bounds.Y);
                }
                PaintEventArgs pe = new PaintEventArgs(grf, clipRect);
                renderer.DrawBackground(pe);
                renderer.DrawItem(new TabItemPaintEventArgs(pe, this, ti));
            }

            return bmp;
        }

        void OnTabItemClick(TabItem tabItem)
        {
            if (tabItem != null)
            {
                tabItem.OnClick();
            }
        }
        #endregion

        #region Paint

        void InitializePaint()
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var renderer = Renderer ?? DefaultRenderer;
            if (renderer == null)
            {
                base.OnPaint(e);
                return;
            }

            renderer.Initialize(e);
            renderer.DrawBackground(e);

            var rect = TabArea;// GetWorkArea();
            var region = e.Graphics.Clip;
            e.Graphics.Clip = new Region(IsHorizontal ? new Rectangle(rect.X, 0, rect.Width, Height) : new Rectangle(0, rect.Y, Width, rect.Height));
            foreach (var ti in DisplayItems)
            {
                renderer.DrawItem(new TabItemPaintEventArgs(e, this, ti));
            }
            e.Graphics.Clip = region;

            renderer.DrawButtons(e, LeftButtons.ToArray());
            renderer.DrawButtons(e, RightButtons.ToArray());

            // special tabs
            foreach (var tab in LeftSpecialTabs)
            {
                if (tab.Visible)
                    renderer.DrawItem(new TabItemPaintEventArgs(e, this, tab));
            }
            foreach (var tab in RightSpecialTabs)
            {
                if (tab.Visible)
                    renderer.DrawItem(new TabItemPaintEventArgs(e, this, tab));
            }

            if (ShowBaseLine && BaseLineSize > 0 && SelectedItem != null)
            {
                renderer.DrawBaseLine(e);
            }

            renderer.Close();
        }
        
        void DrawMovingGraduate(int pos)
        {
            Rectangle wa = TabArea;// GetWorkArea();
            if (IsHorizontal)
            {
                Point pt = new Point(pos, ClientRectangle.Top);
                if (wa.Left > pos || wa.Right < pos)
                    return;
                pt = PointToScreen(pt);
                PaintHelper.DrawReversibleFrame(new Rectangle(pt.X - 2, pt.Y, 4, ClientSize.Height), SystemColors.ActiveCaption);
            }
            else
            {
                Point pt = new Point(ClientRectangle.Left, pos);
                if (wa.Top > pos || wa.Bottom < pos)
                    return;
                pt = PointToScreen(pt);
                PaintHelper.DrawReversibleFrame(new Rectangle(pt.X, pt.Y - 2, ClientSize.Width, 4), SystemColors.ActiveCaption);
            }
            LastMoveGraduatePos = pos;
        }

        void Invalidate(HitResult hitResult)
        {
            if (hitResult.ButtonIndex > -1)
            {
                InvalidateButton(hitResult.ButtonIndex);
            }
            else if (hitResult.Item != null)
            {
                InvalidateItem(hitResult.Item);
            }
        }

        void InvalidateButton(int index)
        {
            TabBarButton button = GetButton(index);
            if (button != null)
            {
                Invalidate(button.Bounds);
            }
        }

        void InvalidateItem(TabItem item)
        {
            if (item != null)
            {
                Invalidate(item.Bounds);
            }
        }
        #endregion

        #region Layout
        Rectangle TabArea { get; set; }

        Rectangle GetWorkArea()
        {
            Rectangle workArea = ClientRectangle.Inflate(Padding);

            switch (Alignment)
            {
                case TabAlignment.Left:
                    workArea.X += HeaderPadding;
                    break;
                case TabAlignment.Top:
                    workArea.Y += HeaderPadding;
                    break;
                case TabAlignment.Bottom:
                    workArea.Height -= HeaderPadding;
                    break;
                case TabAlignment.Right:
                    workArea.Width -= HeaderPadding;
                    break;
            }

            if (ShowBaseLine && BaseLineSize > 0)
            {
                workArea.Height -= BaseLineSize;
            }

            return workArea;
        }

        bool LayoutItems()
        {
            if (IsUpdating || !Created || (DisplayItems.Count == 0 && LeftButtons.IsEmpty && RightButtons.IsEmpty))
                return false;

            MeasureItemsSize(HalfDisplayItems);
            CalculatePositions();

            /*
            Rectangle workArea = GetWorkArea();
            Rectangle tabArea = workArea;

            Graphics graphics = null;
            if (Created)
                graphics = CreateGraphics();

            //
            int leftSize = LayoutLeftButtons(Padding.Left);
            int rightSize = LayoutRightButtons();

            //
            int maxSize = 0;
            if (IsHorizontal)
            {
                maxSize = (workArea.Width - leftSize - rightSize) / GetVisibleItemsCount();
            }
            else
            {
                maxSize = (workArea.Height - leftSize - rightSize) / GetVisibleItemsCount();
            }

            //
            HalfDisplayItems.Clear();
            if (IsHorizontal)
            {
                tabArea.Width = tabArea.Right - leftSize;
                tabArea.X = leftSize;

                int x = leftSize + (workArea.Left - ScrollPos);
                //x += Margin.Left;
                foreach (TabItem ti in DisplayItems)
                {
                    if (!ti.Visible)
                        continue;
                    int width = MeasureItemSize(ti, graphics);
                    if (width > maxSize)
                    {
                        width = maxSize;
                        HalfDisplayItems.Add(ti);
                    }

                    if (ti == SelectedItem)
                        ti.Bounds = new Rectangle(x, workArea.Top, width, workArea.Height);
                    else
                        ti.Bounds = new Rectangle(x, workArea.Top + InactivedItemLower, width, workArea.Height - InactivedItemLower);
                    x += width + ItemSpace;
                }
            }
            else
            {
                tabArea.Height = tabArea.Bottom - leftSize;
                tabArea.Y = leftSize;

                int y = leftSize + (workArea.Top - ScrollPos);
                //y += Margin.Top;
                foreach (TabItem ti in DisplayItems)
                {
                    if (!ti.Visible)
                        continue;
                    int height = MeasureItemSize(ti, graphics);
                    if (height > maxSize)
                    {
                        height = maxSize;
                        HalfDisplayItems.Add(ti);
                    }

                    if (ti == SelectedItem)
                        ti.Bounds = new Rectangle(workArea.Left, y, workArea.Width, height);
                    else
                        ti.Bounds = new Rectangle(workArea.Left + InactivedItemLower, y, workArea.Width - InactivedItemLower, height);
                    y += height + ItemSpace;
                }
            }

            if (IsHorizontal)
            {
                tabArea.Width -= rightSize;
            }
            else
            {
                tabArea.Height -= rightSize;
            }
            TabArea = tabArea;*/

            return true;
        }

        bool LayoutItem(TabItem item)
        {
            if (IsUpdating || item == null || DisplayItems.Count == 0 || !Created)
                return false;

            if (HalfDisplayItems.Contains(item))
                HalfDisplayItems.Remove(item);

            Rectangle workArea = GetWorkArea();
            if (Alignment == TabAlignment.Top || Alignment == TabAlignment.Bottom)
                item.Size = new Size(MeasureItemSize(item, HalfDisplayItems), workArea.Height);
            else if (Alignment == TabAlignment.Left || Alignment == TabAlignment.Right)
                item.Size = new Size(workArea.Width, MeasureItemSize(item, HalfDisplayItems));

            CalculatePositions();

            return true;
            //return LayoutItems();//
            /*if (IsUpdating || item == null || DisplayItems.Count == 0 || !Created)
                return false;

            Rectangle workArea = TabArea;// GetWorkArea();

            Graphics graphics = null;
            if (Created)
                graphics = CreateGraphics();

            bool selected = item == SelectedItem;
            if (IsHorizontal)
            {
                bool isbegin = false;
                int x = 0;
                foreach (TabItem ti in DisplayItems)
                {
                    if (!ti.Visible)
                        continue;
                    if (isbegin || ti == item)
                    {
                        int width = ti.Bounds.Width;
                        if (!isbegin)
                        {
                            width = MeasureItemSize(ti, graphics);
                            isbegin = true;
                            x = ti.Bounds.Left;
                        }

                        if (selected)
                            ti.Bounds = new Rectangle(x, workArea.Top, width, workArea.Height);
                        else
                            ti.Bounds = new Rectangle(x, workArea.Top + InactivedItemLower, width, workArea.Height - InactivedItemLower);
                        x = ti.Bounds.Right + ItemSpace;
                    }
                }
            }
            else
            {
                bool isbegin = false;
                int y = 0;
                foreach (TabItem ti in DisplayItems)
                {
                    if (!ti.Visible)
                        continue;
                    if (isbegin || ti == item)
                    {
                        int height = ti.Bounds.Height;
                        if (!isbegin)
                        {
                            height = MeasureItemSize(ti, graphics);
                            isbegin = true;
                            y = ti.Bounds.Top;
                        }

                        if (selected)
                            ti.Bounds = new Rectangle(workArea.Left, y, workArea.Width, height);
                        else
                            ti.Bounds = new Rectangle(workArea.Left + 2, y, workArea.Width - 2, height);
                        y = ti.Bounds.Bottom + ItemSpace;
                    }
                }
            }

            return true;*/
        }

        void MeasureItemsSize(List<TabItem> overflowItems)
        {
            overflowItems.Clear();
            Rectangle workArea = GetWorkArea();
            if (Alignment == TabAlignment.Top || Alignment == TabAlignment.Bottom)
            {
                foreach (TabItem tab in DisplayItems)
                {
                    tab.Size = new Size(MeasureItemSize(tab, overflowItems), workArea.Height);
                }

                // special tabs
                foreach (TabItem tab in LeftSpecialTabs)
                {
                    tab.Size = new Size(MeasureItemSize(tab, overflowItems), workArea.Height);
                }
                foreach (TabItem tab in RightSpecialTabs)
                {
                    tab.Size = new Size(MeasureItemSize(tab, overflowItems), workArea.Height);
                }
            }
            else if (Alignment == TabAlignment.Left || Alignment == TabAlignment.Right)
            {
                foreach (TabItem tab in DisplayItems)
                {
                    tab.Size = new Size(workArea.Width, MeasureItemSize(tab, overflowItems));
                }

                // special tabs
                foreach (TabItem tab in LeftSpecialTabs)
                {
                    tab.Size = new Size(workArea.Width, MeasureItemSize(tab, overflowItems));
                }
                foreach (TabItem tab in RightSpecialTabs)
                {
                    tab.Size = new Size(workArea.Width, MeasureItemSize(tab, overflowItems));
                }
            }
        }

        int MeasureItemSize(TabItem ti, List<TabItem> overflowItems)
        {
            int size = 0;
            if (!string.IsNullOrEmpty(ti.Text))
            {
                size = TextRenderer.MeasureText(ti.Text, Font).Width + ItemPadding.Horizontal + 2;
            }

            if (ti.Icon != null)
            {
                size += IconSize.Width + 4;
            }

            if (ti.CanClose && CloseButtonSize.Width > 0)
            {
                size += CloseButtonSize.Width + 4;
            }

            if (ti.Padding != Padding.Empty)
            {
                size += ti.Padding.Horizontal;
            }

            if (MaxItemSize > 0 && size > MaxItemSize)
            {
                size = Math.Min(MaxItemSize, size);
                overflowItems.Add(ti);
            }

            return size;
        }

        void CalculatePositions()
        {
            Rectangle workArea = GetWorkArea();

            // buttons
            int left, right;
            if (IsHorizontal && RightToLeft == RightToLeft.Yes)
            {
                right = LayoutLeftButtons();
                left = LayoutRightButtons();
            }
            else
            {
                left = LayoutLeftButtons();
                right = LayoutRightButtons();
            }

            // special tabs
            if (LeftSpecialTabs.Count > 0)
            {
                left += ItemSpace;
                foreach (TabItem tab in LeftSpecialTabs)
                {
                    tab.Location = new Point(left, workArea.Top);
                    left += tab.Bounds.Width + ItemSpace;
                }
            }
            if (RightSpecialTabs.Count > 0)
            {
                right += ItemSpace;
                foreach (TabItem tab in RightSpecialTabs)
                {
                    right += tab.Bounds.Width + ItemSpace;
                }
            }

            switch (Alignment)
            {
                case TabAlignment.Bottom:
                    workArea.Y += BaseLineSize;
                    break;
                case TabAlignment.Right:
                    workArea.X += BaseLineSize;
                    break;
            }

            // tabs
            TabArea = CalculateTabArea(workArea, left, right);
            if (IsHorizontal)
            {
                int x;
                if (RightToLeft == RightToLeft.Yes)
                    x = workArea.Right - right;
                else
                    x = workArea.Left + left;

                if (AllowScrollPage)
                {
                    if (RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                        x += ScrollPos;
                    else
                        x -= ScrollPos;
                }

                foreach (var ti in DisplayItems)
                {
                    if (!ti.Visible)
                        continue;

                    if (RightToLeft == RightToLeft.Yes)
                    {
                        ti.Location = new Point(x - ti.Size.Width, workArea.Top);
                        x -= ti.Size.Width + ItemSpace;
                    }
                    else
                    {
                        ti.Location = new Point(x, workArea.Top);
                        x += ti.Size.Width + ItemSpace;
                    }
                }

                // right special tabs
                x = Math.Min(x, TabArea.Right + ItemSpace);
                foreach (var tab in RightSpecialTabs)
                {
                    tab.Location = new Point(x, workArea.Top);
                    x += tab.Size.Width + ItemSpace;
                }
            }
            else
            {
                int y = workArea.Top + left;
                if (AllowScrollPage)
                {
                    y -= ScrollPos;
                }

                foreach (var ti in DisplayItems)
                {
                    if (!ti.Visible)
                        continue;

                    ti.Location = new Point(workArea.Left, y);
                    y += ti.Size.Height + ItemSpace;
                }
            }
        }

        Rectangle CalculateTabArea(Rectangle workArea, int left, int right)
        {
            if (IsHorizontal)
            {
                return new Rectangle(left, workArea.Top, workArea.Width - right - left, workArea.Height);
            }
            else
            {
                return new Rectangle(workArea.Left, left, workArea.Width, workArea.Height - right - left);
            }
        }

        protected virtual int MeasureItemSize(TabItem ti, Graphics graphics)
        {
            int size = 0;
            if (!string.IsNullOrEmpty(ti.Text))
            {
                if (graphics != null)
                    size = (int)Math.Ceiling(graphics.MeasureString(ti.Text, Font).Width);
                else
                    size = TextRenderer.MeasureText(ti.Text, Font).Width + 1;
            }

            if (ti.Icon != null)
            {
                size += IconSize.Width + 4;
            }

            if (ti.CanClose && CloseButtonSize.Width > 0)
            {
                size += CloseButtonSize.Width + 4;
            }

            if (ItemPadding != Padding.Empty)
            {
                size += ItemPadding.Horizontal;
            }

            if (ItemMinimumSize > 0)
            {
                size = Math.Max(ItemMinimumSize, size);
            }

            if (ItemMaximumSize > 0)
            {
                size = Math.Min(ItemMaximumSize, size);
            }

            return size;
        }

        int GetVisibleItemsCount()
        {
            int c = 0;
            foreach (TabItem item in DisplayItems)
                if (item.Visible)
                    c++;
            return c;
        }

        //private Rectangle GetCloseButtonRect(Rectangle itemRect)
        //{
        //    if (IsHorizontal)
        //    {
        //        return new Rectangle(itemRect.Right - CloseButtonSize.Width - ItemPadding.Right,
        //            itemRect.Top + (itemRect.Height - CloseButtonSize.Height) / 2,
        //            CloseButtonSize.Width, CloseButtonSize.Height);
        //    }
        //    else
        //    {
        //        return new Rectangle(itemRect.Left + (itemRect.Width - CloseButtonSize.Width) / 2,
        //            itemRect.Bottom - CloseButtonSize.Height - ItemPadding.Right,
        //            CloseButtonSize.Width, CloseButtonSize.Height);
        //    }
        //}

        public TabItem GetItemAt(int x, int y)
        {
            HitResult hr = HitTest(x, y);
            return hr.Item;

            //Rectangle rect = GetWorkArea();
            //if (rect.Contains(x, y))
            //{
            //    foreach (TabItem ti in DisplayItems)
            //    {
            //        if (ti.Visible && ti.Bounds.Contains(x, y))
            //            return ti;
            //    }
            //}

            //return null;
        }

        HitResult HitTest(int x, int y)
        {
            HitResult hr = HitResult.Empty;

            foreach (TabBarButton button in LeftButtons)
            {
                if (button.Visible && button.Bounds.Contains(x, y))
                {
                    hr.ButtonIndex = button.Index;
                    return hr;
                }
            }

            foreach (TabBarButton button in RightButtons)
            {
                if (button.Visible && button.Bounds.Contains(x, y))
                {
                    hr.ButtonIndex = button.Index;
                    return hr;
                }
            }

            foreach (TabItem tab in LeftSpecialTabs)
            {
                if (tab.Visible && tab.Bounds.Contains(x, y))
                {
                    hr.Item = tab;
                    return hr;
                }
            }

            foreach (TabItem tab in RightSpecialTabs)
            {
                if (tab.Visible && tab.Bounds.Contains(x, y))
                {
                    hr.Item = tab;
                    return hr;
                }
            }

            hr.ButtonIndex = -1;
            Rectangle rect = TabArea;
            if (rect.Contains(x, y))
            {
                foreach (TabItem ti in DisplayItems)
                {
                    if (ti.Visible && ti.Enabled && ti.Bounds.Contains(x, y))
                    {
                        hr.Item = ti;
                        if (ti.CanClose && ti.GetCloseButtonRect().Contains(x, y))
                        {
                            hr.InCloseButton = true;
                        }
                        break;
                    }
                }
            }

            return hr;
        }

        TabItem GetItemNear(int x, int y)
        {
            if (DisplayItems.Count == 0)
                return null;

            if (Alignment == TabAlignment.Left || Alignment == TabAlignment.Right)
            {
                if (y < 0)
                    return DisplayItems[0];
                else if (y > DisplayItems[DisplayItems.Count - 1].Bounds.Bottom)
                    return DisplayItems[DisplayItems.Count - 1];
                else
                {
                    foreach (TabItem ti in DisplayItems)
                    {
                        if (ti.Bounds.Y <= y && ti.Bounds.Bottom > y)
                            return ti;
                    }
                }
            }
            else if (Alignment == TabAlignment.Top || Alignment == TabAlignment.Bottom)
            {
                if (x < 0)
                    return DisplayItems[0];
                else if (x > DisplayItems[DisplayItems.Count - 1].Bounds.Right)
                    return DisplayItems[DisplayItems.Count - 1];
                else
                {
                    foreach (TabItem ti in DisplayItems)
                    {
                        if (ti.Bounds.X <= x && ti.Bounds.Right > x)
                            return ti;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 根据指定的坐标, 计算其所处的位置, 0.5的偏移量表示在一个项目之前或者之后
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        float GetTempIndexAt(int x, int y)
        {
            if (DisplayItems.Count == 0)
                return 0;

            if (Alignment == TabAlignment.Left || Alignment == TabAlignment.Right)
            {
                if (y < 0)
                    return 0.5f;
                else if (y > DisplayItems[DisplayItems.Count - 1].Bounds.Bottom)
                    return DisplayItems.Count - 0.5f;
                else
                {
                    foreach (TabItem ti in DisplayItems)
                    {
                        if (ti.Bounds.Y <= y && ti.Bounds.Bottom > y)
                        {
                            if (y < ti.Bounds.Y + ti.Bounds.Height / 2)
                                return ti.DisplayIndex - 0.5f;
                            else
                                return ti.DisplayIndex + 0.5f;
                        }
                    }
                }
            }
            else if (Alignment == TabAlignment.Top || Alignment == TabAlignment.Bottom)
            {
                if (x < 0)
                    return 0.5f;
                else if (x > DisplayItems[DisplayItems.Count - 1].Bounds.Right)
                    return DisplayItems.Count - 0.5f;
                else
                {
                    foreach (TabItem ti in DisplayItems)
                    {
                        if (ti.Bounds.X <= x && ti.Bounds.Right > x)
                        {
                            if (x < ti.Bounds.X + ti.Bounds.Width / 2)
                                return ti.DisplayIndex - 0.5f;
                            else
                                return ti.DisplayIndex + 0.5f;
                        }
                    }
                }
            }

            return 0.5f;
        }

        bool EnsureItemVisible(TabItem item)
        {
            if (item == null || !AllowScrollPage)
                return false;

#if DEBUG
            if (Items.Count == 2 && Items[1].Text == "Remark")
                Console.Write("");
#endif

            int pos = 0;
            if (IsHorizontal)
            {
                int size = item.Bounds.Left + ScrollPos - TabArea.Left;
                //foreach (var ti in DisplayItems)
                //{
                //    if (ti == item)
                //        break;
                //    size += ti.Bounds.Width;
                //}

                if (item.Bounds.Left < TabArea.Left)
                    pos = size;
                else if (item.Bounds.Right > TabArea.Right)
                    pos = (item.Bounds.Right + ScrollPos) - TabArea.Width;
                else
                    return false;
            }
            else
            {
                int size = item.Bounds.Top + ScrollPos - TabArea.Top;
                //foreach (var ti in DisplayItems)
                //{
                //    if (ti == item)
                //        break;
                //    size += ti.Bounds.Height;
                //}

                if (item.Bounds.Top < TabArea.Top)
                    pos = size;
                else if (item.Bounds.Bottom > TabArea.Bottom)
                    pos = (item.Bounds.Bottom + ScrollPos) - TabArea.Height;
                else
                    return false;
            }

            pos = Math.Max(0, pos);
            if (ScrollPos != pos)
            {
                ScrollPos = pos;
                return true;
            }

            return false;
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);

            StandardButtonSize *= factor.Width;
            ButtonSpace *= factor.Width;

            foreach (TabBarButton button in AllButtons)
            {
                if (button.CustomSize > 0)
                    button.CustomSize *= factor.Width;
                button.CustomSpace *= factor.Width;
            }

            LayoutItems();
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (Created && LayoutItems())
            {
                if (SelectedItem != null)
                    EnsureItemVisible(SelectedItem);

                Invalidate();
            }
        }

        public struct HitResult
        {
            TabItem _Item;
            bool _InCloseButton;
            int _ButtonIndex;
            public static readonly HitResult Empty;

            static HitResult()
            {
                Empty.Item = null;
                Empty.InCloseButton = false;
                Empty.ButtonIndex = -1;
            }

            public HitResult(TabItem ti)
            {
                _Item = ti;
                _InCloseButton = false;
                _ButtonIndex = -1;
            }

            public HitResult(TabItem ti, bool inCloseButton)
            {
                _Item = ti;
                _InCloseButton = inCloseButton;
                _ButtonIndex = -1;
            }

            public TabItem Item
            {
                get { return _Item; }
                set { _Item = value; }
            }

            public bool InCloseButton
            {
                get { return _InCloseButton; }
                set { _InCloseButton = value; }
            }

            public int ButtonIndex
            {
                get { return _ButtonIndex; }
                set { _ButtonIndex = value; }
            }

            public static bool operator !=(HitResult hr1, HitResult hr2)
            {
                return hr1.ButtonIndex != hr2.ButtonIndex
                    || hr1.Item != hr2.Item
                    || hr1.InCloseButton != hr2.InCloseButton;
            }

            public static bool operator ==(HitResult hr1, HitResult hr2)
            {
                return hr1.ButtonIndex == hr2.ButtonIndex
                    && hr1.Item == hr2.Item
                    && hr1.InCloseButton == hr2.InCloseButton;
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                return (HitResult)obj == this;
            }

            public override int GetHashCode()
            {
                return string.Format("{0}, {1}, {2}", Item, InCloseButton, ButtonIndex).GetHashCode();
            }

            public static bool IsTabItem(HitResult hitResult)
            {
                return hitResult != null
                    && hitResult.Item != null;
            }

            public bool IsEmpty
            {
                get
                {
                    return ButtonIndex < 0 && Item == null;
                }
            }
        }
        #endregion

        #region Scroll

        [DefaultValue(false)]
        public bool AllowScrollPage { get; set; }

        [Browsable(false), DefaultValue(0)]
        public int ScrollPos
        {
            get { return _ScrollPos; }
            set
            {
                if (_ScrollPos != value)
                {
                    _ScrollPos = value;
                    OnScrollPosChanged();
                }
            }
        }

        void OnScrollPosChanged()
        {
            if (LayoutItems())
                Invalidate();
        }

        public void ScrollToFirst()
        {
            if (!DisplayItems.IsNullOrEmpty())
            {
                EnsureItemVisible(DisplayItems.First());
            }
        }

        public void ScrollToPrev()
        {
            var item = GetFirstVisiableItem();
            if(item != null && item != DisplayItems.First())
            {
                var preItem = DisplayItems[DisplayItems.IndexOf(item) - 1];
                EnsureItemVisible(preItem);
            }
        }

        public void ScrollToNext()
        {
            var item = GetLastVisiableItem();
            if (item != null && item != DisplayItems.Last())
            {
                var nextItem = DisplayItems[DisplayItems.IndexOf(item) + 1];
                EnsureItemVisible(nextItem);
            }
        }

        public void ScrollToLast()
        {
            var lastItem = DisplayItems.Last();
            //var fullWidth = lastItem.Bounds.Right + ScrollPos;
            //ScrollPos = fullWidth - TabArea.Width;
            EnsureItemVisible(lastItem);
        }

        TabItem GetFirstVisiableItem()
        {
            foreach (var di in DisplayItems)
            {
                if (di.Bounds.Right >= TabArea.Left)
                    return di;
            }
            return null;
        }

        TabItem GetLastVisiableItem()
        {
            foreach (var di in DisplayItems.Reverse<TabItem>())
            {
                if (di.Bounds.Right < TabArea.Right)
                    return di;
            }

            return null;
        }
        #endregion

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Escape)
            {
                if (Movable && MoveDrager != null && MoveDrager.IsDraging)
                {
                    MoveDrager.CancelDraging();
                }
            }
        }

        void EndDrag(TabItem item, int x, int y)
        {
            TabItem ti = GetItemNear(x, y);
            if (ti == null || ti == item)
                return;

            MoveItem(item, ti);
        }

        void OnMoveDraging(int x, int y)
        {
            TabItem hi = this.GetItemNear(x, y);

            if (lastMoveNearItem != hi)
            {
                if (LastMoveGraduatePos > -1)
                    DrawMovingGraduate(LastMoveGraduatePos);
                if (hi != null)
                {
                    DrawMovingGraduate(hi.Bounds.Right);
                }
                lastMoveNearItem = hi;
            }
        }

        void MoveItem(TabItem item, TabItem afterThis)
        {
            if (item == null || afterThis == null)
                return;

            int i_f = item.DisplayIndex;
            int i_t = afterThis.DisplayIndex;

            if (i_f > i_t)
            {
                item.DisplayIndex = i_t + 1;
                for (int i = i_t + 1; i < i_f; i++)
                {
                    if (i < DisplayItems.Count)
                        DisplayItems[i].DisplayIndex++;
                }
            }
            else
            {
                item.DisplayIndex = i_t;
                for (int i = i_f + 1; i <= i_t; i++)
                {
                    if (i < DisplayItems.Count)
                        DisplayItems[i].DisplayIndex--;
                }
            }

            DisplayItems.Sort(new TabItemDisplayIndexComparer());
            LayoutItems();
            Invalidate();

            OnItemMoved();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (LayoutItems())
            {
                Invalidate();
            }
        }

        void DropDownMenu()
        {
            if (this.Items.Count == 0)
                return;

            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Renderer = UITheme.Default.ToolStripRenderer;
            menu.Font = Font;
            //menu.Renderer = new MyToolStripReander();
            foreach (TabItem ti in DisplayItems)
            {
                ToolStripMenuItem mi = new ToolStripMenuItem();
                mi.Text = ti.ToString();
                mi.Image = ti.Icon;
                mi.Tag = ti;
                mi.Checked = ti == SelectedItem;
                mi.Click += new EventHandler(MenuItemDropDown_Click);
                menu.Items.Add(mi);
            }

            Point pt = new Point(0, 0);
            ToolStripDropDownDirection dic = ToolStripDropDownDirection.Default;
            switch (Alignment)
            {
                case TabAlignment.Top:
                    pt = new Point(DropDownButton.Bounds.Right, DropDownButton.Bounds.Bottom);
                    dic = ToolStripDropDownDirection.BelowLeft;
                    break;
                case TabAlignment.Left:
                    pt = new Point(DropDownButton.Bounds.Right, DropDownButton.Bounds.Bottom);
                    dic = ToolStripDropDownDirection.AboveRight;
                    break;
                case TabAlignment.Right:
                    pt = new Point(DropDownButton.Bounds.Left, DropDownButton.Bounds.Bottom);
                    dic = ToolStripDropDownDirection.AboveLeft;
                    break;
                case TabAlignment.Bottom:
                    pt = new Point(DropDownButton.Bounds.Right, DropDownButton.Bounds.Top);
                    dic = ToolStripDropDownDirection.AboveLeft;
                    break;
            }
            pt = PointToScreen(pt);
            menu.Show(pt, dic);
        }

        void MenuItemDropDown_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem mi = sender as ToolStripMenuItem;
                if (mi.Tag is TabItem)
                {
                    TabItem ti = (TabItem)mi.Tag;
                    if (this.Items.Contains(ti))
                    {
                        this.SelectedItem = ti;
                    }
                }
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (LayoutItems())
            {
                EnsureItemVisible(SelectedItem);
                Invalidate();
            }
        }

        public bool SelectByTag(object tag)
        {
            TabItem item = GetItemByTag(tag);
            if (item != null)
            {
                SelectedItem = item;
                return true;
            }

            return false;
        }

        public void SelectNextTab(bool forward)
        {
            if (Items.Count == 0)
                return;

            if (SelectedItem != null)
            {
                int index = Items.IndexOf(SelectedItem);
                if (forward)
                {
                    index--;
                    if (index < 0)
                        index = Items.Count - 1;
                }
                else
                {
                    index++;
                    if (index >= Items.Count)
                        index = 0;
                }

                SelectedItem = Items[index];
            }
            else
            {
                SelectedItem = Items[0];
            }
        }

        public TabItem GetItemByTag(object tag)
        {
            foreach (TabItem ti in Items)
            {
                if (ti.Tag == tag)
                {
                    return ti;
                }
            }

            return null;
        }

        int GetMaxDisplayIndex()
        {
            int i = 0;
            foreach (TabItem hi in this.Items)
                i = Math.Max(i, hi.DisplayIndex);
            return i;
        }

        public override void UpdateView(ChangeTypes changes)
        {
            base.UpdateView(changes);

            if ((changes | ChangeTypes.AllVisual) != ChangeTypes.None)
            {
                RefreshDisplayItems();
                if (LayoutItems())
                {
                    Invalidate();
                }
            }
        }

        void RefreshDisplayItems()
        {
            this.DisplayItems.Clear();
            foreach (TabItem hi in this.Items)
            {
                if (hi.Visible)
                    this.DisplayItems.Add(hi);
            }

            this.DisplayItems.Sort(new TabItemDisplayIndexComparer());

            for (int i = 0; i < this.DisplayItems.Count; i++)
            {
                this.DisplayItems[i].DisplayIndex = i;
            }
        }

        public bool IsHalfDisplay(TabItem item)
        {
            return HalfDisplayItems != null && HalfDisplayItems.Contains(item);
        }
    }
}
