using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Blumind.Configuration;
using Blumind.Controls;
using Blumind.Globalization;
using Blumind.Model;
using Blumind.Model.Widgets;
using Blumind.Resources;

namespace Blumind.Dialogs
{
    partial class InternetImageDialog : StandardDialog
    {
        string _Url;
        static List<string> HistoryUrls = new List<string>();
        Image _PreviewImage;
        PictureSource _SourceType = PictureSource.Web;

        public InternetImageDialog()
        {
            InitializeComponent();
            ShowIcon = true;
            Icon = Properties.Resources.globe_network;
            LabInfo.Text = string.Empty;
            MinimumSize = Size;
            this.SetFontNotScale(SystemFonts.MessageBoxFont);
            ImageEmbedIn = Options.Current.GetBool(Blumind.Configuration.OptionNames.Miscellaneous.WebImageEmbedIn);

            // list history
            for (int i = HistoryUrls.Count - 1; i >= 0; i--)
            {
                CmbUrl.Items.Add(HistoryUrls[i]);
            }

            InitializeRecommendedResources();
            AfterInitialize();
        }

        [Browsable(false)]
        public string Url
        {
            get { return _Url; }
            set 
            {
                if (_Url != value)
                {
                    _Url = value;
                    OnUrlChanged();
                }
            }
        }

        [Browsable(false)]
        public Image PreviewImage
        {
            get { return _PreviewImage; }
            set
            {
                if (_PreviewImage != value)
                {
                    _PreviewImage = value;
                    OnPreviewImageChanged();
                }
            }
        }

        [Browsable(false)]
        public bool AddToLibrary
        {
            get { return CkbAddToLibrary.Checked; }
            set { CkbAddToLibrary.Checked = value; }
        }

        [Browsable(false)]
        public bool LimitImageSize
        {
            get { return CkbLimitImageSize.Checked; }
            set { CkbLimitImageSize.Checked = value; }
        }

        [Browsable(false)]
        public PictureSource SourceType
        {
            get { return _SourceType; }
            private set { _SourceType = value; }
        }

        [Browsable(false)]
        public bool ImageEmbedIn
        {
            get { return CkbEmbedIn.Checked; }
            set { CkbEmbedIn.Checked = value; }
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            PicPreview.Size = new Size(160, 120);
        }

        void InitializeRecommendedResources()
        {
            contextMenuStrip1.Items.Clear();

            foreach (string[] link in RecommendedResources.IconSites)
            {
                ToolStripMenuItem mi = new ToolStripMenuItem();
                mi.Text = link[0];
                mi.Tag = link[1];
                mi.ToolTipText = link[1];
                mi.Image = Properties.Resources.internet;
                mi.Click += new System.EventHandler(MenuREcommendedResources_Click);
                contextMenuStrip1.Items.Add(mi);
            }

            BtnRecommendedResources.Enabled = contextMenuStrip1.Items.Count > 0;
        }

        void OnUrlChanged()
        {
            CmbUrl.Text = Url;
        }

        protected override bool OnOKButtonClick()
        {
            string url = CmbUrl.Text.Trim();
            if (string.IsNullOrEmpty(url))
                return false;

            if (HistoryUrls.Contains(url))
                HistoryUrls.Remove(url);
            HistoryUrls.Add(url);

            Url = url;

            if (CkbAddToLibrary.Checked)
            {
                try
                {
                    Url = MyIconLibrary.Share.AddNew(System.IO.Path.GetFileName(Url), Picture.LoadImageFromWeb(url, CkbLimitImageSize.Checked));
                    SourceType = PictureSource.Library;
                }
                catch(System.Exception ex)
                {
                    Helper.WriteLog(ex);
                }
            }

            return base.OnOKButtonClick();
        }

        protected virtual void OnPreviewImageChanged()
        {
            if (PreviewImage == null)
            {
                LabInfo.Text = string.Empty;
            }
            else
            {
                LabInfo.Text = string.Format("{0}:{1}  {2}:{3}",
                    Lang._("Width"), PreviewImage.Width,
                    Lang._("Height"), PreviewImage.Height);
            }

            PicPreview.Image = PreviewImage;
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            Text = Lang._("Download Image From Internet");
            CkbAddToLibrary.Text = Lang._("Add To My Icon Library");
            CkbLimitImageSize.Text = Lang._("Limit Image Size");
            CkbEmbedIn.Text = Lang._("Embed In");
            BtnPreview.Text = Lang._("Preview");
            BtnRecommendedResources.Text = Lang._("Recommended Resources");
        }

        void PreviewUrl()
        {
            string url = CmbUrl.Text.Trim();
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                PreviewImage = Picture.LoadImageFromWeb(url);
                Cursor.Current = Cursors.Default;
            }
            catch (System.Exception ex)
            {
                Helper.WriteLog(ex);
                Cursor.Current = Cursors.Default;
                this.ShowMessage(ex.Message, MessageBoxIcon.Error);
            }
        }

        void BtnPreview_Click(object sender, System.EventArgs e)
        {
            PreviewUrl();
        }

        void CmbUrl_TextChanged(object sender, System.EventArgs e)
        {
            BtnPreview.Enabled = CmbUrl.Text.Trim() != string.Empty;
        }

        void BtnRecommendedResources_Click(object sender, System.EventArgs e)
        {
            if (contextMenuStrip1.Items.Count > 0)
                contextMenuStrip1.Show(BtnRecommendedResources, new Point(0, BtnRecommendedResources.Height));
        }

        void MenuREcommendedResources_Click(object sender, System.EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                string url = ((ToolStripMenuItem)sender).Tag as string;
                if (!string.IsNullOrEmpty(url))
                {
                    Helper.OpenUrl(url);
                }
            }
        }

        public override void ApplyTheme(UITheme theme)
        {
            if (theme != null)
            {
                if (contextMenuStrip1 != null)
                {
                    contextMenuStrip1.Renderer = theme.ToolStripRenderer;
                }
            }
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (BtnPreview != null)
            {
                BtnPreview.Top = OKButton.Top;
                BtnPreview.Height = OKButton.Height;
            }

            if (BtnRecommendedResources != null)
            {
                BtnRecommendedResources.Top = OKButton.Top;
                BtnRecommendedResources.Height = OKButton.Height;
            }

            if (tableLayoutPanel1 != null)
            {
                tableLayoutPanel1.Bounds = this.ControlsRectangle;
            }
        }

        void CkbEmbedIn_CheckedChanged(object sender, System.EventArgs e)
        {
            Options.Current.SetValue(Blumind.Configuration.OptionNames.Miscellaneous.WebImageEmbedIn, CkbEmbedIn.Checked);
        }
    }
}
