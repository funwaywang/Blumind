using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Globalization;

namespace Blumind.Dialogs
{
    partial class AboutDialogBox : BaseDialog
    {
        public AboutDialogBox()
        {
            InitializeComponent();

            Text = LanguageManage.GetText("About");
            ShowIcon = false;
            this.SetFontNotScale(SystemFonts.MessageBoxFont);
            LabProductName.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 36);
            LabVersion.Text = string.Format("{0}({1})", Lang._("Version"), ProductInfo.Version);
            
            TxbEmail.Text = ProductInfo.SupportEmail;

            LnkWebSite.Text = ProductInfo.WebSite;
            LnkWebSite.LinkUrl = ProductInfo.WebSite;

            LabAboutProduct.Text = string.Format("{0}\n{1}\n\n{2}\n{3}",
                ProductInfo.GetInformation(),
                ProductInfo.Copyright,
                "Some Icons are Copyright © Yusuke Kamiyamane.",
                string.Format("Pdfsharp {0}({1})", Lang._("Version"), PdfSharp.ProductVersionInfo.Version));

            AfterInitialize();
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            Text = Lang._("About"); //String.Format("{0} {1}", LanguageManage.GetText("About"), ProductInfo.Title);
            BtnClose.Text = Lang._("Close");
            LabHomePage.Text = Lang._("Home Page");
            BtnDonation.Text = Lang._("Donation");
            LabWechat.Text = Lang.GetText("WeChat");
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);

            BackColor = theme.Colors.Light;
            ForeColor = theme.Colors.Dark;

            if (TxbEmail != null)
                TxbEmail.BackColor = BackColor;

            if (TxbTwitter != null)
                TxbTwitter.BackColor = BackColor;

            if (TxbWechat != null)
                TxbWechat.BackColor = BackColor;
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            LocateButtonsLeft(new Control[] { BtnDonation });
        }

        private void BtnDonation_Click(object sender, EventArgs e)
        {
            Close();

            var dialog = new DonationDialog();
            dialog.ShowInTaskbar = false;
            dialog.ShowDialog(Owner);
        }
    }
}
