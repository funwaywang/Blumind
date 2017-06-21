using System;
using System.Drawing;
using System.Windows.Forms;

namespace Blumind.Controls
{
    class HtmlCodeEditor : TextBox
    {
        public HtmlCodeEditor()
        {
            Multiline = true;
            ScrollBars = System.Windows.Forms.ScrollBars.Both;
            //WordWrap = false;
            BorderStyle = System.Windows.Forms.BorderStyle.None;
            Font = new Font(FontFamily.GenericMonospace, SystemFonts.MessageBoxFont.Size);

            BackColor = Color.WhiteSmoke;
            ForeColor = Color.Black;
        }
    }
}
