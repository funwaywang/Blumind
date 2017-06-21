namespace Blumind.Dialogs
{
    partial class IconLibraryDialog
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
            this.myToolStrip1 = new Blumind.Controls.MyToolStrip();
            this.TsbAddFiles = new System.Windows.Forms.ToolStripButton();
            this.TsbAddFromInternet = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.TsbResize = new System.Windows.Forms.ToolStripButton();
            this.TsbDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.TsbRefresh = new System.Windows.Forms.ToolStripButton();
            this.imageLibraryListBox1 = new Blumind.Controls.ImageLibraryListBox();
            this.BtnClose = new Blumind.Controls.PushButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.PicPreview = new Blumind.Controls.PictureBoxEx();
            this.LabName = new System.Windows.Forms.Label();
            this.LabSize = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.myToolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // myToolStrip1
            // 
            this.myToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsbAddFiles,
            this.TsbAddFromInternet,
            this.toolStripSeparator1,
            this.TsbResize,
            this.TsbDelete,
            this.toolStripSeparator2,
            this.TsbRefresh});
            this.myToolStrip1.Location = new System.Drawing.Point(8, 8);
            this.myToolStrip1.Name = "myToolStrip1";
            this.myToolStrip1.Size = new System.Drawing.Size(578, 31);
            this.myToolStrip1.TabIndex = 0;
            this.myToolStrip1.Text = "myToolStrip1";
            // 
            // TsbAddFiles
            // 
            this.TsbAddFiles.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TsbAddFiles.Image = global::Blumind.Properties.Resources.open;
            this.TsbAddFiles.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TsbAddFiles.Name = "TsbAddFiles";
            this.TsbAddFiles.Padding = new System.Windows.Forms.Padding(2);
            this.TsbAddFiles.Size = new System.Drawing.Size(24, 24);
            this.TsbAddFiles.Text = "Add Files";
            this.TsbAddFiles.Click += new System.EventHandler(this.TsbAddFiles_Click);
            // 
            // TsbAddFromInternet
            // 
            this.TsbAddFromInternet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TsbAddFromInternet.Image = global::Blumind.Properties.Resources.internet;
            this.TsbAddFromInternet.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TsbAddFromInternet.Name = "TsbAddFromInternet";
            this.TsbAddFromInternet.Padding = new System.Windows.Forms.Padding(2);
            this.TsbAddFromInternet.Size = new System.Drawing.Size(24, 24);
            this.TsbAddFromInternet.Text = "Add From Internet";
            this.TsbAddFromInternet.Click += new System.EventHandler(this.TsbAddFromInternet_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Padding = new System.Windows.Forms.Padding(2);
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // TsbResize
            // 
            this.TsbResize.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TsbResize.Image = global::Blumind.Properties.Resources.image_resize;
            this.TsbResize.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TsbResize.Name = "TsbResize";
            this.TsbResize.Padding = new System.Windows.Forms.Padding(2);
            this.TsbResize.Size = new System.Drawing.Size(24, 24);
            this.TsbResize.Text = "Resize";
            this.TsbResize.Click += new System.EventHandler(this.TsbResize_Click);
            // 
            // TsbDelete
            // 
            this.TsbDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TsbDelete.Image = global::Blumind.Properties.Resources.delete;
            this.TsbDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TsbDelete.Name = "TsbDelete";
            this.TsbDelete.Padding = new System.Windows.Forms.Padding(2);
            this.TsbDelete.Size = new System.Drawing.Size(24, 24);
            this.TsbDelete.Text = "Delete";
            this.TsbDelete.Click += new System.EventHandler(this.TsbDelete_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Padding = new System.Windows.Forms.Padding(2);
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // TsbRefresh
            // 
            this.TsbRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TsbRefresh.Image = global::Blumind.Properties.Resources.refresh;
            this.TsbRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TsbRefresh.Name = "TsbRefresh";
            this.TsbRefresh.Padding = new System.Windows.Forms.Padding(2);
            this.TsbRefresh.Size = new System.Drawing.Size(24, 24);
            this.TsbRefresh.Text = "Refresh";
            this.TsbRefresh.Click += new System.EventHandler(this.TsbRefresh_Click);
            // 
            // imageLibraryListBox1
            // 
            this.imageLibraryListBox1.AutoScroll = true;
            this.imageLibraryListBox1.AutoScrollMargin = new System.Drawing.Size(1, 1);
            this.imageLibraryListBox1.AutoScrollMinSize = new System.Drawing.Size(416, 0);
            this.imageLibraryListBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageLibraryListBox1.Location = new System.Drawing.Point(0, 0);
            this.imageLibraryListBox1.MultiSelect = true;
            this.imageLibraryListBox1.Name = "imageLibraryListBox1";
            this.imageLibraryListBox1.Size = new System.Drawing.Size(435, 235);
            this.imageLibraryListBox1.TabIndex = 0;
            this.imageLibraryListBox1.SelectionChanged += new System.EventHandler(this.imageLibraryListBox1_SelectionChanged);
            // 
            // BtnClose
            // 
            this.BtnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnClose.Location = new System.Drawing.Point(508, 290);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(75, 23);
            this.BtnClose.TabIndex = 2;
            this.BtnClose.Text = "&Close";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.PicPreview, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LabName, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.LabSize, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(139, 235);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // PicPreview
            // 
            this.PicPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PicPreview.Location = new System.Drawing.Point(3, 3);
            this.PicPreview.Name = "PicPreview";
            this.PicPreview.Size = new System.Drawing.Size(133, 189);
            this.PicPreview.TabIndex = 0;
            this.PicPreview.TabStop = false;
            // 
            // LabName
            // 
            this.LabName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LabName.AutoSize = true;
            this.LabName.Location = new System.Drawing.Point(49, 198);
            this.LabName.Name = "LabName";
            this.LabName.Size = new System.Drawing.Size(41, 13);
            this.LabName.TabIndex = 1;
            this.LabName.Text = "[Name]";
            // 
            // LabSize
            // 
            this.LabSize.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LabSize.AutoSize = true;
            this.LabSize.Location = new System.Drawing.Point(53, 218);
            this.LabSize.Name = "LabSize";
            this.LabSize.Size = new System.Drawing.Size(33, 13);
            this.LabSize.TabIndex = 2;
            this.LabSize.Text = "[Size]";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(8, 42);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.imageLibraryListBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(578, 235);
            this.splitContainer1.SplitterDistance = 435;
            this.splitContainer1.TabIndex = 1;
            // 
            // IconLibraryDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnClose;
            this.ClientSize = new System.Drawing.Size(594, 324);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.myToolStrip1);
            this.Name = "IconLibraryDialog";
            this.Text = "Icon Library";
            this.myToolStrip1.ResumeLayout(false);
            this.myToolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Blumind.Controls.MyToolStrip myToolStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton TsbDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton TsbRefresh;
        private Blumind.Controls.ImageLibraryListBox imageLibraryListBox1;
        private System.Windows.Forms.ToolStripButton TsbAddFiles;
        private System.Windows.Forms.ToolStripButton TsbAddFromInternet;
        private Blumind.Controls.PushButton BtnClose;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Blumind.Controls.PictureBoxEx PicPreview;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label LabName;
        private System.Windows.Forms.Label LabSize;
        private System.Windows.Forms.ToolStripButton TsbResize;
    }
}