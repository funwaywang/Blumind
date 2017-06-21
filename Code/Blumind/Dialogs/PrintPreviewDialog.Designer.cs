namespace Blumind
{
    partial class PrintPreviewDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStrip1 = new Blumind.Controls.ToolStripPro();
            this.TsbPrint = new System.Windows.Forms.ToolStripButton();
            this.TsbPrintSetup = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.TsbFirstPage = new System.Windows.Forms.ToolStripButton();
            this.TsbPreviousPage = new System.Windows.Forms.ToolStripButton();
            this.TsuPageNO = new Blumind.Controls.ToolStripNumericUpDown();
            this.TsbNextPage = new System.Windows.Forms.ToolStripButton();
            this.TsbLastPage = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.TsbZoomOut = new System.Windows.Forms.ToolStripButton();
            this.TsbZoomIn = new System.Windows.Forms.ToolStripButton();
            this.TsbZooms = new System.Windows.Forms.ToolStripDropDownButton();
            this.MenuZoomFitPage = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuZoomFitWidth = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuZoomFitHeight = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.separatorToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.TsbPages = new System.Windows.Forms.ToolStripDropDownButton();
            this.MenuOnePage = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuTwoPages = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuFourPages = new System.Windows.Forms.ToolStripMenuItem();
            this.separatorToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.TsbClose = new System.Windows.Forms.ToolStripButton();
            this.pageToolStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.previewControl = new Blumind.Controls.PrintPreviewControl();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.HalfRenderBackground = true;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsbPrint,
            this.TsbPrintSetup,
            this.toolStripSeparator1,
            this.TsbFirstPage,
            this.TsbPreviousPage,
            this.TsuPageNO,
            this.TsbNextPage,
            this.TsbLastPage,
            this.toolStripSeparator2,
            this.TsbZoomOut,
            this.TsbZooms,
            this.TsbZoomIn,
            this.separatorToolStripSeparator,
            this.TsbPages,
            this.separatorToolStripSeparator1,
            this.TsbClose,
            this.pageToolStripLabel});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(638, 34);
            this.toolStrip1.TabIndex = 1;
            // 
            // TsbPrint
            // 
            this.TsbPrint.Image = global::Blumind.Properties.Resources.print;
            this.TsbPrint.Name = "TsbPrint";
            this.TsbPrint.Padding = new System.Windows.Forms.Padding(2);
            this.TsbPrint.Size = new System.Drawing.Size(58, 27);
            this.TsbPrint.Text = "Print";
            this.TsbPrint.Click += new System.EventHandler(this.TsbPrint_Click);
            // 
            // TsbPrintSetup
            // 
            this.TsbPrintSetup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TsbPrintSetup.Image = global::Blumind.Properties.Resources.preferences;
            this.TsbPrintSetup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TsbPrintSetup.Name = "TsbPrintSetup";
            this.TsbPrintSetup.Padding = new System.Windows.Forms.Padding(2);
            this.TsbPrintSetup.Size = new System.Drawing.Size(24, 27);
            this.TsbPrintSetup.Text = "Print Setup";
            this.TsbPrintSetup.Click += new System.EventHandler(this.TsbPrintSetup_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Padding = new System.Windows.Forms.Padding(2);
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 30);
            // 
            // TsbFirstPage
            // 
            this.TsbFirstPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TsbFirstPage.Image = global::Blumind.Properties.Resources.first;
            this.TsbFirstPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TsbFirstPage.Name = "TsbFirstPage";
            this.TsbFirstPage.Padding = new System.Windows.Forms.Padding(2);
            this.TsbFirstPage.Size = new System.Drawing.Size(24, 27);
            this.TsbFirstPage.Text = "First Page";
            this.TsbFirstPage.Click += new System.EventHandler(this.TsbFirstPage_Click);
            // 
            // TsbPreviousPage
            // 
            this.TsbPreviousPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TsbPreviousPage.Image = global::Blumind.Properties.Resources.previous;
            this.TsbPreviousPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TsbPreviousPage.Name = "TsbPreviousPage";
            this.TsbPreviousPage.Padding = new System.Windows.Forms.Padding(2);
            this.TsbPreviousPage.Size = new System.Drawing.Size(24, 27);
            this.TsbPreviousPage.Text = "Previous Page";
            this.TsbPreviousPage.Click += new System.EventHandler(this.TsbPreviousPage_Click);
            // 
            // TsuPageNO
            // 
            this.TsuPageNO.Name = "TsuPageNO";
            this.TsuPageNO.Padding = new System.Windows.Forms.Padding(2);
            this.TsuPageNO.Size = new System.Drawing.Size(49, 27);
            this.TsuPageNO.ToolTipText = "Current Page";
            this.TsuPageNO.UpDownControlTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TsuPageNO.Value = 1;
            this.TsuPageNO.ValueChanged += new System.EventHandler(this.TsuPageNO_ValueChanged);
            // 
            // TsbNextPage
            // 
            this.TsbNextPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TsbNextPage.Image = global::Blumind.Properties.Resources.next;
            this.TsbNextPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TsbNextPage.Name = "TsbNextPage";
            this.TsbNextPage.Padding = new System.Windows.Forms.Padding(2);
            this.TsbNextPage.Size = new System.Drawing.Size(24, 27);
            this.TsbNextPage.Text = "Next Page";
            this.TsbNextPage.Click += new System.EventHandler(this.TsbNextPage_Click);
            // 
            // TsbLastPage
            // 
            this.TsbLastPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TsbLastPage.Image = global::Blumind.Properties.Resources.last;
            this.TsbLastPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TsbLastPage.Name = "TsbLastPage";
            this.TsbLastPage.Padding = new System.Windows.Forms.Padding(2);
            this.TsbLastPage.Size = new System.Drawing.Size(24, 27);
            this.TsbLastPage.Text = "Last Page";
            this.TsbLastPage.Click += new System.EventHandler(this.TsbLastPage_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Padding = new System.Windows.Forms.Padding(2);
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 30);
            // 
            // TsbZoomOut
            // 
            this.TsbZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TsbZoomOut.Image = global::Blumind.Properties.Resources.zoom_out;
            this.TsbZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TsbZoomOut.Name = "TsbZoomOut";
            this.TsbZoomOut.Padding = new System.Windows.Forms.Padding(2);
            this.TsbZoomOut.Size = new System.Drawing.Size(24, 27);
            this.TsbZoomOut.Text = "Zoom Out";
            this.TsbZoomOut.Click += new System.EventHandler(this.TsbZoomOut_Click);
            // 
            // TsbZoomIn
            // 
            this.TsbZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TsbZoomIn.Image = global::Blumind.Properties.Resources.zoom_in;
            this.TsbZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TsbZoomIn.Name = "TsbZoomIn";
            this.TsbZoomIn.Padding = new System.Windows.Forms.Padding(2);
            this.TsbZoomIn.Size = new System.Drawing.Size(24, 27);
            this.TsbZoomIn.Text = "Zoom In";
            this.TsbZoomIn.Click += new System.EventHandler(this.TsbZoomIn_Click);
            // 
            // TsbZoom
            // 
            this.TsbZooms.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.TsbZooms.DoubleClickEnabled = true;
            this.TsbZooms.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuZoomFitPage,
            this.MenuZoomFitWidth,
            this.MenuZoomFitHeight,
            this.toolStripSeparator3});
            this.TsbZooms.Image = global::Blumind.Properties.Resources.zoom;
            this.TsbZooms.Name = "TsbZooms";
            this.TsbZooms.Padding = new System.Windows.Forms.Padding(2);
            this.TsbZooms.Size = new System.Drawing.Size(33, 27);
            // 
            // MenuZoomFitPage
            // 
            this.MenuZoomFitPage.Name = "MenuZoomFitPage";
            this.MenuZoomFitPage.Size = new System.Drawing.Size(131, 22);
            this.MenuZoomFitPage.Text = "&Fit Page";
            this.MenuZoomFitPage.Click += new System.EventHandler(this.MenuZoomFitPage_Click);
            // 
            // MenuZoomFitWidth
            // 
            this.MenuZoomFitWidth.Name = "MenuZoomFitWidth";
            this.MenuZoomFitWidth.Size = new System.Drawing.Size(131, 22);
            this.MenuZoomFitWidth.Text = "Fit Width";
            this.MenuZoomFitWidth.Click += new System.EventHandler(this.MenuZoomFitWidth_Click);
            // 
            // MenuZoomFitHeight
            // 
            this.MenuZoomFitHeight.Name = "MenuZoomFitHeight";
            this.MenuZoomFitHeight.Size = new System.Drawing.Size(131, 22);
            this.MenuZoomFitHeight.Text = "Fit Height";
            this.MenuZoomFitHeight.Click += new System.EventHandler(this.MenuZoomFitHeight_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(128, 6);
            // 
            // separatorToolStripSeparator
            // 
            this.separatorToolStripSeparator.Name = "separatorToolStripSeparator";
            this.separatorToolStripSeparator.Padding = new System.Windows.Forms.Padding(2);
            this.separatorToolStripSeparator.Size = new System.Drawing.Size(6, 30);
            // 
            // TsbPages
            // 
            this.TsbPages.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TsbPages.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuOnePage,
            this.MenuTwoPages,
            this.MenuFourPages});
            this.TsbPages.Image = global::Blumind.Properties.Resources.print_pages;
            this.TsbPages.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TsbPages.Name = "TsbPages";
            this.TsbPages.Padding = new System.Windows.Forms.Padding(2);
            this.TsbPages.Size = new System.Drawing.Size(33, 27);
            this.TsbPages.Text = "Four Pages";
            // 
            // MenuOnePage
            // 
            this.MenuOnePage.Name = "MenuOnePage";
            this.MenuOnePage.Size = new System.Drawing.Size(141, 22);
            this.MenuOnePage.Text = "One Page";
            this.MenuOnePage.Click += new System.EventHandler(this.MenuOnePage_Click);
            // 
            // MenuTwoPages
            // 
            this.MenuTwoPages.Name = "MenuTwoPages";
            this.MenuTwoPages.Size = new System.Drawing.Size(141, 22);
            this.MenuTwoPages.Text = "Two Pages";
            this.MenuTwoPages.Click += new System.EventHandler(this.MenuTwoPages_Click);
            // 
            // MenuFourPages
            // 
            this.MenuFourPages.Name = "MenuFourPages";
            this.MenuFourPages.Size = new System.Drawing.Size(141, 22);
            this.MenuFourPages.Text = "Four Pages";
            this.MenuFourPages.Click += new System.EventHandler(this.MenuFourPages_Click);
            // 
            // separatorToolStripSeparator1
            // 
            this.separatorToolStripSeparator1.Name = "separatorToolStripSeparator1";
            this.separatorToolStripSeparator1.Padding = new System.Windows.Forms.Padding(2);
            this.separatorToolStripSeparator1.Size = new System.Drawing.Size(6, 30);
            // 
            // TsbClose
            // 
            this.TsbClose.Image = global::Blumind.Properties.Resources.close;
            this.TsbClose.Name = "TsbClose";
            this.TsbClose.Padding = new System.Windows.Forms.Padding(2);
            this.TsbClose.Size = new System.Drawing.Size(64, 27);
            this.TsbClose.Text = "Close";
            this.TsbClose.Click += new System.EventHandler(this.TsbClose_Click);
            // 
            // pageToolStripLabel
            // 
            this.pageToolStripLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.pageToolStripLabel.Name = "pageToolStripLabel";
            this.pageToolStripLabel.Padding = new System.Windows.Forms.Padding(2);
            this.pageToolStripLabel.Size = new System.Drawing.Size(4, 27);
            // 
            // previewControl
            // 
            this.previewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewControl.Location = new System.Drawing.Point(0, 34);
            this.previewControl.Name = "previewControl";
            this.previewControl.Size = new System.Drawing.Size(638, 317);
            this.previewControl.TabIndex = 0;
            this.previewControl.PageCountChanged += new System.EventHandler(this.previewControl_PageCountChanged);
            this.previewControl.RowsChanged += new System.EventHandler(this.previewControl_RowsChanged);
            this.previewControl.StartPageChanged += new System.EventHandler(this.previewControl_StartPageChanged);
            this.previewControl.ZoomChanged += new System.EventHandler(this.previewControl_ZoomChanged);
            this.previewControl.ColumnsChanged += new System.EventHandler(this.previewControl_ColumnsChanged);
            // 
            // PrintPreviewDialog
            // 
            this.ClientSize = new System.Drawing.Size(638, 351);
            this.Controls.Add(this.previewControl);
            this.Controls.Add(this.toolStrip1);
            this.Name = "PrintPreviewDialog";
            this.ShowInTaskbar = false;
            this.Text = "Print Preview";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripButton TsbClose;
        private Blumind.Controls.ToolStripNumericUpDown TsuPageNO;
        private System.Windows.Forms.ToolStripLabel pageToolStripLabel;
        private Blumind.Controls.PrintPreviewControl previewControl;
        private System.Windows.Forms.ToolStripButton TsbPrint;
        private System.Windows.Forms.ToolStripSeparator separatorToolStripSeparator;
        private System.Windows.Forms.ToolStripSeparator separatorToolStripSeparator1;
        private Blumind.Controls.ToolStripPro toolStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton TsbZoomOut;
        private System.Windows.Forms.ToolStripButton TsbZoomIn;
        private System.Windows.Forms.ToolStripButton TsbFirstPage;
        private System.Windows.Forms.ToolStripButton TsbPreviousPage;
        private System.Windows.Forms.ToolStripButton TsbNextPage;
        private System.Windows.Forms.ToolStripButton TsbLastPage;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripDropDownButton TsbZooms;
        private System.Windows.Forms.ToolStripMenuItem MenuZoomFitPage;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton TsbPrintSetup;
        private System.Windows.Forms.ToolStripMenuItem MenuZoomFitWidth;
        private System.Windows.Forms.ToolStripMenuItem MenuZoomFitHeight;
        private System.Windows.Forms.ToolStripDropDownButton TsbPages;
        private System.Windows.Forms.ToolStripMenuItem MenuOnePage;
        private System.Windows.Forms.ToolStripMenuItem MenuTwoPages;
        private System.Windows.Forms.ToolStripMenuItem MenuFourPages;
    }
}