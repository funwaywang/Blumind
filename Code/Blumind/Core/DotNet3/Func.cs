using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public delegate TResult Func<in T, out TResult>(T arg);

    public delegate TResult Func<in T1, in T2, out TResult>(T1 arg1, T2 arg2);
}
