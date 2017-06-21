using System.Drawing;
using System.Windows.Forms;
using Blumind.Core;
using System.Drawing.Drawing2D;

namespace Blumind.Controls
{
    class ToolStripProReander : ToolStripProfessionalRenderer
    {
        static ColorBackImage _BarBackImage;
        static ColorBackImage _ToolButtonBackImage;
        static ColorBackImage _SplitterVBackImage;
        static ColorBackImage _SplitterHBackImage;
        public static readonly ToolStripProReander Default = new ToolStripProReander();

        static ColorBackImage BarBackImage
        {
            get
            {
                if (_BarBackImage == null)
                    _BarBackImage = new ColorBackImage(Properties.Resources.bar);
                return _BarBackImage;
            }
        }

        static ColorBackImage ToolButtonBackImage
        {
            get
            {
                if (_ToolButtonBackImage == null)
                    _ToolButtonBackImage = new ColorBackImage(Properties.Resources.tool_button);
                return _ToolButtonBackImage;
            }
        }

        static ColorBackImage SplitterVBackImage
        {
            get
            {
                if (_SplitterVBackImage == null)
                    _SplitterVBackImage = new ColorBackImage(Properties.Resources.splitter_v);
                return _SplitterVBackImage;
            }
        }

        static ColorBackImage SplitterHBackImage
        {
            get
            {
                if (_SplitterHBackImage == null)
                    _SplitterHBackImage = new ColorBackImage(Properties.Resources.splitter_h);
                return _SplitterHBackImage;
            }
        }

        private ToolStripProReander()
        {
            RoundedEdges = false;
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            if (e.Item is ToolStripMenuItem && e.Item.Selected)
            {
                e.ArrowColor = Color.Black;
            }

            base.OnRenderArrow(e);
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            e.Graphics.Clear(e.BackColor);

            if (e.ToolStrip is ToolStripDropDownMenu)
            {
                //base.OnRenderToolStripBackground(e);
                Rectangle rect = e.AffectedBounds;
                //Image image = MenuBackImage.Image;
                //PT.ExpandImage(e.Graphics, image, 10, rect, new Rectangle(0, 0, image.Width, image.Height));
                //e.ToolStrip.Region = new Region(PT.GetRoundRectangle(new Rectangle(rect.Left, rect.Top, rect.Width + 1, rect.Height + 1), 2));

                e.Graphics.FillRectangle(new SolidBrush(UITheme.Default.Colors.MediumLight), rect);

                GraphicsPath path = PaintHelper.GetRoundRectangle(rect, 2);
                e.Graphics.DrawPath(Pens.Gray, path);

                e.Graphics.FillRectangle(Brushes.Silver, new Rectangle(rect.Right - 1, rect.Bottom - 1, 1, 1));
            }
            else
            {
                int def = 0;
                if (e.ToolStrip is ToolStripPro && ((ToolStripPro)e.ToolStrip).HalfRenderBackground)
                    def = 2;

                Image image = BarBackImage.Image;
                PaintHelper.ExpandImage(e.Graphics, image, 5, e.AffectedBounds, new Rectangle(0, def, image.Width, image.Height - def));
            }
            //base.OnRenderToolStripBackground(e);
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            //base.OnRenderImageMargin(e);
            Rectangle rect = e.AffectedBounds;
            rect.X += 2;
            rect.Width -= 2;
            rect.Y += 2;
            rect.Height -= 4;

            Color color = UITheme.Default.Colors.MediumDark;// PaintHelper.AdjustColor(Blumind.Core.Settings.Options.Default.Appearance.WindowBackColor, 15, 25, 90, 90);
            e.Graphics.FillRectangle(new SolidBrush(color), rect);
            e.Graphics.DrawLine(new Pen(PaintHelper.GetDarkColor(color, .1f)), rect.Right - 1, rect.Y, rect.Right - 1, rect.Bottom - 1);
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            Rectangle rect = new Rectangle(0, 0, e.Item.Bounds.Width, e.Item.Bounds.Height);

            const int ItemSize = 71;
            Image image = ToolButtonBackImage.Image;

            int x = 0;
            if (e.Item.Pressed)
                x = 1 * ItemSize;
            else if (e.Item.Selected)
                x = 3 * ItemSize;

            if (e.Item is ToolStripButton)
            {
                if (((ToolStripButton)e.Item).Checked)
                    x = 1 * ItemSize;
            }

            PaintHelper.ExpandImage(e.Graphics, image, 5, rect, new Rectangle(x, 0, ItemSize, image.Height));
        }

        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            OnRenderButtonBackground(e);
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            Rectangle rect = new Rectangle(0, 0, e.Item.Bounds.Width, e.Item.Bounds.Height);
            if (e.Vertical)
            {
                PaintHelper.DrawImageTiled(e.Graphics, SplitterVBackImage.Image, rect, PaintHelper.DrawImageMethod.RepeatY);
            }
            else
            {
                rect.X += 27;
                rect.Width -= 27;
                int y = rect.Y + rect.Height / 2;
                e.Graphics.DrawLine(Pens.LightGray, rect.X, y, rect.Right, y);
                //PT.DrawImageTiled(e.Graphics, SplitterHBackImage.Image, rect, PT.DrawImageMethod.RepeatX);
            }

            //base.OnRenderSeparator(e);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (e.Item is ToolStripMenuItem)
            {
                if (e.Item.Selected)
                    e.TextColor = Color.Black;
                e.TextFont = SystemFonts.MenuFont;
                //TextRenderer.DrawText(e.Graphics, e.TextColor.ToString(), e.TextFont, e.TextRectangle, e.TextColor, e.TextFormat);
                //return;
            }

            base.OnRenderItemText(e);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(e.ToolStrip.ToString());
            if (e.ToolStrip is ToolStripDropDownMenu)
            {
                Color color;
                if (e.Item.Selected)
                {
                    if (e.Item.Enabled)
                        color = PaintHelper.AdjustColor(UITheme.Default.Colors.Sharp, 40, 50, 85, 90);
                    else
                        color = Color.WhiteSmoke;
                }
                else
                {
                    return;
                }
                Rectangle rect = new Rectangle(0, 0, e.Item.Bounds.Width, e.Item.Bounds.Height);
                rect.Inflate(-2, 0);

                PaintHelper.DrawHoverBackground(e.Graphics, rect, color);
                return;
            }

            base.OnRenderMenuItemBackground(e);
        }

        protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStripSplitButton tssb = (ToolStripSplitButton)e.Item;
            Rectangle rect = new Rectangle(0, 0, e.Item.Bounds.Width, e.Item.Bounds.Height);

            const int ItemSize = 71;
            Image image = ToolButtonBackImage.Image;

            // button
            int x = 0;
            if (tssb.ButtonPressed)
                x = 1 * ItemSize;
            else if (tssb.ButtonSelected)
                x = 3 * ItemSize;
            PaintHelper.ExpandImage(e.Graphics, image, 5, tssb.ButtonBounds, new Rectangle(x, 0, ItemSize, image.Height));

            // drop down arrow
            x = 0;
            if (tssb.DropDownButtonPressed)
                x = 1 * ItemSize;
            else if (tssb.DropDownButtonSelected)
                x = 3 * ItemSize;
            PaintHelper.ExpandImage(e.Graphics, image, 5, tssb.DropDownButtonBounds, new Rectangle(x, 0, ItemSize, image.Height));

            ToolStripArrowRenderEventArgs tsarea = new ToolStripArrowRenderEventArgs(e.Graphics, e.Item, tssb.DropDownButtonBounds, Color.Black, ArrowDirection.Down);
            DrawArrow(tsarea);
        }

        protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
        {
            //base.OnRenderGrip(e);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            //base.OnRenderToolStripBorder(e);
        }
    }
}
