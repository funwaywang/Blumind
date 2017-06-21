using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Configuration.Dialog
{
    partial class SettingDialog : StandardDialog
    {
        //TabControl tabControl1;
        GeneralPage settingControl1;
        Blumind.Controls.PushButton BtnRestoreDefault;

        public SettingDialog()
        {
            InitializeComponent();
            Icon = Properties.Resources.preferences_icon;
            ShowIcon = true;
            ShowApplyButton = true;
            AfterInitialize();

            MinimumSize = new Size(480, 300);
        }

        void InitializeComponent()
        {
            settingControl1 = new GeneralPage();
            settingControl1.TabIndex = 0;
            settingControl1.AutoScroll = true;
            settingControl1.BackColor = SystemColors.Window;
            settingControl1.ForeColor = SystemColors.WindowText;
            settingControl1.ModifiedChanged += settingControl1_ModifiedChanged;

            // BtnRestoreDefault
            BtnRestoreDefault = new PushButton();
            BtnRestoreDefault.TabIndex = 1;
            BtnRestoreDefault.Text = "&Restore Default";
            BtnRestoreDefault.Click += new EventHandler(this.BtnRestoreDefault_Click);

            Controls.Add(BtnRestoreDefault);
            //Controls.Add(tabControl1);
            Controls.Add(settingControl1);
            Size = new Size(500, 480);

            //SetMainControl(tabControl1);
            SetMainControl(settingControl1);
        }

        void settingControl1_ModifiedChanged(object sender, EventArgs e)
        {
            OKButton.Enabled = ApplyButton.Enabled = settingControl1.Modified;
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            Text = Lang._("Options");
            BtnRestoreDefault.Text = Lang._("Restore Default");
        }

        protected override bool OnOKButtonClick()
        {
            if (!OnApplyButtonClick())
                return false;

            return base.OnOKButtonClick();
        }

        protected override bool OnApplyButtonClick()
        {
            if (!settingControl1.Validate())
                return false;
            settingControl1.CommitSettings();

            try
            {
                Options.Current.Save();
                Options.Current.InvokeChanged();
            }
            catch (System.Exception ex)
            {
                this.ShowMessage(ex);
                return false;
            }

            return base.OnApplyButtonClick();
        }

        void BtnRestoreDefault_Click(object sender, System.EventArgs e)
        {
            //if (tabControl1.SelectedTab != null 
            //    && tabControl1.SelectedTab.Controls.Count > 0
            //    && tabControl1.SelectedTab.Controls[0] is SettingPage)
            //{
            //    if (MessageBox.Show(Lang._("Are you sure reset settings?"), 
            //        Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
            //        == System.Windows.Forms.DialogResult.Cancel)
            //        return;

            //    SettingPage page = (SettingPage)tabControl1.SelectedTab.Controls[0];
            //    page.ResetSettings();
            //}

            settingControl1.ResetSettings();
        }

        protected override void ResetButtonLocation()
        {
            base.ResetButtonLocation();

            if (BtnRestoreDefault != null)
            {
                BtnRestoreDefault.Size = new Size(125, ButtonHeight);
                BtnRestoreDefault.Location = new Point(8, OKButton.Top);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
            {
                var asm = AutoScaleMode;
                AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
                Font = UITheme.Default.DefaultFont;
                AutoScaleMode = asm;

                BtnRestoreDefault.Height = ButtonHeight;
                OKButton.Enabled = ApplyButton.Enabled = settingControl1.Modified;
            }
        }
    }
}
