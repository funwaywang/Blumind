using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Design
{
    class LanguagesEditor : ListEditor<Language>
    {
        protected override Language[] GetStandardValues()
        {
            return LanguageManage.Languages;
        }

        protected override IEnumerable<ListItem<Language>> GetStandardItems()
        {
            foreach (var lang in LanguageManage.Languages)
            {
                yield return new ListItem<Language>(lang.ToString(), lang);
            }
        }

        protected override int ListControlMinWidth
        {
            get
            {
                return 250;
            }
        }

        /*protected override void DrawListItem(DrawItemEventArgs e, Rectangle rect, Language value)
        {
            if (value != null)
            {
                //
                if (!value.Stable)
                {
                    Brush brushDesc = ((e.State & DrawItemState.Selected) == DrawItemState.Selected) ? SystemBrushes.HighlightText : SystemBrushes.ControlDark;
                    e.Graphics.DrawString("test", e.Font, brushDesc, rect, PaintHelper.SFRight);
                }

                //
                Brush brushFore = ((e.State & DrawItemState.Selected) == DrawItemState.Selected) ? SystemBrushes.HighlightText : SystemBrushes.WindowText;
                StringFormat sf = PaintHelper.SFLeft;
                sf.FormatFlags |= StringFormatFlags.NoWrap;
                sf.Trimming = StringTrimming.None;
                e.Graphics.DrawString(value.ToString(), e.Font, brushFore, rect, sf);
            }
            //base.DrawItem(e, rect, brushFore, value);
        }*/
    }

    class LanguageConverter : TypeConverter
    {
        //private TypeConverter.StandardValuesCollection Languages;

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Language)
            {
                return ((Language)value).Name;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
