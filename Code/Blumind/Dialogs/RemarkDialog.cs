using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Blumind.Configuration;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Globalization;
using Blumind.Model;
using Blumind.Model.Widgets;

namespace Blumind.Dialogs
{
    class RemarkDialog : BaseDialog, IWidgetEditDialog
    {
        public const string RemarkDialogSize = "RemarkDialogSize";
        HtmlEditor htmlEditor;
        Button btnEdit;
        Button btnSave;
        Button btnCancelEdit;
        Button btnClose;
        bool _IsEditMode;
        IRemark _RemarkObject;

        public RemarkDialog()
        {
            InitializeComponent();

            AfterInitialize();
        }

        void InitializeComponent()
        {
            htmlEditor = new HtmlEditor();
            htmlEditor.ReadOnly = ReadOnly;

            btnEdit = new Button();
            btnEdit.Text = "Edit";
            btnEdit.Click += btnEdit_Click;

            btnSave = new Button();
            btnSave.Text = "Accept Changes";
            btnSave.Width = 120;
            btnSave.Click += btnAcceptChanges_Click;

            btnCancelEdit = new Button();
            btnCancelEdit.Text = "Cancel";
            btnCancelEdit.Click += btnCancelEdit_Click;

            btnClose = new Button();
            btnClose.Text = "Close";
            btnClose.Click += btnClose_Click;

            SuspendLayout();
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            CancelButton = btnClose;
            Controls.AddRange(new Control[] { htmlEditor, btnEdit, btnSave, btnCancelEdit, btnClose });
            MinimumSize = new Size(400, 300);
            if (Options.Current.Contains(RemarkDialogSize))
                Size = Options.Current.GetValue(RemarkDialogSize, Size);
            ResumeLayout(false);
        }

        public string Remark
        {
            get { return htmlEditor.Text; }
            set { htmlEditor.Text = value; }
        }

        public bool IsEditMode
        {
            get { return _IsEditMode; }
            set 
            {
                if (_IsEditMode != value)
                {
                    _IsEditMode = value;
                    OnIsEditModeChanged();
                }
            }
        }

        [DefaultValue(null)]
        public IRemark RemarkObject
        {
            get { return _RemarkObject; }
            set 
            {
                if (_RemarkObject != value)
                {
                    var ce = new CancelEventArgs();
                    OnRemarkObjectChanging(_RemarkObject, value, ce);
                    if (ce.Cancel)
                        return;

                    _RemarkObject = value;
                    OnRemarkObjectChanged();
                }
            }
        }

        Widget IWidgetEditDialog.Widget
        {
            get
            {
                return RemarkObject as Widget;
            }
            set
            {
                RemarkObject = value;
            }
        }

        protected override bool ShowButtonArea
        {
            get
            {
                return true;
            }
        }

        void OnIsEditModeChanged()
        {
            if (!IsEditMode && htmlEditor.ViewType != HtmlEditorViewType.Design)
                htmlEditor.ViewType = HtmlEditorViewType.Design;
            RefreshViewStatus();
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            Text = Lang._("Remark");
            btnEdit.Text = Lang._("Edit");
            btnSave.Text = Lang._("Accept Changes");
            btnCancelEdit.Text = Lang._("Cancel");
            btnClose.Text = Lang._("Close");
        }

        void RefreshViewStatus()
        {
            var readOnly = !IsEditMode || ReadOnly;

            htmlEditor.ReadOnly = readOnly;
            btnSave.Visible = btnSave.Enabled = !ReadOnly && IsEditMode;
            btnCancelEdit.Visible = btnCancelEdit.Enabled = !ReadOnly && IsEditMode;
            btnEdit.Visible = btnEdit.Enabled = !ReadOnly && !IsEditMode;
        }

        protected override void OnReadOnlyChanged()
        {
            base.OnReadOnlyChanged();

            RefreshViewStatus();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
            {
                RefreshViewStatus();
            }
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (htmlEditor != null)
            {
                htmlEditor.Bounds = this.ControlsRectangle;
            }

            if (btnEdit != null)
            {
                LocateButtonsLeft(new Control[] { btnEdit, btnSave, btnCancelEdit });
            }

            if (btnClose != null)
            {
                LocateButtonsRight(new Control[] { btnClose });
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            Options.Current.SetValue(RemarkDialogSize, Size);
        }

        void btnEdit_Click(object sender, EventArgs e)
        {
            if (!ReadOnly)
            {
                IsEditMode = true;
            }
        }

        void btnCancelEdit_Click(object sender, EventArgs e)
        {
            IsEditMode = false;
        }

        void btnAcceptChanges_Click(object sender, EventArgs e)
        {
            if (RemarkObject != null && IsEditMode)
            {
                htmlEditor.EndEdit();
                if (htmlEditor.Modified)
                {
                    RemarkObject.Remark = Remark;
                }
            }

            IsEditMode = false;
        }

        void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        void OnRemarkObjectChanging(IRemark oldValue, IRemark newValue, CancelEventArgs e)
        {
            if (oldValue != null && IsEditMode)
            {
                htmlEditor.EndEdit();
                if (htmlEditor.Modified)
                {
                    var msg = Lang._("The content has changed, do you want to accept the changes?");
                    var dr = this.ShowMessage(msg, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    switch (dr)
                    {
                        case System.Windows.Forms.DialogResult.Yes:
                            oldValue.Remark = Remark;
                            break;
                        case System.Windows.Forms.DialogResult.No:
                            break;
                        case System.Windows.Forms.DialogResult.Cancel:
                            e.Cancel = true;
                            break;
                    }
                }
            }
        }

        void OnRemarkObjectChanged()
        {
            if (RemarkObject != null)
                Remark = RemarkObject.Remark;
            else
                Remark = null;

            IsEditMode = false;
        }

        public void ShowDialog(IRemark remarkObject, bool readOnly)
        {
            this.RemarkObject = remarkObject;
            this.ReadOnly = readOnly;

            if (remarkObject != null)
            {
                if (!Visible)
                    this.Show(Program.MainForm);
            }
            else
            {
                if (Visible)
                    this.Hide();
            }
        }

        #region GlobalRemarkDialog
        static RemarkDialog _Global;

        public static RemarkDialog Global
        {
            get
            {
                if (_Global == null || _Global.IsDisposed)
                    _Global = new RemarkDialog();

                return _Global;
            }
        }
        #endregion
    }
}
