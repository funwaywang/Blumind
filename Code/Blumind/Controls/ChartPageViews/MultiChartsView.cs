using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Blumind.ChartPageView;
using Blumind.Controls;
using Blumind.Controls.MapViews;
using Blumind.Core;
using Blumind.Globalization;
using Blumind.Model.Documents;

namespace Blumind.ChartPageView
{
    class MultiChartsView : MyTabControl
    {
        Document _Document;
        SpecialTabItem BtnNewChart;
        List<ToolStripItem> LastCustomTabMenus { get; set; }

        public event EventHandler NewChartPage;
        public event NeedShowPropertyEventHandler NeedShowProperty;

        public MultiChartsView()
        {
            InitializeComponents();

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        [Browsable(false), DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Document Document
        {
            get { return _Document; }
            set
            {
                if (_Document != value)
                {
                    Document old = _Document;
                    _Document = value;
                    OnDocumentChanged(old);
                }
            }
        }

        [DefaultValue(null)]
        public ContextMenuStrip TabsMenuStrip { get; set; }

        BaseChartPage CreateNewPage(ChartPage chart)
        {
            var cp = BaseChartPage.CreateChartPage(chart);
            cp.BackColorChanged += new EventHandler(cp_BackColorChanged);
            cp.NeedShowProperty += new NeedShowPropertyEventHandler(cp_NeedShowProperty);
            cp.Owner = this;
            return cp;
        }

        void InitializeComponents()
        {
            Alignment = System.Windows.Forms.TabAlignment.Bottom;

            BtnNewChart = new SpecialTabItem(Properties.Resources._new);
            BtnNewChart.Click += new EventHandler(BtnNewChart_Click);
            TabBar.RightSpecialTabs.Add(BtnNewChart);

            var navBtnFirst = new TabBarNavButton(Lang._("First"), Properties.Resources.nav_small_first);
            navBtnFirst.Click += navBtnFirst_Click;
            var navBtnPrev = new TabBarNavButton(Lang._("Previous"), Properties.Resources.nav_small_prev);
            navBtnPrev.Click += navBtnPrev_Click;
            var navBtnNext = new TabBarNavButton(Lang._("Next"), Properties.Resources.nav_small_next);
            navBtnNext.Click += navBtnNext_Click;
            var navBtnLast = new TabBarNavButton(Lang._("Last"), Properties.Resources.nav_small_last);
            navBtnLast.Click += navBtnLast_Click;
            TabBar.LeftButtons.Add(navBtnFirst);
            TabBar.LeftButtons.Add(navBtnPrev);
            TabBar.RightButtons.Add(navBtnNext);
            TabBar.RightButtons.Add(navBtnLast);

            TabBar.MaxItemSize = 250;
            TabBar.AllowScrollPage = true;
            TabBar.Renderer = new MultiChartsTabRenderer(TabBar);
            TabBar.Padding = new System.Windows.Forms.Padding(TabBar.Padding.Left + 2, TabBar.Padding.Top, TabBar.Padding.Right, TabBar.Padding.Bottom);
            TabBar.MouseDown += new MouseEventHandler(TabBar_MouseDown);
        }

        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);

            if (TabBar != null)
            {
                //TabBar.ItemBackColor = this.BackColor;
                TabBar.ItemBackColor = PaintHelper.GetDarkColor(this.BackColor, 0.15);
                TabBar.ItemForeColor = PaintHelper.FarthestColor(TabBar.ItemBackColor, theme.Colors.Dark, theme.Colors.Light);
            }
        }

        void navBtnLast_Click(object sender, EventArgs e)
        {
            TabBar.ScrollToLast();
        }

        void navBtnPrev_Click(object sender, EventArgs e)
        {
            TabBar.ScrollToPrev();
        }

        void navBtnNext_Click(object sender, EventArgs e)
        {
            TabBar.ScrollToNext();
        }

        void navBtnFirst_Click(object sender, EventArgs e)
        {
            TabBar.ScrollToFirst();
        }

        Control FindPage(ChartPage chart)
        {
            foreach (var page in TabPages)
            {
                if (page is BaseChartPage && ((BaseChartPage)page).Chart == chart)
                    return page;
            }

            return null;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if(!DesignMode && SelectedPage != null)
                ContentBackColor = SelectedPage.BackColor;
        }

        void OnTabsMenuStripChanged(System.Windows.Forms.ContextMenuStrip old)
        {
        }

        void OnDocumentChanged(Document old)
        {
            TabPages.Clear();

            if (old != null)
            {
                old.Charts.ItemAdded -= new XListEventHandler<ChartPage>(Charts_ItemAdded);
                old.Charts.ItemRemoved -= new XListEventHandler<ChartPage>(Charts_ItemRemoved);
            }

            if (Document != null)
            {
                var activeIndex = Document.ActiveChartIndex;
                foreach (var chart in Document.Charts)
                {
                    var page = CreateNewPage(chart);
                    page.IconChanged += new EventHandler(Page_IconChanged);
                    TabPages.Add(page);
                }

                if (activeIndex > -1 && activeIndex < TabPages.Count)
                {
                    ActivePage(activeIndex);
                }

                Document.Charts.ItemAdded += new XListEventHandler<ChartPage>(Charts_ItemAdded);
                Document.Charts.ItemRemoved += new XListEventHandler<ChartPage>(Charts_ItemRemoved);
                Document.ModifiedChanged += new EventHandler(Document_ModifiedChanged);
            }
        }

        void Document_ModifiedChanged(object sender, EventArgs e)
        {
            if (Document != null && !Document.Modified)
            {
                foreach(BaseChartPage page in TabPages)
                    page.Chart.Modified = false;
            }
        }

        void Charts_ItemAdded(object sender, XListEventArgs<ChartPage> e)
        {
            var page = CreateNewPage(e.Item);
            page.IconChanged += new EventHandler(Page_IconChanged);
            TabPages.Add(page);
        }

        void Page_IconChanged(object sender, EventArgs e)
        {
            if (sender is BaseChartPage)
            {
                var bcp = (BaseChartPage)sender;
                var item = TabBar.GetItemByTag(bcp);
                if (item != null)
                    item.Icon = bcp.Icon;
            }
        }

        void Charts_ItemRemoved(object sender, XListEventArgs<ChartPage> e)
        {
            Control page = FindPage(e.Item);
            if (page != null)
                TabPages.Remove(page);
        }

        void BtnNewChart_Click(object sender, EventArgs e)
        {
            if (NewChartPage != null)
                NewChartPage(this, EventArgs.Empty);
        }

        void TabBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && TabsMenuStrip != null)
            {
                TabItem item = TabBar.GetItemAt(e.X, e.Y);
                if (item != null && !(item is SpecialTabItem))
                {
                    CustomTabMenu(item, TabsMenuStrip);
                    TabsMenuStrip.Show(TabBar, new Point(e.X, e.Y), ToolStripDropDownDirection.AboveRight);
                }
            }
        }

        void CustomTabMenu(TabItem item, System.Windows.Forms.ContextMenuStrip TabsMenuStrip)
        {
            if (LastCustomTabMenus != null)
            {
                foreach (var mi in LastCustomTabMenus)
                    if (TabsMenuStrip.Items.Contains(mi))
                        TabsMenuStrip.Items.Remove(mi);
                LastCustomTabMenus.Clear();
                LastCustomTabMenus = null;
            }

            List<ToolStripItem> menuItems = null;
            if(item != null && TabPages[item.DisplayIndex] is BaseChartPage)
            {
                menuItems = ((BaseChartPage)TabPages[item.DisplayIndex]).CustomTabMenuItems();
                if (menuItems != null && menuItems.Count > 0)
                {
                    int index = 2;
                    foreach (var mi in menuItems)
                        TabsMenuStrip.Items.Insert(index++, mi);
                }

                LastCustomTabMenus = menuItems;
            }
        }

        protected override void OnSelectedIndexChanged()
        {
            base.OnSelectedIndexChanged();

            if (SelectedPage != null)
            {
                ContentBackColor = SelectedPage.BackColor;
            }
        }

        void cp_BackColorChanged(object sender, EventArgs e)
        {
            if (sender == SelectedPage && SelectedPage != null)
            {
                ContentBackColor = SelectedPage.BackColor;
            }
        }

        void cp_NeedShowProperty(object sender, NeedShowPropertyEventArgs e)
        {
            OnNeedShowProperty(e);
        }

        void OnNeedShowProperty(NeedShowPropertyEventArgs e)
        {
            if (NeedShowProperty != null)
                NeedShowProperty(this, e);
        }

        public void NotifyCurrentLanguageChanged()
        {
            foreach (var ctrl in TabPages)
            {
                if (ctrl is ChartControl)
                {
                    ((ChartControl)ctrl).NotifyCurrentLanguageChanged();
                }
            }
        }

        public void ActiveChartPage(ChartPage chartPage)
        {
            if (chartPage == null)
                return;

            var tp = FindPage(chartPage);
            if (tp != null)
            {
                ActivePage(tp);
            }
        }
    }
}
