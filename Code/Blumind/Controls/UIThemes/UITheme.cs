using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

namespace Blumind.Controls
{
    class UITheme
    {
        UIColorTheme _Colors = UIColorThemeManage.Default;
        ToolStripRenderer _ToolStripRenderer = DefaultToolStripRenderer.Default;
        //ToolStripRenderer _NormalToolStripRenderer = Darsson.Controls.NormalToolStripRenderer.Default;
        int _RoundCorner = 0;
        bool Suspend;
        bool NeedUpdate;
        static UITheme _Default;
        List<IThemableUI> _Listeners;

        public event EventHandler Changing;
        public event EventHandler Changed;

        private UITheme()
        {
            Listeners = new List<IThemableUI>();
        }

        public static UITheme Default
        {
            get
            {
                if (_Default == null)
                    _Default = new UITheme();

                return _Default;
            }
        }

        public UIColorTheme Colors
        {
            get { return _Colors; }
            set 
            {
                if (value == null)
                    value = UIColorThemeManage.Default;

                if (_Colors != value)
                {
                    _Colors = value;
                    OnColorsChanged();
                }
            }
        }

        public ToolStripRenderer ToolStripRenderer
        {
            get { return UITheme.Default._ToolStripRenderer; }
            set 
            {
                if (_ToolStripRenderer != value)
                {
                    _ToolStripRenderer = value;
                    OnToolStripRendererChanged();
                }
            }
        }

        //public ToolStripRenderer NormalToolStripRenderer
        //{
        //    get { return _NormalToolStripRenderer; }
        //    set
        //    {
        //        if (_NormalToolStripRenderer != value)
        //        {
        //            _NormalToolStripRenderer = value;
        //            OnNormalToolStripRendererChanged();
        //        }
        //    }
        //}

        public int RoundCorner
        {
            get { return _RoundCorner; }
            set 
            {
                if (_RoundCorner != value)
                {
                    _RoundCorner = value;
                    OnChanged();
                }
            }
        }

        public Font DefaultFont
        {
            get { return SystemFonts.MessageBoxFont; }
        }

        public List<IThemableUI> Listeners
        {
            get { return _Listeners; }
            private set { _Listeners = value; }
        }

        private void OnColorsChanged()
        {
            if (ToolStripRenderer == DefaultToolStripRenderer.Default)
                DefaultToolStripRenderer.Reset();

            OnChanged();
        }

        private void OnToolStripRendererChanged()
        {
            OnChanged();
        }

        private void OnNormalToolStripRendererChanged()
        {
            OnChanged();
        }

        public void Reset()
        {
            Suspend = true;
            if (Changing != null)
            {
                Changing(null, EventArgs.Empty);
            }
            Suspend = false;

            if (NeedUpdate)
            {
                OnChanged();
            }
        }

        private void OnChanged()
        {
            if (Suspend)
            {
                NeedUpdate = true;
            }
            else
            {
                if (Changed != null)
                {
                    Changed(null, EventArgs.Empty);
                }

                NotifyListeners();

                NeedUpdate = false;
            }
        }

        public void NotifyListeners()
        {
            foreach (IThemableUI ctrl in Listeners)
            {
                if (ctrl == null)
                    continue;
                ctrl.ApplyTheme(this);
            }
        }
    }
}
