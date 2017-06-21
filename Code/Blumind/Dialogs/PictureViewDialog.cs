using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Dialogs
{
    class PictureViewDialog : BaseDialog
    {
        Image _Image;
        string _ImageName;
        ImageBox imageBox1;
        ToolStrip toolStrip1;
        ToolStripButton TsbSave;
        ToolStripButton TsbCopy;
        ToolStripButton TsbRotateLeft;
        ToolStripButton TsbRotateRight;
        ToolStripButton TsbZoomOut;
        ToolStripButton TsbZoomIn;
        ToolStripMenuItem MenuZoomFitWidth;
        ToolStripMenuItem MenuZoomFitHeight;
        ToolStripMenuItem MenuZoomFitPage;
        ToolStripDropDownButton TsbZooms;
        ToolStripButton TsbFullScreen;

        public PictureViewDialog()
        {
            InitializeComponents();
            AfterInitialize();
        }

        public PictureViewDialog(Image image)
            : this()
        {
            Image = image;
        }

        public Image Image
        {
            get { return _Image; }
            set 
            {
                if (_Image != value)
                {
                    _Image = value;
                    OnPictureChanged();
                }
            }
        }

        public string ImageName
        {
            get { return _ImageName; }
            set 
            {
                if (_ImageName != value)
                {
                    _ImageName = value;
                    OnImageNameChanged();
                }
            }
        }

        void InitializeComponents()
        {
            toolStrip1 = new ToolStrip();
            imageBox1 = new ImageBox();
            SuspendLayout();

            //
            InitializeToolStrip();

            //
            imageBox1.Dock = DockStyle.Fill;
            imageBox1.ZoomChanged += imageBox1_ZoomChanged;

            //
            this.MaximizeBox = true;
            this.Icon = Properties.Resources.image_icon;
            StartPosition = FormStartPosition.CenterScreen;
            Controls.AddRange(new Control[] { imageBox1, toolStrip1 });
            ResumeLayout();
        }

        void InitializeToolStrip()
        {
            TsbCopy = new ToolStripButton();
            TsbSave = new ToolStripButton();
            TsbRotateLeft = new ToolStripButton();
            TsbRotateRight = new ToolStripButton();
            TsbZoomOut = new ToolStripButton();
            TsbZoomIn = new ToolStripButton();
            TsbZooms = new ToolStripDropDownButton();
            MenuZoomFitHeight = new ToolStripMenuItem();
            MenuZoomFitPage = new ToolStripMenuItem();
            MenuZoomFitWidth = new ToolStripMenuItem();
            TsbFullScreen = new ToolStripButton();
            toolStrip1.SuspendLayout();

            //
            TsbSave.DisplayStyle = ToolStripItemDisplayStyle.Image;
            TsbSave.Image = Properties.Resources.save;
            TsbSave.Text = "Save As";
            TsbSave.Click += TsbSave_Click;

            //
            TsbCopy.DisplayStyle = ToolStripItemDisplayStyle.Image;
            TsbCopy.Image = Properties.Resources.copy;
            TsbCopy.Text = "Copy";
            TsbCopy.Click += TsbCopy_Click;

            //
            TsbRotateLeft.DisplayStyle = ToolStripItemDisplayStyle.Image;
            TsbRotateLeft.Image = Properties.Resources.rotate_left;
            TsbRotateLeft.Text = "Rotate Left";
            TsbRotateLeft.Click += TsbRotateLeft_Click;

            // TsbRotateRight
            TsbRotateRight.DisplayStyle = ToolStripItemDisplayStyle.Image;
            TsbRotateRight.Image = Properties.Resources.rotate_right;
            TsbRotateRight.Text = "Rotate Right";
            TsbRotateRight.Click += TsbRotateRight_Click;

            // TsbZoomIn
            TsbZoomIn.DisplayStyle = ToolStripItemDisplayStyle.Image;
            TsbZoomIn.Image = Blumind.Properties.Resources.zoom_in;
            TsbZoomIn.Text = "Zoom In";
            TsbZoomIn.Click += TsbZoomIn_Click;

            //
            TsbZoomOut.DisplayStyle = ToolStripItemDisplayStyle.Image;
            TsbZoomOut.Image = Blumind.Properties.Resources.zoom_out;
            TsbZoomOut.Text = "Zoom Out";
            TsbZoomOut.Click += tsbZoomOut_Click;

            // MenuZoomFitPage
            MenuZoomFitPage.Name = "MenuZoomFitPage";
            MenuZoomFitPage.Text = "&Fit Page";
            MenuZoomFitPage.Click += MenuZoomFitPage_Click;

            // MenuZoomFitWidth
            MenuZoomFitWidth.Name = "MenuZoomFitWidth";
            MenuZoomFitWidth.Text = "Fit Width";
            MenuZoomFitWidth.Click += MenuZoomFitWidth_Click;

            // MenuZoomFitHeight
            MenuZoomFitHeight.Name = "MenuZoomFitHeight";
            MenuZoomFitHeight.Text = "Fit Height";
            MenuZoomFitHeight.Click += MenuZoomFitHeight_Click;

            //
            TsbZooms = new ToolStripDropDownButton();
            TsbZooms.DisplayStyle = ToolStripItemDisplayStyle.Text;
            TsbZooms.DropDownItems.AddRange(new ToolStripItem[] { 
                MenuZoomFitPage,
                MenuZoomFitWidth,
                MenuZoomFitHeight,
                new ToolStripSeparator()
            });
            TsbZooms.Text = imageBox1.Zoom.ToString("P0");
            float[] zooms = new float[] { 0.25f, 0.5f, 0.75f, 1.0f, 1.5f, 2.0f, 3.0f, 4.0f };
            foreach (float zoom in zooms)
            {
                ToolStripMenuItem miZoom = new ToolStripMenuItem();
                miZoom.Text = string.Format("{0}%", (int)(zoom * 100));
                miZoom.Tag = zoom;
                miZoom.Click += miZoom_Click;
                TsbZooms.DropDownItems.Add(miZoom);
            }

            // TsbFullScreen
            TsbFullScreen.DisplayStyle = ToolStripItemDisplayStyle.Image;
            TsbFullScreen.Image = global::Blumind.Properties.Resources.full_screen;
            TsbFullScreen.Text = "Full Screen";
            TsbFullScreen.Click += TsbFullScreen_Click;
            
            //
            toolStrip1.Dock = DockStyle.Top;
            toolStrip1.Items.AddRange(new ToolStripItem[]{
                TsbSave,
                TsbCopy,
                new ToolStripSeparator(),
                TsbRotateLeft,
                TsbRotateRight,
                new ToolStripSeparator(),
                TsbZoomOut,
                TsbZooms,
                TsbZoomIn,
                new ToolStripSeparator(),
                TsbFullScreen
            });
            toolStrip1.ResumeLayout();
        }

        void TsbCopy_Click(object sender, EventArgs e)
        {
            if (imageBox1.Image != null)
            {
                try
                {
                    Clipboard.SetImage(imageBox1.Image);
                }
                catch(System.Exception ex)
                {
                    this.ShowMessage(ex);
                }
            }
        }

        void TsbSave_Click(object sender, EventArgs e)
        {
            if (imageBox1.Image != null)
            {
                SaveImageAs(imageBox1.Image);
            }
        }

        void TsbFullScreen_Click(object sender, EventArgs e)
        {
            this.FullScreen = !FullScreen;
            TsbFullScreen.Checked = FullScreen;
        }

        void TsbRotateRight_Click(object sender, EventArgs e)
        {
            RotateImage(RotateFlipType.Rotate90FlipNone);
        }

        void TsbRotateLeft_Click(object sender, EventArgs e)
        {
            RotateImage(RotateFlipType.Rotate270FlipNone);
        }

        void RotateImage(RotateFlipType rotateFlipType)
        {
            if (Image == null)
                return;

            var image = imageBox1.Image;
            if (Image == imageBox1.Image)
            {
                var bitmap = new Bitmap(Image.Width, Image.Height);
                using (var grf = Graphics.FromImage(bitmap))
                {
                    grf.DrawImage(Image, new Rectangle(0, 0, Image.Width, Image.Height), 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel);
                }
                image = bitmap;
            }

            image.RotateFlip(rotateFlipType);
            if (image != imageBox1.Image)
                imageBox1.Image = image;
            else
            {
                imageBox1.RefreshView();
                imageBox1.Invalidate();
            }
        }

        void imageBox1_ZoomChanged(object sender, EventArgs e)
        {
            switch (imageBox1.ZoomType)
            {
                case ZoomType.Custom:
                    TsbZooms.Text = imageBox1.Zoom.ToString("P0");
                    break;
                case ZoomType.FitHeight:
                    TsbZooms.Text = Lang._("Fit Height");
                    break;
                case ZoomType.FitWidth:
                    TsbZooms.Text = Lang._("Fit Width");
                    break;
                case ZoomType.FitPage:
                    TsbZooms.Text = Lang._("Fit Page");
                    break;
            }
        }

        void TsbZoomIn_Click(object sender, EventArgs e)
        {
            imageBox1.ZoomIn();
        }

        void MenuZoomFitHeight_Click(object sender, EventArgs e)
        {
            imageBox1.ZoomType = ZoomType.FitHeight;
        }

        void MenuZoomFitWidth_Click(object sender, EventArgs e)
        {
            imageBox1.ZoomType = ZoomType.FitWidth;
        }

        void MenuZoomFitPage_Click(object sender, EventArgs e)
        {
            imageBox1.ZoomType = ZoomType.FitPage;
        }

        void tsbZoomOut_Click(object sender, EventArgs e)
        {
            imageBox1.ZoomOut();
        }

        void miZoom_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                var mi = (ToolStripMenuItem)sender;
                if (mi.Tag is float && imageBox1 != null)
                {
                    imageBox1.Zoom = (float)mi.Tag;
                    imageBox1.ZoomType = ZoomType.Custom;
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Image != null)
            {
                var wk = Screen.GetWorkingArea(this);
                this.ClientSize = new Size(
                    Math.Min(wk.Width, Math.Max(250, Image.Width)),
                    Math.Min(wk.Height, Math.Max(200, Image.Height + toolStrip1.Height)));
                this.Size = new Size(Math.Min(wk.Width, Width), Math.Min(wk.Height, Height));
                //this.StartPosition = FormStartPosition.Manual;
                this.MoveToCenterScreen();
            }
        }

        protected virtual void OnPictureChanged()
        {
            imageBox1.Image = this.Image;
        }

        protected virtual void OnImageNameChanged()
        {
            Text = ImageName;
        }

        public override void ApplyTheme(UITheme theme)
        {
            if (toolStrip1 != null)
            {
                toolStrip1.Renderer = theme.ToolStripRenderer;
            }

            if (imageBox1 != null)
            {
                imageBox1.BackColor = theme.Colors.MediumDark;
            }
        }

        bool SaveImageAs(System.Drawing.Image image)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = DocumentType.GetImageTypeFilters();
            dialog.FileName = ImageName;

            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    var imageFormat = DocumentType.GetImageFormat(Path.GetExtension(dialog.FileName));
                    if (imageFormat == null)
                        image.Save(dialog.FileName);
                    else
                        image.Save(dialog.FileName, imageFormat);
                    return true;
                }
                catch (System.Exception ex)
                {
                    this.ShowMessage(ex);
                }
            }

            return false;
        }
    }
}
