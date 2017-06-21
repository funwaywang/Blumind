using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Blumind.Core;
using Blumind.ChartControls.FillTypes;

namespace Blumind.Design
{
    class FillTypeEditor : ListEditor<string>
    {
        protected override string[] GetStandardValues()
        {
            return FillType.GeneralFillTypes.Keys.ToArray();
        }
    }
}
