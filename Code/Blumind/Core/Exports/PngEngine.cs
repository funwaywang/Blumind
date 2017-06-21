using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Blumind.Configuration;
using Blumind.Controls;
using Blumind.Globalization;

namespace Blumind.Core.Exports
{
    class PngEngine : ImageExportEngine
    {
        [Serializable]
        class PngExportOptions
        {
            [DefaultValue(true), LocalDisplayName("Transparent Background")]
            [TypeConverter(typeof(Blumind.Design.BoolConverter))]
            public bool TransparentBackground { get; set; }
        }

        //bool _TransparentBackground;

        public PngEngine()
        {
            ExportOptions = new PngExportOptions();
        }

        public override string TypeMime
        {
            get { return "image/png"; }
        }

        PngExportOptions ExportOptions { get; set; }

        protected override bool TransparentBackground
        {
            get
            {
                return ExportOptions.TransparentBackground;// _TransparentBackground;
            }
        }

        protected override bool GetOptions()
        {
            ExportOptions.TransparentBackground = Options.Current.GetValue("Export.PNG.TransparentBackground", true);

            var dialog = new PropertyDialog(ExportOptions);
            dialog.Text = Lang._("Options");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Options.Current.SetValue("Export.PNG.TransparentBackground", ExportOptions.TransparentBackground);
            }
            else
            {
                return false;
            }

            return base.GetOptions();

            //var dialog = new PngOptionsDialog();
            //dialog.TransparentBackground = ST.GetBoolDefault(_Options.Current.Customizations["export_png_transparent_background"]);
            //if (dialog.ShowDialog() == DialogResult.OK)
            //{
            //    _TransparentBackground = dialog.TransparentBackground;
            //    _Options.Current.Customizations["export_png_transparent_background"] = TransparentBackground.ToString();
            //    return base.GetOptions();
            //}
            //else
            //{
            //    return false;
            //}
        }

        protected override void SaveImage(Image image, string filename)
        {
            if (image == null)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("filename");

            image.Save(filename, ImageFormat.Png);
        }
    }
}
