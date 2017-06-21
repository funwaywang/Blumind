using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Blumind.Controls
{
    delegate void TabItemEventHandler(object sender, TabItemEventArgs e);

    delegate void TabItemCancelEventHandler(object sender, TabItemCancelEventArgs e);

    class TabItemEventArgs : EventArgs
    {
        public TabItem Item { get; private set; }

        public TabBar Bar { get; private set; }

        public TabItemEventArgs(TabBar bar, TabItem item)
        {
            Bar = bar;
            Item = item;
        }
    }

    class TabItemCancelEventArgs : CancelEventArgs
    {
        public TabItem Item { get; private set; }

        public TabBar Bar { get; private set; }

        public TabItemCancelEventArgs(TabBar bar, TabItem item)
        {
            Bar = bar;
            Item = item;
        }
    }
}
