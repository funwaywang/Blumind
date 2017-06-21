using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Blumind.Controls
{
    public class ChartPaintEventArgs : PaintEventArgs
    {
        private Brush _BackBrush = null;
        private Brush _ForeBrush = null;
        private Font _Font = null;
        private Rectangle _LogicViewPort = Rectangle.Empty;

        public ChartPaintEventArgs(Graphics graphics, Rectangle clipRect)
            :base(graphics, clipRect)
        {
        }

        public ChartPaintEventArgs(PaintEventArgs e)
            :base(e.Graphics, e.ClipRectangle)
        {
        }

        public Brush BackBrush
        {
            get { return _BackBrush; }
            set { _BackBrush = value; }
        }

        public Brush ForeBrush
        {
            get { return _ForeBrush; }
            set { _ForeBrush = value; }
        }

        public Font Font
        {
            get { return _Font; }
            set { _Font = value; }
        }

        public Rectangle LogicViewPort
        {
            get { return _LogicViewPort; }
            set { _LogicViewPort = value; }
        }

        public void CopyTo(ChartPaintEventArgs e)
        {
            e.BackBrush = this.BackBrush;
            e.ForeBrush = this.ForeBrush;
            e.Font = this.Font;
        }
    }
}
