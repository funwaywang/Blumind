using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Xml;
using Blumind.Globalization;

namespace Blumind.Model.Styles
{
    [TypeConverter(typeof(StyleConverter))]
    [Editor(typeof(StyleEditor), typeof(UITypeEditor))]
    public abstract class Style : Blumind.Model.ISerializable
    {
        public event EventHandler ValueChanged;

        public virtual Style Clone()
        {
            return (Style)this.MemberwiseClone();
        }

        public abstract void Copy(Style style);

        protected void OnValueChanged()
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, EventArgs.Empty);
            }
        }

        protected void BuildStyleString(StringBuilder sb, string name, bool validate, object value)
        {
            if (validate)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(Lang._(name));
                sb.Append(":");
                sb.Append(value.ToString());
            }
        }

        #region I/O
        public virtual void Serialize(XmlDocument dom, XmlElement node)
        {
        }

        public virtual void Deserialize(Version documentVersion, XmlElement node)
        {
        }
        #endregion
    }
}
