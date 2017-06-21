namespace Blumind.Dialogs
{
    partial class InputDialog
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.LabMessage = new System.Windows.Forms.Label();
            this.TxbValue = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // LabMessage
            // 
            this.LabMessage.AutoSize = true;
            this.LabMessage.Location = new System.Drawing.Point(11, 8);
            this.LabMessage.Name = "LabMessage";
            this.LabMessage.Size = new System.Drawing.Size(59, 12);
            this.LabMessage.TabIndex = 0;
            this.LabMessage.Text = "[Message]";
            // 
            // TxbValue
            // 
            this.TxbValue.Location = new System.Drawing.Point(11, 35);
            this.TxbValue.Name = "TxbValue";
            this.TxbValue.Size = new System.Drawing.Size(381, 21);
            this.TxbValue.TabIndex = 1;
            this.TxbValue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxbValue_KeyDown);
            // 
            // InputDialog
            // 
            this.ClientSize = new System.Drawing.Size(400, 120);
            this.Controls.Add(this.LabMessage);
            this.Controls.Add(this.TxbValue);
            this.Name = "InputDialog";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Controls.SetChildIndex(this.TxbValue, 0);
            this.Controls.SetChildIndex(this.LabMessage, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LabMessage;
        private System.Windows.Forms.TextBox TxbValue;
    }
}
