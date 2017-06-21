using System;
using System.ComponentModel;
using System.Globalization;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Design
{
    class BoolConverter : TypeConverter
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

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is bool)
                {
                    if ((bool)value)
                        return Lang._("Yes");
                    else
                        return Lang._("No");
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string s = (string)value;
                bool? b = ST.GetBool(s);
                if (b.HasValue)
                    return b.Value;

                if (StringComparer.OrdinalIgnoreCase.Equals(s, Lang._("Yes")))
                    return true;
                else
                    return false;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new bool[] { true, false });
            //return new StandardValuesCollection(new string[] { LanguageManage.GetText("Yes"), LanguageManage.GetText("No") });
            //return base.GetStandardValues(context);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
            //return base.GetStandardValuesSupported(context);
        }
    }
}
