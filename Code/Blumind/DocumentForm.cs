using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
using Blumind.ChartPageView;
using Blumind.Configuration;
using Blumind.Controls;
using Blumind.Controls.MapViews;
using Blumind.Core;
using Blumind.Design;
using Blumind.Dialogs;
using Blumind.Globalization;
using Blumind.Model;
using Blumind.Model.Documents;
using Blumind.Model.MindMaps;
using Blumind.Model.Styles;

namespace Blumind
{
    partial class DocumentForm : BaseDocumentForm, IThemableUI
    {
        const string ShowSidebarOptionName = "editor_show_sidebar";
        Document _Document;
        ObjectTreeView objectTree1;
        ChartOverviewBox cob;
        TimerDialog MyTimerDialog;
        ChartPageView.BaseChartPage _ActivedChartPage;
        List<PropertyBox> PropertyBoxies = new List<PropertyBox>();
        NormalPropertyBox _NormalPropertyBox;
        PropertyBox CurrentPropertyBox;
        ChartControl _ActiveChartBox;
        bool _ShowSidebar = true;
        object[] _SelectedObjects;
        ShortcutKeysTable ShortcutKeys;
        PrintDialog MyPrintDialog;
        bool HardClose;
        long Last_TsbFormatPainter_ClickTick = 0;
        //RemarkEditor remarkEditor;
        MyTabControl tabControl2; // right-bottom
        List<ToolStripItem> CurrentChartBoxItems = new List<ToolStripItem>();

        public event EventHandler ReadOnlyChanged;

        public DocumentForm()
        {
            InitializeComponent();

            Icon = Properties.Resources.document_icon;
            FormatPainter_DataChanged(null, EventArgs.Empty);
            FormatPainter.Default.DataChanged += new EventHandler(FormatPainter_DataChanged);

            if (System.Globalization.CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft && RightToLeft != RightToLeft.No)
                ChangeSideBarSide();

            InitializeControls();
            InitializeZoomMenu();
            InitializeShortcutKeys();
            //InitializeTimers();

            UITheme.Default.Listeners.Add(this);
            ApplyTheme(UITheme.Default);

            AfterInitialize();
        }

        public DocumentForm(Document document)
            : this()
        {
            Document = document;
        }

        public DocumentForm(string filename)
            : this()
        {
            if(!string.IsNullOrEmpty(filename) && File.Exists(filename))
                Document = Document.Load(filename);
        }

        public override Document Document
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

        public ChartPageView.BaseChartPage ActivedChartPage
        {
            get { return _ActivedChartPage; }

            set
            {
                if (_ActivedChartPage != value)
                {
                    ChartPageView.BaseChartPage old = _ActivedChartPage;
                    _ActivedChartPage = value;
                    OnActivedChartPageChanged(old);
                }
            }
        }

        public ChartControl ActiveChartBox
        {
            get { return _ActiveChartBox; }
            private set
            {
                if (_ActiveChartBox != value)
                {
                    var old = _ActiveChartBox;
                    _ActiveChartBox = value;
                    OnActiveChartBoxChanged(old);
                }
            }
        }

        [DefaultValue(true)]
        public bool ShowSidebar
        {
            get { return _ShowSidebar; }
            set
            {
                if (_ShowSidebar != value)
                {
                    _ShowSidebar = value;
                    OnShowSidebarChanged();
                }
            }
        }

        object[] SelectedObjects
        {
            get { return _SelectedObjects; }
            set
            {
                if (_SelectedObjects != value)
                {
                    object[] old = _SelectedObjects;
                    _SelectedObjects = value;
                    OnSelectedObjectsChanged(old);
                }
            }
        }

        void OnShowSidebarChanged()
        {
            splitContainer2.Visible = ShowSidebar;
            TsbSidebar.Checked = ShowSidebar;
        }

        void OnActiveChartBoxChanged(ChartControl old)
        {
            cob.Chart = ActiveChartBox;

            if (old != null)
            {
                old.CommandHistoryChanged -= new EventHandler(ActivedChartBox_CommandHistoryChanged);
                old.ZoomChanged -= new EventHandler(ActivedChartBox_ZoomChanged);
                old.ModifiedChanged -= new EventHandler(ActivedChartBox_ModifiedChanged);
                old.MouseMethodChanged -= new EventHandler(ActivedChartBox_MouseMethodChanged);
            }

            if (ActiveChartBox != null)
            {
                objectTree1.ChartPage = ActiveChartBox.ChartPage;
                ActiveChartBox.CommandHistoryChanged += new EventHandler(ActivedChartBox_CommandHistoryChanged);
                ActiveChartBox.ZoomChanged += new EventHandler(ActivedChartBox_ZoomChanged);
                ActiveChartBox.ModifiedChanged += new EventHandler(ActivedChartBox_ModifiedChanged);
                ActiveChartBox.MouseMethodChanged += new EventHandler(ActivedChartBox_MouseMethodChanged);
            }
            else
            {
                objectTree1.ChartPage = null;
            }

            ResetToolStripItems();
        }

        void OnActivedChartPageChanged(ChartPageView.BaseChartPage old)
        {
            if (old != null)
            {
                old.SelectedObjectsChanged -= new EventHandler(ActivedChartPage_SelectedObjectsChanged);
            }

            if (ActivedChartPage != null)
            {
                ActiveChartBox = ActivedChartPage.ChartBox;
                if(Document != null)
                    Document.ActiveChart = ActivedChartPage.Chart;

                SelectedObjects = ActivedChartPage.SelectedObjects;
                ActivedChartPage.SelectedObjectsChanged += new EventHandler(ActivedChartPage_SelectedObjectsChanged);
            }
            else
            {
                ActiveChartBox = null;
                SelectedObjects = null;
                if (Document != null)
                    Document.ActiveChart = null;
            }

            ResetControlStatus();
        }

        void OnSelectedObjectsChanged(object[] old)
        {
            if (SelectedObjects != null)
                ShowProperty(SelectedObjects);
            else if (ActivedChartPage != null)
                ShowProperty(ActivedChartPage.Chart);
            else
                ShowProperty(null);

            if (SelectedObjects != null && SelectedObjects.Length == 1 && SelectedObjects[0] is Topic)
                TsbFormatPainter.Enabled = true;
            else
                TsbFormatPainter.Enabled = TsbFormatPainter.Checked;
        }

        protected override void OnReadOnlyChanged()
        {
            foreach (BaseChartPage page in this.multiChartsView1.TabPages)
            {
                page.ReadOnly = ReadOnly;
            }
            //if (ActivedChartBox != null)
            //    ActivedChartBox.ReadOnly = ReadOnly;

            if (ReadOnlyChanged != null)
                ReadOnlyChanged(this, EventArgs.Empty);

            ResetControlStatus();

            base.OnReadOnlyChanged();
        }

        #region Initializetions
        void InitializeZoomMenu()
        {
            float[] zooms = new float[] { 0.25f, 0.5f, 0.75f, 1.0f, 1.5f, 2.0f, 3.0f, 4.0f };
            foreach (float zoom in zooms)
            {
                ToolStripMenuItem miZoom = new ToolStripMenuItem();
                miZoom.Text = string.Format("{0}%", (int)(zoom * 100));
                miZoom.Tag = zoom;
                miZoom.Checked = ActiveChartBox != null && zoom == ActiveChartBox.Zoom;
                miZoom.Click += new EventHandler(MenuZoom_Click);
                TsbZoom.DropDownItems.Add(miZoom);
            }
        }

        void InitializeTimers()
        {
            while (TsbTimer.DropDownItems.Count > 2)
                TsbTimer.DropDownItems.RemoveAt(2);

            foreach (TimeSegmentInfo tsi in TimeSegmentInfo.DefaultTimeSegments)
            {
                var mi = new ToolStripMenuItem();
                mi.Text = tsi.Name;
                mi.ToolTipText = tsi.Description;
                mi.Tag = tsi;
                mi.Click += new EventHandler(MenuTimer_Click);

                TsbTimer.DropDownItems.Add(mi);
            }
        }

        void InitializeControls()
        {
            //
            this.objectTree1 = new ObjectTreeView();
            cob = new ChartOverviewBox();
            tabControl2 = new MyTabControl();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            SuspendLayout();

            // 
            // objectTree1
            // 
            objectTree1.Text = "Objects";
            objectTree1.Icon = Properties.Resources.objects;
            objectTree1.TabIndex = 2;
            //objectTree1.TreeViewContextMenuStrip = this.menuMapChart;
            objectTree1.AfterSelect += new TreeViewEventHandler(this.objectTree1_SelectedObjectChanged);

            //
            cob.Icon = Properties.Resources.zoom;
            cob.Text = "Overview";
            cob.Chart = ActiveChartBox;

            //
            myTabControl1.SelectedBackColor = Color.White;
            myTabControl1.SelectedForeColor = Color.Black;
            myTabControl1.AddPage(objectTree1);
            myTabControl1.AddPage(cob);
            myTabControl1.SelectedIndex = 0;

            //
            tabControl2.Dock = DockStyle.Fill;
            tabControl2.SelectedBackColor = Color.White;
            tabControl2.SelectedForeColor = Color.Black;
            splitContainer2.Panel2.Controls.Add(tabControl2);

            //
            splitContainer2.Panel2.ResumeLayout(false);
            splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        void InitializeShortcutKeys()
        {
            KeyMap.Default.KeyManChanged += new EventHandler(Default_KeyManChanged);
            Default_KeyManChanged(null, EventArgs.Empty);

            ShortcutKeys = new ShortcutKeysTable();
            ShortcutKeys.Register(KeyMap.Save, delegate() { Save(); });
            ShortcutKeys.Register(KeyMap.Find, delegate() { ShowFindDialog(FindDialog.FindDialogMode.Find); });
            ShortcutKeys.Register(KeyMap.Replace, delegate() { ShowFindDialog(FindDialog.FindDialogMode.Replace); });
            ShortcutKeys.Register(KeyMap.Sidebar, delegate() { ShowSidebar = !ShowSidebar; });

            ShortcutKeys.Register(KeyMap.FullScreen, delegate() { FullScreen = !FullScreen; });
        }

        void InitializeThemes()
        {
            while (TsbThemes.DropDownItems.Count > 4)
            {
                TsbThemes.DropDownItems.RemoveAt(4);
            }

            GenerateThemesMenu(TsbThemes.DropDownItems, ChartThemeManage.Default.Internals);
            TsbThemes.DropDownItems.Add(new ToolStripSeparator());
            GenerateThemesMenu(TsbThemes.DropDownItems, ChartThemeManage.Default.Extensions);
        }

        void GenerateThemesMenu(ToolStripItemCollection menus, ChartThemeFolder parentFolder)
        {
            foreach (ChartThemeFolder folder in parentFolder.Folders)
            {
                ToolStripMenuItem miFolder = new ToolStripMenuItem(folder.Name);
                if (parentFolder == ChartThemeManage.Default.Internals)
                    miFolder.Text = Lang._(folder.Name);
                miFolder.Image = Properties.Resources.folder;
                menus.Add(miFolder);

                GenerateThemesMenu(miFolder.DropDownItems, folder);
            }

            foreach (var theme in parentFolder.Themes)
            {
                var miTheme = new ToolStripMenuItem(theme.Name);
                if (theme.IsInternal)
                    miTheme.Text = Lang._(theme.Name);
                miTheme.Tag = theme;
                miTheme.Image = theme.Icon;
                miTheme.Click += new EventHandler(MenuTheme_Click);

                menus.Add(miTheme);
            }
        }
        #endregion

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

#if DEBUG
            ToolStripMenuItem miAddFreeDiagram = new ToolStripMenuItem("Add Free Diagram");
            miAddFreeDiagram.Click += new EventHandler(miAddFreeDiagram_Click);
            MenuStripChartTab.Items.Add(miAddFreeDiagram);
#endif
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (!HardClose && Document != null && Document.Modified && !ReadOnly)
            {
                DialogResult dr = this.ShowMessage("Confirm Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    Save();
                }
                else if (dr == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            if (ActiveChartBox != null && ActiveChartBox.Visible)
            {
                if(ActiveChartBox.CanFocus)
                    ActiveChartBox.Focus();
                ((Form)TopLevelControl).ActiveControl = ActiveChartBox;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Visible)
            {
                if (ShortcutKeys.Haldle(e.KeyData))
                {
                    e.SuppressKeyPress = true;
                }
            }

            base.OnKeyDown(e);
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            TsbCut.Text = Lang._("Cut");
            TsbCopy.Text = Lang._("Copy");
            TsbPaste.Text = Lang._("Paste");
            TsbDelete.Text = Lang._("Delete");

            TsbFormatPainter.Text = Lang._("Format Painter");
            TsbFormatPainter.ToolTipText = Lang._("Format Painter Description");
            TsbThemes.Text = Lang._("Themes");
            TsbSave.Text = Lang._("Save");
            TsbSave.ToolTipText = Lang._("Save", KeyMap.Save.Keys);
            TsbFullScreen.Text = FullScreen ? Lang._("Exit Full Screen") : Lang._("Full Screen");
            TsbFullScreen.ToolTipText = Lang._(TsbFullScreen.Text, KeyMap.FullScreen.Keys);
            TsbZoomIn.Text = Lang._("Zoom In");
            TsbZoomIn.ToolTipText = Lang._("Zoom In", KeyMap.ZoomIn.Keys);
            TsbZoomOut.Text = Lang._("Zoom Out");
            TsbZoomOut.ToolTipText = Lang._("Zoom Out", KeyMap.ZoomOut.Keys);
            TsbScrollMode.Text = Lang._("Scroll Mode");
            TsbSelectMode.Text = Lang._("Select Mode");
            TsbUndo.Text = Lang._("Undo");
            TsbUndo.ToolTipText = Lang._("Undo", KeyMap.Undo.Keys);
            TsbRedo.Text = Lang._("Redo");
            TsbRedo.ToolTipText = Lang._("Redo", KeyMap.Redo.Keys);
            MenuZoomFitPage.Text = Lang._("Fit Page");
            MenuZoomFitWidth.Text = Lang._("Fit Width");
            MenuZoomFitHeight.Text = Lang._("Fit Height");
            MenuThemes.Text = Lang.GetTextWithEllipsis("Themes Manage");
            MenuRefreshThemes.Text = Lang._("Refresh");
            MenuSaveThemeAs.Text = Lang.GetTextWithEllipsis("Save Current Theme");
            TsbFind.Text = Lang._("Find");
            TsbFind.ToolTipText = Lang._(Lang.GetTextWithEllipsis("Find"), KeyMap.Find.Keys);
            TsbReplace.Text = Lang._("Replace");
            TsbReplace.ToolTipText = Lang._(Lang.GetTextWithEllipsis("Replace"), KeyMap.Replace.Keys);
            TsbSidebar.Text = Lang._("Sidebar");
            TsbTimer.Text = Lang._("Timer");
            MenuStartTimer.Text = Lang.GetTextWithEllipsis("Start Timer");

            MenuInsertTab.Text = Lang._("Insert Chart");
            MenuDeleteTab.Text = Lang.GetTextWithEllipsis("Remove Chart");
            MenuRenameTab.Text = Lang.GetTextWithEllipsis("Rename");

            objectTree1.Text = Lang._("Objects");
            cob.Text = Lang._("Overview");

            //if (remarkEditor != null)
            //    remarkEditor.Text = Lang._("Remark");

            saveFileDialog1.Filter = string.Format("{0} (*{1})|*{1}",
                Lang._("Blumind Mind Map File"),
                DocumentType.Blumind.DefaultExtension);

            multiChartsView1.NotifyCurrentLanguageChanged();
            InitializeTimers();
        }

        void OnDocumentChanged(Document old)
        {
            if (old != null)
            {
                old.NameChanged -= new EventHandler(Document_NameChanged);
                old.ModifiedChanged -= new EventHandler(Document_ModifiedChanged);
            }

            if (Document != null)
            {
                Text = Path.GetFileNameWithoutExtension(Document.FileName);

                if (Document.Attributes.ContainsKey(ShowSidebarOptionName))
                    ShowSidebar = ST.GetBool(Document.Attributes[ShowSidebarOptionName], true);

                Document.NameChanged += new EventHandler(Document_NameChanged);
                Document.ModifiedChanged += new EventHandler(Document_ModifiedChanged);
            }

            multiChartsView1.Document = Document;
            ResetFormTitle();
        }

        void Document_ModifiedChanged(object sender, EventArgs e)
        {
            ResetFormTitle();
            ResetControlStatus();
        }

        void Document_NameChanged(object sender, EventArgs e)
        {
            ResetFormTitle();
        }

        void multiChartsView1_NewChartPage(object sender, EventArgs e)
        {
            InsertNewTab();
        }

        string GetNewChartName(string baseName)
        {
            if (baseName == null)
                baseName = "New Chart";
            baseName = Lang._(baseName);

            int index = 1;
            string name = string.Format("{0} {1}", baseName, index);
            while(Document.Charts.Exists(c=>StringComparer.OrdinalIgnoreCase.Equals(c.Name, name)))
            {
                index++;
                name = string.Format("{0} {1}", baseName, index);
            }

            return name;
        }

        MindMap CreateNewMap()
        {
            var map = new MindMap();
            map.Name = GetNewChartName(null);
            map.Author = System.Environment.UserName;
            map.Root.Text = Lang._("Center Topic");

            if (ChartThemeManage.Default.DefaultTheme != null)
            {
                map.ApplyTheme(ChartThemeManage.Default.DefaultTheme);
            }

            return map;
        }

        private FlowDiagram CreateNewFlowDiagram()
        {
            var fd = new FlowDiagram();
            fd.Name = GetNewChartName(null);

            if (ChartThemeManage.Default.DefaultTheme != null)
            {
                fd.ApplyTheme(ChartThemeManage.Default.DefaultTheme);
            }

            return fd;
        }

        void OnBeforeSave(Document doc)
        {
            if (doc != null)
            {
                doc.Attributes[ShowSidebarOptionName] = ShowSidebar.ToString();
            }
        }

        void ResetFormTitle()
        {
            //Text = Path.GetFileNameWithoutExtension(Document2.FileName);
            if (Document != null)
            {
                if (Document.Modified && !ReadOnly)
                    Text = string.Format("{0} *", Document.Name);
                else
                    Text = Document.Name;
            }
            else
            {
                Text = string.Empty;
            }
        }

        public override void EnterFullScreen()
        {
            if (this.FullScreen)
                return;
            ControlFullScreenShell.EnterFullScreen(this);
        }

        public override void ExitFullScreen()
        {
            if (!this.FullScreen)
                return;
            ControlFullScreenShell.ExitFullScreen(this);

            //if (!Focused && CanFocus)
            //    Focus();
        }

        protected override void OnFullScreenChanged()
        {
            base.OnFullScreenChanged();

            if (FullScreen)
            {
                TsbFullScreen.Image = Properties.Resources.full_screen_exit;
                TsbFullScreen.Text = Lang._("Exit Full Screen");
                TsbFullScreen.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            }
            else
            {
                TsbFullScreen.Image = Properties.Resources.full_screen;
                TsbFullScreen.Text = Lang._("Full Screen");
                TsbFullScreen.DisplayStyle = ToolStripItemDisplayStyle.Image;
            }

            splitContainer2.Visible = !FullScreen;
            splitter1.Visible = !FullScreen;
            TsbFullScreen.ToolTipText = Lang._(TsbFullScreen.Text, Keys.F11);
        }

        public override string GetFileName()
        {
            if (Document != null)
                return Document.FileName;
            else
                return null;
        }

        public override void ApplyTheme(UITheme theme)
        {
            if (theme != null)
            {
                BackColor = theme.Colors.Control;
                ForeColor = theme.Colors.ControlText;
                MenuStripChartTab.Renderer = theme.ToolStripRenderer;
                toolStrip1.Renderer = theme.ToolStripRenderer;

                if (myTabControl1 != null)
                {
                    myTabControl1.SelectedBackColor = theme.Colors.Window;
                    myTabControl1.SelectedForeColor = theme.Colors.WindowText;
                }

                if (tabControl2 != null)
                {
                    tabControl2.SelectedBackColor = theme.Colors.Window;
                    tabControl2.SelectedForeColor = theme.Colors.WindowText;
                }

                if (multiChartsView1 != null)
                {
                    multiChartsView1.ItemBackColor = theme.Colors.MediumDark;
                    multiChartsView1.ItemForeColor = PaintHelper.FarthestColor(multiChartsView1.ItemBackColor,
                        theme.Colors.Dark,
                        theme.Colors.Light);
                }

                foreach (var pp in PropertyBoxies)
                {
                    if (pp != null && !pp.IsDisposed)
                        pp.Font = theme.DefaultFont;
                }

                if (objectTree1 != null)
                {
                    objectTree1.Font = theme.DefaultFont;
                }
            }
        }

        void MenuZoom_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem mi = (ToolStripMenuItem)sender;
                if (mi.Tag is float && ActiveChartBox != null)
                {
                    ActiveChartBox.Zoom = (float)mi.Tag;
                }
            }
        }

        void TsbZoomOut_Click(object sender, EventArgs e)
        {
            if(ActiveChartBox != null)
                ActiveChartBox.ZoomOut();
        }

        void TsbZoomIn_Click(object sender, EventArgs e)
        {
            if (ActiveChartBox != null)
                ActiveChartBox.ZoomIn();
        }

        void MenuTimer_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem mi = (ToolStripMenuItem)sender;
                if (mi.Tag is TimeSegmentInfo)
                {
                    TimeSegmentInfo tsi = (TimeSegmentInfo)mi.Tag;
                    ShowTimer(tsi);
                }
            }
        }

        void ShowTimer(TimeSegmentInfo tsi)
        {
            if (MyTimerDialog != null && !MyTimerDialog.IsDisposed)
            {
                MyTimerDialog.Close();
            }

            if (tsi != null)
                MyTimerDialog = new TimerDialog(tsi.TimeSegment);
            else
                MyTimerDialog = new TimerDialog();
            MyTimerDialog.Owner = this;
            MyTimerDialog.StartPosition = FormStartPosition.Manual;

            Point pt = new Point((Width - MyTimerDialog.Width) / 2, ActivedChartPage.Top);
            MyTimerDialog.Location = PointToScreen(pt);
            MyTimerDialog.Show();
        }

        void objectTree1_SelectedObjectChanged(object sender, TreeViewEventArgs e)
        {
            //SelectedObjects = objectTree1.SelectedObjects;
            if (e.Action != TreeViewAction.Unknown && e.Node != null && e.Node.Tag != null)
            {
                if (e.Node.Tag is Topic)
                {
                    Topic topic = (Topic)e.Node.Tag;
                    if (ActiveChartBox != null)
                    {
                        if (objectTree1.IsNodeSelected(e.Node))
                            ActiveChartBox.Select(topic);
                        else
                            ActiveChartBox.Unselect(topic);
                    }
                }
                else if (e.Node.Tag is MindMap && objectTree1.IsNodeSelected(e.Node))
                {
                    SelectedObjects = new MindMap[] { (MindMap)e.Node.Tag };
                }
            }
        }

        void ResetControlStatus()
        {
            bool hasSelected = ActiveChartBox != null && ActiveChartBox.HasSelected();
            bool hasTopicSelected = hasSelected && ActiveChartBox.HasSelected();

            TsbSave.Enabled = CanSave;

            TsbCopy.Enabled = ActiveChartBox != null && ActiveChartBox.CanCopy;
            TsbCut.Enabled = ActiveChartBox != null && ActiveChartBox.CanCut;
            TsbPaste.Enabled = ActiveChartBox != null && ActiveChartBox.CanPaste;
            TsbDelete.Enabled = ActiveChartBox != null && ActiveChartBox.CanDelete;
            TsbFind.Enabled = ActiveChartBox != null;
            TsbReplace.Enabled = hasSelected;

            TsbSelectMode.Enabled = ActiveChartBox != null;
            TsbSelectMode.Checked = ActiveChartBox != null && ActiveChartBox.MouseMethod == ChartMouseMethod.Select;
            TsbScrollMode.Enabled = ActiveChartBox != null;
            TsbScrollMode.Checked = ActiveChartBox != null && ActiveChartBox.MouseMethod == ChartMouseMethod.Scroll;

            TsbUndo.Enabled = ActiveChartBox != null && ActiveChartBox.CanUndo;
            TsbRedo.Enabled = ActiveChartBox != null && ActiveChartBox.CanRedo;

            if(ActiveChartBox != null)
                ActiveChartBox.ResetControlStatus();
        }

        void ActivedChartPage_SelectedObjectsChanged(object sender, EventArgs e)
        {
            if (ActivedChartPage == sender)
            {
                SelectedObjects = ActivedChartPage.SelectedObjects;
            }

            ResetControlStatus();
        }

        void ActivedChartBox_CommandHistoryChanged(object sender, EventArgs e)
        {
            ResetControlStatus();
        }

        void ActivedChartBox_ModifiedChanged(object sender, EventArgs e)
        {
            if (ActiveChartBox != null && ActiveChartBox.Modified)
                Document.Modified = true;
        }

        void ActivedChartBox_ZoomChanged(object sender, EventArgs e)
        {
            if (ActiveChartBox != null)
            {
                foreach (ToolStripItem item in TsbZoom.DropDownItems)
                {
                    if (item is ToolStripMenuItem && item.Tag is float)
                    {
                        ToolStripMenuItem mi = (ToolStripMenuItem)item;
                        mi.Checked = (float)mi.Tag == ActiveChartBox.Zoom;
                    }
                }

                TsbZoom.Text = string.Format("{0:f0}%", ActiveChartBox.Zoom * 100);
            }
        }

        void ShowProperty(object obj)
        {
            ShowProperty(new object[] { obj });
        }

        void ShowProperty(object[] objects)
        {
            var selectedPropertyPage = tabControl2.SelectedPage;
            var objectType = objects.GetType().GetElementType();
            var pb = GetPropertyBox(objectType);//[0].GetType()
            if (pb != null)
            {
                if (!tabControl2.Controls.Contains(pb))// splitContainer2.Panel2.Controls.Count == 0 || splitContainer2.Panel2.Controls[0] != pb)
                {
                    if (CurrentPropertyBox != null && tabControl2.TabPages.Contains(CurrentPropertyBox))
                        tabControl2.TabPages.Remove(CurrentPropertyBox);
                    //tabControl2.Controls.Clear();
                    pb.Dock = DockStyle.Fill;
                    tabControl2.InsertPage(0, pb, Properties.Resources.property);
                }

                CurrentPropertyBox = pb;
                pb.SelectedObjects = objects;
            }

            // IRemark Object
            //if (objects != null && objects.Length == 1 && objects[0] is IRemark)
            //{
            //    if (remarkEditor == null || remarkEditor.IsDisposed)
            //    {
            //        remarkEditor = new RemarkEditor();
            //        remarkEditor.Text = Lang._("Remark");

            //        tabControl2.AddPage(remarkEditor, Properties.Resources.notes);
            //    }
            //    else if (!tabControl2.TabPages.Contains(remarkEditor))
            //    {
            //        tabControl2.AddPage(remarkEditor, Properties.Resources.notes);
            //    }

            //    remarkEditor.CurrentObject = (IRemark)objects[0];
            //    remarkEditor.ReadOnly = ReadOnly;
            //}
            //else
            //{
            //    if (remarkEditor != null && tabControl2.TabPages.Contains(remarkEditor))
            //        tabControl2.TabPages.Remove(remarkEditor);
            //}

            //
            if (selectedPropertyPage != null && tabControl2.TabPages.Contains(selectedPropertyPage) && tabControl2.SelectedPage != selectedPropertyPage)
                tabControl2.SelectedPage = selectedPropertyPage;

            //
            objectTree1.SelectedObjects = objects;
        }

        PropertyBox GetPropertyBox(Type type)
        {
            var pva = Attribute.GetCustomAttribute(type, typeof(PropertyViewAttribute)) as PropertyViewAttribute;
            if (pva == null || pva.PropertyBoxType == null)
            {
                if (_NormalPropertyBox == null)
                {
                    _NormalPropertyBox = new NormalPropertyBox();
                    _NormalPropertyBox.Font = UITheme.Default.DefaultFont;
                }
                return _NormalPropertyBox;
            }
            else
            {
                if (CurrentPropertyBox != null && CurrentPropertyBox.GetType() == pva.PropertyBoxType)
                    return CurrentPropertyBox;

                foreach (var pb in PropertyBoxies)
                {
                    if (pb.GetType() == pva.PropertyBoxType)
                        return pb;
                }

                var cif = pva.PropertyBoxType.GetConstructor(new Type[0]);
                if (cif != null)
                {
                    var pb = cif.Invoke(new object[0]) as PropertyBox;
                    pb.Font = UITheme.Default.DefaultFont;
                    PropertyBoxies.Add(pb);
                    return pb;
                }

                return null;
            }
        }

        void multiChartsView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (multiChartsView1.SelectedIndex > -1 && multiChartsView1.SelectedIndex < multiChartsView1.TabPages.Count)
            {
                ActivedChartPage = multiChartsView1.TabPages[multiChartsView1.SelectedIndex] as ChartPageView.BaseChartPage;
            }
        }

        void multiChartsView1_NeedShowProperty(object sender, NeedShowPropertyEventArgs e)
        {
            if (e.Force)
            {
                ShowSidebar = true;
                CurrentPropertyBox.Focus();
            }
        }

        void splitter1_DoubleClick(object sender, EventArgs e)
        {
            ShowSidebar = !ShowSidebar;
        }

        void TsbSidebar_Click(object sender, EventArgs e)
        {
            ShowSidebar = !ShowSidebar;
        }

        void ChangeSideBarSide()
        {
            if (splitContainer2.Dock == DockStyle.Left)
            {
                splitContainer2.Dock = DockStyle.Right;
                splitter1.Dock = DockStyle.Right;
            }
            else
            {
                splitContainer2.Dock = DockStyle.Left;
                splitter1.Dock = DockStyle.Left;
            }
        }

        void FormatPainter_DataChanged(object sender, EventArgs e)
        {
            TsbFormatPainter.Checked = !FormatPainter.Default.IsEmpty;
        }

        void Default_KeyManChanged(object sender, EventArgs e)
        {
            OnCurrentLanguageChanged();
        }

        void ShowFindDialog(FindDialog.FindDialogMode mode)
        {
            if (ActiveChartBox != null)
            {
                Program.MainForm.ShowFindDialog(this.ActiveChartBox, mode);
            }
        }

        void MenuTheme_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem && ActiveChartBox.ChartPage != null)
            {
                ToolStripMenuItem mi = (ToolStripMenuItem)sender;
                if (mi.Tag is ChartTheme)
                {
                    ActiveChartBox.ApplyChartTheme((ChartTheme)mi.Tag);
                }
            }
        }

        void TsbThemes_DropDownOpening(object sender, EventArgs e)
        {
            if (TsbThemes.DropDownItems.Count <= 4)
            {
                try
                {
                    InitializeThemes();
                }
                catch (System.Exception ex)
                {
                    Helper.WriteLog(ex);
                    this.ShowMessage(ex.Message, MessageBoxIcon.Error);
                }
            }
        }

        void RefreshThemesMenu()
        {
            try
            {
                ChartThemeManage.Default.Refresh();
                InitializeThemes();
            }
            catch (System.Exception ex)
            {
                Helper.WriteLog(ex);
                this.ShowMessage(ex.Message, MessageBoxIcon.Error);
            }
        }

        #region DocumentForm Members

        public override DocumentTypeGroup[] GetExportDocumentTypes()
        {
            return new DocumentTypeGroup[] {
                new DocumentTypeGroup("Image", new DocumentType[]{
                    DocumentType.Png, 
                    DocumentType.Jpeg, 
                    DocumentType.Bmp, 
                    DocumentType.Gif, 
                    DocumentType.Tiff,}),
                new DocumentTypeGroup("XML", new DocumentType[]{
                    DocumentType.Svg,
                    DocumentType.FreeMind}),
                new DocumentTypeGroup("Text", new DocumentType[]{
                    DocumentType.Txt,
                    DocumentType.Csv}),
                };
        }

        //public override void ExportDocument(string filename, string typeMime)
        //{
        //    if (ActiveChartBox != null)
        //    {
        //        ActiveChartBox.ExportImage(filename, typeMime);
        //    }
        //}

        public override bool Save()
        {
            if (string.IsNullOrEmpty(Document.FileName))
            {
                SaveAs();
            }
            else
            {
                OnBeforeSave(Document);
                try
                {
                    Document.Save(Document.FileName);
                    Document.Modified = false;
                    //MindMapIO.Save(Map, Map.Filename);
                    //mapView1.Modified = false;
                }
                catch (System.Exception ex)
                {
                    Helper.WriteLog(ex);
                    this.ShowMessage(ex.Message, MessageBoxIcon.Error);
                    return false;
                }

                RecentFilesManage.Default.Push(Document.FileName, Document.CreateThumbImage());
            }

            return true;
        }

        public override bool SaveAs()
        {
            if (Document == null)
                return false;

            if (string.IsNullOrEmpty(Document.FileName))
            {
                saveFileDialog1.FileName = ST.EscapeFileName(Document.Name);
            }
            else
            {
                saveFileDialog1.InitialDirectory = Path.GetDirectoryName(Document.FileName);
                saveFileDialog1.FileName = Path.GetFileName(Document.FileName);
            }

            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                string filename = saveFileDialog1.FileName;
                try
                {
                    OnBeforeSave(Document);
                    Document.Save(filename);
                    //MindMapIO.Save(Map, filename);
                    Document.Modified = false;
                }
                catch (System.Exception ex)
                {
                    Helper.WriteLog(ex);
                    this.ShowMessage(ex.Message, MessageBoxIcon.Error);
                    return false;
                }

                RecentFilesManage.Default.Push(filename, Document.CreateThumbImage());
            }

            return true;
        }

        public override bool Print(PageSettings pageSettings)
        {
            var charts = Document.Charts.ToArray();

            if (charts.Length > 1)
            {
                var dialog = new SelectChartDialog(Document);
                dialog.Text = Lang._("Select Charts To Print...");
                if (dialog.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
                    return false;
                if (dialog.SelectedCharts.IsNullOrEmpty())
                    return false;
                charts = dialog.SelectedCharts;
            }

            if (charts.Length == 0)
                return false;

            if (MyPrintDialog == null)
            {
                MyPrintDialog = new PrintDialog();
                MyPrintDialog.UseEXDialog = true; // workaround 64bit PrintDialog bug
            }

            MyPrintDialog.Document = Document.Print(charts);// ActiveChartBox.ReadyPrint();
            if (pageSettings != null)
                MyPrintDialog.Document.DefaultPageSettings = pageSettings;
            if (MyPrintDialog.ShowDialog(Program.MainForm) == DialogResult.OK)
            {
                MyPrintDialog.Document.Print();
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool PrintPreview()
        {
            var ppd = new PrintPreviewDialog();
            ppd.WindowState = FormWindowState.Maximized;
            ppd.PrintDocument = Document.Print();// ActiveChartBox.ReadyPrint();
            ppd.Print += PrintPreviewDialog_Print;
            ppd.ShowDialog(this);

            return true;
        }

        void PrintPreviewDialog_Print(object sender, EventArgs e)
        {
            PageSettings pageSettings = null;
            if (sender is PrintPreviewDialog)
            {
                pageSettings = ((PrintPreviewDialog)sender).PageSettings;
            }

            Print(pageSettings);
        }

        #endregion

        void TsbSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        void TsbUndo_Click(object sender, EventArgs e)
        {
            if (ActiveChartBox != null && ActiveChartBox.CanUndo)
            {
                ActiveChartBox.Undo();
            }
        }

        void TsbRedo_Click(object sender, EventArgs e)
        {
            if (ActiveChartBox != null && ActiveChartBox.CanRedo)
            {
                ActiveChartBox.Redo();
            }
        }

        void TsbFormatPainter_Click(object sender, EventArgs e)
        {
            long tick = DateTime.Now.Ticks / 10000;
            //System.Diagnostics.Debug.WriteLine(tick - Last_TsbFormatPainter_ClickTick);
            if (tick - Last_TsbFormatPainter_ClickTick < SystemInformation.DoubleClickTime && !FormatPainter.Default.IsEmpty)
                FormatPainter.Default.HoldOn = true;
            else if(ActiveChartBox != null)
                ActiveChartBox.CopyStyle(false);//Helper.TestModifierKeys(Keys.Control));

            Last_TsbFormatPainter_ClickTick = tick;
        }

        //private void MenuAddHyperlink_Click(object sender, EventArgs e)
        //{
        //    mapView1.AddHyperlink();
        //}

        public override void AskSave(ref bool cancel)
        {
            if (Document != null && Document.Modified && !ReadOnly)
            {
                DialogResult dr = this.ShowMessage("Confirm Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    Save();
                }
                else if (dr == DialogResult.Cancel)
                {
                    cancel = true;
                    return;
                }
                else
                {
                    HardClose = true;
                    Close();
                }
            }

            base.AskSave(ref cancel);
        }

        void MenuRefreshThemes_Click(object sender, EventArgs e)
        {
            RefreshThemesMenu();
        }

        void splitter1_DoubleClick_1(object sender, EventArgs e)
        {
            ShowSidebar = !ShowSidebar;
        }

        void MenuThemes_Click(object sender, EventArgs e)
        {
            ThemesDialog dlg = new ThemesDialog();
            dlg.ThemeApply += new EventHandler(ThemesDialog_ApplyTheme);
            dlg.ShowDialog(this);
        }

        void ThemesDialog_ApplyTheme(object sender, EventArgs e)
        {
            if (sender is ThemesDialog && ActiveChartBox != null)
            {
                ThemesDialog dlg = (ThemesDialog)sender;
                if (dlg.CurrentTheme != null)
                {
                    ActiveChartBox.ApplyChartTheme(dlg.CurrentTheme);
                }
            }
        }

        void splitter1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            //panel1.GlobalBackground.Update();
        }

        void TsbFullScreen_Click(object sender, EventArgs e)
        {
            FullScreen = !FullScreen;
        }

        void TsbFind_Click(object sender, EventArgs e)
        {
            ShowFindDialog(FindDialog.FindDialogMode.Find);
        }

        void TsbReplace_Click(object sender, EventArgs e)
        {
            ShowFindDialog(FindDialog.FindDialogMode.Replace);
        }

        void MenuSaveThemeAs_Click(object sender, EventArgs e)
        {
            if (ActiveChartBox != null && ActiveChartBox.ChartPage != null)
            {
                try
                {
                    SaveTheme(ActiveChartBox.ChartPage);
                    RefreshThemesMenu();
                }
                catch (System.Exception ex)
                {
                    Helper.WriteLog(ex);
                    this.ShowMessage(ex.Message, MessageBoxIcon.Error);
                }
            }
        }

        void SaveTheme(ChartPage chartPage)
        {
            InputDialog dlg = new InputDialog(Lang._("Save Theme"), Lang._("Please enter theme name"));
            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                ChartTheme theme = chartPage.GetChartTheme();
                theme.Name = dlg.Value;
                theme.Filename = ChartThemeManage.GetThemeFilename(theme.Name);
                if (File.Exists(theme.Filename))
                {
                    var dr = this.ShowMessage(Lang._("The same filename already exists. Do you want to replace it?"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (dr == System.Windows.Forms.DialogResult.Cancel)
                        return;

                    if (dr == System.Windows.Forms.DialogResult.No)
                    {
                        while (true)
                        {
                            dlg.Value = theme.Name;
                            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.Cancel)
                            {
                                return;
                            }

                            theme.Name = dlg.Value;
                            theme.Filename = ChartThemeManage.GetThemeFilename(theme.Name);
                            if (!File.Exists(theme.Filename))
                                break;
                        }
                    }
                }

                ChartThemeManage.Default.Extensions.Themes.Add(theme);
                ChartThemeManage.Default.SaveTheme(theme, theme.Filename);
            }
        }

        void MenuZoomFitPage_Click(object sender, EventArgs e)
        {
            if(ActiveChartBox != null)
                ActiveChartBox.ZoomAs(ZoomType.FitPage);
        }

        void MenuZoomFitWidth_Click(object sender, EventArgs e)
        {
            if (ActiveChartBox != null)
                ActiveChartBox.ZoomAs(ZoomType.FitWidth);
        }

        void MenuZoomFitHeight_Click(object sender, EventArgs e)
        {
            if (ActiveChartBox != null)
                ActiveChartBox.ZoomAs(ZoomType.FitHeight);
        }

        void TsbSidebar_Click_1(object sender, EventArgs e)
        {
            ShowSidebar = !ShowSidebar;
        }

        void ActivedChartBox_MouseMethodChanged(object sender, EventArgs e)
        {
            ResetControlStatus();
        }

        void TsbSelectMode_Click(object sender, EventArgs e)
        {
            if (ActiveChartBox != null)
                ActiveChartBox.MouseMethod = ChartMouseMethod.Select;
        }

        void TsbScrollMode_Click(object sender, EventArgs e)
        {
            if (ActiveChartBox != null)
                ActiveChartBox.MouseMethod = ChartMouseMethod.Scroll;
        }

        void MenuStartTimer_Click(object sender, EventArgs e)
        {
            ShowTimer(null);
        }

        void MenuRenameTab_Click(object sender, EventArgs e)
        {
            if (ActivedChartPage != null && ActivedChartPage.Chart != null)
            {
                InputDialog dlg = new InputDialog(Lang._("Rename"), Lang._("Please enter the new name"));
                dlg.Value = ActivedChartPage.Chart.Name;
                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    ActivedChartPage.Chart.Name = dlg.Value;
                }
            }
        }

        void MenuInsertTab_Click(object sender, EventArgs e)
        {
            InsertNewTab();
        }

        void miAddFreeDiagram_Click(object sender, EventArgs e)
        {
            if (Document != null)
            {
                var map = CreateNewFlowDiagram();
                Document.Charts.Add(map);

                ActiveChart(map);
            }
        }

        void MenuDeleteTab_Click(object sender, EventArgs e)
        {
            if (ActivedChartPage != null)
            {
                ChartPage doc = ActivedChartPage.Chart;
                string msg = Lang.Format("Are you sure delete the tab \"{0}\"?", doc.Name);
                if (this.ShowMessage(msg, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                    return;

                DeleteActivedTab();
            }
        }

        void DeleteActivedTab()
        {
            if (ActivedChartPage != null && ActivedChartPage.Chart != null)
            {
                Document.Charts.Remove(ActivedChartPage.Chart);
                if (Document.Charts.Count == 0)
                {
                    InsertNewTab();
                }
            }
        }

        void InsertNewTab()
        {
            if (Document != null)
            {
                var map = CreateNewMap();
                Document.Charts.Add(map);

                ActiveChart(map);
            }
        }

        void TsbDelete_Click(object sender, EventArgs e)
        {
            if(ActiveChartBox != null)
                ActiveChartBox.DeleteObject();
        }

        void TsbPaste_Click(object sender, EventArgs e)
        {
            if (ActiveChartBox != null && ActiveChartBox.CanPaste)
            {
                ActiveChartBox.Paste();
            }
        }

        void TsbCopy_Click(object sender, EventArgs e)
        {
            if (ActiveChartBox != null && ActiveChartBox.CanCopy)
            {
                ActiveChartBox.Copy();
            }
        }

        void TsbCut_Click(object sender, EventArgs e)
        {
            if (ActiveChartBox != null && ActiveChartBox.CanCut)
            {
                ActiveChartBox.Cut();
            }
        }

        void ResetToolStripItems()
        {
            if (CurrentChartBoxItems != null && CurrentChartBoxItems.Count > 0)
            {
                foreach (var item in CurrentChartBoxItems)
                    toolStrip1.Items.Remove(item);
            }

            if (ActiveChartBox != null)
            {
                var toolStripItems = ActiveChartBox.GetToolStripItems();
                if (toolStripItems != null)
                {
                    int index = 17;
                    foreach (var item in toolStripItems)
                    {
                        item.Visible = true;
                        toolStrip1.Items.Insert(index++, item);
                        CurrentChartBoxItems.Add(item);
                    }
                }
            }
        }

        void ActiveChart(ChartPage chartPage)
        {
            multiChartsView1.ActiveChartPage(chartPage);
        }
    }
}
