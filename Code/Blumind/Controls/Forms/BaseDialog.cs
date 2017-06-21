using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Blumind.Core;

namespace Blumind.Controls
{
    class BaseDialog : BaseForm
    {
        public BaseDialog()
        {
            //FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            MaximizeBox = false;
            MinimizeBox = false;
            //ShowIcon = false;
            ButtonHeight = 27;

            //SetPaintStyles();
            if (ShowButtonArea)
            {
                ResizeRedraw = true;
            }
        }

        #region Button Area
        int _BaseLinePosition;
        Rectangle _ControlsRectangle;

        [Browsable(false)]
        public Color ButtonAreaColor { get; private set; }

        protected virtual bool ShowButtonArea
        {
            get { return false; }
        }

        [Browsable(false)]
        public int BaseLinePosition
        {
            get { return _BaseLinePosition; }
            private set
            {
                if (_BaseLinePosition != value)
                {
                    _BaseLinePosition = value;
                    OnBaseLinePositionChanged();
                }
            }
        }

        [Browsable(false)]
        public int ButtonHeight { get; private set; }

        protected int ButtonAreaHeight
        {
            get { return ClientSize.Height - BaseLinePosition; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle ControlsRectangle
        {
            get { return _ControlsRectangle; }
            private set
            {
                if (_ControlsRectangle != value)
                {
                    _ControlsRectangle = value;
                    OnControlsRectangleChanged();
                }
            }
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            BaseLinePosition = ClientSize.Height - 12 - ButtonHeight - 8;

            //
            Rectangle rect = ClientRectangle;
            rect.Height = BaseLinePosition;
            rect.X += Padding.Left;
            rect.Y += Padding.Top;
            rect.Width -= Padding.Horizontal;
            rect.Height -= Padding.Bottom;
            ControlsRectangle = rect;
        }

        protected virtual void OnBaseLinePositionChanged()
        {
        }

        protected virtual void OnControlsRectangleChanged()
        {
        }

        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);

            if (theme != null)
                ButtonAreaColor = theme.Colors.MediumDark;
            else
                ButtonAreaColor = Color.Empty;

            if (ShowButtonArea)
            {
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (ShowButtonArea && !ButtonAreaColor.IsEmpty)
            {
                Rectangle rect = ClientRectangle;
                rect.Height -= BaseLinePosition;
                rect.Y = BaseLinePosition;
                if (rect.Width > 0 && rect.Height > 0 && !ButtonAreaColor.IsEmpty)
                {
                    e.Graphics.FillRectangle(new SolidBrush(ButtonAreaColor), rect);
                }
            }
        }

        protected virtual void LocateButtons(IEnumerable<Control> buttons)
        {
            LocateButtonsRight(buttons);
        }

        protected void LocateButtonsLeft(IEnumerable<Control> buttons)
        {
            if (buttons == null)
                return;

            var pt = new Point(12, ClientSize.Height - 12);
            foreach (var btn in buttons)
            {
                if (btn != null && btn.Visible)
                {
                    btn.Height = ButtonHeight;
                    btn.Location = new Point(pt.X, pt.Y - btn.Height);
                    pt.X += btn.Width + 8;
                }
            }
        }

        protected void LocateButtonsRight(IEnumerable<Control> buttons)
        {
            if (buttons == null)
                return;

            var pt = new Point(ClientSize.Width - 12, ClientSize.Height - 12);
            foreach (var btn in buttons.Reverse())
            {
                if (btn != null && btn.Visible)
                {
                    btn.Height = ButtonHeight;
                    btn.Location = new Point(pt.X - btn.Width, pt.Y - btn.Height);
                    pt.X -= btn.Width + 8;
                }
            }
        }

        protected void LocateButtonsNoX(IEnumerable<Control> buttons)
        {
            if (buttons == null)
                return;

            foreach (var button in buttons)
            {
                button.Height = ButtonHeight;
                button.Top = BaseLinePosition + 8;
            }
        }
        #endregion

        #region IEditableControl 成员
        bool _ReadOnly;
        bool _Modified;

        public event EventHandler ModifiedChanged;
        public event EventHandler ReadOnlyChanged;

        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set
            {
                if (_ReadOnly != value)
                {
                    _ReadOnly = value;
                    OnReadOnlyChanged();
                }
            }
        }

        [DefaultValue(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        protected virtual void OnReadOnlyChanged()
        {
            if (ReadOnlyChanged != null)
                ReadOnlyChanged(this, EventArgs.Empty);
        }

        protected virtual void OnModifiedChanged()
        {
            if (ModifiedChanged != null)
                ModifiedChanged(this, EventArgs.Empty);
        }

        #endregion
        
        public void MoveToCenterScreen()
        {
            var rect = Screen.GetWorkingArea(this);
            Location = new Point(rect.X + (rect.Width - Width) / 2, rect.Y + (rect.Height - Height) / 2);
        }
    }
}
