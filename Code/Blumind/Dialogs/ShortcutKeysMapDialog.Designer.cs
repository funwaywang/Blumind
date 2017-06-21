namespace Blumind.Dialogs
{
    partial class ShortcutKeysMapDialog
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.ColumnOperation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnKey = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuModify = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuClear = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuReset = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invertSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.BtnReset = new Blumind.Controls.PushButton();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnOperation,
            this.ColumnKey,
            this.ColumnDescription});
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.FullRowSelect = true;
            this.listView1.Location = new System.Drawing.Point(11, 10);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(466, 438);
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // ColumnOperation
            // 
            this.ColumnOperation.Text = "Operation";
            this.ColumnOperation.Width = 120;
            // 
            // ColumnKey
            // 
            this.ColumnKey.Text = "Key";
            this.ColumnKey.Width = 120;
            // 
            // ColumnDescription
            // 
            this.ColumnDescription.Text = "Notes";
            this.ColumnDescription.Width = 200;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuModify,
            this.MenuClear,
            this.MenuReset,
            this.toolStripMenuItem1,
            this.selectAllToolStripMenuItem,
            this.invertSelectionToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(166, 142);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // MenuModify
            // 
            this.MenuModify.Name = "MenuModify";
            this.MenuModify.Size = new System.Drawing.Size(165, 22);
            this.MenuModify.Text = "&Modify...";
            this.MenuModify.Click += new System.EventHandler(this.MenuModify_Click);
            // 
            // MenuClear
            // 
            this.MenuClear.Name = "MenuClear";
            this.MenuClear.Size = new System.Drawing.Size(165, 22);
            this.MenuClear.Text = "&Clear";
            this.MenuClear.Click += new System.EventHandler(this.MenuClear_Click);
            // 
            // MenuReset
            // 
            this.MenuReset.Name = "MenuReset";
            this.MenuReset.Size = new System.Drawing.Size(165, 22);
            this.MenuReset.Text = "&Reset";
            this.MenuReset.Click += new System.EventHandler(this.MenuReset_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(162, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.selectAllToolStripMenuItem.Text = "Select All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // invertSelectionToolStripMenuItem
            // 
            this.invertSelectionToolStripMenuItem.Name = "invertSelectionToolStripMenuItem";
            this.invertSelectionToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.invertSelectionToolStripMenuItem.Text = "Invert Selection";
            this.invertSelectionToolStripMenuItem.Click += new System.EventHandler(this.invertSelectionToolStripMenuItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // BtnReset
            // 
            this.BtnReset.Enabled = false;
            this.BtnReset.Location = new System.Drawing.Point(11, 454);
            this.BtnReset.Name = "BtnReset";
            this.BtnReset.Size = new System.Drawing.Size(100, 25);
            this.BtnReset.TabIndex = 10003;
            this.BtnReset.Text = "&Reset All";
            this.BtnReset.UseVisualStyleBackColor = true;
            this.BtnReset.Click += new System.EventHandler(this.BtnReset_Click);
            // 
            // ShortcutKeysMapDialog
            // 
            this.AcceptButton = null;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = null;
            this.ClientSize = new System.Drawing.Size(488, 491);
            this.Controls.Add(this.BtnReset);
            this.Controls.Add(this.listView1);
            this.Name = "ShortcutKeysMapDialog";
            this.Text = "Accelerator Keys Table";
            this.Controls.SetChildIndex(this.listView1, 0);
            this.Controls.SetChildIndex(this.BtnReset, 0);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader ColumnOperation;
        private System.Windows.Forms.ColumnHeader ColumnKey;
        private System.Windows.Forms.ColumnHeader ColumnDescription;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem MenuModify;
        private System.Windows.Forms.ToolStripMenuItem MenuClear;
        private System.Windows.Forms.ToolStripMenuItem MenuReset;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem invertSelectionToolStripMenuItem;
        private Controls.PushButton BtnReset;
    }
}