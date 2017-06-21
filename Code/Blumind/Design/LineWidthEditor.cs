using System.Drawing;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Design
{
    class LineWidthEditor : ListEditor<int>
    {
        protected override int[] GetStandardValues()
        {
            return new int[] { 1, 2, 3, 6, 10 };
        }

        protected override System.Collections.Generic.IEnumerable<ListItem<int>> GetStandardItems()
        {
            return new ListItem<int>[]{
                new ListItem<int>(Lang._("Extra Thin"), 1),
                new ListItem<int>(Lang._("Thin"), 2),
                new ListItem<int>(Lang._("Medium"), 3),
                new ListItem<int>(Lang._("Bold"), 6),
                new ListItem<int>(Lang._("Extra Bold"), 10),
            };
        }
    }
}
