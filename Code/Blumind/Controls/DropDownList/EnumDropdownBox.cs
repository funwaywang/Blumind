using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using Blumind.Configuration;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Controls
{
    class EnumDropdownBox<T> : ComboBox
        where T : struct
    {
        T? _Value;

        public event EventHandler ValueChanged;

        public EnumDropdownBox()
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

            if (!DesignMode && Items.Count == 0)
            {
                RefreshList();
            }
        }

        void RefreshList()
        {
            Items.Clear();
            foreach (T value in Enum.GetValues(typeof(T)))
            {
                Items.Add(new ListItem<T>(ST.EnumToString<T>(value), value));
            }
        }

        [DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public T? Value
        {
            get { return _Value; }
            set
            {
                if (!_Value.Equals(value))
                {
                    _Value = value;
                    OnValueChanged();
                }
            }
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (SelectedItem is ListItem<T>)
                Value = ((ListItem<T>)SelectedItem).Value;
            else if (SelectedItem is T)
                Value = (T)SelectedItem;
            else
                Value = default(T);

            base.OnSelectedIndexChanged(e);
        }

        protected virtual void OnValueChanged()
        {
            if (!Value.HasValue)
                SelectedIndex = -1;
            else if (Items.Count > 0)
                ListItem.Select<T>(this, Value.Value);

            if (ValueChanged != null)
                ValueChanged(this, EventArgs.Empty);
        }
    }

    class SaveTabsDropdownBox : EnumDropdownBox<SaveTabsType>
    {
    }
}
