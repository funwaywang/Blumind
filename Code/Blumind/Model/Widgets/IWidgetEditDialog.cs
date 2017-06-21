using System;
using System.Windows.Forms;

namespace Blumind.Model.Widgets
{
    interface IWidgetEditDialog
    {
        DialogResult ShowDialog(IWin32Window owner);

        Widget Widget { get; set; }
    }
}
