using System;
using System.ComponentModel;
using System.Drawing;
using Blumind.Controls;

namespace Blumind.Dialogs
{
    internal partial class InputDialog : Blumind.Controls.StandardDialog
    {
        private bool _AllowEmpty = false;

        public InputDialog()
        {
            InitializeComponent();
            LabMessage.Text = string.Empty;

            AfterInitialize();
        }

        public InputDialog(string text, string message)
            : this()
        {
            Text = text;
            Message = message;
        }

        [DefaultValue(null)]
        public string Message
        {
            get { return LabMessage.Text; }
            set { LabMessage.Text = value; }
        }

        [DefaultValue(null)]
        public string Value
        {
            get { return TxbValue.Text; }
            set { TxbValue.Text = value; }
        }

        [DefaultValue(false)]
        public bool AllowEmpty
        {
            get { return _AllowEmpty; }
            set { _AllowEmpty = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ActiveControl = TxbValue;
            if(TxbValue.CanFocus)
                TxbValue.Focus();
            TxbValue.SelectAll();
            this.SetFontNotScale(SystemFonts.MessageBoxFont);
        }

        protected override bool OnOKButtonClick()
        {
            if (!AllowEmpty && Value.Trim() == string.Empty)
                return false;

            return base.OnOKButtonClick();
        }

        private void TxbValue_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                OnOKButtonClick();
                e.SuppressKeyPress = true;
                return;
            }
        }
    }
}
