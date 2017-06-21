using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Controls
{
    class PropertyControl : Control, IThemableUI
    {
        PropertyGrid propertyGrid1;
        ContextMenuStrip MenuProperty;
        ToolStripMenuItem MenuReset;
        ToolStripMenuItem MenuDescription;
        Hashtable History = new Hashtable();
        bool _ShowBorder;

        public PropertyControl()
        {
            InitializeComponent();
            ToolbarVisible = false;
            HelpVisible = false;
            //ImeMode = ImeMode.On;

            UITheme.Default.Listeners.Add(this);

            LanguageManage.CurrentChanged += new EventHandler(LanguageManage_CurrentChanged);
        }

        [DefaultValue(true)]
        public bool ShowBorder
        {
            get { return _ShowBorder; }
            set
            {
                if (_ShowBorder != value)
                {
                    _ShowBorder = value;
                    OnShowBorderChanged();
                }
            }
        }

        [DefaultValue(false)]
        public bool ToolbarVisible
        {
            get { return propertyGrid1.ToolbarVisible; }
            set { propertyGrid1.ToolbarVisible = value; }
        }

        [DefaultValue(false)]
        public bool HelpVisible
        {
            get { return propertyGrid1.HelpVisible; }
            set { propertyGrid1.HelpVisible = value; }
        }

        [Browsable(false)]
        public object SelectedObject
        {
            get { return propertyGrid1.SelectedObject; }

            set
            {
                if (propertyGrid1.SelectedObject != value)
                {
                    BeforeSelectedObjectChange();
                    propertyGrid1.SelectedObject = value;
                    AfterSelectedObjectChanged();
                }
            }
        }

        [Browsable(false)]
        public object[] SelectedObjects
        {
            get { return propertyGrid1.SelectedObjects; }
            set { propertyGrid1.SelectedObjects = value; }
        }

        [Browsable(false)]
        public PropertySort PropertySort
        {
            get { return propertyGrid1.PropertySort; }
            set { propertyGrid1.PropertySort = value; }
        }

        void InitializeComponent()
        {
            propertyGrid1 = new PropertyGrid();
            MenuReset = new ToolStripMenuItem();
            MenuDescription = new ToolStripMenuItem();
            MenuProperty = new ContextMenuStrip();
            SuspendLayout();

            //
            propertyGrid1 = new PropertyGrid();

            //
            MenuReset.Text = LanguageManage.GetText("Reset");
            MenuReset.Image = Properties.Resources.reset;
            MenuReset.Click += new EventHandler(MenuReset_Click);

            //
            MenuDescription.Text = LanguageManage.GetText("Show Description");
            MenuDescription.Click += new EventHandler(MenuDescription_Click);

            //
            MenuProperty.Items.AddRange(new ToolStripItem[] { 
                MenuReset, new ToolStripSeparator(), MenuDescription});
            MenuProperty.Opening += new System.ComponentModel.CancelEventHandler(MenuProperty_Opening);

            //
            Controls.Add(propertyGrid1);
            ContextMenuStrip = MenuProperty;
            ResumeLayout();
        }

        void OnShowBorderChanged()
        {
            //foreach (Control ctrl in Controls)
            //{
            //    if (ctrl.GetType().Name == "PropertyGridView")  //System.Windows.Forms.PropertyGridInternal.PropertyGridView
            //    {
            //        if (HideBorder)
            //        {
            //            ctrl.Paint += new PaintEventHandler(PropertyGridView_Paint);
            //            ctrl.BackColor = System.Drawing.Color.Blue;
            //        }
            //        else
            //        {
            //            ctrl.Paint -= new PaintEventHandler(PropertyGridView_Paint);
            //        }
            //    }
            //}
            PerformLayout();
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (propertyGrid1 != null)
            {
                var rect = ClientRectangle;
                if (!ShowBorder)
                    rect.Inflate(1, 1);
                propertyGrid1.Bounds = rect;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.DrawRectangle(new System.Drawing.Pen(BackColor), 0, 0, Width - 1, Height - 1);
        }

        void PropertyGridView_Paint(object sender, PaintEventArgs e)
        {
            if (sender is Control)
            {
                Control control = (Control)sender;
                e.Graphics.DrawRectangle(new System.Drawing.Pen(BackColor), 0, 0, control.Width - 1, control.Height - 1);
            }
        }

        void MenuProperty_Opening(object sender, CancelEventArgs e)
        {
            MenuDescription.Checked = HelpVisible;
            MenuReset.Enabled = false;

            try
            {
                if (propertyGrid1.SelectedGridItem != null && propertyGrid1.SelectedGridItem.PropertyDescriptor != null)
                {
                    if (propertyGrid1.SelectedGridItem.Parent != null && propertyGrid1.SelectedGridItem.Parent.Value != null)
                    {
                        MenuReset.Enabled = propertyGrid1.SelectedGridItem.PropertyDescriptor.CanResetValue(propertyGrid1.SelectedGridItem.Parent.Value);
                    }
                    else if (SelectedObjects.Length == 1)
                    {
                        MenuReset.Enabled = propertyGrid1.SelectedGridItem.PropertyDescriptor.CanResetValue(SelectedObjects[0]);
                    }
                    else if (SelectedObjects.Length > 1)
                    {
                        MenuReset.Enabled = propertyGrid1.SelectedGridItem.PropertyDescriptor.CanResetValue(SelectedObjects);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Helper.WriteLog(ex.Message);
            }
        }

        void MenuReset_Click(object sender, EventArgs e)
        {
            try
            {
                propertyGrid1.ResetSelectedProperty();
            }
            catch (System.Exception ex)
            {
                Helper.WriteLog(ex);
                this.ShowMessage(ex.Message, MessageBoxIcon.Error);
            }
        }

        void MenuDescription_Click(object sender, EventArgs e)
        {
            HelpVisible = !HelpVisible;
        }

        void AfterSelectedObjectChanged()
        {
            if (SelectedObject != null)
            {
                if(History.Contains(SelectedObject.GetType()))
                {
                    SelectProperty((string)History[SelectedObject.GetType()]);
                }
            }
        }

        void BeforeSelectedObjectChange()
        {
            if (propertyGrid1.SelectedGridItem != null)
            {
                History[SelectedObject.GetType()] = GetItemPath(propertyGrid1.SelectedGridItem);
            }
        }

        void SelectProperty(string propertyPath)
        {
            if (SelectedObject == null || string.IsNullOrEmpty(propertyPath) || propertyGrid1.SelectedGridItem == null)
                return;

            GridItem root = propertyGrid1.SelectedGridItem;
            while (root.Parent != null)
                root = root.Parent;

            GridItem item = FindProperty(root, propertyPath, string.Empty);
            if (item != null && item != propertyGrid1.SelectedGridItem)
            {
                //SelectedGridItem = item;
                EnsureItemVisible(item);
                item.Select();
            }
        }

        GridItem FindProperty(GridItem parent, string propertyPath, string path)
        {
            foreach (GridItem gi in parent.GridItems)
            {
                string mypath = gi.Label;
                if(path != string.Empty)
                    mypath = path + "."+gi.Label;

                if (mypath == propertyPath)
                    return gi;

                GridItem sgi = FindProperty(gi, propertyPath, mypath);
                if (sgi != null)
                    return sgi;
            }

            return null;
        }

        string GetItemPath(GridItem gi)
        {
            string path = gi.Label;
            while(gi.Parent != null && gi.Parent.Parent != null)
            {
                gi = gi.Parent;
                path = gi.Label + "." + path;
            }

            return path;
        }

        void EnsureItemVisible(GridItem gi)
        {
            while (gi.Parent != null)
            {
                gi = gi.Parent;
                if (gi.Expandable && !gi.Expanded)
                    gi.Expanded = true;
            }
        }

        void LanguageManage_CurrentChanged(object sender, EventArgs e)
        {
            MenuDescription.Text = LanguageManage.GetText("Show Description");
            MenuReset.Text = LanguageManage.GetText("Reset");
        }

        public void ApplyTheme(UITheme theme)
        {
            if (theme != null)
            {
                MenuProperty.Renderer = theme.ToolStripRenderer;
                //propertyGrid1.ToolStripRenderer = theme.ToolStripRenderer;
            }
        }
    }
}
