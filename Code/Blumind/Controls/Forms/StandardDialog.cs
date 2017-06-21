using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Controls
{
    class StandardDialog : BaseDialog
    {
        Button BtnOK;
        Button BtnCancel;
        Button BtnApply;
        Button[] Buttons;
        bool _ShowApplyButton;

        public event CancelEventHandler Apply;

        public StandardDialog()
        {
            AutoClose = true;

            BtnOK = new Button();
            BtnOK.Text = "&OK";
            BtnOK.TabIndex = 10000;
            BtnOK.UseVisualStyleBackColor = true;
            BtnOK.Click += new EventHandler(BtnOK_Click);

            BtnCancel = new Button();
            BtnCancel.Text = "&Cancel";
            BtnCancel.TabIndex = 10001;
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += new EventHandler(BtnCancel_Click);

            BtnApply = new Button();
            BtnApply.Text = "&Apply";
            BtnApply.TabIndex = 10002;
            BtnApply.Visible = ShowApplyButton;
            BtnApply.UseVisualStyleBackColor = true;
            BtnApply.Click += new EventHandler(BtnApply_Click);

            Controls.Add(BtnOK);
            Controls.Add(BtnCancel);
            Controls.Add(BtnApply);
            //AcceptButton = BtnOK;
            CancelButton = BtnCancel;

            Buttons = new Button[] { BtnOK, BtnCancel, BtnApply };
            foreach (var btn in Buttons)
            {
                btn.Font = UITheme.Default.DefaultFont;
                btn.Height = ButtonHeight;
            }
            ResetButtonLocation();
        }

        [DefaultValue(false)]
        public bool ShowApplyButton
        {
            get { return _ShowApplyButton; }
            set
            {
                if (_ShowApplyButton != value)
                {
                    _ShowApplyButton = value;
                    OnShowApplyButtonChanged();
                }
            }
        }

        protected Button OKButton
        {
            get { return BtnOK; }
        }

        protected Button ApplyButton
        {
            get { return BtnApply; }
        }

        protected override bool ShowButtonArea
        {
            get
            {
                return true;
            }
        }

        [DefaultValue(true)]
        public bool AutoClose { get; set; }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            ResetButtonLocation();
        }

        void OnShowApplyButtonChanged()
        {
            BtnApply.Visible = ShowApplyButton;
            BtnApply.Text = Lang.GetTextWithAccelerator("Apply", 'A');
            ResetButtonLocation();
        }

        protected virtual void ResetButtonLocation()
        {
            Point pt = new Point(ClientSize.Width - 12, ClientSize.Height - 12);
            for (int i = Buttons.Length - 1; i >= 0; i--)
            {
                Button btn = Buttons[i];
                if (btn.Visible)
                {
                    btn.Location = new Point(pt.X - btn.Width, pt.Y - btn.Height);
                    pt.X -= btn.Width + 8;
                }
            }
        }

        void BtnOK_Click(object sender, EventArgs e)
        {
            OnOKButtonClick();
        }

        void BtnCancel_Click(object sender, EventArgs e)
        {
            OnCancelButtonClick();
        }

        void BtnApply_Click(object sender, EventArgs e)
        {
            OnApplyButtonClick();
        }

        protected override void OnModifiedChanged()
        {
            base.OnModifiedChanged();

            BtnApply.Enabled = Modified;
        }

        protected virtual bool OnOKButtonClick()
        {
            if (ValidateInput() && AutoClose)
            {
                DialogResult = DialogResult.OK;
                Close();
                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual bool OnCancelButtonClick()
        {
            if (AutoClose)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
            return true;
        }

        protected virtual bool OnApplyButtonClick()
        {
            if (Apply != null)
            {
                CancelEventArgs ce = new CancelEventArgs();
                Apply(this, ce);
                if (ce.Cancel)
                    return false;
            }
            Modified = false;
            return true;
        }

        protected virtual bool ValidateInput()
        {
            return true;
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            if (Created)
            {
                ResetButtonLocation();
            }

            base.OnLayout(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            BtnApply.Visible = ShowApplyButton;
            ResetButtonLocation();
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            BtnOK.Text = Lang._("OK");
            BtnCancel.Text = Lang._("Cancel");

            if (BtnApply != null)
                BtnApply.Text = Lang._("Apply");
        }

        public void SetMainControl(Control control)
        {
            if (control == null)
                throw new ArgumentNullException();

            control.Bounds = ControlsRectangle;
            control.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            if (!Controls.Contains(control))
                Controls.Add(control);
        }
    }
}
