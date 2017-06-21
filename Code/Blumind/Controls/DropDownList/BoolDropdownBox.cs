using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Blumind.Core;
using System.ComponentModel;
using Blumind.Globalization;

namespace Blumind.Controls
{
    [DefaultEvent("ValueChanged")]
    class BoolDropdownBox : ComboBox
    {
        bool _Value;

        public event EventHandler ValueChanged;

        public BoolDropdownBox()
        {
            DropDownStyle = ComboBoxStyle.DropDownList;

            LanguageManage.CurrentChanged += new EventHandler(LanguageManage_CurrentChanged);
        }

        void LanguageManage_CurrentChanged(object sender, EventArgs e)
        {
            if (Items.Count >= 2)
            {
                Items[0] = Lang._("Yes");
                Items[1] = Lang._("NO");
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (!DesignMode)
            {
                Items.Add(Lang._("Yes"));
                Items.Add(Lang._("NO"));

                OnValueChanged();
            }
        }

        [DefaultValue(null)]
        public bool Value
        {
            get { return _Value; }
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
            if(Items.Count > 0)
            {
                if (Value)
                    SelectedIndex = 0;
                else
                    SelectedIndex = 1;
            }

            if (ValueChanged != null)
                ValueChanged(this, EventArgs.Empty);             
        }
    }
}
