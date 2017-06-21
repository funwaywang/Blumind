using System.ComponentModel;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Dialogs
{
    partial class ModifyShortcutsDialog : StandardDialog
    {
        private ShortcutKey _ShortcutKeys;

        public ModifyShortcutsDialog()
        {
            InitializeComponent();

            Icon = Properties.Resources.keyboard_icon;
            ShowIcon = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;

            AfterInitialize();
        }

        [Browsable(false)]
        public Keys NewValue { get; private set; }

        public ShortcutKey ShortcutKeys
        {
            get { return _ShortcutKeys; }
            set 
            {
                if (_ShortcutKeys != value)
                {
                    _ShortcutKeys = value;
                    OnAcceleratorKeysChanged();
                }
            }
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            Text = Lang._("Modify Shortcuts");
            label1.Text = Lang._("Operation");
            OnAcceleratorKeysChanged();
        }

        protected virtual void OnAcceleratorKeysChanged()
        {
            if (ShortcutKeys != null)
            {
                LabOperation.Text = Lang._(ShortcutKeys.Name);
                if (!string.IsNullOrEmpty(ShortcutKeys.Description))
                    LabOperation.Text += Lang.Format(" ({0})", ShortcutKeys.Description);
                this.shortcutKeysEditor1.Value = ShortcutKeys.Keys;
            }
            else
            {
                LabOperation.Text = string.Empty;
                this.shortcutKeysEditor1.Value = Keys.None;
            }
        }

        protected override bool OnOKButtonClick()
        {
            NewValue = shortcutKeysEditor1.Value;
            if ((NewValue & Keys.KeyCode) == Keys.None)
            {
                NewValue = Keys.None;
            }

            return base.OnOKButtonClick();
        }
    }
}
