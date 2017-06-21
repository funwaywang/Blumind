using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blumind.Configuration.Formaters
{
    interface IObjectFormater
    {
        string Name { get; }

        bool SupportType(Type type);

        void SerializeValue(System.Xml.XmlElement node, object value);

        object DeserializeValue(System.Xml.XmlElement node);
    }
}
