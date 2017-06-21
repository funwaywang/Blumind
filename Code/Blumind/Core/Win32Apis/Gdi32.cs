using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Blumind.Controls.OS
{
    public static class Gdi32
    {
        const string DllName = "Gdi32";

        [DllImport(DllName)]
        public static extern int BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight,
            IntPtr hdcSrc, int nXSrc, int nYSrc, ROP2 dwRop);

        [DllImport(DllName)]
        public static extern IntPtr CreateSolidBrush(uint crColor);

        [DllImport(DllName)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport(DllName)]
        public static extern RopModes SetROP2(IntPtr hdc, RopModes fnDrawMode);

        [DllImport(DllName)]
        public static extern bool Rectangle(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect );

        [DllImport(DllName)]
        public static extern bool Ellipse(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport(DllName)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport(DllName)]
        public static extern IntPtr GetStockObject(StockObjects fnObject);

        [DllImport(DllName)]
        public static extern int GetDeviceCaps(IntPtr hdc, DeviceCapsIndex nIndex);

        [DllImport(DllName)]
        public static extern IntPtr CreatePen(PenStyles fnPenStyle, int nWidth, uint crColor);

        [DllImport(DllName)]
        public static extern int SetGraphicsMode(IntPtr hdc, GraphicsModes iMode);

        [DllImport(DllName)]
        public static extern bool SetWorldTransform(IntPtr hdc, [MarshalAs(UnmanagedType.LPStruct)]XFORM lpXform);

        [DllImport(DllName)]
        public static extern bool GetWorldTransform(IntPtr hdc, [MarshalAs(UnmanagedType.LPStruct)]XFORM lpXform);

        [DllImport(DllName)]
        public static extern int SaveDC(IntPtr hdc);

        [DllImport(DllName)]
        public static extern bool RestoreDC(IntPtr hdc, int nSavedDC);

        [DllImport(DllName)]
        public static extern bool SetViewportOrgEx(IntPtr hdc, int X, int Y, [MarshalAs(UnmanagedType.LPStruct)]Point lpPoint);

        [DllImport(DllName)]
        public static extern bool SetViewportOrgEx(IntPtr hdc, int X, int Y, IntPtr lpPoint);

        [DllImport(DllName)]
        public static extern bool MoveToEx(IntPtr hdc, int X, int Y, IntPtr point);

        [DllImport(DllName)]
        public static extern bool LineTo(IntPtr hdc, int nXEnd, int nYEnd);

        public static uint RGB(byte red, byte green, byte blue)
        {
            return red | ((uint)green << 8) | ((uint)blue << 16);
        }

        public static uint RGB(Color color)
        {
            return RGB(color.R, color.G, color.B);
        }
    }
}
