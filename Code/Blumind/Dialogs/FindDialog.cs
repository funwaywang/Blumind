using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Controls.MapViews;
using Blumind.Core;
using Blumind.Core.Documents;
using Blumind.Globalization;
using Blumind.Model;
using Blumind.Model.MindMaps;

namespace Blumind.Dialogs
{
    partial class FindDialog : BaseDialog
    {
        public enum FindDialogMode
        {
            Find,
            Replace,
        }

        FindDialogMode _Mode = FindDialogMode.Find;
        float ControlSpace = 10;
        MindMapFinder Finder = new MindMapFinder();
        DocumentManageForm MainForm;
        string LastFindWhat;
        
        public FindDialog()
        {
            InitializeComponent();

            OnModeChanged();
            OpenOptions = false;
            CancelButton = BtnClose;

            CkbCaseSensitive.Checked = FindOptions.Default.CaseSensitive;
            CkbWholeWordOnly.Checked = FindOptions.Default.WholeWordOnly;
            CkbRegularExpression.Checked = FindOptions.Default.RegularExpression;
            //CkbForwardSearch.Checked = FindOptions.Default.Direction == FindDirection.Forward;
            CkbWithHiddenItems.Checked = FindOptions.Default.WithHiddenItems;

            AfterInitialize();
        }

        public FindDialog(DocumentManageForm mainForm)
            :this()
        {
            MainForm = mainForm;
        }

        [DefaultValue(FindDialogMode.Find)]
        public FindDialogMode Mode
        {
            get { return _Mode; }
            set 
            {
                if (_Mode != value)
                {
                    _Mode = value;
                    OnModeChanged();
                }
            }
        }

        [Browsable(false), DefaultValue(null)]
        public ChartControl ChartPageView
        {
            get 
            {
                if (MainForm != null && MainForm.SelectedForm is DocumentForm)
                    return ((DocumentForm)MainForm.SelectedForm).ActiveChartBox;
                else
                    return null; 
            }
        }

        protected override bool ShowButtonArea
        {
            get
            {
                return true;
            }
        }

        [DefaultValue(true)]
        public bool OpenOptions
        {
            get { return !FbOptions.Folded; }
            set { FbOptions.Folded = !value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LayoutControls();

            FillHistory(CmbFindWhat, FindTextHistory);
            FillHistory(CmbReplaceWith, ReplaceTextHistory);

            CmbFindWhat.SelectAll();
            ResetFocus();
        }

        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);

            if (this.myToolStrip1 != null)
                this.myToolStrip1.Renderer = theme.ToolStripRenderer;
            this.SetFontNotScale(theme.DefaultFont);
        }

        void OnModeChanged()
        {
            Text = Mode == FindDialogMode.Find ? Lang._("Find") : Lang._("Replace");

            TsbFind.Checked = Mode == FindDialogMode.Find;
            TsbReplace.Checked = Mode == FindDialogMode.Replace;

            LabReplaceWith.Visible = Mode == FindDialogMode.Replace;
            CmbReplaceWith.Visible = Mode == FindDialogMode.Replace;
            BtnReplace.Visible = Mode == FindDialogMode.Replace;

            Finder.Reset();
            LayoutControls();
            //LayoutButtons();
            //PerformLayout();
        }

        void OnMapViewChanged()
        {
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Hide();
                e.Cancel = true;
            }
            base.OnFormClosing(e);
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            LocateButtonsRight(new Button[] { BtnFind, BtnReplace, BtnClose });

            //LayoutControls();
        }

        protected override void OnCurrentLanguageChanged()
        {
            Text = Mode == FindDialogMode.Find ? Lang._("Find") : Lang._("Replace");

            TsbFind.Text = Lang._("Find");
            TsbReplace.Text = Lang._("Replace");
            LabFindWhat.Text = Lang._("Find What");
            LabReplaceWith.Text = Lang._("Replace With");
            FbOptions.Text = Lang._("Find Options");
            CkbCaseSensitive.Text = Lang._("Case Sensitive");
            CkbWholeWordOnly.Text = Lang._("Whole Word Only");
            CkbRegularExpression.Text = Lang._("Regular Expression");
            //CkbForwardSearch.Text = Lang._("Forward Search");
            CkbWithHiddenItems.Text = Lang._("With Hidden Items");

            BtnFind.Text = Lang._("Find Next");
            BtnReplace.Text = Lang._("Replace");
            BtnClose.Text = Lang._("Close");

            base.OnCurrentLanguageChanged();
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);

            if ((specified & BoundsSpecified.Height) == BoundsSpecified.Height)
            {
                ControlSpace *= factor.Height;
            }
        }

        void FillHistory(ComboBox list, List<string> history)
        {
            list.Items.Clear();
            for (int i = history.Count - 1; i >= 0; i--)
            {
                list.Items.Add(history[i]);
            }

            if (list.Items.Count > 0)
            {
                list.SelectedIndex = 0;
            }
        }

        void AddHistoryItem(ComboBox list, string item)
        {
            if (list.Items.Count == 0)
            {
                list.Items.Add(item);
            }
            else if ((string)list.Items[0] != item)
            {
                if (list.Items.Contains(item))
                    list.Items.Remove(item);
                list.Items.Insert(0, item);
            }
        }

        void LayoutControls()
        {
            float y = myToolStrip1.Bottom;
            y += ControlSpace;
            
            int h = Math.Max(LabFindWhat.Height, CmbFindWhat.Height);
            LabFindWhat.Top = (int)Math.Ceiling(y + (h - LabFindWhat.Height) / 2);
            CmbFindWhat.Top = (int)Math.Ceiling(y + (h - CmbFindWhat.Height) / 2);
            y += h + ControlSpace;

            if (Mode == FindDialogMode.Replace)
            {
                h = Math.Max(LabReplaceWith.Height, CmbReplaceWith.Height);
                LabReplaceWith.Top = (int)Math.Ceiling(y + (h - LabReplaceWith.Height) / 2);
                CmbReplaceWith.Top = (int)Math.Ceiling(y + (h - CmbReplaceWith.Height) / 2);
                y += h + ControlSpace;
            }

            FbOptions.Top = (int)Math.Ceiling(y);
            y += FbOptions.Height + ControlSpace;

            h = (int)Math.Ceiling(y + ControlSpace) + ButtonAreaHeight;
            MinimumSize = new Size(200, h + SystemInformation.ToolWindowCaptionHeight + SystemInformation.SizingBorderWidth);
            ClientSize = new Size(ClientSize.Width, h);
        }

        //void LayoutButtons()
        //{
        //    // buttons
        //    int x = ClientSize.Width - (int)Math.Ceiling(ControlSpace);
        //    int y = ClientSize.Height - (int)Math.Ceiling(ControlSpace) - BtnClose.Height;
        //    Button[] buttons = new Button[] { BtnFind, BtnReplace, BtnClose };
        //    for (int i = buttons.Length - 1; i >= 0; i--)
        //    {
        //        if (!buttons[i].Visible)
        //            continue;

        //        x -= buttons[i].Width;
        //        buttons[i].Location = new Point(x, y);
        //        x -= (int)Math.Ceiling(ControlSpace);
        //    }
        //}

        void TsbFind_Click(object sender, System.EventArgs e)
        {
            Mode = FindDialogMode.Find;
        }

        void TsbReplace_Click(object sender, System.EventArgs e)
        {
            Mode = FindDialogMode.Replace;
        }

        void FbOptions_FoldedChanged(object sender, System.EventArgs e)
        {
            LayoutControls();
        }

        void BtnClose_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        void BtnFind_Click(object sender, EventArgs e)
        {
            if (DoFind() == null)
            {
                string msg = string.Format(Lang._("Can't find \"{0}\""), CmbFindWhat.Text);
                this.ShowMessage(msg, MessageBoxIcon.Information);
                Finder.Reset();
            }
        }

        void BtnReplace_Click(object sender, EventArgs e)
        {
            if (!DoReplace())
            {
                string msg = Lang.Format("Can't find \"{0}\"", CmbFindWhat.Text);
                this.ShowMessage(msg, MessageBoxIcon.Information);
            }
        }

        void CkbCaseSensitive_CheckedChanged(object sender, EventArgs e)
        {
            FindOptions.Default.CaseSensitive = CkbCaseSensitive.Checked;
        }

        void CkbWholeWordOnly_CheckedChanged(object sender, EventArgs e)
        {
            FindOptions.Default.WholeWordOnly = CkbWholeWordOnly.Checked;
        }

        void CkbRegularExpression_CheckedChanged(object sender, EventArgs e)
        {
            FindOptions.Default.RegularExpression = CkbRegularExpression.Checked;
        }

        //private void CkbForwardSearch_CheckedChanged(object sender, EventArgs e)
        //{
        //    FindOptions.Default.Direction = CkbForwardSearch.Checked ? FindDirection.Forward : FindDirection.Backward;
        //}

        void CkbWithHiddenItems_CheckedChanged(object sender, EventArgs e)
        {
            FindOptions.Default.WithHiddenItems = CkbWithHiddenItems.Checked;
        }

        void CmbFindWhat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (BtnFind.Enabled)
                {
                    BtnFind_Click(this, EventArgs.Empty);
                    e.SuppressKeyPress = true;
                }
            }
        }

        void CmbReplaceWith_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (BtnReplace.Enabled)
                {
                    BtnReplace_Click(this, EventArgs.Empty);
                    e.SuppressKeyPress = true;
                }
            }
        }

        bool DoReplace()
        {
            string findWhat = CmbFindWhat.Text;
            string replaceWith = CmbReplaceWith.Text;

            if (string.IsNullOrEmpty(findWhat))
                return false;

            if (ChartPageView.SelectedObject != null)
            {
                var topic = ChartPageView.SelectedObject;
                string newText = Finder.Replace(topic.Text, findWhat, replaceWith);
                if (topic.Text != newText)
                {
                    ChartPageView.ChangeObjectText(topic, newText);
                }
            }

            // find next
            return DoFind() != null;
        }

        ChartObject DoFind()
        {
            string findWhat = CmbFindWhat.Text;
            if (!string.IsNullOrEmpty(findWhat))
            {
                if (LastFindWhat != findWhat)
                    Finder.Reset();
                LastFindWhat = findWhat;
                
                ChartObject chartObject = ChartPageView.FindNext(Finder, findWhat);
                if(chartObject != null)
                {
                    //MapView.SelectTopic(topic, true);
                    ChartPageView.Select(chartObject);
                    PopFindTextHistory(findWhat);
                    AddHistoryItem(CmbFindWhat, findWhat);

                    return chartObject;
                }
            }

            return null;
        }

        public void ResetFocus()
        {
            if (CmbFindWhat.CanFocus)
                CmbFindWhat.Focus();
            else
                ActiveControl = CmbFindWhat;
        }
        
        #region history
        static List<string> FindTextHistory = new List<string>();
        static List<string> ReplaceTextHistory = new List<string>();

        static void PopFindTextHistory(string text)
        {
            if (FindTextHistory.Contains(text))
                FindTextHistory.Remove(text);
            FindTextHistory.Add(text);
        }

        static void PopReplaceTextHistory(string text)
        {
            if (ReplaceTextHistory.Contains(text))
                ReplaceTextHistory.Remove(text);
            ReplaceTextHistory.Add(text);
        }
        #endregion
    }
}
