namespace Blumind
{
    partial class AboutDialog
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.BtnOK = new Blumind.Controls.PushButton();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.LnkWebSite = new Blumind.Controls.HyperLink();
            this.labelIcons = new Blumind.Controls.HyperLink();
            this.LnkEmail = new Blumind.Controls.HyperLink();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.LabVersion = new System.Windows.Forms.Label();
            this.LabEMail = new System.Windows.Forms.Label();
            this.LabHomePage = new System.Windows.Forms.Label();
            this.TpgThanks = new System.Windows.Forms.TabPage();
            this.listBoxEx1 = new Blumind.Controls.ListBox2();
            this.label1 = new System.Windows.Forms.Label();
            this.TpgInformation = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tableLayoutPanel1.SuspendLayout();
            this.TpgThanks.SuspendLayout();
            this.TpgInformation.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnOK
            // 
            this.BtnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnOK.Location = new System.Drawing.Point(237, 300);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(75, 25);
            this.BtnOK.TabIndex = 5;
            this.BtnOK.Text = "确定(&O)";
            // 
            // labelVersion
            // 
            this.labelVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelVersion.Location = new System.Drawing.Point(3, 6);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(74, 16);
            this.labelVersion.TabIndex = 1;
            this.labelVersion.Text = "版本";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelCopyright
            // 
            this.labelCopyright.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.labelCopyright, 2);
            this.labelCopyright.Location = new System.Drawing.Point(3, 34);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(281, 16);
            this.labelCopyright.TabIndex = 2;
            this.labelCopyright.Text = "版权";
            this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LnkWebSite
            // 
            this.LnkWebSite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.LnkWebSite.Location = new System.Drawing.Point(83, 90);
            this.LnkWebSite.Name = "LnkWebSite";
            this.LnkWebSite.Size = new System.Drawing.Size(201, 16);
            this.LnkWebSite.TabIndex = 4;
            this.LnkWebSite.Text = "Home Page";
            this.LnkWebSite.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelIcons
            // 
            this.labelIcons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.labelIcons, 2);
            this.labelIcons.Location = new System.Drawing.Point(3, 118);
            this.labelIcons.Name = "labelIcons";
            this.labelIcons.Size = new System.Drawing.Size(281, 16);
            this.labelIcons.TabIndex = 7;
            this.labelIcons.Text = "[ICON]";
            this.labelIcons.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LnkEmail
            // 
            this.LnkEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.LnkEmail.Location = new System.Drawing.Point(83, 62);
            this.LnkEmail.Name = "LnkEmail";
            this.LnkEmail.Size = new System.Drawing.Size(201, 16);
            this.LnkEmail.TabIndex = 8;
            this.LnkEmail.Text = "E-Mail";
            this.LnkEmail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.labelIcons, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelCopyright, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelVersion, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LabVersion, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.LnkEmail, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.LnkWebSite, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.LabEMail, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.LabHomePage, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(287, 252);
            this.tableLayoutPanel1.TabIndex = 9;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // LabVersion
            // 
            this.LabVersion.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.LabVersion.AutoSize = true;
            this.LabVersion.Location = new System.Drawing.Point(83, 8);
            this.LabVersion.Name = "LabVersion";
            this.LabVersion.Size = new System.Drawing.Size(47, 12);
            this.LabVersion.TabIndex = 9;
            this.LabVersion.Text = "0.0.0.0";
            // 
            // LabEMail
            // 
            this.LabEMail.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.LabEMail.AutoSize = true;
            this.LabEMail.Location = new System.Drawing.Point(3, 64);
            this.LabEMail.Name = "LabEMail";
            this.LabEMail.Size = new System.Drawing.Size(41, 12);
            this.LabEMail.TabIndex = 10;
            this.LabEMail.Text = "E-Mail";
            // 
            // LabHomePage
            // 
            this.LabHomePage.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.LabHomePage.AutoSize = true;
            this.LabHomePage.Location = new System.Drawing.Point(3, 92);
            this.LabHomePage.Name = "LabHomePage";
            this.LabHomePage.Size = new System.Drawing.Size(59, 12);
            this.LabHomePage.TabIndex = 11;
            this.LabHomePage.Text = "Home Page";
            // 
            // TpgThanks
            // 
            this.TpgThanks.Controls.Add(this.listBoxEx1);
            this.TpgThanks.Controls.Add(this.label1);
            this.TpgThanks.Location = new System.Drawing.Point(4, 22);
            this.TpgThanks.Name = "TpgThanks";
            this.TpgThanks.Padding = new System.Windows.Forms.Padding(3);
            this.TpgThanks.Size = new System.Drawing.Size(295, 258);
            this.TpgThanks.TabIndex = 1;
            this.TpgThanks.Text = "Thanks";
            this.TpgThanks.UseVisualStyleBackColor = true;
            // 
            // listBoxEx1
            // 
            this.listBoxEx1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxEx1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listBoxEx1.IntegralHeight = false;
            this.listBoxEx1.ItemHeight = 23;
            this.listBoxEx1.Location = new System.Drawing.Point(3, 3);
            this.listBoxEx1.Name = "listBoxEx1";
            this.listBoxEx1.Size = new System.Drawing.Size(289, 213);
            this.listBoxEx1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(3, 216);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(289, 39);
            this.label1.TabIndex = 1;
            this.label1.Text = "In no particular order.\r\nIf you provide help, and your name does not appear above" +
    ", please let me know";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TpgInformation
            // 
            this.TpgInformation.Controls.Add(this.tableLayoutPanel1);
            this.TpgInformation.Location = new System.Drawing.Point(4, 22);
            this.TpgInformation.Name = "TpgInformation";
            this.TpgInformation.Padding = new System.Windows.Forms.Padding(3);
            this.TpgInformation.Size = new System.Drawing.Size(293, 258);
            this.TpgInformation.TabIndex = 0;
            this.TpgInformation.Text = "Information";
            this.TpgInformation.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.TpgInformation);
            this.tabControl1.Controls.Add(this.TpgThanks);
            this.tabControl1.Location = new System.Drawing.Point(11, 10);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(301, 284);
            this.tabControl1.TabIndex = 10;
            // 
            // AboutDialog
            // 
            this.AcceptButton = this.BtnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.CancelButton = this.BtnOK;
            this.ClientSize = new System.Drawing.Size(323, 335);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.BtnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AboutDialog";
            this.Padding = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Text = "About";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.TpgThanks.ResumeLayout(false);
            this.TpgInformation.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Blumind.Controls.PushButton BtnOK;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelCopyright;
        private Blumind.Controls.HyperLink LnkWebSite;
        private Blumind.Controls.HyperLink labelIcons;
        private Blumind.Controls.HyperLink LnkEmail;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label LabVersion;
        private System.Windows.Forms.Label LabEMail;
        private System.Windows.Forms.Label LabHomePage;
        private System.Windows.Forms.TabPage TpgThanks;
        private System.Windows.Forms.TabPage TpgInformation;
        private System.Windows.Forms.TabControl tabControl1;
        private Blumind.Controls.ListBox2 listBoxEx1;
        private System.Windows.Forms.Label label1;
    }
}
