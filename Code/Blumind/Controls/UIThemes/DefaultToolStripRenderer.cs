using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Blumind.Controls
{
    class DefaultToolStripRenderer : ToolStripProfessionalRenderer
    {
        private static Rectangle[] SizeGripRectangles = new Rectangle[] { 
            new Rectangle(8, 0, 2, 2), 
            new Rectangle(8, 4, 2, 2), 
            new Rectangle(8, 8, 2, 2), 
            new Rectangle(4, 4, 2, 2), 
            new Rectangle(4, 8, 2, 2), 
            new Rectangle(0, 8, 2, 2) };
        public static DefaultToolStripRenderer Default = new DefaultToolStripRenderer(new DefaultToolStripColors());

        private DefaultToolStripRenderer(DefaultToolStripColors colors)
            : base(colors)
        {
            Colors = colors;
            RoundedEdges = false;
        }

        public UIColorTheme ColorTheme
        {
            get { return UITheme.Default.Colors; }
        }

        public DefaultToolStripColors Colors { get; private set; }

        public static void Reset()
        {
            Default = new DefaultToolStripRenderer(new DefaultToolStripColors());
            UITheme.Default.ToolStripRenderer = Default;
        }

        protected override void InitializeItem(ToolStripItem item)
        {
            base.InitializeItem(item);

            if (item is ToolStripMenuItem)
            {
                InitializeMenuItem((ToolStripMenuItem)item);
            }
            else if (item is ToolStripDropDownButton)
            {
                foreach (var subItem in ((ToolStripDropDownButton)item).DropDownItems)
                {
                    if (subItem is ToolStripMenuItem)
                        InitializeMenuItem((ToolStripMenuItem)subItem);
                }
            }
        }

        protected virtual void InitializeMenuItem(ToolStripMenuItem item)
        {
            if (item == null)
                return;

            if (item.Owner is MenuStrip)
            {
                if (item.Padding.Left >= 4)
                    return;
                item.Padding = new Padding(item.Padding.Left + 4,
                    item.Padding.Top,
                    item.Padding.Right + 4,
                    item.Padding.Bottom);
            }
            else
            {
                if (item.Padding.Top >= 3)
                    return;
                item.Padding = new Padding(item.Padding.Left,
                    item.Padding.Top + 3,
                    item.Padding.Right,
                    item.Padding.Bottom + 3);
            }

            foreach (ToolStripItem submi in item.DropDownItems)
            {
                if (submi is ToolStripMenuItem)
                {
                    InitializeMenuItem((ToolStripMenuItem)submi);
                }
            }
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            //const int ROUND = 3;
            Rectangle rect = e.AffectedBounds;

            if (/*e.ToolStrip is MenuStrip || */e.ToolStrip is StatusStrip)
            {
                //LinearGradientBrush brush = new LinearGradientBrush(rect, ColorTable.MenuStripGradientBegin, ColorTable.MenuStripGradientEnd, 90.0f);
                //HatchBrush brush = new HatchBrush(HatchStyle.Percent20, Theme.Dark, PaintHelper.GetDarkColor(Theme.Dark, 0.1f));
                //e.Graphics.FillRectangle(brush, rect);
                e.Graphics.Clear(ColorTheme.Workspace);

                //int[] rounds = new int[] { ROUND, ROUND, ROUND, ROUND };
                //switch (e.ToolStrip.Dock)
                //{
                //    case DockStyle.Left:
                //        rounds[0] = rounds[3] = 0;
                //        break;
                //    case DockStyle.Top:
                //        rounds[0] = rounds[1] = 0;
                //        rect.Y--;
                //        rect.Inflate(1, 0);
                //        break;
                //    case DockStyle.Right:
                //        rounds[1] = rounds[2] = 0;
                //        break;
                //    case DockStyle.Bottom:
                //        rounds[2] = rounds[3] = 0;
                //        rect.Y++;
                //        rect.Inflate(1, 0);
                //        break;
                //}

                //SmoothingMode sm = e.Graphics.SmoothingMode;
                //e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                //GraphicsPath path = PaintHelper.GetRoundRectangle(rect, rounds[0], rounds[1], rounds[2], rounds[3]);
                //LinearGradientBrush brush2 = new LinearGradientBrush(rect, ColorTable.MenuStripGradientBegin, ColorTable.MenuStripGradientEnd, 90.0f);
                //e.Graphics.FillPath(brush2, path);
                //e.Graphics.DrawPath(new Pen(PaintHelper.GetDarkColor(ColorTable.MenuStripGradientBegin, 0.1f)), path);
                //e.Graphics.SmoothingMode = sm;
            }
            //else if (e.ToolStrip is StatusStrip)
            //{
            //    SolidBrush brush = new SolidBrush(ColorTable.MenuStripGradientBegin);
            //    e.Graphics.FillRectangle(brush, rect);
            //}
            //else 
            //if (e.ToolStrip is ToolStripDropDownMenu)
            //{
            //    base.OnRenderToolStripBackground(e);
            //    LinearGradientBrush brush = new LinearGradientBrush(rect, ColorTable.ToolStripDropDownBackground, PaintHelper.GetDarkColor(ColorTable.ToolStripDropDownBackground, .1), 45.0f);
            //    e.Graphics.FillRectangle(brush, rect);
            //    //e.Graphics.FillRectangle(new SolidBrush(DropDownMenuBackColor), rect.Left, rect.Top, 28, rect.Height);
            //    //e.Graphics.DrawRectangle(new Pen(DropDownMenuBorderColor), rect.Left, rect.Top, rect.Width - 1, rect.Height - 1);
            //}
            else
            {
                //if (!e.ToolStrip.GetType().IsSubclassOf(typeof(ToolStrip)) && e.ToolStrip.Parent is Form)
                //{
                //    LinearGradientBrush brush2 = new LinearGradientBrush(rect, Theme.Sharp, 
                //        Theme.Light, //PaintHelper.GetDarkColor(Theme.Sharp, 0.2), 
                //        90.0f);
                //    e.Graphics.FillRectangle(brush2, rect);
                //}
                //else
                {
                    base.OnRenderToolStripBackground(e);
                }
            }
        }

        //protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        //{
        //    Rectangle rect = new Rectangle(0, 0, e.Item.Bounds.Width, e.Item.Bounds.Height);

        //    const int ItemSize = 71;
        //    Image image = ToolButtonBackImage.Image;

        //    int x = 0;
        //    if (e.Item.Pressed)
        //        x = 1 * ItemSize;
        //    else if (e.Item.Selected)
        //        x = 3 * ItemSize;

        //    if (e.Item is ToolStripButton)
        //    {
        //        if (((ToolStripButton)e.Item).Checked)
        //            x = 1 * ItemSize;
        //    }

        //    PaintHelper.ExpandImage(e.Graphics, image, 5, rect, new Rectangle(x, 0, ItemSize, image.Height));
        //}

        //protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        //{
        //    OnRenderButtonBackground(e);
        //}

        //protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        //{
        //    Rectangle rect = new Rectangle(0, 0, e.Item.Bounds.Width, e.Item.Bounds.Height);

        //    if (e.Vertical)
        //    {
        //        rect.X += rect.Width/2;
        //        rect.Inflate(0, -2);
        //        e.Graphics.DrawLine(new Pen(SeparatorColor), rect.X, rect.Y, rect.X, rect.Bottom);
        //    }
        //    else
        //    {
        //        rect.Y += rect.Height / 2;
        //        rect.Inflate(-4, 0);
        //        if (e.ToolStrip is ToolStripDropDownMenu)
        //        {
        //            rect.X += 28;
        //            rect.Width -= 28;
        //        }
        //        e.Graphics.DrawLine(new Pen(SeparatorColor), rect.X, rect.Y, rect.Right, rect.Y);
        //    }

        //    //base.OnRenderSeparator(e);
        //}

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (e.ToolStrip is StatusStrip)
            {
                if (!e.Item.Pressed && !e.Item.Selected)
                {
                    e.TextColor = PaintHelper.FarthestColor(ColorTheme.Workspace, ColorTheme.MediumLight, ColorTheme.MediumDark);
                }
            }
            else
            {
                if (e.Item is ToolStripMenuItem)
                {
                    if (e.ToolStrip is MenuStrip)
                    {
                        if (!e.Item.Pressed && !e.Item.Selected)
                        {
                            //e.TextColor = PaintHelper.FarthestColor(ColorTheme.Workspace, ColorTheme.MediumLight, ColorTheme.MediumDark);
                            e.TextColor = Colors.MenuStripItemForeColor;
                        }
                    }
                    else if (e.Item.Selected)
                    {
                        e.TextColor = Colors.MenuStripItemSelectedForeColor;//
                    }

                    e.TextRectangle = new Rectangle(e.TextRectangle.X, 
                        e.TextRectangle.Y, 
                        e.TextRectangle.Width, 
                        e.TextRectangle.Height + e.Item.Padding.Vertical);

                    //e.Graphics.DrawRectangle(Pens.Red, e.TextRectangle.X, e.TextRectangle.Y, e.TextRectangle.Width - 1, e.TextRectangle.Height - 1);
                }
                else if (e.Item is ToolStripButton || e.Item is ToolStripDropDownButton || e.Item is ToolStripSplitButton)
                {
                    if (e.Item is ToolStripButton && ((ToolStripButton)e.Item).Checked)
                    {
                        e.TextColor = PaintHelper.FarthestColor(ColorTable.ButtonCheckedGradientMiddle, ColorTheme.Light, ColorTheme.Dark);//
                    }
                    else if (e.Item is ToolStripDropDownButton && e.Item.Pressed)
                    {
                        e.TextColor = PaintHelper.FarthestColor(ColorTable.ToolStripDropDownBackground, ColorTheme.Light, ColorTheme.Dark);//
                    }
                    else if (e.Item.Pressed || e.Item.Selected)
                    {
                        e.TextColor = PaintHelper.FarthestColor(ColorTable.ButtonSelectedGradientMiddle, ColorTheme.Light, ColorTheme.Dark);//
                    }
                    else
                    {
                        e.TextColor = PaintHelper.FarthestColor(ColorTable.ToolStripGradientMiddle, ColorTheme.Light, ColorTheme.Dark);//
                    }
                }
            }

            base.OnRenderItemText(e);
        }

        //protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
        //{
        //    ToolStripSplitButton tssb = (ToolStripSplitButton)e.Item;
        //    Rectangle rect = new Rectangle(0, 0, e.Item.Bounds.Width, e.Item.Bounds.Height);

        //    const int ItemSize = 71;
        //    Image image = ToolButtonBackImage.Image;

        //    // button
        //    int x = 0;
        //    if (tssb.ButtonPressed)
        //        x = 1 * ItemSize;
        //    else if (tssb.ButtonSelected)
        //        x = 3 * ItemSize;
        //    PaintHelper.ExpandImage(e.Graphics, image, 5, tssb.ButtonBounds, new Rectangle(x, 0, ItemSize, image.Height));

        //    // drop down arrow
        //    x = 0;
        //    if (tssb.DropDownButtonPressed)
        //        x = 1 * ItemSize;
        //    else if (tssb.DropDownButtonSelected)
        //        x = 3 * ItemSize;
        //    PaintHelper.ExpandImage(e.Graphics, image, 5, tssb.DropDownButtonBounds, new Rectangle(x, 0, ItemSize, image.Height));

        //    ToolStripArrowRenderEventArgs tsarea = new ToolStripArrowRenderEventArgs(e.Graphics, e.Item, tssb.DropDownButtonBounds, Color.Black, ArrowDirection.Down);
        //    DrawArrow(tsarea);
        //}

        protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
        {
            StatusStrip toolStrip = e.ToolStrip as StatusStrip;
            if (toolStrip != null)
            {
                Rectangle sizeGripBounds = toolStrip.SizeGripBounds;
                int size = Math.Min(sizeGripBounds.Width, sizeGripBounds.Height);
                if (sizeGripBounds.Width > 0 && sizeGripBounds.Height > 0)
                {
                    Rectangle[] rects1 = new Rectangle[SizeGripRectangles.Length];
                    Rectangle[] rects2 = new Rectangle[SizeGripRectangles.Length];

                    for (int i = 0; i < SizeGripRectangles.Length; i++)
                    {
                        Rectangle rect1 = SizeGripRectangles[i];
                        if (toolStrip.RightToLeft == RightToLeft.Yes)
                            rect1.X = (sizeGripBounds.Width - rect1.X) - rect1.Width;
                        rect1.Offset(sizeGripBounds.X, sizeGripBounds.Bottom - size);

                        Rectangle rect2 = rect1;
                        if (toolStrip.RightToLeft == RightToLeft.Yes)
                            rect2.Offset(1, -1);
                        else
                            rect2.Offset(-1, -1);

                        rects1[i] = rect1;
                        rects2[i] = rect2;
                    }

                    e.Graphics.FillRectangles(new SolidBrush(ColorTable.GripLight), rects1);
                    e.Graphics.FillRectangles(new SolidBrush(ColorTable.GripDark), rects2);
                }
            }

            //base.OnRenderStatusStripSizingGrip(e);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (e.ToolStrip is StatusStrip || e.ToolStrip is ToolStripPro)
            {
            }
            else if (!e.ToolStrip.GetType().IsSubclassOf(typeof(ToolStrip)) && e.ToolStrip.Parent is Form)
            {
            }
            else
            {
                base.OnRenderToolStripBorder(e);
            }
        }

        protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
        {
            //base.OnRenderGrip(e);
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            //Rectangle rect = e.AffectedBounds;
            //LinearGradientBrush brush = new LinearGradientBrush(rect, ColorTable.ImageMarginGradientBegin, ColorTable.ImageMarginGradientEnd, 0.0f);
            //e.Graphics.FillRectangle(brush, rect);
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            base.OnRenderSeparator(e);
        }
    }
}
