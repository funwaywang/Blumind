using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace Blumind.Controls
{
    class NormalTabBarRenderer : TabBarRenderer
    {
        public NormalTabBarRenderer(TabBar bar)
            : base(bar)
        {
        }

        public override void DrawBackground(PaintEventArgs e)
        {
            e.Graphics.Clear(Bar.BackColor);
        }

        public override void DrawItem(TabItemPaintEventArgs e)
        {
            Rectangle rect = e.Bounds;
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            // draw background
            DrawItemBackground(e);

            rect = e.ContentRectangle;
            // draw icon
            if (e.Item.Icon != null)
            {
                Rectangle rect_icon;
                if (e.IsHorizontal)
                {
                    if (string.IsNullOrEmpty(e.Item.Text) && !e.Item.CanClose)
                        rect_icon = new Rectangle(rect.Left + (rect.Width - e.IconSize.Width) / 2, rect.Top + (rect.Height - e.IconSize.Height) / 2, e.IconSize.Width, e.IconSize.Height);
                    else
                        rect_icon = new Rectangle(rect.Left, rect.Top + (rect.Height - e.IconSize.Height) / 2, e.IconSize.Width, e.IconSize.Height);
                }
                else
                {
                    if (string.IsNullOrEmpty(e.Item.Text) && !e.Item.CanClose)
                        rect_icon = new Rectangle(rect.Left + (rect.Width - e.IconSize.Width) / 2, rect.Top + (rect.Height - e.IconSize.Height) / 2, e.IconSize.Width, e.IconSize.Height);
                    else
                        rect_icon = new Rectangle(rect.Left + (rect.Width - e.IconSize.Width) / 2, rect.Top, e.IconSize.Width, e.IconSize.Height);
                }

                PaintHelper.DrawImageInRange(e.Graphics, e.Item.Icon, rect_icon);
                if (e.IsHorizontal)
                {
                    rect.X += rect_icon.Width;
                    rect.Width -= rect_icon.Width;
                }
                else
                {
                    rect.Y += rect_icon.Height;
                    rect.Height -= rect_icon.Height;
                }
            }

            if (e.Item.CanClose)
            {
                //System.Diagnostics.Debug.WriteLine(string.Format("> {0}, {1}", HoverHitResult.Item, HoverHitResult.InCloseButton));

                Rectangle rect_close_btn = e.Item.GetCloseButtonRect();
                DrawCloseButton(e, rect_close_btn);
                if (e.IsHorizontal)
                    rect.Width -= e.Bar.CloseButtonSize.Width;
                else
                    rect.Height -= e.Bar.CloseButtonSize.Height;
            }

            if (!string.IsNullOrEmpty(e.Item.Text))
            {
                Rectangle rect_text = rect;
                GraphicsState gs = null;
                if (!e.IsHorizontal)
                {
                    gs = e.Graphics.Save();
                    e.Graphics.RotateTransform(90);
                    rect_text = new Rectangle(rect.Y, -rect.X - rect.Width, rect.Height, rect.Width);
                    //e.Graphics.DrawRectangle(Pens.Blue, rect_text);
                }

                DrawItemText(e, rect_text);

                if (!e.IsHorizontal)
                {
                    e.Graphics.Restore(gs);
                }
            }
        }

        protected virtual void DrawItemText(TabItemPaintEventArgs e, Rectangle rect)
        {
            Color foreColor = e.Bar.ItemForeColor;
            switch (e.Status)
            {
                case UIControlStatus.Selected:
                case UIControlStatus.Focused:
                    foreColor = e.Bar.SelectedItemForeColor;
                    break;
                case UIControlStatus.Hover:
                    foreColor = e.Bar.HoverItemForeColor;
                    break;
            }

            if (!foreColor.IsEmpty)
            {
                StringFormat sf = e.HalfDisplay ? PaintHelper.SFLeft : PaintHelper.SFCenter;
                sf.FormatFlags |= StringFormatFlags.NoWrap;
                sf.Trimming = StringTrimming.None;

                if (e.IsHorizontal)
                {
                    rect.X += e.Padding.Left;
                    rect.Width -= e.Padding.Horizontal;
                }
                else
                {
                    rect.Y += e.Padding.Top;
                    rect.Height -= e.Padding.Vertical;
                }

                e.Graphics.DrawString(e.Item.Text, e.Font, new SolidBrush(foreColor), rect, sf);
            }
        }

        protected override void DrawCloseButton(TabItemPaintEventArgs e, Rectangle rect)
        {
            //Skin.Instance.DrawTaskBarItemCloseButton(new SkinPaintEventArgs(this, e), rect, tis, Alignment, item == SelectedItem);
            Image image = Properties.Resources.close_button; // Images.close_button;

            Rectangle rectS = new Rectangle(0, 0, image.Width / 4, image.Height);
            if (!e.Selected)
            {
                rectS.X = rectS.Width * 3;
            }

            switch (e.CloseButtonStatus)
            {
                case UIControlStatus.Hover:
                    rectS.X = rectS.Width * 1;
                    break;
                case UIControlStatus.Selected:
                    rectS.X = rectS.Width * 2;
                    break;
                case UIControlStatus.Disabled:
                    rectS.X = rectS.Width * 3;
                    break;
            }

            PaintHelper.DrawImageInRange(e.Graphics, image, rect, rectS);
        }

        public override void Initialize(PaintEventArgs e)
        {
            base.Initialize(e);

            PaintHelper.SetHighQualityRender(e.Graphics);
            e.Graphics.SmoothingMode = SmoothingMode.Default;
        }

        protected virtual void DrawItemBackground(TabItemPaintEventArgs e)
        {
            var rect = e.Bounds;
            Color borderColor = Color.Empty;
            if (e.Selected)
            {
                // 把BaseLineSize加回去
                switch (e.Bar.Alignment)
                {
                    case TabAlignment.Left:
                        rect.Width += e.Bar.BaseLineSize;
                        break;
                    case TabAlignment.Top:
                        rect.Height += e.Bar.BaseLineSize;
                        break;
                    case TabAlignment.Right:
                        rect.X -= e.Bar.BaseLineSize;
                        rect.Width += e.Bar.BaseLineSize;
                        break;
                    case TabAlignment.Bottom:
                        rect.Y -= e.Bar.BaseLineSize;
                        rect.Height += e.Bar.BaseLineSize;
                        break;
                }

                Color backColor = e.Item.BackColor ?? e.Bar.SelectedItemBackColor;
                if (!backColor.IsEmpty)
                {
                    //LinearGradientBrush backBrush = new LinearGradientBrush(e.Bounds, PaintHelper.GetLightColor(backColor), backColor, 90.0f);
                    var backBrush = new SolidBrush(backColor);
                    e.Graphics.FillRectangle(backBrush, rect);
                }

                borderColor = e.Bar.BaseLineColor;
            }
            else
            {
                Color backColor = e.Item.BackColor ?? e.Bar.ItemBackColor;
                switch (e.Status)
                {
                    case UIControlStatus.Hover:
                        backColor = e.Bar.HoverItemBackColor;
                        break;
                }

                if (!backColor.IsEmpty)
                {
                    var backBrush = new SolidBrush(backColor);
                    e.Graphics.FillRectangle(backBrush, e.Item.Bounds);

                    borderColor = PaintHelper.AdjustColorS(PaintHelper.GetDarkColor(backColor, 0.15), 10, 20);
                }
            }

            if (!borderColor.IsEmpty)
            {
                DrawItemBorder(e, rect, borderColor);
            }
        }

        protected virtual void DrawItemBorder(TabItemPaintEventArgs e, Rectangle rect, Color color)
        {
            rect.Width--;
            rect.Height--;
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            Point[] pts = null;
            switch (Bar.Alignment)
            {
                case TabAlignment.Left:
                    pts = new Point[]{ 
                                new Point(rect.Right, rect.Top), 
                                new Point(rect.Left, rect.Top), 
                                new Point(rect.Left, rect.Bottom), 
                                new Point(rect.Right, rect.Bottom) };
                    break;
                case TabAlignment.Bottom:
                    pts = new Point[]{ 
                                new Point(rect.X, rect.Top), 
                                new Point(rect.X, rect.Bottom), 
                                new Point(rect.Right, rect.Bottom), 
                                new Point(rect.Right, rect.Top) };
                    break;
                case TabAlignment.Right:
                    pts = new Point[]{ 
                                new Point(rect.Left, rect.Top), 
                                new Point(rect.Right, rect.Top), 
                                new Point(rect.Right, rect.Bottom), 
                                new Point(rect.Left, rect.Bottom) };
                    break;
                case TabAlignment.Top:
                default:
                    pts = new Point[]{ 
                                new Point(rect.X, rect.Bottom), 
                                new Point(rect.X, rect.Y), 
                                new Point(rect.Right, rect.Y), 
                                new Point(rect.Right, rect.Bottom) };
                    break;
            }

            Pen pen = new Pen(color);
            var pom = e.Graphics.PixelOffsetMode;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.Default;
            e.Graphics.DrawLines(pen, pts);
            e.Graphics.PixelOffsetMode = pom;
            //e.Graphics.DrawPath(new Pen(Bar.BaseLineColor), path);
        }

        protected override void DrawButton(PaintEventArgs e, TabBarButton button, UIControlStatus ucs)
        {
            DrawButtonBackground(e, button, ucs);

            if (button.DisplayStyle == ToolStripItemDisplayStyle.Image)
            {
                if (button.Icon != null)
                {
                    PaintHelper.DrawImageInRange(e.Graphics, button.Icon, button.Bounds);
                }
            }
            else if (button.DisplayStyle == ToolStripItemDisplayStyle.ImageAndText)
            {
                DrawButtonImageAndText(e, button, ucs);
            }
            else if (button.DisplayStyle == ToolStripItemDisplayStyle.Text)
            {
                DrawButtonText(e, button, ucs);
            }
        }

        protected virtual void DrawButtonBackground(PaintEventArgs e, TabBarButton button, UIControlStatus ucs)
        {
            Rectangle rect = button.Bounds;
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            Color backColor;
            if (button.IsDroppingDown)
            {
                //backColor = UITheme.Default.Colors.MenuImageMargin;
                backColor = Bar.SelectedItemBackColor;
            }
            else
            {
                switch (ucs)
                {
                    case UIControlStatus.Hover:
                        backColor = Bar.HoverItemBackColor;
                        break;
                    case UIControlStatus.Selected:
                        backColor = Bar.SelectedItemBackColor;
                        break;
                    default:
                        return;
                }
            }

            e.Graphics.FillRectangle(new SolidBrush(backColor), rect);
        }

        void DrawButtonText(PaintEventArgs e, TabBarButton button, UIControlStatus ucs)
        {
            if (Bar != null)
            {
                e.Graphics.DrawString(button.Text, Bar.Font, new SolidBrush(Bar.ForeColor), button.Bounds, PaintHelper.SFCenter);
            }
        }

        void DrawButtonImageAndText(PaintEventArgs e, TabBarButton button, UIControlStatus ucs)
        {
            if (button.Icon != null && string.IsNullOrEmpty(button.Text))
            {
                PaintHelper.DrawImageInRange(e.Graphics, button.Icon, button.Bounds);
            }
            else if (button.Icon == null && string.IsNullOrEmpty(button.Text))
            {
                DrawButtonText(e, button, ucs);
            }
            else
            {
                Size size = Size.Ceiling(e.Graphics.MeasureString(button.Text, Bar.Font));
                size.Width += button.Icon.Width;
                Rectangle rect = new Rectangle(
                    button.Bounds.X + (button.Bounds.Width - size.Width) / 2,
                    button.Bounds.Y + (button.Bounds.Height - size.Height) / 2,
                    size.Width,
                    size.Height);

                e.Graphics.DrawImage(button.Icon,
                    new Rectangle(rect.Left, rect.Top + (rect.Height - button.Icon.Height) / 2, button.Icon.Width, button.Icon.Height),
                    0, 0, button.Icon.Width, button.Icon.Height, GraphicsUnit.Pixel);

                rect.X += button.Icon.Width;
                rect.Width -= button.Icon.Width;

                e.Graphics.DrawString(button.Text, Bar.Font, new SolidBrush(Bar.ForeColor), rect, PaintHelper.SFCenter);
            }
        }

        public override void DrawBaseLine(PaintEventArgs e)
        {
            GraphicsState gs = e.Graphics.Save();
            e.Graphics.SmoothingMode = SmoothingMode.None;

            if (Bar.SelectedItem != null)
            {
                if (Bar.IsHorizontal)
                    e.Graphics.ExcludeClip(new Rectangle(Bar.SelectedItem.Bounds.X, 0, Bar.SelectedItem.Bounds.Width, Bar.Height));
                else
                    e.Graphics.ExcludeClip(new Rectangle(0, Bar.SelectedItem.Bounds.Y, Bar.Width, Bar.SelectedItem.Bounds.Height));
            }

            Rectangle rect;
            switch (Bar.Alignment)
            {
                case TabAlignment.Bottom:
                    rect = new Rectangle(0, 0, Bar.Width, Bar.BaseLineSize);
                    break;
                case TabAlignment.Top:
                default:
                    rect = new Rectangle(0, Bar.Height - Bar.BaseLineSize, Bar.Width, Bar.BaseLineSize);
                    break;
            }
            e.Graphics.FillRectangle(new SolidBrush(Bar.BaseLineColor), rect);

            e.Graphics.Restore(gs);
        }
    }
}
