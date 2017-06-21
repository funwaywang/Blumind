using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Blumind.Configuration;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Controls
{
    class BaseForm : Form, IThemableUI//Blumind.Controls.Aero.GlassForm//, IGlobalBackground
    {
        Image _IconImage = null;

        public BaseForm()
        {
            Icon = Properties.Resources.logo_icon;

#if DEBUG
            if (!DesignMode && CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
                RightToLeft = RightToLeft.Yes;
#endif

            LanguageManage.CurrentChanged += new EventHandler(LanguageManage_CurrentChanged);
            Options.Current.OpitonsChanged += new EventHandler(Options_OpitonsChanged);
            //Options.Default.UISettingChanged += new EventHandler(Options_UISettingChanged);
        }

        [DefaultValue(null)]
        public Image IconImage
        {
            get { return _IconImage; }
            set { _IconImage = value; }
        }

        protected virtual bool UseGlobalColors
        {
            get { return true; }
        }

        #region FullScreen
        private bool _FullScreen = false;
        private FullScreenInfo FullScreenInfomation = null;

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

        protected virtual void OnFullScreenChanged()
        {
            if (FullScreenChanged != null)
                this.FullScreenChanged(this, EventArgs.Empty);
        }

        public virtual void EnterFullScreen()
        {
            if (this.FullScreen)
                return;

            FullScreenInfo info = new FullScreenInfo(this.Parent, this.Bounds);
            info.MdiParent = this.MdiParent;
            info.TopLevel = this.TopLevel;
            info.BorderStyle = this.FormBorderStyle;
            info.TopMost = this.TopMost;
            info.WindowState = this.WindowState;
            info.ShowInTaskbar = this.ShowInTaskbar;
            info.Owner = this.Owner;

            MdiParent = null;
            Parent = null;
            TopLevel = true;
            if(info.Parent != null && info.Parent.TopLevelControl is Form)
                Owner = (Form)info.Parent.TopLevelControl;
            //TopMost = true;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Normal;
            Bounds = Screen.GetBounds(Program.MainForm);
            ShowInTaskbar = false;

            FullScreenInfomation = info;
        }

        public virtual void ExitFullScreen()
        {
            if (!this.FullScreen)
                return;

            if (this.FullScreenInfomation != null)
            {
                FullScreenInfo info = FullScreenInfomation;
                TopLevel = info.TopLevel;
                MdiParent = info.MdiParent;
                Parent = info.Parent;
                FormBorderStyle = info.BorderStyle;
                WindowState = info.WindowState;
                Bounds = info.Bounds;
                TopMost = info.TopMost;
                Owner = info.Owner;
                ShowInTaskbar = info.ShowInTaskbar;
            }
        }
        #endregion

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (!UITheme.Default.Listeners.Contains(this))
            {
                UITheme.Default.Listeners.Add(this);
                ApplyTheme(UITheme.Default);
            }
        }

        protected virtual void AfterInitialize()
        {
            SuspendLayout();
            OnOptionsChanged();
            //Options_UISettingChanged(this, EventArgs.Empty);
            OnCurrentLanguageChanged();
            ResumeLayout();
        }

        void Options_OpitonsChanged(object sender, EventArgs e)
        {
            OnOptionsChanged();
        }

        void LanguageManage_CurrentChanged(object sender, EventArgs e)
        {
            OnCurrentLanguageChanged();
        }

        protected virtual void OnOptionsChanged()
        {
            if (!DesignMode && TopLevel)
            {
                /*if (Options.Default.Appearance.Font != null)
                    this.Font = Options.Default.Appearance.Font;
                else
                    this.Font = Control.DefaultFont;// SystemInformation.MenuFont;*/
            }
        }

        void Options_UISettingChanged(object sender, EventArgs e)
        {
            //if (UseGlobalColors && !_Options.Current.Appearance.WindowBackColor.IsEmpty && !_Options.Current.Appearance.WindowForeColor.IsEmpty)
            //{
            //}
        }

        protected void SetPaintStyles()
        {
            SetStyle(ControlStyles.UserPaint |
                   ControlStyles.AllPaintingInWmPaint |
                   ControlStyles.OptimizedDoubleBuffer |
                   ControlStyles.ResizeRedraw, true);
        }

        protected virtual void OnCurrentLanguageChanged()
        {
        }

        public virtual void ApplyTheme(UITheme theme)
        {
        }
    }
}
