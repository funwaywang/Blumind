using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Blumind.Model
{
    interface IExtendActionProvider
    {
        IEnumerable<ExtendActionInfo> GetExtendActions(bool readOnly);
    }

    class ExtendActionInfo
    {
        public ExtendActionInfo()
        {
        }

        public ExtendActionInfo(string text, Image icon, EventHandler processor)
        {
            Text = text;
            Icon = icon;
            Processor = processor;
        }

        public ExtendActionInfo(string text, Image icon, string shortcutKey, EventHandler processor)
            : this(text, icon, processor)
        {
            ShortcutKeyDisplayString = shortcutKey;
        }

        public Image Icon { get; set; }

        public string Text { get; set; }

        public string ShortcutKeyDisplayString { get; set; }

        public EventHandler Processor { get; set; }

        public override string ToString()
        {
            return Text;
        }

        public void Invoke(object sender, EventArgs e)
        {
            if (Processor != null)
            {
                Processor(sender, e);
            }
        }
    }
}
