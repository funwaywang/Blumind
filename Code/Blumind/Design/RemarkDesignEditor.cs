using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms.Design;
using Blumind.Core;
using Blumind.Dialogs;
using Blumind.Model;

namespace Blumind.Design
{
    class RemarkDesignEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value is string)
            {
                var edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc != null)
                {
                    //var dlg = new RemarkDialog();
                    //dlg.RemarkObject = (IRemark)value;
                    NoteWidgetDialog dlg = new NoteWidgetDialog();
                    dlg.Remark = (string)value;

                    if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
                    {
                        return dlg.Remark;
                    }
                }
            }

            return base.EditValue(context, provider, value);
        }
    }

    class RemarkTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return false;
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is string)
            {
                return ST.HtmlToText((string)value);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
