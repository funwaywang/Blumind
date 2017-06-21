using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Blumind.Canvas;
using Blumind.Controls;
using Blumind.Controls.MapViews;
using Blumind.Core;
using Blumind.Globalization;
using Blumind.Model.MindMaps;

namespace Blumind.Model.Widgets
{
    public enum PictureSource
    {
        File,
        Web,
        Library,
        Document,
        Memory,
    }

    [Serializable]
    public enum PictureSizeType
    {
        Thumb,
        Original,
        Customize,
    }

    [DefaultProperty("Image")]
    class PictureWidget : Widget, IExtendActionProvider
    {
        public const string TypeID = "PICTURE";
        Image _Data;
        string _ImageUrl;
        PictureSource _SourceType;
        PictureDesign _Image;
        PictureSizeType _SizeType;
        bool _EmbedIn;
        Image _ThumbImage;

        public PictureWidget()
        {
            _SizeType = PictureSizeType.Thumb;
            Alignment = WidgetAlignment.Left;
        }

        [DefaultValue(null), Browsable(false)]
        public Image Data
        {
            get { return _Data; }
            set 
            {
                if (_Data != value)
                {
                    //if (_Data != null)
                    //    _Data.Dispose();
                    var old = _Data;
                    _Data = value;

                    OnDataChanged(old);
                }
            }
        }

        [DefaultValue(null), Browsable(false)]
        public string ImageUrl
        {
            get { return _ImageUrl; }
            set { _ImageUrl = value; }
        }

        [DefaultValue(WidgetAlignment.Left)]
        public override WidgetAlignment Alignment
        {
            get
            {
                return base.Alignment;
            }
            set
            {
                base.Alignment = value;
            }
        }

        [LocalDisplayName("Original Size"), LocalCategory("Layout"), ReadOnly(true)]
        public Size OriginalSize { get; private set; }

        [DefaultValue(PictureSource.File), ReadOnly(true), DesignOnly(true)]
        [LocalDisplayName("Source Type"), LocalCategory("Data")]
        [Browsable(false)]
        public PictureSource SourceType
        {
            get { return _SourceType; }
            set { _SourceType = value; }
        }

        [DefaultValue(PictureSizeType.Thumb)]
        [LocalDisplayName("Size Type"), LocalCategory("Layout")]
        public PictureSizeType SizeType
        {
            get { return _SizeType; }
            set
            {
                if (_SizeType != value)
                {
                    var old = _SizeType;
                    _SizeType = value;
                    OnSizeTypeChanged(old);
                }
            }
        }

        [Browsable(false), DefaultValue(false)]
        public Image ThumbImage 
        {
            get { return _ThumbImage; }
            private set
            {
                if (_ThumbImage != value)
                {
                    var old = _ThumbImage;
                    _ThumbImage = value;
                    OnThumbImageChanged(old);
                }
            }
        }
        
        [DefaultValue(null), DesignOnly(true), LocalDisplayName("Image"), LocalCategory("Data")]
        public PictureDesign Image
        {
            get { return _Image; }
            set 
            {
                _Image = value;
                OnImageChanged();
            }
        }

        [DefaultValue(false), LocalDisplayName("Embed In"), LocalCategory("Data")]
        [TypeConverter(typeof(Blumind.Design.BoolConverter))]
        public bool EmbedIn
        {
            get { return _EmbedIn; }
            set
            {
                if (_EmbedIn != value)
                {
                    _EmbedIn = value;
                    OnEmbedInChanged();
                }
            }
        }

        [Browsable(true)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public override string GetTypeID()
        {
            return TypeID;
        }

        void OnDataChanged(System.Drawing.Image old)
        {
            if (Image != null)
                Image.Data = Data;

            OnPropertyChanged("Data", old, Image, ChangeTypes.All);

            //
            OriginalSize = Data == null ? Size.Empty : Data.Size;

            //
            if (SizeType == PictureSizeType.Thumb)
                CreateThumbImage();
        }

        public override void Serialize(XmlDocument dom, XmlElement node)
        {
            base.Serialize(dom, node);

            node.SetAttribute("image_url", ImageUrl);
            node.SetAttribute("size_type", this.SizeType.ToString());
            node.SetAttribute("original_size", ST.ToString(OriginalSize));
            node.SetAttribute("embed_in", ST.ToString(EmbedIn));

            if (ThumbImage != null)
            {
                if(Data == null || !EmbedIn || Data.Width > ThumbImage.Width || Data.Height > ThumbImage.Height)
                    ST.WriteImageNode(node, "thumb", ThumbImage);
            }

            if (Data != null && EmbedIn)
            {
                ST.WriteImageNode(node, "data", Data);
            }
        }

        public override void Deserialize(Version documentVersion, XmlElement node)
        {
            base.Deserialize(documentVersion, node);

            if (node.HasAttribute("image_url"))
                ImageUrl = node.GetAttribute("image_url");
            SizeType = ST.GetEnumValue(node.GetAttribute("size_type"), this.SizeType);
            OriginalSize = ST.GetValue(node.GetAttribute("original_size"), Size.Empty);

            if (ST.HasImageNode(node, "thumb"))
                ThumbImage = ST.ReadImageNode(node, "thumb");

            if (ST.HasImageNode(node))
            {
                Data = ST.ReadImageNode(node);
            }
            else if (ST.HasImageNode(node, "data"))
            {
                Data = ST.ReadImageNode(node, "data");
            }

            EmbedIn = ST.GetBool(node.GetAttribute("embed_in"), EmbedIn);
        }

        public override Size CalculateSize(MindMapLayoutArgs e)
        {
            Size size = Size.Empty;
            switch (SizeType)
            {
                case PictureSizeType.Thumb:
                    if (ThumbImage != null)
                    {
                        try
                        {
                            size = ThumbImage.Size;
                        }
                        catch
                        {
                        }
                    }
                    break;
                case PictureSizeType.Original:
                    if (Data != null)
                        size = Data.Size;
                    else
                        return new Size(16, 16);
                    break;
                case PictureSizeType.Customize:
                    size = new Size(CustomWidth ?? 16, CustomHeight ?? 16);
                    break;
            }

            return size;
        }

        public override void Paint(RenderArgs e)
        {
            //base.Paint(e);

            Image image = Data;
            if (SizeType == PictureSizeType.Thumb && ThumbImage != null)
            {
                image = ThumbImage;
            }

            var rect = DisplayRectangle;
            if (image != null)
            {
                var s = e.Graphics.Save();
                e.Graphics.SetClip(rect);
                e.Graphics.DrawImage(image, rect, 0, 0, image.Width, image.Height);
                e.Graphics.Restore(s);
            }
            else
            {
                e.Graphics.FillRectangle(e.Graphics.SolidBrush(Color.White), rect);
                e.Graphics.DrawRectangle(e.Graphics.Pen(Color.Red), rect.X, rect.Y, rect.Width, rect.Height);
                e.Graphics.DrawLine(e.Graphics.Pen(Color.Red), rect.X, rect.Y, rect.Right - 1, rect.Bottom - 1);
                e.Graphics.DrawLine(e.Graphics.Pen(Color.Red), rect.Right - 1, rect.Y, rect.Left, rect.Bottom - 1);
            }
        }

        public override void CopyTo(Widget widget)
        {
            base.CopyTo(widget);
            if (widget is PictureWidget)
            {
                var pic = (PictureWidget)widget;
                pic.Data = Data;
                pic.CustomWidth = CustomWidth;
                pic.CustomHeight = CustomHeight;
                pic.SourceType = SourceType;
                pic.SizeType = SizeType;
                if (Image != null)
                    pic.Image = Image.Clone();
                else if (ThumbImage != null)
                    pic.ThumbImage = ThumbImage.Clone() as Image;
                pic.ImageUrl = ImageUrl;
                pic.OriginalSize = OriginalSize;
                pic.EmbedIn = EmbedIn;
            }
        }

        void OnImageChanged()
        {
            if (Image != null)
            {
                ImageUrl = Image.Url;
                SourceType = Image.SourceType;
                Data = Image.LoadImage();
                Text = Image.Name;
                EmbedIn = Image.EmbedIn;

                if (SizeType == PictureSizeType.Thumb && ThumbImage == null)
                    CreateThumbImage();
            }
            else
            {
                ImageUrl = null;
                SourceType = PictureSource.File;
                Data = null;

                if (ThumbImage != null)
                {
                    ThumbImage.Dispose();
                    ThumbImage = null;
                }
            }
        }

        public override void OnAddByCommand(Topic parent)
        {
            base.OnAddByCommand(parent);

            //
            /*if (Data != null && Chart != null)
            {
                if ((Data.Width > Chart.PictureThumbSize.Width || Data.Height > Chart.PictureThumbSize.Height))
                    EmbedIn = false;
                else
                    EmbedIn = true;
            }*/
        }

        void OnThumbImageChanged(System.Drawing.Image old)
        {
            if (old != null)
            {
                old.Dispose();
            }
        }

        void OnSizeTypeChanged(PictureSizeType old)
        {
            if (Data == null)
            {
                TryLoadData();
            }

            if (SizeType == PictureSizeType.Thumb)
            {
                if (ThumbImage == null || Data != null)
                {
                    CreateThumbImage();
                }
            }

            OnPropertyChanged("SizeType", old, SizeType, ChangeTypes.All);
        }

        public void CreateThumbImage()
        {
            if (Chart == null)
                return;

            Image sourceImage = Data;
            if (Data == null)
            {
                if (ThumbImage == null || ThumbImage.Size == Chart.PictureThumbSize)
                    return;
                else
                    sourceImage = ThumbImage;
            }

            ThumbImage = PaintHelper.CreateThumbImage(sourceImage, Chart.PictureThumbSize);
            OnPropertyChanged("ThumbImage", null, ThumbImage, ChangeTypes.All);
        }

        protected override void OnChartChanged()
        {
            base.OnChartChanged();

            if (Chart != null && SizeType == PictureSizeType.Thumb )
            {
                if (Data != null && (ThumbImage == null || ThumbImage.Size != Chart.PictureThumbSize))
                    CreateThumbImage();
                else if (Data == null && ThumbImage != null && ThumbImage.Size != Chart.PictureThumbSize)
                    CreateThumbImage();
            }
        }

        protected virtual void OnEmbedInChanged()
        {
            if (EmbedIn && Data == null)
            {
                //TryLoadData();
            }

            if (Image != null)
            {
                Image.EmbedIn = EmbedIn;
            }

            OnPropertyChanged("EmbedIn", !EmbedIn, EmbedIn, ChangeTypes.Data);
        }

        /// <summary>
        /// try load data from url
        /// </summary>
        /// <returns></returns>
        bool TryLoadData()
        {
            if (string.IsNullOrEmpty(ImageUrl))
                return false;

            Image image = null;

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                image = Picture.LoadImageFromUrl(ImageUrl);
                if (image == null)
                    return false;
            }
            catch
            {
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            if (image != null)
            {
                Data = image;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void OnDoubleClick(HandledEventArgs e)
        {
            base.OnDoubleClick(e);
            Enlarge();
            e.Handled = true;
        }

        void Enlarge()
        {
            var image = Data;
            if (image == null)
            {
                if (TryLoadData())
                    image = Data;
                else
                    image = ThumbImage;
            }

            if (image != null)
            {
                var dialog = new Blumind.Dialogs.PictureViewDialog(image);
                dialog.ImageName = this.Text;
                dialog.ShowDialog();
            }
        }

        public IEnumerable<ExtendActionInfo> GetExtendActions(bool readOnly)
        {
            return new ExtendActionInfo[]
            { 
                new ExtendActionInfo("Large View", Properties.Resources.zoom, (sender, e) => Enlarge()) 
            };
        }

        public override void CopyExtendContent(DataObject dataObject)
        {
            base.CopyExtendContent(dataObject);

            if (Data != null )
            {
                dataObject.SetImage(Data);
            }
            else if (ThumbImage != null)
            {
                dataObject.SetImage(ThumbImage);
            }
        }

        [Editor(typeof(Blumind.Design.PictureEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public class PictureDesign
        {
            public PictureDesign()
            {
                LimitImageSize = true;
            }

            public PictureDesign(Image image)
            {
                Data = image;
            }

            public static PictureDesign FromClipboard()
            {
                if (Clipboard.ContainsImage())
                {
                    PictureDesign pd = new PictureDesign();
                    pd.Data = Clipboard.GetImage();
                    pd.SourceType = PictureSource.Memory;
                    pd.LimitImageSize = true;
                    pd.EmbedIn = true;

                    return pd;
                }

                return null;
            }

            public PictureSource SourceType { get; set; }

            public string Name { get; set; }

            public string Url { get; set; }

            public bool AddToLibrary { get; set; }

            public bool LimitImageSize { get; set; }

            public bool EmbedIn { get; set; }

            public Image Data { get; set; }

            public override string ToString()
            {
                //string pre;
                //switch (SourceType)
                //{
                //    case PictureSource.Url:
                //        pre = "URL:";
                //        break;
                //    case PictureSource.Library:
                //        pre = "LIB:";
                //        break;
                //    case PictureSource.File:
                //    default:
                //        pre = "FILE:";
                //        break;
                //}
                //return pre + Url;
                return Url;
            }

            public Image LoadImage()
            {
                switch (SourceType)
                {
                    case PictureSource.File:
                        return Picture.LoadImageFromFile(Url);
                    case PictureSource.Library:
                        return Picture.LoadImageFromLibrary(Url);
                    case PictureSource.Web:
                        return Picture.LoadImageFromWeb(Url, LimitImageSize);
                    case PictureSource.Memory:
                        return Data;
                    default:
                        return null;
                }
            }

            public PictureDesign Clone()
            {
                var pd = (PictureDesign)this.MemberwiseClone();
                return pd;
            }
        }
    }
}
