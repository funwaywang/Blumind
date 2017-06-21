using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using Blumind.Core;
using Blumind.Model;

namespace Blumind.Controls
{
    class BaseControl : Control, IThemableUI
    {
        bool UIThemeInited;

        public BaseControl()
        {
            UITheme.Default.Listeners.Add(this);
        }

        #region IMultiLang Members
        private string _LangID;
        public event System.EventHandler LangIDChanged;

        [DefaultValue(null), Browsable(false)]
        public string LangID
        {
            get { return _LangID; }
            set
            {
                if (_LangID != value)
                {
                    _LangID = value;
                    OnLangIDChanged();
                }
            }
        }

        protected virtual void OnLangIDChanged()
        {
            if (LangIDChanged != null)
                LangIDChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Update View
        private int UpdateCount = 0;
        private ChangeTypes UpdateTypes = ChangeTypes.None;
        public event EventHandler ViewUpdated;

        [Browsable(false)]
        public bool IsUpdating
        {
            get { return UpdateCount > 0; }
        }

        public void BeginUpdateView()
        {
            UpdateCount = Math.Max(UpdateCount, 0) + 1;
        }

        public void EndUpdateView(ChangeTypes ut)
        {
            UpdateTypes |= ut;
            UpdateCount = Math.Max(0, UpdateCount - 1);
            if (UpdateCount == 0)
                UpdateView(UpdateTypes);
        }

        public void CancelUpdateView()
        {
            UpdateCount = Math.Max(0, UpdateCount - 1);
        }

        public void TryUpdate(ChangeTypes ut)
        {
            UpdateTypes |= ut;
            UpdateCount = Math.Max(0, UpdateCount - 1);
            if (UpdateCount == 0)
                UpdateView(ut);
            else
                UpdateCount = Math.Max(UpdateCount, 0) + 1;
        }

        public virtual void UpdateView(ChangeTypes ut)
        {
            if (ViewUpdated != null)
                ViewUpdated(this, EventArgs.Empty);
        }
        #endregion

        #region Full Screen
        private bool _FullScreen = false;

        public event System.EventHandler FullScreenChanged;

        [Browsable(false), DefaultValue(false)]
        public bool FullScreen
        {
            get { return _FullScreen; }
            set
            {
                if (_FullScreen != value)
                {
                    if (_FullScreen)
                        this.ExitFullScreen();
                    else
                        this.EnterFullScreen();

                    _FullScreen = value;
                    this.OnFullScreenChanged();
                }
            }
        }

        private void OnFullScreenChanged()
        {
            if (FullScreenChanged != null)
                this.FullScreenChanged(this, EventArgs.Empty);
        }

        public void EnterFullScreen()
        {
            if (this.FullScreen)
                return;
            ControlFullScreenShell.EnterFullScreen(this);
        }

        public void ExitFullScreen()
        {
            if (!this.FullScreen)
                return;
            ControlFullScreenShell.ExitFullScreen(this);
        }
        #endregion

        protected override Padding DefaultPadding
        {
            get
            {
                return new Padding(0);
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (!DesignMode && !UIThemeInited)
            {
                ApplyTheme(UITheme.Default);
                UIThemeInited = true;
            }
        }

        protected void SetPaintStyles()
        {
            SetStyle(ControlStyles.UserPaint |
                   ControlStyles.AllPaintingInWmPaint |
                   ControlStyles.OptimizedDoubleBuffer |
                   ControlStyles.ResizeRedraw, true);
        }

        public static string GetControlPath(Control ctl)
        {
            StringBuilder sb = new StringBuilder();
            GetControlPath(ctl, sb);
            return sb.ToString();
        }

        private static void GetControlPath(Control ctl, StringBuilder sb)
        {
            if (ctl.Parent != null)
                GetControlPath(ctl.Parent, sb);

            if (sb.Length > 0)
                sb.Append(".");
            sb.Append(ctl.Name);
        }

        #region IThemableUI
        public virtual void ApplyTheme(UITheme theme)
        {
        }

        #endregion
    }
}
