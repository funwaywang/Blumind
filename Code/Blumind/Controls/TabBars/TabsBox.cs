using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Darsson.Corlib.Collections;

namespace Blumind.Controls
{
    [DefaultEvent("SelectedIndexChanged")]
    public class TabsBox : BaseUserControl
    {
        TabAlignment _Alignment = TabAlignment.Top;
        int _TabSize = 28;
        Panel ControlContainer;
        int BorderSize = 1;
        int _SelectedIndex = -1;
        bool _ShowBorder = true;

        public event EventHandler SelectedIndexChanged;
        public event TabItemEventHandler TabItemClose;
        public event TabItemCancelEventHandler TabItemClosing;

        public TabsBox()
        {
            TabBar = new TabBar();
            TabBar.Dock = DockStyle.Top;
            TabBar.ShowDropDownButton = false;
            TabBar.ItemMinimumSize = 50;
            TabBar.SelectedItemChanged += new EventHandler(TabBar_SelectedItemChanged);
            TabBar.ItemClose += new TabItemEventHandler(TabBar_ItemClose);
            TabBar.ItemClosing += new TabItemCancelEventHandler(TabBar_ItemClosing);
            Controls.Add(TabBar);

            ControlContainer = new Panel();
            ControlContainer.Layout += new LayoutEventHandler(ControlContainer_Layout);
            Controls.Add(ControlContainer);

            TabPages = new XList<Page>();
            TabPages.ItemAdded += new XListEventHandler<Page>(TabPages_ItemAdded);
            TabPages.ItemRemoved += new XListEventHandler<Page>(TabPages_ItemRemoved);
        }

        [Browsable(false)]
        public TabBar TabBar { get; private set; }

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

        [DefaultValue(28)]
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
        public int Count
        {
            get { return ControlContainer.Controls.Count; }
        }

        [DefaultValue(typeof(Color), "Goldenrod")]
        public Color SelectedBackColor
        {
            get { return TabBar.SelectedItemBackColor; }
            set { TabBar.SelectedItemBackColor = value; }
        }

        [DefaultValue(typeof(Color), "Black")]
        public Color SelectedForeColor
        {
            get { return TabBar.SelectedItemForeColor; }
            set { TabBar.SelectedItemForeColor = value; }
        }

        [DefaultValue(typeof(Color), "Empty")]
        public Color ItemBackColor
        {
            get { return TabBar.ItemBackColor; }
            set { TabBar.ItemBackColor = value; }
        }

        [DefaultValue(typeof(Color), "Black")]
        public Color ItemForeColor
        {
            get { return TabBar.ItemForeColor; }
            set { TabBar.ItemForeColor = value; }
        }

        [DefaultValue(typeof(Color), "Empty")]
        public Color HoverItemBackColor
        {
            get { return TabBar.HoverItemBackColor; }
            set { TabBar.HoverItemBackColor = value; }
        }

        [DefaultValue(typeof(Color), "Black")]
        public Color HoverItemForeColor
        {
            get { return TabBar.HoverItemForeColor; }
            set { TabBar.HoverItemForeColor = value; }
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
        public Control ActivedPage
        {
            get
            {
                if (SelectedIndex > -1 && SelectedIndex < ControlContainer.Controls.Count)
                    return ControlContainer.Controls[SelectedIndex];
                else
                    return null;
            }
        }

        protected override bool ScaleChildren
        {
            get
            {
                return false;
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 100);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public XList<Page> TabPages { get; private set; }

        //[Browsable(false)]
        //public Control[] TabPages
        //{
        //    get
        //    {
        //        return ArrayHelper.ToArray<Control>(ControlContainer.Controls);
        //    }
        //}

        [DefaultValue(null)]
        public TabBarRenderer Renderer
        {
            get { return TabBar.Renderer; }
            set { TabBar.Renderer = value; }
        }

        private void OnAlignmentChanged()
        {
            TabBar.Alignment = Alignment;
            LayoutControls();
        }

        private void OnTabSizeChanged()
        {
            LayoutControls();
        }

        private void OnSelectedIndexChanged()
        {
            if (SelectedIndex > -1 && SelectedIndex < ControlContainer.Controls.Count)
                ActivePage(ControlContainer.Controls[SelectedIndex]);

            if (SelectedIndexChanged != null)
            {
                SelectedIndexChanged(this, EventArgs.Empty);
            }
        }

        private void OnShowBorderChanged()
        {
            LayoutControls();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            LayoutControls();
        }

        private void LayoutControls()
        {
            Rectangle rect = ClientRectangle;
            if(ShowBorder)
                rect.Inflate(-BorderSize, -BorderSize);

            switch (Alignment)
            {
                case TabAlignment.Right:
                    TabBar.Dock = DockStyle.Right;
                    TabBar.Width = TabSize;
                    rect.Width -= TabBar.Width - BorderSize;
                    break;
                case TabAlignment.Bottom:
                    TabBar.Dock = DockStyle.Bottom;
                    TabBar.Height = TabSize;
                    rect.Height -= TabBar.Height - BorderSize;
                    break;
                case TabAlignment.Left:
                    TabBar.Dock = DockStyle.Left;
                    TabBar.Width = TabSize;
                    rect.X += TabBar.Width - BorderSize;
                    rect.Width -= TabBar.Width - BorderSize;
                    break;
                case TabAlignment.Top:
                default:
                    TabBar.Dock = DockStyle.Top;
                    TabBar.Height = TabSize;
                    rect.Y += TabBar.Height - BorderSize;
                    rect.Height -= TabBar.Height - BorderSize;
                    break;
            }

            ControlContainer.Bounds = rect;
        }

        public Page AddPage(Control bodyControl)
        {
            return AddPage(bodyControl, null, false);
        }

        public Page AddPage(Control bodyControl, Image icon)
        {
            return AddPage(bodyControl, icon, false);
        }

        public Page AddPage(Control bodyControl, Image icon, bool closable)
        {
            Page p = new Page(bodyControl);
            p.Header.Icon = icon;
            p.Header.CanClose = closable;
            TabBar.Items.Add(p.Header);

            if (bodyControl is Form)
            {
                Form form = (Form)bodyControl;
                form.TopLevel = false;
                form.FormBorderStyle = FormBorderStyle.None;
                form.WindowState = FormWindowState.Normal;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.ControlBox = false;
                form.Dock = DockStyle.None;
            }
            //page.Dock = DockStyle.Fill;
            bodyControl.TextChanged += new EventHandler(Page_TextChanged);

            TabPages.Add(p);

            return p;
        }

        public void ActivePage(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException();

            ActivePage(ControlContainer.Controls[index]);
        }

        public void ActivePage(Control page)
        {
            if (page == null)
                return;

            TabBar.SelectByTag(page);
            SelectedIndex = ControlContainer.Controls.IndexOf(page);
            if(page.Bounds != ControlContainer.ClientRectangle)
                page.Bounds = ControlContainer.ClientRectangle;
            //page.Dock = DockStyle.Fill;

            foreach (Control control in ControlContainer.Controls)
            {
                control.Visible = control == page;
            }
        }

        public void ActivePage(Page page)
        {
            if (page == null)
                throw new ArgumentNullException();

            if (page.Body != null)
                ActivePage(page.Body);
            else if (page.Header != null)
                TabBar.SelectedItem = page.Header;
        }

        public bool RemovePage(Control bodyControl)
        {
            Page page = TabPages.Find(p => p.Body == bodyControl);
            if (page != null)
            {
                return TabPages.Remove(page);
            }
            else
            {
                return false;
            }
        }

        private void TabBar_SelectedItemChanged(object sender, EventArgs e)
        {
            if (TabBar.SelectedItem != null && TabBar.SelectedItem.Tag is Control)
            {
                ActivePage((Control)TabBar.SelectedItem.Tag);
            }
        }

        private void ControlContainer_Layout(object sender, LayoutEventArgs e)
        {
            if (ActivedPage != null && ActivedPage.Bounds != ControlContainer.ClientRectangle)
                ActivedPage.Bounds = ControlContainer.ClientRectangle;
        }

        private void Page_TextChanged(object sender, EventArgs e)
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

        protected override void OnPaint(PaintEventArgs e)
        {
            if (ShowBorder)
            {
                e.Graphics.Clear(BackColor);

                if (SelectedIndex > -1 && ControlContainer.Controls.Count > 0)
                    e.Graphics.Clear(TabBar.SelectedItemBackColor);
                else
                    e.Graphics.Clear(BackColor);
            }
            else
            {
                base.OnPaint(e);
            }

            if (DesignMode)
            {
                Pen penLine = new Pen(SystemColors.ControlDark);
                penLine.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                e.Graphics.DrawRectangle(penLine, 0, 0, Width - 1, Height - 1);
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            Control control = ActivedPage;
            if(control != null && control.CanFocus)
            {
                control.Focus();
            }
        }

        public void SetPageVisible(int index, bool visible)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException();

            TabBar.Items[index].Visible = visible;

            if (SelectedIndex == index)
                SelectNextPage(index);
        }

        private void SelectNextPage(int excludeIndex)
        {
            int index = -1;
            if (excludeIndex > 0)
                index = excludeIndex - 1;
            else if (excludeIndex < Count - 1)
                index = excludeIndex + 1;
            if (index > -1)
            {
                ActivePage(index);
            }
            else
            {
                SelectedIndex = -1;
                foreach (Control ctrl in ControlContainer.Controls)
                {
                    if (ctrl.Visible)
                        ctrl.Visible = false;
                }
            }
        }

        void TabBar_ItemClosing(object sender, TabItemCancelEventArgs e)
        {
            if (TabItemClosing != null)
                TabItemClosing(this, e);
        }

        void TabBar_ItemClose(object sender, TabItemEventArgs e)
        {
            Page page = TabPages.Find(p => p.Header == e.Item);
            if (page != null)
            {
                TabPages.Remove(page);
            }

            if (TabItemClose != null)
                TabItemClose(this, e);
        }

        void TabPages_ItemRemoved(object sender, XListEventArgs<TabsBox.Page> e)
        {
            if (e.Item != null)
            {
                if (TabBar.Items.Contains(e.Item.Header))
                    TabBar.Items.Remove(e.Item.Header);

                if (e.Item.Body != null && ControlContainer.Controls.Contains(e.Item.Body))
                {
                    e.Item.Body.SuspendLayout();
                    e.Item.Body.Visible = false;
                    ControlContainer.Controls.Remove(e.Item.Body);
                    e.Item.Body.ResumeLayout();
                }
            }
        }

        void TabPages_ItemAdded(object sender, XListEventArgs<TabsBox.Page> e)
        {
            if (e.Item != null)
            {
                if (e.Item.Body != null && !ControlContainer.Controls.Contains(e.Item.Body))
                {
                    ControlContainer.Controls.Add(e.Item.Body);
                    ActivePage(e.Item);
                }
            }
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            if (e.Control != TabBar && e.Control != ControlContainer)
            {
                Controls.Remove(e.Control);
                AddPage(e.Control);
            }
            else
            {
                base.OnControlAdded(e);
            }
        }

        public class Page
        {
            public TabItem Header { get; private set; }

            public Control Body { get; private set; }

            public Page(Control body)
                : this(new TabItem(), body)
            {
            }

            public Page(TabItem header, Control body)
            {
                Header = header;
                Body = body;

                if(Header == null)
                    Header = new TabItem();

                if(Body == null)
                    throw new ArgumentNullException("Body");

                Header.Text = Body.Text;
                Header.Tag = Body;
            }

            public bool CanClose
            {
                get { return Header.CanClose; }
                set { Header.CanClose = value; }
            }

            public Image Icon
            {
                get { return Header.Icon; }
                set { Header.Icon = value; }
            }

            public string Text
            {
                get { return Header.Text; }
                set { Header.Text = value; }
            }

            public override string ToString()
            {
                if (Header != null)
                    return Text;
                else
                    return null;
            }
        }
    }
}
