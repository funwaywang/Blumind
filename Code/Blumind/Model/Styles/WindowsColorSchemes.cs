using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Drawing;

namespace Blumind.Model.Styles
{
    class WindowsColorSchemes
    {
        public static ChartThemeFolder LoadThemes()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                return LoadThemesFromRes();
            }
            else
            {
                return LoadThemesFromReg();
            }
        }

        private static ChartThemeFolder LoadThemesFromRes()
        {
            return ChartThemeManage.LoadFromXml(Properties.Resources.windows_themes);
        }

        private static ChartThemeFolder LoadThemesFromReg()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Appearance\Schemes", false);
            if (key == null)
                return null;

            ChartThemeFolder folder = new ChartThemeFolder("Windows Color Schemes");
            string[] schemes = key.GetValueNames();
            foreach (string scheme in schemes)
            {
                int si = scheme.IndexOf('(');
                if (si > -1 && folder.HasTheme(scheme.Substring(0, si).Trim()))
                    continue;

                ChartTheme theme = new ChartTheme(scheme);
                LoadWindowScheme(theme, (byte[])key.GetValue(scheme));
                folder.Themes.Add(theme);
            }

            return folder;
        }

        private static void LoadWindowScheme(ChartTheme theme, byte[] data)
        {
            const int COLOR_3DDKSHADOW = 21;
            //const int COLOR_3DFACE = 15;
            //const int COLOR_3DHIGHLIGHT = 20;
            //const int COLOR_3DHILIGHT = 20;
            //const int COLOR_3DLIGHT = 22;
            //const int COLOR_3DSHADOW = 16;
            //const int COLOR_ACTIVEBORDER = 10;
            const int COLOR_ACTIVECAPTION = 2;
            //const int COLOR_APPWORKSPACE = 12;
            //const int COLOR_BACKGROUND = 1;
            const int COLOR_BTNFACE = 15;
            //const int COLOR_BTNHIGHLIGHT = 20;
            const int COLOR_BTNSHADOW = 16;
            const int COLOR_BTNTEXT = 18;
            const int COLOR_CAPTIONTEXT = 9;
            //const int COLOR_DESKTOP = 1;
            const int COLOR_GRADIENTACTIVECAPTION = 27;
            //const int COLOR_GRADIENTINACTIVECAPTION = 28;
            //const int COLOR_GRAYTEXT = 17;
            const int COLOR_HIGHLIGHT = 13;
            //const int COLOR_HIGHLIGHTTEXT = 14;
            const int COLOR_HOTLIGHT = 26;
            //const int COLOR_INACTIVEBORDER = 11;
            //const int COLOR_INACTIVECAPTION = 3;
            //const int COLOR_INACTIVECAPTIONTEXT = 19;
            //const int COLOR_INFOBK = 24;
            //const int COLOR_INFOTEXT = 23;
            //const int COLOR_MENU = 4;
            //const int COLOR_MENUHILIGHT = 29;
            //const int COLOR_MENUBAR = 30;
            //const int COLOR_MENUTEXT = 7;
            //const int COLOR_SCROLLBAR = 0;
            const int COLOR_WINDOW = 5;
            //const int COLOR_WINDOWFRAME = 6;
            const int COLOR_WINDOWTEXT = 8;

            theme.SetIsInternal(true);

            theme.BackColor = GetColor(data, COLOR_WINDOW);
            theme.ForeColor = GetColor(data, COLOR_WINDOWTEXT);
            theme.LineColor = GetColor(data, COLOR_3DDKSHADOW);
            theme.BorderColor = GetColor(data, COLOR_BTNSHADOW);
            theme.NodeBackColor = GetColor(data, COLOR_BTNFACE);
            theme.NodeForeColor = GetColor(data, COLOR_BTNTEXT);
            theme.RootBackColor = GetColor(data, COLOR_ACTIVECAPTION);
            theme.RootForeColor = GetColor(data, COLOR_CAPTIONTEXT);
            theme.RootBorderColor = GetColor(data, COLOR_GRADIENTACTIVECAPTION);
            theme.SelectColor = GetColor(data, COLOR_HIGHLIGHT);
            theme.HoverColor = GetColor(data, COLOR_HOTLIGHT);
            theme.LinkLineColor = GetColor(data, COLOR_ACTIVECAPTION);
            
        //<back_color>Window</back_color>
        //<fore_color>WindowText</fore_color>
        //<line_color>ControlDarkDark</line_color>
        //<border_color>ControlDark</border_color>
        //<node_back_color>Control</node_back_color>
        //<node_fore_color>ControlText</node_fore_color>
        //<root_back_color>ActiveCaption</root_back_color>
        //<root_fore_color>ActiveCaptionText</root_fore_color>
        //<root_border_color>GradientActiveCaption</root_border_color>
        //<select_color>Highlight</select_color>
        //<hover_color>HotTrack</hover_color>
        }

        private static Color GetColor(byte[] data, int index)
        {
            int bi = 0x254 + index * 4;
            if (bi + 4 >= data.Length)
                return Color.Empty;
            else
                return Color.FromArgb(data[bi], data[bi + 1], data[bi + 2]);
        }
    }
}
