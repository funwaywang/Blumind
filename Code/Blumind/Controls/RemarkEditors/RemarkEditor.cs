using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Blumind.Configuration;
using Blumind.Dialogs;
using Blumind.Globalization;
using Blumind.Model;

namespace Blumind.Controls
{
    class RemarkEditor : ModifyControl
    {
        const string ExternalRemarkDialogSize = "ExternalRemarkDialogSize";
        HtmlEditor htmlEditor;
        IRemark _CurrentObject;
        ToolStripButton tsbExternal;
        ToolStripButton tsbEdit;
        ToolStripSeparator toolStripSeparator1;
        bool _ExternalMode;
        bool _EditMode;
        Form externalDialog;

        public RemarkEditor()
        {
            InitializeComponents();

            LanguageManage.CurrentChanged += LanguageManage_CurrentChanged;
        }

        [DefaultValue(null)]
        public IRemark CurrentObject
        {
            get { return _CurrentObject; }
            set 
            {
                if (_CurrentObject != value)
                {
                    var old = _CurrentObject;
                    _CurrentObject = value;
                    OnCurrentObjectChanged(old);
                }
            }
        }

        [DefaultValue(false), Browsable(false)]
        public bool ExternalMode
        {
            get { return _ExternalMode; }
            private set 
            {
                if (_ExternalMode != value)
                {
                    _ExternalMode = value;
                    OnExternalModeChanged();
                }
            }
        }

        [DefaultValue(false), Browsable(false)]
        public bool EditMode
        {
            get { return _EditMode; }
            private set
            {
                if (_EditMode != value)
                {
                    _EditMode = value;
                    OnEditModeChanged();
                }
            }
        }

        protected Size ExternalDialogSize { get; private set; }

        void InitializeComponents()
        {
            htmlEditor = new HtmlEditor();
            tsbExternal = new ToolStripButton();
            tsbEdit = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();

            //
            tsbExternal.Text = Lang._("External");
            tsbExternal.Image = Properties.Resources.external;
            tsbExternal.Alignment = ToolStripItemAlignment.Right;
            tsbExternal.Overflow = ToolStripItemOverflow.Never;
            tsbExternal.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbExternal.Click += tsbExternal_Click;

            //
            tsbEdit.Text = Lang._("Edit");
            tsbEdit.Image = Properties.Resources.edit;
            tsbEdit.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;//.Image;
            tsbEdit.Click += tsbEdit_Click;

            //
            htmlEditor.Dock = DockStyle.Fill;
            htmlEditor.ReadOnly = this.ReadOnly || !EditMode;
            htmlEditor.TextChanged += htmlEditor_TextChanged;

            //
            var toolStrip = htmlEditor.ToolStrip;
            toolStrip.RenderMode = ToolStripRenderMode.System;
            toolStrip.MiniMode = !ExternalMode;
            toolStrip.Items.Insert(0, toolStripSeparator1);
            toolStrip.Items.Insert(0, tsbExternal);
            toolStrip.Items.Insert(0, tsbEdit);
            toolStrip.PriorityItems.AddRange(new ToolStripItem[] { tsbExternal, tsbEdit, toolStripSeparator1 });

            //
            ExternalDialogSize = Options.Current.GetValue(ExternalRemarkDialogSize, new Size(600, 360));
            BackColor = SystemColors.Window;
            ForeColor = SystemColors.WindowText;
            Controls.Add(htmlEditor);
        }

        protected virtual void OnExternalModeChanged()
        {
            if (ExternalMode)
            {
                if (htmlEditor != null && Controls.Contains(htmlEditor))
                    Controls.Add(htmlEditor);

                externalDialog = new BaseDialog();
                externalDialog.Text = Lang._("Remark");
                externalDialog.Size = ExternalDialogSize;
                externalDialog.Resize += externalDialog_Resize;
                externalDialog.FormClosed += externalDialog_FormClosed;
                externalDialog.Controls.Add(htmlEditor);
                externalDialog.Show(this);
            }
            else
            {
                if (htmlEditor != null && htmlEditor.Parent != null && htmlEditor.Parent != this)
                    htmlEditor.Parent.Controls.Remove(htmlEditor);

                Controls.Add(htmlEditor);
                htmlEditor.ViewType = HtmlEditorViewType.Design;

                if (externalDialog != null && !externalDialog.IsDisposed)
                    externalDialog.Close();
            }

            tsbExternal.Checked = ExternalMode;
            htmlEditor.ToolStrip.MiniMode = !ExternalMode;
        }

        void externalDialog_Resize(object sender, EventArgs e)
        {
            if (externalDialog != null
                && !externalDialog.IsDisposed
                && externalDialog.WindowState == FormWindowState.Normal)
            {
                ExternalDialogSize = externalDialog.Size;
                Options.Current.SetValue(ExternalRemarkDialogSize, ExternalDialogSize);
            }
        }

        void externalDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (IsDisposed || Disposing)
                return;

            ExternalMode = false;
        }

        protected virtual void OnCurrentObjectChanged(IRemark old)
        {
            if (old != null)
            {
                old.RemarkChanged -= CurrentObject_RemarkChanged;
            }

            htmlEditor.Modified = false;

            if (CurrentObject != null)
            {
                htmlEditor.Text = CurrentObject.Remark;
                htmlEditor.Enabled = true;
                CurrentObject.RemarkChanged += CurrentObject_RemarkChanged;
            }
            else
            {
                htmlEditor.Text = null;
                htmlEditor.Enabled = false;
            }
        }

        void CurrentObject_RemarkChanged(object sender, EventArgs e)
        {
            if (CurrentObject != null)
            {
                htmlEditor.Text = CurrentObject.Remark;
            }
        }

        void htmlEditor_TextChanged(object sender, EventArgs e)
        {
            if (!ReadOnly && CurrentObject != null)
            {
                CurrentObject.Remark = htmlEditor.Text;
            }
        }

        void tsbExternal_Click(object sender, EventArgs e)
        {
            ExternalMode = !ExternalMode;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (ExternalMode)
            {
                var image = Properties.Resources.external;
                var rect = ClientRectangle;
                PaintHelper.DrawImageInRange(e.Graphics, image, rect);
            }
        }

        protected override void OnReadOnlyChanged()
        {
            base.OnReadOnlyChanged();

            OnEditModeChanged();
        }

        void OnEditModeChanged()
        {
            if (ReadOnly || !EditMode)
                htmlEditor.ReadOnly = true;
            else
                htmlEditor.ReadOnly = false;

            //tsbEdit.Checked = !htmlEditor.ReadOnly && this.EditMode;
            if (htmlEditor.ReadOnly || !this.EditMode)
            {
                tsbEdit.Image = Properties.Resources.edit;
                tsbEdit.Text = Lang._("Edit");
            }
            else
            {
                tsbEdit.Image = Properties.Resources.save;
                tsbEdit.Text = Lang._("Save");
            }
        }

        protected override void OnValidated(EventArgs e)
        {
            base.OnValidated(e);

            if (EditMode && !ReadOnly && CurrentObject != null)
            {
                CurrentObject.Remark = htmlEditor.Text;
            }
        }

        void tsbEdit_Click(object sender, EventArgs e)
        {
            if (EditMode)
            {
                if (!ReadOnly && CurrentObject != null)
                {
                    CurrentObject.Remark = htmlEditor.Text;
                }

                EditMode = false;
            }
            else
            {
                if (ReadOnly)
                    return;

                EditMode = true;
            }
        }

        void LanguageManage_CurrentChanged(object sender, EventArgs e)
        {

            tsbExternal.Text = Lang._("External");
            tsbEdit.Text = Lang._("Edit");
        }
    }
}
