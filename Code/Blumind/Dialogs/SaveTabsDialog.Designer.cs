namespace Blumind.Dialogs
{
    partial class SaveTabsDialog
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.LabMessage = new System.Windows.Forms.Label();
            this.BtnYes = new Blumind.Controls.PushButton();
            this.BtnNo = new Blumind.Controls.PushButton();
            this.CkbDoNotAskMeAgain = new System.Windows.Forms.CheckBox();
            this.BtnCancel = new Blumind.Controls.PushButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Blumind.Properties.Resources.question_large;
            this.pictureBox1.Location = new System.Drawing.Point(11, 11);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // LabMessage
            // 
            this.LabMessage.AutoSize = true;
            this.LabMessage.Location = new System.Drawing.Point(74, 21);
            this.LabMessage.Name = "LabMessage";
            this.LabMessage.Size = new System.Drawing.Size(347, 12);
            this.LabMessage.TabIndex = 3;
            this.LabMessage.Text = "Do you want Blumind save tabs and open them next startup?";
            // 
            // BtnYes
            // 
            this.BtnYes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnYes.Location = new System.Drawing.Point(232, 88);
            this.BtnYes.Name = "BtnYes";
            this.BtnYes.Size = new System.Drawing.Size(75, 25);
            this.BtnYes.TabIndex = 0;
            this.BtnYes.Text = "Yes";
            this.BtnYes.UseVisualStyleBackColor = true;
            this.BtnYes.Click += new System.EventHandler(this.BtnYes_Click);
            // 
            // BtnNo
            // 
            this.BtnNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnNo.Location = new System.Drawing.Point(313, 88);
            this.BtnNo.Name = "BtnNo";
            this.BtnNo.Size = new System.Drawing.Size(75, 25);
            this.BtnNo.TabIndex = 1;
            this.BtnNo.Text = "NO";
            this.BtnNo.UseVisualStyleBackColor = true;
            this.BtnNo.Click += new System.EventHandler(this.BtnNo_Click);
            // 
            // CkbDoNotAskMeAgain
            // 
            this.CkbDoNotAskMeAgain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CkbDoNotAskMeAgain.AutoSize = true;
            this.CkbDoNotAskMeAgain.Location = new System.Drawing.Point(77, 51);
            this.CkbDoNotAskMeAgain.Name = "CkbDoNotAskMeAgain";
            this.CkbDoNotAskMeAgain.Size = new System.Drawing.Size(138, 16);
            this.CkbDoNotAskMeAgain.TabIndex = 4;
            this.CkbDoNotAskMeAgain.Text = "Do not ask me again";
            this.CkbDoNotAskMeAgain.UseVisualStyleBackColor = true;
            this.CkbDoNotAskMeAgain.CheckedChanged += new System.EventHandler(this.CkbDoNotAskMeAgain_CheckedChanged);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(394, 88);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 25);
            this.BtnCancel.TabIndex = 2;
            this.BtnCancel.Text = "&Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // SaveTabsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(480, 124);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.CkbDoNotAskMeAgain);
            this.Controls.Add(this.BtnYes);
            this.Controls.Add(this.BtnNo);
            this.Controls.Add(this.LabMessage);
            this.Controls.Add(this.pictureBox1);
            this.Name = "SaveTabsDialog";
            this.Text = "Save Tabs";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label LabMessage;
        private Blumind.Controls.PushButton BtnYes;
        private Blumind.Controls.PushButton BtnNo;
        private System.Windows.Forms.CheckBox CkbDoNotAskMeAgain;
        private Blumind.Controls.PushButton BtnCancel;
    }
}