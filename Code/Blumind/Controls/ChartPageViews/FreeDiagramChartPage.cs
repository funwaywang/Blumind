using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Controls.Charts;
using Blumind.Model.Documents;

namespace Blumind.ChartPageView
{
    partial class FreeDiagramChartPage : BaseChartPage, IThemableUI
    {
        FlowDiagramView chartBox1;

        public FreeDiagramChartPage()
        {
            InitializeComponent();

            InitializeControls();
        }

        public override ChartControl ChartBox
        {
            get
            {
                return chartBox1;
            }
        }

        [Browsable(false)]
        public FlowDiagram ChartDocument
        {
            get { return chartBox1.Diagram; }
        }

        void InitializeControls()
        {
            chartBox1 = new FlowDiagramView();
            chartBox1.Dock = DockStyle.Fill;
            chartBox1.ShowBorder = false;
            chartBox1.ShowNavigationMap = true;
            chartBox1.SelectionChanged += new System.EventHandler(this.chartBox1_SelectionChanged);
            chartBox1.ChartBackColorChanged += new System.EventHandler(this.chartBox1_ChartBackColorChanged);
            Controls.Add(chartBox1);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (!this.DesignMode)
                BackColor = PaintHelper.WithoutAlpha(chartBox1.ChartBackColor);
        }

        void chartBox1_SelectionChanged(object sender, EventArgs e)
        {
            var so = chartBox1.SelectedObjects;
            if (so == null || so.Length == 0)
                SelectedObjects = new object[] { ChartDocument };
            else
                SelectedObjects = so;
        }

        void chartBox1_ChartBackColorChanged(object sender, EventArgs e)
        {
            BackColor = PaintHelper.WithoutAlpha(chartBox1.ChartBackColor);
        }

        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);
        }
    }
}
