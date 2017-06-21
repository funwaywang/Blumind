using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Blumind.Controls
{
    class TaskBarRenderer : NormalTabBarRenderer
    {
        public TaskBarRenderer(TabBar bar)
            : base(bar)
        {
        }

        public TaskBar TaskBar
        {
            get
            {
                return (TaskBar)Bar;
            }
        }

        public override void Initialize(PaintEventArgs e)
        {
            base.Initialize(e);

            //PaintHelper.SetHighQualityRender(e.Graphics);
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            //e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }

        protected override void DrawItemText(TabItemPaintEventArgs e, Rectangle rect)
        {
            //base.DrawItemText(e, rect);

            var taskBar = e.GetBar<TaskBar>();
            Color foreColor = taskBar.ItemForeColor;
            switch (e.Status)
            {
                case UIControlStatus.Selected:
                case UIControlStatus.Focused:
                    if (taskBar.IsActive)
                        foreColor = taskBar.SelectedItemForeColor;
                    else
                        foreColor = taskBar.InactiveSelectedItemForeColor;
                    break;
                case UIControlStatus.Hover:
                    foreColor = taskBar.HoverItemForeColor;
                    break;
            }

            if (!foreColor.IsEmpty)
            {
                StringFormat sf = PaintHelper.SFCenter;
                sf.FormatFlags |= StringFormatFlags.NoWrap;
                e.Graphics.DrawString(e.Item.Text, e.Font, new SolidBrush(foreColor), rect, sf);
            }
        }

        protected override void DrawItemBackground(TabItemPaintEventArgs e)
        {
            //base.DrawItemBackground(e);

            TaskBar taskBar = e.GetBar<TaskBar>();
            Color backColor = e.Bar.ItemBackColor;
            switch (e.Status)
            {
                case UIControlStatus.Selected:
                case UIControlStatus.Focused:
                    if (taskBar.IsActive)
                        backColor = taskBar.SelectedItemBackColor;
                    else
                        backColor = taskBar.InactiveSelectedItemBackColor;
                    break;
                case UIControlStatus.Hover:
                    backColor = taskBar.HoverItemBackColor;
                    break;
            }

            if (!backColor.IsEmpty)
            {
                GraphicsPath path = PaintHelper.GetRoundRectangle(e.Bounds, taskBar.TabRounded, taskBar.TabRounded, 0, 0);
                //if (e.Status == UIControlStatus.Hover)
                //{
                //    LinearGradientBrush backBrush = new LinearGradientBrush(e.Bounds, backColor, backColor, 90.0f);
                //    ColorBlend cb = new ColorBlend(3);
                //    cb.Colors = new Color[] { PaintHelper.GetLightColor(backColor), backColor, backColor };
                //    cb.Positions = new float[] { 0.0f, 0.5f, 1.0f };
                //    backBrush.InterpolationColors = cb;
                //    e.Graphics.FillPath(backBrush, path);
                //}
                //else
                //{
                    SolidBrush backBrush = new SolidBrush(backColor);
                    e.Graphics.FillPath(backBrush, path);
                //}

                //
                if ((e.Status == UIControlStatus.Normal || e.Status == UIControlStatus.Hover)
                    && e.Bounds.Height > 10)
                {
                    Rectangle rectShadow = e.Bounds;
                    rectShadow.Y = rectShadow.Bottom - 6;
                    rectShadow.Height = 5;
                    var brush = new LinearGradientBrush(rectShadow, Color.Transparent, 
                        PaintHelper.AdjustColorSLess(PaintHelper.GetDarkColor(backColor), 20) , 90.0f);
                    rectShadow.Y++;//规避一个Gdi+错误, 会导致第一行有一个很深的线
                    //e.Graphics.FillRectangle(Brushes.Red, rectShadow);
                    e.Graphics.FillRectangle(brush, rectShadow);
                }
            }
        }

        public override void DrawBaseLine(PaintEventArgs e)
        {
            Rectangle rect = new Rectangle(0, Bar.Height - Bar.BaseLineSize, Bar.Width, Bar.BaseLineSize);
            GraphicsPath path = PaintHelper.GetRoundRectangle(rect, Bar.BaseLineSize, Bar.BaseLineSize, 0, 0);
            Color color = TaskBar.IsActive ? TaskBar.SelectedItemBackColor : TaskBar.InactiveSelectedItemBackColor;
            e.Graphics.FillPath(new SolidBrush(color), path);
        }

        protected override void DrawCloseButton(TabItemPaintEventArgs e, Rectangle rect)
        {
            Image image = Properties.Resources.taskbar_close_button;
            UIStatusImage img = UIStatusImage.FromVertical(image, 
                new UIControlStatus[] {
                    UIControlStatus.Normal,
                    UIControlStatus.Hover,
                    UIControlStatus.Selected,
                    UIControlStatus.InactivedHover,
                    UIControlStatus.Inactived,
                    UIControlStatus.Disabled});

            var bs = e.CloseButtonStatus;
            if (!e.Selected)
            {
                if (bs == UIControlStatus.Hover || bs == UIControlStatus.Selected)
                    bs = UIControlStatus.InactivedHover;
                else
                    bs = UIControlStatus.Inactived;
            }

            img.Draw(e.Graphics, bs, rect);
        }
    }
}
