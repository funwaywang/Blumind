using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Blumind.Controls
{
    class ImageButton : Button
    {
        bool _IsMouseHover;
        bool _IsMouseDown;
        Image _HoverImage;

        protected bool IsMouseHover
        {
            get { return _IsMouseHover; }
            set
            {
                if (_IsMouseHover != value)
                {
                    _IsMouseHover = value;
                    Invalidate();
                }
            }
        }

        protected bool IsMouseDown
        {
            get { return _IsMouseDown; }
            set
            {
                if (_IsMouseDown != value)
                {
                    _IsMouseDown = value;
                    Invalidate();
                }
            }
        }

        [DefaultValue(null)]
        public Image HoverImage
        {
            get { return _HoverImage; }
            set
            {
                if (_HoverImage != value)
                {
                    _HoverImage = value;
                    OnHoverImageChanged();
                }
            }
        }

        protected bool ShowDefaultStatus
        {
            get { return false; }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            IsMouseDown = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            IsMouseDown = false;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            IsMouseHover = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            IsMouseHover = false;
        }

        protected virtual void OnHoverImageChanged()
        {
            if (IsMouseHover)
                Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Image == null)
            {
                base.OnPaint(e);
            }
            else
            {
                InvokePaintBackground(this, e);
                PaintBackground(e);

                Size textSize = Size.Empty;
                Image image = (IsMouseHover && HoverImage != null) ? HoverImage : Image;
                StringFormat sf = new StringFormat();
                sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Hide;
                if (!string.IsNullOrEmpty(Text))
                {
                    textSize = Size.Ceiling(e.Graphics.MeasureString(Text, Font, Width - image.Width, sf));
                }

                Size clientSize = ClientSize;

                // Draw Image
                int totalWidth = image.Width + textSize.Width;
                //int totalHeight = Math.Max(Image.Height, textSize.Height);

                var rectImage = new Rectangle((clientSize.Width - totalWidth) / 2, (clientSize.Height - image.Height) / 2, image.Width, image.Height);

                if (!Enabled)
                {
                    PaintHelper.DrawImageDisabled(e.Graphics,
                        image,
                        rectImage,
                        new Rectangle(0, 0, image.Width, image.Height),
                        BackColor);
                }
                else
                {
                    e.Graphics.DrawImage(image
                        , rectImage
                        , 0, 0, image.Width, image.Height
                        , GraphicsUnit.Pixel);
                }

                // Draw Text
                if (!string.IsNullOrEmpty(Text))
                {
                    e.Graphics.DrawString(Text
                        , Font
                        , new SolidBrush(ForeColor)
                        , (clientSize.Width - totalWidth) / 2 + image.Width
                        , (clientSize.Height - textSize.Height) / 2 + 2
                        , sf);
                }
            }
        }

        protected virtual void PaintBackground(PaintEventArgs e)
        {
            if (VisualStyleRenderer.IsSupported)
            {
                VisualStyleElement element;
                if (!Enabled)
                    element = VisualStyleElement.Button.PushButton.Disabled;
                else if (IsMouseDown)
                    element = VisualStyleElement.Button.PushButton.Pressed;
                else if (IsMouseHover)
                    element = VisualStyleElement.Button.PushButton.Hot;
                else if (IsDefault && ShowDefaultStatus)
                    element = VisualStyleElement.Button.PushButton.Default;
                else
                    element = VisualStyleElement.Button.PushButton.Normal;

                if (VisualStyleRenderer.IsElementDefined(element))
                {
                    VisualStyleRenderer renderer = new VisualStyleRenderer(element);
                    renderer.DrawBackground(e.Graphics, ClientRectangle);
                }
            }
            else
            {
            }
        }
    }
}
