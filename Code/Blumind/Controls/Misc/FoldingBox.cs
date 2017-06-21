using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Blumind.Controls
{
    [DefaultEvent("FoldedChanged")]
    class FoldingBox : ContainerControl
    {
        private const int FoldingButtonSize = 16;
        private Rectangle FoldingButtonRect;
        private bool _Folded;
        private int TitleHeight = FoldingButtonSize;
        private int _FullHeight = -1;
        private List<Control> HideControls = new List<Control>();

        public event EventHandler FoldedChanged;

        public FoldingBox()
        {
            SetStyle(ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.Selectable, true);
        }

        [DefaultValue(false)]
        public bool Folded
        {
            get { return _Folded; }
            set 
            {
                if (_Folded != value)
                {
                    _Folded = value;
                    OnFoldedChanged();
                }
            }
        }

        [DefaultValue(-1)]
        public int FullHeight
        {
            get { return _FullHeight; }
            set { _FullHeight = value; }
        }

        protected override Padding DefaultPadding
        {
            get
            {
                return new Padding(3, TitleHeight, 3, 3);
            }
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            Size sizeText = TextRenderer.MeasureText(Text, Font);
            TitleHeight = Math.Max(FoldingButtonSize, sizeText.Height);

            FoldingButtonRect = new Rectangle(0, 0, FoldingButtonSize, TitleHeight);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            InvokePaintBackground(this, e);

            Rectangle rectTitle = ClientRectangle;
            rectTitle.Height = FoldingButtonRect.Height;

            Size sizeText = Size.Ceiling(e.Graphics.MeasureString(Text, Font));
            rectTitle.Height = Math.Max(rectTitle.Height, sizeText.Height);

            Image image = Folded ? Properties.Resources.folding_expand : Properties.Resources.folding_collapse;
            PaintHelper.DrawImageInRange(e.Graphics, image, FoldingButtonRect);
            rectTitle.X += FoldingButtonRect.Width;
            rectTitle.Width -= FoldingButtonRect.Width;

            e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), rectTitle, PaintHelper.SFLeft);
            rectTitle.X += sizeText.Width;
            rectTitle.Width -= sizeText.Width;

            PaintHelper.SetHighQualityRender(e.Graphics);
            e.Graphics.ExcludeClip(new Rectangle(ClientRectangle.Left, ClientRectangle.Top, ClientRectangle.Width - rectTitle.Width, rectTitle.Height));
            Rectangle rectBorder = new Rectangle(0, rectTitle.Y + rectTitle.Height / 2, ClientSize.Width, ClientSize.Height - rectTitle.Height / 2);
            Pen penLine = new Pen(PaintHelper.GetDarkColor(BackColor));
            if (Folded)
            {
                e.Graphics.DrawLine(penLine, rectBorder.X, rectBorder.Y, rectBorder.Right, rectBorder.Y);
            }
            else
            {
                GraphicsPath path = PaintHelper.GetRoundRectangle(rectBorder, 6);
                e.Graphics.DrawPath(penLine, path);
                path.Dispose();
            }

            if (Focused)
            {
                ControlPaint.DrawFocusRectangle(e.Graphics, new Rectangle(0, 0, ClientSize.Width, ClientSize.Height));
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (FoldingButtonRect.Contains(e.X, e.Y))
            {
                Folded = !Folded;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                Folded = !Folded;
                e.SuppressKeyPress = true;
                return;
            }

            base.OnKeyDown(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            Invalidate();
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);

            if ((specified & BoundsSpecified.Height) == BoundsSpecified.Height)
            {
                TitleHeight = (int)Math.Ceiling(TitleHeight * factor.Height);
                if (Folded)
                {
                    FullHeight = (int)Math.Ceiling(FullHeight * factor.Height);
                }
            }
        }

        private void OnFoldedChanged()
        {
            if (Folded)
            {
                HideControls.Clear();
                foreach (Control ctl in Controls)
                {
                    if (ctl.Visible)
                    {
                        HideControls.Add(ctl);
                        ctl.Visible = false;
                    }
                }
                FullHeight = Height;
                Height = TitleHeight;
            }
            else
            {
                foreach (Control ctl in HideControls)
                {
                    if (!ctl.Visible)
                    {
                        ctl.Visible = true;
                    }
                }
                HideControls.Clear();
                Height = FullHeight;
            }

            if (FoldedChanged != null)
            {
                FoldedChanged(this, EventArgs.Empty);
            }

            Invalidate();
        }
    }
}
