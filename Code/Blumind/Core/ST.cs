using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Linq;
using Blumind.Model.Widgets;
using Blumind.Globalization;
using Blumind.Model.MindMaps;
using Blumind.Model.Styles;

namespace Blumind.Core
{
    public static class ST
    {
        public static T GetValue<T>(object value)
        {
            return GetValue<T>(value, default(T));
        }

        public static T GetValue<T>(object value, T defaultValue)
        {
            if (value == null)
                return defaultValue;

            if (value is T)
                return (T)value;
            else if (value is string)
            {
                string str = (string)value;
                if (typeof(T) == typeof(int))
                    return (T)(object)GetInt(str, (int)(object)defaultValue);
                else if (typeof(T) == typeof(decimal))
                    return (T)(object)GetDecimal(str, (decimal)(object)defaultValue);
                else if (typeof(T) == typeof(long))
                    return (T)(object)GetLong(str, (long)(object)defaultValue);
                else if (typeof(T) == typeof(float))
                    return (T)(object)GetFloat(str, (float)(object)defaultValue);
                else if (typeof(T) == typeof(bool))
                    return (T)(object)GetBool(str, (bool)(object)defaultValue);
                else if (typeof(T) == typeof(DateTime))
                    return (T)(object)GetDateTime(str, (DateTime)(object)defaultValue);
                else if (typeof(T) == typeof(Color))
                    return (T)(object)GetColor(str, (Color)(object)defaultValue);
                else if (typeof(T) == typeof(Size))
                    return (T)(object)GetSize(str, (Size)(object)defaultValue);
                else if (typeof(T) == typeof(Point))
                    return (T)(object)GetPoint(str, (Point)(object)defaultValue);
                else if (typeof(T) == typeof(string[]))
                    return (T)(object)GetStringArray(str, (string[])(object)defaultValue);
                else if (typeof(T) == typeof(Font))
                    return (T)(object)GetFont(str, (Font)(object)defaultValue);
                else if (typeof(T) == typeof(Rectangle))
                    return (T)(object)GetRectangle(str, (Rectangle)(object)defaultValue);
                else if (typeof(T).IsEnum)
                {
                    object obj = Enum.Parse(typeof(T), str, true);
                    if (obj is T)
                        return (T)obj;
                    else
                        return defaultValue;
                }
                else
                    return defaultValue;
            }
            else
            {
                return defaultValue;
                //throw new InvalidCastException();
            }
        }

        public static void WriteTextNode(XmlElement parent, string name, string value)
        {
            if (parent == null)
                throw new ArgumentNullException();

            var element = parent.SelectSingleNode(name) as XmlElement;
            if (element == null)
            {
                element = parent.OwnerDocument.CreateElement(name);
                parent.AppendChild(element);
            }

            element.InnerText = value;
        }

        public static string ReadTextNode(XmlElement parent, string name)
        {
            XmlNode node = parent.SelectSingleNode(name);
            if (node != null)
                return node.InnerText;
            else
                return string.Empty;
        }

        public static void WriteCDataNode(XmlElement parent, string name, string value)
        {
            XmlElement element = parent.OwnerDocument.CreateElement(name);
            XmlCDataSection cdata = parent.OwnerDocument.CreateCDataSection(value);
            element.AppendChild(cdata);
            parent.AppendChild(element);
        }

        public static string ReadCDataNode(XmlElement parent, string name)
        {
            XmlNode node = parent.SelectSingleNode(name);
            XmlNode cNode = FindCDataSection(node);
            if (cNode != null)
                return cNode.InnerText;
            else
                return string.Empty;
        }

        public static int GetIntDefault(object obj)
        {
            return GetInt(obj, default(int));
        }

        public static int? GetInt(object obj)
        {
            if (obj is int)
                return (int)obj;
            else if (obj is decimal)
                return (int)(decimal)obj;
            else if (obj is float)
                return (int)(float)obj;
            else if (obj is double)
                return (int)(double)obj;
            else if (obj is byte)
                return (int)(byte)obj;
            else if (obj is Single)
                return (int)(Single)obj;
            else if (obj is short)
                return (int)(short)obj;
            else if (obj is long)
                return (int)(long)obj;
            else if (obj is string)
            {
                int ri;
                if (int.TryParse((string)obj, out ri))
                {
                    return ri;
                }
            }

            return null;
        }

        public static int GetInt(object obj, int defaultValue)
        {
            var v = GetInt(obj);
            if (v.HasValue)
                return v.Value;
            else
                return defaultValue;
        }

        public static float? GetFloat(object value)
        {
            if (value is float)
                return (float)value;
            else if (value is decimal)
                return (float)value;
            else if (value is double)
                return (float)value;
            else if (value is int)
                return (float)value;
            else if (value is string)
            {
                float f;
                if (float.TryParse((string)value, out f))
                    return f;
            }

            return null;
        }

        public static float GetFloat(object value, float defaultValue)
        {
            float? f = GetFloat(value);
            if (f.HasValue)
                return f.Value;
            else
                return defaultValue;
        }

        public static float GetFloatDefault(string value)
        {
            return GetFloat(value, default(float));
        }

        public static long GetLong(object obj, long defaultValue)
        {
            if (obj is long)
                return (long)obj;
            else if (obj is int)
                return (long)(int)obj;
            else if (obj is decimal)
                return (long)(decimal)obj;
            else if (obj is float)
                return (long)(float)obj;
            else if (obj is double)
                return (long)(double)obj;
            else if (obj is Single)
                return (long)(Single)obj;
            else if (obj is short)
                return (long)(short)obj;
            else if (obj is string)
            {
                long ri;
                if (long.TryParse((string)obj, out ri))
                {
                    return ri;
                }
            }

            return defaultValue;
        }

        public static long GetLongDefault(object obj)
        {
            return GetLong(obj, 0);
        }

        public static decimal GetDecimalDefault(object obj)
        {
            return GetDecimal(obj, 0);
        }

        public static decimal GetDecimal(object obj, decimal nullValue)
        {
            decimal? d = GetDecimal(obj);
            if (d.HasValue)
                return d.Value;
            else
                return nullValue;
        }

        public static decimal? GetDecimal(object obj)
        {
            if (obj is decimal)
                return (decimal)obj;
            else if (obj is int)
                return (decimal)(int)obj;
            else if (obj is double)
                return (decimal)(double)obj;
            else if (obj is Single)
                return (decimal)(Single)obj;
            else if (obj is string)
            {
                decimal d;
                if (decimal.TryParse((string)obj, out d))
                    return d;
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        public static bool GetBoolDefault(object value)
        {
            return GetBool(value, false);
        }

        public static bool? GetBool(object value)
        {
            if (value == null || string.Empty.Equals(value))
                return null;

            if (value is bool)
            {
                return (bool)value;
            }
            else if (value is string)
            {
                switch (((string)value).ToLower())
                {
                    case "true":
                    case "yes":
                    case "1":
                        return true;
                    case "false":
                    case "no":
                    case "0":
                        return false;
                    default:
                        return null;
                }
            }
            else if (value is int)
            {
                return (int)value != 0;
            }
            else
            {
                return null;
            }
        }

        public static bool GetBool(object value, bool defaultValue)
        {
            bool? b = GetBool(value);
            if (b.HasValue)
                return b.Value;
            else
                return defaultValue;
        }

        public static DateTime GetDateTime(object p, DateTime defaultValue)
        {
            if (p == null || p is DBNull)
            {
                return defaultValue;
            }

            if (p is DateTime)
            {
                return (DateTime)p;
            }
            else if (p is string)
            {
                DateTime dt;
                if (DateTime.TryParse((string)p, out dt))
                    return dt;
                else
                    return defaultValue;
            }
            else
            {
                return defaultValue;
            }
        }

        public static Nullable<DateTime> GetDateTime(object p)
        {
            if (p == null || p is DBNull)
            {
                return null;
            }

            if (p is DateTime)
            {
                return (DateTime)p;
            }
            else if (p is string)
            {
                DateTime dt;
                if (DateTime.TryParse((string)p, out dt))
                    return dt;
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        public static Size GetSize(string value)
        {
            return GetSize(value, default(Size));
        }

        public static Size GetSize(string value, Size defaultValue)
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            string[] pis = value.Split(',');
            if (pis.Length != 2)
                return defaultValue;
            int w;
            int h;
            if (int.TryParse(pis[0].Trim(), out w) && int.TryParse(pis[1].Trim(), out h))
            {
                return new Size(w, h);
            }
            else
            {
                return defaultValue;
            }
        }

        public static string ToString(Size value)
        {
            return string.Format("{0}, {1}", value.Width, value.Height);
        }

        public static string ToString(Rectangle value)
        {
            return string.Format("{0}, {1}, {2}, {3}", value.X, value.Y, value.Width, value.Height);
        }

        public static Rectangle GetRectangle(string value)
        {
            return GetRectangle(value, default(Rectangle));
        }

        public static Rectangle GetRectangle(string value, Rectangle defaultValue)
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            string[] pis = value.Split(',');
            if (pis.Length != 4)
                return defaultValue;

            int x, y, w, h;
            if (int.TryParse(pis[0].Trim(), out x) 
                && int.TryParse(pis[1].Trim(), out y) 
                && int.TryParse(pis[2].Trim(), out w) 
                && int.TryParse(pis[3].Trim(), out h))
            {
                return new Rectangle(x, y, w, h);
            }
            else
            {
                return defaultValue;
            }
        }

        public static Color GetColor(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Color.Empty;
            }

            ColorConverter cc = new ColorConverter();
            try
            {
                return (Color)cc.ConvertFrom(value);
            }
            catch(System.Exception ex)
            {
                Helper.WriteLog(ex);
                return Color.Empty;
            }
        }

        public static Color GetColor(string value, Color nullValue)
        {
            Color c = GetColor(value);
            if (c.IsEmpty)
                return nullValue;
            else
                return c;
        }

        public static Point? GetPoint(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            string[] ss = value.Split(',');
            if (ss.Length != 2)
                return null;

            int[] pis = new int[2];
            if (!int.TryParse(ss[0], out pis[0]) || !int.TryParse(ss[1], out pis[1]))
                return null;

            return new Point(pis[0], pis[1]);
        }

        public static Point GetPoint(string value, Point defaultValue)
        {
            Point? pt = GetPoint(value);
            if (pt.HasValue)
                return pt.Value;
            else
                return defaultValue;
        }

        public static string ToString(Color color)
        {
            if (color.IsEmpty)
                return string.Empty;
            else if (color.IsNamedColor)
                return color.Name;
            else if (color.A < 255)
                return string.Format("{0}, {1}, {2}, {3}", color.A, color.R, color.G, color.B);
            else
                return string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
        }

        public static string ToWebColor(Color color)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
        }

        public static string ToWebColorWithoutSign(Color color)
        {
            return string.Format("{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
        }

        public static Padding? GetPadding(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            if (value.Contains(","))
            {
                string[] piStr = value.Split(',');
                if (piStr.Length == 4)
                {
                    int[] pis = new int[4];
                    if (int.TryParse(piStr[0], out pis[0])
                        && int.TryParse(piStr[1], out pis[1])
                        && int.TryParse(piStr[2], out pis[2])
                        && int.TryParse(piStr[3], out pis[3]))
                    {
                        return new Padding(pis[0], pis[1], pis[2], pis[3]);
                    }
                }
            }
            else
            {
                int pi;
                if (int.TryParse(value, out pi))
                    return new Padding(pi);
            }

            return null;
        }

        public static Padding GetPadding(string value, Padding defaultValue)
        {
            Padding? padding = GetPadding(value);
            if (padding.HasValue)
                return padding.Value;
            else
                return defaultValue;
        }

        public static string ToString(Padding padding)
        {
            if (padding.All > -1)
                return padding.All.ToString();
            else
                return string.Format("{0}, {1}, {2}, {3}", padding.Left, padding.Top, padding.Right, padding.Bottom);
        }

        public static string EncodeBase64(string code)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(code);
            return Convert.ToBase64String(bytes);
        }

        public static string DecodeBase64(string code)
        {
            byte[] bytes = Convert.FromBase64String(code);
            return Encoding.UTF8.GetString(bytes);
        }

        public static void WriteFontNode(XmlElement parentNode, string fontNodeName, Font font)
        {
            if (parentNode == null)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(fontNodeName))
                throw new ArgumentException();

            var node = parentNode.OwnerDocument.CreateElement(fontNodeName);
            WriteFontNode(node, font);
            parentNode.AppendChild(node);
        }

        public static void WriteFontNode(XmlElement node, Font font)
        {
            if (node == null)
                throw new ArgumentNullException();

            if (font != null)
            {
                node.SetAttribute("font_family", font.FontFamily.Name);
                node.SetAttribute("size", font.Size.ToString());
                node.SetAttribute("style", font.Style.ToString());
            }
            else
            {
                node.RemoveAttribute("font_family");
                node.RemoveAttribute("size");
                node.RemoveAttribute("style");
            }
        }

        public static Font ReadFontNode(XmlElement parentNode, string fontNodeName)
        {
            if (parentNode == null || fontNodeName == null)
                throw new ArgumentNullException();

            var node = parentNode.SelectSingleNode(fontNodeName) as XmlElement;
            if (node != null)
                return ReadFontNode(node);
            else
                return null;
        }

        public static Font ReadFontNode(XmlElement node)
        {
            if (node == null)
                throw new ArgumentNullException();

            string fontFamily = node.GetAttribute("font_family");
            float fontSize = GetFloat(node.GetAttribute("size"), 10);
            string fontStyleStr = node.GetAttribute("style");

            if (string.IsNullOrEmpty(fontFamily) || fontSize <= 0)
                return null;

            FontStyle fs = FontStyle.Regular;
            try
            {
                fs = (FontStyle)Enum.Parse(typeof(FontStyle), fontStyleStr);
            }
            catch(System.Exception ex)
            {
                Helper.WriteLog(ex);
            }

            if (fontSize > 0)
                return new Font(fontFamily, fontSize, fs);
            else
                return null;
        }

        public static void WriteImageNode(XmlElement parentNode, string name, Image image)
        {
            if (parentNode == null)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(name))
                throw new ArgumentException();

            var node = parentNode.OwnerDocument.CreateElement(name);
            WriteImageNode(node, image);
            parentNode.AppendChild(node);
        }

        public static void WriteImageNode(XmlElement iconNode, Image image)
        {
            if (iconNode == null || image == null)
                throw new ArgumentNullException();

            Image image2 = Blumind.Controls.PaintHelper.CopyImage(image);

            XmlCDataSection cdata = iconNode.OwnerDocument.CreateCDataSection(ImageBase64String(image2));
            iconNode.AppendChild(cdata);

            image2.Dispose();
        }

        public static void SerializeColor(XmlElement node, string name, Color color)
        {
            if (!color.IsEmpty)
            {
                ST.WriteTextNode(node, name, ST.ToString(color));
            }
        }

        public static Color DeserializeColor(XmlElement node, string name, Color colorDefault)
        {
            return ST.GetColor(ST.ReadTextNode(node, name), colorDefault);
        }

        public static string ImageBase64String(Image image)
        {
            if (image == null)
                return string.Empty;

            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    image.Save(stream, ImageFormat.Png);
                    stream.Position = 0;
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    stream.Close();
                    return Convert.ToBase64String(buffer);
                }
                catch (System.Exception ex)
                {
                    Helper.WriteLog(ex);
                    stream.Close();
                    return string.Empty;
                }
            }
        }

        public static Image ReadImageNode(XmlElement parentNode, string name)
        {
            if (parentNode == null || name == null)
                throw new ArgumentNullException();

            var node = parentNode.SelectSingleNode(name) as XmlElement;
            if (node != null)
                return ReadImageNode(node);
            else
                return null;
        }

        public static Image ReadImageNode(XmlElement iconNode)
        {
            if (iconNode == null)
                throw new ArgumentNullException();

            XmlNode node = FindCDataSection(iconNode);
            if (node != null)
            {
                byte[] buffer = Convert.FromBase64String(node.InnerText);
                if (buffer != null && buffer.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ms.Write(buffer, 0, buffer.Length);
                        ms.Position = 0;
                        try
                        {
                            return Image.FromStream(ms);
                        }
                        catch(System.Exception ex)
                        {
                            Helper.WriteLog(ex);
                        }
                    }
                }
            }

            return null;
        }

        public static bool HasImageNode(XmlElement iconNode)
        {
            return HasCDataSection(iconNode);
        }

        public static bool HasImageNode(XmlElement parentNode, string name)
        {
            if (parentNode == null || name == null)
                throw new ArgumentNullException();

            var node = parentNode.SelectSingleNode(name) as XmlElement;
            if (node != null)
                return HasImageNode(node);
            else
                return false;
        }

        public static bool HasCDataSection(XmlElement node)
        {
            if (node != null && node.HasChildNodes)
            {
                XmlNode cNode = node.FirstChild;
                while (cNode != null)
                {
                    if (cNode is XmlCDataSection)
                    {
                        return true;
                    }
                    cNode = cNode.NextSibling;
                }
            }

            return false;
        }

        private static XmlNode FindCDataSection(XmlNode node)
        {
            if (node != null && node.HasChildNodes)
            {
                XmlNode cNode = node.FirstChild;
                while (cNode != null)
                {
                    if (cNode is XmlCDataSection)
                    {
                        return cNode;
                    }
                    cNode = cNode.NextSibling;
                }
            }

            return null;
        }

        public static string JoinArray(string[] array, string separator)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(separator);
                }

                sb.Append(array[i]);
            }

            return sb.ToString();
        }

        public static TopicShape GetTopicShape(string str, TopicShape defaultValue)
        {
            int i;
            if (!int.TryParse(str, out i))
                return defaultValue;

            try
            {
                return (TopicShape)i;
            }
            catch(System.Exception ex)
            {
                Helper.WriteLog(ex);
                return defaultValue;
            }
        }

        public static T GetEnumValue<T>(string text, T defaultValue)
            where T : struct
        {
            try
            {
                object obj = Enum.Parse(typeof(T), text, true);
                if (obj is T)
                    return (T)obj;

                var t = Enum.GetUnderlyingType(typeof(T));
                if (t == typeof(int))
                {
                    int v = 0;
                    if (int.TryParse(text, out v))
                        return (T)Enum.ToObject(typeof(T), v);
                }
                else if (t == typeof(short))
                {
                    short v = 0;
                    if (short.TryParse(text, out v))
                        return (T)Enum.ToObject(typeof(T), v);
                }
                else if (t == typeof(sbyte))
                {
                    sbyte v = 0;
                    if (sbyte.TryParse(text, out v))
                        return (T)Enum.ToObject(typeof(T), v);
                }
                else if (t == typeof(long))
                {
                    long v = 0;
                    if (long.TryParse(text, out v))
                        return (T)Enum.ToObject(typeof(T), v);
                }
                else if (t == typeof(uint))
                {
                    uint v = 0;
                    if (uint.TryParse(text, out v))
                        return (T)Enum.ToObject(typeof(T), v);
                }
                else if (t == typeof(ushort))
                {
                    ushort v = 0;
                    if (ushort.TryParse(text, out v))
                        return (T)Enum.ToObject(typeof(T), v);
                }
                else if (t == typeof(byte))
                {
                    byte v = 0;
                    if (byte.TryParse(text, out v))
                        return (T)Enum.ToObject(typeof(T), v);
                }
                else if (t == typeof(ulong))
                {
                    ulong v = 0;
                    if (ulong.TryParse(text, out v))
                        return (T)Enum.ToObject(typeof(T), v);
                }

                return defaultValue;
            }
            catch (System.Exception ex)
            {
                Helper.WriteLog(ex);
                return defaultValue;
            }

            //
            /*string chartType = element.GetAttribute("type");
            var cts = from ct in Enum.GetValues(typeof(ChartType)).Cast<ChartType>()
                      select new
                      {
                          Key = ct.ToString(),
                          Value = ct
                      };
            foreach (var ct in cts)
            {
                if (StringComparer.OrdinalIgnoreCase.Equals(ct.Key, chartType))
                {
                    this.Type = ct.Value;
                    break;
                }
            }*/
        }

        public static string EnumToString<EnumType>(EnumType value)
        {
            if (LanguageManage.Current != null)
            {
                LanguageIDAttribute lid = Attribute.GetCustomAttribute(
                    typeof(EnumType).GetMember(value.ToString())[0], typeof(LanguageIDAttribute)
                    ) as LanguageIDAttribute;

                if (lid != null && !string.IsNullOrEmpty(lid.LanguageID))
                {
                    return lid.GetText();
                }
                else
                {
                    return LanguageManage.Current.GetText(value.ToString());
                }
            }

            return value.ToString();
        }

        public static MindMapLayoutType GetLayoutType(string str)
        {
            if (string.IsNullOrEmpty(str))
                return MindMapLayoutType.MindMap;

            switch (str.Trim().ToUpper())
            {
                case "ORGANIZATION_DOWN":
                    return MindMapLayoutType.OrganizationDown;
                case "ORGANIZATION_UP":
                    return MindMapLayoutType.OrganizationUp;
                case "TREE_LEFT":
                    return MindMapLayoutType.TreeLeft;
                case "TREE_RIGHT":
                    return MindMapLayoutType.TreeRight;
                case "LOGIC_LEFT":
                    return MindMapLayoutType.LogicLeft;
                case "LOGIC_RIGHT":
                    return MindMapLayoutType.LogicRight;
                case "MIND_MAP":
                default:
                    return MindMapLayoutType.MindMap;
            }
        }

        public static string ToString(MindMapLayoutType layoutType)
        {
            switch (layoutType)
            {
                case MindMapLayoutType.MindMap:
                    return "MIND_MAP";
                case MindMapLayoutType.OrganizationDown:
                    return "ORGANIZATION_DOWN";
                case MindMapLayoutType.OrganizationUp:
                    return "ORGANIZATION_UP";
                case MindMapLayoutType.TreeLeft:
                    return "TREE_LEFT";
                case MindMapLayoutType.TreeRight:
                    return "TREE_RIGHT";
                case MindMapLayoutType.LogicLeft:
                    return "LOGIC_LEFT";
                case MindMapLayoutType.LogicRight:
                    return "LOGIC_RIGHT";
                default:
                    return string.Empty;
            }
        }

        public static string ToString(Point point)
        {
            return string.Format("{0}, {1}", point.X, point.Y);
        }

        public static string ToString(Keys keys)
        {
            return ToString(keys, "+");
        }

        public static string ToString(Keys keys, string separator)
        {
            StringBuilder sb = new StringBuilder();

            if ((keys & Keys.Control) != Keys.None)
            {
                sb.Append("Ctrl");
            }

            if ((keys & Keys.Shift) != Keys.None)
            {
                if (sb.Length > 0)
                    sb.Append(separator);
                sb.Append("Shift");
            }

            if ((keys & Keys.Alt) != Keys.None)
            {
                if (sb.Length > 0)
                    sb.Append(separator);
                sb.Append("Alt");
            }

            keys = keys & ~(Keys.Control | Keys.Shift | Keys.Alt);
            if (keys != Keys.None)
            {
                if (sb.Length > 0)
                    sb.Append(separator);

                switch (keys)
                {
                    case Keys.Add:
                        sb.Append(Lang._("Key_Add"));
                        break;
                    case Keys.Subtract:
                        sb.Append(Lang._("Key_Subtract"));
                        break;
                    case Keys.Multiply:
                        sb.Append(Lang._("Key_Multiply"));
                        break;
                    case Keys.Left:
                        sb.Append(Lang._("Key_Left"));
                        break;
                    case Keys.Up:
                        sb.Append(Lang._("Key_Up"));
                        break;
                    case Keys.Right:
                        sb.Append(Lang._("Key_Right"));
                        break;
                    case Keys.Down:
                        sb.Append(Lang._("Key_Down"));
                        break;
                    case Keys.Space:
                        sb.Append(Lang._("Key_Space"));
                        break;
                    case Keys.Enter:
                        sb.Append(Lang._("Key_Enter"));
                        break;
                    case Keys.Escape:
                        sb.Append(Lang._("Key_Escape"));
                        break;
                    default:
                        sb.Append(keys.ToString());
                        break;
                }
            }

            return sb.ToString();
        }

        public static string AddAccelerator(string str, char accelerator)
        {
            if (accelerator > 0)
            {
                int index = str.IndexOf(new string(accelerator, 1), System.StringComparison.OrdinalIgnoreCase);
                if (index > -1)
                    str.Insert(index, "&");
                else
                    str += string.Format("(&{0})", accelerator);
            }

            return str;
        }

        public static bool IsSameFile(string file1, string file2)
        {
            if (file1 == file2)
                return true;
            if (file1 == null || file2 == null)
                return false;
            if (file1 == string.Empty || file2 == string.Empty)
                return false;

            Uri uri1, uri2;
            if (!Uri.TryCreate(file1, UriKind.RelativeOrAbsolute, out uri1)
                || !Uri.TryCreate(file2, UriKind.RelativeOrAbsolute, out uri2)
                || uri1 != uri2)
                return false;

            return true;
        }

        public static string[] GetStringArray(string value, string[] defaultValue)
        {
            if (value == null)
                return defaultValue;
            else if (value == string.Empty)
                return new string[0];

            var list = new List<string>();
            var sb = new StringBuilder();
            var lastc = '\0';
            foreach (var c in value)
            {
                if (c == ';' && lastc != '\\')
                    list.Add(sb.ToString());
                sb.Append(c);
                lastc = c;
            }

            return list.ToArray();
        }

        public static string ToString(string[] value)
        {
            if (value.IsNullOrEmpty())
                return null;

            var sb = new StringBuilder();
            foreach (var v in value)
            {
                foreach (var c in v)
                {
                    if (c == ';')
                        sb.Append('\\');
                    sb.Append(c);
                }

                sb.Append(";");
            }

            return sb.ToString();
        }

        public static string ToString(Font font)
        {
            if (font == null)
                return null;
            else
                return font.ToString();
        }

        public static Font GetFont(string value, Font defaultValue)
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            return new FontConverter().ConvertFromString(value) as Font;
        }

        public static string ToString(object value, bool localization)
        {
            if (value == null)
                return null;

            if (value is decimal)
                return ((decimal)value).ToString("G0");
            else if (value is double)
                return ((double)value).ToString("G0");
            else if (value is float)
                return ((float)value).ToString("G0");
            else if (value is DateTime)
                return value.ToString();
            else if (value is Size)
                return ToString((Size)value);
            else if (value is Point)
                return ToString((Point)value);
            else if (value is string[])
                return ToString((string[])value);
            else if (value is Font)
                return ToString((Font)value);
            else
            {
                if (localization)
                    return Lang._(value.ToString());
                else
                    return value.ToString();
            }
        }

        public static string HtmlToText(string html)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            Regex reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            html = reg.Replace(html, "");
            return System.Web.HttpUtility.HtmlDecode(html).Trim();
        }

        public static string EscapeFileName(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return filename;

            var illegal = Path.GetInvalidFileNameChars();
            var charts = from c in filename
                         where !illegal.Contains(c)
                         select c;
            return new string(charts.ToArray());
        }

        public static string EscapePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            var illegal = Path.GetInvalidPathChars();
            var charts = from c in path
                         where !illegal.Contains(c)
                         select c;
            return new string(charts.ToArray());
        }

        public static string ToString(bool flag)
        {
            return flag.ToString();
        }

        public static string RemoveUnvisibleCharts(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var regex = new Regex("\\s");
            return regex.Replace(text, string.Empty);
        }
    }
}
