namespace Blumind.Dialogs
{
    partial class FindDialog
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
            this.myToolStrip1 = new Blumind.Controls.ToolStripPro();
            this.TsbFind = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.TsbReplace = new System.Windows.Forms.ToolStripButton();
            this.FbOptions = new Blumind.Controls.FoldingBox();
            this.CkbWithHiddenItems = new System.Windows.Forms.CheckBox();
            this.CkbRegularExpression = new System.Windows.Forms.CheckBox();
            this.CkbCaseSensitive = new System.Windows.Forms.CheckBox();
            this.CkbWholeWordOnly = new System.Windows.Forms.CheckBox();
            this.LabFindWhat = new System.Windows.Forms.Label();
            this.CmbFindWhat = new System.Windows.Forms.ComboBox();
            this.LabReplaceWith = new System.Windows.Forms.Label();
            this.CmbReplaceWith = new System.Windows.Forms.ComboBox();
            this.BtnClose = new Blumind.Controls.PushButton();
            this.BtnReplace = new Blumind.Controls.PushButton();
            this.BtnFind = new Blumind.Controls.PushButton();
            this.myToolStrip1.SuspendLayout();
            this.FbOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // myToolStrip1
            // 
            this.myToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsbFind,
            this.toolStripSeparator1,
            this.TsbReplace});
            this.myToolStrip1.Location = new System.Drawing.Point(8, 7);
            this.myToolStrip1.Name = "myToolStrip1";
            this.myToolStrip1.Size = new System.Drawing.Size(332, 32);
            this.myToolStrip1.TabIndex = 8;
            this.myToolStrip1.Text = "myToolStrip1";
            // 
            // TsbFind
            // 
            this.TsbFind.Image = global::Blumind.Properties.Resources.find;
            this.TsbFind.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TsbFind.Name = "TsbFind";
            this.TsbFind.Padding = new System.Windows.Forms.Padding(2);
            this.TsbFind.Size = new System.Drawing.Size(56, 25);
            this.TsbFind.Text = "Find";
            this.TsbFind.Click += new System.EventHandler(this.TsbFind_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Padding = new System.Windows.Forms.Padding(2);
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // TsbReplace
            // 
            this.TsbReplace.Image = global::Blumind.Properties.Resources.replace;
            this.TsbReplace.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TsbReplace.Name = "TsbReplace";
            this.TsbReplace.Padding = new System.Windows.Forms.Padding(2);
            this.TsbReplace.Size = new System.Drawing.Size(78, 25);
            this.TsbReplace.Text = "Replace";
            this.TsbReplace.Click += new System.EventHandler(this.TsbReplace_Click);
            // 
            // FbOptions
            // 
            this.FbOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FbOptions.Controls.Add(this.CkbWithHiddenItems);
            this.FbOptions.Controls.Add(this.CkbRegularExpression);
            this.FbOptions.Controls.Add(this.CkbCaseSensitive);
            this.FbOptions.Controls.Add(this.CkbWholeWordOnly);
            this.FbOptions.FullHeight = 105;
            this.FbOptions.Location = new System.Drawing.Point(8, 94);
            this.FbOptions.Name = "FbOptions";
            this.FbOptions.Padding = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.FbOptions.Size = new System.Drawing.Size(329, 111);
            this.FbOptions.TabIndex = 4;
            this.FbOptions.Text = "Find Options";
            this.FbOptions.FoldedChanged += new System.EventHandler(this.FbOptions_FoldedChanged);
            // 
            // CkbWithHiddenItems
            // 
            this.CkbWithHiddenItems.AutoSize = true;
            this.CkbWithHiddenItems.BackColor = System.Drawing.Color.Transparent;
            this.CkbWithHiddenItems.Location = new System.Drawing.Point(6, 88);
            this.CkbWithHiddenItems.Name = "CkbWithHiddenItems";
            this.CkbWithHiddenItems.Size = new System.Drawing.Size(126, 16);
            this.CkbWithHiddenItems.TabIndex = 4;
            this.CkbWithHiddenItems.Text = "With Hidden Items";
            this.CkbWithHiddenItems.UseVisualStyleBackColor = false;
            this.CkbWithHiddenItems.CheckedChanged += new System.EventHandler(this.CkbWithHiddenItems_CheckedChanged);
            // 
            // CkbRegularExpression
            // 
            this.CkbRegularExpression.AutoSize = true;
            this.CkbRegularExpression.BackColor = System.Drawing.Color.Transparent;
            this.CkbRegularExpression.Location = new System.Drawing.Point(6, 66);
            this.CkbRegularExpression.Name = "CkbRegularExpression";
            this.CkbRegularExpression.Size = new System.Drawing.Size(132, 16);
            this.CkbRegularExpression.TabIndex = 2;
            this.CkbRegularExpression.Text = "Regular Expression";
            this.CkbRegularExpression.UseVisualStyleBackColor = false;
            this.CkbRegularExpression.CheckedChanged += new System.EventHandler(this.CkbRegularExpression_CheckedChanged);
            // 
            // CkbCaseSensitive
            // 
            this.CkbCaseSensitive.AutoSize = true;
            this.CkbCaseSensitive.BackColor = System.Drawing.Color.Transparent;
            this.CkbCaseSensitive.Location = new System.Drawing.Point(6, 24);
            this.CkbCaseSensitive.Name = "CkbCaseSensitive";
            this.CkbCaseSensitive.Size = new System.Drawing.Size(108, 16);
            this.CkbCaseSensitive.TabIndex = 0;
            this.CkbCaseSensitive.Text = "Case Sensitive";
            this.CkbCaseSensitive.UseVisualStyleBackColor = false;
            this.CkbCaseSensitive.CheckedChanged += new System.EventHandler(this.CkbCaseSensitive_CheckedChanged);
            // 
            // CkbWholeWordOnly
            // 
            this.CkbWholeWordOnly.AutoSize = true;
            this.CkbWholeWordOnly.BackColor = System.Drawing.Color.Transparent;
            this.CkbWholeWordOnly.Location = new System.Drawing.Point(6, 45);
            this.CkbWholeWordOnly.Name = "CkbWholeWordOnly";
            this.CkbWholeWordOnly.Size = new System.Drawing.Size(114, 16);
            this.CkbWholeWordOnly.TabIndex = 1;
            this.CkbWholeWordOnly.Text = "Whole Word Only";
            this.CkbWholeWordOnly.UseVisualStyleBackColor = false;
            this.CkbWholeWordOnly.CheckedChanged += new System.EventHandler(this.CkbWholeWordOnly_CheckedChanged);
            // 
            // LabFindWhat
            // 
            this.LabFindWhat.AutoSize = true;
            this.LabFindWhat.Location = new System.Drawing.Point(5, 47);
            this.LabFindWhat.Name = "LabFindWhat";
            this.LabFindWhat.Size = new System.Drawing.Size(65, 12);
            this.LabFindWhat.TabIndex = 0;
            this.LabFindWhat.Text = "Find What:";
            // 
            // CmbFindWhat
            // 
            this.CmbFindWhat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CmbFindWhat.FormattingEnabled = true;
            this.CmbFindWhat.Location = new System.Drawing.Point(94, 44);
            this.CmbFindWhat.Name = "CmbFindWhat";
            this.CmbFindWhat.Size = new System.Drawing.Size(243, 20);
            this.CmbFindWhat.TabIndex = 1;
            this.CmbFindWhat.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CmbFindWhat_KeyDown);
            // 
            // LabReplaceWith
            // 
            this.LabReplaceWith.AutoSize = true;
            this.LabReplaceWith.Location = new System.Drawing.Point(5, 72);
            this.LabReplaceWith.Name = "LabReplaceWith";
            this.LabReplaceWith.Size = new System.Drawing.Size(83, 12);
            this.LabReplaceWith.TabIndex = 2;
            this.LabReplaceWith.Text = "Replace With:";
            // 
            // CmbReplaceWith
            // 
            this.CmbReplaceWith.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CmbReplaceWith.FormattingEnabled = true;
            this.CmbReplaceWith.Location = new System.Drawing.Point(94, 69);
            this.CmbReplaceWith.Name = "CmbReplaceWith";
            this.CmbReplaceWith.Size = new System.Drawing.Size(242, 20);
            this.CmbReplaceWith.TabIndex = 3;
            this.CmbReplaceWith.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CmbReplaceWith_KeyDown);
            // 
            // BtnClose
            // 
            this.BtnClose.Location = new System.Drawing.Point(262, 231);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(75, 25);
            this.BtnClose.TabIndex = 7;
            this.BtnClose.Text = "Close";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // BtnReplace
            // 
            this.BtnReplace.Location = new System.Drawing.Point(181, 231);
            this.BtnReplace.Name = "BtnReplace";
            this.BtnReplace.Size = new System.Drawing.Size(75, 25);
            this.BtnReplace.TabIndex = 6;
            this.BtnReplace.Text = "Replace";
            this.BtnReplace.UseVisualStyleBackColor = true;
            this.BtnReplace.Click += new System.EventHandler(this.BtnReplace_Click);
            // 
            // BtnFind
            // 
            this.BtnFind.Location = new System.Drawing.Point(100, 231);
            this.BtnFind.Name = "BtnFind";
            this.BtnFind.Size = new System.Drawing.Size(100, 25);
            this.BtnFind.TabIndex = 5;
            this.BtnFind.Text = "Find Next";
            this.BtnFind.UseVisualStyleBackColor = true;
            this.BtnFind.Click += new System.EventHandler(this.BtnFind_Click);
            // 
            // FindDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 266);
            this.Controls.Add(this.BtnFind);
            this.Controls.Add(this.BtnReplace);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.CmbReplaceWith);
            this.Controls.Add(this.LabReplaceWith);
            this.Controls.Add(this.CmbFindWhat);
            this.Controls.Add(this.LabFindWhat);
            this.Controls.Add(this.FbOptions);
            this.Controls.Add(this.myToolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "FindDialog";
            this.Text = "Find";
            this.myToolStrip1.ResumeLayout(false);
            this.myToolStrip1.PerformLayout();
            this.FbOptions.ResumeLayout(false);
            this.FbOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Blumind.Controls.ToolStripPro myToolStrip1;
        private System.Windows.Forms.ToolStripButton TsbFind;
        private System.Windows.Forms.ToolStripButton TsbReplace;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private Blumind.Controls.FoldingBox FbOptions;
        private System.Windows.Forms.CheckBox CkbCaseSensitive;
        private System.Windows.Forms.CheckBox CkbWholeWordOnly;
        private System.Windows.Forms.CheckBox CkbRegularExpression;
        private System.Windows.Forms.Label LabFindWhat;
        private System.Windows.Forms.ComboBox CmbFindWhat;
        private System.Windows.Forms.Label LabReplaceWith;
        private System.Windows.Forms.ComboBox CmbReplaceWith;
        private Blumind.Controls.PushButton BtnClose;
        private Blumind.Controls.PushButton BtnReplace;
        private Blumind.Controls.PushButton BtnFind;
        private System.Windows.Forms.CheckBox CkbWithHiddenItems;
    }
}