using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Blumind.Controls
{
    class FontsDropDownList : ComboBox
    {
        public FontsDropDownList()
        {
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (!DesignMode && Items.Count == 0)
            {
                FillFonts();
            }
        }

        void FillFonts()
        {
            Items.Clear();

            var sysFonts = new Font[] { SystemFonts.DefaultFont, 
                SystemFonts.CaptionFont, 
                SystemFonts.DialogFont, 
                SystemFonts.MessageBoxFont,
                SystemFonts.MenuFont, 
                SystemFonts.IconTitleFont,
                SystemFonts.SmallCaptionFont,
                SystemFonts.StatusFont };

            var sysFontNames = sysFonts.Select(f => f.FontFamily.Name).Distinct().ToArray();
            foreach (var fn in sysFontNames)
            {
                Items.Add(fn);
            }

            using (var fonts = new InstalledFontCollection())
            {
                foreach (var font in fonts.Families)
                {
                    Items.Add(font.Name);
                }
            }
        }
    }
}
