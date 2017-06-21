namespace Blumind.Core
{
    partial class ShortcutKeysEditor
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lblModifiers = new System.Windows.Forms.Label();
            this.chkCtrl = new System.Windows.Forms.CheckBox();
            this.chkShift = new System.Windows.Forms.CheckBox();
            this.chkAlt = new System.Windows.Forms.CheckBox();
            this.lblKey = new System.Windows.Forms.Label();
            this.cmbKey = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lblModifiers
            // 
            this.lblModifiers.AutoSize = true;
            this.lblModifiers.Location = new System.Drawing.Point(7, 4);
            this.lblModifiers.Name = "lblModifiers";
            this.lblModifiers.Size = new System.Drawing.Size(59, 12);
            this.lblModifiers.TabIndex = 0;
            this.lblModifiers.Text = "Modifiers";
            // 
            // chkCtrl
            // 
            this.chkCtrl.AutoSize = true;
            this.chkCtrl.Location = new System.Drawing.Point(26, 19);
            this.chkCtrl.Name = "chkCtrl";
            this.chkCtrl.Size = new System.Drawing.Size(48, 16);
            this.chkCtrl.TabIndex = 1;
            this.chkCtrl.Text = "Ctrl";
            this.chkCtrl.CheckedChanged += new System.EventHandler(this.chkModifier_CheckedChanged);
            // 
            // chkShift
            // 
            this.chkShift.AutoSize = true;
            this.chkShift.Location = new System.Drawing.Point(26, 39);
            this.chkShift.Name = "chkShift";
            this.chkShift.Size = new System.Drawing.Size(54, 16);
            this.chkShift.TabIndex = 2;
            this.chkShift.Text = "Shift";
            this.chkShift.CheckedChanged += new System.EventHandler(this.chkModifier_CheckedChanged);
            // 
            // chkAlt
            // 
            this.chkAlt.AutoSize = true;
            this.chkAlt.Location = new System.Drawing.Point(26, 59);
            this.chkAlt.Name = "chkAlt";
            this.chkAlt.Size = new System.Drawing.Size(42, 16);
            this.chkAlt.TabIndex = 3;
            this.chkAlt.Text = "Alt";
            this.chkAlt.CheckedChanged += new System.EventHandler(this.chkModifier_CheckedChanged);
            // 
            // lblKey
            // 
            this.lblKey.Location = new System.Drawing.Point(7, 76);
            this.lblKey.Name = "lblKey";
            this.lblKey.Size = new System.Drawing.Size(100, 20);
            this.lblKey.TabIndex = 0;
            this.lblKey.Text = "Key";
            // 
            // cmbKey
            // 
            this.cmbKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbKey.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbKey.Location = new System.Drawing.Point(26, 99);
            this.cmbKey.Name = "cmbKey";
            this.cmbKey.Size = new System.Drawing.Size(177, 20);
            this.cmbKey.TabIndex = 1;
            this.cmbKey.SelectedIndexChanged += new System.EventHandler(this.cmbKey_SelectedIndexChanged);
            // 
            // ShortcutKeysEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblModifiers);
            this.Controls.Add(this.lblKey);
            this.Controls.Add(this.chkCtrl);
            this.Controls.Add(this.chkShift);
            this.Controls.Add(this.cmbKey);
            this.Controls.Add(this.chkAlt);
            this.Name = "ShortcutKeysEditor";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.Size = new System.Drawing.Size(210, 126);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkAlt;
        private System.Windows.Forms.CheckBox chkCtrl;
        private System.Windows.Forms.CheckBox chkShift;
        private System.Windows.Forms.ComboBox cmbKey;
        private System.Windows.Forms.Label lblKey;
        private System.Windows.Forms.Label lblModifiers;
    }
}
