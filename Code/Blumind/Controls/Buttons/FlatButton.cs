using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Blumind.Controls
{
    class FlatButton : ImageButton
    {
        protected override void PaintBackground(PaintEventArgs e)
        {
            if (!IsMouseDown && !IsMouseHover && !IsDefault)
                return;

            base.PaintBackground(e);
        }
    }
}
