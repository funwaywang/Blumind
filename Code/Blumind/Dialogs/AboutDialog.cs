using System;
using System.Drawing;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Core;
using System.IO;
using Blumind.Model;
using Blumind.Globalization;

namespace Blumind
{
    partial class AboutDialog : BaseDialog
    {
        public AboutDialog()
        {
            InitializeComponent();

            Text = LanguageManage.GetText("About");// String.Format("{0} {1}", LanguageManage.GetText("About"), ProductInfo.Title);
            LabVersion.Text = ProductInfo.Version;
            labelCopyright.Text = ProductInfo.Copyright;
            //labelCompanyName.Text = ProductInfo.Company;
            LnkWebSite.Text = LnkWebSite.LinkUrl = ProductInfo.WebSite;
            LnkEmail.Text = ProductInfo.SupportEmail;
            LnkEmail.LinkUrl = string.Format("mailto:{0}", ProductInfo.SupportEmail);

            labelIcons.Text = "Some Icons are Copyright © Yusuke Kamiyamane.";
            labelIcons.LinkUrl = @"http://p.yusukekamiyamane.com/?client=blumind";

            ListThanks();

            AfterInitialize();
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            Text = Lang._("About"); //String.Format("{0} {1}", LanguageManage.GetText("About"), ProductInfo.Title);
            TpgInformation.Text = Lang._("Information");
            TpgThanks.Text = Lang._("Thanks");
            BtnOK.Text = Lang._("OK");
            labelVersion.Text = Lang._("Version");
            LabHomePage.Text = Lang._("Home Page");
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            Image image = Properties.Resources.logo_large;
            Size size = tableLayoutPanel1.ClientSize;
            e.Graphics.DrawImage(image,
                new Rectangle(size.Width - image.Width - 10, 10, image.Width, image.Height),
                0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
        }

        private void ListThanks()
        {
            listBoxEx1.Items.Clear();

            Thanks.ThankItem[] items = Thanks.GetList();
            foreach (Thanks.ThankItem item in items)
            {
                listBoxEx1.Items.Add(item.Name);
            }
        }
    }
}
