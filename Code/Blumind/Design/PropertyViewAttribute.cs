using System;
using System.Collections.Generic;
using System.Text;

namespace Blumind.Design
{
    class PropertyViewAttribute : Attribute
    {
        private Type _PropertyBoxType;

        public PropertyViewAttribute(Type propertyBoxType)
        {
            PropertyBoxType = propertyBoxType;
        }

        public Type PropertyBoxType
        {
            get { return _PropertyBoxType; }
            private set { _PropertyBoxType = value; }
        }
    }
}
