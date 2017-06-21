using System;
using System.Collections.Generic;
using System.Text;
using Blumind.Model;
using Blumind.Model.MindMaps;

namespace Blumind.Model.Styles
{
    class FormatPainter
    {
        TopicStyle Clipboard;
        bool _HoldOn;
        public static readonly FormatPainter Default = new FormatPainter();

        public event System.EventHandler DataChanged;

        private FormatPainter()
        {
        }

        public bool HoldOn
        {
            get { return _HoldOn; }
            set { _HoldOn = value; }
        }

        public bool IsEmpty
        {
            get { return Clipboard == null; }
        }

        public void Copy(Topic topic)
        {
            if (topic != null)
            {
                if (Clipboard == null)
                    Clipboard = new TopicStyle();
                Clipboard.Copy(topic.Style);
                OnDataChanged();
            }
        }

        public void Assign(Topic topic)
        {
            if (topic != null && Clipboard != null)
            {
                topic.Style.Copy(Clipboard);
            }
        }

        public void Clear()
        {
            if (Clipboard != null)
            {
                Clipboard = null;
                OnDataChanged();
            }
            HoldOn = false;
        }

        private void OnDataChanged()
        {
            if (DataChanged != null)
            {
                DataChanged(null, EventArgs.Empty);
            }
        }
    }
}
