using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Blumind.Controls
{
    class PushButton : Button//, IGlobalBackground
    {
        /*private bool _IsMouseHover = false;
        private bool _IsMouseDown = false;

        public PushButton()
        {
        }

        protected bool IsMouseHover
        {
            get { return _IsMouseHover; }
            private set 
            {
                if (_IsMouseHover != value)
                {
                    _IsMouseHover = value;
                    OnIsMouseHoverChanged();
                }
            }
        }

        protected bool IsMouseDown
        {
            get { return _IsMouseDown; }
            private set 
            {
                if (_IsMouseDown != value)
                {
                    _IsMouseDown = value;
                    OnIsMouseDownChanged();
                }
            }
        }

        private void OnIsMouseHoverChanged()
        {
            Invalidate();
        }

        private void OnIsMouseDownChanged()
        {
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            IsMouseHover = true;
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);

            IsMouseDown = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            IsMouseHover = false;
            
            base.OnMouseLeave(e);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            IsMouseDown = false;

            base.OnMouseUp(mevent);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //if (Environment.OSVersion.Version > new Version(6, 0))
            //{
                base.OnPaint(e);
            //}
            //else
            //{
            //    e.Graphics.Clear(BackColor);
            //    GlobalBackground.Draw(e.Graphics);

            //    DrawBackground(e);
            //    DrawText(e);
            //}
        }

        private void DrawText(PaintEventArgs e)
        {
            Rectangle rect = ClientRectangle;

            if (Image != null)
            {
                SizeF sizef = e.Graphics.MeasureString(Text, Font, rect.Width - 16);
                Rectangle rectImage = new Rectangle(rect.Left + (rect.Width - (int)Math.Ceiling(sizef.Width) - 16) / 2, rect.Top + (rect.Height - 16) / 2, 16, 16);
                if (Enabled)
                    e.Graphics.DrawImage(Image, rectImage, 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel);
                else
                    PT.DrawImageDisabledInRect(e.Graphics, Image, rectImage, BackColor);
                rect.X = rectImage.Right;
                rect.Width = (int)Math.Ceiling(sizef.Width);
            }

            if (!string.IsNullOrEmpty(Text))
            {
                PT.SetHighQualityRender(e.Graphics);
                if (Enabled)
                    e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), rect, PT.SfCenter);
                else
                    ControlPaint.DrawStringDisabled(e.Graphics, Text, Font, this.BackColor, rect, PT.SfCenter);
            }
        }

        protected virtual void DrawBackground(PaintEventArgs e)
        {
            //const int ItemSize = 51;
            //int x = 3 * ItemSize;
            //if (!Enabled)
            //    x = 2 * ItemSize;
            //else if (IsMouseDown)
            //    x = 1 * ItemSize;
            //else if (IsMouseHover)// || IsDefault)
            //    x = 4 * ItemSize;

            //Image image = BackImage.Image;
            //PT.ExpandImage(e.Graphics, image, 8, ClientRectangle, new Rectangle(x, 0, ItemSize, image.Height));

            //if (Focused)
            //{
            //    Rectangle rect = ClientRectangle;
            //    rect.Inflate(-4, -4);
            //    ControlPaint.DrawFocusRectangle(e.Graphics, rect);
            //}
        }

        #region IGlobalBackground Members
        //private Image _GlobalBackgroundImage;
        //private Rectangle _GlobalBackgroundBounds;

        //[Browsable(false)]
        //public bool AllowGlobalBackground
        //{
        //    get { return true; }
        //}

        //[Browsable(false)]
        //public Image GlobalBackgroundImage
        //{
        //    get
        //    {
        //        return _GlobalBackgroundImage;
        //    }
        //}

        //[Browsable(false)]
        //public Rectangle GlobalBackgroundBounds
        //{
        //    get
        //    {
        //        return _GlobalBackgroundBounds;
        //    }
        //}

        //public void SetGlobalBackground(Image backgroundImage, Rectangle backgroundBounds)
        //{
        //    _GlobalBackgroundImage = backgroundImage;
        //    _GlobalBackgroundBounds = backgroundBounds;

        //    GlobalBackground.SetControls(this, this);
        //    Invalidate();
        //}
        private GlobalBackground _GlobalBackground;

        [Browsable(false)]
        public GlobalBackground GlobalBackground
        {
            get
            {
                if (_GlobalBackground == null)
                    _GlobalBackground = new GlobalBackground(this);
                return _GlobalBackground;
            }
        }

        #endregion*/
    }
}
