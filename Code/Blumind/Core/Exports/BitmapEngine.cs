using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Blumind.Core.Exports
{
    class BitmapEngine : ImageExportEngine
    {
        public override string TypeMime
        {
            get { return "image/bmp"; }
        }

        protected override void SaveImage(Image image, string filename)
        {
            if (image == null)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("filename");

            image.Save(filename, ImageFormat.Bmp);
        }
    }
}
