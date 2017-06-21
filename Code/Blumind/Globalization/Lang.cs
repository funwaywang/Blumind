using System.Windows.Forms;
using Blumind.Core;

namespace Blumind.Globalization
{
    static class Lang
    {
        public static Language CurrentLanguage
        {
            get { return LanguageManage.Current; }
        }

        public static string _(string name)
        {
            return GetText(name);
        }

        public static string _(string name, string defaultValue)
        {
            return GetText(name, defaultValue);
        }

        public static string _(string name, bool withEllipsis, bool withColon, char accelerator)
        {
            return GetText(name, withEllipsis, withColon, accelerator);
        }

        public static string _(string name, Keys keys)
        {
            return GetText(name, keys);
        }

        public static string _2(string name1, string name2)
        {
            return GetText2(name1, name2);
        }

        public static string GetText(string name)
        {
            if (LanguageManage.Current != null)
                return LanguageManage.Current.GetText(name);
            else
                return name;
        }

        public static string GetText(string name, string defaultValue)
        {
            if (LanguageManage.Current != null)
                return LanguageManage.Current.GetText(name, defaultValue);
            else
                return defaultValue;
        }

        public static string GetText2(string name1, string name2)
        {
            if (LanguageManage.Current != null)
                return LanguageManage.Current.GetText2(name1, name2);
            else if (name1 == null)
                return name2;
            else
                return name1;
        }

        public static string GetText(string name, Keys keys)
        {
            return string.Format("{0} ({1})", GetText(name), ST.ToString(keys));
        }

        public static string GetText(string name, bool withEllipsis, bool withColon, char accelerator)
        {
            return _FormatText(GetText(name), withEllipsis, withColon, accelerator);
        }

        public static string GetTextWithEllipsis(string name)
        {
            if (LanguageManage.Current != null)
                return LanguageManage.Current.GetText(name) + "...";
            else
                return name + "...";
        }

        public static string GetTextWithColon(string name)
        {
            return GetText(name, false, true, '\0');
        }

        public static string GetTextWithAccelerator(string name, bool withEllipsis, char accelerator)
        {
            return GetText(name, true, false, accelerator);
        }

        public static string GetTextWithAccelerator(string name, char accelerator)
        {
            return GetText(name, false, false, accelerator);
        }

        public static string Format(string name, params object[] args)
        {
            return Format(name, true, args);
        }

        public static string Format(string name, bool withArgs, params object[] args)
        {
            if (withArgs && args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    object arg = args[i];
                    if (arg is string)
                    {
                        args[i] = GetText((string)arg);
                    }
                }
            }

            return string.Format(GetText(name), args);
        }

        private static string _FormatText(string str, bool withEllipsis, bool withColon, char accelerator)
        {
            if (str == null)
                return str;

            if (accelerator > 0)
                str = ST.AddAccelerator(str, accelerator);

            if (withEllipsis)
                str += "...";

            if (withColon)
                str += ":";

            return str;
        }
    }
}
