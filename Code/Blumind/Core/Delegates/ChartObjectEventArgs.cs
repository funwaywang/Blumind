using System;
using System.Collections.Generic;
using System.Text;
using Blumind.Model;

namespace Blumind.Core
{
    delegate void ChartObjectEventHandler(object sender, ChartObjectEventArgs e);

    class ChartObjectEventArgs : EventArgs
    {
        public ChartObjectEventArgs(ChartObject chartObject)
        {
            Object = chartObject;
        }

        public ChartObject Object { get; private set; }
    }
}
