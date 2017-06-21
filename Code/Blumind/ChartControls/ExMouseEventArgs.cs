using System.Windows.Forms;

namespace Blumind.Controls
{
    public class ExMouseEventArgs : MouseEventArgs
    {
        private bool _Suppress;

        public ExMouseEventArgs(MouseEventArgs e)
            : base(e.Button, e.Clicks, e.X, e.Y, e.Delta)
        {
        }

        public bool Suppress
        {
            get { return _Suppress; }
            set { _Suppress = value; }
        }
    }
}
