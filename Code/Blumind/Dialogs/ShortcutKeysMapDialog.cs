using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Dialogs
{
    partial class ShortcutKeysMapDialog : StandardDialog
    {
        private Hashtable Changes = new Hashtable(StringComparer.OrdinalIgnoreCase);

        public ShortcutKeysMapDialog()
        {
            InitializeComponent();

            AfterInitialize();
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            RefreshList();
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            Text = Lang._("Accelerator Keys Table");
            ColumnOperation.Text = Lang._("Operation");
            ColumnKey.Text = Lang._("Keys");
            ColumnDescription.Text = Lang._("Notes");

            MenuModify.Text = Lang.GetTextWithEllipsis("Modify");
            MenuClear.Text = Lang._("Clear");
            MenuReset.Text = Lang._("Reset");
            selectAllToolStripMenuItem.Text = Lang._("Select All");
            invertSelectionToolStripMenuItem.Text = Lang._("Invert Selection");
            BtnReset.Text = Lang._("Reset All");
        }

        private void RefreshList()
        {
            listView1.Items.Clear();

            bool hasChanged = false;
            foreach (ShortcutKeyGroup group in ShortcutKeyGroup.AllGroups)
            {
                var lvg = new ListViewGroup(Lang._(group.Name));
                listView1.Groups.Add(lvg);
                foreach (var key in group.Keys)
                {
                    ListViewItem lvi = new ListViewItem(Lang._(key.Name), lvg);
                    lvi.SubItems.Add(key.KeysToString());
                    if(!string.IsNullOrEmpty(key.Description))
                        lvi.SubItems.Add(Lang._(key.Description));
                    lvi.Tag = key;
                    listView1.Items.Add(lvi);

                    hasChanged |= key.Changed;

#if DEBUG
                    // test invalid shortcuts
                    if (key.Keys != Keys.None && !KeyMap.IsValidKey(key.Keys))
                        lvi.ForeColor = System.Drawing.Color.Red;
#endif

                    if (key.Image != null)
                    {
                        listView1.SmallImageList.Images.Add(key.Image);
                        lvi.ImageIndex = listView1.SmallImageList.Images.Count - 1;
                    }
                }
            }

            Changes.Clear();
            BtnReset.Enabled = hasChanged;
            OKButton.Enabled = false;
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool hasSelected = listView1.SelectedItems.Count > 0;
            MenuModify.Enabled = hasSelected;
            MenuClear.Enabled = hasSelected;
            MenuReset.Enabled = hasSelected;
        }

        private void MenuModify_Click(object sender, System.EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ShortcutKey key = (ShortcutKey)listView1.SelectedItems[0].Tag;
                ModifyShortcutsDialog dialog = new ModifyShortcutsDialog();
                dialog.ShortcutKeys = key;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    TryChange(key, dialog.NewValue);
                    listView1.SelectedItems[0].SubItems[1].Text = ST.ToString(dialog.NewValue);
                }
            }
        }

        private void MenuClear_Click(object sender, System.EventArgs e)
        {
            foreach (ListViewItem lvi in listView1.SelectedItems)
            {
                if (lvi.Tag is ShortcutKey)
                {
                    ShortcutKey key = (ShortcutKey)lvi.Tag;
                    TryChange(key, Keys.None);
                    lvi.SubItems[1].Text = string.Empty;
                }
            }
        }

        private void MenuReset_Click(object sender, System.EventArgs e)
        {
            foreach (ListViewItem lvi in listView1.SelectedItems)
            {
                if (lvi.Tag is ShortcutKey)
                {
                    ShortcutKey key = (ShortcutKey)lvi.Tag;
                    TryChange(key, key.DefaultKeys);
                    lvi.SubItems[1].Text = ST.ToString(key.DefaultKeys);
                }
            }
        }

        private void TryChange(ShortcutKey key, Keys keys)
        {
            if (key.Keys == keys)
            {
                if (Changes.ContainsKey(key.Name))
                    Changes.Remove(key.Name);
            }
            else
            {
                Changes[key.Name] = keys;
            }

            BtnReset.Enabled = Changes.Count > 0;
            OKButton.Enabled = Changes.Count > 0;
        }

        protected override bool OnOKButtonClick()
        {
            if (Changes.Count > 0)
            {
                KeyMap.ApplyChanges(Changes);
            }

            return base.OnOKButtonClick();
        }

        void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in listView1.Items)
                if (!lvi.Selected)
                    lvi.Selected = true;
        }

        void invertSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in listView1.Items)
                lvi.Selected = !lvi.Selected;
        }

        void BtnReset_Click(object sender, EventArgs e)
        {
            if (this.ShowMessage("Are you sure reset all shortcuts?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Cancel)
                return;

            KeyMap.ResetAll();
            RefreshList();
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if(listView1 != null)
                listView1.Bounds = this.ControlsRectangle;

            LocateButtonsLeft(new Button[] { BtnReset });
        }

        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);

            this.SetFontNotScale(theme.DefaultFont);
        }
    }
}
