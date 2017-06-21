using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Core;

namespace Blumind.Design
{
    class EnumEditor<EnumType> : ListEditor<EnumType>
    {
        protected override EnumType[] GetStandardValues()
        {
            return (EnumType[])Enum.GetValues(typeof(EnumType));
        }

        protected override IEnumerable<ListItem<EnumType>> GetStandardItems()
        {
            foreach (EnumType item in Enum.GetValues(typeof(EnumType)))
            {
                yield return new ListItem<EnumType>(ST.EnumToString(item), item);
            }
        }
    }

    class EnumConverter<EnumType> : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ST.EnumToString<EnumType>((EnumType)value);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
