using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Blumind.Model.Styles;

namespace Blumind.Model.Documents
{
    class FlowDiagram : ChartPage
    {
        public FlowDiagram()
        {
        }

        [Browsable(false)]
        public override ChartType Type
        {
            get { return ChartType.FlowDiagram; }
        }

        public override void ApplyTheme(ChartTheme chartTheme)
        {
        }
    }
}
