using System;
using System.Collections.Generic;
using System.Text;

namespace Blumind.Controls
{
    class TabItemDisplayIndexComparer : IComparer<TabItem>
    {
        #region IComparer<TabItem> 成员

        int IComparer<TabItem>.Compare(TabItem x, TabItem y)
        {
            if (x != null && y != null)
                return x.DisplayIndex.CompareTo(y.DisplayIndex);
            else
                return 0;
        }

        #endregion
    }
}
