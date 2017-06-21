using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Blumind.Model.Documents;

namespace Blumind.Controls
{
    class FileThumbItem : ThumbItem
    {
        string _Filename;

        public FileThumbItem(string filename, Image image)
        {
            Filename = filename;
            Image = image;

            Initialize();
        }

        public FileThumbItem(Configuration.RecentFileInfo file)
        {
            Filename = file.Filename;
            Image = file.ThumbImage;
            FileIcon = file.Icon;

            if (FileIcon == null && !string.IsNullOrEmpty(Filename))
            {
                if (File.Exists(Filename))
                    FileIcon = IconExtractor.ExtractAssociatedIconImage(Filename);

                if (FileIcon == null)
                    FileIcon = IconExtractor.ExtractSmallIconByExtension(Path.GetExtension(Filename));
            }

            Initialize();
        }

        public Image FileIcon { get; set; }

        bool FileImageIsValid { get; set; }

        public string Filename
        {
            get { return _Filename; }
            set 
            {
                if (_Filename != value)
                {
                    _Filename = value;
                    OnFilenameChanged();
                }
            }
        }

        void Initialize()
        {
            FileImageIsValid = Image != null;

            if (Image == null && !string.IsNullOrEmpty(Filename))
            {
                switch (Path.GetExtension(Filename).ToLower())
                {
                    case Document.Extension:
                        Image = CreateBmdImage();
                        break;
                    default:
                        Image = IconExtractor.ExtractLargeIconByExtension(Path.GetExtension(Filename));
                        break;
                }
            }
        }

        Image CreateBmdImage()
        {
            return Properties.Resources.document_128;//.logo_trans;
        }

        void OnFilenameChanged()
        {
            if (string.IsNullOrEmpty(Filename))
                Text = null;
            else
                Text = Path.GetFileNameWithoutExtension(Filename);
        }

        public override string GetToolTipAt(int x, int y)
        {
            return Filename;
            //return base.GetToolTipAt(x, y);
        }

        protected override bool OnPaint(ThumbViewPaintEventArgs e)
        {
            if (base.OnPaint(e))
            {
                if (FileIcon != null && FileImageIsValid)
                {
                    e.Graphics.DrawImage(FileIcon, Left + 10, Top + 10);
                }
                return true;
            }

            return false;
        }
    }
}
