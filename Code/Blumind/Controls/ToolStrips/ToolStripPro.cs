using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace Blumind.Controls
{
    class ToolStripPro : ToolStrip
    {
        public ToolStripPro()
        {
        }

        [DefaultValue(false)]
        public bool HalfRenderBackground { get; set; }

        protected override void OnFontChanged(System.EventArgs e)
        {
            base.OnFontChanged(e);

            foreach (ToolStripItem item in this.Items)
            {
                item.Font = this.Font;
            }
        }

        #region Items Priority
        Dictionary<ToolStripItem, int> ItemsPriority = new Dictionary<ToolStripItem, int>();

        public void SetItemOverflowPriority(ToolStripItem item, int priority)
        {
            if (item != null)
            {
                ItemsPriority[item] = priority;
            }
        }

        public int GetItemOverflowPriority(ToolStripItem item)
        {
            if (item != null && ItemsPriority.ContainsKey(item))
                return ItemsPriority[item];
            else
                return 0;
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            if (ItemsPriority.Count != 0)
            {
                ResetItemsOverflow();
            }

            base.OnLayout(e);
        }

        void ResetItemsOverflow()
        {
            var rect = ClientRectangle;
            rect = rect.Inflate(Padding);
            rect.Width -= OverflowButton.Width;

            var items = Items.Cast<ToolStripItem>().OrderByDescending(item => GetItemOverflowPriority(item));
            int width = 0;
            foreach (var item in items)
            {
                width += item.Width;

                if (item.Overflow != ToolStripItemOverflow.Always)
                {
                    if (width < rect.Width)
                        item.Overflow = ToolStripItemOverflow.Never;
                    else
                        item.Overflow = ToolStripItemOverflow.AsNeeded;
                }
            }
        }

        #endregion
    }
}
