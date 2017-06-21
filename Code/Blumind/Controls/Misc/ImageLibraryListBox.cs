using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Blumind.Model;

namespace Blumind.Controls
{
    class ImageLibraryListBox : CellListBox<Picture>
    {
        public ImageLibraryListBox()
        {
            RefreshItems();
        }

        protected override void DrawCell(int index, Rectangle rect, PaintEventArgs e)
        {
            Picture picture = Items[index];
            if (picture == null || picture.Data == null)
                return;

            PaintHelper.DrawImageInRange(e.Graphics, picture.Data, rect);
        }

        public void RefreshItems()
        {
            SuspendLayout();

            int old = SelectedIndex;
            Clear();
            foreach (KeyValuePair<string, Picture> obj in MyIconLibrary.Share)
            {
                if (obj.Value.Data != null)
                    Items.Add(obj.Value);
            }

            if (old > -1 && old < Items.Count)
                SelectedIndex = old;

            ResumeLayout();
        }
    }
}
