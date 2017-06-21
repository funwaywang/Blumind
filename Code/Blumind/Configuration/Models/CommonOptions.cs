using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Blumind.Configuration
{
    static class CommonOptions
    {
        public static class Appearances
        {
            public static string UIThemeName
            {
                get { return Options.Current.GetString(OptionNames.Appearances.UIThemeName); }
                set { Options.Current.GetString(OptionNames.Appearances.UIThemeName); }
            }
        }

        public static class Localization
        {
            public static string LanguageID
            {
                get { return Options.Current.GetString(OptionNames.Localization.LanguageID); }
                set { Options.Current.GetString(OptionNames.Localization.LanguageID); }
            }
        }

        public static class Charts
        {
            public static Font DefaultFont
            {
                get { return Options.Current.GetValue<Font>(OptionNames.Charts.DefaultFont); }
                set { Options.Current.SetValue(OptionNames.Charts.DefaultFont, value); }
            }

            public static bool ShowRemarkIcon
            {
                get { return Options.Current.GetBool(OptionNames.Charts.ShowRemarkIcon, true); }
                set { Options.Current.SetValue(OptionNames.Charts.ShowRemarkIcon, value); }
            }

            public static bool ShowLineArrowCap
            {
                get { return Options.Current.GetBool(OptionNames.Charts.ShowLineArrowCap, true); }
                set { Options.Current.SetValue(OptionNames.Charts.ShowLineArrowCap, value); }
            }

            public static int FoldingButtonSize
            {
                get { return Options.Current.GetValue(OptionNames.Charts.FoldingButtonSize, 13); }
            }
        }
    }
}
