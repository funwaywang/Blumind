using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Blumind.Controls
{
    class SpecialTabItem : TabItem
    {
        public SpecialTabItem()
        {
            Padding = new Padding(3, Padding.Top, 3, Padding.Bottom);
            Selectable = false;
        }

        public SpecialTabItem(Image icon)
            : this()
        {
            Icon = icon;
        }

        public SpecialTabItem(Image icon, EventHandler clickProc)
            : this()
        {
            Icon = icon;
            Click += new EventHandler(clickProc);
        }
    }
}
