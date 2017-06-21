using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using Blumind.Canvas.Pdf;
using Blumind.Configuration;
using Blumind.Controls;
using Blumind.Controls.MapViews;
using Blumind.Design;
using Blumind.Globalization;
using Blumind.Model;
using Blumind.Model.Documents;
using Blumind.Model.MindMaps;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Blumind.Core.Exports
{
    class PdfEngine : ChartsExportEngine
    {
        [Serializable]
        class PdfExportOptions
        {
            public PdfExportOptions()
            {
                Orientation = Model.PageOrientation.Landscape;
                WithBackground = true;
            }

            [DefaultValue(Model.PageOrientation.Landscape), LocalDisplayName("Page Orientation")]
            [Editor(typeof(EnumEditor<Model.PageOrientation>), typeof(UITypeEditor))]
            public PageOrientation Orientation { get; set; }

            [DefaultValue(true), LocalDisplayName("With Background")]
            [TypeConverter(typeof(Blumind.Design.BoolConverter))]
            public bool WithBackground { get; set; }
        }

        public PdfEngine()
        {
            ExportOptions = new PdfExportOptions();
        }

        PdfExportOptions ExportOptions { get; set; }

        public override string TypeMime
        {
            get { return "application/pdf"; }
        }

        protected override bool SupportMultiCharts
        {
            get
            {
                return true;
            }
        }

        protected override bool GetOptions()
        {
            ExportOptions.Orientation = Options.Current.GetValue("Export.PDF.PageOrientation", Model.PageOrientation.Landscape);
            ExportOptions.WithBackground = Options.Current.GetValue("Export.PDF.WithBackground", true);

            var dialog = new PropertyDialog(ExportOptions);
            dialog.Text = Lang._("Options");
            dialog.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Options.Current.SetValue("Export.PDF.PageOrientation", ExportOptions.Orientation);
                Options.Current.SetValue("Export.PDF.WithBackground", ExportOptions.WithBackground);

                return base.GetOptions();
            }
            else
                return false;
        }

        protected override bool ExportChartsToFile(Document document, IEnumerable<ChartPage> charts, string filename)
        {
            if (!GetOptions())
                return false;

            // Create a new PDF document
            PdfDocument doc = new PdfDocument();
            doc.Info.Title = document.Name;
            doc.Info.Author = document.Author;
            doc.Info.Elements.Add("/Company", new PdfStringObject(doc, document.Company));
            doc.Info.Elements.Add("/Version", new PdfStringObject(doc, document.Version));

            foreach (var chart in charts)
            {
                ExportPage(doc, chart);
            }

            doc.Save(filename);

            return true;
            //return base.ExportChartsToFile(document, charts, filename);
        }

        void ExportPage(PdfDocument doc, ChartPage chart)
        {
            // Create an empty page
            var page = doc.AddPage();

            if (ExportOptions.Orientation == PageOrientation.Landscape)
            {
                page.Orientation = PdfSharp.PageOrientation.Landscape;
            }

            // Get an XGraphics object for drawing
            var graphics = XGraphics.FromPdfPage(page);

            //
            if (ExportOptions.WithBackground && !chart.BackColor.IsEmpty)
            {
                graphics.Clear(chart.BackColor);
            }

            //
            var contentSize = chart.GetContentSize();

            var pageSize = new Size((int)page.Width.Point, (int)page.Height.Point);
            var zoom = PaintHelper.GetZoom(contentSize, pageSize);
            var zoomedSize = PaintHelper.Zoom(contentSize, zoom);
            if (zoomedSize.Width < pageSize.Width || zoomedSize.Height < pageSize.Height)
            {
                graphics.TranslateTransform(
                    Math.Max(0, (pageSize.Width - zoomedSize.Width) / 2),
                    Math.Max(0, (pageSize.Height - zoomedSize.Height) / 2));
            }
            graphics.ScaleTransform(zoom);

            //
            if (chart is MindMap)
            {
                var grf = new PdfGraphics(graphics);
                var args = new RenderArgs(grf, (MindMap)chart, grf.Font(ChartBox.DefaultChartFont));
                var renderer = new GeneralRender();
                renderer.Paint((MindMap)chart, args);
            }
        }
    }
}
