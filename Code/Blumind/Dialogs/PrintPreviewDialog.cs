using System;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind
{
    partial class PrintPreviewDialog : BaseForm
    {
        public event EventHandler Print;

        public PrintPreviewDialog()
        {
            InitializeComponent();

            InitializeZoomMenu();

            ShowInTaskbar = false;
            MinimizeBox = false;
            WindowState = FormWindowState.Maximized;

            AfterInitialize();
        }

        [DefaultValue(null)]
        public PrintDocument PrintDocument
        {
            get
            {
                return this.previewControl.Document;
            }
            set
            {
                this.previewControl.Document = value;
            }
        }

        [Browsable(false)]
        public Blumind.Controls.PrintPreviewControl PrintPreviewControl
        {
            get
            {
                return this.previewControl;
            }
        }

        [DefaultValue(false)]
        public bool UseAntiAlias
        {
            get
            {
                return this.PrintPreviewControl.UseAntiAlias;
            }
            set
            {
                this.PrintPreviewControl.UseAntiAlias = value;
            }
        }

        [DefaultValue(1), Browsable(false)]
        public double Zoom
        {
            get { return previewControl.Zoom; }
            set
            {
                previewControl.Zoom = value;
            }
        }

        public ZoomType ZoomType
        {
            get { return previewControl.ZoomType; }
            set
            {
                previewControl.ZoomType = value;
            }
        }

        [Browsable(false)]
        public PageSettings PageSettings
        {
            get
            {
                if (PrintDocument != null)
                    return PrintDocument.DefaultPageSettings;
                else
                    return null;
            }
        }

        void InitializeZoomMenu()
        {
            float[] zooms = new float[] { 0.25f, 0.5f, 0.75f, 1.0f, 1.5f, 2.0f, 2.5f, 3.0f };
            foreach (float zoom in zooms)
            {
                ToolStripMenuItem miZoom = new ToolStripMenuItem();
                miZoom.Text = string.Format("{0}%", (int)(zoom * 100));
                miZoom.Tag = zoom;
                miZoom.Click += new EventHandler(MenuZoom_Click);
                TsbZooms.DropDownItems.Add(miZoom);
            }

            RefreshTsbZoomsMenu();
        }

        void RefreshTsbZoomsMenu()
        {
            foreach (var item in this.TsbZooms.DropDownItems)
            {
                if (item is ToolStripMenuItem)
                {
                    var mi = (ToolStripMenuItem)item;
                    if (mi.Tag is float)
                    {
                        mi.Checked = ((float)mi.Tag == Zoom) && (ZoomType == ZoomType.Custom);
                    }
                }
            }

            MenuZoomFitPage.Checked = ZoomType == ZoomType.FitPage;
            MenuZoomFitWidth.Checked = ZoomType == ZoomType.FitWidth;
            MenuZoomFitHeight.Checked = ZoomType == ZoomType.FitHeight;

            //
            switch (ZoomType)
            {
                case ZoomType.Custom:
                    TsbZooms.Text = previewControl.Zoom.ToString("P0");
                    break;
                case ZoomType.FitHeight:
                    TsbZooms.Text = Lang._("Fit Height");
                    break;
                case ZoomType.FitWidth:
                    TsbZooms.Text = Lang._("Fit Width");
                    break;
                case ZoomType.FitPage:
                    TsbZooms.Text = Lang._("Fit Page");
                    break;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            this.previewControl.InvalidatePreview();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ResetButtonStatus();
        }

        void ResetButtonStatus()
        {
            TsuPageNO.Maximum = Math.Max(1, previewControl.PageCount);
            TsuPageNO.Minimum = 1;

            TsbFirstPage.Enabled = previewControl.StartPage > 0;
            TsbPreviousPage.Enabled = previewControl.StartPage > 0;
            TsbNextPage.Enabled = previewControl.StartPage < previewControl.PageCount - 1;
            TsbLastPage.Enabled = previewControl.StartPage < previewControl.PageCount - 1;

            int pages = previewControl.Rows * previewControl.Columns;
            MenuOnePage.Checked = pages == 1;
            MenuTwoPages.Checked = pages == 2;
            MenuFourPages.Checked = pages == 4;
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            Text = Lang._("Print Preview");

            TsbPrint.Text = Lang._("Print");
            TsbPrintSetup.Text = Lang._("Print Setup");
            TsbFirstPage.Text = Lang._("First Page");
            TsbPreviousPage.Text = Lang._("Previous Page");
            TsbNextPage.Text = Lang._("Next Page");
            TsbLastPage.Text = Lang._("Last Page");
            TsbZoomOut.Text = Lang._("Zoom Out");
            TsbZoomIn.Text = Lang._("Zoom In");
            MenuZoomFitPage.Text = Lang._("Fit Page");
            MenuZoomFitWidth.Text = Lang._("Fit Width");
            MenuZoomFitHeight.Text = Lang._("Fit Height");
            MenuOnePage.Text = Lang._("One Page");
            MenuTwoPages.Text = Lang._("Two Pages");
            MenuFourPages.Text = Lang._("Four Pages");
            TsbPages.Text = Lang._("Four Pages");
            TsbClose.Text = Lang._("Close");

            RefreshTsbZoomsMenu();
        }

        void TsuPageNO_ValueChanged(object sender, EventArgs e)
        {
            int num = TsuPageNO.Value - 1;
            if (num >= 0)
            {
                previewControl.StartPage = num;
            }
            else
            {
                TsuPageNO.Value = previewControl.StartPage + 1;
            }
        }

        void TsbFirstPage_Click(object sender, EventArgs e)
        {
            previewControl.FirstPage();
        }

        void TsbPreviousPage_Click(object sender, EventArgs e)
        {
            previewControl.PreviousPage();
        }

        void TsbNextPage_Click(object sender, EventArgs e)
        {
            previewControl.NextPage();
        }

        void TsbLastPage_Click(object sender, EventArgs e)
        {
            previewControl.LastPage();
        }

        void TsbClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        void TsbPrint_Click(object sender, EventArgs e)
        {
            if (Print != null)
                Print(this, EventArgs.Empty);

            /*if (this.previewControl.Document != null)
            {
                var pd = new PrintDialog();
                pd.UseEXDialog = true;
                pd.Document = PrintDocument;
                //pd.AllowSelection = true;
                //pd.AllowSomePages = true;
                if (pd.ShowDialog(this) == DialogResult.OK)
                {
                    PrintDocument.Print();
                }
            }*/
        }

        void previewControl_StartPageChanged(object sender, EventArgs e)
        {
            TsuPageNO.Value = previewControl.StartPage + 1;

            ResetButtonStatus();
        }

        void previewControl_PageCountChanged(object sender, EventArgs e)
        {
            ResetButtonStatus();
        }

        void previewControl_ZoomChanged(object sender, System.EventArgs e)
        {
            RefreshTsbZoomsMenu();
        }

        void previewControl_RowsChanged(object sender, EventArgs e)
        {
            ResetButtonStatus();
        }

        void previewControl_ColumnsChanged(object sender, EventArgs e)
        {
            ResetButtonStatus();
        }

        void TsbPrintSetup_Click(object sender, EventArgs e)
        {
            var dlg = new PageSetupDialog();
            dlg.Document = PrintDocument;
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                PrintDocument.DefaultPageSettings = dlg.PageSettings;
                previewControl.InvalidatePreview();
            }
        }

        void MenuZoomFitPage_Click(object sender, EventArgs e)
        {
            ZoomType = ZoomType.FitPage;
        }

        void MenuZoomFitHeight_Click(object sender, EventArgs e)
        {
            ZoomType = ZoomType.FitHeight;
        }

        void MenuZoomFitWidth_Click(object sender, EventArgs e)
        {
            ZoomType = ZoomType.FitWidth;
        }

        void TsbZoomOut_Click(object sender, EventArgs e)
        {
            Zoom -= 0.25f;
        }

        void TsbZoomIn_Click(object sender, EventArgs e)
        {
            Zoom += 0.25f;
        }

        void MenuZoom_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem mi = (ToolStripMenuItem)sender;
                if (mi.Tag is float)
                {
                    Zoom = (float)mi.Tag;
                }
            }
        }

        void MenuFourPages_Click(object sender, EventArgs e)
        {
            previewControl.Rows = 2;
            previewControl.Columns = 2;
        }

        void MenuTwoPages_Click(object sender, EventArgs e)
        {
            previewControl.Rows = 1;
            previewControl.Columns = 2;
        }

        void MenuOnePage_Click(object sender, EventArgs e)
        {
            previewControl.Rows = 1;
            previewControl.Columns = 1;
        }

        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);

            if (toolStrip1 != null)
            {
                toolStrip1.Renderer = theme.ToolStripRenderer;
            }

            if (previewControl != null)
                previewControl.BackColor = theme.Colors.MediumDark;
        }
    }
}
