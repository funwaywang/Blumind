using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using Blumind.Controls.MapViews;

namespace Blumind.Controls
{
    class SortBox : BaseControl
    {
        private ListBox listBox;
        private PushButton btnMoveUp;
        private PushButton btnMoveDown;
        private object[] _Items;

        public SortBox()
        {
            listBox = new ListBox2();
            listBox.SelectionMode = SelectionMode.MultiExtended;
            listBox.SelectedIndexChanged += new EventHandler(listBox_SelectedIndexChanged);

            btnMoveUp = new PushButton();
            btnMoveUp.Enabled = false;
            btnMoveUp.Image = Properties.Resources.up;
            btnMoveUp.Size = new Size(50, 30);
            btnMoveUp.Click += new EventHandler(BtnMoveUp_Click);

            btnMoveDown = new PushButton();
            btnMoveDown.Enabled = false;
            btnMoveDown.Image = Properties.Resources.down;
            btnMoveDown.Size = btnMoveUp.Size;
            btnMoveDown.Click += new EventHandler(BtnMoveDown_Click);

            Controls.AddRange(new Control[] { btnMoveUp, btnMoveDown, listBox });
        }

        public object[] Items
        {
            get { return _Items; }
            set 
            {
                if (_Items != value)
                {
                    _Items = value;
                    OnItemsChanged();
                }
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 100);
            }
        }

        /*private void ResetButtons()
        {
            if (listBox.SelectedIndices.Count == 0)
            {
                btnMoveUp.Enabled = false;
                btnMoveDown.Enabled = false;
            }
            else
            {
                int max_sel = -1;
                int min_sel = Items.Length;
                foreach (int index in listBox.SelectedIndices)
                {
                    max_sel = Math.Max(max_sel, index);
                    min_sel = Math.Min(min_sel, index);
                }

                btnMoveUp.Enabled = min_sel > 0;
                btnMoveDown.Enabled = max_sel < Items.Length - 1;

                // move buttons
                Rectangle rect = Rectangle.Union(listBox.GetItemRectangle(min_sel), listBox.GetItemRectangle(max_sel));
                rect.Y = Math.Max(0, rect.Y) + listBox.Top;
                rect.Height = Math.Min(listBox.Height - rect.Y, rect.Height);

                if (btnMoveUp.Enabled && btnMoveDown.Enabled)
                {
                    btnMoveUp.Location = new Point(ClientSize.Width - btnMoveUp.Width, rect.Y + rect.Height / 2 - btnMoveUp.Height);
                    btnMoveDown.Location = new Point(ClientSize.Width - btnMoveDown.Width, rect.Y + rect.Height / 2);
                }
                else if (btnMoveUp.Enabled)
                {
                    btnMoveUp.Location = new Point(ClientSize.Width - btnMoveUp.Width, Math.Min(ClientSize.Height - btnMoveDown.Height, rect.Y + (rect.Height - btnMoveUp.Height) / 2));
                }
                else if (btnMoveDown.Enabled)
                {
                    btnMoveDown.Location = new Point(ClientSize.Width - btnMoveDown.Width, Math.Max(0, rect.Y + (rect.Height - btnMoveDown.Height) / 2));
                }
            }

            if (!btnMoveUp.Enabled && !btnMoveDown.Enabled)
            {
                btnMoveUp.Visible = true;
                btnMoveDown.Visible = true;

                btnMoveUp.Location = new Point(ClientSize.Width - btnMoveUp.Width, ClientSize.Height / 2 - btnMoveUp.Height - 8);
                btnMoveDown.Location = new Point(ClientSize.Width - btnMoveDown.Width, ClientSize.Height / 2 + 8);
            }
            else
            {
                btnMoveUp.Visible = btnMoveUp.Enabled;
                btnMoveDown.Visible = btnMoveDown.Enabled;
            }
        }*/

        #region move methods

        public void MoveUp()
        {
            MoveUp(1);
        }

        public void MoveDown()
        {
            MoveDown(1);
        }

        public void MoveUp(int step)
        {
            if (step < 0)
            {
                MoveDown(-step);
            }
            else
            {
                MoveItems(GetSelectedIndices(), -step);
            }
        }

        public void MoveDown(int step)
        {
            if (step < 0)
            {
                MoveUp(-step);
            }
            else
            {
                MoveItems(GetSelectedIndices(), step);
            }
        }

        public void MoveItems(int[] rows, int step)
        {
            if (step == 0 || rows.Length == 0)
                return;

            // calculate new indices
            int[] newIndices = CustomSortCommand.GetNewIndices(Items.Length, rows, step);

            // save list box current status
            bool[] selects = new bool[Items.Length];
            object[] items = new object[Items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = listBox.Items[i];
                selects[i] = listBox.GetSelected(i);
            }

            // set list box items
            for (int i = 0; i < newIndices.Length; i++)
            {
                int ni = newIndices[i];
                if (ni != i)
                {
                    listBox.Items[ni] = items[i];
                    listBox.SetSelected(ni, selects[i]);
                }
            }
        }

        #endregion

        public int[] GetNewIndices()
        {
            int[] nis = new int[Items.Length];
            for (int i = 0; i < listBox.Items.Count; i++)
            {
                int oi = Array.IndexOf(Items, listBox.Items[i]);
                if(oi > -1 && oi < nis.Length)
                    nis[oi] = i;
            }

            return nis;
        }

        private void OnItemsChanged()
        {
            listBox.Items.Clear();
            if (Items != null)
            {
                listBox.Items.AddRange(Items);
                //foreach (object obj in Items)
                //{
                //    ListBoxExItem item = new ListBoxExItem();
                //    if (obj != null)
                //    {
                //        item.Text = obj.ToString();
                //        item.Tag = obj;
                //    }
                //    listBox.Items.Add(item);
                //}
            }
        }

        private int[] GetSelectedIndices()
        {
            int[] indices = new int[listBox.SelectedIndices.Count];
            for (int i = 0; i < listBox.SelectedIndices.Count; i++)
                indices[i] = listBox.SelectedIndices[i];

            return indices;
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            listBox.Size = new Size(ClientSize.Width - btnMoveUp.Width - 2, ClientSize.Height);

            btnMoveUp.Location = new Point(ClientSize.Width - btnMoveUp.Width, ClientSize.Height / 2 - btnMoveUp.Height - 8);
            btnMoveDown.Location = new Point(ClientSize.Width - btnMoveDown.Width, ClientSize.Height / 2 + 8);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            if (listBox.CanFocus)
                listBox.Focus();
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ResetButtons();

            if (listBox.SelectedIndices.Count == 0)
            {
                btnMoveUp.Enabled = false;
                btnMoveDown.Enabled = false;
            }
            else
            {
                int max_sel = -1;
                int min_sel = Items.Length;
                foreach (int index in listBox.SelectedIndices)
                {
                    max_sel = Math.Max(max_sel, index);
                    min_sel = Math.Min(min_sel, index);
                }

                btnMoveUp.Enabled = min_sel > 0;
                btnMoveDown.Enabled = max_sel < Items.Length - 1;
            }
        }

        private void BtnMoveUp_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedIndices.Count > 0)
            {
                MoveUp();
            }
        }

        private void BtnMoveDown_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedIndices.Count > 0)
            {
                MoveDown();
            }
        }

        #region draw feedback box
        class DragFeedback : Control
        {
            private Control _Owner;

            public DragFeedback(Control owner)
            {
                Owner = owner;

                BackColor = Color.SlateBlue;// SystemColors.Highlight;
                ForeColor = Color.LavenderBlush;// SystemColors.HighlightText;
                Visible = false;
            }

            public Control Owner
            {
                get { return _Owner; }
                private set { _Owner = value; }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics grf = e.Graphics;
                grf.Clear(Owner.BackColor);

                Rectangle rect = new Rectangle(0, 0, Width, Height);
                //rect.Inflate(0, -1);
                //grf.FillRectangle(SystemBrushes.Highlight, rect);

                LinearGradientBrush brush = new LinearGradientBrush(rect, PaintHelper.GetLightColor(this.BackColor),
                    this.BackColor, 90.0f);
                grf.FillRectangle(brush, rect);

                if (!string.IsNullOrEmpty(Text))
                {
                    grf.DrawString(Text, Owner.Font, new SolidBrush(this.ForeColor), rect, PaintHelper.SFLeft);
                }

                grf.DrawRectangle(new Pen(PaintHelper.GetDarkColor(this.BackColor)), rect.Left, rect.Top, rect.Width - 1, rect.Height - 1);
            }
        }
        #endregion
    }
}
