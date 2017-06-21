using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Blumind.Core;

namespace Blumind.Controls
{
    class MultiSelectTreeView : TreeView
    {
        private bool _MultiSelect;
        private XList<TreeNode> _SelectedNodes;
        private bool SuspendBeforeSelect = false;
        private TreeNode _LastSelectedNode;

        public MultiSelectTreeView()
        {
            SelectedNodes = new XList<TreeNode>();
        }

        [DefaultValue(false)]
        public bool MultiSelect
        {
            get { return _MultiSelect; }
            set { _MultiSelect = value; }
        }

        [Browsable(false)]
        public XList<TreeNode> SelectedNodes
        {
            get { return _SelectedNodes; }
            private set 
            {
                if (_SelectedNodes != value)
                {
                    _SelectedNodes = value;
                    SelectedNodes.ItemAdded += new XListEventHandler<TreeNode>(SelectedNodes_ItemAdded);
                    SelectedNodes.ItemRemoved += new XListEventHandler<TreeNode>(SelectedNodes_ItemRemoved);
                    SelectedNodes.BeforeClear += new EventHandler(SelectedNodes_BeforeClear);
                }
            }
        }

        [Browsable(false), DefaultValue(null)]
        public TreeNode LastSelectedNode
        {
            get { return _LastSelectedNode; }
            private set { _LastSelectedNode = value; }
        }

        public bool IsNodeSelected(TreeNode node)
        {
            return SelectedNodes.Contains(node);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (MultiSelect)
            {
                TreeNode node = GetNodeAt(e.X, e.Y);
                if (node != null && InNodeTextRect(node, e.X, e.Y))
                {
                    if (Helper.TestModifierKeys(Keys.Control))
                    {
                        SelectNode(node, !IsNodeSelected(node), TreeViewAction.ByMouse);
                    }
                    else if (Helper.TestModifierKeys(Keys.Shift))
                    {
                        SelectTo(node, TreeViewAction.ByMouse);
                    }
                    else
                    {
                        for (int i = SelectedNodes.Count - 1; i >= 0; i--)
                        {
                            if (node != SelectedNodes[i])
                            {
                                SelectNode(SelectedNodes[i], false, TreeViewAction.ByMouse);
                            }
                        }
                        SelectNode(node, true, TreeViewAction.ByMouse);
                    }
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            if (MultiSelect)
            {
                if (!SuspendBeforeSelect)
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnBeforeSelect(e);
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);

            if (MultiSelect)
            {
                if (e.Node != null && IsNodeSelected(e.Node))
                    LastSelectedNode = e.Node;
                else if (SelectedNodes.Count > 0)
                    LastSelectedNode = SelectedNodes[SelectedNodes.Count - 1];
                else
                    LastSelectedNode = null;
            }
        }

        private bool InNodeTextRect(TreeNode node, int x, int y)
        {
            if (node != null)
            {
                return node.Bounds.Contains(x, y);
                //Rectangle rect = node.Bounds;
                //int level = GetNodeLevel(node);
                //if (ShowRootLines)
                //    level++;
                //if (x >= rect.X + (level * Indent) && x < rect.Right && y >= rect.Y && y <= rect.Bottom)
                //{
                //    return true;
                //}
            }

            return false;
        }

        private void SelectedNodes_BeforeClear(object sender, EventArgs e)
        {
            foreach (TreeNode node in SelectedNodes)
            {
                SetNodeStyle(node, false);
            }
        }

        private void SelectedNodes_ItemRemoved(object sender, XListEventArgs<TreeNode> e)
        {
            SetNodeStyle(e.Item, false);
        }

        private void SelectedNodes_ItemAdded(object sender, XListEventArgs<TreeNode> e)
        {
            SetNodeStyle(e.Item, true);
        }

        public bool SelectNode(TreeNode node, bool select, TreeViewAction action)
        {
            if (node == null || IsNodeSelected(node) == select)
                return false;

            TreeViewCancelEventArgs ce = new TreeViewCancelEventArgs(node, false, action);
            SuspendBeforeSelect = true;
            OnBeforeSelect(ce);
            SuspendBeforeSelect = false;
            if (ce.Cancel)
                return false;

            if (select)
            {
                node.EnsureVisible();
                SelectedNodes.Add(node);
            }
            else
            {
                SelectedNodes.Remove(node);
            }

            OnAfterSelect(new TreeViewEventArgs(node, action));
            return true;
        }

        public void SelectNode(TreeNode[] nodes, TreeViewAction action, bool exclusive)
        {
            if (exclusive)
                ClearSelectedNodes(action);

            foreach (TreeNode node in nodes)
            {
                SelectNode(node, true, action);
            }
        }

        public void SelectNode(TreeNode[] nodes, bool exclusive)
        {
            SelectNode(nodes, TreeViewAction.Unknown, exclusive);
        }

        private void ClearSelectedNodes(TreeViewAction action)
        {
            for (int i = SelectedNodes.Count - 1; i >= 0; i--)
            {
                SelectNode(SelectedNodes[i], false, action);
            }
        }

        private void SelectTo(TreeNode node, TreeViewAction action)
        {
            if (LastSelectedNode == null)
            {
                SelectNode(node, true, action);
            }
            else if (LastSelectedNode == node)
            {
                SelectNode(node, false, action);
            }
            else
            {
                TreeNode nodeBegin = LastSelectedNode;
                TreeNode tn = nodeBegin;
                while ((tn != null) && (tn != node))
                {
                    if (nodeBegin.Bounds.Y > node.Bounds.Y)
                        tn = tn.PrevVisibleNode;
                    else
                        tn = tn.NextVisibleNode;

                    if (tn != null)
                    {
                        SelectNode(tn, true, action);
                    }
                }
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            RefreshSelectedNodesStyle();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            RefreshSelectedNodesStyle();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (LastSelectedNode != null && LastSelectedNode.PrevVisibleNode != null)
                    {
                        SelectNode(new TreeNode[]{LastSelectedNode.PrevVisibleNode}, TreeViewAction.ByKeyboard, !e.Control);
                        e.SuppressKeyPress = true;
                    }
                    break;
                case Keys.Down:
                    if (LastSelectedNode != null && LastSelectedNode.NextVisibleNode != null)
                    {
                        SelectNode(new TreeNode[] { LastSelectedNode.NextVisibleNode }, TreeViewAction.ByKeyboard, !e.Control);
                        e.SuppressKeyPress = true;
                    }
                    break;
            }

            base.OnKeyDown(e);
        }

        private void RefreshSelectedNodesStyle()
        {
            foreach (TreeNode node in SelectedNodes)
            {
                SetNodeStyle(node, true);
            }
        }

        private void SetNodeStyle(TreeNode item, bool selected)
        {
            if (item != null)
            {
                if (selected)
                {
                    if (Focused)
                    {
                        item.BackColor = SystemColors.Highlight;
                        item.ForeColor = SystemColors.HighlightText;
                    }
                    else
                    {
                        item.BackColor = SystemColors.Control;
                        item.ForeColor = SystemColors.ControlText;
                    }
                }
                else
                {
                    item.BackColor = BackColor;
                    item.ForeColor = ForeColor;
                }
            }
        }

        public int GetNodeLevel(TreeNode node)
        {
            int level = 0;
            while (node.Parent != null)
            {
                level++;
                node = node.Parent;
            }

            return level;
        }
    }
}
