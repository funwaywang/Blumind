namespace Blumind.Dialogs
{
    partial class AboutDialogBox
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
            this.imageBox1 = new Blumind.Controls.ImageBox();
            this.LabProductName = new System.Windows.Forms.Label();
            this.LabVersion = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.LabAboutProduct = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.TxbEmail = new System.Windows.Forms.TextBox();
            this.LnkWebSite = new Blumind.Controls.HyperLink();
            this.LabHomePage = new System.Windows.Forms.Label();
            this.LabEMail = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.LabWechat = new System.Windows.Forms.Label();
            this.BtnClose = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.BtnDonation = new System.Windows.Forms.Button();
            this.TxbTwitter = new System.Windows.Forms.TextBox();
            this.TxbWechat = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageBox1
            // 
            this.imageBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.imageBox1.AutoScroll = true;
            this.imageBox1.AutoScrollMinSize = new System.Drawing.Size(64, 64);
            this.imageBox1.Image = global::Blumind.Properties.Resources.logo64;
            this.imageBox1.Location = new System.Drawing.Point(3, 8);
            this.imageBox1.Name = "imageBox1";
            this.tableLayoutPanel1.SetRowSpan(this.imageBox1, 2);
            this.imageBox1.Size = new System.Drawing.Size(64, 64);
            this.imageBox1.TabIndex = 0;
            this.imageBox1.Text = "Logo";
            this.imageBox1.ZoomType = Blumind.Controls.ZoomType.FitPage;
            // 
            // LabProductName
            // 
            this.LabProductName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LabProductName.AutoSize = true;
            this.LabProductName.Location = new System.Drawing.Point(194, 19);
            this.LabProductName.Name = "LabProductName";
            this.LabProductName.Size = new System.Drawing.Size(47, 12);
            this.LabProductName.TabIndex = 1;
            this.LabProductName.Text = "Blumind";
            // 
            // LabVersion
            // 
            this.LabVersion.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.LabVersion.AutoSize = true;
            this.LabVersion.Location = new System.Drawing.Point(194, 59);
            this.LabVersion.Name = "LabVersion";
            this.LabVersion.Size = new System.Drawing.Size(47, 12);
            this.LabVersion.TabIndex = 2;
            this.LabVersion.Text = "Version";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.imageBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LabVersion, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.LabProductName, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.LabAboutProduct, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(366, 278);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // LabAboutProduct
            // 
            this.LabAboutProduct.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.LabAboutProduct.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.LabAboutProduct, 2);
            this.LabAboutProduct.Location = new System.Drawing.Point(3, 169);
            this.LabAboutProduct.Name = "LabAboutProduct";
            this.LabAboutProduct.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.LabAboutProduct.Size = new System.Drawing.Size(119, 20);
            this.LabAboutProduct.TabIndex = 1;
            this.LabAboutProduct.Text = "Product Information";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.TxbEmail, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.LnkWebSite, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.LabHomePage, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.LabEMail, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.LabWechat, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.TxbTwitter, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.TxbWechat, 1, 3);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(12, 296);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(366, 120);
            this.tableLayoutPanel2.TabIndex = 4;
            // 
            // TxbEmail
            // 
            this.TxbEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TxbEmail.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TxbEmail.Location = new System.Drawing.Point(83, 25);
            this.TxbEmail.Name = "TxbEmail";
            this.TxbEmail.ReadOnly = true;
            this.TxbEmail.Size = new System.Drawing.Size(280, 14);
            this.TxbEmail.TabIndex = 0;
            // 
            // LnkWebSite
            // 
            this.LnkWebSite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.LnkWebSite.Location = new System.Drawing.Point(83, 0);
            this.LnkWebSite.Name = "LnkWebSite";
            this.LnkWebSite.Size = new System.Drawing.Size(280, 22);
            this.LnkWebSite.TabIndex = 5;
            this.LnkWebSite.Text = "Home Page";
            this.LnkWebSite.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LabHomePage
            // 
            this.LabHomePage.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.LabHomePage.AutoSize = true;
            this.LabHomePage.Location = new System.Drawing.Point(3, 5);
            this.LabHomePage.Name = "LabHomePage";
            this.LabHomePage.Size = new System.Drawing.Size(59, 12);
            this.LabHomePage.TabIndex = 12;
            this.LabHomePage.Text = "Home Page";
            // 
            // LabEMail
            // 
            this.LabEMail.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.LabEMail.AutoSize = true;
            this.LabEMail.Location = new System.Drawing.Point(3, 29);
            this.LabEMail.Name = "LabEMail";
            this.LabEMail.Size = new System.Drawing.Size(41, 12);
            this.LabEMail.TabIndex = 13;
            this.LabEMail.Text = "E-Mail";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 14;
            this.label1.Text = "Twitter";
            // 
            // LabWechat
            // 
            this.LabWechat.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.LabWechat.AutoSize = true;
            this.LabWechat.Location = new System.Drawing.Point(3, 83);
            this.LabWechat.Name = "LabWechat";
            this.LabWechat.Size = new System.Drawing.Size(41, 12);
            this.LabWechat.TabIndex = 15;
            this.LabWechat.Text = "Wechat";
            // 
            // BtnClose
            // 
            this.BtnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnClose.Location = new System.Drawing.Point(303, 422);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(75, 25);
            this.BtnClose.TabIndex = 5;
            this.BtnClose.Text = "Close";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // BtnDonation
            // 
            this.BtnDonation.Location = new System.Drawing.Point(12, 422);
            this.BtnDonation.Name = "BtnDonation";
            this.BtnDonation.Size = new System.Drawing.Size(75, 25);
            this.BtnDonation.TabIndex = 6;
            this.BtnDonation.Text = "Donation";
            this.BtnDonation.UseVisualStyleBackColor = true;
            this.BtnDonation.Click += new System.EventHandler(this.BtnDonation_Click);
            // 
            // TxbTwitter
            // 
            this.TxbTwitter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TxbTwitter.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TxbTwitter.Location = new System.Drawing.Point(83, 52);
            this.TxbTwitter.Name = "TxbTwitter";
            this.TxbTwitter.ReadOnly = true;
            this.TxbTwitter.Size = new System.Drawing.Size(280, 14);
            this.TxbTwitter.TabIndex = 16;
            this.TxbTwitter.Text = "@funway_wang";
            // 
            // TxbWechat
            // 
            this.TxbWechat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TxbWechat.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TxbWechat.Location = new System.Drawing.Point(83, 79);
            this.TxbWechat.Name = "TxbWechat";
            this.TxbWechat.ReadOnly = true;
            this.TxbWechat.Size = new System.Drawing.Size(280, 14);
            this.TxbWechat.TabIndex = 17;
            this.TxbWechat.Text = "funway_wang";
            // 
            // AboutDialogBox
            // 
            this.ClientSize = new System.Drawing.Size(390, 459);
            this.Controls.Add(this.BtnDonation);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "AboutDialogBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.ImageBox imageBox1;
        private System.Windows.Forms.Label LabProductName;
        private System.Windows.Forms.Label LabVersion;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TextBox TxbEmail;
        private System.Windows.Forms.Label LabAboutProduct;
        private Controls.HyperLink LnkWebSite;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.Label LabHomePage;
        private System.Windows.Forms.Label LabEMail;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button BtnDonation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label LabWechat;
        private System.Windows.Forms.TextBox TxbTwitter;
        private System.Windows.Forms.TextBox TxbWechat;
    }
}