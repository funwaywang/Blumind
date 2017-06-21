using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Blumind.Controls
{
    class ButtonInfo
    {
        public event EventHandler Click;

        public ButtonInfo()
        {
        }

        public ButtonInfo(string text, Image image)
        {
            Text = text;
            Image = image;
        }

        public string Text { get; set; }

        public Image Image { get; set; }

        public Rectangle Bounds { get; set; }

        internal void NotifyClick()
        {
            if (Click != null)
                Click(this, EventArgs.Empty);
        }
    }
}
