using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Blumind.Core;

namespace Blumind.Model.Styles
{
    public class TopicStyle : Style
    {
        public const int DefaultRoundRadius = 0;
        public const int DefaultRootPadding = 12;
        public const int DefaultNodePadding = 2;

        //Color _BackColor = Color.Empty;
        //Color _ForeColor = Color.Empty;
        //Color _LineColor = Color.Empty;
        //Color _BorderColor = Color.Empty;
        //Padding _Padding = new Padding(DefaultNodePadding);

        public TopicStyle()
        {
            Styles = new Dictionary<string, object>();
        }

        public Dictionary<string, object> Styles { get; private set; }

        public void SetValue(string name, object value)
        {
            if (name == null)
                throw new ArgumentNullException();

            if (!Styles.ContainsKey(name))
                Styles.Add(name, value);
            else if (Styles[name] != value)
                Styles[name] = value;
            else
                return;

            OnValueChanged();
        }

        public object GetValue(string name)
        {
            if (name == null)
                throw new ArgumentNullException();

            if (Styles.ContainsKey(name))
                return Styles[name];
            else
                return null;
        }

        public T GetValue<T>(string name)
        {
            return GetValue<T>(name, default(T));
        }

        public T GetValue<T>(string name, T defaultValue)
        {
            if (name == null)
                throw new ArgumentNullException();

            if (Styles.ContainsKey(name))
                return ST.GetValue(Styles[name], defaultValue);
            else
                return defaultValue;
        }

        public Color BackColor
        {
            get { return GetValue("Back Color", Color.Empty); }
            set { SetValue("Back Color", value); }
        }

        public Color ForeColor
        {
            get { return GetValue("Fore Color", Color.Empty); }
            set { SetValue("Fore Color", value); }
        }

        public Color LineColor
        {
            get { return GetValue("Line Color", Color.Empty); }
            set { SetValue("Line Color", value); }
        }

        public Color BorderColor
        {
            get { return GetValue("Border Color", Color.Empty); }
            set { SetValue("Border Color", value); }
        }

        public Font Font
        {
            get { return GetValue<Font>("Font", null); }
            set { SetValue("Font", value); }
        }

        public TopicShape Shape
        {
            get { return GetValue("Shape", TopicShape.Default); }
            set { SetValue("Shape", value); }
        }

        public Padding Padding
        {
            get { return GetValue("Padding", new Padding(DefaultNodePadding)); }
            set { SetValue("Padding", value); }
        }

        public HorizontalAlignment TextAlignment
        {
            get { return GetValue("TextAlignment", HorizontalAlignment.Center); }
            set { SetValue("TextAlignment", value); }
        }

        public int RoundRadius
        {
            get { return GetValue("Round Radius", DefaultRoundRadius); }
            set { SetValue("Round Radius", value); }
        }

        public string FillType
        {
            get { return GetValue<string>("FillType"); }
            set { SetValue("FillType", value); }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var style in Styles)
            {
                if (sb.Length > 0)
                    sb.Append(",");
                sb.AppendFormat("{0}:{1}", style.Key, style.Value);
            }

            BuildStyleString(sb, "Back Color", !BackColor.IsEmpty, ST.ToString(BackColor));
            BuildStyleString(sb, "Fore Color", !ForeColor.IsEmpty, ST.ToString(ForeColor));
            BuildStyleString(sb, "Line Color", !LineColor.IsEmpty, ST.ToString(LineColor));
            BuildStyleString(sb, "Border Color", !BorderColor.IsEmpty, ST.ToString(BorderColor));
            BuildStyleString(sb, "Font", Font != null, Font);
            BuildStyleString(sb, "Padding", Padding != Padding.Empty, ST.ToString(Padding));
            BuildStyleString(sb, "Round Radius", RoundRadius != DefaultRoundRadius, RoundRadius);
            //BuildStyleString(sb, "Widget Padding", WidgetPadding != DefaultWidgetPadding, WidgetPadding);

            return sb.ToString();
        }

        public override void Copy(Style style)
        {
            if (style is TopicStyle)
            {
                TopicStyle ts = (TopicStyle)style;
                BackColor = ts.BackColor;
                ForeColor = ts.ForeColor;
                LineColor = ts.LineColor;
                BorderColor = ts.BorderColor;
                Font = ts.Font;
                Padding = ts.Padding;
                Shape = ts.Shape;
                RoundRadius = ts.RoundRadius;
                //WidgetPadding = ts.WidgetPadding;
            }
            else
            {
                throw new InvalidCastException();
            }
        }

        public override Style Clone()
        {
            var ts = (TopicStyle)this.MemberwiseClone();
            ts.Styles = new Dictionary<string, object>();
            foreach (var sv in this.Styles)
            {
                ts.Styles.Add(sv.Key, sv.Value);
            }
            return ts;
        }

        #region I/O
        public override void Serialize(XmlDocument dom, XmlElement node)
        {
            base.Serialize(dom, node);

            ST.SerializeColor(node, "back_color", BackColor);
            ST.SerializeColor(node, "fore_color", ForeColor);
            ST.SerializeColor(node, "line_color", LineColor);
            ST.SerializeColor(node, "border_color", BorderColor);
            if (Shape != TopicShape.Default)
                ST.WriteTextNode(node, "shape", ((int)Shape).ToString());

            if (RoundRadius != TopicStyle.DefaultRoundRadius)
                ST.WriteTextNode(node, "round_radius", RoundRadius.ToString());

            if (Padding.All != TopicStyle.DefaultNodePadding)
                ST.WriteTextNode(node, "padding", ST.ToString(Padding));

            if (TextAlignment != HorizontalAlignment.Center)
                ST.WriteTextNode(node, "text_align", TextAlignment.ToString());

            if (!string.IsNullOrEmpty(FillType))
                ST.WriteTextNode(node, "fill_type", FillType);

            if (Font != null)
            {
                XmlElement fontNode = dom.CreateElement("font");
                ST.WriteFontNode(fontNode, Font);
                node.AppendChild(fontNode);
            }
        }

        public override void Deserialize(Version documentVersion, XmlElement node)
        {
            base.Deserialize(documentVersion, node);

            BackColor = ST.DeserializeColor(node, "back_color", BackColor);
            ForeColor = ST.DeserializeColor(node, "fore_color", ForeColor);
            LineColor = ST.DeserializeColor(node, "line_color", LineColor);
            BorderColor = ST.DeserializeColor(node, "border_color", BorderColor);
            Shape = ST.GetTopicShape(ST.ReadTextNode(node, "shape"), TopicShape.Default);
            RoundRadius = ST.GetInt(ST.ReadTextNode(node, "round_radius"), TopicStyle.DefaultRoundRadius);
            TextAlignment = ST.GetEnumValue(ST.ReadTextNode(node, "text_align"), HorizontalAlignment.Center);
            FillType = ST.ReadTextNode(node, "fill_type");

            Padding? padding = ST.GetPadding(ST.ReadTextNode(node, "padding"));
            if (padding.HasValue)
            {
                Padding = padding.Value;
            }
            else
            {
                Padding = new Padding(TopicStyle.DefaultNodePadding);
            }

            XmlElement fontNode = node.SelectSingleNode("font") as XmlElement;
            if (fontNode != null)
            {
                Font = ST.ReadFontNode(fontNode);
            }
        }
        #endregion
    }
}
