using System;
using System.ComponentModel;

namespace Blumind.Model.Styles
{
    public class StyleConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return false;

            return base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return false;

            return base.CanConvertFrom(context, sourceType);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            if (value != null)
                return TypeDescriptor.GetProperties(value.GetType(), attributes);
            else
                return base.GetProperties(context, value, attributes);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
