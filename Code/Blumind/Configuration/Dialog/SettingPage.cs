using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using Blumind.Configuration.Models;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Configuration.Dialog
{
    class SettingPage : UserControl, IThemableUI, IModifyObject
    {
        bool langInited;
        bool themeInited;
        List<string> OptionNames;

        public SettingPage()
        {
            OptionNames = new List<string>();
            Options = new MemoryOptions(Options.Current);
            Options.ModifiedChanged += Options_ModifiedChanged;

            LanguageManage.CurrentChanged += new EventHandler(LanguageManage_CurrentChanged);
            UITheme.Default.Listeners.Add(this);
        }

        public string LanguageID { get; set; }

        protected Options Options { get; private set; }

        bool ChangesSuspended { get; set; }

        #region Get/Set
        void LogOptionName(string key)
        {
            if (!OptionNames.Contains(key))
                OptionNames.Add(key);
        }

        protected T GetValue<T>(string key)
        {
            LogOptionName(key);
            return Options.GetValue<T>(key);
        }

        protected T GetValue<T>(string key, T defaultValue)
        {
            LogOptionName(key);
            return Options.GetValue<T>(key, defaultValue);
        }

        protected bool GetBool(string key)
        {
            LogOptionName(key);
            return Options.GetBool(key);
        }

        protected string GetString(string key)
        {
            LogOptionName(key);
            return Options.GetString(key);
        }

        protected int GetInt(string key)
        {
            LogOptionName(key);
            return Options.GetInt(key);
        }

        protected void SetValue(string key, object value)
        {
            if (!Created)
                return;

            if (!ChangesSuspended)
            {
                LogOptionName(key);
                Options.SetValue(key, value);
            }
        }
        #endregion

        void Options_ModifiedChanged(object sender, EventArgs e)
        {
            if (Options != null)
            {
                Modified = Options.Modified;
            }
        }

        void LanguageManage_CurrentChanged(object sender, EventArgs e)
        {
            OnCurrentLanguageChanged();
        }

        protected virtual void OnCurrentLanguageChanged()
        {
            langInited = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
            {
                if (!langInited)
                {
                    OnCurrentLanguageChanged();
                }

                if (!themeInited)
                {
                    ApplyTheme(UITheme.Default);
                }

                SuspendChanges();
                PutSettings();
                ResumeChanges();
                Options.Modified = false;
            }
        }

        protected virtual void PutSettings()
        {
        }

        public void CommitSettings()
        {
            if (Options != null)
            {
                var options = Options.Current;
                foreach (var key in Options.Changes)
                {
                    options.SetValue(key, Options.GetValue(key));
                }

                Options.Modified = false;
            }
        }

        public void ResetSettings()
        {
            if (Options != null)
            {
                Options.Clear();
                Options.AcceptChanges();

                foreach (var key in OptionNames)
                {
                    Options.SetValue(key, Options.DefaultValue.GetValue(key));
                }

                SuspendChanges();
                PutSettings();
                ResumeChanges();
            }
        }

        public virtual void ApplyTheme(UITheme theme)
        {
            if (!DesignMode && theme != null)
            {
                Font = theme.DefaultFont;
            }

            themeInited = true;
        }

        void SuspendChanges()
        {
            ChangesSuspended = true;
        }

        void ResumeChanges()
        {
            ChangesSuspended = false;
        }

        #region IModifyObject
        bool _Modified;

        public event EventHandler ModifiedChanged;

        public bool Modified
        {
            get { return _Modified; }
            set 
            {
                if (_Modified != value)
                {
                    _Modified = value;
                    OnModifiedChanged();
                }
            }
        }

        protected virtual void OnModifiedChanged()
        {
            if (ModifiedChanged != null)
                ModifiedChanged(this, EventArgs.Empty);
        }
        #endregion
    }
}
