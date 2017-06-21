using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using Blumind.Controls.MapViews;
using Blumind.Core;
using Blumind.Controls;
using Blumind.Model;
using Blumind.Globalization;

namespace Blumind.ChartPageView
{
    class BaseChartPage : UserControl, IIconProvider
    {
        Model.Documents.ChartPage _Chart;
        object[] _SelectedObjects;
        bool _ReadOnly;
        Image _Icon;

        public event EventHandler SelectedObjectsChanged;
        public event NeedShowPropertyEventHandler NeedShowProperty;
        public event EventHandler IconChanged;

        public BaseChartPage()
        {
            InitializationChartContextMenuStrip();
        }

        public Model.Documents.ChartPage Chart
        {
            get { return _Chart; }
            set
            {
                if (_Chart != value)
                {
                    _Chart = value;
                    OnChartChanged();
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        [Browsable(false)]
        public virtual ChartControl ChartBox
        {
            get { return null; }
        }

        [Browsable(false)]
        public object[] SelectedObjects
        {
            get { return _SelectedObjects; }
            set
            {
                if (_SelectedObjects != value)
                {
                    _SelectedObjects = value;
                    OnSelectedObjectsChanged();
                }
            }
        }

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

        [DefaultValue(null)]
        public Image Icon
        {
            get { return _Icon; }
            set
            {
                if (_Icon != value)
                {
                    _Icon = value;
                    OnIconChanged();
                }
            }
        }

        [DefaultValue(null), Browsable(false)]
        public MultiChartsView Owner { get; internal set; }

        #region Chart ContextMenu
        List<ToolStripItem> ExtendActionItems { get; set; }

        protected ContextMenuStrip ChartContextMenuStrip { get; private set; }

        protected virtual int ExtendActionMenuIndex
        {
            get
            {
                return -1;
            }
        }

        void InitializationChartContextMenuStrip()
        {
            //
            ChartContextMenuStrip = new ContextMenuStrip();
            ChartContextMenuStrip.Opening += ChartContextMenu_Opening;
            ChartContextMenuStrip.Closed += ChartContextMenuStrip_Closed;
            InitializationChartContextMenuStrip(ChartContextMenuStrip);

            //
            ExtendActionItems = new List<ToolStripItem>();
        }

        protected virtual void InitializationChartContextMenuStrip(ContextMenuStrip contextMenu)
        {
        }

        void ChartContextMenu_Opening(object sender, CancelEventArgs e)
        {
            CreateExtendActionMenus();

            OnChartContextMenuStripOpening(e);
        }

        protected virtual void OnChartContextMenuStripOpening(CancelEventArgs e)
        {
        }

        void ChartContextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            ClearExtendActionMenus();
        }

        public virtual IEnumerable<ExtendActionInfo> GetExtendActions()
        {
            return null;
        }

        void CreateExtendActionMenus()
        {
            ClearExtendActionMenus();

            if (ChartContextMenuStrip == null || SelectedObjects.Length != 1)
                return;

            var mindex = ExtendActionMenuIndex;
            if (mindex < 0 || mindex >= ChartContextMenuStrip.Items.Count)
                mindex = ChartContextMenuStrip.Items.Count;


            var so = SelectedObjects.First();
            if (so is IExtendActionProvider)
            {
                var ep = (IExtendActionProvider)so;
                CreateExtendActionMenus(ep.GetExtendActions(ReadOnly), ref mindex);
            }

            //
            CreateExtendActionMenus(GetExtendActions(), ref mindex);

            // separators
            if (!ExtendActionItems.IsEmpty())
            {
                var min = ChartContextMenuStrip.Items.IndexOf(ExtendActionItems.First());
                var max = ChartContextMenuStrip.Items.IndexOf(ExtendActionItems.Last());

                if (max < ChartContextMenuStrip.Items.Count - 1 && !(ChartContextMenuStrip.Items[max + 1] is ToolStripSeparator))
                {
                    var tss = new ToolStripSeparator();
                    ChartContextMenuStrip.Items.Insert(max, tss);
                    ExtendActionItems.Add(tss);
                }

                if (min > 0 && !(ChartContextMenuStrip.Items[min - 1] is ToolStripSeparator))
                {
                    var tss = new ToolStripSeparator();
                    ChartContextMenuStrip.Items.Insert(min, tss);
                    ExtendActionItems.Add(tss);
                }
            }
        }

        void CreateExtendActionMenus(IEnumerable<ExtendActionInfo> actions, ref int index)
        {
            if (actions.IsNullOrEmpty())
                return;

            if (!ExtendActionItems.IsEmpty())
            {
                var tss = new ToolStripSeparator();
                ChartContextMenuStrip.Items.Insert(index++, tss);
                ExtendActionItems.Add(tss);
            }

            foreach (var ea in actions)
            {
                var mi = new ToolStripMenuItem();
                mi.Text = Lang._(ea.Text);
                mi.Image = ea.Icon;
                mi.ShortcutKeyDisplayString = ea.ShortcutKeyDisplayString;
                mi.Click += ea.Processor;

                //
                ChartContextMenuStrip.Items.Insert(index++, mi);
                ExtendActionItems.Add(mi);
            }
        }

        void ClearExtendActionMenus()
        {
            if (ChartContextMenuStrip == null || ExtendActionItems.IsNullOrEmpty())
                return;

            foreach (var mi in ExtendActionItems)
            {
                if (ChartContextMenuStrip.Items.Contains(mi))
                    ChartContextMenuStrip.Items.Remove(mi);
            }

            ExtendActionItems.Clear();
        }

        #endregion 

        void InitializeShortcutKeys()
        {
            KeyMap.Default.KeyManChanged += new EventHandler(Default_KeyManChanged);
            Default_KeyManChanged(null, EventArgs.Empty);
        }

        public static BaseChartPage CreateChartPage(Model.Documents.ChartPage chart)
        {
            BaseChartPage chartPage = null;
            switch (chart.Type)
            {
                case Model.Documents.ChartType.MindMap:
                    chartPage = new MindMapChartPage();
                    break;
                case Model.Documents.ChartType.FlowDiagram:
                    chartPage = new FreeDiagramChartPage();
                    break;
                default:
                    break;
            }

            if(chartPage != null)
            {
                chartPage.Chart = chart;
                chartPage.Text = chart.Name;
            }

            return chartPage;
        }

        protected virtual void OnChartChanged()
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
            {
                if (ChartBox != null)
                {
                    ChartBox.ChartContextMenuStrip = this.ChartContextMenuStrip;
                }

                InitializeShortcutKeys();
                LanguageManage.CurrentChanged += new EventHandler(LanguageManage_CurrentChanged);
            }
        }

        protected virtual void OnReadOnlyChanged()
        {
        }

        protected virtual void OnIconChanged()
        {
            if(IconChanged != null)
                IconChanged(this, EventArgs.Empty);
        }

        protected virtual void OnSelectedObjectsChanged()
        {
            if (SelectedObjectsChanged != null)
                SelectedObjectsChanged(this, EventArgs.Empty);

            ResetControlStatus();
        }

        protected virtual void ResetControlStatus()
        {
        }

        protected void OnNeedShowProperty(bool force)
        {
            if (NeedShowProperty != null)
                NeedShowProperty(this, new NeedShowPropertyEventArgs(force));
        }

        void LanguageManage_CurrentChanged(object sender, EventArgs e)
        {
            OnCurrentLanguageChanged();
        }

        void Default_KeyManChanged(object sender, EventArgs e)
        {
            OnCurrentLanguageChanged();

            OnKeyMapChanged();
        }

        protected virtual void OnKeyMapChanged()
        {
        }

        protected virtual void OnCurrentLanguageChanged()
        {
        }

        public virtual List<ToolStripItem> CustomTabMenuItems()
        {
            return null;
        }

        public virtual void ApplyTheme(UITheme theme)
        {
            if (theme != null)
            {
                ChartContextMenuStrip.Renderer = theme.ToolStripRenderer;
            }
        }
    }
}
