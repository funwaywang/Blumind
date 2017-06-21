namespace Blumind.Controls
{
    partial class ExceptionDialog
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
            this.BtnClose = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.LabExceptionMessage = new System.Windows.Forms.Label();
            this.exceptionControl1 = new ExceptionControl();
            this.BtnToggleDetails = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnClose
            // 
            this.BtnClose.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.BtnClose.Location = new System.Drawing.Point(426, 8);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(75, 25);
            this.BtnClose.TabIndex = 0;
            this.BtnClose.Text = "&Close";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 24);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(383, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "An exception occurred, Please contact your system administrator";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LabExceptionMessage, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.exceptionControl1, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(76, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(425, 232);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // LabExceptionMessage
            // 
            this.LabExceptionMessage.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.LabExceptionMessage.AutoSize = true;
            this.LabExceptionMessage.Location = new System.Drawing.Point(3, 12);
            this.LabExceptionMessage.Name = "LabExceptionMessage";
            this.LabExceptionMessage.Size = new System.Drawing.Size(59, 12);
            this.LabExceptionMessage.TabIndex = 6;
            this.LabExceptionMessage.Text = "[Message]";
            // 
            // exceptionControl1
            // 
            this.exceptionControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.exceptionControl1.Location = new System.Drawing.Point(3, 27);
            this.exceptionControl1.Name = "exceptionControl1";
            this.exceptionControl1.Size = new System.Drawing.Size(419, 202);
            this.exceptionControl1.TabIndex = 7;
            this.exceptionControl1.Text = "exceptionControl1";
            // 
            // BtnToggleDetails
            // 
            this.BtnToggleDetails.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.BtnToggleDetails.Location = new System.Drawing.Point(12, 8);
            this.BtnToggleDetails.Name = "BtnToggleDetails";
            this.BtnToggleDetails.Size = new System.Drawing.Size(120, 25);
            this.BtnToggleDetails.TabIndex = 7;
            this.BtnToggleDetails.Text = "Show &Details";
            this.BtnToggleDetails.UseVisualStyleBackColor = true;
            this.BtnToggleDetails.Click += new System.EventHandler(this.BtnToggleDetails_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.BtnClose);
            this.panel1.Controls.Add(this.BtnToggleDetails);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 261);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(513, 45);
            this.panel1.TabIndex = 8;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // ExceptionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(513, 306);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ExceptionDialog";
            this.Padding = new System.Windows.Forms.Padding(0);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Exception";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label LabExceptionMessage;
        private ExceptionControl exceptionControl1;
        private System.Windows.Forms.Button BtnToggleDetails;
        private System.Windows.Forms.Panel panel1;
    }
}