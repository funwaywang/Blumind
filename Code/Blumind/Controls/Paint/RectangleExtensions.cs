using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Blumind.Controls
{
    static class RectangleExtensions
    {
        public static Rectangle Inflate(this Rectangle rectangle, Padding padding)
        {
            rectangle.X += padding.Left;
            rectangle.Y += padding.Top;
            rectangle.Width -= padding.Horizontal;
            rectangle.Height -= padding.Vertical;

            return rectangle;
        }
    }
}
