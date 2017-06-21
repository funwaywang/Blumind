using System.Drawing;
using System.Windows.Forms;

namespace Blumind.Controls
{
    class DefaultToolStripColors : ProfessionalColorTable
    {
        Color _MenuStripGradientBegin = Color.FromArgb(46, 64, 91);
        Color _MenuStripGradientEnd = Color.FromArgb(52, 72, 104);
        Color _MenuItemSelectedGradientBegin = Color.FromArgb(255, 251, 240);
        Color _MenuItemSelectedGradientMiddle;
        Color _MenuItemSelectedGradientEnd = Color.FromArgb(255, 236, 181);
        Color _MenuItemSelectedBorder;
        Color _MenuItemSelected = Color.FromArgb(229, 195, 101);
        Color _MenuItemPressedGradientBegin = Color.FromArgb(233, 236, 238);
        Color _MenuBorder = Color.FromArgb(155, 167, 183);

        Color _GripDark = Color.FromArgb(26, 33, 48);
        Color _GripLight = Color.FromArgb(64, 96, 128);
        Color _ToolStripGradientBegin = Color.FromArgb(188, 199, 216);
        Color _ToolStripGradientMiddle = Color.FromArgb(188, 199, 216);
        Color _ToolStripGradientEnd = Color.FromArgb(188, 199, 216);
        Color _ToolStripDropDownBackground = Color.FromArgb(225, 229, 234);

        Color _ImageMarginGradientBegin = Color.FromArgb(233, 236, 238);
        Color _ImageMarginGradientMiddle = Color.FromArgb(233, 236, 238);
        Color _ImageMarginGradientEnd = Color.FromArgb(233, 236, 238);

        Color _CheckBackground, _CheckSelectedBackground;
        Color _SeparatorDark, _SeparatorLight;

        Color _ButtonCheckedGradientBegin;
        Color _ButtonCheckedGradientMiddle;
        Color _ButtonCheckedGradientEnd;
        Color _ButtonSelectedBorder;
        Color _ButtonSelectedGradientBegin;
        Color _ButtonSelectedGradientMiddle;
        Color _ButtonSelectedGradientEnd;

        public DefaultToolStripColors()
        {
            UIColorTheme theme = UITheme.Default.Colors;
            if (theme == null)
                theme = UIColorThemeManage.Default;

            _MenuStripGradientEnd = theme.Workspace;
            _MenuStripGradientBegin = theme.Workspace;// PaintHelper.GetDarkColor(_MenuStripGradientEnd, 0.1);
            _MenuItemPressedGradientBegin = PaintHelper.GetLightColor(theme.Light, 0.2f);
            _MenuItemSelected = theme.MediumLight;// PaintHelper.GetDarkColor(theme.Sharp, 0.1f);
            MenuStripItemSelectedForeColor = PaintHelper.FarthestColor(_MenuItemSelected, theme.Dark, theme.Light);
            _MenuItemSelectedGradientMiddle = _MenuItemSelected;
            _MenuItemSelectedGradientBegin = PaintHelper.GetLightColor(_MenuItemSelectedGradientMiddle, 0.1f);
            _MenuItemSelectedGradientEnd = PaintHelper.GetLightColor(_MenuItemSelectedGradientMiddle, 0.05f);
            _MenuItemSelectedBorder = Color.Empty;// PaintHelper.AdjustColor(PaintHelper.GetDarkColor(theme.Sharp), 0, 50, 30, 40);
            _MenuBorder = theme.MediumLight;
            MenuStripItemForeColor = PaintHelper.FarthestColor(theme.Workspace, theme.MediumLight, theme.MediumDark, 50);

            _GripDark = PaintHelper.GetDarkColor(_MenuStripGradientBegin, 0.2);
            _GripLight = PaintHelper.GetLightColor(_MenuStripGradientBegin, 0.1);

            _ToolStripGradientBegin = theme.MediumLight;
            _ToolStripGradientMiddle = theme.MediumLight;
            _ToolStripGradientEnd = theme.MediumLight;// PaintHelper.GetDarkColor(theme.MediumLight, 0.1);
            _ToolStripDropDownBackground = theme.Light;

            _ImageMarginGradientBegin = theme.MenuImageMargin;
            _ImageMarginGradientMiddle = _ImageMarginGradientBegin;
            _ImageMarginGradientEnd = _ImageMarginGradientBegin;

            _CheckBackground = PaintHelper.GetLightColor(theme.MediumDark);
            _CheckSelectedBackground = PaintHelper.GetLightColor(_CheckBackground);

            //_SeparatorDark = _ToolStripGradientMiddle;
            _SeparatorDark = PaintHelper.GetDarkColor(_ToolStripGradientMiddle, 0.1);
            _SeparatorLight = Color.Empty;// PaintHelper.GetLightColor(_ToolStripGradientMiddle, 0.1);

            _ButtonCheckedGradientMiddle = PaintHelper.AdjustColor(theme.MediumLight, 0, 60, 40, 70);// .GetDarkColor(theme.MediumLight, 0.2);
            _ButtonCheckedGradientBegin = PaintHelper.GetDarkColor(_ButtonCheckedGradientMiddle, 0.2);
            _ButtonCheckedGradientEnd = PaintHelper.GetDarkColor(_ButtonCheckedGradientMiddle, 0.1);
            _ButtonSelectedBorder = PaintHelper.AdjustColor(theme.MediumDark, 0, 0, 30, 40);
            _ButtonSelectedGradientMiddle = PaintHelper.AdjustColor(theme.Sharp, 0, 100, 70, 80);
            _ButtonSelectedGradientBegin = PaintHelper.GetLightColor(_ButtonSelectedGradientMiddle, 0.2);
            _ButtonSelectedGradientEnd = PaintHelper.GetLightColor(_ButtonSelectedGradientMiddle, 0.1);
        }

        public Color MenuStripItemForeColor { get; set; }

        public Color MenuStripItemSelectedForeColor { get; set; }

        public override Color MenuStripGradientBegin { get { return _MenuStripGradientBegin; } }

        public override Color MenuStripGradientEnd { get { return _MenuStripGradientEnd; } }

        public override Color MenuItemSelectedGradientBegin { get { return _MenuItemSelectedGradientBegin; } }

        public Color MenuItemSelectedGradientMiddle { get { return _MenuItemSelectedGradientMiddle; } }

        public override Color MenuItemSelectedGradientEnd { get { return _MenuItemSelectedGradientEnd; } }

        public Color MenuItemSelectedBorder { get { return _MenuItemSelectedBorder; } }

        public override Color MenuItemSelected { get { return _MenuItemSelected; } }

        public override Color MenuItemPressedGradientBegin
        {
            get { return _MenuItemPressedGradientBegin; }
        }

        public override Color MenuItemPressedGradientEnd
        {
            get
            {
                return MenuItemPressedGradientBegin;
            }
        }

        public override Color MenuItemPressedGradientMiddle
        {
            get
            {

                return MenuItemPressedGradientBegin;
            }
        }

        public override Color MenuItemBorder
        {
            get
            {
                return _MenuItemSelected;
                //return Color.FromArgb(229, 195, 101);
            }
        }

        public override Color MenuBorder
        {
            get
            {
                return _MenuBorder;
            }
        }

        public override Color ToolStripDropDownBackground
        {
            get { return _ToolStripDropDownBackground; }
        }

        public override Color ImageMarginGradientBegin
        {
            get { return _ImageMarginGradientBegin; }
        }

        public override Color ImageMarginGradientMiddle
        {
            get
            {
                return _ImageMarginGradientMiddle;
            }
        }

        public override Color ImageMarginGradientEnd
        {
            get
            {
                return _ImageMarginGradientEnd;
            }
        }

        public override Color SeparatorDark
        {
            get
            {
                return _SeparatorDark;
            }
        }

        public override Color SeparatorLight
        {
            get
            {
                return _SeparatorLight;
            }
        }

        public override Color StatusStripGradientBegin
        {
            get
            {
                return MenuStripGradientBegin;
                //return base.StatusStripGradientBegin;
            }
        }

        public override Color StatusStripGradientEnd
        {
            get
            {
                return MenuStripGradientEnd;
                //return base.StatusStripGradientEnd;
            }
        }

        public override Color GripDark { get { return _GripDark; } }

        public override Color GripLight { get { return _GripLight; } }

        public override Color ToolStripGradientBegin
        {
            get
            {
                return _ToolStripGradientBegin;
            }
        }

        public override Color ToolStripGradientMiddle
        {
            get
            {
                return _ToolStripGradientMiddle;
            }
        }

        public override Color ToolStripGradientEnd
        {
            get
            {
                return _ToolStripGradientEnd;
            }
        }

        public override Color ButtonSelectedBorder
        {
            get
            {
                return _ButtonSelectedBorder;
            }
        }

        public override Color ButtonSelectedGradientBegin
        {
            get
            {
                return _ButtonSelectedGradientBegin;
            }
        }

        public override Color ButtonSelectedGradientMiddle
        {
            get
            {
                return _ButtonSelectedGradientMiddle;
            }
        }

        public override Color ButtonSelectedGradientEnd
        {
            get
            {
                return _ButtonSelectedGradientEnd;
            }
        }

        public override Color ButtonCheckedGradientBegin
        {
            get
            {
                return _ButtonCheckedGradientBegin;
            }
        }

        public override Color ButtonCheckedGradientMiddle
        {
            get
            {
                return _ButtonCheckedGradientMiddle;
            }
        }

        public override Color ButtonCheckedGradientEnd
        {
            get
            {
                return _ButtonCheckedGradientEnd;
            }
        }

        public override Color CheckBackground
        {
            get
            {
                return _CheckBackground;
            }
        }

        public override Color CheckSelectedBackground
        {
            get
            {
                return _CheckSelectedBackground;
            }
        }
    }
}
