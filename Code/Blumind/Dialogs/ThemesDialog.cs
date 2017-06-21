using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Controls.MapViews;
using Blumind.Core;
using Blumind.Dialogs.Components;
using Blumind.Globalization;
using Blumind.Model;
using Blumind.Model.MindMaps;
using Blumind.Model.Styles;

namespace Blumind
{
    partial class ThemesDialog : StandardDialog
    {
        ChartTheme _CurrentTheme;
        List<ChartTheme> DeleteThemes;
        bool _ShowApplyThemeButton = true;

        MindMapView mindMapView1;
        PropertyControl myPropertyGrid1;
        Panel PanelRemark;
        TextBox TxbRemark;
        ChartTheme _DefaultTheme;

        public event System.EventHandler ThemeApply;

        public ThemesDialog()
        {
            InitializeComponent();

            InitializeControls();

            InitializePreviewMap();
            imageList1.Images.Add(Properties.Resources.folder);
            DeleteThemes = new List<ChartTheme>();
            InitializeThemes();

            //
            var stateImageList = new ImageList();
            stateImageList.ColorDepth = ColorDepth.Depth32Bit;
            stateImageList.ImageSize = new System.Drawing.Size(16, 16);
            stateImageList.Images.Add(Properties.Resources._blank);
            stateImageList.Images.Add(Properties.Resources.tick);
            treeView1.StateImageList = stateImageList;

            AfterInitialize();
        }

        [Browsable(false)]
        public ChartTheme CurrentTheme
        {
            get { return _CurrentTheme; }
            private set
            {
                if (_CurrentTheme != value)
                {
                    _CurrentTheme = value;
                    OnCurrentThemeChanged();
                }
            }
        }

        [DefaultValue(true)]
        public bool ShowApplyThemeButton
        {
            get { return _ShowApplyThemeButton; }
            set
            {
                if (_ShowApplyThemeButton != value)
                {
                    _ShowApplyThemeButton = value;
                    OnShowApplyThemeButtonChanged();
                }
            }
        }

        ChartTheme DefaultTheme
        {
            get 
            {
                if (_DefaultTheme == null)
                    return ChartThemeManage.Default.DefaultTheme;
                else
                    return _DefaultTheme; 
            }
            set 
            {
                if (_DefaultTheme != value)
                {
                    var old = _DefaultTheme;
                    _DefaultTheme = value;
                    OnDefaultThemeChanged(old);
                }
            }
        }

        void InitializeControls()
        {
            // mindMapView1
            mindMapView1 = new MindMapView();
            mindMapView1.Dock = DockStyle.Fill;
            mindMapView1.ShowNavigationMap = true;
            mindMapView1.Text = Lang._("Preview");
            mindMapView1.Padding = new Padding(0);
            mindMapView1.ShowBorder = false;

            // myPropertyGrid1
            myPropertyGrid1 = new PropertyControl();
            myPropertyGrid1.Dock = DockStyle.Fill;
            myPropertyGrid1.ShowBorder = false;
            myPropertyGrid1.HelpVisible = false;
            myPropertyGrid1.ToolbarVisible = false;
            myPropertyGrid1.Text = Lang._("Property");

            // TxbRemark
            TxbRemark = new TextBox();
            TxbRemark.Dock = DockStyle.Fill;
            TxbRemark.Multiline = true;
            TxbRemark.ScrollBars = ScrollBars.Both;
            TxbRemark.BorderStyle = BorderStyle.None;
            TxbRemark.BackColor = SystemColors.Window;
            TxbRemark.ForeColor = SystemColors.WindowText;
            TxbRemark.TextChanged += new EventHandler(this.TxbRemark_TextChanged);

            // PanelRemark
            PanelRemark = new Panel();
            PanelRemark.Dock = DockStyle.Fill;
            PanelRemark.Controls.Add(TxbRemark);
            PanelRemark.Text = Lang._("Notes");

            tabControl1.AddPage(mindMapView1);
            tabControl1.AddPage(myPropertyGrid1);
            tabControl1.AddPage(PanelRemark);
            tabControl1.SelectedIndex = 0;
        }

        void InitializePreviewMap()
        {
            MindMap map = new MindMap();
            map.Margin = Padding.Empty;
            map.Root.Text = LanguageManage.GetText("Center Topic");
            map.Root.Children.Add(new Topic("Topic 1"));
            map.Root.Children.Add(new Topic("Topic 2"));
            map.Root.Children.Add(new Topic("Topic 3"));

            mindMapView1.Map = map;
        }

        void InitializeThemes()
        {
            treeView1.Nodes.Clear();
            GenerateTree(treeView1.Nodes, ChartThemeManage.Default.Internals);
            GenerateTree(treeView1.Nodes, ChartThemeManage.Default.Extensions);
            foreach (TreeNode node in treeView1.Nodes)
            {
                node.Expand();
            }

            TreeNode nodeDefault = FindTreeNode(ChartThemeManage.Default.DefaultTheme);
            if (nodeDefault != null)
            {
                treeView1.SelectedNode = nodeDefault;
            }
        }

        void GenerateTree(TreeNodeCollection nodes, ChartThemeFolder folder)
        {
            ThemeFolderNode nodeFolder = new ThemeFolderNode(folder);
            nodeFolder.Text = Lang._(folder.Name);
            nodeFolder.ImageIndex = 0;
            nodeFolder.SelectedImageIndex = 0;
            nodes.Add(nodeFolder);

            foreach (var subFolder in folder.Folders)
            {
                GenerateTree(nodeFolder.Nodes, subFolder);
            }

            foreach (var theme in folder.Themes)
            {
                var nodeTheme = new ThemeNode(theme);
                if (theme.IsInternal)
                    nodeTheme.Text = Lang._(theme.Name);
                nodeTheme.ImageIndex = nodeTheme.SelectedImageIndex = GetImageIndex(theme.Icon);
                nodeTheme.StateImageIndex = IsDefaultTheme(theme) ? 1 : 0;
                nodeFolder.Nodes.Add(nodeTheme);
            }
        }

        ThemeNode CreateThemeNode(ChartTheme theme)
        {
            ThemeNode node = new ThemeNode(theme);
            node.ImageIndex = node.SelectedImageIndex = GetImageIndex(theme.Icon);
            node.StateImageIndex = IsDefaultTheme(theme) ? 1 : 0;
           // node.Text = theme.Name;

            return node;
        }

        bool IsDefaultTheme(ChartTheme theme)
        {
            return DefaultTheme == theme;
        }

        //int GetImageIndex(ChartTheme theme)
        //{
        //    if (theme == null || theme.Icon == null)
        //        return -1;

        //    var image = theme.Icon;
        //    if (image != null && IsDefaultTheme(theme))
        //    {
        //        image = PaintHelper.CopyImage(image);
        //        PaintHelper.SynthesisImage(image, Properties.Resources.tick_small, ContentAlignment.BottomLeft);
        //    }
        //    return GetImageIndex(image);
        //}

        int GetImageIndex(Image image)
        {
            imageList1.Images.Add(image);
            return imageList1.Images.Count - 1;
        }

        void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node is ThemeNode)
            {
                CurrentTheme = ((ThemeNode)e.Node).Theme;
            }
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            Text = Lang._("Themes Manage");
            mindMapView1.Text = Lang._("Preview");
            myPropertyGrid1.Text = Lang._("Property");
            PanelRemark.Text = Lang._("Notes");
            LalName.Text = Lang._("Name");
            BtnApply.Text = Lang._("Apply Theme");

            TsbDelete.Text = Lang._("Delete");
            TsbCopy.Text = Lang._("Copy");
            TsbNew.Text = Lang._("New");
            TsbRefresh.Text = Lang._("Refresh");

            CkbDefaultTheme.Text = Lang._("Is Default");
        }

        void OnCurrentThemeChanged()
        {
            if (CurrentTheme != null)
            {
                mindMapView1.ApplyChartTheme(CurrentTheme);
                myPropertyGrid1.SelectedObject = CurrentTheme;
                TxbThemeName.Text = CurrentTheme.IsInternal ? Lang._(CurrentTheme.Name) : CurrentTheme.Name;
                TxbThemeName.ReadOnly = CurrentTheme.IsInternal;
                TsbCopy.Enabled = true;
                TsbDelete.Enabled = !CurrentTheme.IsInternal;
                TxbRemark.Text = CurrentTheme.Description;
                TxbRemark.ReadOnly = CurrentTheme.IsInternal;
                CkbDefaultTheme.Checked = IsDefaultTheme(CurrentTheme);
            }
            else
            {
                TxbThemeName.Text = string.Empty;
                TxbThemeName.ReadOnly = true;
                myPropertyGrid1.SelectedObject = null;
                TsbCopy.Enabled = false;
                TsbDelete.Enabled = false;
                TxbRemark.Text = string.Empty;
                TxbRemark.ReadOnly = true;
                CkbDefaultTheme.Checked = false;
            }

            ThemeNode node = FindTreeNode(CurrentTheme);
            if (node != null)
            {
                treeView1.SelectedNode = node;
            }
        }

        void OnShowApplyThemeButtonChanged()
        {
            BtnApply.Visible = ShowApplyThemeButton;
        }

        void TsbNew_Click(object sender, System.EventArgs e)
        {
            AddNewTheme(new ChartTheme("New Theme"));
        }

        void TsbDelete_Click(object sender, System.EventArgs e)
        {
            if (CurrentTheme != null && !CurrentTheme.IsInternal)
            {
                DeleteTheme(CurrentTheme);
            }
        }

        void TsbCopy_Click(object sender, System.EventArgs e)
        {
            if (CurrentTheme != null)
            {
                CopyTheme(CurrentTheme);
            }
        }

        void TsbRefresh_Click(object sender, EventArgs e)
        {
            RefreshThemes();
        }

        void CopyTheme(ChartTheme sourceTheme)
        {
            ChartTheme theme = new ChartTheme(sourceTheme);
            theme.SetIsInternal(false);
            if (sourceTheme.IsInternal)
                theme.Name = string.Format("{0} {1}", Lang._("Copy of"), Lang._(sourceTheme.Name));
            else
                theme.Name = string.Format("{0} {1}", Lang._("Copy of"), sourceTheme.Name);
            AddNewTheme(theme);
        }

        void AddNewTheme(ChartTheme theme)
        {
            ChartThemeManage.Default.Extensions.Themes.Add(theme);
            ThemeFolderNode node = FindTreeNode(ChartThemeManage.Default.Extensions);
            if (node != null)
            {
                node.Nodes.Add(CreateThemeNode(theme));
            }

            CurrentTheme = theme;

            if (tabControl1.SelectedIndex != 1)
                tabControl1.SelectedIndex = 1;

            if (TxbThemeName.CanFocus && !TxbThemeName.Focused)
                TxbThemeName.Focus();
        }

        void DeleteTheme(ChartTheme theme)
        {
            if (theme == null)
                throw new ArgumentNullException();

            ThemeNode node = FindTreeNode(theme);
            if (node != null)
            {
                node.Remove();
                DeleteThemes.Add(theme);
            }
        }

        void UpdateThemes()
        {
            try
            {
                if (_DefaultTheme != null && DefaultTheme != ChartThemeManage.Default.DefaultTheme && !DeleteThemes.Contains(DefaultTheme))
                {
                    ChartThemeManage.Default.DefaultTheme = DefaultTheme;
                }

                foreach (var theme in DeleteThemes)
                {
                    ChartThemeManage.Delete(theme);
                }

                ChartThemeManage.Default.Save();
            }
            catch (Exception ex)
            {
                Helper.WriteLog(ex);
                this.ShowMessage(ex.Message, MessageBoxIcon.Error);
            }
        }

        void RefreshThemes()
        {
            try
            {
                ChartThemeManage.Default.Refresh();
                InitializeThemes();
            }
            catch (Exception ex)
            {
                Helper.WriteLog(ex);
                this.ShowMessage(ex.Message, MessageBoxIcon.Error);
            }
        }

        ThemeNode FindTreeNode(ChartTheme theme)
        {
            return FindTreeNode(treeView1.Nodes, theme);
        }

        ThemeNode FindTreeNode(TreeNodeCollection nodes, ChartTheme theme)
        {
            foreach (TreeNode node in nodes)
            {
                if (node is ThemeNode && ((ThemeNode)node).Theme == theme)
                    return (ThemeNode)node;

                ThemeNode sn = FindTreeNode(node.Nodes, theme);
                if (sn != null)
                    return sn;
            }

            return null;
        }

        ThemeFolderNode FindTreeNode(ChartThemeFolder themeFolder)
        {
            return FindTreeNode(treeView1.Nodes, themeFolder);
        }

        ThemeFolderNode FindTreeNode(TreeNodeCollection nodes, ChartThemeFolder themeFolder)
        {
            foreach (TreeNode node in nodes)
            {
                if (node is ThemeFolderNode && ((ThemeFolderNode)node).ThemeFolder == themeFolder)
                    return (ThemeFolderNode)node;

                ThemeFolderNode sn = FindTreeNode(node.Nodes, themeFolder);
                if (sn != null)
                    return sn;
            }

            return null;
        }

        void TxbThemeName_TextChanged(object sender, System.EventArgs e)
        {
            if (CurrentTheme != null && !CurrentTheme.IsInternal)
            {
                CurrentTheme.Name = TxbThemeName.Text;
            }
        }

        void TxbRemark_TextChanged(object sender, EventArgs e)
        {
            if (CurrentTheme != null && !CurrentTheme.IsInternal)
            {
                CurrentTheme.Description = TxbRemark.Text;
            }
        }

        void CkbDefaultTheme_CheckedChanged(object sender, EventArgs e)
        {
            if (CurrentTheme != null)
            {
                if (CkbDefaultTheme.Checked)
                {
                    DefaultTheme = CurrentTheme;
                }
                else if (DefaultTheme == CurrentTheme)
                {
                    DefaultTheme = null;
                }
            }
        }

        void OnDefaultThemeChanged(ChartTheme old)
        {
            if (old != null)
            {
                var n = FindTreeNode(old);
                if (n != null)
                {
                    n.StateImageIndex = 0;
                }
            }

            if (DefaultTheme != null)
            {
                var n = FindTreeNode(DefaultTheme);
                if (n != null)
                {
                    n.StateImageIndex = 1;
                }
            }
        }

        void BtnApply_Click(object sender, EventArgs e)
        {
            OnApplyThemeButtonClick();
        }

        protected override bool OnOKButtonClick()
        {
            try
            {
                UpdateThemes();
            }
            catch (Exception ex)
            {
                Helper.WriteLog(ex);
                this.ShowMessage(ex.Message, MessageBoxIcon.Error);
                return false;
            }

            return base.OnOKButtonClick();
        }

        protected void OnApplyThemeButtonClick()
        {
            if (ThemeApply != null)
            {
                ThemeApply(this, EventArgs.Empty);
            }
        }

        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);

            if (toolStrip1 != null)
            {
                toolStrip1.Renderer = theme.ToolStripRenderer;
            }

            this.SetFontNotScale(theme.DefaultFont);
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (splitContainer1 != null)
            {
                var rect = this.ControlsRectangle;
                rect.Y += toolStrip1.Height;
                rect.Height -= toolStrip1.Height;
                splitContainer1.Bounds = rect;
            }

            LocateButtonsLeft(new Button[] { BtnApply });
        }
    }
}
