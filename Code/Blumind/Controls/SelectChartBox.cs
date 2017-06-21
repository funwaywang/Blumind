using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Blumind.Configuration;
using Blumind.Core;
using Blumind.Globalization;
using Blumind.Model.Documents;

namespace Blumind.Controls
{
    class SelectChartBox : BaseControl
    {
        [Serializable]
        enum ExportCharts
        {
            AllCharts,
            CurrentChart,
            SelectCharts,
        }

        Document _Document;
        ChartPage _CurrentChart;
        RadioButton radioButton1;
        RadioButton radioButton2;
        RadioButton radioButton3;
        ListBoxControl<ChartPage> listBoxEx2;
        TableLayoutPanel tableLayoutPanel1;
        ChartPage[] _SelectedCharts;
        ExportCharts _ChartsToExport;

        public event EventHandler SelectedChartsChanged;

        public SelectChartBox()
        {
            InitializeComponent();

            LanguageManage.CurrentChanged += LanguageManage_CurrentChanged;
        }

        public SelectChartBox(Document document)
            : this()
        {
            Document = document;
        }

        [Browsable(false), DefaultValue(null)]
        public Document Document
        {
            get { return _Document; }
            set
            {
                if (_Document != value)
                {
                    _Document = value;
                    OnDocumentChanged();
                }
            }
        }

        ChartPage CurrentChart
        {
            get { return _CurrentChart; }
            set
            {
                if (_CurrentChart != value)
                {
                    _CurrentChart = value;
                    OnCurrentChartChanged();
                }
            }
        }

        ExportCharts ChartsToExport
        {
            get { return _ChartsToExport; }
            set
            {
                if (_ChartsToExport != value)
                {
                    _ChartsToExport = value;
                    OnChartsToExportChanged();
                }
            }
        }

        [Browsable(false)]
        public ChartPage[] SelectedCharts
        {
            get { return _SelectedCharts; }
            set
            {
                if (_SelectedCharts != value)
                {
                    _SelectedCharts = value;
                    OnSelectedChartsChanged();
                }
            }
        }

        void OnSelectedChartsChanged()
        {
            if (SelectedChartsChanged != null)
                SelectedChartsChanged(this, EventArgs.Empty);
        }

        void InitializeComponent()
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            radioButton3 = new RadioButton();
            listBoxEx2 = new ListBoxControl<ChartPage>();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();

            // tableLayoutPanel1
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(this.radioButton1, 0, 0);
            tableLayoutPanel1.Controls.Add(this.radioButton2, 0, 1);
            tableLayoutPanel1.Controls.Add(this.radioButton3, 0, 2);
            tableLayoutPanel1.Controls.Add(this.listBoxEx2, 0, 3);
            tableLayoutPanel1.TabIndex = 0;

            // radioButton1
            radioButton1.Anchor = AnchorStyles.Left;
            radioButton1.AutoSize = true;
            radioButton1.TabIndex = 1;
            radioButton1.Text = "All Charts";
            radioButton1.CheckedChanged += radioButton1_CheckedChanged;

            // radioButton2
            radioButton2.Anchor = AnchorStyles.Left;
            radioButton2.AutoSize = true;
            radioButton2.TabIndex = 2;
            radioButton2.Text = "Current Chart";
            radioButton2.CheckedChanged += radioButton2_CheckedChanged;

            // radioButton3
            radioButton3.Anchor = AnchorStyles.Left;
            radioButton3.AutoSize = true;
            radioButton3.TabIndex = 3;
            radioButton3.Text = "Select Below:";
            radioButton3.CheckedChanged += radioButton3_CheckedChanged;

            // listBoxEx2
            listBoxEx2.Dock = DockStyle.Fill;
            listBoxEx2.TabIndex = 4;
            listBoxEx2.MultiSelect = true;
            listBoxEx2.ItemHeight = 28;
            listBoxEx2.SelectionChanged += listBoxEx2_SelectionChanged;

            // ExportDocumentDialog
            ClientSize = new Size(600, 400);
            Controls.Add(this.tableLayoutPanel1);
            Text = "Export Document";
            tableLayoutPanel1.ResumeLayout();
            ResumeLayout();
        }

        protected virtual void OnDocumentChanged()
        {
            listBoxEx2.Items.Clear();
            if (Document != null)
            {
                listBoxEx2.Items.AddRange(Document.Charts);

                if (Document.ActiveChart != null)
                    listBoxEx2.SelectedItem = Document.ActiveChart;

                CurrentChart = Document.ActiveChart;
            }
            else
            {
                CurrentChart = null;
            }

            var defaultCharts = Document.ActiveChart != null ? ExportCharts.CurrentChart : ExportCharts.AllCharts;
            var charts = defaultCharts;
            if (Options.Current.Contains(OptionNames.Miscellaneous.SelectChartsMethod))
            {
                charts = Options.Current.GetValue(OptionNames.Miscellaneous.SelectChartsMethod, defaultCharts);
                if (Document.ActiveChart == null && charts == ExportCharts.CurrentChart)
                    charts = defaultCharts;
            }
            this.ChartsToExport = charts;
            SelectedCharts = GetSelectedCharts();
        }

        void OnCurrentChartChanged()
        {
            if (CurrentChart != null)
                radioButton2.Text = string.Format("{0}: {1}", Lang._("Current Chart"), CurrentChart.Name);
            else
                radioButton2.Text = Lang._("Current Chart");
        }

        ChartPage[] GetSelectedCharts()
        {
            switch (ChartsToExport)
            {
                case ExportCharts.AllCharts:
                    return Document.Charts.ToArray();
                case ExportCharts.CurrentChart:
                    if (CurrentChart != null)
                        return new ChartPage[] { CurrentChart };
                    else
                        return null;
                case ExportCharts.SelectCharts:
                    return listBoxEx2.SelectedItems;
                default:
                    return null;
            }
        }

        void OnChartsToExportChanged()
        {
            switch (ChartsToExport)
            {
                case ExportCharts.AllCharts:
                    radioButton1.Checked = true;
                    radioButton2.Checked = false;
                    radioButton3.Checked = false;
                    break;
                case ExportCharts.CurrentChart:
                    radioButton1.Checked = false;
                    radioButton2.Checked = true;
                    radioButton3.Checked = false;
                    break;
                case ExportCharts.SelectCharts:
                    radioButton1.Checked = false;
                    radioButton2.Checked = false;
                    radioButton3.Checked = true;
                    break;
            }

            SelectedCharts = GetSelectedCharts();
            listBoxEx2.Enabled = ChartsToExport == ExportCharts.SelectCharts;
            Options.Current.SetValue(OptionNames.Miscellaneous.SelectChartsMethod, ChartsToExport);
        }

        void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            ChartsToExport = ExportCharts.AllCharts;
        }

        void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            ChartsToExport = ExportCharts.CurrentChart;
        }

        void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            ChartsToExport = ExportCharts.SelectCharts;
        }

        void LanguageManage_CurrentChanged(object sender, EventArgs e)
        {
            radioButton1.Text = Lang._("All Charts");
            radioButton3.Text = Lang.GetTextWithColon("Select Below");

            //radioButton2.Text = Lang._("Current Chart");
            OnCurrentChartChanged();
        }

        void listBoxEx2_SelectionChanged(object sender, EventArgs e)
        {
            SelectedCharts = GetSelectedCharts();
        }

        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);

            if (listBoxEx2 != null)
            {
                listBoxEx2.SelectionBackColor = theme.Colors.Sharp;
                listBoxEx2.SelectionForeColor = theme.Colors.SharpText;
            }
        }
    }
}
