using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Linq;
using Blumind.Core;

namespace Blumind.Controls
{
    class MyTabControl : BaseControl
    {
        //TabBar TabBar;
        TabAlignment _Alignment = TabAlignment.Top;
        int _TabSize = 25;
        //Panel ControlContainer;
        int BorderSize = 1;
        int _SelectedIndex = -1;
        bool _ShowBorder = true;
        int _RoundSize = 5;
        XList<Control> _TabPages;
        Color _ContentBackColor = Color.White;

        public event EventHandler SelectedIndexChanged;

        public MyTabControl()
        {
            TabBar = new TabBar();
            TabBar.SelectedItemChanged += new EventHandler(TabBar_SelectedItemChanged);
            TabBar.Font = SystemFonts.MessageBoxFont;
            Controls.Add(TabBar);

            //ControlContainer = new Panel();
            //Controls.Add(ControlContainer);

            TabPages = new XList<Control>();
            TabPages.ItemAdded += new XListEventHandler<Control>(TabPages_ItemAdded);
            TabPages.ItemRemoved += new XListEventHandler<Control>(TabPages_ItemRemoved);
        }

        [DefaultValue(TabAlignment.Top)]
        public virtual TabAlignment Alignment
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

        [DefaultValue(25)]
        public int TabSize
        {
            get { return _TabSize; }
            set
            {
                if (_TabSize != value)
                {
                    _TabSize = value;
                    OnTabSizeChanged();
                }
            }
        }

        [Browsable(false)]
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set 
            {
                if (_SelectedIndex != value)
                {
                    _SelectedIndex = value;
                    OnSelectedIndexChanged();
                }
            }
        }

        [Browsable(false)]
        public Control SelectedPage
        {
            get
            {
                if (SelectedIndex < 0 || SelectedIndex >= TabPages.Count)
                    return null;

                return TabPages[SelectedIndex];
            }

            set
            {
                if (value == null || !TabPages.Contains(value))
                    SelectedIndex = -1;
                else
                    SelectedIndex = TabPages.IndexOf(value);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color SelectedBackColor
        {
            get { return TabBar.SelectedItemBackColor; }
            set { TabBar.SelectedItemBackColor = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color SelectedForeColor
        {
            get { return TabBar.SelectedItemForeColor; }
            set { TabBar.SelectedItemForeColor = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color ItemBackColor
        {
            get { return TabBar.ItemBackColor; }
            set { TabBar.ItemBackColor = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color ItemForeColor
        {
            get { return TabBar.ItemForeColor; }
            set { TabBar.ItemForeColor = value; }
        }

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
        
        [Browsable(false)]
        public XList<Control> TabPages
        {
            get { return _TabPages; }
            private set { _TabPages = value; }
        }

        protected TabBar TabBar { get; private set; }

        //protected override Padding DefaultPadding
        //{
        //    get
        //    {
        //        return new Padding(1);
        //    }
        //}

        //protected override Padding DefaultMargin
        //{
        //    get
        //    {
        //        return new Padding(1);
        //    }
        //}

        [DefaultValue(5)]
        public int RoundSize
        {
            get { return _RoundSize; }
            set
            {
                if (_RoundSize != value)
                {
                    _RoundSize = value;
                    OnRoundSizeChanged();
                }
            }
        }

        public override Rectangle DisplayRectangle
        {
            get
            {
                Rectangle rect = ClientRectangle.Inflate(Padding);

                if (ShowBorder)
                    rect.Inflate(-BorderSize, -BorderSize);

                switch (Alignment)
                {
                    case TabAlignment.Right:
                        rect.Width -= TabBar.Width - BorderSize;
                        rect.Width += Padding.Right;
                        break;
                    case TabAlignment.Bottom:
                        rect.Height -= TabBar.Height - BorderSize;
                        rect.Height += Padding.Bottom;
                        break;
                    case TabAlignment.Left:
                        rect.X += TabBar.Width - BorderSize;
                        rect.Width -= TabBar.Width - BorderSize;
                        rect.X -= Padding.Left;
                        rect.Width += Padding.Left;
                        break;
                    case TabAlignment.Top:
                    default:
                        rect.Y += TabBar.Height - BorderSize;
                        rect.Height -= TabBar.Height - BorderSize;
                        rect.Y -= Padding.Top;
                        rect.Height += Padding.Top;
                        break;
                }

                return rect;// base.DisplayRectangle;
            }
        }

        protected override bool ScaleChildren
        {
            get
            {
                return false;
            }
        }

        [DefaultValue(typeof(Color), "White")]
        public Color ContentBackColor
        {
            get { return _ContentBackColor; }
            set
            {
                if (_ContentBackColor != value)
                {
                    _ContentBackColor = value;
                    OnContentBackColorChanged();
                }
            }
        }

        void OnRoundSizeChanged()
        {
            Invalidate(false);
        }

        void OnAlignmentChanged()
        {
            TabBar.Alignment = Alignment;
            //LayoutControls();
            PerformLayout();
        }

        void OnTabSizeChanged()
        {
            //LayoutControls();
            PerformLayout();
        }

        protected virtual void OnSelectedIndexChanged()
        {
            if (SelectedIndex > -1 && SelectedIndex < TabPages.Count)
                ActivePage(TabPages[SelectedIndex]);

            if (SelectedIndexChanged != null)
            {
                SelectedIndexChanged(this, EventArgs.Empty);
            }
        }

        void OnShowBorderChanged()
        {
            //LayoutControls();
            PerformLayout();
        }

        void OnContentBackColorChanged()
        {
            Invalidate(false);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (!DesignMode)
            {
                //TabBar.Dock = DockStyle.Top;
                TabBar.ItemSpace = 2;
                TabBar.ShowDropDownButton = false;
            }
        }

        //protected override void OnResize(EventArgs e)
        //{
        //    base.OnResize(e);

        //    //LayoutControls();
        //    PerformLayout();
        //}

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            LayoutTabBar(e);
        }

        void LayoutTabBar(LayoutEventArgs e)
        {
            Rectangle rect = ClientRectangle;

            switch (Alignment)
            {
                case TabAlignment.Right:
                    TabBar.Bounds = new Rectangle(rect.Right - TabSize, rect.Top, TabSize, rect.Height);
                    break;
                case TabAlignment.Bottom:
                    TabBar.Bounds = new Rectangle(rect.Left, rect.Bottom - TabSize, rect.Width, TabSize);
                    break;
                case TabAlignment.Left:
                    TabBar.Bounds = new Rectangle(rect.Left, rect.Top, TabSize, rect.Height);
                    break;
                case TabAlignment.Top:
                default:
                    TabBar.Bounds = new Rectangle(rect.Left, rect.Top, rect.Width, TabSize);
                    break;
            }
        }
        
        public void AddPage(Control page)
        {
            AddPage(page, null);
        }

        public void AddPage(Control page, Image icon)
        {
            TabPages.Add(page);

            if (icon != null)
            {
                var tabItem = TabBar.Items.Find(it => it.Tag == page);
                if (tabItem != null)
                    tabItem.Icon = icon;
            }
        }

        public void InsertPage(int index, Control page)
        {
            InsertPage(index, page, null);
        }

        public void InsertPage(int index, Control page, Image icon)
        {
            TabPages.Insert(index, page);

            if (icon != null)
            {
                var tabItem = TabBar.Items.Find(it => it.Tag == page);
                if (tabItem != null)
                    tabItem.Icon = icon;
            }
        }

        public void ActivePage(int index)
        {
            if (index < 0 || index >= TabPages.Count)
                throw new ArgumentOutOfRangeException();

            ActivePage(TabPages[index]);
        }

        public void ActivePage(Control page)
        {
            if (page == null)
                return;

            if (TabBar.SelectByTag(page))
            {
                //HidePages();

                // 注意一下操作的顺序, 可以减少切换闪烁

                if (!Controls.Contains(page))
                {
                    page.Dock = DockStyle.Fill;
                    page.Bounds = DisplayRectangle;
                    Controls.Add(page);
                }

                foreach (Control ctrl in Controls)
                {
                    if (ctrl != page && ctrl != TabBar)
                        Controls.Remove(ctrl);
                }

                if (this.ContainsFocus && !page.ContainsFocus && page.CanFocus)
                {
                    page.Focus();
                }

                SelectedIndex = TabPages.IndexOf(page);
            }
        }

        void HidePages()
        {
            for (int i = Controls.Count - 1; i >= 0; i--)
            {
                Control ctrl = Controls[i];
                if (ctrl != TabBar)
                {
                    Controls.Remove(ctrl);
                }
            }
        }

        void TabBar_SelectedItemChanged(object sender, EventArgs e)
        {
            if (TabBar.SelectedItem != null && TabBar.SelectedItem.Tag is Control)
            {
                ActivePage((Control)TabBar.SelectedItem.Tag);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (ShowBorder)
            {
                e.Graphics.ExcludeClip(TabBar.Bounds);
                e.Graphics.Clear(BackColor);
                
                Rectangle rect = ClientRectangle;
                e.Graphics.DrawRectangle(new Pen(TabBar.BaseLineColor),
                    rect.X,
                    rect.Y,
                    rect.Width - 1,
                    rect.Height - 1);
            }
            else
            {
                base.OnPaint(e);
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            Control control = SelectedPage;
            if(control != null && control.CanFocus)
            {
                control.Focus();
            }
        }

        public void SetPageVisible(int index, bool visible)
        {
            if (index < 0 || index >= TabPages.Count)
                throw new ArgumentOutOfRangeException();

            TabBar.Items[index].Visible = visible;

            if (SelectedIndex == index)
                SelectNextPage(index);
        }

        void SelectNextPage(int excludeIndex)
        {
            int index = -1;
            if (excludeIndex > 0)
            {
                index = excludeIndex - 1;
            }
            else if (excludeIndex < TabPages.Count - 1)
            {
                index = excludeIndex + 1;
            }

            if (index > -1)
            {
                ActivePage(index);
            }
            else
            {
                SelectedIndex = -1;
                HidePages();
            }
        }

        void TabPages_ItemRemoved(object sender, XListEventArgs<Control> e)
        {
            if (e.Item != null)
            {
                TabItem item = TabBar.GetItemByTag(e.Item);
                if (item != null)
                    TabBar.Items.Remove(item);

                //
                if (Controls.Contains(e.Item))
                {
                    Controls.Remove(e.Item);
                }
                SelectNextPage(e.Index);
            }
        }

        void TabPages_ItemAdded(object sender, XListEventArgs<Control> e)
        {
            if (e.Item != null)
            {
                TabItem ti = new TabItem(e.Item.Text);
                ti.Tag = e.Item;
                //ti.Icon = icon;
                if (e.Item is IIconProvider)
                    ti.Icon = ((IIconProvider)e.Item).Icon;

                if (e.Index > -1 && e.Index < TabBar.Items.Count)
                    TabBar.Items.Insert(e.Index, ti);
                else
                    TabBar.Items.Add(ti);

                //page.Dock = DockStyle.Fill;
                e.Item.TextChanged += new EventHandler(TabPage_TextChanged);

                //ControlContainer.Controls.Add(page);
                if (TabBar.Items.Count == 1 || SelectedIndex < 0)
                {
                    ActivePage(e.Item);
                }
            }
        }

        void TabPage_TextChanged(object sender, EventArgs e)
        {
            if (sender is Control)
            {
                TabItem ti = TabBar.GetItemByTag(sender);
                if (ti != null)
                {
                    ti.Text = ((Control)sender).Text;
                }
            }
        }

        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);

            if (TabBar != null)
            {
                TabBar.ItemBackColor = this.BackColor;
                //TabBar.ItemBackColor = PaintHelper.GetDarkColor(this.BackColor, 0.1);
            }
        }
    }
}
