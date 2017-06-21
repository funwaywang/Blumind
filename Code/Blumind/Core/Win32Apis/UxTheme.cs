using System;
using System.Runtime.InteropServices;

namespace Blumind.Controls.OS
{
    static class UxTheme
    {
        const string DllName = "UxTheme";

        [DllImport(DllName)]
        public static extern IntPtr OpenThemeData(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string pszClassList);

        [DllImport(DllName)]
        public static extern IntPtr CloseThemeData(IntPtr hTheme);

        [DllImport(DllName)]
        public static extern IntPtr DrawThemeBackground(IntPtr hTheme, IntPtr hdc,
            ThemeParts part, int iStateId, RECT pRect, RECT pClipRect);
        
        [DllImport(DllName)]
        public static extern IntPtr GetThemeMetric(IntPtr hTheme, IntPtr hdc, ThemeParts iPartId, 
            int iStateId, UxThemeProperties iPropId, ref int piVal);

        [DllImport(DllName)]
        public static extern IntPtr GetThemePartSize(IntPtr hTheme, IntPtr hdc, ThemeParts iPartId, int iStateId,
            [MarshalAs(UnmanagedType.Struct)] SIZE prc, THEMESIZE eSize, out SIZE psz);

        [DllImport(DllName)]
        public static extern IntPtr GetThemePartSize(IntPtr hTheme, IntPtr hdc, ThemeParts iPartId, int iStateId,
            IntPtr prc, THEMESIZE eSize, out SIZE psz);
    }
}
