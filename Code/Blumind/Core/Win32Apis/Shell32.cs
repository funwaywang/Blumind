using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Blumind.Controls.OS
{
    public static class Shell32
    {
        const string DllName = "Shell32";

        [DllImport(DllName)]
        public static extern void DragAcceptFiles(IntPtr hWnd, bool fAccept);

        [DllImport(DllName)]
        public static extern int DragQueryFile(IntPtr hDrop, uint iFile, StringBuilder lpszFile, uint cch);

        [DllImport(DllName)]
        public static extern void DragFinish(IntPtr hDrop);
    }
}
