using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Model;

namespace Blumind.Dialogs
{
    partial class IconLibraryDialog : BaseDialog
    {
        public IconLibraryDialog()
        {
            InitializeComponent();
            ShowIcon = true;
            MinimumSize = Size;
            Icon = Properties.Resources.icon_lib;
            LabSize.Text = string.Empty;
            LabName.Text = string.Empty;

            AfterInitialize();
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            Text = CurrentLanguage["Icon Library"];
            BtnClose.Text = CurrentLanguage.GetTextWithAccelerator("Close", 'C');
            TsbAddFiles.Text = CurrentLanguage["Add Image Files"];
            TsbAddFromInternet.Text = CurrentLanguage["Download Image From Internet"];
            TsbRefresh.Text = CurrentLanguage["Refresh"];
            TsbDelete.Text = CurrentLanguage["Delete"];
            TsbResize.Text = CurrentLanguage["Resize"];
        }

        private void imageLibraryListBox1_SelectionChanged(object sender, System.EventArgs e)
        {
            if (imageLibraryListBox1.SelectedIndexes.Length == 1 && imageLibraryListBox1.SelectedItem != null)
            {
                Picture pic = imageLibraryListBox1.SelectedItem;
                PicPreview.Image = pic.Data;
                LabName.Text = pic.Name;
                LabSize.Text = string.Format("Size: {0}X{1}", pic.Data.Width, pic.Data.Height);
            }
            else
            {
                PicPreview.Image = null;
                LabName.Text = string.Empty;
                LabSize.Text = string.Empty;
            }
        }

        private void TsbAddFiles_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog dialog = Picture.GetOpenFileDialog();
            dialog.Multiselect = true;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                foreach (string filename in dialog.FileNames)
                {
                    MyIconLibrary.Share.AddFile(filename);
                }

                imageLibraryListBox1.RefreshItems();
            }
        }

        private void TsbAddFromInternet_Click(object sender, System.EventArgs e)
        {
            InternetImageDialog dialog = new InternetImageDialog();
            dialog.AddToLibrary = true;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {


                imageLibraryListBox1.RefreshItems();
            }
        }

        private void TsbRefresh_Click(object sender, System.EventArgs e)
        {
            MyIconLibrary.Share.Refresh();
            imageLibraryListBox1.RefreshItems();
        }

        private void TsbDelete_Click(object sender, System.EventArgs e)
        {
            if (imageLibraryListBox1.SelectedIndexes.Length > 0)
            {
                string msg = CurrentLanguage.GetText("Are you sure delete the selected items?");

                if (Helper.ShowMessage(msg, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    imageLibraryListBox1.SuspendLayout();
                    int[] items = imageLibraryListBox1.SelectedIndexes;
                    for (int i = items.Length - 1; i >= 0; i--)
                    {
                        int index = items[i];
                        Picture pic = imageLibraryListBox1.Items[index];
                        if (MyIconLibrary.Share.Delete(pic))
                        {
                            imageLibraryListBox1.Items.RemoveAt(index);
                        }
                    }
                    imageLibraryListBox1.ResumeLayout();
                }
            }
        }

        private void TsbResize_Click(object sender, System.EventArgs e)
        {

        }

        private void BtnClose_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
