using System.Drawing;
using System.Windows.Forms;

namespace Blumind.Controls
{
    class ImageListBox : CellListBox<Image>
    {
        protected override void DrawCell(int index, Rectangle rect, PaintEventArgs e)
        {
            Image image = Items[index];
            if (image == null)
                return;

            PaintHelper.DrawImageInRange(e.Graphics, image, rect);
        }
    }
}
