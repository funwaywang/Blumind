using System;
using System.ComponentModel;
using System.Windows.Forms;
using Blumind.Core;
using Blumind.Model;
using Blumind.Model.MindMaps;

namespace Blumind.Controls.MapViews
{
    class TopicTreeNode : TreeNode
    {
        private Topic _Topic;

        public TopicTreeNode()
        {
        }

        public TopicTreeNode(Topic topic)
        {
            Topic = topic;
        }

        [DefaultValue(true)]
        public Topic Topic
        {
            get { return _Topic; }
            set
            {
                if (_Topic != value)
                {
                    Topic old = _Topic;
                    _Topic = value;
                    OnTopicChanged(old);
                }
            }
        }

        private void OnTopicChanged(Topic old)
        {
            if (old != null)
            {
                old.TextChanged -= new EventHandler(Topic_TextChanged);
                old.Children.ItemAdded -= new XListEventHandler<Topic>(Topic_Children_ItemAdded);
                old.Children.ItemRemoved -= new XListEventHandler<Topic>(Topic_Children_ItemRemoved);
                old.Children.ItemChanged -= new XListValueEventHandler<Topic>(Topic_Children_ItemChanged);
                old.Children.AfterClear -= new EventHandler(Topic_Children_ItemsClear);
                old.Children.AfterSort -= new EventHandler(Topic_Children_AfterSort);
            }

            if (Topic != null)
            {
                Text = Topic.ToString();
                Tag = Topic;
                Topic.TextChanged += new EventHandler(Topic_TextChanged);
                Topic.Children.ItemAdded += new XListEventHandler<Topic>(Topic_Children_ItemAdded);
                Topic.Children.ItemRemoved += new XListEventHandler<Topic>(Topic_Children_ItemRemoved);
                Topic.Children.ItemChanged += new XListValueEventHandler<Topic>(Topic_Children_ItemChanged);
                Topic.Children.AfterClear += new EventHandler(Topic_Children_ItemsClear);
                Topic.Children.AfterSort += new EventHandler(Topic_Children_AfterSort);
            }
        }

        private void Topic_TextChanged(object sender, EventArgs e)
        {
            if (Topic != null)
            {
                Text = Topic.Text;
            }
        }

        private void Topic_Children_ItemsClear(object sender, EventArgs e)
        {
            Nodes.Clear();
        }

        private void Topic_Children_ItemChanged(object sender, XListValueEventArgs<Topic> e)
        {
            TopicTreeNode node = FindNode(e.OldValue);
            if (node != null)
            {
                node.Topic = e.NewValue;
            }
        }

        private void Topic_Children_ItemRemoved(object sender, XListEventArgs<Topic> e)
        {
            TopicTreeNode node = FindNode(e.Item);
            if (node != null)
            {
                node.Remove();
            }
        }

        private void Topic_Children_ItemAdded(object sender, XListEventArgs<Topic> e)
        {
            if (FindNode(e.Item) != null)
                return;

            TopicTreeNode node = new TopicTreeNode(e.Item);
            Nodes.Add(node);

            //if (TreeView is ObjectTree)
            //{
            //    ((ObjectTree)TreeView).OnNodeAdded(node);
            //}
        }

        private void Topic_Children_AfterSort(object sender, EventArgs e)
        {

        }

        public TopicTreeNode FindNode(Topic item)
        {
            return FindNode(item, false);
        }

        public TopicTreeNode FindNode(Topic item, bool recursive)
        {
            foreach (TreeNode node in Nodes)
            {
                if (node is TopicTreeNode)
                {
                    TopicTreeNode ttn = (TopicTreeNode)node;
                    if (ttn.Topic == item)
                        return ttn;

                    if (recursive && node.Nodes.Count > 0)
                    {
                        TopicTreeNode subTtn = FindNode(item, recursive);
                        if (subTtn != null)
                            return subTtn;
                    }
                }
            }

            return null;
        }
    }
}
