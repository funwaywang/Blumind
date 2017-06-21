using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Globalization;
using Blumind.Model.Documents;

namespace Blumind.Dialogs
{
    class SelectChartDialog : StandardDialog
    {
        SelectChartBox selectChartBox1;
        Document _Document;

        public SelectChartDialog()
        {
            InitializeComponents();
        }

        public SelectChartDialog(Document document)
            : this()
        {
            Document = document;
        }

        [Browsable(false)]
        public ChartPage[] SelectedCharts
        {
            get { return selectChartBox1.SelectedCharts; }
        }

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

        void InitializeComponents()
        {
            selectChartBox1 = new SelectChartBox();
            Controls.Add(selectChartBox1);

            Text = Lang._("Select Charts");
            Name = "Select Charts";
            Size = new System.Drawing.Size(300, 400);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        }

        void OnDocumentChanged()
        {
            selectChartBox1.Document = this.Document;
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (selectChartBox1 != null)
            {
                selectChartBox1.Bounds = ControlsRectangle;
            }
        }
    }
}
