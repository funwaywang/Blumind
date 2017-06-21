using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Model
{
    [Serializable]
    enum PageOrientation
    {
        [LanguageID("Portrait")]
        Portrait = 0,

        [LanguageID("Landscape")]
        Landscape = 1,
    }
}
