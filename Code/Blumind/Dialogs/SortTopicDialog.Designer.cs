namespace Blumind.Dialogs
{
    partial class SortTopicDialog
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
            this.sortBox1 = new Blumind.Controls.SortBox();
            this.SuspendLayout();
            // 
            // sortBox1
            // 
            this.sortBox1.Items = new object[0];
            this.sortBox1.Location = new System.Drawing.Point(11, 10);
            this.sortBox1.Name = "sortBox1";
            this.sortBox1.Size = new System.Drawing.Size(309, 324);
            this.sortBox1.TabIndex = 0;
            // 
            // SortTopicDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 375);
            this.Controls.Add(this.sortBox1);
            this.Name = "SortTopicDialog";
            this.Text = "Sort Topics";
            this.Controls.SetChildIndex(this.sortBox1, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private Blumind.Controls.SortBox sortBox1;
    }
}