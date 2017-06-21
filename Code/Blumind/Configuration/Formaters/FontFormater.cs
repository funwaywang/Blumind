using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using Blumind.Core;

namespace Blumind.Configuration.Formaters
{
    class FontFormater : IObjectFormater
    {
        public string Name
        {
            get { return "FONT"; }
        }

        public bool SupportType(Type type)
        {
            if (type == null)
                return false;
            else
                return type == typeof(Font) || type.IsSubclassOf(typeof(Font));
        }

        public void SerializeValue(XmlElement node, object value)
        {
            if (value == null)
                return;
            var font = value as Font;
            if (font == null)
                return;
            ST.WriteFontNode(node, "font", font);
        }

        public object DeserializeValue(XmlElement node)
        {
            if (node == null)
                return null;

            return ST.ReadFontNode(node, "font");
        }
    }
}
