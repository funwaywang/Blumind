using System;
using System.Collections.Generic;
using System.Text;

namespace Blumind.ChartPageView
{
    class NeedShowPropertyEventArgs : EventArgs
    {
        public bool Force { get; private set; }

        public NeedShowPropertyEventArgs(bool force)
        {
            Force = force;
        }
    }

    delegate void NeedShowPropertyEventHandler(object sender, NeedShowPropertyEventArgs e);
}
