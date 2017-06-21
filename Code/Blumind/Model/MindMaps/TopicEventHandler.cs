using System;
using Blumind.Core;
using Blumind.Model;

namespace Blumind.Model.MindMaps
{
    delegate void TopicEventHandler(object sender, TopicEventArgs e);

    class TopicEventArgs : EventArgs
    {
        Topic _Topic;
        ChangeTypes _Changes;

        public TopicEventArgs(Topic topic)
        {
            Topic = topic;
        }

        public TopicEventArgs(Topic topic, ChangeTypes changes)
        {
            Topic = topic;
            Changes = changes;
        }

        public Topic Topic
        {
            get { return _Topic; }
            private set { _Topic = value; }
        }

        public ChangeTypes Changes
        {
            get { return _Changes; }
            private set { _Changes = value; }
        }
    }
}
