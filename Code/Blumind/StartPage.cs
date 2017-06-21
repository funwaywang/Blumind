using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Blumind.Configuration;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind
{
    class StartPage : BaseForm
    {
        RecentFilesView recentFilesView1;
        ButtonListBox buttonListBox1;
        SplitContainer splitContainer1;
        ButtonInfo btnNew, btnOpen, btnOptions;

        public StartPage()
        {
            Text = "Start";

            InitializeComponent();

            AfterInitialize();
        }

        void InitializeComponent()
        {
            recentFilesView1 = new RecentFilesView();
            buttonListBox1 = new ButtonListBox();
            splitContainer1 = new SplitContainer();
            btnNew = new ButtonInfo("New", Properties.Resources.new_24);
            btnOpen = new ButtonInfo("Open", Properties.Resources.open_24);
            btnOptions = new ButtonInfo("Options", Properties.Resources.preferences_24);

            buttonListBox1.SuspendLayout();
            splitContainer1.SuspendLayout();
            this.SuspendLayout();

            //
            recentFilesView1.Dock = DockStyle.Fill;
            recentFilesView1.Dimension = new System.Drawing.Size(4, 3);
            recentFilesView1.ItemClick += metroBox1_ItemClick;

            //
            btnNew.Click += btnNew_Click;

            //
            btnOpen.Click += btnOpen_Click;

            //
            btnOptions.Click += btnOptions_Click;

            //
            buttonListBox1.Dock = DockStyle.Fill;
            buttonListBox1.ButtonSize = 40;
            buttonListBox1.IconSize = new System.Drawing.Size(24, 24);
            buttonListBox1.Buttons.AddRange(new ButtonInfo[] { 
                btnNew,
                btnOpen,
                btnOptions
            });

            //
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Panel1.Controls.Add(buttonListBox1);
            splitContainer1.Panel2.Controls.Add(recentFilesView1);

            Controls.Add(splitContainer1);

#if DEBUG
            Size = new Size(800, 500);
#endif

            buttonListBox1.ResumeLayout(false);
            splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            //
            splitContainer1.SplitterDistance = 150;
        }

        void metroBox1_ItemClick(object sender, ThumbViewItemEventArgs e)
        {
            if (e.Item is FileThumbItem)
            {
                var item = (FileThumbItem)e.Item;
                if (Program.MainForm != null && !string.IsNullOrEmpty(item.Filename))
                {
                    Program.MainForm.OpenDocument(item.Filename);
                }
            }
        }

        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);

            this.Font = theme.DefaultFont;
            this.BackColor = theme.Colors. MediumLight;

            if (recentFilesView1 != null)
            {
                recentFilesView1.BackColor = theme.Colors.MediumLight;
                recentFilesView1.CellBackColor = Color.Transparent;// metroBox1.BackColor;
                recentFilesView1.CellForeColor = PaintHelper.FarthestColor(recentFilesView1.BackColor, theme.Colors.Dark, theme.Colors.Light);
                recentFilesView1.ActiveCellBackColor = Color.FromArgb(128, theme.Colors.Sharp);
                recentFilesView1.ActiveCellForeColor = theme.Colors.SharpText;
            }

            if (buttonListBox1 != null)
            {
                buttonListBox1.BackColor = PaintHelper.GetDarkColor(theme.Colors.MediumLight, 0.1f);
                buttonListBox1.ButtonBackColor = Color.Transparent;// theme.Colors.MediumDark;
                buttonListBox1.ButtonForeColor = PaintHelper.FarthestColor(buttonListBox1.BackColor, theme.Colors.Dark, theme.Colors.Light);
                buttonListBox1.ButtonHoverBackColor = theme.Colors.Sharp;
                buttonListBox1.ButtonHoverForeColor = theme.Colors.SharpText;
            }
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            if (buttonListBox1 != null)
            {
                btnNew.Text = Lang._("New");
                btnOpen.Text = Lang._("Open");
                btnOptions.Text = Lang._("Options");
            }
        }

        void btnOptions_Click(object sender, EventArgs e)
        {
            if (Program.MainForm != null)
            {
                Program.MainForm.ShowOptionsDialog();
            }
        }

        void btnOpen_Click(object sender, EventArgs e)
        {
            if (Program.MainForm != null)
            {
                Program.MainForm.OpenDocument();
            }
        }

        void btnNew_Click(object sender, EventArgs e)
        {
            if (Program.MainForm != null)
            {
                Program.MainForm.NewDocument();
            }
        }
    }
}
