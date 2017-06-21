using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace Blumind
{
    static class D
    {
        const string DEBUG = "DEBUG";

        [Conditional(DEBUG)]
        public static void Message(string message)
        {
            //MessageBox.Show(message, "Debug", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Console.WriteLine(message);
        }

        [Conditional(DEBUG)]
        public static void Message(string format, params object[] args)
        {
            Message(string.Format(format, args));
        }
    }
}
