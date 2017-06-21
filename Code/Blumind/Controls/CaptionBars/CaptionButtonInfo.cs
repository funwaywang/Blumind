using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Blumind.Controls
{
    enum CaptionButtonStatus
    {
        Normal,
        Hot,
        Pressed,
    }

    class CaptionButtonClickEventArgs : EventArgs
    {
        public CaptionButtonClickEventArgs(CaptionButtonInfo button)
        {
            Button = button;
        }

        public CaptionButtonInfo Button { get; private set; }
    }

    delegate void CaptionButtonClickEventHandler(object sender, CaptionButtonClickEventArgs e);

    class CaptionButtonInfo
    {
        public event EventHandler Click;

        public CaptionButtonInfo(string text, Image image)
        {
            Text = text;
            Image = image;
            Visible = true;
        }

        internal CaptionButtonInfo(string text, CaptionButton systemButtonID)
        {
            Text = text;
            SystemButtonID = systemButtonID;
            Visible = true;
        }

        public string Name { get; set; }

        public string Text { get; set; }

        public Rectangle Bounds { get; internal set; }

        internal CaptionButton? SystemButtonID { get; set; }

        public Image Image { get; set; }

        public bool Visible { get; set; }

        public object Tag { get; set; }

        internal bool IsSystemButton(CaptionButton id)
        {
            return SystemButtonID.HasValue && SystemButtonID.Value == id;
        }

        internal void NotifyClick()
        {
            if (Click != null)
                Click(this, EventArgs.Empty);
        }

        public CaptionButtonInfo SetName(string name)
        {
            Name = name;
            return this;
        }

        public CaptionButtonInfo SetTag(object tag)
        {
            Tag = tag;
            return this;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
