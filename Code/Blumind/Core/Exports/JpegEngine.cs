using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Blumind.Configuration;
using Blumind.Controls;
using Blumind.Globalization;

namespace Blumind.Core.Exports
{
    class JpegEngine : ImageExportEngine
    {
        [Serializable]
        class JpegExportOptions
        {
            int _Quality = 80;

            public JpegExportOptions()
            {
            }

            [DefaultValue(80), LocalDisplayName("Quality")]
            [Editor(typeof(Blumind.Design.JpegQualityEditor), typeof(UITypeEditor))]
            public int Quality
            {
                get { return _Quality; }
                set 
                {
                    value = Math.Max(10, Math.Min(100, value));
                    _Quality = value; 
                }
            }
        }

        public JpegEngine()
        {
            ExportOptions = new JpegExportOptions();
        }

        JpegExportOptions ExportOptions { get; set; }

        public override string TypeMime
        {
            get { return "image/jpeg"; }
        }

        protected override bool GetOptions()
        {
            ExportOptions.Quality = Options.Current.GetValue("Export.Jpeg.Quality", 80);

            var dialog = new PropertyDialog(ExportOptions);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Options.Current.SetValue("Export.Jpeg.Quality", ExportOptions.Quality);
            }
            else
            {
                return false;
            }

            return base.GetOptions();

            /*JpegOptionsDialog dialog = new JpegOptionsDialog();
            dialog.ImageQuality = ST.GetInt(_Options.Current.Customizations["export_jpeg_quality"], 85);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Quality = dialog.ImageQuality;
                _Options.Current.Customizations["export_jpeg_quality"] = Quality.ToString();
                return base.GetOptions();
            }
            else
            {
                return false;
            }*/
        }

        protected override void SaveImage(Image image, string filename)
        {
            if (image == null)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("filename");

            ImageCodecInfo coder = GetEncoder(TypeMime);
            if (coder != null)
            {
                EncoderParameters ep = new EncoderParameters();
                ep.Param = new EncoderParameter[]{
                    new EncoderParameter(Encoder.Quality, (long)ExportOptions.Quality)
                };

                image.Save(filename, coder, ep);
            }
            else
            {
                image.Save(filename, ImageFormat.Jpeg);
            }
        }
    }
}
