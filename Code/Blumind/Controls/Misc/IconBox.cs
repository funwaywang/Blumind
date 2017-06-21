using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Blumind.Controls
{
    [DefaultProperty("Icon")]
    class IconBox : BaseControl
    {
        Icon _Icon;

        public IconBox()
        {
            SetPaintStyles();
        }

        public IconBox(Icon icon)
            : this()
        {
            this.Icon = icon;
        }

        [DefaultValue(null)]
        public Icon Icon
        {
            get { return _Icon; }
            set
            {
                if (_Icon != value)
                {
                    _Icon = value;
                    OnIconChanged();
                }
            }
        }

        void OnIconChanged()
        {
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            InvokePaintBackground(this, e);

            var rect = ClientRectangle;
            if (Icon != null)
            {
                var targetRect = new Rectangle(
                    rect.X + (rect.Width - Icon.Width) / 2,
                    rect.Y + (rect.Height - Icon.Height) / 2,
                    Icon.Width,
                    Icon.Height);
                e.Graphics.DrawIconUnstretched(Icon, targetRect);
            }

            if (DesignMode)
            {
                Pen pen = new Pen(SystemColors.ControlDark);
                pen.DashStyle = DashStyle.Dash;
                e.Graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }
        }
    }
}
