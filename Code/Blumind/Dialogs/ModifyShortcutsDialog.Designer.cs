namespace Blumind.Dialogs
{
    partial class ModifyShortcutsDialog
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.shortcutKeysEditor1 = new Blumind.Core.ShortcutKeysEditor();
            this.LabOperation = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.shortcutKeysEditor1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.LabOperation, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(11, 11);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(296, 180);
            this.tableLayoutPanel1.TabIndex = 10003;
            // 
            // shortcutKeysEditor1
            // 
            this.shortcutKeysEditor1.Location = new System.Drawing.Point(68, 23);
            this.shortcutKeysEditor1.Name = "shortcutKeysEditor1";
            this.shortcutKeysEditor1.Padding = new System.Windows.Forms.Padding(4);
            this.shortcutKeysEditor1.Size = new System.Drawing.Size(225, 131);
            this.shortcutKeysEditor1.TabIndex = 10004;
            // 
            // LabOperation
            // 
            this.LabOperation.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.LabOperation.AutoSize = true;
            this.LabOperation.Location = new System.Drawing.Point(68, 0);
            this.LabOperation.Name = "LabOperation";
            this.LabOperation.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.LabOperation.Size = new System.Drawing.Size(59, 20);
            this.LabOperation.TabIndex = 10004;
            this.LabOperation.Text = "Operation";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 10004;
            this.label1.Text = "Operation";
            // 
            // ModifyShortcutsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(319, 232);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ModifyShortcutsDialog";
            this.Text = "Modify Accelerator Keys";
            this.Controls.SetChildIndex(this.tableLayoutPanel1, 0);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label LabOperation;
        private Core.ShortcutKeysEditor shortcutKeysEditor1;
    }
}