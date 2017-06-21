using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using Blumind.Configuration;
using Blumind.Controls;
using Blumind.Controls.OS;
using Blumind.Core;
using Blumind.Core.Exports;
using Blumind.Dialogs;
using Blumind.Globalization;
using Blumind.Model.Documents;
using Blumind.Model.MindMaps;
using Blumind.Model.Styles;
using Blumind.Core.Win32Apis;

namespace Blumind
{
    partial class MainForm : DocumentManageForm, IThemableUI
    {
        StartMenuButton BtnStart;
        SpecialTabItem TabNew;
        TabBarButton BtnOpen;
        TabBarButton BtnHelp;
        AboutDialogBox AboutDialog;
        FindDialog MyFindDialog;
        ShortcutKeysMapDialog ShortcutsMapDialog;
        CheckUpdate CheckUpdateForm;
        ToolStripMenuItem MenuClearRecentFiles;
        ShortcutKeysTable ShortcutKeys;
        bool ImportMenusHasBuilded;
        StartPage startPage;

        public MainForm()
        {
            InitializeComponent();
            MdiClient = mdiWorkSpace1;
            MinimumSize = new Size(600, 400);
            UITheme.Default.Listeners.Add(this);

            RecentFilesManage.Default.FilesChanged += new EventHandler(Options_RecentFilesChanged);

            InitializeTaskBar();
            InitializeShortcutKeys();

            //
            startPage = new StartPage();
            ShowForm(startPage, false, false);

            AfterInitialize();

            // open saved tabs
            if (Options.Current.GetValue<SaveTabsType>(OptionNames.Miscellaneous.SaveTabs) != SaveTabsType.No)
                OpenSavedTabs();
        }

        public MainForm(string[] args)
            : this()
        {
            var files = args.Where(arg => !arg.StartsWith("-")).ToArray();
            OpenDocuments(files);
        }

        void InitializeTaskBar()
        {
            TaskBar = taskBar1;
            TaskBar.Font = SystemFonts.MenuFont;
            TaskBar.Height = Math.Max(32, TaskBar.Height);
            TaskBar.MaxItemSize = 300;
            //TaskBar.Padding = new Padding(2, 0, 2, 0);

            BtnStart = new StartMenuButton();
            BtnStart.Text = "Menu";
            BtnStart.Click += new EventHandler(BtnStart_Click);

            //BtnNew = new TabBarButton();
            //BtnNew.Icon = Properties.Resources._new;
            //BtnNew.ToolTipText = "Create New Document";
            //BtnNew.Click += new EventHandler(MenuNew_Click);

            BtnOpen = new TabBarButton();
            BtnOpen.Icon = Properties.Resources.open;
            BtnOpen.ToolTipText = "Open Document...";
            BtnOpen.Click += new EventHandler(MenuOpen_Click);

            BtnHelp = new TabBarButton();
            BtnHelp.Icon = Properties.Resources.help;
            BtnHelp.Text = "Help";
            BtnHelp.Click += new EventHandler(BtnHelp_Click);

            TaskBar.LeftButtons.Add(BtnStart);
            //TaskBar.LeftButtons.Add(BtnNew);
            TaskBar.LeftButtons.Add(BtnOpen);
            TaskBar.RightButtons.Add(BtnHelp);
            TaskBar.Items.ItemAdded += TaskBar_Items_ItemAdded;
            TaskBar.Items.ItemRemoved += TaskBar_Items_ItemRemoved;

            MenuHelps.DropDown = HelpMenu;
            MenuQuickHelp.Enabled = Helper.HasQuickHelp();

            //
            TabNew = new SpecialTabItem(Properties.Resources._new);
            TabNew.Click +=new EventHandler(MenuNew_Click);
            TaskBar.RightSpecialTabs.Add(TabNew);
        }

        void InitializeWindowStates()
        {
            if (Options.Current.GetValue(OptionNames.Customizations.MainWindowMaximized, true))
                WindowState = FormWindowState.Maximized;
            else
                WindowState = FormWindowState.Normal;

            if (Options.Current.Contains(OptionNames.Customizations.MainWindowSize))
            {
                Size = Options.Current.GetValue(OptionNames.Customizations.MainWindowSize, Size);
                SetAGoodLocation();
            }
        }

        void InitializeShortcutKeys()
        {
            KeyMap.Default.KeyManChanged += new EventHandler(Default_KeyManChanged);
            Default_KeyManChanged(null, EventArgs.Empty);

            ShortcutKeys = new ShortcutKeysTable();
            ShortcutKeys.Register(KeyMap.New, delegate() { NewDocument(); });
            ShortcutKeys.Register(KeyMap.Open, delegate() { OpenDocument(); });
            ShortcutKeys.Register(KeyMap.NextTab, delegate() { taskBar1.SelectNextTab(false); });
            ShortcutKeys.Register(KeyMap.PreviousTab, delegate() { taskBar1.SelectNextTab(true); });
        }

        #region Documents Operator

        public void NewDocument()
        {
            Document doc = CreateNewMap();

            DocumentForm form = new DocumentForm(doc);
            ShowForm(form);
        }

        public void OpenDocument()
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                OpenDocument(openFileDialog1.FileName, openFileDialog1.ReadOnlyChecked);
            }
        }

        public void OpenDocument(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return;

            if (!File.Exists(filename))
            {
                this.ShowMessage(string.Format(Lang._("File \"{0}\" Not Exists"), filename), MessageBoxIcon.Error);
                return;
            }

            if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
            {
                FileInfo fif = new FileInfo(filename);
                OpenDocument(filename, fif.IsReadOnly);
            }
        }

        public void OpenDocument(string filename, bool readOnly)
        {
            BaseDocumentForm form = FindDocumentForm(filename);
            if (form != null)
            {
                SelectForm(form);
            }
            else
            {
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    Document doc = null;
                    string ext = Path.GetExtension(filename);
                    if (ext.ToLower() == ".mm")
                        doc = FreeMindFile.LoadFile(filename);
                    else
                        doc = Document.Load(filename);

                    if (doc != null)
                    {
                        form = OpenDocument(doc, readOnly);
                        if(form != null)
                            form.Filename = filename;
                    }

                    RecentFilesManage.Default.Push(filename);
                    Cursor.Current = Cursors.Default;
                }
                catch (System.Exception ex)
                {
                    Cursor.Current = Cursors.Default;
                    Helper.WriteLog(ex);
                    this.ShowMessage("File name is invalid or the format is not supported", MessageBoxIcon.Error);
                }
            }
        }

        public BaseDocumentForm OpenDocument(Document doc, bool readOnly)
        {
            if (doc != null)
            {
                BaseDocumentForm form = new DocumentForm(doc);
                form.ReadOnly = readOnly;
                ShowForm(form);
                return form;
            }

            return null;
        }

        void OpenDocuments(string[] filenames)
        {
            if (filenames != null)
            {
                for (int i = 0; i < filenames.Length; i++)
                {
                    if (string.IsNullOrEmpty(filenames[i]))
                        continue;
                    OpenDocument(filenames[i]);
                }
            }
        }

        BaseDocumentForm FindDocumentForm(string filename)
        {
            foreach (Form form in Forms)
            {
                if (form is BaseDocumentForm && StringComparer.OrdinalIgnoreCase.Equals(((BaseDocumentForm)form).Filename, filename))
                {
                    return (BaseDocumentForm)form;
                }
            }

            return null;
        }

        Document CreateNewMap()
        {
            MindMap map = new MindMap();
            map.Name = string.Format("{0} 1", Lang._("New Chart"));
            map.Root.Text = Lang._("Center Topic");
            map.Author = System.Environment.UserName;

            if (ChartThemeManage.Default.DefaultTheme != null)
            {
                map.ApplyTheme(ChartThemeManage.Default.DefaultTheme);
            }

            Document doc = new Document();
            doc.Name = Lang._("New Document");
            doc.Author = System.Environment.UserName;
            doc.Charts.Add(map);
            //doc.Modified = true;
            return doc;
        }

        #endregion

        void MenuExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        void MenuOpen_Click(object sender, EventArgs e)
        {
            OpenDocument();
        }

        void MenuNew_Click(object sender, System.EventArgs e)
        {
            NewDocument();
        }

        void MenuSaveAs_Click(object sender, EventArgs e)
        {
            if (SelectedDocumentForm != null)
            {
                SelectedDocumentForm.SaveAs();
            }
        }

        void MenuSave_Click(object sender, EventArgs e)
        {
            if (SelectedDocumentForm != null)
            {
                SelectedDocumentForm.Save();
            }
        }

        void MenuPreview_Click(object sender, EventArgs e)
        {
            if (SelectedDocumentForm != null)
            {
                SelectedDocumentForm.PrintPreview();
            }
        }

        void MenuPrint_Click(object sender, EventArgs e)
        {
            if (SelectedDocumentForm != null)
            {
                SelectedDocumentForm.Print();
            }
        }

        void MenuAbout_Click(object sender, EventArgs e)
        {
            if (AboutDialog == null)
            {
                AboutDialog = new AboutDialogBox();
            }

            AboutDialog.ShowDialog(this);
        }

        void MenuOptions_Click(object sender, EventArgs e)
        {
            //if (OptionsDialog == null)
            //{
            //    OptionsDialog = new OptionsDialog();
            //}

            //OptionsDialog.ShowDialog(this);
            ShowOptionsDialog();
        }

        public void ShowOptionsDialog()
        {
            var dialog = new Blumind.Configuration.Dialog.SettingDialog();
            dialog.ShowDialog(this);
        }

        void MenuRecentFile_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem mi = (ToolStripMenuItem)sender;
                string filename = mi.Tag as string;
                if (File.Exists(filename))
                {
                    OpenDocument(filename);
                }
                else
                {
                    MenuRecentFiles.DropDownItems.Remove(mi);
                    RecentFilesManage.Default.Remove(filename);
                    MenuRecentFiles.Enabled = RecentFilesManage.Default.Count > 0;
                }
            }
        }

        void MenuClearRecentFiles_Click(object sender, EventArgs e)
        {
            if (this.ShowMessage("Are you sure clear the recent files?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                RecentFilesManage.Default.Clear();
                ListRecentFiles();
            }
        }

        void BtnHelp_Click(object sender, EventArgs e)
        {
            //MenuHelps.DropDown.Show(TaskBar, BtnHelp.Bounds.X, BtnHelp.Bounds.Bottom);
            BtnHelp.ShowMenu(MenuHelps.DropDown);
        }

        void Options_RecentFilesChanged(object sender, EventArgs e)
        {
            ListRecentFiles();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Shell32.DragAcceptFiles(this.Handle, true);

            //if (AeroGlassCompositionEnabled)
            //{
            //    //ExcludeControlFromAeroGlass(mdiWorkSpace1);
            //    ResetAeroGlass();
            //}

            //
            taskBar1.AeroBackground = Blumind.Controls.Aero.GlassForm.AeroGlassCompositionEnabled;
            taskBar1.Font = UITheme.Default.DefaultFont;
            StartMenu.Renderer = UITheme.Default.ToolStripRenderer;
            HelpMenu.Renderer = UITheme.Default.ToolStripRenderer;

            if (RecentFilesManage.Default.TrySave())
                ListRecentFiles();

            if (Forms.IsEmpty())
            {
                NewDocument();
            }

            BuildImportMenus();
            RefreshFunctionTaskBarItems();
        }

        void accelaratorKeyMap1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (ShortcutKeys.Haldle(e.KeyData))
                {
                    e.SuppressKeyPress = true;
                }
            }
        }

        protected override void OnOptionsChanged()
        {
            base.OnOptionsChanged();

            ListRecentFiles();
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (!TrySaveTabs())
            {
                e.Cancel = true;
                return;
            }

            if (!mdiWorkSpace1.CloseAll())
            {
                e.Cancel = true;
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            Options.Current.SetValue(OptionNames.Customizations.MainWindowMaximized, WindowState == FormWindowState.Maximized);
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            //D.Message("Render Language");
            ResetWindowTitle();

            BtnStart.Text = Lang._("Menu");
            BtnOpen.ToolTipText = Lang.GetText(Lang.GetTextWithEllipsis("Open a old file"), Keys.Control | Keys.O);
            //BtnNew.Text = Lang._("New");
            TabNew.ToolTipText = Lang.GetText(Lang._("Create a new Mind Map"), Keys.Control | Keys.N);
            BtnHelp.Text = BtnHelp.ToolTipText = Lang._("Help");
            MenuNew.Text = Lang._("New");
            MenuOpen.Text = Lang.GetTextWithEllipsis("Open");
            MenuSave.Text = Lang._("Save");
            MenuSaveAs.Text = Lang.GetTextWithEllipsis("Save As");
            MenuPreview.Text = Lang.GetTextWithEllipsis("Print Preview");
            MenuPrint.Text = Lang.GetTextWithEllipsis("Print");
            toolStripMenuItem1.Text = Lang.GetTextWithEllipsis("Export");
            MenuImport.Text = Lang._("Import");
            MenuOptions.Text = Lang.GetTextWithEllipsis("Options");
            MenuRecentFiles.Text = Lang._("Recent Files");
            MenuExit.Text = Lang._("Exit");

            MenuHelps.Text = Lang._("Help");
            MenuShortcuts.Text = Lang._("Accelerator Keys Table");
            MenuQuickHelp.Text = Lang._("Quick Help");
            MenuMailToMe.Text = Lang.GetTextWithEllipsis("Mail To Me");
            MenuCheckUpdate.Text = Lang.GetTextWithEllipsis("Check Update");
            MenuMailToMe.ToolTipText = Lang.GetTextWithEllipsis("Send Feedback to Author (e.g. Bugs, Ideas)");
            MenuHomePage.Text = Lang.GetTextWithEllipsis("Home Page");
            MenuAbout.Text = Lang.GetTextWithEllipsis("About");
            MenuDonation.Text = Lang.GetTextWithEllipsis("Donation");

            if (MenuClearRecentFiles != null)
                MenuClearRecentFiles.Text = Lang.GetTextWithEllipsis("Clear Recent Files");

            mdiWorkSpace1.Text = ProductInfo.Title;

            // All Support Format (*.bmd, *.mm)|*.bmd,*.mm|
            openFileDialog1.Filter = string.Format("{0} (*{1})|*{1};|{2} (*.*)|*.*",
                Lang._("Blumind Mind Map File"),
                DocumentType.Blumind.DefaultExtension,
                Lang._("All Files"));
        }

        protected override void OnSelectedFormChanged(BaseForm old)
        {
            base.OnSelectedFormChanged(old);

            if (old != null)
            {
                old.TextChanged -= SelectedDocumentForm_TextChanged;
            }

            if (SelectedDocumentForm == null)
            {
                MenuSave.Enabled = false;
                MenuSaveAs.Enabled = false;
                MenuPrint.Enabled = false;
                MenuPreview.Enabled = false;
            }
            else
            {
                MenuSave.Enabled = true;
                MenuSaveAs.Enabled = true;
                MenuPrint.Enabled = true;
                MenuPreview.Enabled = true;

                SelectedDocumentForm.TextChanged += SelectedDocumentForm_TextChanged;
            }

            ChartTip.Global.HideForce();

            ResetWindowTitle();
        }

        void SelectedDocumentForm_TextChanged(object sender, EventArgs e)
        {
            ResetWindowTitle();
        }

        bool TrySaveTabs()
        {
            var saveTabs = Options.Current.GetValue(OptionNames.Miscellaneous.SaveTabs, SaveTabsType.Ask);
            if (saveTabs == SaveTabsType.No)
            {
                return true;
            }

            // ensure document saved
            bool cancel = false;
            ComfirmSaveDocuments(ref cancel);
            if (cancel)
            {
                return false;
            }

            // ask and save
            string[] tabs = GetOpendDocuments();

            if (tabs.Length > 0 && saveTabs == SaveTabsType.Ask)
            {
                if (tabs.Length > 0)
                {
                    var dialog = new SaveTabsDialog();
                    var dr = dialog.ShowDialog(this);
                    if (dr == DialogResult.Cancel)
                        return false;

                    if (dialog.DoNotAskAgain)
                        Options.Current.SetValue(OptionNames.Miscellaneous.SaveTabs, (dr == DialogResult.Yes) ? SaveTabsType.Yes : SaveTabsType.No);
                    else
                        Options.Current.SetValue(OptionNames.Miscellaneous.SaveTabs, SaveTabsType.Ask);

                    if (dr == DialogResult.No)
                    {
                        Options.Current.SetValue(OptionNames.Miscellaneous.LastOpenTabs, null);
                        return true;
                    }
                }
            }

            Options.Current.SetValue(OptionNames.Miscellaneous.LastOpenTabs, tabs);
            return true;
        }

        void OpenSavedTabs()
        {
            var tabs = Options.Current.GetValue<string[]>(OptionNames.Miscellaneous.LastOpenTabs);
            if (!tabs.IsNullOrEmpty())
            {
                foreach (var filename in tabs)
                {
                    if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
                    {
                        OpenDocument(filename);
                    }
                }
            }
        }

        void ResetWindowTitle()
        {
            if (SelectedForm != null)
                Text = string.Format("{0} - {1}", ProductInfo.Title, SelectedForm.Text);
            else
                Text = ProductInfo.Title;
        }

        void SetAGoodLocation()
        {
            if (WindowState == FormWindowState.Normal)
            {
                Rectangle rect = Screen.GetWorkingArea(this);
                Location = new Point(Math.Max(rect.X, Math.Min(rect.Right - Width, Location.X)),
                    Math.Max(rect.Y, Math.Min(rect.Bottom - Height, Location.Y)));
            }
        }

        protected override void AfterInitialize()
        {
            base.AfterInitialize();

            InitializeWindowStates();
        }

        void ListRecentFiles()
        {
            if (MenuRecentFiles == null)
                return;

            MenuRecentFiles.DropDownItems.Clear();
            if (RecentFilesManage.Default.Count == 0)
            {
                this.MenuRecentFiles.Enabled = false;
            }
            else
            {
                const int MaxItems = 8;
                int count = 0;
                for (int i = RecentFilesManage.Default.Count - 1; i >= 0; i--)
                {
                    string filename = RecentFilesManage.Default[i].Filename;
                    var mi = new ToolStripMenuItem();
                    mi.Text = Path.GetFileNameWithoutExtension(filename);
                    mi.ToolTipText = RecentFilesManage.Default[i].Filename;
                    mi.Tag = RecentFilesManage.Default[i].Filename;

                    DocumentType type = DocumentType.FindDocumentType(Path.GetExtension(filename));
                    if (type != null && type.Icon != null)
                        mi.Image = type.Icon;
                    mi.Click += new EventHandler(MenuRecentFile_Click);
                    MenuRecentFiles.DropDownItems.Add(mi);

                    count++;
                    if (count >= MaxItems)
                        break;
                }

                MenuRecentFiles.DropDownItems.Add(new ToolStripSeparator());

                if (MenuClearRecentFiles == null)
                {
                    MenuClearRecentFiles = new ToolStripMenuItem();
                    MenuClearRecentFiles.Click += new EventHandler(MenuClearRecentFiles_Click);
                }
                MenuClearRecentFiles.Text = Lang.GetTextWithEllipsis("Clear Recent Files");
                MenuRecentFiles.DropDownItems.Add(MenuClearRecentFiles);

                this.MenuRecentFiles.Enabled = true;
            }
        }

        void BtnStart_Click(object sender, EventArgs e)
        {
            //StartMenu.Show(taskBar1, new Point(BtnStart.Bounds.Left, BtnStart.Bounds.Bottom + 1), ToolStripDropDownDirection.BelowRight);
            BtnStart.ShowMenu(StartMenu);
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);

            if (WindowState == FormWindowState.Normal)
                Options.Current.SetValue(OptionNames.Customizations.MainWindowSize, Size);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WinMessages.WM_DROPFILES:
                    OnDropFiles(m.WParam);
                    break;
            }

            base.WndProc(ref m);

            switch (m.Msg)
            {
                case WinMessages.WM_COPYDATA:
                    OnWmCopyData(ref m);
                    break;
            }
        }

        void OnWmCopyData(ref Message m)
        {
            var cbd = OSHelper.IntPtrToStruct<COPYDATASTRUCT>(m.LParam);
            if (cbd.dwData.ToInt64() == Program.OPEN_FILES_MESSAGE)
            {
                var buffer = OSHelper.GetBuffer(cbd.lpData, cbd.cbData);
                if (!buffer.IsNullOrEmpty())
                {
                    var str = Encoding.UTF8.GetString(buffer);
                    var files = str.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    OpenDocuments(files);
                    m.Result = new IntPtr(1);

                    if (this.WindowState == FormWindowState.Minimized)
                        this.WindowState = FormWindowState.Normal;
                    this.Activate();
                }
            }
        }

        void OnDropFiles(IntPtr hDrop)
        {
            var filenames = new List<string>();
            int count = Shell32.DragQueryFile(hDrop, 0xFFFFFFFF, null, 0);
            for (uint i = 0; i < count; i++)
            {
                var sb = new StringBuilder(256);
                if (Shell32.DragQueryFile(hDrop, i, sb, (uint)sb.Capacity) > 0)
                {
                    filenames.Add(sb.ToString());
                }
            }

            Shell32.DragFinish(hDrop);

            //
            var pt = this.PointToClient(Control.MousePosition);
            var de = new DropFilesEventArgs(filenames.ToArray(), pt);
            this.PostDropFiles(de);
            if (de.Handled)
                return;

            if (filenames.Count > 0)
            {
                OpenDocuments(filenames.ToArray());
            }
        }

        void MenuHelp_Click(object sender, EventArgs e)
        {
        }

        void MenuHomePage_Click(object sender, EventArgs e)
        {
            Helper.OpenUrl(ProductInfo.WebSite);
        }

        void MenuMailToMe_Click(object sender, EventArgs e)
        {
            Helper.OpenUrl(string.Format("mailto:{0}", ProductInfo.SupportEmail));
        }
        
        public void ShowFindDialog(ChartControl chartControl, FindDialog.FindDialogMode mode)
        {
            if (MyFindDialog == null || MyFindDialog.IsDisposed)
            {
                FindDialog fd = MyFindDialog;
                MyFindDialog = new FindDialog(this);
                if (fd != null)
                {
                    MyFindDialog.StartPosition = FormStartPosition.Manual;
                    MyFindDialog.Location = fd.Location;
                    MyFindDialog.OpenOptions = fd.OpenOptions;
                }
            }

            MyFindDialog.Mode = mode;
            if (MyFindDialog.Visible)
                MyFindDialog.Activate();
            else
                MyFindDialog.Show(this);
            MyFindDialog.ResetFocus();
        }

        void StartMenu_Opening(object sender, CancelEventArgs e)
        {
            BuildImportMenus();
        }

        void BuildImportMenus()
        {
            if (ImportMenusHasBuilded)
                return;

            MenuImport.DropDownItems.Clear();

            var engines = DocumentImportEngine.GetEngines();
            foreach (var engine in engines)
            {
                if (engine.DocumentType == null)
                    continue;

                ToolStripMenuItem miImport = new ToolStripMenuItem();
                miImport.Text = Lang.GetTextWithEllipsis(engine.DocumentType.Name);
                miImport.Tag = engine;
                if (engine.DocumentType.Icon != null)
                    miImport.Image = engine.DocumentType.Icon;
                miImport.Click += new EventHandler(MenuImport_Click);

                MenuImport.DropDownItems.Add(miImport);
            }

            MenuImport.Enabled = MenuImport.DropDownItems.Count > 0;
            ImportMenusHasBuilded = true;
        }

        void MenuImport_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                var menuItem = (ToolStripMenuItem)sender;
                if (menuItem.Tag is DocumentImportEngine)
                {
                    var engine = (DocumentImportEngine)menuItem.Tag;
                    if (engine.DocumentType == null)
                        return;

                    var dlg = new OpenFileDialog();
                    dlg.Filter = engine.DocumentType.FileDialogFilter;
                    dlg.DefaultExt = engine.DocumentType.DefaultExtension;
                    dlg.Title = Lang._("Import");
                    if (dlg.ShowDialog(this) == DialogResult.OK)
                    {
                        string extension = Path.GetExtension(dlg.FileName);
                        if (extension.Equals(Document.Extension, StringComparison.OrdinalIgnoreCase))
                        {
                            OpenDocument(dlg.FileName);
                        }
                        else
                        {
#if !DEBUG
                            try
                            {
#endif
                                var dom = engine.ImportFile(dlg.FileName);
                                if (dom != null)
                                    OpenDocument(dom, false);
#if !DEBUG
                            }
                            catch (System.Exception ex)
                            {
                                this.ShowMessage(ex);
                            }
#endif
                        }
                    }
                }
            }
        }

        void MenuExportDocument_Click(object sender, EventArgs e)
        {
            if (SelectedDocumentForm != null && SelectedDocumentForm.Document != null)
            {
                var dialog = new ExportDocumentDialog(SelectedDocumentForm.Document);
                if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    var engine = ChartsExportEngine.GetEngine(dialog.DocumentType.TypeMime);
                    if (engine == null)
                        this.ShowMessage("The format is not supported", MessageBoxIcon.Error);
                    else
                        engine.Export(dialog.Document, dialog.SelectedCharts);
                }
            }
        }

        void MenuShortcuts_Click(object sender, EventArgs e)
        {
            if (ShortcutsMapDialog == null)
                ShortcutsMapDialog = new ShortcutKeysMapDialog();
            ShortcutsMapDialog.ShowDialog(this);
        }

        void MenuCheckUpdate_Click_1(object sender, EventArgs e)
        {
            if (CheckUpdateForm != null && !CheckUpdateForm.IsDisposed)
            {
                CheckUpdateForm.Activate();
                CheckUpdateForm.MoveToCenterScreen();
            }
            else
            {
                CheckUpdateForm = new CheckUpdate();
                CheckUpdateForm.MoveToCenterScreen();
                CheckUpdateForm.Show(this);
            }
        }

        void MenuQuickHelp_Click(object sender, EventArgs e)
        {
            Helper.OpenQuickHelp();
        }

        void Default_KeyManChanged(object sender, EventArgs e)
        {
            MenuNew.ShortcutKeyDisplayString = KeyMap.New.ToString();
            MenuOpen.ShortcutKeyDisplayString = KeyMap.Open.ToString();
            MenuSave.ShortcutKeyDisplayString = KeyMap.Save.ToString();
            MenuQuickHelp.ShortcutKeys = KeyMap.Help.Keys;
        }

        public override void ApplyTheme(UITheme theme)
        {
            if (theme != null)
            {
                StartMenu.Renderer = theme.ToolStripRenderer;
                HelpMenu.Renderer = theme.ToolStripRenderer;
            }
        }
        
        void TaskBar_Items_ItemRemoved(object sender, XListEventArgs<TabItem> e)
        {
            RefreshFunctionTaskBarItems();
        }

        void TaskBar_Items_ItemAdded(object sender, XListEventArgs<TabItem> e)
        {
            RefreshFunctionTaskBarItems();
        }

        void RefreshFunctionTaskBarItems()
        {
            var hasForms = TaskBar.Items.Exists(item => item.Tag is Form);            
            TabNew.Visible = hasForms;
            BtnOpen.Visible = hasForms;
        }

        private void MenuDonation_Click(object sender, EventArgs e)
        {
            var dialog = new DonationDialog();
            dialog.ShowInTaskbar = false;
            dialog.ShowDialog(this);
        }
    }
}
