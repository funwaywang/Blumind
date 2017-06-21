using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Controls
{
    class ExceptionControl : Control
    {
        TextBox textBox1;
        LinkLabel linkCopy, linkSave;
        Exception _Exception;

        public ExceptionControl()
        {
            InitializeComponent();
        }

        [DefaultValue(null)]
        public Exception Exception
        {
            get { return _Exception; }
            set 
            {
                if (_Exception != value)
                {
                    _Exception = value;
                    OnExceptionChanged();
                }
            }
        }

        private void InitializeComponent()
        {
            textBox1 = new TextBox();
            linkCopy = new LinkLabel();
            linkSave = new LinkLabel();

            // textBox1
            textBox1.Multiline = true;
            textBox1.ReadOnly = true;
            textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            textBox1.WordWrap = false;
            textBox1.BackColor = SystemColors.Window;
            textBox1.ForeColor = SystemColors.WindowText;

            // linkSave
            linkSave.Enabled = false;
            linkSave.Text = "Save...";
            linkSave.AutoSize = true;
            linkSave.LinkClicked += new LinkLabelLinkClickedEventHandler(linkSave_LinkClicked);

            // BtnCopy
            // 
            linkCopy.Enabled = false;
            linkCopy.Text = "Copy";
            linkCopy.AutoSize = true;
            linkCopy.LinkClicked += new LinkLabelLinkClickedEventHandler(linkCopy_LinkClicked);

            //
            Controls.AddRange(new Control[] {
                linkSave,
                linkCopy,
                textBox1
            });
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            ResetControlBounds();
        }

        void OnExceptionChanged()
        {
            if (Exception != null)
            {
                textBox1.Text = Exception.Message
                    + System.Environment.NewLine
                    + new string('-', 60)
                    + System.Environment.NewLine
                    + Exception.GetType().FullName
                    + System.Environment.NewLine
                    + new string('-', 60)
                    + System.Environment.NewLine
                    + Exception.StackTrace;

                linkCopy.Enabled = true;
                linkSave.Enabled = true;
            }
            else
            {
                textBox1.Text = null;

                linkCopy.Enabled = false;
                linkSave.Enabled = false;
            }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            ResetControlBounds();
        }

        void linkCopy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                Clipboard.SetText(textBox1.Text);
            }
        }

        void linkSave_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
                return;

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = string.Format("{0} (*.log)|*.log", Lang._("Exception Log File"));
            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(dialog.FileName))
                {
                    sw.Write(textBox1.Text);
                }
            }
        }

        void ResetControlBounds()
        {
            if (linkCopy == null || linkSave == null || textBox1 == null)
                return;

            Rectangle rect = ClientRectangle;
            rect.X += Padding.Left;
            rect.Y += Padding.Top;
            rect.Width -= Padding.Horizontal;
            rect.Height -= Padding.Vertical;

            //
            int linkHeight = Math.Max(linkCopy.Height, linkSave.Height);

            //
            linkCopy.Location = new Point(rect.X + 4, rect.Bottom - 4 - linkHeight);

            //
            linkSave.Location = new Point(linkCopy.Right + 24, rect.Bottom - 4 - linkHeight);

            //
            textBox1.Bounds = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height - linkHeight - 8);
        }
    }
}
