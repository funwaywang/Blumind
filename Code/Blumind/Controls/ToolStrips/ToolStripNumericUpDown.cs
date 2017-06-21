using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Blumind.Controls
{
    [DefaultEvent("ValueChanged")]
    public class ToolStripNumericUpDown : ToolStripControlHost
    {
        public event System.EventHandler ValueChanged;

        public ToolStripNumericUpDown()
            : base(new NumericUpDown())
        {
        }

        [Browsable(false)]
        public NumericUpDown NumericUpDownControl
        {
            get{return this.Control as NumericUpDown;}
        }

        [DefaultValue(100)]
        public int Maximum
        {
            get { return (int)this.NumericUpDownControl.Maximum; }
            set { this.NumericUpDownControl.Maximum = value; }
        }

        [DefaultValue(0)]
        public int Minimum
        {
            get { return (int)this.NumericUpDownControl.Minimum; }
            set { this.NumericUpDownControl.Minimum = value; }
        }

        [DefaultValue(1)]
        public int Increment
        {
            get { return (int)this.NumericUpDownControl.Increment; }
            set { this.NumericUpDownControl.Increment = value; }
        }

        [DefaultValue(0)]
        public int Value
        {
            get { return (int)this.NumericUpDownControl.Value; }
            set { this.NumericUpDownControl.Value = value; }
        }

        [DefaultValue(false)]
        public bool Hexadecimal 
        {
            get { return this.NumericUpDownControl.Hexadecimal; }
            set { this.NumericUpDownControl.Hexadecimal = value; }
        }

        [DefaultValue(false)]
        public bool ThousandsSeparator 
        {
            get { return this.NumericUpDownControl.ThousandsSeparator; }
            set { this.NumericUpDownControl.ThousandsSeparator = value; }
        }

        [DefaultValue(HorizontalAlignment.Left)]
        public HorizontalAlignment UpDownControlTextAlign
        {
            get{ return this.NumericUpDownControl.TextAlign;}
            set{this.NumericUpDownControl.TextAlign = value;}
        }

        [DefaultValue(0)]
        public int DecimalPlaces
        {
            get { return this.NumericUpDownControl.DecimalPlaces; }
            set { this.NumericUpDownControl.DecimalPlaces = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        protected override void OnSubscribeControlEvents(Control control)
        {
            NumericUpDown ctl = (NumericUpDown)control;
            ctl.ValueChanged += new EventHandler(Control_ValueChanged);

            base.OnSubscribeControlEvents(control);
        }

        private void Control_ValueChanged(object sender, EventArgs e)
        {
            if (this.ValueChanged != null)
                this.ValueChanged(this, EventArgs.Empty);
        }
    }
}
