using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Model.Styles
{
    class ChartStyle : Style
    {
        Color _BackColor = Color.White;
        Color _ForeColor = Color.Black;
        Color _LineColor = Color.Black;
        Color _BorderColor = Color.DarkGray;
        Color _SelectColor = SystemColors.Highlight;
        Color _HoverColor = Color.MediumSlateBlue;
        Font _Font;
        int _LineWidth = 1;
        int _BorderWidth = 1;

        public ChartStyle()
        {
        }

        [DefaultValue(typeof(Color), "White")]
        [LocalDisplayName("Back Color"), LocalCategory("Color")]
        public Color BackColor
        {
            get { return _BackColor; }
            set
            {
                if (_BackColor != value)
                {
                    _BackColor = value;
                    OnValueChanged();
                }
            }
        }

        [DefaultValue(typeof(Color), "Black")]
        [LocalDisplayName("Fore Color"), LocalCategory("Color")]
        public Color ForeColor
        {
            get { return _ForeColor; }
            set
            {
                if (_ForeColor != value)
                {
                    _ForeColor = value;
                    OnValueChanged();
                }
            }
        }

        [DefaultValue(typeof(Color), "Black")]
        [LocalDisplayName("Line Color"), LocalCategory("Color")]
        public Color LineColor
        {
            get { return _LineColor; }
            set
            {
                if (_LineColor != value)
                {
                    _LineColor = value;
                    OnValueChanged();
                }
            }
        }

        [DefaultValue(typeof(Color), "DarkGray")]
        [LocalDisplayName("Border Color"), LocalCategory("Color")]
        public Color BorderColor
        {
            get { return _BorderColor; }
            set
            {
                if (_BorderColor != value)
                {
                    _BorderColor = value;
                    OnValueChanged();
                }
            }
        }

        [DefaultValue(typeof(Color), "Highlight")]
        [LocalDisplayName("Selection Color"), LocalCategory("Color")]
        public Color SelectColor
        {
            get { return _SelectColor; }
            set
            {
                if (_SelectColor != value)
                {
                    _SelectColor = value;
                    OnValueChanged();
                }
            }
        }

        [DefaultValue(typeof(Color), "MediumSlateBlue")]
        [LocalDisplayName("Hover Color"), LocalCategory("Color")]
        public Color HoverColor
        {
            get { return _HoverColor; }
            set
            {
                if (_HoverColor != value)
                {
                    _HoverColor = value;
                    OnValueChanged();
                }
            }
        }

        [DefaultValue(null)]
        [LocalDisplayName("Font"), LocalCategory("Appearance")]
        public virtual Font Font
        {
            get { return _Font; }
            set
            {
                if (_Font != value)
                {
                    _Font = value;
                    OnValueChanged();
                }
            }
        }

        [DefaultValue(1)]
        [LocalDisplayName("Line Width"), LocalCategory("Appearance")]
        public int LineWidth
        {
            get { return _LineWidth; }
            set
            {
                if (value <= 0)
                    return;

                if (_LineWidth != value)
                {
                    _LineWidth = value;
                    OnValueChanged();
                }
            }
        }

        [DefaultValue(1)]
        [LocalDisplayName("Border Width"), LocalCategory("Appearance")]
        public int BorderWidth
        {
            get { return _BorderWidth; }
            set
            {
                if (value <= 0)
                    return;

                if (_BorderWidth != value)
                {
                    _BorderWidth = value;
                    OnValueChanged();
                }
            }
        }

        [Browsable(false)]
        public virtual bool IsEmpty
        {
            get
            {
                return BackColor.IsEmpty
                    && ForeColor.IsEmpty
                    && LineColor.IsEmpty
                    && BorderColor.IsEmpty
                    && SelectColor.IsEmpty
                    && HoverColor.IsEmpty
                    && BorderWidth != 1
                    && LineWidth != 1
                    && Font == null;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            BuildStyleString(sb, "Back Color", !BackColor.IsEmpty, ST.ToString(BackColor));
            BuildStyleString(sb, "Fore Color", !ForeColor.IsEmpty, ST.ToString(ForeColor));
            BuildStyleString(sb, "Line Color", !LineColor.IsEmpty, ST.ToString(LineColor));
            BuildStyleString(sb, "Border Color", !BorderColor.IsEmpty, ST.ToString(BorderColor));
            BuildStyleString(sb, "Select Color", !SelectColor.IsEmpty, ST.ToString(SelectColor));
            BuildStyleString(sb, "Hover Color", !HoverColor.IsEmpty, ST.ToString(HoverColor));
            BuildStyleString(sb, "Font", Font != null, Font);
            BuildStyleString(sb, "Line Width", LineWidth != 1, LineWidth);
            BuildStyleString(sb, "Border Width", BorderWidth != 1, BorderWidth);

            return sb.ToString();
        }

        public override void Copy(Style style)
        {
            if (style is ChartStyle)
            {
                var cs = (ChartStyle)style;
                BackColor = cs.BackColor;
                ForeColor = cs.ForeColor;
                LineColor = cs.LineColor;
                BorderColor = cs.BorderColor;
                Font = cs.Font;
                LineWidth = cs.LineWidth;
                BorderWidth = cs.BorderWidth;
                SelectColor = cs.SelectColor;
                HoverColor = cs.HoverColor;
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}
