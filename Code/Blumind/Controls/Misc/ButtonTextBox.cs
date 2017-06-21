using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Blumind.Controls
{
    class ButtonTextBox : Control
    {
        private TextBox _TextBox;
        private Button _Button;

        public ButtonTextBox()
        {
            TextBox = new TextBox();
            TextBox.Dock = DockStyle.Fill;

            Button = new Button();
            Button.Text = "...";
            Button.Size = new Size(30, TextBox.Height);
            Button.Dock = DockStyle.Right;

            Controls.Add(TextBox);
            Controls.Add(Button);
        }

        public TextBox TextBox
        {
            get { return _TextBox; }
            private set { _TextBox = value; }
        }

        public Button Button
        {
            get { return _Button; }
            private set { _Button = value; }
        }

        //protected override void OnLayout(LayoutEventArgs e)
        //{
        //    base.OnLayout(e);

        //    Rectangle rect = ClientRectangle;
        //    Button.Location = new Point(rect.Right - Button.Width, rect.Top + (rect.Height - Button.Height) / 2);
        //    TextBox.Location = new Point(rect.Left, rect.Top + (rect.Height - TextBox.Height) / 2);
        //    TextBox.Width = rect.Width - Button.Width;
        //}
    }
}
