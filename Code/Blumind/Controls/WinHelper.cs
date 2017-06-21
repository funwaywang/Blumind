using System;
using System.Drawing;
using System.Windows.Forms;
using Blumind.Controls.OS;

namespace Blumind.Controls
{
    public static class WinHelper
    {
        public static Bitmap CopyImage(Control control, int x, int y, int width, int height)
        {
            if (control == null)
                return null;
            IntPtr handle = control.Handle;
            if (handle == IntPtr.Zero)
                return null;

            IntPtr wdc = User32.GetWindowDC(handle);
            if (wdc == IntPtr.Zero)
                return null;

            Bitmap bmp = new Bitmap(width, height);
            using (Graphics grf = Graphics.FromImage(bmp))
            {
                IntPtr dc = grf.GetHdc();
                Gdi32.BitBlt(dc, 0, 0, width, height, wdc, x, y, ROP2.SRCCOPY | ROP2.CAPTUREBLT);
            }

            User32.ReleaseDC(handle, wdc);

            return bmp;
        }

        public static Bitmap CopyImage(Control control)
        {
            if (control == null)
                return null;
            else
                return CopyImage(control, 0, 0, control.Width, control.Height);
        }
    }
}
