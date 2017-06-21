using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Blumind.Configuration;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Globalization;
using Blumind.Model.Documents;

namespace Blumind.Dialogs
{
    class ExportDocumentDialog : StandardDialog
    {
        Document _Document;
        TableLayoutPanel tableLayoutPanel1;
        Label labExportType;
        Label labCharts;
        ListBoxControl<ToolStripMenuItem> listBoxEx1;
        DocumentTypeGroup[] _DocumentTypeGroups;
        SelectChartBox selectChartBox1;

        public ExportDocumentDialog()
        {
            InitializeComponent();

            AfterInitialize();
        }

        public ExportDocumentDialog(Document document)
            : this()
        {
            Document = document;
        }
        
        [Browsable(false), DefaultValue(null)]
        public Document Document
        {
            get { return _Document; }
            private set
            {
                if (_Document != value)
                {
                    _Document = value;
                    OnDocumentChanged();
                }
            }
        }

        DocumentTypeGroup[] DocumentTypeGroups
        {
            get { return _DocumentTypeGroups; }
            set
            {
                if (_DocumentTypeGroups != value)
                {
                    _DocumentTypeGroups = value;
                    OnDocumentTypeGroupsChanged();
                }
            }
        }

        [Browsable(false)]
        public DocumentType DocumentType { get; private set; }

        [Browsable(false)]
        public ChartPage[] SelectedCharts
        {
            get
            {
                return selectChartBox1.SelectedCharts;
            }
        }

        void InitializeComponent()
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            labExportType = new Label();
            labCharts = new Label();
            listBoxEx1 = new ListBoxControl<ToolStripMenuItem>();
            selectChartBox1 = new SelectChartBox();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();

            // tableLayoutPanel1
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(labExportType, 0, 0);
            tableLayoutPanel1.Controls.Add(this.listBoxEx1, 0, 1);
            tableLayoutPanel1.Controls.Add(labCharts, 1, 0);
            tableLayoutPanel1.Controls.Add(this.selectChartBox1, 1, 1);
            tableLayoutPanel1.TabIndex = 0;

            // labExportType
            labExportType.Anchor = AnchorStyles.Left;
            labExportType.Text = "Export Type";
            labExportType.AutoSize = true;
            labExportType.Padding = new Padding(0, 2, 0, 2);

            // labCharts
            labCharts.Anchor = AnchorStyles.Left;
            labCharts.Text = "Charts";
            labCharts.AutoSize = true;
            labCharts.Padding = new Padding(0, 2, 0, 2);

            //listBoxEx1
            listBoxEx1.Dock = DockStyle.Fill;
            listBoxEx1.TabIndex = 0;
            listBoxEx1.ItemIconGetter = (item => item.Image);
            listBoxEx1.ItemToolTipTextGetter = (item => item.ToolTipText);
            listBoxEx1.ItemHeight = 28;
            listBoxEx1.SelectionChanged += listBoxEx1_SelectionChanged;

            //selectChartBox1
            selectChartBox1.Dock = DockStyle.Fill;
            
            // ExportDocumentDialog
            ClientSize = new Size(600, 400);
            Controls.Add(this.tableLayoutPanel1);
            Text = "Export Document";
            tableLayoutPanel1.ResumeLayout();
            ResumeLayout();
        }

        protected virtual void OnDocumentChanged()
        {
            if (Document != null)
            {
                DocumentTypeGroups = Document.GetExportDocumentTypes();
            }
            else
            {
                DocumentTypeGroups = null;
            }

            selectChartBox1.Document = Document;
        }

        void OnDocumentTypeGroupsChanged()
        {
            listBoxEx1.Items.Clear();
            if (DocumentTypeGroups.IsNullOrEmpty())
                return;

            var types = (from g in DocumentTypeGroups
                         from dt in g.Types
                         orderby dt.Name
                         select dt).ToArray();
            if (!types.IsNullOrEmpty())
            {
                listBoxEx1.SuspendLayout();
                foreach (var dt in types)
                {
                    var miExport = new ToolStripMenuItem();
                    miExport.Text = dt.Name;
                    if (!string.IsNullOrEmpty(dt.Description))
                        miExport.ToolTipText = Lang._(dt.Description);
                    miExport.Image = dt.Icon;// IconExtractor.ExtractLargeIconByExtension(dt.DefaultExtension);
                    miExport.Tag = dt;
                    listBoxEx1.Items.Add(miExport);
                }

                // select default
                string selName = null;
                if (Options.Current.Contains(OptionNames.Miscellaneous.ExportDocumentType))
                    selName = Options.Current.GetString(OptionNames.Miscellaneous.ExportDocumentType);
                DocumentType selDt = null;
                if (!string.IsNullOrEmpty(selName))
                    selDt = types.Find(t => StringComparer.OrdinalIgnoreCase.Equals(t.Name, selName));
                if (selDt == null)
                    selDt = types.First();
                listBoxEx1.SelectedItem = listBoxEx1.Items.Find(it => it.Tag == selDt);

                listBoxEx1.ResumeLayout(false);
            }
        }

        void listBoxEx1_SelectionChanged(object sender, EventArgs e)
        {
            if (listBoxEx1.SelectedItem != null)
            {
                var tn = ((DocumentType)listBoxEx1.SelectedItem.Tag).Name;
                Options.Current.SetValue(OptionNames.Miscellaneous.ExportDocumentType, tn);
            }
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (tableLayoutPanel1 != null)
            {
                tableLayoutPanel1.Bounds = this.ControlsRectangle;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            OnDocumentTypeGroupsChanged();
        }

        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);

            if (tableLayoutPanel1 != null)
            {
                tableLayoutPanel1.Font = theme.DefaultFont;
            }

            if (listBoxEx1 != null)
            {
                listBoxEx1.SelectionBackColor = theme.Colors.Sharp;
                listBoxEx1.SelectionForeColor = theme.Colors.SharpText;
            }
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            this.Text = Lang._("Export Document");
            labExportType.Text = Lang._("Export Type");
            labCharts.Text = Lang._("Charts");
        }

        protected override bool ValidateInput()
        {
            if (listBoxEx1.SelectedItem != null)
            {
                DocumentType = (DocumentType)listBoxEx1.SelectedItem.Tag;
            }
            else
            {
                this.ShowMessage("Please select export document type", MessageBoxIcon.Error);
                return false;
            }

            if (SelectedCharts.IsNullOrEmpty())
            {
                this.ShowMessage("No chart to export", MessageBoxIcon.Error);
                return false;
            }

            return base.ValidateInput();
        }
    }
}
