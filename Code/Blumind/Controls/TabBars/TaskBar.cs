using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Blumind.Controls
{
    class TaskBar : TabBar
    {
        Color _InactiveSelectedItemBackColor = Color.LightSteelBlue;
        Color _InactiveSelectedItemForeColor = Color.Black;

        public event EventHandler InactiveSelectedItemBackColorChanged;
        public event EventHandler InactiveSelectedItemForeColorChanged;

        public TaskBar()
        {
            ShowDropDownButton = true;
            BaseLineSize = 3;
            ItemPadding = new Padding(10, 0, 10, 0);
        }

        [DefaultValue(typeof(Color), "LightSteelBlue")]
        public Color InactiveSelectedItemBackColor
        {
            get { return _InactiveSelectedItemBackColor; }
            set
            {
                if (_InactiveSelectedItemBackColor != value)
                {
                    _InactiveSelectedItemBackColor = value;
                    OnInactiveSelectedItemBackColorChanged();
                }
            }
        }

        [DefaultValue(typeof(Color), "Black")]
        public Color InactiveSelectedItemForeColor
        {
            get { return _InactiveSelectedItemForeColor; }
            set
            {
                if (_InactiveSelectedItemForeColor != value)
                {
                    _InactiveSelectedItemForeColor = value;
                    OnInactiveSelectedItemForeColorChanged();
                }
            }
        }

        [DefaultValue(true)]
        public override bool ShowDropDownButton
        {
            get
            {
                return base.ShowDropDownButton;
            }
            set
            {
                base.ShowDropDownButton = value;
            }
        }

        [DefaultValue(4)]
        public override int BaseLineSize
        {
            get
            {
                return base.BaseLineSize;
            }
            set
            {
                base.BaseLineSize = value;
            }
        }

        protected override int HeaderPadding
        {
            get
            {
                return 2;
            }
        }

        protected override int InactivedItemLower
        {
            get
            {
                return 0;
            }
        }

        [DefaultValue(typeof(Padding), "10, 0, 10, 0")]
        public override Padding ItemPadding
        {
            get
            {
                return base.ItemPadding;
            }
            set
            {
                base.ItemPadding = value;
            }
        }

        public bool AeroBackground { get; set; }

        public override TabBarRenderer DefaultRenderer
        {
            get
            {
                return new TaskBarRenderer(this);
            }
        }

        protected virtual void OnInactiveSelectedItemBackColorChanged()
        {
            if (InactiveSelectedItemBackColorChanged != null)
            {
                InactiveSelectedItemBackColorChanged(this, EventArgs.Empty);
            }

            Invalidate();
        }

        protected virtual void OnInactiveSelectedItemForeColorChanged()
        {
            if (InactiveSelectedItemForeColorChanged != null)
            {
                InactiveSelectedItemForeColorChanged(this, EventArgs.Empty);
            }

            Invalidate();
        }

        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);

            if (theme != null)
            {
                BackColor = theme.Colors.Workspace.IsEmpty ? theme.Colors.Dark : theme.Colors.Workspace;
                ForeColor = theme.Colors.Light;
                SelectedItemBackColor = theme.Colors.MediumLight;//.Sharp;
                SelectedItemForeColor = PaintHelper.FarthestColor(SelectedItemBackColor, theme.Colors.Dark, theme.Colors.Light);// theme.Colors.SharpText;
                InactiveSelectedItemBackColor = SelectedItemBackColor;
                InactiveSelectedItemForeColor = SelectedItemForeColor;
                //InactiveSelectedItemBackColor = theme.Colors.MediumDark;
                //InactiveSelectedItemForeColor = PaintHelper.FarthestColor(theme.Colors.MediumDark, theme.Colors.Light, theme.Colors.Dark);
                ItemBackColor = theme.Colors.MediumDark;
                ItemForeColor = PaintHelper.FarthestColor(ItemBackColor, theme.Colors.Dark, theme.Colors.Light);
                HoverItemBackColor = theme.Colors.Sharp;
                HoverItemForeColor = PaintHelper.FarthestColor(HoverItemBackColor, theme.Colors.Dark, theme.Colors.Light);

            }
        }
    }
}
