using Blumind.Controls;
using Blumind.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Blumind.Dialogs
{
    partial class DonationDialog : BaseDialog
    {
        Image imageDonation;

        public DonationDialog()
        {
            InitializeComponent();

            imageDonation = Properties.Resources.donation;
            ClientSize = imageDonation.Size;
            Text = Lang.GetText(Text);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);
            if(imageDonation != null)
            {
                var clientSize = ClientSize;
                PaintHelper.DrawImageInRange(e.Graphics, imageDonation, new Rectangle(0, 0, clientSize.Width, clientSize.Height));
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if(e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}
