using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
using System.Drawing;

namespace Blumind.Controls.Aero
{
    internal static class DWMMessages
    {
        internal const int WM_DWMCOMPOSITIONCHANGED = 0x031E;
        internal const int WM_DWMNCRENDERINGCHANGED = 0x031F;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Margins
    {
        public int LeftWidth;      // width of left border that retains its size
        public int RightWidth;     // width of right border that retains its size
        public int TopHeight;      // height of top border that retains its size
        public int BottomHeight;   // height of bottom border that retains its size

        public Margins(bool fullWindow)
        {
            LeftWidth = RightWidth = TopHeight = BottomHeight = (fullWindow ? -1 : 0);
        }
    };

    [StructLayout(LayoutKind.Sequential)]
    internal class DWM_BLURBEHIND
    {
        public uint dwFlags;
        [MarshalAs(UnmanagedType.Bool)]
        public bool fEnable;
        public IntPtr hRegionBlur;
        [MarshalAs(UnmanagedType.Bool)]
        public bool fTransitionOnMaximized;

        public const uint DWM_BB_ENABLE = 0x00000001;
        public const uint DWM_BB_BLURREGION = 0x00000002;
        public const uint DWM_BB_TRANSITIONONMAXIMIZED = 0x00000004;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class DWM_THUMBNAIL_PROPERTIES
    {
        public uint dwFlags;
        public NativeRect rcDestination;
        public NativeRect rcSource;
        public byte opacity;
        [MarshalAs(UnmanagedType.Bool)]
        public bool fVisible;
        [MarshalAs(UnmanagedType.Bool)]
        public bool fSourceClientAreaOnly;
        public const uint DWM_TNP_RECTDESTINATION = 0x00000001;
        public const uint DWM_TNP_RECTSOURCE = 0x00000002;
        public const uint DWM_TNP_OPACITY = 0x00000004;
        public const uint DWM_TNP_VISIBLE = 0x00000008;
        public const uint DWM_TNP_SOURCECLIENTAREAONLY = 0x00000010;
    }

    internal enum CompositionEnable
    {
        Disable = 0,
        Enable = 1
    }

    /// <summary>
    /// Internal class that contains interop declarations for 
    /// functions that are not benign and are performance critical. 
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static class DesktopWindowManagerNativeMethods
    {
        [DllImport("DwmApi.dll")]
        internal static extern int DwmExtendFrameIntoClientArea(
            IntPtr hwnd,
            ref Margins m);

        [DllImport("DwmApi.dll", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DwmIsCompositionEnabled();

        [DllImport("DwmApi.dll")]
        internal static extern int DwmEnableComposition(
            CompositionEnable compositionAction);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr hwnd, [Out] out NativeRect rect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetClientRect(IntPtr hwnd, [Out] out NativeRect rect);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmEnableBlurBehindWindow(
            IntPtr hWnd, DWM_BLURBEHIND pBlurBehind);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmExtendFrameIntoClientArea(
            IntPtr hWnd, Margins pMargins);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmEnableComposition(bool bEnable);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmGetColorizationColor(
            out int pcrColorization,
            [MarshalAs(UnmanagedType.Bool)]out bool pfOpaqueBlend);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern IntPtr DwmRegisterThumbnail(
            IntPtr dest, IntPtr source);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmUnregisterThumbnail(IntPtr hThumbnail);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmUpdateThumbnailProperties(
            IntPtr hThumbnail, DWM_THUMBNAIL_PROPERTIES props);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmQueryThumbnailSourceSize(
            IntPtr hThumbnail, out Size size);
    }
}
