using System;
using System.Collections.Generic;
using System.Text;
using Blumind.Core;

namespace Blumind.Model
{
    interface IChartControl
    {
        void OnChartObjectPropertyChanged(ChartObject chartObject, PropertyChangedEventArgs e);
    }
}
