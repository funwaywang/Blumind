using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Controls.UIThemes
{
    class UIThemesDropDownList : ComboBox
    {
        bool styleInited;
        string _SelectedTheme;

        public UIThemesDropDownList()
        {
        }

        [DefaultValue(false)]
        public bool Localizable { get; set; }

        public void SetDefaultStyle()
        {
            DropDownStyle = ComboBoxStyle.DropDownList;
            DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            Localizable = true;
            Font = SystemFonts.MessageBoxFont;
            //ItemHeight += 2;
        }
        
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);

            e.DrawBackground();
            if (e.Index > -1)
            {
                if (Items[e.Index] is UIColorTheme)
                {
                    var ct = (UIColorTheme)Items[e.Index];

                    //
                    int ch = Math.Min(e.Bounds.Height - 2, 16);
                    var cr = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + (e.Bounds.Height - ch) / 2, 12, ch);
                    e.Graphics.FillRectangle(new SolidBrush(ct.Light), cr);
                    cr.X += cr.Width;
                    e.Graphics.FillRectangle(new SolidBrush(ct.Dark), cr);
                    cr.X += cr.Width;
                    e.Graphics.FillRectangle(new SolidBrush(ct.MediumLight), cr);
                    cr.X += cr.Width;
                    e.Graphics.FillRectangle(new SolidBrush(ct.MediumDark), cr);
                    cr.X += cr.Width;
                    e.Graphics.FillRectangle(new SolidBrush(ct.Sharp), cr);
                    cr.X += cr.Width;
                    e.Graphics.FillRectangle(new SolidBrush(ct.Workspace), cr);
                    cr.X += cr.Width;
                    cr.X += 4;

                    //
                    if (!string.IsNullOrEmpty(ct.Name))
                    {
                        var sf = PaintHelper.SFLeft;
                        sf.FormatFlags |= StringFormatFlags.NoWrap;
                        sf.Trimming = StringTrimming.None;

                        var text = Localizable ? Lang._(ct.Name) : ct.Name;
                        e.Graphics.DrawString(text,
                            e.Font,
                            new SolidBrush(e.ForeColor),
                            new Rectangle(cr.X, e.Bounds.Top, e.Bounds.Width - cr.X, e.Bounds.Height),
                            sf);
                    }
                }
            }
        }

        public void SelectTheme(string name)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var ct = (UIColorTheme)Items[i];
                if (ct != null && ct.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    SelectedIndex = i;
                    return;
                }
            }

            SelectedIndex = -1;
        }

        [DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedTheme
        {
            get { return _SelectedTheme; }
            set
            {
                if (_SelectedTheme != value)
                {
                    _SelectedTheme = value;
                    OnSelectedThemeChanged();
                }
            }
        }

        public bool HasTheme(string name)
        {
            foreach (UIColorTheme ct in Items)
            {
                if (ct.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (!DesignMode && !styleInited)
            {
                SetDefaultStyle();
                styleInited = true;
            }
        }

        void OnSelectedThemeChanged()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var ct = (UIColorTheme)Items[i];
                if (ct != null && ct.Name.Equals(SelectedTheme, StringComparison.OrdinalIgnoreCase))
                {
                    SelectedIndex = i;
                    return;
                }
            }

            SelectedIndex = -1;
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (SelectedItem is UIColorTheme)
            {
                var theme = (UIColorTheme)SelectedItem;
                SelectedTheme = theme.Name;
            }
            else
            {
                SelectedTheme = null;
            }

            base.OnSelectedIndexChanged(e);
        }
    }
}
