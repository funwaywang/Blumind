using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Controls
{
    partial class ExceptionDialog : BaseDialog
    {
        Exception _Exception;
        bool _DetailsVisible;

        public ExceptionDialog()
        {
            InitializeComponent();
            MinimumSize = new System.Drawing.Size(250, 120);
            Icon = Blumind.Properties.Resources.exclamation;
            var am = AutoScaleMode;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            Font = SystemFonts.MessageBoxFont;
            AutoScaleMode = am;
            ShowInTaskbar = true;
            StartPosition = FormStartPosition.CenterScreen;
            Padding = Padding.Empty;
            KeyPreview = true;

            pictureBox1.Image = Blumind.Properties.Resources.cross_large;
            label1.Padding = new System.Windows.Forms.Padding(4);
            LabExceptionMessage.Padding = new System.Windows.Forms.Padding(4);
            LabExceptionMessage.Font = new System.Drawing.Font(LabExceptionMessage.Font, FontStyle.Bold);
            LabExceptionMessage.Text = null;
            BtnClose.Height += 2;
            BtnToggleDetails.Height += 2;

            //
            DetailsVisible = false;
        }

        public ExceptionDialog(Exception ex)
            : this()
        {
            Exception = ex;
        }

        [DefaultValue(null)]
        public Exception Exception
        {
            get { return _Exception; }
            set 
            {
                if (_Exception != value)
                {
                    _Exception = value;
                    OnExceptionChanged();
                }
            }
        }

        bool DetailsVisible
        {
            get { return _DetailsVisible; }
            set 
            {
                if (_DetailsVisible != value)
                {
                    _DetailsVisible = value;
                    OnDetailsVisibleChanged();
                }
            }
        }

        int ExceptionControlHeight { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
            {
                OnDetailsVisibleChanged();
            }
        }

        void OnDetailsVisibleChanged()
        {
            if (DetailsVisible)
            {
                if (!exceptionControl1.Visible)
                {
                    exceptionControl1.Show();
                    //tableLayoutPanel1.Height += ExceptionControlHeight;
                    Height += ExceptionControlHeight;
                }

                BtnToggleDetails.Text = Lang._("Hide Details");
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            }
            else
            {
                if (exceptionControl1.Visible)
                {
                    ExceptionControlHeight = exceptionControl1.Height;
                    exceptionControl1.Hide();
                    //tableLayoutPanel1.Height -= ExceptionControlHeight;
                    Height -= ExceptionControlHeight;
                }

                BtnToggleDetails.Text = Lang._("Show Details");
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            }
        }

        void OnExceptionChanged()
        {
            exceptionControl1.Exception = Exception;

            if (Exception != null)
            {
                LabExceptionMessage.Text = Exception.Message;
            }
            else
            {
                LabExceptionMessage.Text = null;
            }
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            ResetControls();
        }

        void ResetControls()
        {
            if (BtnClose == null || BtnToggleDetails == null)
                return;

            //BtnClose.Location = new Point(
            //    ClientSize.Width - BtnClose.Width - 8,
            //    ClientSize.Height - BtnClose.Height - 8);

            //BtnToggleDetails.Location = new Point(
            //    tableLayoutPanel1.Left,
            //    BtnClose.Top);

            tableLayoutPanel1.Width = ClientSize.Width - tableLayoutPanel1.Left - 8;
            tableLayoutPanel1.Height = panel1.Top  //BtnClose.Top
                - 8 - tableLayoutPanel1.Top;
        }

        void BtnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public static DialogResult Show(Exception ex)
        {
            ExceptionDialog dialog = new ExceptionDialog(ex);
            return dialog.ShowDialog();
        }

        public static DialogResult Show(IWin32Window owner, Exception ex)
        {
            ExceptionDialog dialog = new ExceptionDialog(ex);
            return dialog.ShowDialog(owner);
        }

        void BtnToggleDetails_Click(object sender, EventArgs e)
        {
            DetailsVisible = !DetailsVisible;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Control && e.KeyCode == Keys.C)
            {
                if (!DetailsVisible && Exception != null)
                {
                    try
                    {
                        Clipboard.SetText(Exception.Message);
                        e.SuppressKeyPress = true;
                    }
                    catch
                    {
                    }
                }
            }
        }

        void panel1_Paint(object sender, PaintEventArgs e)
        {
            var color = UITheme.Default.Colors.MediumDark;
            e.Graphics.Clear(color);
            //var brush = new HatchBrush(HatchStyle.Percent20,
            //    color,
            //    PaintHelper.GetLightColor(color));
            //e.Graphics.FillRectangle(brush, panel1.ClientRectangle);
        }
    }
}
