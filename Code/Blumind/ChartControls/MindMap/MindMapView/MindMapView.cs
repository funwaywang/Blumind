using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Blumind.Core;
using Blumind.Core.Exports;
using Blumind.Model;
using Blumind.Model.Widgets;
using Blumind.Controls.Charts;
using Blumind.Configuration;
using Blumind.Model.MindMaps;
using Blumind.Model.Styles;

namespace Blumind.Controls.MapViews
{
    partial class MindMapView : ChartControl, IDropFilesHandler
    {
        bool HasScrollToCenter = false;
        LinesLayer LinksLayer;
        ChartTooltipLayer ToolTipLayer;

        public MindMapView()
        {
            ShowNavigationMap = true;
            Render = new GeneralRender();
            ShowToolTips = true;
            NavigationBoxSize = new Size(300, 300);
            //_Options.Current.ChartSettingChanged += new EventHandler(Default_ChartSettingChanged);
            Options.Current.OpitonsChanged += Current_OpitonsChanged;

            ExtentChartInputKey = Keys.Left | Keys.Up | Keys.Right | Keys.Down;

            LinksLayer = new LinesLayer(this);
            Layers.Add(LinksLayer);

            ToolTipLayer = new ChartTooltipLayer(this);
            Layers.Add(ToolTipLayer);

            ResetChartStyle();

            InitializeMouse();
            InitializeKeyBoard();
        }

        #region Properties
        MindMap _Map;
        //private IRender _Render;
        bool _ReadOnly;

        [DefaultValue(null), Browsable(false)]
        public MindMap Map
        {
            get { return _Map; }
            set 
            {
                if (_Map != value)
                {
                    MindMap old = _Map;
                    _Map = value;
                    OnMapChanged(old);
                }
            }
        }

        [DefaultValue(null), Browsable(false)]
        public IMindMapRender Render { get; private set; }

        [DefaultValue(true), Browsable(false)]
        public bool ShowToolTips { get; set; }

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

        [DefaultValue(false)]
        public Layouter ChartLayouter { get; private set; }

        public override Model.Documents.ChartPage ChartPage
        {
            get { return Map; }
        }

        //protected override Padding DefaultMargin
        //{
        //    get
        //    {
        //        return new Padding(8);
        //    }
        //}

        //protected override Padding DefaultPadding
        //{
        //    get
        //    {
        //        return new Padding(2);
        //    }
        //}

        private void OnMapChanged(MindMap old)
        {
            if (old != null)
            {
                old.PropertyChanged -= new Blumind.Core.PropertyChangedEventHandler(Map_StyleChanged);
                old.LayoutTypeChanged -= new EventHandler(Map_LayoutTypeChanged);
                old.ChartObjectPropertyChanged -= new Blumind.Core.PropertyChangedEventHandler(Map_ChartObjectPropertyChanged);

                old.TopicAdded -= new TopicEventHandler(Map_TopicAdded);
                old.TopicRemoved -= new TopicEventHandler(Map_TopicRemoved);
                old.TopicAfterSort -= new EventHandler(Map_TopicAfterSort);
                old.ObjectStyleChanged -= new ChartObjectPropertyEventHandler(Chart_ChartObjectStyleChanged);
                //old.TopicTextChanged -= new TopicEventHandler(Map_TopicTextChanged);
                old.TopicWidgetChanged -= new Blumind.Core.PropertyChangedEventHandler(Map_TopicWidgetChanged);
                old.TopicDescriptionChanged -= new TopicEventHandler(Map_TopicDescriptionChanged);
                old.TopicFoldedChanged -= new TopicEventHandler(Map_TopicFoldedChanged);
                old.TopicWidthChanged -= new TopicEventHandler(Map_TopicWidthChanged);
                old.TopicHeightChanged -= new TopicEventHandler(Map_TopicHeightChanged);
                old.TopicIconChanged -= new TopicEventHandler(Map_TopicIconChanged);
                old.TopicBeforeCollapse -= new TopicCancelEventHandler(Map_TopicBeforeCollapse);
                old.LinkAdded -= new LinkEventHandler(Map_LinkAdded);
                old.LinkRemoved -= new LinkEventHandler(Map_LinkRemoved);
                old.WidgetRemoved -= new WidgetEventHandler(Map_WidgetRemoved);

                old.LinkPropertyChanged -= new Blumind.Core.PropertyChangedEventHandler(Map_LinkChanged);
                old.LinkVisibleChanged -= new LinkEventHandler(Map_LinkVisibleChanged);
            }

            if (Map != null)
            {
                ChartLayouter = LayoutManage.GetLayouter(Map.LayoutType);

                Map.PropertyChanged += new Blumind.Core.PropertyChangedEventHandler(Map_StyleChanged);
                Map.LayoutTypeChanged += new EventHandler(Map_LayoutTypeChanged);
                Map.ModifiedChanged += new EventHandler(Map_ModifiedChanged);
                Map.ChartObjectPropertyChanged += new Blumind.Core.PropertyChangedEventHandler(Map_ChartObjectPropertyChanged);

                Map.TopicAdded += new TopicEventHandler(Map_TopicAdded);
                Map.TopicRemoved += new TopicEventHandler(Map_TopicRemoved);
                Map.TopicAfterSort += new EventHandler(Map_TopicAfterSort);
                Map.ObjectStyleChanged += new ChartObjectPropertyEventHandler(Chart_ChartObjectStyleChanged);
                //Map.TopicTextChanged += new TopicEventHandler(Map_TopicTextChanged);
                Map.TopicWidgetChanged += new Blumind.Core.PropertyChangedEventHandler(Map_TopicWidgetChanged);
                Map.TopicDescriptionChanged += new TopicEventHandler(Map_TopicDescriptionChanged);
                Map.TopicFoldedChanged += new TopicEventHandler(Map_TopicFoldedChanged);
                Map.TopicWidthChanged += new TopicEventHandler(Map_TopicWidthChanged);
                Map.TopicHeightChanged += new TopicEventHandler(Map_TopicHeightChanged);
                Map.TopicIconChanged += new TopicEventHandler(Map_TopicIconChanged);
                Map.TopicBeforeCollapse += new TopicCancelEventHandler(Map_TopicBeforeCollapse);
                Map.LinkAdded += new LinkEventHandler(Map_LinkAdded);
                Map.LinkRemoved += new LinkEventHandler(Map_LinkRemoved);
                Map.WidgetRemoved += new WidgetEventHandler(Map_WidgetRemoved);

                Map.LinkPropertyChanged += new Blumind.Core.PropertyChangedEventHandler(Map_LinkChanged);
                Map.LinkVisibleChanged += new LinkEventHandler(Map_LinkVisibleChanged);
            }
            else
            {
                ChartLayouter = null;
            }

            if (Map != null && Map.Root != null)
            {
                Select(Map.Root);
                //SelectTopic(Map.Root);
            }

            Modified = false;
            ClearCommandHistory();
            ResetChartStyle();
            UpdateView(ChangeTypes.All);
            ScrollToCenter();
        }

        private void Map_ChartObjectPropertyChanged(object sender, Blumind.Core.PropertyChangedEventArgs e)
        {
            if (e.Rollbackable)
            {
                ChartObjectPropertyChangedCommand command = new ChartObjectPropertyChangedCommand(
                    this, sender, e.PropertyName, e.OldValue, e.NewValue, e.Changes);
                ExecuteCommand(command);
            }

            if (!IsUpdating)
            {
                if (e.HasChanges(ChangeTypes.Layout))
                    this.UpdateView(e.Changes);
                else if (e.HasChanges(ChangeTypes.Visual))
                    InvalidateChart();
            }
        }

        private void OnReadOnlyChanged()
        {
            if (EditMode)
            {
                CancelEdit();
            }
        }

        protected override void OnChartBackColorChanged()
        {
            base.OnChartBackColorChanged();

            Invalidate();
        }

        #endregion

        #region Map & Topic Events

        void Map_StyleChanged(object sender, Blumind.Core.PropertyChangedEventArgs e)
        {
            if (e.HasChanges(ChangeTypes.Visual))
            {
                ResetChartStyle();
            }

            if (!IsUpdating)
            {
                if (e.HasChanges(ChangeTypes.Layout))
                    UpdateView(e.Changes);
                else if (e.HasChanges(ChangeTypes.Visual))
                    InvalidateChart();
            }
        }

        void Map_LayoutTypeChanged(object sender, EventArgs e)
        {
            ChartLayouter = LayoutManage.GetLayouter(Map.LayoutType);

            if (!IsUpdating)
            {
                UpdateView(ChangeTypes.AllVisual);
            }
        }

        void Chart_ChartObjectStyleChanged(object sender, ChartObjectPropertyEventArgs e)
        {
            if (!IsUpdating)
            {
                UpdateView(e.Changes);
            }
        }

        private void Map_TopicFoldedChanged(object sender, TopicEventArgs e)
        {
            TrySelectFolded(e.Topic);

            if (!IsUpdating)
            {
                UpdateView(ChangeTypes.AllVisual);
            }
        }

        //private void Map_TopicTextChanged(object sender, TopicEventArgs e)
        //{
        //    if (!IsUpdating)
        //    {
        //        UpdateView();
        //    }
        //    //InvalidateTopic(e.Topic);
        //}

        private void Map_TopicWidgetChanged(object sender, Blumind.Core.PropertyChangedEventArgs e)
        {
            if (!IsUpdating)
            {
                if(e.HasChanges(ChangeTypes.Layout))
                    UpdateView(e.Changes);
                else if(e.HasChanges(ChangeTypes.Visual))
                    InvalidateChart();
            }
        }

        private void Map_TopicDescriptionChanged(object sender, TopicEventArgs e)
        {
            InvalidateObject(e.Topic);
        }

        private void Map_TopicIconChanged(object sender, TopicEventArgs e)
        {
            if (!IsUpdating)
            {
                UpdateView(ChangeTypes.AllVisual);
            }
        }

        private void Map_TopicHeightChanged(object sender, TopicEventArgs e)
        {
            if (!IsUpdating)
            {
                UpdateView(ChangeTypes.Layout);
            }
        }

        private void Map_TopicWidthChanged(object sender, TopicEventArgs e)
        {
            if (!IsUpdating)
            {
                UpdateView(ChangeTypes.Layout);
            }
        }

        private void Map_TopicRemoved(object sender, TopicEventArgs e)
        {
            OnTopicRemoved(e.Topic);
        }

        private void Map_TopicAdded(object sender, TopicEventArgs e)
        {
            if (!IsUpdating)
            {
                UpdateView(ChangeTypes.All);
            }

            if (e.Topic != null)
            {
                //SelectTopic(e.Topic);
                Select(e.Topic);
                EnsureVisible(e.Topic);
            }
        }

        private void Map_TopicAfterSort(object sender, EventArgs e)
        {
            if (!IsUpdating)
            {
                UpdateView(ChangeTypes.All);
            }
        }

        private void Map_TopicBeforeCollapse(object sender, TopicCancelEventArgs e)
        {
            if (e.Topic != null && e.Topic.IsRoot)
            {
                e.Cancel = true;
            }
        }

        private void Map_LinkChanged(object sender, Blumind.Core.PropertyChangedEventArgs e)
        {
            if (e.HasChanges(ChangeTypes.Layout))
                UpdateView(e.Changes);
            else if (e.HasChanges(ChangeTypes.Visual))
                InvalidateChart();
            //if (e.Link == null)
            //{
            //    InvalidateChart();
            //}
            //else
            //{
            //    Rectangle rect = e.Link.Bounds;
            //    rect.Offset(TranslatePoint);
            //    rect.Inflate(10, 10);
            //    InvalidateChart(rect);
            //}
        }

        private void Map_LinkVisibleChanged(object sender, LinkEventArgs e)
        {
            UpdateView(ChangeTypes.All);
        }

        private void Map_ModifiedChanged(object sender, EventArgs e)
        {
            OnModifiedChanged();
        }

        private void Map_LinkRemoved(object sender, LinkEventArgs e)
        {
            Unselect(e.Link);
            UpdateView(ChangeTypes.All);
        }

        private void Map_LinkAdded(object sender, LinkEventArgs e)
        {
            UpdateView(ChangeTypes.All);
        }

        private void Map_WidgetRemoved(object sender, WidgetEventArgs e)
        {
            Unselect(e.Widget);
            UpdateView(ChangeTypes.All);
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (StyleBrushCursor != null)
            {
                StyleBrushCursor.Dispose();
                StyleBrushCursor = null;
            }

            DragBox.Dispose();
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            
            if (!IsUpdating)
            {
                UpdateView(ChangeTypes.AllVisual);
                ScrollToCenter();
            }
        }

        private void LayoutView()
        {
            if (Map == null)
            {
                OriginalContentSize = Size.Empty;
            }
            else
            {
                if (ChartLayouter != null && Created)
                {
                    Size size;
                    if (Created)
                    {
                        using (Graphics grf = CreateGraphics())
                        {
                            var args = new MindMapLayoutArgs(grf, this.Map, ChartBox.DefaultChartFont);
                            size = ChartLayouter.LayoutMap(Map, args);
                        }
                    }
                    else
                    {
                        var args = new MindMapLayoutArgs(this.Map, ChartBox.DefaultChartFont);
                        size = ChartLayouter.LayoutMap(Map, args);
                    }

                    OriginalContentSize = size;
                }
            }
        }

        public override void EnsureVisible(ChartObject chartObject)
        {
            if (chartObject is Topic)
            {
                Topic topic = (Topic)chartObject;

                BeginUpdateView();

                // Make sure all nodes in path is Unfolded
                bool changed = false;
                Topic tp = topic.ParentTopic;
                while (tp != null)
                {
                    if (tp.Folded)
                    {
                        tp.Folded = false;
                        changed = true;
                    }
                    tp = tp.ParentTopic;
                }

                if (changed)
                    EndUpdateView(ChangeTypes.All);
                else
                    CancelUpdateView();

                Rectangle rect = topic.Bounds;
                rect.Inflate(10, 10);
                rect = PaintHelper.Zoom(rect, Zoom);
                EnsureVisible(rect);
            }
        }

        public override void UpdateView(ChangeTypes ut)
        {
            CancelEdit();

            if ((ut & ChangeTypes.Layout) == ChangeTypes.Layout)
            {
                LayoutView();
            }

            if (SelectedTopic != null)
                EnsureVisible(SelectedTopic);

            base.UpdateView(ut);
        }

        void ResetChartStyle()
        {
            if (Map != null && Map.Font != null)
                ChartBox.Font = Map.Font;
            else
                ChartBox.Font = this.Font;

            if (Map != null)
            {
                ChartForeColor = Map.ForeColor;
                ChartBackColor = Map.BackColor;

                if (Map.SelectColor.IsEmpty)
                    SelectBoxColor = SystemColors.Highlight;
                else
                    SelectBoxColor = Map.SelectColor;
            }
            else
            {
                ChartForeColor = Color.Black;
                ChartBackColor = Color.White;
                SelectBoxColor = SystemColors.Highlight;
            }
        }

        private void ExpandSelect(bool recursive)
        {
            if (SelectedTopic != null)
            {
                if (recursive)
                    SelectedTopic.ExpandAll();
                else
                    SelectedTopic.Expand();
            }
        }

        private void CollapseSelect(bool recursive)
        {
            if (SelectedTopic != null)
            {
                if (recursive)
                    SelectedTopic.CollapseAll();
                else
                    SelectedTopic.Collapse();
            }
        }

        private void ToggleFolding()
        {
            if (SelectedTopic != null)
            {
                SelectedTopic.Toggle();
            }
        }

        public void ExpandAll()
        {
            Topic topic = SelectedTopic;
            if (topic != null)
            {
                BeginUpdateView();
                topic.ExpandAll();
                EndUpdateView(ChangeTypes.Layout);
            }
            else if (Map != null)
            {
                BeginUpdateView();
                Map.ExpandAll();
                EndUpdateView(ChangeTypes.Layout);
            }
        }

        public void CollapseAll()
        {
            Topic topic = SelectedTopic;
            if (topic != null)
            {
                BeginUpdateView();
                topic.CollapseAll();
                EndUpdateView(ChangeTypes.Layout);
            }
            else if(Map != null)
            {
                BeginUpdateView();
                Map.CollapseAll();
                EndUpdateView(ChangeTypes.Layout);
            }
        }

        private void OnTopicRemoved(Topic topic)
        {
            if (topic.Selected)
            {
                Selection.RemoveInHistory(topic);
                ChartObject[] lastSel = Selection.PopHistory();
                if (lastSel != null)
                    //SelectTopic(lastSel, true);
                    Select(lastSel);
                else if (Map != null)
                    //SelectTopic(Map.Root, true);
                    Select(Map.Root);
                else
                    ClearSelection();
                    //SelectTopic(null);
            }

            if (!IsUpdating)
            {
                UpdateView(ChangeTypes.Layout);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (!HasScrollToCenter && Created)
            {
                ScrollToCenter();
                HasScrollToCenter = true;
            }
        }

        /*public override void ExportImage(string filename, string typeMime)
        {
            ChartsExportEngine engine = ChartsExportEngine.GetEngine(typeMime);
            if (engine == null)
            {
                this.ShowMessage("The format is not supported", MessageBoxIcon.Error);
            }
            else
            {
                engine.Export(this, filename);
            }
        }*/

        public Image GenerateImage(RenderMode renderMode, bool transparentBackground)
        {
            Bitmap bmp = new Bitmap(OriginalContentSize.Width, OriginalContentSize.Height);
            using (Graphics grf = Graphics.FromImage(bmp))
            {
                PaintHelper.SetHighQualityRender(grf);

                if (!transparentBackground)
                {
                    grf.Clear(ChartBackColor);
                }

                if (Render != null && Map != null)
                {
                    RenderArgs args = new RenderArgs(renderMode, grf, this, ChartBox.DefaultChartFont);
                    Render.Paint(Map, args);
                }

                grf.Dispose();
            }

            return bmp;
        }

        void Default_ChartSettingChanged(object sender, EventArgs e)
        {
            UpdateView(ChangeTypes.AllVisual);
            //InvalidateChart();
        }

        void Current_OpitonsChanged(object sender, EventArgs e)
        {
            UpdateView(ChangeTypes.AllVisual);
        }

        public void OpenSelectedUrl()
        {
            if (Selection.Count > 0)
            {
                List<string> urls = new List<string>();
                foreach (object obj in Selection)
                {
                    if (obj is IHyperlink && !string.IsNullOrEmpty(((IHyperlink)obj).Hyperlink))
                    {
                        urls.Add(((IHyperlink)obj).Hyperlink);
                    }
                }

                foreach (string url in urls)
                {
                    Helper.OpenUrl(url);
                }
            }
        }

        public override void ApplyChartTheme(ChartTheme theme)
        {
            if (Map != null)
            {
                BeginUpdateView();
                Map.ApplyTheme(theme);
                EndUpdateView(ChangeTypes.Visual);
            }
        }

        public override ChartObject FindNext(MindMapFinder Finder, string findWhat)
        {
            Topic start = SelectedTopic;
            if (start == null)
                start = Map.Root;
            if (start == null)
                return null;

            return Finder.FindNext(start, findWhat);
        }
    }
}
