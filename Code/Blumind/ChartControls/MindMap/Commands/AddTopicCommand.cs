using System;
using Blumind.Core;
using Blumind.Model;
using Blumind.Model.Documents;
using Blumind.Model.MindMaps;

namespace Blumind.Controls.MapViews
{
    class AddTopicCommand : Command
    {
        Topic ParentTopic;
        Topic[] SubTopics;
        int Index;

        public AddTopicCommand(Topic parentTopic, Topic subTopic, int index)
        {
            if (parentTopic == null || subTopic == null)
            {
                throw new ArgumentNullException();
            }

            ParentTopic = parentTopic;
            SubTopics = new Topic[] { subTopic };
            Index = index;
        }

        public AddTopicCommand(Topic parentTopic, Topic[] subTopics, int index)
        {
            if (parentTopic == null || subTopics.IsNullOrEmpty())
            {
                throw new ArgumentNullException();
            }

            ParentTopic = parentTopic;
            SubTopics = subTopics;
            Index = index;
        }

        public override string Name
        {
            get { return "Add"; }
        }

        public override bool Rollback()
        {
            foreach (var st in SubTopics)
            {
                if (ParentTopic.Children.Contains(st))
                {
                    ParentTopic.Children.Remove(st);
                }
            }

            return true;
        }

        public override bool Execute()
        {
            foreach (var st in SubTopics)
            {
                if (ParentTopic == st || st.IsDescent(ParentTopic))
                    return false;
            }

            if (Index >= 0 && Index < ParentTopic.Children.Count)
            {
                var index = Index;
                foreach (var st in SubTopics)
                {
                    ParentTopic.Children.Insert(index, st);
                    index++;
                }
            }
            else
            {
                foreach (var st in SubTopics)
                {
                    ParentTopic.Children.Add(st);
                }
            }

            return true;
        }
    }
}
