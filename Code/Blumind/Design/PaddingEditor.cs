using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Blumind.Controls;
using Blumind.Core;

namespace Blumind.Design
{
    class PaddingEditor : UITypeEditor
    {
        private PaddingBox EditUI;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value is Padding)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc != null)
                {
                    if (EditUI == null)
                    {
                        EditUI = new PaddingBox();
                        EditUI.AutoSize = true;
                    }

                    //EditUI.Initialize(edSvc);
                    EditUI.Value = (Padding)value;
                    edSvc.DropDownControl(EditUI);
                    return EditUI.Value;
                }
            }

            return base.EditValue(context, provider, value);
        }
    }

    class PaddingConverter : BaseTypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                Padding? padding = ST.GetPadding((string)value);
                if (padding.HasValue)
                    return padding.Value;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Padding)
            {
                return ST.ToString((Padding)value);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }
}
