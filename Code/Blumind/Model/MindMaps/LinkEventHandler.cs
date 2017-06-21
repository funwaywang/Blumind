using System;

namespace Blumind.Model.MindMaps
{
    delegate void LinkEventHandler(object sender, LinkEventArgs e);

    class LinkEventArgs : EventArgs
    {
        private Link _Link;

        public LinkEventArgs(Link link)
        {
            Link = link;
        }

        public Link Link
        {
            get { return _Link; }
            private set { _Link = value; }
        }
    }
}
