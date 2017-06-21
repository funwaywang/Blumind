using System.Drawing;
using System.Windows.Forms;

namespace Blumind.Controls
{
    class FullScreenInfo
    {
        public Control Parent;
        public Rectangle Bounds;
        public Form MdiParent;
        public bool TopLevel;
        public FormBorderStyle BorderStyle = FormBorderStyle.Sizable;
        public bool TopMost;
        public FormWindowState WindowState = FormWindowState.Normal;
        public bool ShowInTaskbar;
        public Form Owner;

        public FullScreenInfo(Control parent, Rectangle bounds)
        {
            this.Parent = parent;
            this.Bounds = bounds;
        }
    }
}
