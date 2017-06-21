using System.ComponentModel;

namespace Blumind.Model.MindMaps
{
    delegate void TopicCancelEventHandler(object sender, TopicCancelEventArgs e);

    class TopicCancelEventArgs : CancelEventArgs
    {
        private Topic _Topic;

        public TopicCancelEventArgs(Topic topic)
        {
            Topic = topic;
        }

        public Topic Topic
        {
            get { return _Topic; }
            private set { _Topic = value; }
        }
    }
}
