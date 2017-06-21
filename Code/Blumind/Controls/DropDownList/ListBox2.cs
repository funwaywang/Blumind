using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Blumind.Controls
{
    class ListBox2 : ListBox
    {
        //private Point MouseDownPos;
        //private bool IsMouseDown;
        //private bool IsDraging;
        //private DragFeedback DragFeedbackBox;

        public ListBox2()
        {
            DrawMode = DrawMode.OwnerDrawVariable;//.OwnerDrawFixed;
            IntegralHeight = false;
            ItemHeight += 10;

            SetStyle(
                ControlStyles.OptimizedDoubleBuffer, true);
        }

        /*private void ShowDragBox(int x, int y)
        {
            if (DragFeedbackBox == null)
            {
                DragFeedbackBox = new DragFeedback(this);
                this.Controls.Add(DragFeedbackBox);
            }

            DragFeedbackBox.Size = new Size(ClientSize.Width, 4);
            DragFeedbackBox.Location = new Point(ClientRectangle.X, y);
            DragFeedbackBox.Show();
        }*/

        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //    if (IsDraging)
        //    {
        //        ShowDragBox(e.X, e.Y);
        //    }
        //    else if (IsMouseDown)
        //    {
        //        if (Math.Abs(e.X - MouseDownPos.X) > SystemInformation.DragSize.Width ||
        //            Math.Abs(e.Y - MouseDownPos.Y) > SystemInformation.DragSize.Height)
        //        {
        //            IsDraging = true;
        //        }
        //    }
        //    else
        //    {
        //        base.OnMouseMove(e);
        //    }
        //}
        /*
        protected override void OnMouseUp(MouseEventArgs e)
        {
            IsMouseDown = false;
            IsDraging = false;
            if (DragFeedbackBox != null && DragFeedbackBox.Visible)
            {
                DragFeedbackBox.Hide();
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MouseDownPos = new Point(e.X, e.Y);
                IsMouseDown = true;
            }

            base.OnMouseDown(e);
        }*/

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            Update();
            //Invalidate();
        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            base.OnMeasureItem(e);

            e.ItemWidth = this.Width;
            e.ItemHeight = ItemHeight;
            if (e.Index > -1 && e.Index < Items.Count)
            {
                Size size = Size.Ceiling(e.Graphics.MeasureString(Items[e.Index].ToString(), Font));
                e.ItemHeight = (ItemHeight - Font.Height) + size.Height;
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= Items.Count)
            {
                base.OnDrawItem(e);
                return;
            }

            e.DrawBackground();

            Rectangle rect = GetItemRectangle(e.Index);
            Brush brushFore;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                //Color color = PaintHelper.AdjustColor(SystemColors.Highlight, 40, 50, 85, 90); //SystemColors.Highlight;
                //LinearGradientBrush brushBack = new LinearGradientBrush(rect, PT.GetLightColor(color, .2), color, 90.0f);
                //e.Graphics.FillRectangle(brushBack, rect.Left, rect.Top, rect.Width, rect.Height);
                PaintHelper.DrawHoverBackgroundFlat(e.Graphics, rect, UITheme.Default.Colors.Sharp);
                brushFore = new SolidBrush(UITheme.Default.Colors.SharpText);
                //brushFore = Brushes.Black;
            }
            else
            {
                //e.Graphics.FillRectangle(new LinearGradientBrush(rect, Color.White, Color.GhostWhite, 90.0f)
                //    , rect.Left, rect.Top, rect.Width, rect.Height);

                //e.Graphics.DrawLine(new Pen(Color.FromArgb(100, Color.Silver)), rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
                brushFore = Brushes.Black;
            }
            e.Graphics.DrawLine(new Pen(Color.FromArgb(100, Color.Silver)), rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);

            object obj = Items[e.Index];
            if (obj != null)
            {
                rect.Inflate(-1, -1);
                StringFormat sf = PaintHelper.SFLeft;
                sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
                e.Graphics.DrawString(obj.ToString(), Font, brushFore, rect, sf);
            }
        }
    }
}
