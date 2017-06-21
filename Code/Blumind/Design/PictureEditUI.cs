using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Blumind.Core;
using Blumind.Dialogs;
using Blumind.Globalization;
using Blumind.Model;
using Blumind.Model.Widgets;

namespace Blumind.Controls
{
    class PictureEditUI : Control
    {
        IWindowsFormsEditorService Service;
        PictureWidget.PictureDesign _CurrentObject;
        Button BtnOpenFile;
        Button BtnUrl;
        Label LabShareLibrary;
        ImageLibraryListBox LsbShareLibrary;
        LinkLabel LnkManageLib;
        LinkLabel LnkRefreshLib;
        Form OwnerDialog;

        public PictureEditUI()
        {
            BtnOpenFile = new Button();
            BtnOpenFile.Text = Lang.GetTextWithEllipsis("Open");
            BtnOpenFile.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            //BtnOpenFile.FlatStyle = FlatStyle.Popup;
            BtnOpenFile.Click += new EventHandler(BtnOpenFile_Click);

            BtnUrl = new Button();
            BtnUrl.Text = Lang.GetTextWithEllipsis("From Internet");
            BtnUrl.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            //BtnUrl.FlatStyle = FlatStyle.Popup;
            BtnUrl.Click += new EventHandler(BtnUrl_Click);

            //LsbDocLibrary = new ImageLibraryListBox();
            //LsbDocLibrary.Height = LsbDocLibrary.CellSize.Height + 2 + SystemInformation.HorizontalScrollBarHeight;
            //LsbDocLibrary.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            //LsbDocLibrary.ExtendOrientation = Orientation.Horizontal;

            LabShareLibrary = new Label();
            LabShareLibrary.Anchor = AnchorStyles.Left;
            LabShareLibrary.Text = Lang.GetText("Icon Library");

            LsbShareLibrary = new ImageLibraryListBox();
            LsbShareLibrary.Dock = DockStyle.Fill;
            LsbShareLibrary.Click += new EventHandler(LsbShareLibrary_Click);

            LnkManageLib = new LinkLabel();
            LnkManageLib.Anchor = AnchorStyles.Left;
            LnkManageLib.AutoSize = true;
            LnkManageLib.Text = Lang.GetTextWithEllipsis("Manage My Icon Library");
            LnkManageLib.LinkClicked += new LinkLabelLinkClickedEventHandler(BtnManageLib_LinkClicked);

            LnkRefreshLib = new LinkLabel();
            LnkRefreshLib.Anchor = AnchorStyles.Right;
            LnkRefreshLib.AutoSize = true;
            LnkRefreshLib.Text = Lang.GetText("Refresh");
            LnkRefreshLib.LinkClicked += new LinkLabelLinkClickedEventHandler(LnkRefreshLib_LinkClicked);

            // links
            TableLayoutPanel plinks = new TableLayoutPanel();
            plinks.Dock = DockStyle.Fill;
            plinks.ColumnCount = 2;
            plinks.Controls.Add(LnkManageLib, 0, 0);
            plinks.Controls.Add(LnkRefreshLib, 1, 0);
            plinks.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            plinks.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            TableLayoutPanel panel = new TableLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.ColumnCount = 2;
            panel.RowCount = 5;
            panel.Controls.Add(BtnOpenFile, 0, 0);
            panel.Controls.Add(BtnUrl, 1, 0);
            //panel.Controls.Add(LsbDocLibrary, 0, 1);
            panel.Controls.Add(LabShareLibrary, 0, 1);
            panel.Controls.Add(LsbShareLibrary, 0, 2);
            panel.Controls.Add(plinks, 0, 3);

            panel.SetColumnSpan(LabShareLibrary, 2);
            panel.SetColumnSpan(LsbShareLibrary, 2);
            //panel.SetColumnSpan(LsbDocLibrary, 2);
            panel.SetColumnSpan(plinks, 2);
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            //panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 1.0f));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, .5f));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, .5f));
            Controls.Add(panel);

            BackColor = SystemColors.Control;//.Menu;
            ForeColor = SystemColors.ControlText;
            Font = SystemFonts.MessageBoxFont;
        }

        public PictureWidget.PictureDesign CurrentObject
        {
            get { return _CurrentObject; }
            set { _CurrentObject = value; }
        }

        public void Initialize(IWindowsFormsEditorService service)
        {
            Service = service;
        }

        public void Initialize(Form ownerDialog)
        {
            OwnerDialog = ownerDialog;
        }

        void BtnUrl_Click(object sender, EventArgs e)
        {
            var dialog = new InternetImageDialog();
            if (CurrentObject != null)
            {
                if (CurrentObject.SourceType == PictureSource.Web)
                    dialog.Url = CurrentObject.Url;
                dialog.PreviewImage = CurrentObject.Data;
                dialog.AddToLibrary = CurrentObject.AddToLibrary;
                dialog.LimitImageSize = CurrentObject.LimitImageSize;
                dialog.ImageEmbedIn = CurrentObject.EmbedIn;
            }

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                CurrentObject = new PictureWidget.PictureDesign();
                CurrentObject.SourceType = dialog.SourceType;
                CurrentObject.Url = dialog.Url;
                CurrentObject.AddToLibrary = dialog.AddToLibrary;
                CurrentObject.LimitImageSize = dialog.LimitImageSize;
                CurrentObject.Name = Path.GetFileNameWithoutExtension(dialog.Url);
                CurrentObject.EmbedIn = dialog.ImageEmbedIn;

                CloseDropDown(true);
            }
        }

        void BtnOpenFile_Click(object sender, EventArgs e)
        {
            var dialog = Picture.GetOpenFileDialog();

            bool? embedin = null;
            if (CurrentObject != null)
            {
                if (CurrentObject.SourceType == PictureSource.File)
                    dialog.FileName = CurrentObject.Url;
                embedin = CurrentObject.EmbedIn;
            }

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                CurrentObject = new PictureWidget.PictureDesign();
                CurrentObject.SourceType = PictureSource.File;
                CurrentObject.Url = dialog.FileName;
                CurrentObject.Name = Path.GetFileNameWithoutExtension(dialog.FileName);

                if (embedin.HasValue)
                    CurrentObject.EmbedIn = embedin.Value;

                CloseDropDown(true);
            }
        }

        void LsbShareLibrary_Click(object sender, EventArgs e)
        {
            if (LsbShareLibrary.SelectedIndex > -1)
            {
                CurrentObject = new PictureWidget.PictureDesign();
                CurrentObject.SourceType = PictureSource.Library;
                CurrentObject.Url = LsbShareLibrary.SelectedItem.Name;
                CurrentObject.Name = Path.GetFileNameWithoutExtension(LsbShareLibrary.SelectedItem.Name);
                CloseDropDown(true);
            }
        }

        void BtnManageLib_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(MyIconLibrary.BaseDirectory2) && !Directory.Exists(MyIconLibrary.BaseDirectory2))
            {
                try
                {
                    Directory.CreateDirectory(MyIconLibrary.BaseDirectory2);
                }
                catch (System.Exception ex)
                {
                    Helper.WriteLog(ex);
                    this.ShowMessage(ex.Message, MessageBoxIcon.Error);
                    return;
                }
            }

            Helper.OpenUrl(MyIconLibrary.BaseDirectory2);
        }

        void LnkRefreshLib_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MyIconLibrary.Share.Refresh();
            LsbShareLibrary.RefreshItems();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            if (LsbShareLibrary.CanFocus)
                LsbShareLibrary.Focus();
        }

        public DialogResult ShowDialog(IWin32Window owner)
        {
            StandardDialog dialog = new StandardDialog();
            dialog.Text = Text;
            dialog.SetMainControl(this);
            Initialize(dialog);
            return dialog.ShowDialog(owner);
        }

        void CloseDropDown(bool isOk)
        {
            if (Service != null)
            {
                Service.CloseDropDown();
            }
            else if (OwnerDialog != null)
            {
                OwnerDialog.DialogResult = isOk ? DialogResult.OK : DialogResult.Cancel;
                OwnerDialog.Close();
            }
        }
    }
}
