using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using Blumind.Core;
using Blumind.Design;
using Blumind.Globalization;

namespace Blumind.Configuration
{
    [Serializable]
    [Editor(typeof(EnumEditor<SaveTabsType>), typeof(UITypeEditor))]
    [TypeConverter(typeof(EnumConverter<SaveTabsType>))]
    enum SaveTabsType
    {
        [LanguageID("Confirm")]
        Ask,
        [LanguageID("Yes")]
        Yes,
        [LanguageID("NO")]
        No,
    }
}
