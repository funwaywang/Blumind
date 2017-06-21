namespace Blumind
{
    partial class ThemesDialog
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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new Blumind.Controls.MultiSelectTreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tabControl1 = new Blumind.Controls.MyTabControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.CkbDefaultTheme = new System.Windows.Forms.CheckBox();
            this.TxbThemeName = new System.Windows.Forms.TextBox();
            this.LalName = new System.Windows.Forms.Label();
            this.toolStrip1 = new Blumind.Controls.ToolStripPro();
            this.TsbNew = new System.Windows.Forms.ToolStripButton();
            this.TsbCopy = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.TsbDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.TsbRefresh = new System.Windows.Forms.ToolStripButton();
            this.BtnApply = new Blumind.Controls.PushButton();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(8, 39);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(619, 297);
            this.splitContainer1.SplitterDistance = 185;
            this.splitContainer1.TabIndex = 1;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(185, 297);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // tabControl1
            // 
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 58);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = -1;
            this.tabControl1.SelectedPage = null;
            this.tabControl1.Size = new System.Drawing.Size(430, 239);
            this.tabControl1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.CkbDefaultTheme);
            this.panel1.Controls.Add(this.TxbThemeName);
            this.panel1.Controls.Add(this.LalName);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(430, 58);
            this.panel1.TabIndex = 0;
            // 
            // CkbDefaultTheme
            // 
            this.CkbDefaultTheme.AutoSize = true;
            this.CkbDefaultTheme.Location = new System.Drawing.Point(45, 32);
            this.CkbDefaultTheme.Name = "CkbDefaultTheme";
            this.CkbDefaultTheme.Size = new System.Drawing.Size(84, 16);
            this.CkbDefaultTheme.TabIndex = 2;
            this.CkbDefaultTheme.Text = "Is Default";
            this.CkbDefaultTheme.UseVisualStyleBackColor = true;
            this.CkbDefaultTheme.CheckedChanged += new System.EventHandler(this.CkbDefaultTheme_CheckedChanged);
            // 
            // TxbThemeName
            // 
            this.TxbThemeName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxbThemeName.Location = new System.Drawing.Point(45, 5);
            this.TxbThemeName.Name = "TxbThemeName";
            this.TxbThemeName.Size = new System.Drawing.Size(378, 21);
            this.TxbThemeName.TabIndex = 1;
            this.TxbThemeName.TextChanged += new System.EventHandler(this.TxbThemeName_TextChanged);
            // 
            // LalName
            // 
            this.LalName.AutoSize = true;
            this.LalName.Location = new System.Drawing.Point(1, 7);
            this.LalName.Name = "LalName";
            this.LalName.Size = new System.Drawing.Size(35, 12);
            this.LalName.TabIndex = 0;
            this.LalName.Text = "Name:";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsbNew,
            this.TsbCopy,
            this.toolStripSeparator1,
            this.TsbDelete,
            this.toolStripSeparator2,
            this.TsbRefresh});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(635, 27);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "myToolStrip1";
            // 
            // TsbNew
            // 
            this.TsbNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TsbNew.Image = global::Blumind.Properties.Resources._new;
            this.TsbNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TsbNew.Name = "TsbNew";
            this.TsbNew.Padding = new System.Windows.Forms.Padding(2);
            this.TsbNew.Size = new System.Drawing.Size(24, 24);
            this.TsbNew.Text = "New";
            this.TsbNew.Click += new System.EventHandler(this.TsbNew_Click);
            // 
            // TsbCopy
            // 
            this.TsbCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TsbCopy.Image = global::Blumind.Properties.Resources.copy;
            this.TsbCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TsbCopy.Name = "TsbCopy";
            this.TsbCopy.Padding = new System.Windows.Forms.Padding(2);
            this.TsbCopy.Size = new System.Drawing.Size(24, 24);
            this.TsbCopy.Text = "Copy";
            this.TsbCopy.Click += new System.EventHandler(this.TsbCopy_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Padding = new System.Windows.Forms.Padding(2);
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
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
            // BtnApply
            // 
            this.BtnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnApply.Location = new System.Drawing.Point(11, 342);
            this.BtnApply.Name = "BtnApply";
            this.BtnApply.Size = new System.Drawing.Size(113, 25);
            this.BtnApply.TabIndex = 2;
            this.BtnApply.Text = "&Apply Theme";
            this.BtnApply.UseVisualStyleBackColor = true;
            this.BtnApply.Click += new System.EventHandler(this.BtnApply_Click);
            // 
            // ThemesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 378);
            this.Controls.Add(this.BtnApply);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ThemesDialog";
            this.Text = "Themes";
            this.Controls.SetChildIndex(this.toolStrip1, 0);
            this.Controls.SetChildIndex(this.splitContainer1, 0);
            this.Controls.SetChildIndex(this.BtnApply, 0);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private Blumind.Controls.MultiSelectTreeView treeView1;
        private System.Windows.Forms.ImageList imageList1;
        private Blumind.Controls.MyTabControl tabControl1;
        private Blumind.Controls.ToolStripPro toolStrip1;
        private System.Windows.Forms.ToolStripButton TsbNew;
        private System.Windows.Forms.ToolStripButton TsbDelete;
        private System.Windows.Forms.ToolStripButton TsbCopy;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox TxbThemeName;
        private System.Windows.Forms.Label LalName;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private Blumind.Controls.PushButton BtnApply;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton TsbRefresh;
        private System.Windows.Forms.CheckBox CkbDefaultTheme;
    }
}