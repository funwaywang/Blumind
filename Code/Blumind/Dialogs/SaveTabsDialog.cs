using System;
using System.ComponentModel;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Dialogs
{
    partial class SaveTabsDialog : BaseDialog
    {
        private bool _DoNotAskAgain;

        public SaveTabsDialog()
        {
            InitializeComponent();

            AfterInitialize();
        }

        [DefaultValue(false)]
        public bool DoNotAskAgain
        {
            get { return _DoNotAskAgain; }
            set
            {
                if (_DoNotAskAgain != value)
                {
                    _DoNotAskAgain = value;
                    OnDoNotAskAgainChanged();
                }
            }
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            Text = Lang._("Save tabs");
            BtnYes.Text = Lang._("Yes");
            BtnNo.Text = Lang._("No");
            BtnCancel.Text = Lang._("Cancel");
            CkbDoNotAskMeAgain.Text = Lang._("Do not ask me again");
            LabMessage.Text = Lang._("Do you want Blumind save tabs and open them next startup?");
        }

        private void OnDoNotAskAgainChanged()
        {
            CkbDoNotAskMeAgain.Checked = DoNotAskAgain;
        }

        private void BtnNo_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }

        private void BtnYes_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void CkbDoNotAskMeAgain_CheckedChanged(object sender, EventArgs e)
        {
            DoNotAskAgain = CkbDoNotAskMeAgain.Checked;
        }
    }
}
