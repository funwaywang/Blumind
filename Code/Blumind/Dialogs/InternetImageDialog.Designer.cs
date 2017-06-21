namespace Blumind.Dialogs
{
    partial class InternetImageDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.CmbUrl = new System.Windows.Forms.ComboBox();
            this.CkbAddToLibrary = new System.Windows.Forms.CheckBox();
            this.PicPreview = new Blumind.Controls.ImageBox();
            this.LabInfo = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.CkbLimitImageSize = new System.Windows.Forms.CheckBox();
            this.CkbEmbedIn = new System.Windows.Forms.CheckBox();
            this.BtnPreview = new System.Windows.Forms.Button();
            this.BtnRecommendedResources = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "URL:";
            // 
            // CmbUrl
            // 
            this.CmbUrl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.CmbUrl, 2);
            this.CmbUrl.Location = new System.Drawing.Point(38, 3);
            this.CmbUrl.Name = "CmbUrl";
            this.CmbUrl.Size = new System.Drawing.Size(408, 20);
            this.CmbUrl.TabIndex = 1;
            this.CmbUrl.TextChanged += new System.EventHandler(this.CmbUrl_TextChanged);
            // 
            // CkbAddToLibrary
            // 
            this.CkbAddToLibrary.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CkbAddToLibrary.AutoSize = true;
            this.CkbAddToLibrary.Location = new System.Drawing.Point(38, 29);
            this.CkbAddToLibrary.Name = "CkbAddToLibrary";
            this.CkbAddToLibrary.Size = new System.Drawing.Size(156, 16);
            this.CkbAddToLibrary.TabIndex = 4;
            this.CkbAddToLibrary.Text = "Add To My Icon Library";
            this.CkbAddToLibrary.UseVisualStyleBackColor = true;
            // 
            // PicPreview
            // 
            this.PicPreview.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PicPreview.Location = new System.Drawing.Point(292, 52);
            this.PicPreview.Name = "PicPreview";
            this.tableLayoutPanel1.SetRowSpan(this.PicPreview, 4);
            this.PicPreview.Size = new System.Drawing.Size(61, 59);
            this.PicPreview.TabIndex = 10006;
            this.PicPreview.TabStop = false;
            this.PicPreview.ZoomType = Blumind.Controls.ZoomType.FitPage;
            // 
            // LabInfo
            // 
            this.LabInfo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.LabInfo.AutoSize = true;
            this.LabInfo.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.LabInfo.Location = new System.Drawing.Point(38, 109);
            this.LabInfo.Name = "LabInfo";
            this.LabInfo.Size = new System.Drawing.Size(71, 12);
            this.LabInfo.TabIndex = 10007;
            this.LabInfo.Text = "Information";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.CkbLimitImageSize, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.CmbUrl, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.CkbAddToLibrary, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.LabInfo, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.CkbEmbedIn, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.PicPreview, 2, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(11, 10);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(449, 138);
            this.tableLayoutPanel1.TabIndex = 10008;
            // 
            // CkbLimitImageSize
            // 
            this.CkbLimitImageSize.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CkbLimitImageSize.AutoSize = true;
            this.CkbLimitImageSize.Checked = true;
            this.CkbLimitImageSize.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CkbLimitImageSize.Location = new System.Drawing.Point(38, 51);
            this.CkbLimitImageSize.Name = "CkbLimitImageSize";
            this.CkbLimitImageSize.Size = new System.Drawing.Size(120, 16);
            this.CkbLimitImageSize.TabIndex = 10010;
            this.CkbLimitImageSize.Text = "Limit Image Size";
            this.CkbLimitImageSize.UseVisualStyleBackColor = true;
            // 
            // CkbEmbedIn
            // 
            this.CkbEmbedIn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CkbEmbedIn.AutoSize = true;
            this.CkbEmbedIn.Location = new System.Drawing.Point(38, 73);
            this.CkbEmbedIn.Name = "CkbEmbedIn";
            this.CkbEmbedIn.Size = new System.Drawing.Size(72, 16);
            this.CkbEmbedIn.TabIndex = 10011;
            this.CkbEmbedIn.Text = "Embed In";
            this.CkbEmbedIn.CheckedChanged += new System.EventHandler(this.CkbEmbedIn_CheckedChanged);
            // 
            // BtnPreview
            // 
            this.BtnPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnPreview.Enabled = false;
            this.BtnPreview.Location = new System.Drawing.Point(187, 168);
            this.BtnPreview.Name = "BtnPreview";
            this.BtnPreview.Size = new System.Drawing.Size(75, 25);
            this.BtnPreview.TabIndex = 10008;
            this.BtnPreview.Text = "Preview";
            this.BtnPreview.UseVisualStyleBackColor = true;
            this.BtnPreview.Click += new System.EventHandler(this.BtnPreview_Click);
            // 
            // BtnRecommendedResources
            // 
            this.BtnRecommendedResources.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnRecommendedResources.Enabled = false;
            this.BtnRecommendedResources.Location = new System.Drawing.Point(11, 168);
            this.BtnRecommendedResources.Name = "BtnRecommendedResources";
            this.BtnRecommendedResources.Size = new System.Drawing.Size(170, 25);
            this.BtnRecommendedResources.TabIndex = 10009;
            this.BtnRecommendedResources.Text = "Recommended Resources";
            this.BtnRecommendedResources.UseVisualStyleBackColor = true;
            this.BtnRecommendedResources.Click += new System.EventHandler(this.BtnRecommendedResources_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // InternetImageDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 205);
            this.Controls.Add(this.BtnRecommendedResources);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.BtnPreview);
            this.Name = "InternetImageDialog";
            this.Padding = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Text = "Internet Image Dialog";
            this.Controls.SetChildIndex(this.BtnPreview, 0);
            this.Controls.SetChildIndex(this.tableLayoutPanel1, 0);
            this.Controls.SetChildIndex(this.BtnRecommendedResources, 0);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox CmbUrl;
        private System.Windows.Forms.CheckBox CkbAddToLibrary;
        private Blumind.Controls.ImageBox PicPreview;
        private System.Windows.Forms.Label LabInfo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button BtnPreview;
        private System.Windows.Forms.Button BtnRecommendedResources;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.CheckBox CkbLimitImageSize;
        private System.Windows.Forms.CheckBox CkbEmbedIn;
    }
}