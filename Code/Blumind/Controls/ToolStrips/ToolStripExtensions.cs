using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Blumind.Controls
{
    static class ToolStripExtensions
    {
        public static void SmartHideSeparators(this ToolStrip toolStrip)
        {
            if (toolStrip == null)
                throw new NullReferenceException();

            SmartHideSeparators(toolStrip.Items.AsAsEnumerable());
        }

        public static void SmartHideSeparators(this IEnumerable<ToolStripItem> items)
        {
            var visibleItems = new List<ToolStripItem>();
            ToolStripSeparator lastSeparator = null;
            foreach (var item in items)
            {
                if (item is ToolStripSeparator)
                {
                    if (visibleItems.Count == 0)
                    {
                        item.Available = false;
                    }
                    else
                    {
                        visibleItems.Clear();
                        lastSeparator = (ToolStripSeparator)item;
                    }
                }
                else
                {
                    if (!item.Available)
                        continue;
                    visibleItems.Add(item);
                    lastSeparator = null;
                }
            }

            if (lastSeparator != null)
            {
                lastSeparator.Available = false;
            }
        }

        public static IEnumerable<ToolStripItem> AsAsEnumerable(this ToolStripItemCollection collection)
        {
            if (collection == null)
                throw new NullReferenceException();

            foreach (ToolStripItem c in collection)
                yield return c;
        }

        public static bool HasAvailableItems(this ToolStripDropDownItem item)
        {
            if (item == null)
                throw new NullReferenceException();

            foreach (ToolStripItem subItem in item.DropDownItems)
            {
                if (subItem.Available)
                    return true;
            }

            return false;
        }
    }
}
