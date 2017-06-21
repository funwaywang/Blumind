using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blumind.Configuration
{
    class OptionNames
    {
        public static class Appearances
        {
            public const string UIThemeName = "Appearance.UIThemeName";
        }

        //public static class Associations
        //{
        //    public const string Bmd = "Associations.Bmd";
        //}

        public static class Charts
        {
            public const string DefaultFont = "Charts.DefaultFont";

            public const string ShowRemarkIcon = "Charts.ShowRemarkIcon";

            public const string ShowLineArrowCap = "Charts.ShowLineArrowCap";

            public const string FoldingButtonSize = "Charts.FoldingButtonSize";

            public const string DefaultChartTheme = "Charts.DefaultTheme";
        }

        public class Customizations
        {
            public const string MainWindowMaximized = "MainWindowMaximized";

            public const string MainWindowSize = "MainWindowSize";
        }

        public static class Localization
        {
            public const string LanguageID = "Localization.LanguageID";
        }

        public static class Miscellaneous
        {
            public const string SaveTabs = "Miscellaneous.SaveTabs";

            public const string LastOpenTabs = "LastOpenTabs";

            public const string SaveRecentFiles = "SaveRecentFiles";

            public const string SelectChartsMethod = "ChartsToExport";

            public const string ExportDocumentType = "ExportDocumentType";

            public const string WebImageEmbedIn = "WebImageEmbedIn";
        }

        public static class PageSettigs
        {
            public const string PrintDocumentTitle = "PageSettigs.PrintDocumentTitle";

            public const string Landscape = "PageSettigs.Landscape";
        }
    }
}
