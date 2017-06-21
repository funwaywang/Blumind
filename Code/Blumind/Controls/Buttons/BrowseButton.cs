using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Blumind.Controls
{
    class BrowseButton : FlatButton
    {
        public BrowseButton()
        {
            Width = 32;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (!DesignMode)
            {
                Image = Properties.Resources.folder_sys;
            }
        }
    }
}
