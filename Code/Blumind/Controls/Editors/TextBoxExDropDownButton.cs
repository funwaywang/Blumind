using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Blumind.Controls
{
    class TextBoxExDropDownButton : ResizableTextBox.TextBoxExItem
    {
        public TextBoxExDropDownButton()
            : base(null, ResizableTextBox.ItemAlignment.Right)
        {
        }

        public override void OnPaint(PaintEventArgs e, Color backColor, Color foreColor, Font font, bool hover, bool pressed)
        {
            if (VisualStyleInformation.IsEnabledByUser)
            {
                VisualStyleElement element = VisualStyleElement.ComboBox.DropDownButton.Normal;
                if (hover)
                    element = VisualStyleElement.ComboBox.DropDownButton.Hot;
                else if (pressed)
                    element = VisualStyleElement.ComboBox.DropDownButton.Pressed;


                VisualStyleRenderer renderer = new VisualStyleRenderer(element);
                renderer.DrawBackground(e.Graphics, Bounds);
            }
            else
            {
                base.OnPaint(e, backColor, foreColor, font, hover, pressed);
            }
        }
    }
}
