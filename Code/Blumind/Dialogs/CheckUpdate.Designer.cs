namespace Blumind.Dialogs
{
    partial class CheckUpdate
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
            this.BtnCancel = new Blumind.Controls.PushButton();
            this.BtnDownload = new Blumind.Controls.PushButton();
            this.LabMessage = new System.Windows.Forms.Label();
            this.TxbVersions = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(382, 282);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 25);
            this.BtnCancel.TabIndex = 5;
            this.BtnCancel.Text = "&Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // BtnDownload
            // 
            this.BtnDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnDownload.Location = new System.Drawing.Point(11, 282);
            this.BtnDownload.Name = "BtnDownload";
            this.BtnDownload.Size = new System.Drawing.Size(144, 25);
            this.BtnDownload.TabIndex = 3;
            this.BtnDownload.Text = "&Download";
            this.BtnDownload.UseVisualStyleBackColor = true;
            this.BtnDownload.Click += new System.EventHandler(this.BtnDownload_Click);
            // 
            // LabMessage
            // 
            this.LabMessage.AutoSize = true;
            this.LabMessage.Location = new System.Drawing.Point(11, 7);
            this.LabMessage.Name = "LabMessage";
            this.LabMessage.Size = new System.Drawing.Size(71, 12);
            this.LabMessage.TabIndex = 6;
            this.LabMessage.Text = "Checking...";
            // 
            // TxbVersions
            // 
            this.TxbVersions.Location = new System.Drawing.Point(11, 22);
            this.TxbVersions.Multiline = true;
            this.TxbVersions.Name = "TxbVersions";
            this.TxbVersions.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TxbVersions.Size = new System.Drawing.Size(446, 254);
            this.TxbVersions.TabIndex = 7;
            this.TxbVersions.WordWrap = false;
            // 
            // CheckUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 317);
            this.Controls.Add(this.TxbVersions);
            this.Controls.Add(this.LabMessage);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnDownload);
            this.Name = "CheckUpdate";
            this.Padding = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Text = "Check Update";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Blumind.Controls.PushButton BtnCancel;
        private Blumind.Controls.PushButton BtnDownload;
        private System.Windows.Forms.Label LabMessage;
        private System.Windows.Forms.TextBox TxbVersions;
    }
}