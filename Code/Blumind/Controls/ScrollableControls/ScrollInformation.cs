using System.Windows.Forms;

namespace Blumind.Controls
{
    public class ScrollInformation
    {
        private int _Maximum = 0;
        private int _Minimum = 0;
        private int _LargeChange = 0;
        private int _SmallChange = 0;
        private int _Value = 0;
        private bool _Enabled = false;
        private bool _Visible = false;

        public ScrollInformation()
        {
        }

        public int Maximum
        {
            get { return _Maximum; }
            set { _Maximum = value; }
        }

        public int Minimum
        {
            get { return _Minimum; }
            set { _Minimum = value; }
        }

        public int LargeChange
        {
            get { return _LargeChange; }
            set { _LargeChange = value; }
        }

        public int SmallChange
        {
            get { return _SmallChange; }
            set { _SmallChange = value; }
        }

        public int Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public bool Enabled
        {
            get { return _Enabled; }
            set { _Enabled = value; }
        }

        public bool Visible
        {
            get { return _Visible; }
            set { _Visible = value; }
        }

        public void GetInformation(ScrollBar bar)
        {
            this.Maximum = bar.Maximum;
            this.Minimum = bar.Minimum;
            this.LargeChange = bar.LargeChange;
            this.SmallChange = bar.SmallChange;
            this.Value = bar.Value;
            this.Enabled = bar.Enabled;
            this.Visible = bar.Visible;
        }
    }
}
