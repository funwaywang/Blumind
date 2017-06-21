using System.Runtime.InteropServices;

namespace Blumind.Controls.OS
{
    [StructLayout(LayoutKind.Sequential)]
    public class RECT
    {
        int left; 
        int top; 
        int right; 
        int bottom;

        public RECT(System.Drawing.Rectangle rect)
            : this(rect.Left, rect.Top, rect.Right, rect.Bottom)
        {
        }

        public RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class XFORM
    {
        float eM11;
        float eM12;
        float eM21;
        float eM22;
        float eDx;
        float eDy;

        public XFORM(float eM11, float eM12, float eM21, float eM22, float eDx, float eDy)
        {
            this.eM11 = eM11;
            this.eM12 = eM12;
            this.eM21 = eM21;
            this.eM22 = eM22;
            this.eDx = eDx;
            this.eDy = eDy;
        }
    }
}
