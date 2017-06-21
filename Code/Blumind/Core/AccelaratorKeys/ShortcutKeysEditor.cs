using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Blumind.Globalization;

namespace Blumind.Core
{
    partial class ShortcutKeysEditor : UserControl
    {
        private Keys _Value;
        private bool updateCurrentValue;

        public ShortcutKeysEditor()
        {
            InitializeComponent();

            // init keys
            foreach (Keys keys in KeyMap.ValidKeys)
            {
                this.cmbKey.Items.Add(ST.ToString(keys));
            }

            LanguageManage.CurrentChanged += new EventHandler(LanguageManage_CurrentChanged);
            OnCurrentLanguageChanged();
        }

        [DefaultValue(Keys.None)]
        public Keys Value
        {
            get
            {
                return this._Value;
            }

            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    OnValueChanged();
                }
            }
        }

        protected virtual void OnValueChanged()
        {
            chkCtrl.Checked = (Value & Keys.Control) != Keys.None;
            chkAlt.Checked = (Value & Keys.Alt) != Keys.None;
            chkShift.Checked = (Value & Keys.Shift) != Keys.None;

            Keys keyCode = Value & Keys.KeyCode;
            if (keyCode == Keys.None)
            {
                cmbKey.SelectedIndex = -1;
            }
            else if (KeyMap.IsValidKey(keyCode))
            {
                cmbKey.SelectedItem = ST.ToString(keyCode);
            }
            else
            {
                cmbKey.SelectedIndex = -1;
            }
            this.updateCurrentValue = true;
        }

        protected virtual void OnCurrentLanguageChanged()
        {
            lblKey.Text = Lang._("Key");
            lblModifiers.Text = Lang._("Modifiers");

            chkCtrl.Text = Lang._("Ctrl");
            chkShift.Text = Lang._("Shift");
            chkAlt.Text = Lang._("Alt");
        }

        private void chkModifier_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateCurrentValue();
        }

        private void cmbKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdateCurrentValue();
        }

        private void LanguageManage_CurrentChanged(object sender, EventArgs e)
        {
            OnCurrentLanguageChanged();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            this.chkCtrl.Focus();
        }

        private void UpdateCurrentValue()
        {
            if (this.updateCurrentValue)
            {
                int selectedIndex = this.cmbKey.SelectedIndex;
                Keys value = Keys.None;
                if (chkCtrl.Checked)
                {
                    value |= Keys.Control;
                }
                if (chkAlt.Checked)
                {
                    value |= Keys.Alt;
                }
                if (chkShift.Checked)
                {
                    value |= Keys.Shift;
                }

                if (selectedIndex != -1)
                {
                    value |= KeyMap.ValidKeys[selectedIndex];
                }
                this._Value = value;
            }
        }
    }
}
