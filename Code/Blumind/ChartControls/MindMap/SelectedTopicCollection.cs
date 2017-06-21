using System;
using System.Collections;
using Blumind.Core;
using Blumind.Model;
using Blumind.Model.MindMaps;

namespace Blumind.Controls.MapViews
{
    class SelectedTopicCollection : CollectionBase
    {
        public SelectedTopicCollection()
        {
        }

        public SelectedTopicCollection(Topic[] topics)
        {
            AddRange(topics);
        }

        public Topic this[int index]
        {
            get
            {
                if (index < 0 || index >= List.Count)
                    throw new ArgumentOutOfRangeException();
                else
                    return (Topic)List[index];
            }
        }

        public bool Contains(Topic topic)
        {
            return List.Contains(topic);
        }

        internal void Add(Topic topic)
        {
            if (!Contains(topic))
            {
                List.Add(topic);
            }
        }

        internal void AddRange(Topic[] topics)
        {
            for (int i = 0; i < topics.Length; i++)
            {
                List.Add(topics[i]);
            }
        }

        public Topic[] ToArray()
        {
            Topic[] array = new Topic[Count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this[i];
            }

            return array;
        }

        public void Remove(Topic topic)
        {
            if (List.Contains(topic))
            {
                List.Remove(topic);
            }
        }

        public int IndexOf(Topic topic)
        {
            return List.IndexOf(topic);
        }
    }
}
