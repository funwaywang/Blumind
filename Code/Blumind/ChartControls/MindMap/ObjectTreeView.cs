using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Blumind.Core;
using Blumind.Model.Documents;
using Blumind.Model;
using Blumind.Model.MindMaps;

namespace Blumind.Controls.MapViews
{
    class ObjectTreeView : MultiSelectTreeView, IIconProvider
    {
        ContextMenuStrip _TreeViewContextMenuStrip;

        public ObjectTreeView()
        {
            ImageList il = new ImageList();
            il.ColorDepth = ColorDepth.Depth32Bit;
            il.Images.Add(Properties.Resources.document);
            il.Images.Add(Properties.Resources.topic);

            ImageList = il;
            Dock = System.Windows.Forms.DockStyle.Fill;
            HideSelection = false;
            MultiSelect = true;
            ShowRootLines = false;
            BorderStyle = BorderStyle.None;

            //Text = LanguageID = "Objects";
        }

        #region Properties
        private ChartPage _ChartPage;
        private object[] _SelectedObjects;

        //public event System.EventHandler SelectedObjectsChanged;

        [DefaultValue(null), Browsable(false)]
        public ChartPage ChartPage
        {
            get { return _ChartPage; }
            set
            {
                if (_ChartPage != value)
                {
                    ChartPage old = _ChartPage;
                    _ChartPage = value;
                    OnChartPageChanged(old);
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

        private void OnChartPageChanged(ChartPage old)
        {
            if (old != null)
            {
                old.ChartObjectAdded -= new ChartObjectEventHandler(Map_ChartObjectAdded);
            }

            BuildTree();

            if (ChartPage != null)
            {
                ChartPage.ChartObjectAdded += new ChartObjectEventHandler(Map_ChartObjectAdded);
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

                SelectNode(nodes.ToArray(), true);
            }

            //if (SelectedObjectsChanged != null)
            //{
            //    SelectedObjectsChanged(this, EventArgs.Empty);
            //}
        }
        #endregion

        void BuildTree()
        {
            Nodes.Clear();

            if (ChartPage != null)
            {
                DocumentTreeNode nodeDoc = new DocumentTreeNode(ChartPage);
                nodeDoc.ImageIndex = nodeDoc.SelectedImageIndex = 0;
                Nodes.Add(nodeDoc);

                //========
                if (ChartPage is MindMap)
                {
                    var mindMap = (MindMap)ChartPage;
                    if (mindMap.Root != null)
                    {
                        TreeNode root = BuildTree(mindMap.Root, nodeDoc.Nodes);

                        root.Expand();
                    }
                }

                nodeDoc.Expand();
            }
        }

        TreeNode BuildTree(Topic topic, TreeNodeCollection nodes)
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

        void SelectNode(object obj)
        {
            SelectedNode = FindNode(obj);
        }

        TreeNode FindNode(object obj)
        {
            return FindNode(Nodes, obj);
        }

        TreeNode FindNode(TreeNodeCollection nodes, object obj)
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

        //protected override void OnCurrentLanguageChanged(object sender, EventArgs e)
        //{
        //    LabCaption.Text = CurrentLanguage["Objects"];
        //}

        void Map_ChartObjectAdded(object sender, ChartObjectEventArgs e)
        {
            //if (e.Topic != null && e.Topic.Parent != null)
            //{
            //    TopicTreeNode node = FindNode(e.Topic.Parent) as TopicTreeNode;
            //    if (node != null)
            //    {
            //        BuildTree(e.Topic, node.Nodes);
            //    }
            //}
            if (e.Object != null && e.Object.Parent != null)
            {
                TopicTreeNode node = FindNode(e.Object.Parent) as TopicTreeNode;
                if (node != null)
                {
                    //BuildTree(e.Object, node.Nodes);
                }
            }
        }

        protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
        {
            if (e.Node.Parent == null && Nodes.Count <= 1)
                e.Cancel = true;

            base.OnBeforeCollapse(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && TreeViewContextMenuStrip != null)
            {
                TreeNode node = GetNodeAt(e.X, e.Y);
                if (node is TopicTreeNode)
                {
                    TreeViewContextMenuStrip.Show(this, e.X, e.Y);
                }
            }

            base.OnMouseUp(e);
        }

        #region IIconProvider 成员
        Image _Icon;

        public Image Icon
        {
            get { return _Icon; }
            set { _Icon = value; }
        }

        #endregion
    }
}
