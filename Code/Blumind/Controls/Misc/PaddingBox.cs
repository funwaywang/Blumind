using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Controls
{
    class PaddingBox : BaseControl
    {
        TableLayoutPanel layoutPanel;
        NumericUpDown NudLeft, NudTop, NudRight, NudBottom, NudAll;
        Label LabLeft, LabTop, LabRight, LabBottom, LabAll;
        Padding _Value = Padding.Empty;
        bool ChangingBySelf = false;

        public PaddingBox()
        {
            InitializeComponent();
            OnValueChanged();
            LanguageManage.CurrentChanged += new EventHandler(LanguageManage_CurrentChanged);
            LanguageManage_CurrentChanged(this, EventArgs.Empty);
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(120, 120);
            }
        }

        [DefaultValue(typeof(Padding), "0,0,0,0")]
        public Padding Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    OnValueChanged();
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(false)]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                base.AutoSize = value;
            }
        }

        private void InitializeComponent()
        {
            layoutPanel = new TableLayoutPanel();
            NudLeft = new NumericUpDown();
            NudTop = new NumericUpDown();
            NudRight = new NumericUpDown();
            NudBottom = new NumericUpDown();
            NudAll = new NumericUpDown();
            LabLeft = new Label();
            LabTop = new Label();
            LabRight = new Label();
            LabBottom = new Label();
            LabAll = new Label();
            this.SuspendLayout();
            layoutPanel.SuspendLayout();

            //
            layoutPanel.Controls.Add(LabAll, 0, 0);
            layoutPanel.Controls.Add(NudAll, 1, 0);
            layoutPanel.Controls.Add(LabLeft, 0, 1);
            layoutPanel.Controls.Add(NudLeft, 1, 1);
            layoutPanel.Controls.Add(LabTop, 0, 2);
            layoutPanel.Controls.Add(NudTop, 1, 2);
            layoutPanel.Controls.Add(LabRight, 0, 3);
            layoutPanel.Controls.Add(NudRight, 1, 3);
            layoutPanel.Controls.Add(LabBottom, 0, 4);
            layoutPanel.Controls.Add(NudBottom, 1, 4);
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1.0f));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 0.2f));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 0.2f));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 0.2f));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 0.2f));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 0.2f));
            layoutPanel.Dock = DockStyle.Fill;

            //
            LabAll.Text = "All";
            LabAll.Anchor = AnchorStyles.Left;
            LabAll.AutoSize = true;

            //
            LabLeft.Text = "Left";
            LabLeft.Anchor = AnchorStyles.Left;
            LabLeft.AutoSize = true;

            //
            LabTop.Text = "Top";
            LabTop.Anchor = AnchorStyles.Left;
            LabTop.AutoSize = true;

            //
            LabRight.Text = "Right";
            LabRight.Anchor = AnchorStyles.Left;
            LabRight.AutoSize = true;

            //
            LabBottom.Text = "Bottom";
            LabBottom.Anchor = AnchorStyles.Left;
            LabBottom.AutoSize = true;

            //
            NudAll.Minimum = int.MinValue;
            NudAll.Maximum = int.MaxValue;
            NudAll.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            NudAll.ValueChanged += new EventHandler(NumberUpDown_ValueChanged);

            //
            NudLeft.Minimum = int.MinValue;
            NudLeft.Maximum = int.MaxValue;
            NudLeft.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            NudLeft.ValueChanged += new EventHandler(NumberUpDown_ValueChanged);

            //
            NudTop.Minimum = int.MinValue;
            NudTop.Maximum = int.MaxValue;
            NudTop.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            NudTop.ValueChanged += new EventHandler(NumberUpDown_ValueChanged);

            //
            NudRight.Minimum = int.MinValue;
            NudRight.Maximum = int.MaxValue;
            NudRight.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            NudRight.ValueChanged += new EventHandler(NumberUpDown_ValueChanged);

            //
            NudBottom.Minimum = int.MinValue;
            NudBottom.Maximum = int.MaxValue;
            NudBottom.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            NudBottom.ValueChanged += new EventHandler(NumberUpDown_ValueChanged);

            //
            this.Controls.Add(layoutPanel);
            layoutPanel.ResumeLayout();
            this.ResumeLayout();
        }

        protected virtual void OnValueChanged()
        {
            ChangingBySelf = true;
            NudLeft.Value = Value.Left;
            NudTop.Value = Value.Top;
            NudRight.Value = Value.Right;
            NudBottom.Value = Value.Bottom;
            NudAll.Value = Value.All;
            ChangingBySelf = false;
        }

        protected override void OnAutoSizeChanged(EventArgs e)
        {
            base.OnAutoSizeChanged(e);

            if (AutoSize)
            {
                ResetSize();
            }
        }

        private void NumberUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (ChangingBySelf)
                return;

            ChangingBySelf = true;
            if (sender == NudAll)
            {
                Value = new Padding((int)NudAll.Value);
            }
            else
            {
                Value = new Padding(
                    (int)NudLeft.Value,
                    (int)NudTop.Value,
                    (int)NudRight.Value,
                    (int)NudBottom.Value);
            }
            ChangingBySelf = false;
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (AutoSize)
            {
                ResetSize();
            }
            base.OnLayout(levent);
        }

        private void ResetSize()
        {
            int height = Math.Max(LabAll.Height, NudAll.Height) * 5
                + 4 * 4 // space
                + layoutPanel.Margin.Vertical
                + layoutPanel.Padding.Vertical;

            Label[] labels = new Label[] { LabAll, LabLeft, LabTop, LabRight, LabBottom };
            int width = 0;
            foreach (Label label in labels)
            {
                width = Math.Max(label.Width, width);
            }
            width += layoutPanel.Margin.Horizontal + layoutPanel.Padding.Horizontal + 4 + 50;

            //
            Size = new Size(
                width,
                Math.Max(Height, height));
        }

        private void LanguageManage_CurrentChanged(object sender, EventArgs e)
        {
            LabAll.Text = Lang._("All");
            LabLeft.Text = Lang._("Left");
            LabTop.Text = Lang._("Top");
            LabRight.Text = Lang._("Right");
            LabBottom.Text = Lang._("Bottom");
        }
    }
}
