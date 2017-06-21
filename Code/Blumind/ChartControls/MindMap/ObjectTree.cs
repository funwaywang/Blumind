using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Blumind.Core;
using Blumind.Globalization;
using Blumind.Model;
using Blumind.Model.MindMaps;

namespace Blumind.Controls.MapViews
{
    class ObjectTree : BorderPanel// MultiSelectTreeView
    {
        private CaptionBar LabCaption;
        private MultiSelectTreeView InnerTreeView;
        private ContextMenuStrip _TreeViewContextMenuStrip;

        public event TreeViewEventHandler AfterSelect;

        public ObjectTree()
        {
            ImageList il = new ImageList();
            il.ColorDepth = ColorDepth.Depth32Bit;
            il.Images.Add(Properties.Resources.document);
            il.Images.Add(Properties.Resources.topic);

            InnerTreeView = new MultiSelectTreeView();
            InnerTreeView.ImageList = il;
            InnerTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            InnerTreeView.HideSelection = false;
            InnerTreeView.MultiSelect = true;
            InnerTreeView.ShowRootLines = false;
            InnerTreeView.BorderStyle = BorderStyle.None;
            InnerTreeView.BackColor = ContentBackColor;
            InnerTreeView.MouseUp += new MouseEventHandler(InnerTreeView_MouseUp);
            InnerTreeView.AfterSelect += new TreeViewEventHandler(InnerTreeView_AfterSelect);
            InnerTreeView.BeforeCollapse += new TreeViewCancelEventHandler(InnerTreeView_BeforeCollapse);

            LabCaption = new CaptionBar();
            LabCaption.Dock = DockStyle.Top;
            LabCaption.Icon = Properties.Resources.objects;
            LabCaption.Size = new System.Drawing.Size(208, 16);
            LabCaption.Text = Lang._("Objects");
            LabCaption.BackgroundStyle = CaptionStyle.BaseLine;
            LabCaption.BaseLineColor = Color.Silver;
            LabCaption.BackColor = ContentBackColor;

            Controls.Add(InnerTreeView);
            Controls.Add(LabCaption);
            //ImageList = il;
        }

        #region Properties
        private MindMap _Map;
        private object[] _SelectedObjects;

        //public event System.EventHandler SelectedObjectsChanged;

        [DefaultValue(null), Browsable(false)]
        public MindMap Map
        {
            get { return _Map; }
            set
            {
                if (_Map != value)
                {
                    MindMap old = _Map;
                    _Map = value;
                    OnMapChanged(old);
                }
            }
        }

        [DefaultValue(null), Browsable(false)]
        public object[] SelectedObjects
        {
            get { return _SelectedObjects; }
            set 
            {
                if(!Helper.Equals(_SelectedObjects, value))
                {
                    _SelectedObjects = value;
                    OnSelectedObjectsChanged();
                }
            }
        }

        [DefaultValue(null)]
        public ContextMenuStrip TreeViewContextMenuStrip
        {
            get { return _TreeViewContextMenuStrip; }
            set { _TreeViewContextMenuStrip = value; }
        }

        private void OnMapChanged(MindMap old)
        {
            if (old != null)
            {
                old.TopicAdded -= new TopicEventHandler(Map_TopicAdded);
            }

            BuildTree();

            if (Map != null)
            {
                Map.TopicAdded += new TopicEventHandler(Map_TopicAdded);
            }
        }

        private void OnSelectedObjectsChanged()
        {
            List<TreeNode> nodes = new List<TreeNode>();
            if (SelectedObjects != null && SelectedObjects.Length > 0)
            {
                foreach (object obj in SelectedObjects)
                {
                    TreeNode node = FindNode(obj);
                    if (node != null && !nodes.Contains(node))
                    {
                        nodes.Add(node);
                    }
                }

                InnerTreeView.SelectNode(nodes.ToArray(), true);
            }

            //if (SelectedObjectsChanged != null)
            //{
            //    SelectedObjectsChanged(this, EventArgs.Empty);
            //}
        }
        #endregion

        private void BuildTree()
        {
            InnerTreeView.Nodes.Clear();

            if (Map != null)
            {
                DocumentTreeNode nodeDoc = new DocumentTreeNode(Map);
                nodeDoc.ImageIndex = nodeDoc.SelectedImageIndex = 0;
                InnerTreeView.Nodes.Add(nodeDoc);

                if (Map.Root != null)
                {
                    TreeNode root = BuildTree(Map.Root, nodeDoc.Nodes);

                    root.Expand();
                }

                nodeDoc.Expand();
            }
        }

        private TreeNode BuildTree(Topic topic, TreeNodeCollection nodes)
        {
            TopicTreeNode node = new TopicTreeNode(topic);
            node.ImageIndex = node.SelectedImageIndex = 1;
            nodes.Add(node);

            foreach (Topic subTopic in topic.Children)
            {
                BuildTree(subTopic, node.Nodes);
            }

            if (!topic.Folded)
                node.Expand();
            return node;
        }

        private void SelectNode(object obj)
        {
            InnerTreeView.SelectedNode = FindNode(obj);
        }

        private TreeNode FindNode(object obj)
        {
            return FindNode(InnerTreeView.Nodes, obj);
        }

        private TreeNode FindNode(TreeNodeCollection nodes, object obj)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag == obj)
                    return node;

                if (node.Nodes.Count > 0)
                {
                    TreeNode tn = FindNode(node.Nodes, obj);
                    if (tn != null)
                        return tn;
                }
            }

            return null;
        }

        internal void OnNodeAdded(TreeNode node)
        {
            if (node != null)
            {
                if (node is DocumentTreeNode)
                    node.ImageIndex = node.SelectedImageIndex = 0;
                else if (node is TopicTreeNode)
                    node.ImageIndex = node.SelectedImageIndex = 1;
                else
                    node.ImageIndex = node.SelectedImageIndex = -1;
            }
        }

        protected override void OnCurrentLanguageChanged(object sender, EventArgs e)
        {
            LabCaption.Text = Lang._("Objects");
        }

        private void Map_TopicAdded(object sender, TopicEventArgs e)
        {
            if (e.Topic != null && e.Topic.ParentTopic != null)
            {
                var node = FindNode(e.Topic.ParentTopic) as TopicTreeNode;
                if (node != null)
                {
                    BuildTree(e.Topic, node.Nodes);//
                    //TopicTreeNode subNode = new TopicTreeNode(e.Topic);
                    //subNode.ImageIndex = subNode.SelectedImageIndex = 1;
                    //node.Nodes.Add(subNode);
                }
            }
        }

        public bool IsNodeSelected(TreeNode node)
        {
            return InnerTreeView.IsNodeSelected(node);
        }

        private void InnerTreeView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Parent == null && InnerTreeView.Nodes.Count <= 1)
                e.Cancel = true;
        }

        private void InnerTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (AfterSelect != null)
            {
                AfterSelect(sender, e);
            }
        }

        private void InnerTreeView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && TreeViewContextMenuStrip != null)
            {
                TreeNode node = InnerTreeView.GetNodeAt(e.X, e.Y);
                if (node is TopicTreeNode)
                {
                    TreeViewContextMenuStrip.Show(InnerTreeView, e.X, e.Y);
                }
            }
        }
    }
}
