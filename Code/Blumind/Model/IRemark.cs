using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blumind.Model
{
    interface IRemark
    {
        event EventHandler RemarkChanged;

        string Remark { get; set; }
    }
}
