using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Printing;
using System.Text;
using Blumind.Configuration;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Model.Documents
{
    partial class Document : ModifyObject
    {
        string _Author;
        string _Company;
        string _Version;
        string _Description;
        ChartPage _ActiveChart;

        public event EventHandler ChartActived;

        public Document()
        {
            Charts = new XList<ChartPage>();
            Charts.ItemAdded += new XListEventHandler<ChartPage>(Charts_ItemAdded);
            Charts.ItemRemoved += new XListEventHandler<ChartPage>(Charts_ItemRemoved);

            Attributes = new Dictionary<string, object>();

            Version = "3.0";
        }

        public const string Extension = ".bmd";
        public static readonly Version DocumentVersion = new Version(3, 0, 0, 0);
        public static readonly Version DV_3 = new Version(3, 0, 0, 0);

        [Browsable(false)]
        public XList<ChartPage> Charts { get; private set; }

        [Browsable(false)]
        public ChartPage ActiveChart
        {
            get { return _ActiveChart; }
            set
            {
                if (_ActiveChart != value)
                {
                    var old = _ActiveChart;
                    _ActiveChart = value;
                    OnActiveChartChanged(old);
                }
            }
        }

        [Browsable(false)]
        public int ActiveChartIndex
        {
            get
            {
                if (ActiveChart == null)
                    return -1;
                else
                    return Charts.IndexOf(ActiveChart);
            }

            set
            {
                if (value < 0 || value >= Charts.Count)
                    return;
                else
                    ActiveChart = Charts[value];
            }
        }

        [Browsable(false)]
        public Dictionary<string, object> Attributes { get; private set; }

        [DefaultValue(null), LocalDisplayName("Author"), LocalCategory("Description")]
        public string Author
        {
            get { return _Author; }
            set
            {
                if (_Author != value)
                {
                    _Author = value;
                    this.Modified = true;
                }
            }
        }

        [DefaultValue(null), LocalDisplayName("Company"), LocalCategory("Description")]
        public string Company
        {
            get { return _Company; }
            set
            {
                if (_Company != value)
                {
                    _Company = value;
                    this.Modified = true;
                }
            }
        }

        [DefaultValue("3.0"), LocalDisplayName("Version"), LocalCategory("Description"), ReadOnly(true)]
        public string Version
        {
            get { return _Version; }
            set
            {
                if (_Version != value)
                {
                    _Version = value;
                    this.Modified = true;
                }
            }
        }

        [DefaultValue(null), LocalDisplayName("Notes"), LocalCategory("Description")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor))]
        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                    this.Modified = true;
                }
            }
        }

        void OnActiveChartChanged(ChartPage old)
        {
            if (ChartActived != null)
            {
                ChartActived(this, EventArgs.Empty);
            }
        }

        void Charts_ItemAdded(object sender, XListEventArgs<ChartPage> e)
        {
            this.Modified = true;
            if (e.Item != null && e.Item.Document != this)
                e.Item.Document = this;
        }

        void Charts_ItemRemoved(object sender, XListEventArgs<ChartPage> e)
        {
            this.Modified = true;
            if (e.Item != null && e.Item.Document == this)
                e.Item.Document = null;
        }

        public DocumentTypeGroup[] GetExportDocumentTypes()
        {
            return new DocumentTypeGroup[] {
                new DocumentTypeGroup("PDF", new DocumentType[]{
                    DocumentType.Pdf}),
                new DocumentTypeGroup("Image", new DocumentType[]{
                    DocumentType.Png, 
                    DocumentType.Jpeg, 
                    DocumentType.Bmp, 
                    DocumentType.Gif, 
                    DocumentType.Tiff,}),
                new DocumentTypeGroup("XML", new DocumentType[]{
                    DocumentType.Svg,
                    DocumentType.FreeMind}),
                new DocumentTypeGroup("Text", new DocumentType[]{
                    DocumentType.Txt,
                    DocumentType.Csv}),
                };
        }

        public Image CreateThumbImage()
        {
            if (ActiveChart == null)
                return null;

            return ActiveChart.CreateThumbImage(new Size(210, 150));
        }

        public PrintDocument Print()
        {
            return Print(this.Charts);
        }

        public PrintDocument Print(IEnumerable<ChartPage> charts)
        {
            PrintDocument doc = new PrintDocument();

            var ps = doc.DefaultPageSettings;
            ps.Landscape = Options.Current.GetBool(Blumind.Configuration.OptionNames.PageSettigs.Landscape, true);
            //ps.Margins = new Margins(Margins.Left, Margins.Right, Margins.Top, Margins.Bottom);

            //_Options.Current.PageSettigs.Assign(doc.DefaultPageSettings);
            //doc.DefaultPageSettings.Landscape = ContentSize.Width > ContentSize.Height;
            doc.DocumentName = Name;

            var dp = new DocumentPrint(this, charts);
            doc.PrintPage += dp.PrintPage;

            return doc;
        }

        #region Object ID
        int ObjectSeed = 1;
        Dictionary<string, object> RegistedObjects = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public string GetNextObjectID()
        {
            var id = (ObjectSeed++).ToString();
            while (RegistedObjects.ContainsKey(id))
            {
                id = (ObjectSeed++).ToString();
            }

            return id;
        }

        public void RegisterObjectID(string id, object obj)
        {
            if (id == null)
                return;
            RegistedObjects[id] = obj;
        }

        #endregion
    }
}
