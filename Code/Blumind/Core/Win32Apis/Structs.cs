using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Blumind.Controls.OS
{
    struct SIZE
    {
        public int cx;
        public int cy;

        public SIZE(int width, int height)
        {
            cx = width;
            cy = height;
        }

        public SIZE(Size size)
        {
            cx = size.Width;
            cy = size.Height;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        public IntPtr lpData;
    }
}
