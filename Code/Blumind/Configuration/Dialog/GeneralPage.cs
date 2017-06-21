using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Configuration.Dialog
{
    partial class GeneralPage : SettingPage
    {
        bool CkbAssociate_ChangeBySelf = false;

        public GeneralPage()
        {
            InitializeComponent();

            InitializeControls();
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;

            tableLayoutPanel1.Dock = DockStyle.Top;
        }

        void InitializeControls()
        {
            CmbLanguages.Items.Clear();
            foreach (var lang in LanguageManage.Languages)
            {
                var li = new ListItem<string>(lang.ToString(), lang.ID);
                CmbLanguages.Items.Add(li);
            }

            //
            uiThemesDropDownList1.DataSource = UIColorThemeManage.All.ToArray(); 

            //
            BtnClearDefaultFont.Image = Properties.Resources.delete;
            BtnClearDefaultFont.Enabled = false;

            //
            BtnLanguagesDir.Enabled = !string.IsNullOrEmpty(ProgramEnvironment.LanguagesDirectory);
            BtnUIThemesDir.Enabled = !string.IsNullOrEmpty(ProgramEnvironment.UIThemesDirectory);
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            captionBar1.Text = Lang._("Appearance");
            captionBar2.Text = Lang._("General");
            captionBar3.Text = Lang._("Charts");
            captionBar4.Text = Lang._("Page Settings");
            labUILanguage.Text = Lang._("UI Language");
            LabUITheme.Text = Lang._("UI Theme");
            labSaveTabs.Text = Lang._("Save Tabs");
            CkbAssociate.Text = Lang._("Associate .bmd File");
            CkbSaveRecentFiles.Text = Lang._("Save Recent Files");

            CkbShowLineCap.Text = Lang._("Show Line Arrow");
            CkbShowRemarkIcon.Text = Lang._("Show Remark Icon");
            labDefaultFont.Text = Lang._("Default Font");

            CkbPrintDocumentTitle.Text = Lang._("Print Document Title");
        }

        protected override void PutSettings()
        {
            ListItem.Select(CmbLanguages, GetString(OptionNames.Localization.LanguageID), StringComparer.OrdinalIgnoreCase);
            uiThemesDropDownList1.SelectedTheme = GetString(OptionNames.Appearances.UIThemeName);
            CmbSaveTabs.Value = GetValue(OptionNames.Miscellaneous.SaveTabs, SaveTabsType.Ask);
            CkbAssociate.Checked = AssociationHelper.TestBmdAssociation();
            CkbSaveRecentFiles.Checked = GetValue(OptionNames.Miscellaneous.SaveRecentFiles, true);

            CkbShowLineCap.Checked = GetBool(OptionNames.Charts.ShowLineArrowCap);
            CkbShowRemarkIcon.Checked = GetBool(OptionNames.Charts.ShowRemarkIcon);
            BtnDefaultFont.SelectedFont = GetValue<Font>(OptionNames.Charts.DefaultFont);

            CkbPrintDocumentTitle.Checked = GetBool(OptionNames.PageSettigs.PrintDocumentTitle);
        }

        void fontButton1_Click(object sender, EventArgs e)
        {
            FontDialog dialog = new FontDialog();
            dialog.Font = BtnDefaultFont.SelectedFont;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                BtnDefaultFont.SelectedFont = dialog.Font;
            }
        }

        void BtnClearDefaultFont_Click(object sender, EventArgs e)
        {
            BtnDefaultFont.SelectedFont = null;
        }

        void BtnLanguagesDir_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ProgramEnvironment.LanguagesDirectory))
            {
                Helper.OpenFolder(ProgramEnvironment.LanguagesDirectory, true);
            }
        }

        void BtnUIThemesDir_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ProgramEnvironment.UIThemesDirectory))
            {
                Helper.OpenFolder(ProgramEnvironment.UIThemesDirectory, true);
            }
        }

        void CmbLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetValue(OptionNames.Localization.LanguageID, ListItem.GetSelectedValue<string>(CmbLanguages));
        }

        void uiThemesDropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetValue(OptionNames.Appearances.UIThemeName, uiThemesDropDownList1.SelectedTheme);
        }

        void CmbSaveTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetValue(OptionNames.Miscellaneous.SaveTabs, CmbSaveTabs.Value);
        }

        void CkbSaveRecentFiles_CheckedChanged(object sender, EventArgs e)
        {
            SetValue(OptionNames.Miscellaneous.SaveRecentFiles, CkbSaveRecentFiles.Checked);
        }

        void CkbAssociate_CheckedChanged(object sender, EventArgs e)
        {
            if (!CkbAssociate_ChangeBySelf && CkbAssociate.Checked != AssociationHelper.TestBmdAssociation())
            {
                CkbAssociate_ChangeBySelf = true;
                try
                {
                    AssociationHelper.RegisterDocType(CkbAssociate.Checked);
                }
                catch (System.Exception ex)
                {
                    this.ShowMessage(ex);
                }
                Thread.Sleep(500);
                CkbAssociate.Checked = AssociationHelper.TestBmdAssociation();
                CkbAssociate_ChangeBySelf = false;
            }
        }

        void fontButton1_SelectedFontChanged(object sender, EventArgs e)
        {
            SetValue(OptionNames.Charts.DefaultFont, BtnDefaultFont.SelectedFont);
            BtnClearDefaultFont.Enabled = BtnDefaultFont.SelectedFont != null;
        }

        void CkbShowLineCap_CheckedChanged(object sender, EventArgs e)
        {
            SetValue(OptionNames.Charts.ShowLineArrowCap, CkbShowLineCap.Checked);
        }

        void CkbShowRemarkIcon_CheckedChanged(object sender, EventArgs e)
        {
            SetValue(OptionNames.Charts.ShowRemarkIcon, CkbShowRemarkIcon.Checked);
        }

        void CkbPrintDocumentTitle_CheckedChanged(object sender, EventArgs e)
        {
            SetValue(OptionNames.PageSettigs.PrintDocumentTitle, CkbPrintDocumentTitle.Checked);
        }
    }
}
