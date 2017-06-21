using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Blumind.Core
{
    class ShortcutKey
    {
        private Keys _Keys;

        public ShortcutKey(string name, Keys keys)
            : this(name, keys, null, null)
        {
        }

        public ShortcutKey(string name, Keys keys, Image image)
            : this(name, keys, null, image)
        {
        }

        public ShortcutKey(string name, Keys keys, string description, Image image)
        {
            Name = name;
            Keys = keys;
            Description = description;
            Image = image;

            DefaultKeys = Keys;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public Image Image { get; set; }

        public Keys Keys
        {
            get { return _Keys; }
            set
            {
                if (_Keys != value)
                {
                    _Keys = value;
                    OnKeysChanged();
                }
            }
        }

        public Keys DefaultKeys { get; private set; }

        public bool Changed
        {
            get { return Keys != DefaultKeys; }
        }

        private void OnKeysChanged()
        {
        }

        public string KeysToString()
        {
            return ST.ToString(Keys, "+");
        }

        public override string ToString()
        {
            return ST.ToString(Keys);
        }

        public bool Test(Keys keys)
        {
            if (Keys == Keys.None)
                return false;

            //return keys == Keys || (Keys2 != Keys.None && keys == Keys2);
            if (keys == Keys)
                return true;

            Keys key = keys & Keys.KeyCode;
            switch (key)
            {
                case System.Windows.Forms.Keys.Oemplus:
                    keys = keys & System.Windows.Forms.Keys.Modifiers | System.Windows.Forms.Keys.Add;
                    break;
                case System.Windows.Forms.Keys.Add:
                    keys = keys & System.Windows.Forms.Keys.Modifiers | System.Windows.Forms.Keys.Oemplus;
                    break;
                case System.Windows.Forms.Keys.OemMinus:
                    keys = keys & System.Windows.Forms.Keys.Modifiers | System.Windows.Forms.Keys.Subtract;
                    break;
                case System.Windows.Forms.Keys.Subtract:
                    keys = keys & System.Windows.Forms.Keys.Modifiers | System.Windows.Forms.Keys.OemMinus;
                    break;
                case System.Windows.Forms.Keys.D8:
                    keys = keys & System.Windows.Forms.Keys.Modifiers | System.Windows.Forms.Keys.Multiply;
                    break;
                case System.Windows.Forms.Keys.Multiply:
                    keys = keys & System.Windows.Forms.Keys.Modifiers | System.Windows.Forms.Keys.D8;
                    break;
                default:
                    return false;
            }

            return keys == Keys;
        }

        public void Clear()
        {
            Keys = System.Windows.Forms.Keys.None;
        }

        public void Reset()
        {
            Keys = DefaultKeys;
        }
    }
}
