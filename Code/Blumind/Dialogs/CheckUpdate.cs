using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Blumind.Controls;
using System.Text.RegularExpressions;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Dialogs
{
    partial class CheckUpdate : BaseDialog
    {
        class VersionInfo
        {
            public string VersionString;
            public Version Version;
            public string DownloadUrl;
            public string Message;
        }

        Thread CheckThread;
        VersionInfo NewVersionInfo;
        const string LastVersionUrl = "http://www.blumind.org/products/blumind/last_version.txt";
        const string ChangesUrl = "http://www.blumind.org/products/blumind/changes.txt";

        private delegate void CheckEndCallBack(bool haveNew, VersionInfo version);

        public CheckUpdate()
        {
            InitializeComponent();
            ShowInTaskbar = true;
            Icon = Properties.Resources.check_update1;
            ShowIcon = true;

            //
            BtnDownload.Visible = false;
            TxbVersions.Visible = false;
            ClientSize = new Size(ClientSize.Width, LabMessage.Bottom + 10 + BtnCancel.Height + 10);

            this.SetFontNotScale(SystemFonts.MessageBoxFont);

            //
            AfterInitialize();
        }

        private Version CurrentVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            CheckThread = new Thread(new ThreadStart(CheckUpdateProc));
            CheckThread.Start();
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            Text = Lang._("Check Update");
            BtnCancel.Text = Lang._("Cancel");
            LabMessage.Text = Lang.GetTextWithEllipsis("Checking");
        }

        private void OnCheckEnd(bool haveNew, VersionInfo vif)
        {
            if (haveNew)
            {
                LabMessage.Text = string.Format(Lang._("There is a new version available"), vif.Version);

                TxbVersions.Text = vif.Message;
                TxbVersions.Visible = true;
                TxbVersions.Bounds = new Rectangle(LabMessage.Left, LabMessage.Bottom + 10, BtnCancel.Right - LabMessage.Left, 300);

                BtnDownload.Text = string.Format("{0} {1}...", Lang._("Download"), vif.Version);
                BtnDownload.Visible = true;

                //
                ClientSize = new Size(ClientSize.Width, TxbVersions.Bottom + 10 + BtnCancel.Height + 10);
            }
            else
            {
                if(vif == null)
                    LabMessage.Text = Lang._("Unable to connect to the remote server, please visit our website.");
                else
                    LabMessage.Text = Lang._("Your current version is the latest");
            }
            NewVersionInfo = vif;
            BtnCancel.Text = Lang._("Close");
            MoveToCenterScreen();
        }

        private string DownloadUrl(string url)
        {
            WebClient wc = new WebClient();
            byte[] buffer = wc.DownloadData(url);
            return Encoding.UTF8.GetString(buffer);
        }

        private void CheckUpdateProc()
        {
            string version = null;

            try
            {
                version = DownloadUrl(LastVersionUrl);
            }
            catch(System.Exception ex)
            {
                Helper.WriteLog(ex);
            }

            if (string.IsNullOrEmpty(version))
            {
                Invoke(new CheckEndCallBack(OnCheckEnd), new object[] { false, null });
                return;
            }

            StringReader sr = new StringReader(version);
            VersionInfo vif = new VersionInfo();
            vif.VersionString = sr.ReadLine();
            vif.DownloadUrl = sr.ReadLine();
            sr.Close();

            if (!Regex.IsMatch(vif.VersionString, @"(\d+\.\d+\.\d+\.\d+)"))
            {
                Invoke(new CheckEndCallBack(OnCheckEnd), new object[] { false, null });
                return;
            }

            vif.Version = new Version(vif.VersionString);

            if (vif.Version <= CurrentVersion)
            {
                Invoke(new CheckEndCallBack(OnCheckEnd), new object[] { false, vif });
            }
            else
            {
                try
                {
                    vif.Message = DownloadUrl(ChangesUrl);
                }
                catch(System.Exception ex)
                {
                    Helper.WriteLog(ex);
                }
                Invoke(new CheckEndCallBack(OnCheckEnd), new object[] { true, vif });
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (CheckThread != null && CheckThread.IsAlive)
            {
                try
                {
                    CheckThread.Abort();
                }
                catch(System.Exception ex)
                {
                    Helper.WriteLog(ex);
                }
            }

            DialogResult = DialogResult.Cancel;
            Close();
        }

        void BtnDownload_Click(object sender, EventArgs e)
        {
            if (NewVersionInfo != null)
            {
                if (string.IsNullOrEmpty(NewVersionInfo.DownloadUrl))
                {
                    this.ShowMessage("Invalid URL", MessageBoxIcon.Error);
                    return;
                }

                Helper.OpenUrl(NewVersionInfo.DownloadUrl);
            }

            Close();
        }
    }
}
