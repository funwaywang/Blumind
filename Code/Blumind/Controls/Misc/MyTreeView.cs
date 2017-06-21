using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Blumind.Core;

namespace Blumind.Controls
{
    /*class MyTreeView : MultiSelectTreeView // BaseControl
    {
        private MultiSelectTreeView InternalTreeView;

        public event TreeViewEventHandler AfterSelect;
        public event TreeViewCancelEventHandler BeforeCollapse;

        public MyTreeView()
        {
            SuspendLayout();

            InternalTreeView = new MultiSelectTreeView();
            InternalTreeView.Dock = DockStyle.Fill;
            InternalTreeView.BorderStyle = BorderStyle.None;
            InternalTreeView.AfterSelect += new TreeViewEventHandler(InternalTreeView_AfterSelect);
            InternalTreeView.BeforeCollapse += new TreeViewCancelEventHandler(InternalTreeView_BeforeCollapse);
            Controls.Add(InternalTreeView);

            SetPaintStyles();
            ResumeLayout();
        }

        protected override Padding DefaultPadding
        {
            get
            {
                return new Padding(2);
            }
        }

        #region TreeView Properties
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TreeNodeCollection Nodes
        {
            get { return InternalTreeView.Nodes; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TreeNode SelectedNode 
        {
            get { return InternalTreeView.SelectedNode; }
            set { InternalTreeView.SelectedNode = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public XList<TreeNode> SelectedNodes
        {
            get { return InternalTreeView.SelectedNodes; }
        }

        [DefaultValue(true)]
        public bool HideSelection 
        {
            get { return InternalTreeView.HideSelection; }
            set { InternalTreeView.HideSelection = value; }
        }

        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        public ImageList ImageList 
        {
            get { return InternalTreeView.ImageList; }
            set { InternalTreeView.ImageList = value; }
        }

        [Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(-1)]
        [RelatedImageList("ImageList")]
        [Localizable(true)]
        public int ImageIndex
        {
            get { return InternalTreeView.ImageIndex; }
            set { InternalTreeView.ImageIndex = value; }
        }

        [Localizable(true)]
        [DefaultValue(-1)]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [RelatedImageList("ImageList")]
        public int SelectedImageIndex
        {
            get { return InternalTreeView.SelectedImageIndex; }
            set { InternalTreeView.SelectedImageIndex = value; }
        }

        [DefaultValue(true)]
        public bool ShowRootLines
        {
            get { return InternalTreeView.ShowRootLines; }
            set { InternalTreeView.ShowRootLines = value; }
        }

        [DefaultValue(false)]
        public bool MultiSelect
        {
            get { return InternalTreeView.MultiSelect; }
            set { InternalTreeView.MultiSelect = value; }
        }

        #endregion

        #region TreeView Methods
        protected virtual void OnAfterSelect(TreeViewEventArgs e)
        {
            if (AfterSelect != null)
            {
                AfterSelect(this, e);
            }
        }

        protected virtual void OnBeforeCollapse(TreeViewCancelEventArgs e)
        {
            if (BeforeCollapse != null)
            {
                BeforeCollapse(this, e);
            }
        }

        public TreeNode GetNodeAt(int x, int y)
        {
            return InternalTreeView.GetNodeAt(x, y);
        }

        public TreeNode GetNodeAt(Point pt)
        {
            return InternalTreeView.GetNodeAt(pt);
        }

        public void ExpandAll()
        {
            InternalTreeView.ExpandAll();
        }

        public void CollapseAll()
        {
            InternalTreeView.CollapseAll();
        }

        public void Select(TreeNode[] nodes, bool exclusive)
        {
            //InternalTreeView.
                SelectNode(nodes, TreeViewAction.Unknown, exclusive);
        }

        public bool IsNodeSelected(TreeNode node)
        {
            return InternalTreeView.IsNodeSelected(node);
        }

        #endregion

        private void InternalTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            OnAfterSelect(e);
        }

        private void InternalTreeView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            OnBeforeCollapse(e);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            LayoutControls();
        }

        private void LayoutControls()
        {
            Rectangle rect = ClientRectangle;
            rect.X += Padding.Left;
            rect.Y += Padding.Top;
            rect.Width -= Padding.Horizontal;
            rect.Height -= Padding.Vertical;

            InternalTreeView.Bounds = rect;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            if (InternalTreeView.CanFocus)
                InternalTreeView.Focus();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //e.Graphics.Clear(BackColor);
            GlobalBackground.Draw(e.Graphics);

            PT.SetHighQualityRender(e.Graphics);
            Rectangle rect = ClientRectangle;
            GraphicsPath gp = PT.GetRoundRectangle(rect, Padding.Left * 2);

            e.Graphics.FillPath(SystemBrushes.Window, gp);
            e.Graphics.DrawPath(new Pen(PT.GetDarkColor(BackColor)), gp);
        }
    }*/
}
