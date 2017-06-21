using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Blumind.Core;

namespace Blumind.Controls
{
    class ResizableTextBox : BaseControl
    {
        #region TextBoxExItem
        public delegate void TextBoxExItemEventHandler(object sender, TextBoxExItem item, EventArgs e);

        public enum ItemAlignment
        {
            Left,
            Right,
        }

        public class TextBoxExItem
        {
            private Image _Image = null;
            private object _Tag = null;
            private bool _Enabled = true;
            private bool _Visible = true;
            private bool _RespondMouse = true;
            private ItemAlignment _Alignment = ItemAlignment.Left;
            private string _ToolTipText = null;
            private Rectangle _Bounds = Rectangle.Empty;
            private int _CustomSize = -1;
            private Color? _BackColor;
            public ResizableTextBox Owner { get; internal set; }

            public event System.EventHandler BackColorChanged;
            public event System.EventHandler ImageChanged;
            public event System.EventHandler EnabledChanged;
            public event System.EventHandler VisibleChanged;
            public event System.EventHandler AlignmentChanged;
            public event System.EventHandler CustomSizeChanged;
            public event System.EventHandler Click;

            public TextBoxExItem()
            {
            }

            public TextBoxExItem(Image image, ItemAlignment align)
            {
                Image = image;
                Alignment = align;
            }

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

            public object Tag
            {
                get { return _Tag; }
                set { _Tag = value; }
            }

            public bool Enabled
            {
                get { return _Enabled; }
                set
                {
                    if (_Enabled != value)
                    {
                        _Enabled = value;
                        OnEnabledChanged();
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

            public bool RespondMouse
            {
                get { return _RespondMouse; }
                set { _RespondMouse = value; }
            }

            [DefaultValue(-1)]
            public int CustomSize
            {
                get { return _CustomSize; }
                set 
                {
                    if (_CustomSize != value)
                    {
                        _CustomSize = value;
                        OnCustomSizeChanged();
                    }
                }
            }

            public ItemAlignment Alignment
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

            public Rectangle Bounds
            {
                get { return _Bounds; }
                internal set { _Bounds = value; }
            }

            public string ToolTipText
            {
                get { return _ToolTipText; }
                set { _ToolTipText = value; }
            }

            public Color? BackColor
            {
                get { return _BackColor; }
                set 
                {
                    if (_BackColor != value)
                    {
                        _BackColor = value;
                        OnBackColorChanged();
                    }
                }
            }

            private void OnImageChanged()
            {
                if (ImageChanged != null)
                    ImageChanged(this, EventArgs.Empty);
            }

            private void OnEnabledChanged()
            {
                if (EnabledChanged != null)
                    EnabledChanged(this, EventArgs.Empty);
            }

            private void OnVisibleChanged()
            {
                if (VisibleChanged != null)
                    VisibleChanged(this, EventArgs.Empty);
            }

            private void OnAlignmentChanged()
            {
                if (AlignmentChanged != null)
                    AlignmentChanged(this, EventArgs.Empty);
            }

            private void OnCustomSizeChanged()
            {
                if (CustomSizeChanged != null)
                    CustomSizeChanged(this, EventArgs.Empty);
            }

            private void OnBackColorChanged()
            {
                if (BackColorChanged != null)
                {
                    BackColorChanged(this, EventArgs.Empty);
                }
            }

            internal void OnClick()
            {
                if (Click != null)
                    Click(this, EventArgs.Empty);
            }

            public virtual void OnPaint(PaintEventArgs e, Color backColor, Color foreColor, Font font, bool hover, bool pressed)
            {
                DrawItemBackground(e, hover, pressed);

                if (Image != null)
                {
                    if (Enabled && this.Enabled)
                        PaintHelper.DrawImageInRange(e.Graphics, Image, Bounds);
                    else
                        PaintHelper.DrawImageDisabledInRect(e.Graphics, Image, Bounds, backColor);
                }
            }

            protected virtual void DrawItemBackground(PaintEventArgs e, bool hover, bool pressed)
            {
                Color color;
                //UIControlStatus us = UIControlStatus.Normal;
                if (pressed)
                    color = SystemColors.ControlDark;
                //us = UIControlStatus.Selected;
                else if (hover)
                    color = SystemColors.ControlLight;
                //us = UIControlStatus.Hover;
                else if(BackColor.HasValue)
                    color = BackColor.Value;
                else
                    return;

                e.Graphics.FillRectangle(new SolidBrush(color), Bounds);
                e.Graphics.DrawRectangle(new Pen(PaintHelper.GetDarkColor(color)), 0, 0, Bounds.Width, Bounds.Height);

                //Skin.DrawToolButtonBackground(new SkinPaintEventArgs(this, e), item.Bounds, us);
            }
        }
        #endregion

        protected TextBox InnerTextBox;
        private XList<TextBoxExItem> _Items;
        private int _DefaultItemSize = 20;
        private int _ItemSpace = 2;
        private Rectangle InnerTextBoxBounds = Rectangle.Empty;
        private TextBoxExItem _HoverItem = null;
        private TextBoxExItem _PressedItem = null;
        private bool _ReadOnly = false;

        public event TextBoxExItemEventHandler ItemClick;
        public event TextBoxExItemEventHandler ItemMouseDown;
        public event TextBoxExItemEventHandler ItemMouseUp;

        public ResizableTextBox()
        {
            SetPaintStyles();

            BackColor = SystemColors.Window;
            ForeColor = SystemColors.WindowText;

            Items = new XList<TextBoxExItem>();

            InnerTextBox = new TextBox();
            InnerTextBox.BorderStyle = BorderStyle.None;
            InnerTextBox.TextChanged += new EventHandler(InnerTextBox_TextChanged);
            InnerTextBox.MultilineChanged += new EventHandler(InnerTextBox_MultilineChanged);
            InnerTextBox.KeyDown += new KeyEventHandler(InnerTextBox_KeyDown);
            InnerTextBox.KeyPress += new KeyPressEventHandler(InnerTextBox_KeyPress);
            InnerTextBox.KeyUp += new KeyEventHandler(InnerTextBox_KeyUp);
            this.Controls.Add(InnerTextBox);
        }

        #region TextBox(base) Properties
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set 
            {
                if (_ReadOnly != value)
                {
                    _ReadOnly = value;
                    OnReadOnlyChanged();
                }
            }
        }

        [DefaultValue(0)]
        public char PasswordChar
        {
            get { return InnerTextBox.PasswordChar; }
            set { InnerTextBox.PasswordChar = value; }
        }

        [DefaultValue(32767)]
        public virtual int MaxLength
        {
            get { return InnerTextBox.MaxLength; }
            set { InnerTextBox.MaxLength = value; }
        }

        [DefaultValue(false)]
        public bool UseSystemPasswordChar
        {
            get { return InnerTextBox.UseSystemPasswordChar; }
            set { InnerTextBox.UseSystemPasswordChar = value; }
        }

        protected override bool ScaleChildren
        {
            get
            {
                return false;
            }
        }

        [DefaultValue(false)]
        public bool Multiline
        {
            get { return InnerTextBox.Multiline; }
            set { InnerTextBox.Multiline = value; }
        }

        [DefaultValue(ScrollBars.None)]
        public ScrollBars ScrollBars
        {
            get { return InnerTextBox.ScrollBars; }
            set { InnerTextBox.ScrollBars = value; }
        }

        protected virtual bool RelevanceInnerTextBoxReadOnly
        {
            get { return true; }
        }
        #endregion

        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 20);
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

        [DefaultValue(20)]
        public int DefaultItemSize
        {
            get { return _DefaultItemSize; }
            set
            {
                value = Math.Max(0, value);
                if (value != _DefaultItemSize)
                {
                    _DefaultItemSize = value;
                    OnDefaultItemSizeChanged();
                }
            }
        }

        [Browsable(false)]
        public XList<TextBoxExItem> Items
        {
            get { return _Items; }
            private set 
            {
                if (_Items != value)
                {
                    _Items = value;
                    OnItemsChanged();
                }
            }
        }

        private TextBoxExItem HoverItem
        {
            get { return _HoverItem; }
            set
            {
                if (_HoverItem != value)
                {
                    TextBoxExItem old = _HoverItem;
                    _HoverItem = value;
                    OnHoverItemChanged(old);
                }
            }
        }

        private TextBoxExItem PressedItem
        {
            get { return _PressedItem; }
            set
            {
                if (_PressedItem != value)
                {
                    TextBoxExItem old = _PressedItem;
                    _PressedItem = value;
                    OnPressedItemChanged(old);
                }
            }
        }

        [DefaultValue(2)]
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

        [DefaultValue(false)]
        public bool AutoJumpToNextControl { get; set; }

        #region TextBox(base) Methods
        public void Select(int start, int length)
        {
            InnerTextBox.Select(start, length);
        }

        public void SelectAll()
        {
            InnerTextBox.SelectAll();
        }
        #endregion
        
        public bool JumpToNextControl()
        {
            if (Parent != null)
            {
                return Parent.SelectNextControl(this, true, true, false, true);
            }
            else
            {
                return false;
            }
        }

        private StringFormat GetStringFormat()
        {
            switch (InnerTextBox.TextAlign)
            {
                case HorizontalAlignment.Center:
                    return PaintHelper.SFCenter;
                case HorizontalAlignment.Right:
                    return PaintHelper.SFRight;
                case HorizontalAlignment.Left:
                default:
                    return PaintHelper.SFLeft;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            PressedItem = HitTest(e.X, e.Y);
            if (PressedItem != null)
            {
                OnItemMouseDown(PressedItem);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (PressedItem != null && HitTest(e.X, e.Y) == PressedItem)
            {
                OnItemMouseUp(PressedItem);
                OnItemClick(PressedItem);
            }

            PressedItem = null;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            HoverItem = HitTest(e.X, e.Y);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            HoverItem = null;
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            InnerTextBox.Font = this.Font;
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);

            if (InnerTextBox != null)
            {
                InnerTextBox.BackColor = BackColor;
            }
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);

            if (InnerTextBox != null)
            {
                InnerTextBox.ForeColor = ForeColor;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (ReadOnly && RelevanceInnerTextBoxReadOnly)
                e.Graphics.Clear(SystemColors.Control);
            else
                e.Graphics.Clear(BackColor);

            if (!InnerTextBox.Visible && !string.IsNullOrEmpty(Text))
            {
                if (this.PasswordChar != 0)
                    e.Graphics.DrawString(new string(PasswordChar, this.Text.Length), Font, new SolidBrush(this.ForeColor), InnerTextBoxBounds, GetStringFormat());
                else
                    e.Graphics.DrawString(this.Text, Font, new SolidBrush(this.ForeColor), InnerTextBoxBounds, GetStringFormat());
            }

            foreach (TextBoxExItem item in Items)
            {
                if (item == null || !item.Visible)
                    continue;
                item.OnPaint(e, BackColor, ForeColor, Font, item == HoverItem, item == PressedItem);
            }

            Rectangle rect = ClientRectangle;
            ControlPaint.DrawVisualStyleBorder(e.Graphics, new Rectangle(rect.Left, rect.Top, rect.Width - 1, rect.Height - 1));
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            LayoutItemBounds();
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            LayoutItemBounds();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            InnerTextBox.Text = this.Text;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            if (InnerTextBox.CanFocus)
            {
                InnerTextBox.Focus();
            }
        }

        private void OnDefaultItemSizeChanged()
        {
            LayoutItemBounds();
            Invalidate();
        }

        private void OnItemSpaceChanged()
        {
            LayoutItemBounds();
            Invalidate();
        }

        protected virtual void OnReadOnlyChanged()
        {
            if (RelevanceInnerTextBoxReadOnly)
            {
                InnerTextBox.ReadOnly = ReadOnly;
            }
            else
            {
                InnerTextBox.BackColor = BackColor;
            }

            Invalidate();
        }

        private TextBoxExItem HitTest(int x, int y)
        {
            foreach (TextBoxExItem item in Items)
            {
                if (item == null || !item.Visible || !item.RespondMouse || !item.Enabled)
                    continue;
                if (item.Bounds.Contains(x, y))
                    return item;
            }

            return null;
        }

        private void InnerTextBox_TextChanged(object sender, EventArgs e)
        {
            this.Text = InnerTextBox.Text;
            if (!InnerTextBox.Visible)
                Invalidate();
        }

        private void InnerTextBox_MultilineChanged(object sender, EventArgs e)
        {
            LayoutItemBounds();

            Invalidate();
        }

        private void InnerTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e);

            if (e.KeyCode == Keys.Enter && !e.SuppressKeyPress && AutoJumpToNextControl)
            {
                if (JumpToNextControl())
                {
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void InnerTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            OnKeyUp(e);
        }

        private void InnerTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnKeyPress(e);
        }

        private void LayoutItemBounds()
        {
            Rectangle rect = ClientRectangle;
            rect.Inflate(-1, -1);

            int leftX = rect.Left;
            int rightX = rect.Right;
            foreach (TextBoxExItem item in Items)
            {
                if (item == null || !item.Visible)
                    continue;
                int width = item.CustomSize > -1 ? item.CustomSize : DefaultItemSize;
                if (item.Alignment == ItemAlignment.Left)
                {
                    item.Bounds = new Rectangle(leftX, rect.Y, width, rect.Height);
                    leftX += width + ItemSpace;
                }
                else
                {
                    rightX -= width;
                    item.Bounds = new Rectangle(rightX, rect.Y, width, rect.Height);
                    rightX -= ItemSpace;
                }
            }

            InnerTextBoxBounds = new Rectangle(leftX, rect.Top, Math.Max(0, rightX - leftX), rect.Height);
            InnerTextBox.Location = new Point(InnerTextBoxBounds.Left, rect.Y + (rect.Height - InnerTextBox.Height) / 2);
            InnerTextBox.Width = InnerTextBoxBounds.Width;
            if (Multiline)
                InnerTextBox.Height = InnerTextBoxBounds.Height;
            else
                InnerTextBox.Height = Math.Min(rect.Height, InnerTextBox.Height);
        }

        private void OnHoverItemChanged(TextBoxExItem old)
        {
            if (old != null)
                Invalidate(old.Bounds);

            if(HoverItem != null)
                Invalidate(HoverItem.Bounds);
        }

        private void OnPressedItemChanged(TextBoxExItem old)
        {
            if (old != null)
                Invalidate(old.Bounds);

            if (PressedItem != null)
                Invalidate(PressedItem.Bounds);
        }

        protected virtual void OnItemClick(TextBoxExItem item)
        {
            if (item != null)
                item.OnClick();

            if (ItemClick != null)
                ItemClick(this, item, EventArgs.Empty);
        }

        protected virtual void OnItemMouseDown(TextBoxExItem item)
        {
            if (ItemMouseDown != null)
                ItemMouseDown(this, item, EventArgs.Empty);
        }

        protected virtual void OnItemMouseUp(TextBoxExItem item)
        {
            if (ItemMouseUp != null)
                ItemMouseUp(this, item, EventArgs.Empty);
        }

        #region Items
        private void OnItemsChanged()
        {
            Items.ItemAdded += new XListEventHandler<TextBoxExItem>(Items_ItemAdded);
            Items.ItemRemoved += new XListEventHandler<TextBoxExItem>(Items_ItemRemoved);
            Items.ItemChanged += new XListValueEventHandler<TextBoxExItem>(Items_ItemChanged);
        }

        private void Items_ItemAdded(object sender, XListEventArgs<ResizableTextBox.TextBoxExItem> e)
        {
            if (e.Item != null)
            {
                if (e.Item.Owner != this)
                    e.Item.Owner = this;

                e.Item.BackColorChanged += new EventHandler(Item_BackColorChanged);
                e.Item.ImageChanged += new EventHandler(Item_ImageChanged);
                e.Item.EnabledChanged += new EventHandler(Item_EnabledChanged);
                e.Item.VisibleChanged += new EventHandler(Item_VisibleChanged);
                e.Item.AlignmentChanged += new EventHandler(Item_AlignmentChanged);
                e.Item.CustomSizeChanged += new EventHandler(Item_CustomSizeChanged);
            }
        }

        private void Items_ItemRemoved(object sender, XListEventArgs<ResizableTextBox.TextBoxExItem> e)
        {
            if (e.Item != null)
            {
                if (e.Item.Owner == this)
                    e.Item.Owner = null;

                e.Item.ImageChanged -= new EventHandler(Item_ImageChanged);
                e.Item.EnabledChanged -= new EventHandler(Item_EnabledChanged);
                e.Item.VisibleChanged -= new EventHandler(Item_VisibleChanged);
                e.Item.AlignmentChanged -= new EventHandler(Item_AlignmentChanged);
                e.Item.CustomSizeChanged -= new EventHandler(Item_CustomSizeChanged);
            }
        }

        private void Items_ItemChanged(object sender, XListValueEventArgs<ResizableTextBox.TextBoxExItem> e)
        {
            if (e.OldValue != null)
            {
                e.OldValue.ImageChanged -= new EventHandler(Item_ImageChanged);
                e.OldValue.EnabledChanged -= new EventHandler(Item_EnabledChanged);
                e.OldValue.VisibleChanged -= new EventHandler(Item_VisibleChanged);
                e.OldValue.AlignmentChanged -= new EventHandler(Item_AlignmentChanged);
                e.OldValue.CustomSizeChanged -= new EventHandler(Item_CustomSizeChanged);
            }

            if (e.NewValue != null)
            {
                e.NewValue.ImageChanged += new EventHandler(Item_ImageChanged);
                e.NewValue.EnabledChanged += new EventHandler(Item_EnabledChanged);
                e.NewValue.VisibleChanged += new EventHandler(Item_VisibleChanged);
                e.NewValue.AlignmentChanged += new EventHandler(Item_AlignmentChanged);
                e.NewValue.CustomSizeChanged += new EventHandler(Item_CustomSizeChanged);
            }
        }

        private void Item_AlignmentChanged(object sender, EventArgs e)
        {
            LayoutItemBounds();
            Invalidate();
        }

        private void Item_VisibleChanged(object sender, EventArgs e)
        {
            LayoutItemBounds();
            Invalidate();
        }

        private void Item_EnabledChanged(object sender, EventArgs e)
        {
            if (sender is TextBoxExItem)
                Invalidate(((TextBoxExItem)sender).Bounds);
            else
                Invalidate();
        }

        private void Item_ImageChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void Item_BackColorChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void Item_CustomSizeChanged(object sender, EventArgs e)
        {
            this.LayoutItemBounds();
            this.Invalidate();
        }
        #endregion
    }
}
