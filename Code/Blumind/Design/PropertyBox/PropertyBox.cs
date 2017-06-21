using System;
using System.ComponentModel;
using Blumind.Controls;
using System.Windows.Forms;
using System.Drawing;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Design
{
    class PropertyBox : BorderPanel
    {
        bool _HelpVisible;
        bool _ToolbarVisible;
        object[] _SelectedObjects;
        //CaptionBar Caption;

        public event EventHandler SelectedObjectsChanged;
        public event EventHandler HelpVisibleChanged;
        public event EventHandler ToolbarVisibleChanged;

        public PropertyBox()
        {
            //Caption = new CaptionBar();
            //Caption.Icon = Properties.Resources.property;
            //Caption.Text = Text;
            //Caption.Dock = DockStyle.Top;
            //Caption.BackColor = ContentBackColor;
            //Controls.Add(Caption);

            LanguageManage.CurrentChanged += new EventHandler(LanguageManage_CurrentChanged);

            Text = Lang._("Property");
        }

        [DefaultValue(null)]
        public object SelectedObject
        {
            get 
            {
                if (_SelectedObjects == null || _SelectedObjects.Length == 0)
                    return null;
                else
                    return _SelectedObjects[0]; 
            }
            set             
            {
                SelectedObjects = new object[] { value };
            }
        }

        [DefaultValue(null)]
        public object[] SelectedObjects
        {
            get { return _SelectedObjects; }
            set
            {
                if (_SelectedObjects != value)
                {
                    _SelectedObjects = value;
                    OnSelectedObjectsChanged();
                }
            }
        }

        [Browsable(false)]
        public bool HelpVisible
        {
            get { return _HelpVisible; }
            set
            {
                if (_HelpVisible != value)
                {
                    _HelpVisible = value;
                    OnHelpVisibleChanged();
                }
            }
        }

        [Browsable(false)]
        public bool ToolbarVisible
        {
            get { return _ToolbarVisible; }
            set 
            {
                if (_ToolbarVisible != value)
                {
                    _ToolbarVisible = value;
                    OnToolbarVisibleChanged();
                }
            }
        }

        public override Rectangle DisplayRectangle
        {
            get
            {
                return ClientRectangle;
                //return base.DisplayRectangle;
            }
        }

        protected virtual void OnHelpVisibleChanged()
        {
            if (HelpVisibleChanged != null)
                HelpVisibleChanged(this, EventArgs.Empty);
        }

        protected virtual void OnToolbarVisibleChanged()
        {
            if (ToolbarVisibleChanged != null)
                ToolbarVisibleChanged(this, EventArgs.Empty);
        }

        protected virtual void OnSelectedObjectsChanged()
        {
            if (SelectedObjectsChanged != null)
                SelectedObjectsChanged(this, EventArgs.Empty);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            if (Controls.Count > 0 && Controls[0].CanFocus)
                Controls[0].Focus();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            //Caption.Text = Text;
        }

        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);

            //if (theme != null)
            //{
            //    Caption.BackColor = theme.Colors.MediumLight;
            //    Caption.ForeColor = theme.Colors.Dark;
            //}
        }

        private void LanguageManage_CurrentChanged(object sender, EventArgs e)
        {
            OnCurrentLanguageChanged();
        }

        protected virtual void OnCurrentLanguageChanged()
        {
            Text = Lang._("Property");
        }
    }
}
