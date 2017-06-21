using System;
using System.Runtime.InteropServices;

namespace Blumind.Controls.OS
{
    public static class User32
    {
        const string DllName = "User32";

        [DllImport(DllName)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport(DllName)]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport(DllName)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport(DllName)]
        public static extern int ShowWindow(IntPtr hWnd, ShowWindowFlags nCmdShow);

        [DllImport(DllName)]
        public static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport(DllName)]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport(DllName)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport(DllName)]
        public static extern bool IsWindowEnabled(IntPtr hWnd);

        [DllImport(DllName)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport(DllName)]
        public static extern IntPtr GetCapture();

        [DllImport(DllName)]
        public static extern bool IsChild(IntPtr hWndParent, IntPtr hWnd);

        [DllImport(DllName)]
        public static extern bool ReleaseCapture();

        [DllImport(DllName)]
        public static extern bool SetCursorPos(int X, int Y);

        //[DllImport(DllName)]
        //public static extern int SetScrollInfo(IntPtr hwnd, Scrollbars fnBar, [MarshalAs(UnmanagedType.LPStruct)]ScrollInfo lpsi, bool fRedraw);

        //[DllImport(DllName)]
        //public static extern bool GetScrollBarInfo(IntPtr hwnd, Scrollbars fnBar, [MarshalAs(UnmanagedType.LPStruct)]ScrollInfo lpsi);

        [DllImport(DllName)]
        public static extern int GetSystemMetrics(SystemMetrics nIndex);

        [DllImport(DllName)]
        public static extern int SetWindowLong(IntPtr hWnd, GetWindowLongFlags nIndex, int dwNewLong);

        [DllImport(DllName)]
        public static extern int GetWindowLong(IntPtr hWnd, GetWindowLongFlags nIndex);

        [DllImport(DllName)]
        public static extern int SetClassLong(IntPtr hWnd, GetClassLongFlags nIndex, int dwNewLong);

        [DllImport(DllName)]
        public static extern int GetClassLong(IntPtr hWnd, GetClassLongFlags nIndex);

        [DllImport(DllName)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport(DllName)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(DllName)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport(DllName)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, uint wParam, int lParam);

        [DllImport(DllName)]
        public static extern IntPtr LoadCursor(IntPtr hInstance, string lpCursorName);

        [DllImport(DllName)]
        public static extern IntPtr LoadCursorFromFile(string fileName);

        [DllImport(DllName)]
        public static extern IntPtr SetCursor(IntPtr cursorHandle);

        [DllImport(DllName)]
        public static extern uint DestroyCursor(IntPtr cursorHandle);

        [DllImport(DllName, SetLastError = true)]
        public static extern uint RegisterClipboardFormat(string lpszFormat);

        [DllImport(DllName)]
        public static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport(DllName, SetLastError = true)]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport(DllName, SetLastError = true)]
        public static extern bool CloseClipboard();
    }
}
