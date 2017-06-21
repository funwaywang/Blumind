using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;

namespace Blumind.Core
{
    [DefaultEvent("KeyDown")]
    class HotKeyMap : Component, IMessageFilter, IDisposable
    {
        private bool _Enabled = true;
        private Hashtable KeyMap = new Hashtable();

        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;

        public HotKeyMap()
        {
            if (!DesignMode && Enabled)
            {
                RegisterMessageFilter();
            }
        }

        [DefaultValue(true)]
        public bool Enabled
        {
            get { return _Enabled; }
            set
            {
                if (_Enabled != value)
                {
                    _Enabled = value;
                    OnEnabledChanged();
                }
            }
        }

        private void OnEnabledChanged()
        {
            if (Enabled)
                RegisterMessageFilter();
            else
                UnregisterMessageFilter();
        }

        private void RegisterMessageFilter()
        {
            Application.AddMessageFilter(this);
        }

        private void UnregisterMessageFilter()
        {
            Application.RemoveMessageFilter(this);
        }

        public bool ContainsKey(Keys key)
        {
            return KeyMap.ContainsKey(key);
        }

        public bool RegisterHotKey(Keys key, KeyEventHandler callback)
        {
            if (KeyMap.ContainsKey(key))
                return false;

            KeyMap.Add(key, callback);
            return true;
        }

        public bool UnregisterHotKey(Keys key, KeyEventHandler callback)
        {
            if (!KeyMap.ContainsKey(key))
                return false;

            KeyMap.Remove(key);
            return true;
        }

        #region IMessageFilter

        public bool PreFilterMessage(ref Message m)
        {
            const int WM_KEYDOWN = 0x0100;
            const int WM_KEYUP = 0x0101;

            if (m.Msg == WM_KEYDOWN)
            {
                Keys key = (Keys)m.WParam.ToInt32() | Control.ModifierKeys;
                if (KeyMap.ContainsKey(key) || KeyDown != null)
                {
                    KeyEventArgs e = new KeyEventArgs(key);

                    if (KeyDown != null)
                    {
                        KeyDown(this, e);
                        if (e.SuppressKeyPress)
                            return true;
                    }

                    if (KeyMap.ContainsKey(key))
                    {
                        ((KeyEventHandler)KeyMap[key]).Invoke(this, e);
                        if (e.SuppressKeyPress)
                            return true;
                    }
                }
            }
            else if (m.Msg == WM_KEYUP)
            {
                if (KeyUp != null)
                {
                    Keys key = (Keys)m.WParam.ToInt32() | Control.ModifierKeys;
                    KeyEventArgs e = new KeyEventArgs(key);
                    KeyUp(this, e);
                    if (e.SuppressKeyPress)
                        return true;
                }
            }

            return false;
        }

        #endregion

        #region IDisposable

        void IDisposable.Dispose()
        {
            UnregisterMessageFilter();
        }

        #endregion
    }
}
