using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Linq;
using Blumind.Core;
using Blumind.Controls.OS;
using Blumind.Model.Documents;

namespace Blumind.Controls
{
    class ListBoxControl : ListBoxControl<object>
    {
    }

    class ListBoxControl<T> : ScrollableControl
    {
        ListBoxControlItem<T>[] ListItems;
        int _ItemHeight = 23;
        BorderStyle _BorderStyle = BorderStyle.FixedSingle;
        ToolTip toolTip1;

        public event System.EventHandler SelectionChanged;

        public ListBoxControl()
        {
            ForeColor = SystemColors.WindowText;
            BackColor = SystemColors.Window;
            IconSize = new System.Drawing.Size(16, 16);
            ShowItemToolTip = true;
            ListItems = new ListBoxControlItem<T>[0];

            Items = new XList<T>();
            Items.ItemChanged += Items_ItemChanged;
            Items.ItemAdded += Items_ItemAdded;
            Items.ItemRemoved += Items_ItemRemoved;

            SetStyle(ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw, true);
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 100);
            }
        }

        [DefaultValue(23)]
        public int ItemHeight
        {
            get { return _ItemHeight; }
            set 
            {
                if (_ItemHeight != value)
                {
                    _ItemHeight = value;
                    OnItemHeightChanged();
                }
            }
        }

        [DefaultValue(typeof(Size), "16, 16")]
        public Size IconSize { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public XList<T> Items { get; private set; }

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

        [DefaultValue(BorderStyle.FixedSingle)]
        public BorderStyle BorderStyle
        {
            get { return _BorderStyle; }
            set
            {
                if (_BorderStyle != value)
                {
                    _BorderStyle = value;
                    OnBorderStyleChanged();
                }
            }
        }

        [Browsable(false), DefaultValue(null)]
        public Func<T, Image> ItemIconGetter { get; set; }

        [Browsable(false), DefaultValue(null)]
        public Func<T, string> ItemToolTipTextGetter { get; set; }

        [DefaultValue(true)]
        public bool ShowItemToolTip { get; set; }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                if (BorderStyle != System.Windows.Forms.BorderStyle.None)
                {
                    cp.Style |= (int)Blumind.Controls.OS.WindowStyle.WS_BORDER;
                }
                return cp;
            }
        }

        void OnItemHeightChanged()
        {
            PerformLayout();
            Invalidate();
        }

        ListBoxControlItem<T> FindListItem(T value)
        {
            if (ListItems == null)
                return null;

            if (value == null)
            {
                return ListItems.Find(li => li.Value == null);
            }
            else
            {
                return ListItems.Find(li => value.Equals(li.Value));
            }
        }

        void Items_ItemChanged(object sender, XListValueEventArgs<T> e)
        {
            PerformLayout();
            Invalidate();
        }

        void Items_ItemAdded(object sender, XListEventArgs<T> e)
        {
            PerformLayout();
            Invalidate();
        }

        void Items_ItemRemoved(object sender, XListEventArgs<T> e)
        {
            var li = FindListItem(e.Item);
            if (li != null && SelectedIndices != null && SelectedIndices.Contains(li.Index))
            {
                _SelectedIndices = SelectedIndices.Where(si => si != li.Index).ToArray();
            }

            PerformLayout();
            Invalidate();
        }

        void Items_AfterClear(object sender, EventArgs e)
        {
            SelectedIndices = new int[0];
            LayoutItems();
            Invalidate();
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);

            Invalidate();
        }

        void OnBorderStyleChanged()
        {
            if (Created)
            {
                var wlng = User32.GetWindowLong(this.Handle, GetWindowLongFlags.GWL_STYLE);
                if (wlng != 0)
                {
                    if (BorderStyle == System.Windows.Forms.BorderStyle.None)
                    {
                        wlng &= ~(int)WindowStyle.WS_BORDER;
                    }
                    else
                    {
                        wlng |= (int)WindowStyle.WS_BORDER;
                    }
                    User32.SetWindowLong(this.Handle, GetWindowLongFlags.GWL_STYLE, wlng);
                }
            }
        }

        protected void ShowToolTip(string text)
        {
            if (toolTip1 == null && string.IsNullOrEmpty(text))
                return;

            if (toolTip1 == null)
            {
                toolTip1 = new ToolTip();
            }

            toolTip1.SetToolTip(this, text);
        }

        #region layout
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            LayoutItems();
        }

        void LayoutItems()
        {
            int fullWidth = 0;
            if (Items.IsNullOrEmpty())
            {
                ListItems = new ListBoxControlItem<T>[0];
            }
            else
            {
                var rect = DisplayRectangle;
                int y = rect.Y;
                int index = 0;
                var listItems = new ListBoxControlItem<T>[Items.Count];
                foreach (var item in Items)
                {
                    var li = new ListBoxControlItem<T>(item.ToString(), item);
                    listItems[index] = li;
                    li.Index = index++;
                    li.Value = item;
                    li.Bounds = new Rectangle(rect.Left, y, rect.Width, ItemHeight);

                    if (ItemIconGetter != null)
                        li.Icon = ItemIconGetter(item);

                    if (ItemToolTipTextGetter != null)
                        li.ToolTipText = ItemToolTipTextGetter(item);

                    y += li.Bounds.Height;
                    fullWidth += li.Bounds.Height;
                }

                ListItems = listItems;
            }

            AutoScrollMinSize = new Size(0, fullWidth);
            AutoScrollMargin = new Size(10, 10);
        }

        #endregion

        #region selection
        bool _MultiSelect;
        int[] _SelectedIndices;
        Color _SelectionBackColor = SystemColors.Highlight;
        Color _SelectionForeColor = SystemColors.HighlightText;

        [DefaultValue(false)]
        public bool MultiSelect
        {
            get { return _MultiSelect; }
            set
            {
                if (_MultiSelect != value)
                {
                    _MultiSelect = value;
                    OnMultiSelectChanged();
                }
            }
        }

        [Browsable(false), DefaultValue(null)]
        public T SelectedItem
        {
            get
            {
                if (SelectedIndex > -1 && SelectedIndex < Items.Count)
                    return Items[SelectedIndex];
                else
                    return default(T);
            }
            set
            {
                SelectedIndex = Items.IndexOf(value);
            }
        }

        [DefaultValue(-1)]
        public int SelectedIndex
        {
            get
            {
                if (SelectedIndices.IsNullOrEmpty())
                    return -1;
                else
                    return SelectedIndices.First();
            }
            set
            {
                if (value >= Items.Count)
                    throw new IndexOutOfRangeException();
                else if (value < 0)
                    SelectedIndices = new int[0];
                else
                    SelectedIndices = new int[] { value };
            }
        }

        [DefaultValue(null)]
        public int[] SelectedIndices
        {
            get { return _SelectedIndices; }
            set
            {
                if (!_SelectedIndices.CollectionEqual(value))
                {
                    _SelectedIndices = value;
                    OnSelectionChanged();
                }
            }
        }

        [Browsable(false), DefaultValue(null)]
        public T[] SelectedItems
        {
            get
            {
                if (SelectedIndices.IsNullOrEmpty())
                    return new T[0];
                else
                    return (from li in ListItems
                            where _SelectedIndices.Contains(li.Index)
                            select li.Value).ToArray();
            }
            set
            {
                if (value.IsNullOrEmpty())
                {
                    SelectedIndices = new int[0];
                }
                else
                {
                    SelectedIndices = (from li in ListItems
                                       where value.Contains(li.Value)
                                       select li.Index).ToArray();
                }
            }
        }

        [DefaultValue(typeof(Color), "Highlight")]
        public Color SelectionBackColor
        {
            get { return _SelectionBackColor; }
            set
            {
                if (_SelectionBackColor != value)
                {
                    _SelectionBackColor = value;
                    OnSelectionBackColorChanged();
                }
            }
        }

        [DefaultValue(typeof(Color), "HighlightText")]
        public Color SelectionForeColor
        {
            get { return _SelectionForeColor; }
            set
            {
                if (_SelectionForeColor != value)
                {
                    _SelectionForeColor = value;
                    OnSelectionForeColorChanged();
                }
            }
        }

        protected virtual void OnSelectionBackColorChanged()
        {
            if (!SelectedIndices.IsNullOrEmpty())
                Invalidate();
        }

        protected virtual void OnSelectionForeColorChanged()
        {
            if (!SelectedIndices.IsNullOrEmpty())
                Invalidate();
        }

        protected virtual void OnMultiSelectChanged()
        {
        }

        protected virtual void OnSelectionChanged()
        {
            Invalidate();

            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);
        }

        public void ClearSelection()
        {
            SelectedIndices = null;
            Invalidate();
        }

        #endregion

        #region mouse event
        Point MouseDownPos;
        bool MouseDownForSelect;
        Rectangle? _SelectRectangle;
        ListBoxControlItem<T> _HoverItem;

        Rectangle? SelectRectangle
        {
            get { return _SelectRectangle; }
            set
            {
                if (_SelectRectangle != value)
                {
                    var old = _SelectRectangle;
                    _SelectRectangle = value;
                    OnSelectRectangleChanged(old);
                }
            }
        }

        ListBoxControlItem<T> HoverItem
        {
            get { return _HoverItem; }
            set
            {
                if (_HoverItem != value)
                {
                    var old = _HoverItem;
                    _HoverItem = value;
                    OnHoverItemChanged(old);
                }
            }
        }

        void OnHoverItemChanged(ListBoxControlItem<T> old)
        {
            if (old != null)
                InvalidateItem(old.Index);

            if (HoverItem != null)
            {
                if (ShowItemToolTip)
                {
                    ShowToolTip(HoverItem.ToolTipText);
                }
                InvalidateItem(HoverItem.Index);
            }
        }

        private void OnSelectRectangleChanged(Rectangle? old)
        {
            if (old.HasValue)
                DrawSelectionBox(old.Value);

            if (SelectRectangle.HasValue)
            {
                DrawSelectionBox(SelectRectangle.Value);
            }
        }

        void DrawSelectionBox(Rectangle rectangle)
        {
            //rectangle.Location = this.PointToScreen(rectangle.Location);
            //ControlPaint.DrawReversibleFrame(rectangle, this.BackColor, FrameStyle.Thick);

            rectangle.Inflate(2, 2);
            var region = new Region(rectangle);
            rectangle.Inflate(-5, -5);
            region.Exclude(rectangle);
            Invalidate(region);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (MouseDownForSelect)
            {
                var rect = PaintHelper.GetRectangle(MouseDownPos, new Point(e.X, e.Y));
                rect.Intersect(DisplayRectangle);
                SelectRectangle = rect;
            }
            else
            {
                HoverItem = HitTest(e.X, e.Y);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            HoverItem = null;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (MouseDownForSelect)
            {
                if (SelectRectangle.HasValue)
                {
                    var items = GetItemsInRange(SelectRectangle.Value);
                    if (MultiSelect
                        && ((Control.ModifierKeys & Keys.Control) == Keys.Control || (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                        && !SelectedIndices.IsNullOrEmpty())
                    {
                        SelectedIndices = SelectedIndices.Union(items).Distinct().ToArray();
                    }
                    else
                    {
                        SelectedIndices = items;
                    }
                }

                MouseDownForSelect = false;
                SelectRectangle = null;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!Focused && CanFocus)
                Focus();

            if (e.Button == MouseButtons.Left)
            {
                var listItem = HitTest(e.X, e.Y);
                if (listItem != null)
                {
                    // 暂时封印取消选择操作, 其是否需要还没有确定
                    //if (SelectedIndices != null && SelectedIndices.Contains(listItem.Index))
                    //{
                    // 排除选中项
                    //SelectedIndices = SelectedIndices.Where(si => si != listItem.Index).ToArray();
                    //}
                    //else
                    //{
                    if (MultiSelect && (Control.ModifierKeys & Keys.Shift) == Keys.Shift && !SelectedIndices.IsNullOrEmpty())
                    {
                        var lastIndex = SelectedIndices.Last();
                        var range = Enumerable.Range(Math.Min(lastIndex, listItem.Index), Math.Max(lastIndex, listItem.Index) + 1);
                        SelectedIndices = SelectedIndices.Union(range).Distinct().ToArray();
                    }
                    else if (MultiSelect && (Control.ModifierKeys & Keys.Control) == Keys.Control && !SelectedIndices.IsNullOrEmpty())
                    {
                        SelectedIndices = SelectedIndices.Union(new int[] { listItem.Index }).ToArray();
                    }
                    else
                    {
                        SelectedIndices = new int[] { listItem.Index };
                    }
                    //}
                }
                else if(MultiSelect)
                {
                    MouseDownForSelect = true;
                }
            }

            MouseDownPos = new Point(e.X, e.Y);
        }

        ListBoxControlItem<T> HitTest(int x, int y)
        {
            if (this.VerticalScroll.Enabled)
                y += this.VerticalScroll.Value;

            return ListItems.Find(li => li.Bounds.Contains(x, y));
        }

        int[] GetItemsInRange(Rectangle rectangle)
        {
            if (this.VerticalScroll.Enabled)
                rectangle.Y += this.VerticalScroll.Value;

            return (from item in ListItems
                    where item.Bounds.IntersectsWith(rectangle)
                    select item.Index).ToArray();
        }

        public T GetItemAt(int x, int y)
        {
            var li = HitTest(x, y);
            if (li != null)
                return li.Value;
            else
                return default(T);
        }

        #endregion

        #region paint
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);

            //
            var viewPort = DisplayRectangle;
            if (this.VerticalScroll.Enabled)
            {
                e.Graphics.TranslateTransform(0, -this.VerticalScroll.Value);
                viewPort.Y += this.VerticalScroll.Value;
            }

            //
            bool globalHaveIcon = ListItems.Exists(li => li.Icon != null);
            foreach(var item in ListItems)
            {
                var rectItem = item.Bounds;
                if (rectItem.Bottom < viewPort.Y)
                    continue;
                if (rectItem.Y > viewPort.Bottom)
                    break;

                bool selected = SelectedIndices != null && SelectedIndices.Contains(item.Index);
                bool hover = HoverItem != null && HoverItem == item;

                DrawItem(e, item, rectItem, selected, hover, globalHaveIcon);
                rectItem.Y += ItemHeight;
            }

            //
            if (SelectRectangle.HasValue)
            {
                var rectSel = SelectRectangle.Value;
                if (rectSel.Width > 0 && rectSel.Height > 0)
                {
                    e.Graphics.DrawRectangle(Pens.Black, rectSel);
                    rectSel.Inflate(-1, -1);
                    if (rectSel.Width > 0 && rectSel.Height > 0)
                    {
                        e.Graphics.DrawRectangle(Pens.White, rectSel);
                    }
                }
            }
        }

        void DrawItem(PaintEventArgs e, ListBoxControlItem<T> item, Rectangle rect, bool selected, bool hover, bool globalHaveIcon)
        {
            Color foreColor = ForeColor;
            Color backColor = BackColor;
            if (selected)
            {
                foreColor = SelectionForeColor;
                backColor = SelectionBackColor;
            }

            if (!Enabled)
            {
                if (selected)
                {
                    backColor = PaintHelper.ReduceSaturation(backColor, 60);
                    foreColor = PaintHelper.ReduceSaturation(foreColor, 60);
                }
                else
                {
                    foreColor = Color.Gray;
                }
            }

            e.Graphics.FillRectangle(new SolidBrush(backColor), rect);

            if (!selected && hover)
            {
                e.Graphics.DrawLine(Pens.Gainsboro, rect.X + 2, rect.Top, rect.Right - 3, rect.Top);
                e.Graphics.DrawLine(Pens.Gainsboro, rect.X + 2, rect.Bottom - 1, rect.Right - 3, rect.Bottom - 1);
            }

            if (!string.IsNullOrEmpty(item.Text) || item.Icon != null)
            {
                rect.Inflate(-1, -1);

                if (item.Icon != null)
                {
                    e.Graphics.DrawImage(item.Icon,
                        new Rectangle(rect.X, rect.Y + (rect.Height - IconSize.Height) / 2, IconSize.Width, IconSize.Height),
                        0, 0, item.Icon.Width, item.Icon.Height,
                        GraphicsUnit.Pixel);
                }

                if (globalHaveIcon)
                {
                    rect.X += IconSize.Width;
                    rect.Width -= IconSize.Width;
                }

                if (!string.IsNullOrEmpty(item.Text))
                {
                    e.Graphics.DrawString(item.Text, Font, new SolidBrush(foreColor), rect, PaintHelper.SFLeft);
                }
            }
        }

        void InvalidateItem(int index)
        {
            if (index < 0 || index >= ListItems.Length)
                return;

            var rect = ListItems[index].Bounds;
            if (VerticalScroll.Enabled)
                rect.Y -= VerticalScroll.Value;
            Invalidate(rect);
        }
        #endregion
    }
}
