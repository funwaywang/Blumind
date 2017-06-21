using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Controls.MapViews;
using Blumind.Dialogs;
using Blumind.Globalization;
using Blumind.Model.Documents;

namespace Blumind.Core.Exports
{
    abstract class ChartsExportEngine
    {
        public abstract string TypeMime { get; }

        public ChartsExportEngine()
        {
        }

        public ChartPage ChartPage { get; private set; }

        protected virtual bool TransparentBackground
        {
            get { return false; }
        }

        /// <summary>
        /// support multi-charts in one document
        /// </summary>
        protected virtual bool SupportMultiCharts
        {
            get { return false; }
        }

        public DocumentType DocumentType
        {
            get
            {
                return DocumentType.GetDocumentTypes().Find(dt => StringComparer.OrdinalIgnoreCase.Equals(dt.TypeMime, this.TypeMime));
            }
        }

        protected bool OptionsInitalizated { get; private set; }

        /*public virtual bool Export(MindMapView view, string filename)
        {
            if (view == null)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("filename");

            ChartPage = view.Map;

            if (!GetOptions())
                return false;

            Image image = view.GenerateImage(RenderMode.Export, TransparentBackground);
            SaveImage(image, filename);

            return true;
        }*/

        protected virtual bool GetOptions()
        {
            OptionsInitalizated = true;
            return true;
        }

        public static ChartsExportEngine[] GetEngines()
        {
            return new ChartsExportEngine[] { 
                new PngEngine(),
                new JpegEngine(),
                new BitmapEngine(),
                new GifEngine(),
                new TiffEngine(),
                new SvgEngine(),
                new TxtEngine(),
                new CsvEngine(),
                new FreeMindEngine(),
                new PdfEngine(),
            };
        }

        public static ChartsExportEngine GetEngine(string typeMime)
        {
            ChartsExportEngine[] engines = GetEngines();
            foreach (ChartsExportEngine engine in engines)
            {
                if (StringComparer.OrdinalIgnoreCase.Equals(engine.TypeMime, typeMime))
                {
                    return engine;
                }
            }

            return null;
        }

        protected ImageCodecInfo GetEncoder(string mimeType)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo cif in encoders)
            {
                if (cif.MimeType == mimeType)
                    return cif;
            }

            return null;
        }

        public virtual void Export(Document document, IEnumerable<ChartPage> charts)
        {
            if (document == null || charts == null)
                throw new ArgumentNullException();

            if (charts.IsEmpty())
                return;

            var documentType = DocumentType;
            if (documentType == null)
                return;

            if (charts.Count() > 1 && !SupportMultiCharts)
            {
                var dialog = new FolderBrowserDialog();
                dialog.Description = Lang._("Select a folder to export charts");
                if (dialog.ShowDialog(Program.MainForm) == DialogResult.OK)
                {
                    ExportChartsToFolder(document, charts, dialog.SelectedPath);
                }
            }
            else
            {
                var dialog = new SaveFileDialog();
                dialog.Filter = documentType.FileDialogFilter;
                dialog.DefaultExt = documentType.DefaultExtension;
                dialog.Title = Lang._("Export");
                dialog.FileName = ST.EscapeFileName(document.Name);
                if (dialog.ShowDialog(Program.MainForm) == DialogResult.OK)
                {
                    if (ExportChartsToFile(document, charts, dialog.FileName))
                    {
                        var fld = new FileLocationDialog(dialog.FileName, dialog.FileName);
                        fld.Text = Lang._("Export Success");
                        fld.ShowDialog(Program.MainForm);
                    }
                }
            }

        }

        protected virtual bool ExportChartsToFile(Document document, IEnumerable<ChartPage> charts, string filename)
        {
            if (charts.Count() == 1)
            {
                return ExportChartToFile(document, charts.First(), filename);
            }

            return false;
        }

        protected virtual bool ExportChartsToFolder(Document document, IEnumerable<ChartPage> charts, string directory)
        {
            if (document == null || charts.IsNullOrEmpty())
                return false;

            if (!OptionsInitalizated && !GetOptions())
                return false;

            var ext = DocumentType.DefaultExtension;
            var files = new List<string>();
            bool success = false;
            foreach (var c in charts)
            {
                string filename = ST.EscapeFileName(c.Name);
                int index = 1;
                while (files.Contains(filename))
                {
                    filename = ST.EscapeFileName(c.Name) + index.ToString();
                    index++;
                }

                filename = Path.Combine(directory, filename + ext);
                success |= ExportChartToFile(document, c, filename);
            }

            return success;
        }

        protected virtual bool ExportChartToFile(Document document, ChartPage chart, string filename)
        {
            if (chart == null)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("filename");

            return true;
        }
    }
}
