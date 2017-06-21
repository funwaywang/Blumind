using System;
using Blumind.Controls;
using Blumind.Core.Settings;
using System.Drawing;

namespace Blumind
{
    class OptionsDialog : PropertyDialog
    {
        public OptionsDialog()
        {
            SelectedObject = _Options.Current;
            Size = new Size(360, 480);
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            if (CurrentLanguage != null)
            {
                Text = CurrentLanguage["Options"];
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _Options.Current.BeginEdit();
        }

        protected override bool OnOKButtonClick()
        {
            _Options.Current.EndEdit();

            _Options.Current.InvokeChanged();

            return base.OnOKButtonClick();
        }

        protected override bool OnCancelButtonClick()
        {
            _Options.Current.CancelEdit();

            return base.OnCancelButtonClick();
        }
    }
}
