using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Blumind.Core;

namespace Blumind.Controls
{
    [DefaultEvent("SelectionChanged")]
    public class CellListBox<T> : ScrollableControl
    {
        private const int BorderSize = 1;
        private XList<T> _Items;
        private Size _CellSize;
        private int ColumnCount;
        private int RowCount;
        private int _SelectedIndex = -1;
        private List<int> _SelectedIndexes;
        private int _HoverIndex = -1;
        private bool _MultiSelect = false;
        private Orientation _ExtendOrientation = Orientation.Vertical;
        private bool IsMouseDown;
        private Point MouseDownPos;
        private Rectangle _SelectionRect;

        public event EventHandler SelectionChanged;

        public CellListBox()
        {
            CellSize = new Size(32, 32);
            AutoScroll = true;
            BackColor = SystemColors.Window;
            ForeColor = SystemColors.WindowText;
            AutoScrollMargin = new Size(BorderSize, BorderSize);
            _SelectedIndexes = new List<int>();

            SetStyle(ControlStyles.UserPaint |
               ControlStyles.AllPaintingInWmPaint |
               ControlStyles.OptimizedDoubleBuffer |
               ControlStyles.ResizeRedraw, true);

            Items = new XList<T>();
            Items.ItemAdded += new XListEventHandler<T>(Items_ItemAdded);
            Items.ItemRemoved += new XListEventHandler<T>(Items_ItemRemoved);
        }

        [Browsable(false)]
        public XList<T> Items
        {
            get { return _Items; }
            private set { _Items = value; }
        }

        [DefaultValue(typeof(Size), "32, 32")]
        public Size CellSize
        {
            get { return _CellSize; }
            set 
            {
                if (_CellSize != value)
                {
                    _CellSize = value;
                    OnCellSizeChanged();
                }
            }
        }

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

        [DefaultValue(-1)]
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set 
            {
                if (_SelectedIndex != value)
                {
                    int old = _SelectedIndex;
                    _SelectedIndex = value;
                    OnSelectionChanged(old);
                }
            }
        }

        [Browsable(false)]
        public int[] SelectedIndexes
        {
            get { return _SelectedIndexes.ToArray(); }
            private set
            {
                _SelectedIndexes.Clear();
                _SelectedIndexes.AddRange(value);
                _SelectedIndexes.Sort();
                Invalidate();
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

        protected int HoverIndex
        {
            get { return _HoverIndex; }
            set 
            {
                if (_HoverIndex != value)
                {
                    int old = _HoverIndex;
                    _HoverIndex = value;
                    OnHoverIndexChanged(old);
                }
            }
        }

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

        [DefaultValue(Orientation.Vertical)]
        public Orientation ExtendOrientation
        {
            get { return _ExtendOrientation; }
            set 
            {
                if (_ExtendOrientation != value)
                {
                    _ExtendOrientation = value;
                    OnExtendOrientationChanged();
                }
            }
        }

        private Rectangle SelectionRect
        {
            get { return _SelectionRect; }
            set 
            {
                if (_SelectionRect != value)
                {
                    Rectangle old = _SelectionRect;
                    _SelectionRect = value;
                    OnSelectionRectChanged(old);
                }
            }
        }

        protected virtual void OnCellSizeChanged()
        {
            LayoutEngine.Layout(this, new LayoutEventArgs(this, "CellSize"));
        }

        protected virtual void OnExtendOrientationChanged()
        {
            LayoutEngine.Layout(this, new LayoutEventArgs(this, "ExtendOrientation"));
        }

        protected virtual void OnSelectionChanged(int old)
        {
            InvalidateItem(old);
            InvalidateItem(SelectedIndex);

            if (SelectedIndex == -1)
            {
                _SelectedIndexes.Clear();
            }
            else if (!_SelectedIndexes.Contains(SelectedIndex))
            {
                if (!MultiSelect)
                    _SelectedIndexes.Clear();
                _SelectedIndexes.Add(SelectedIndex);
                _SelectedIndexes.Sort();
            }

            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);
        }

        protected virtual void OnMultiSelectChanged()
        {
        }

        private void OnHoverIndexChanged(int old)
        {
            InvalidateItem(old);
            InvalidateItem(HoverIndex);
        }

        private void OnSelectionRectChanged(Rectangle old)
        {
            if (!old.IsEmpty && !SelectionRect.IsEmpty)
                Invalidate(Rectangle.Union(old, SelectionRect));
            else if (!old.IsEmpty)
                Invalidate(old);
            else if (!SelectionRect.IsEmpty)
                Invalidate(SelectionRect);
        }

        private void Items_ItemAdded(object sender, XListEventArgs<T> e)
        {
            RefreshLayout();
            //LayoutEngine.Layout(this, new LayoutEventArgs(this, "Items"));
        }

        private void Items_ItemRemoved(object sender, XListEventArgs<T> e)
        {
            RefreshLayout();
            if (SelectedIndex == e.Index)
                SelectedIndex = -1;
            else if (MultiSelect && _SelectedIndexes.Contains(e.Index))
                _SelectedIndexes.Remove(e.Index);
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            RefreshLayout();
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            RefreshLayout();
        }

        private void RefreshLayout()
        {
            if (!Created)
                return;

            CalculateLayout(out ColumnCount, out RowCount);
            AutoScrollMinSize = new Size(ColumnCount * CellSize.Width, RowCount * CellSize.Height);
            Invalidate(false);
        }

        protected virtual void CalculateLayout(out int columns, out int rows)
        {
            Size size = ClientSize;
            size.Width -= BorderSize;
            size.Height -= BorderSize;

            if (ExtendOrientation == Orientation.Vertical)
            {
                columns = Math.Max(1, size.Width / CellSize.Width);
                rows = Items.Count / columns;
                if (Items.Count % columns > 0)
                    rows++;
            }
            else
            {
                rows = Math.Max(1, size.Height / CellSize.Height);
                columns = Items.Count / rows;
                if (Items.Count % rows > 0)
                    columns++;
            }
        }

        protected void InvalidateItem(int index)
        {
            if (index < 0 || index >= Items.Count)
                return;

            Rectangle rect = GetItemBounds(index);
            Invalidate(rect);
        }

        protected Rectangle GetItemBounds(int index)
        {
            if (index < 0 || index >= Items.Count)
                throw new ArgumentOutOfRangeException();

            int column = index % ColumnCount;
            int row = index / ColumnCount;

            Rectangle rect = new Rectangle(column * CellSize.Width, row * CellSize.Height, CellSize.Width, CellSize.Height);
            Point pt = GetScrollPoint();
            rect.X -= pt.X;
            rect.Y -= pt.Y;

            // border
            rect.X += BorderSize;
            rect.Y += BorderSize;

            return rect;
        }

        private int HitTest(int x, int y)
        {
            Point scroll = GetScrollPoint();
            x += scroll.X;
            y += scroll.Y;

            // border
            x -= BorderSize;
            y -= BorderSize;

            return GetItemAtPosition(y / CellSize.Height, x / CellSize.Width);
        }

        private int GetItemAtPosition(int row, int column)
        {
            if (column >= ColumnCount || row >= RowCount)
                return -1;
            int index = row * ColumnCount + column;
            if (index < 0 || index >= Items.Count)
                return -1;
            else
                return index;
        }

        private void TryMultiSelect(int[] indexes, bool append)
        {
            if (indexes.Length == 1 && indexes[0] == -1)
            {
                if (!append)
                    SelectedIndexes = new int[0];
                return;
            }

            List<int> sis = new List<int>(indexes);

            if (append)
            {
                foreach (int index in _SelectedIndexes)
                {
                    if (sis.Contains(index))
                        sis.Remove(index);
                    else
                        sis.Add(index);
                }
            }

            SelectedIndexes = sis.ToArray();
        }

        private int[] GetItemsBetween(int start, int end)
        {
            if (start < 0 || start >= Items.Count || end < 0 || end >= Items.Count)
                return new int[0];

            if (start == end)
                return new int[] { start };

            int from = Math.Min(start, end);
            int to = Math.Max(start, end);
            int[] items = new int[to - from + 1];
            for (int i = from; i <= to; i++)
            {
                items[i - from] = i;
            }

            return items;
        }

        private int[] GetItemsInRect(Rectangle rect)
        {
            Rectangle rectClient = ClientRectangle;
            rectClient.Inflate(-1, -1);
            rect = Rectangle.Intersect(rectClient, rect);

            Point scroll = GetScrollPoint();
            rect.X += scroll.X;
            rect.Y += scroll.Y;
            int r1, r2, c1, c2;
            r1 = Math.Max(0, rect.Y / CellSize.Height);
            r2 = Math.Min(RowCount - 1, rect.Bottom / CellSize.Height);
            c1 = Math.Max(0, rect.X / CellSize.Width);
            c2 = Math.Min(ColumnCount - 1, rect.Right / CellSize.Width);

            if (r1 >= RowCount || c1 >= ColumnCount || r2 < 0 || c2 < 0)
                return new int[0];

            r1 = Math.Min(r1, r2);
            c1 = Math.Min(c1, c2);

            List<int> items = new List<int>();
            for (int r = r1; r <= r2; r++)
            {
                for (int c = c1; c <= c2; c++)
                {
                    int index = r * ColumnCount + c;
                    if(index >-1 && index < Items.Count)
                        items.Add(index);
                }
            }

            return items.ToArray();
        }

        public void SelectAll()
        {
            if (MultiSelect)
            {
                int[] items = new int[Items.Count];
                for (int i = 0; i < Items.Count; i++)
                    items[i] = i;
                TryMultiSelect(items, false);
            }
        }

        public void InvertSelect()
        {
            if (MultiSelect)
            {
                int[] items = new int[Items.Count - _SelectedIndexes.Count];
                if (items.Length > 0)
                {
                    int index = 0;
                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (!_SelectedIndexes.Contains(i))
                        {
                            items[index] = i;
                            index++;
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            Items.Clear();
            _SelectedIndexes.Clear();
            SelectedIndex = -1;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (CanFocus && !Focused)
                Focus();

            if (e.Button == MouseButtons.Left)
            {
                int index = HitTest(e.X, e.Y);

                if (MultiSelect)
                {
                    if (Helper.TestModifierKeys(Keys.Control))
                        TryMultiSelect(new int[] { index }, true);
                    else if (Helper.TestModifierKeys(Keys.Shift))
                        TryMultiSelect(GetItemsBetween(SelectedIndex, index), false);
                    else
                        TryMultiSelect(new int[] { index }, false);
                }

                SelectedIndex = index;
                IsMouseDown = true;
                MouseDownPos = new Point(e.X, e.Y);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (MultiSelect && SelectedIndex == -1 && IsMouseDown)
            {
                SelectionRect = new Rectangle(Math.Min(e.X, MouseDownPos.X),
                    Math.Min(e.Y, MouseDownPos.Y),
                    Math.Abs(e.X - MouseDownPos.X),
                    Math.Abs(e.Y - MouseDownPos.Y));                
            }
            else
            {
                HoverIndex = HitTest(e.X, e.Y);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            HoverIndex = -1;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            Invalidate(false);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (MultiSelect && IsMouseDown && !SelectionRect.IsEmpty)
            {
                TryMultiSelect(GetItemsInRect(SelectionRect), Helper.TestModifierKeys(Keys.Control));
            }

            IsMouseDown = false;
            MouseDownPos = Point.Empty;
            SelectionRect = Rectangle.Empty;
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);

            Invalidate(false);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control)
            {
                if (MultiSelect)
                {
                    SelectAll();
                    e.SuppressKeyPress = true;
                }
            }

            base.OnKeyDown(e);
        }

        private Point GetScrollPoint()
        {
            return new Point(
                HorizontalScroll.Enabled ? HorizontalScroll.Value : 0,
                VerticalScroll.Enabled ? VerticalScroll.Value : 0);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            e.Graphics.Clear(BackColor);

            Rectangle rect = ClientRectangle;

            // border
            ControlPaint.DrawVisualStyleBorder(e.Graphics, new Rectangle(0, 0, Width - 1, Height - 1));
            rect.Inflate(-BorderSize, -BorderSize);

            int x0 = rect.X;
            int y0 = rect.Y;
            int index = 0;
            Point scroll = GetScrollPoint();
            index += (scroll.Y / CellSize.Height) * ColumnCount + (scroll.X / CellSize.Width);
            y0 -= scroll.Y % CellSize.Height;
            x0 -= scroll.X % CellSize.Width;

            //
            int x = x0;
            int y = y0;
            Region old = e.Graphics.Clip;
            for (int i = index; i < Items.Count; i++)
            {
                Rectangle rectCell = new Rectangle(x, y, CellSize.Width, CellSize.Height);
                e.Graphics.Clip = new Region(Rectangle.Intersect(rectCell, rect));

                if (i == HoverIndex)
                    DrawHover(rectCell, e);

                bool selected = (MultiSelect && _SelectedIndexes.Contains(i)) || i == SelectedIndex;
                if (selected)
                    DrawSelection(rectCell, e);

                Rectangle r2 = rectCell;
                r2.Inflate(-1, -1);
                DrawCell(i, r2, e);

                if (i % ColumnCount == ColumnCount - 1)
                {
                    x = x0;
                    y += CellSize.Height;
                }
                else
                {
                    x += CellSize.Width;
                }

                if (y > rect.Bottom)
                    break;
            }
            e.Graphics.Clip = old;

            // 
            if (MultiSelect && IsMouseDown && !SelectionRect.IsEmpty)
            {
                e.Graphics.FillRectangle(
                    new SolidBrush(Color.FromArgb(100, SystemColors.Highlight)),
                    SelectionRect);
                e.Graphics.DrawRectangle(SystemPens.Highlight, SelectionRect.X, SelectionRect.Y, SelectionRect.Width - 1, SelectionRect.Height - 1);
            }
        }

        protected virtual void DrawHover(Rectangle rectCell, PaintEventArgs e)
        {
            Color color = PaintHelper.AdjustColor(SystemColors.Highlight, 40, 50, 85, 90);
            PaintHelper.DrawHoverBackgroundFlat(e.Graphics, rectCell, color);


            //LinearGradientBrush brush = new LinearGradientBrush(rectCell, PT.GetLightColor(SystemColors.Highlight), SystemColors.Highlight, 90.0f);
            //e.Graphics.FillRectangle(brush, rectCell);
            //e.Graphics.DrawRectangle(SystemPens.Highlight, rectCell.X, rectCell.Y, rectCell.Width - 1, rectCell.Height - 1);
        }

        protected virtual void DrawSelection(Rectangle rectCell, PaintEventArgs e)
        {
            //Rectangle rect1 = rectCell;
            //rect1.Inflate(2, 2);
            //LinearGradientBrush brush = new LinearGradientBrush(
            //    rect1, 
            //    Color.FromArgb(50, SystemColors.Highlight), 
            //    Color.FromArgb(100, SystemColors.Highlight),
            //    45.0f);
            //e.Graphics.FillRectangle(brush, rectCell);

            Color color = PaintHelper.AdjustColor(SystemColors.Highlight, 40, 50, 85, 90);
            PaintHelper.DrawHoverBackgroundFlat(e.Graphics, rectCell, color);

            //e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, SystemColors.Highlight)), rectCell);
            //rectCell.Width--;
            //rectCell.Height--;
            //e.Graphics.DrawRectangle(SystemPens.Highlight, rectCell);
        }

        protected virtual void DrawCell(int index, Rectangle rect, PaintEventArgs e)
        {
        }
    }
}
