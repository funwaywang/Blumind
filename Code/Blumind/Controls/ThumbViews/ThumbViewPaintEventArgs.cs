using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Blumind.Controls
{
    class ThumbViewPaintEventArgs : EventArgs
    {
        public ThumbViewPaintEventArgs(ThumbView view, Graphics graphics, ThumbItem item, Font font)
        {
            View = view;
            Graphics = graphics;
            Item = item;
            Font = font;
        }

        public ThumbView View { get; private set; }

        public Graphics Graphics { get; private set; }

        public ThumbItem Item { get; private set; }

        public Font Font { get; private set; }

        public void PaintBackground()
        {
            if (View != null)
            {
                View.PaintCellBackground(this, Item);
            }
        }
    }
}
