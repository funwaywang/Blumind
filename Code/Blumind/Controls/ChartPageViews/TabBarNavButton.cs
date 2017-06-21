using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Blumind.Controls;

namespace Blumind.ChartPageView
{
    class TabBarNavButton : TabBarButton
    {
        public TabBarNavButton(string text, Image icon)
            : base(text, icon)
        {
            CustomSize = 13;
            CustomSpace = 1;
        }

        //protected override void OnTextChanged()
        //{
        //    base.OnTextChanged();

        //    ToolTipText = Text;
        //}
    }
}
