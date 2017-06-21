using System;
using System.ComponentModel;
using System.Globalization;

namespace Blumind.Design
{
    abstract class BaseTypeConverter : TypeConverter
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

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ConvertValueToString(value);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        protected virtual object ConvertValueToString(object value)
        {
            if (value == null)
                return null;
            else
                return value.ToString();
        }
    }
}
