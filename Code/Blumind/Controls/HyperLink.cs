using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace Blumind.Controls
{
    class HyperLink : System.Windows.Forms.Label
    {
        private Color _HoverColor = Color.MediumBlue;
        private bool _IsHover;
        private string _LinkUrl;
        private bool IsMouseDown;
        private Rectangle _TextBounds;

        public HyperLink()
        {
        }

        [DefaultValue(typeof(Color), "MediumBlue")]
        public Color HoverColor
        {
            get { return _HoverColor; }
            set { _HoverColor = value; }
        }

        private bool IsHover
        {
            get { return _IsHover; }
            set 
            {
                if (_IsHover != value)
                {
                    _IsHover = value;
                    OnIsHoverChanged();
                }
            }
        }

        [DefaultValue(null)]
        public string LinkUrl
        {
            get { return _LinkUrl; }
            set { _LinkUrl = value; }
        }

        private Rectangle TextBounds
        {
            get { return _TextBounds; }
            set 
            {
                if (_TextBounds != value)
                {
                    _TextBounds = value;
                    OnTextBoundsChanged();
                }
            }
        }

        private void OnTextBoundsChanged()
        {
            Invalidate();
        }

        private void OnIsHoverChanged()
        {
            if (IsHover)
                Cursor = Cursors.Hand;
            else
                Cursor = Cursors.Default;
            Invalidate();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            MeasureTextBounds();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            MeasureTextBounds();
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            MeasureTextBounds();
        }

        private void MeasureTextBounds()
        {
            if (!Created)
                return;

            if (string.IsNullOrEmpty(Text))
            {
                TextBounds = Rectangle.Empty;
            }
            else
            {
                Size size;
                using (Graphics grf = CreateGraphics())
                {
                    size = Size.Ceiling(grf.MeasureString(Text, Font));
                    grf.Dispose();
                }

                int x = 0;
                int y = 0;
                switch (TextAlign)
                {
                    case ContentAlignment.TopCenter:
                        x = (ClientSize.Width - size.Width) / 2;
                        break;
                    case ContentAlignment.TopRight:
                        x = ClientSize.Width - size.Width;
                        break;
                    case ContentAlignment.MiddleLeft:
                        y = (ClientSize.Height - size.Height) / 2;
                        break;
                    case ContentAlignment.MiddleCenter:
                        x = (ClientSize.Width - size.Width) / 2;
                        y = (ClientSize.Height - size.Height) / 2;
                        break;
                    case ContentAlignment.MiddleRight:
                        x = ClientSize.Width - size.Width;
                        y = (ClientSize.Height - size.Height) / 2;
                        break;
                    case ContentAlignment.BottomLeft:
                        y = ClientSize.Height - size.Height;
                        break;
                    case ContentAlignment.BottomCenter:
                        x = (ClientSize.Width - size.Width) / 2;
                        y = ClientSize.Height - size.Height;
                        break;
                    case ContentAlignment.BottomRight:
                        x = ClientSize.Width - size.Width;
                        y = ClientSize.Height - size.Height;
                        break;
                    case ContentAlignment.TopLeft:
                    default:
                        break;
                }

                TextBounds = new Rectangle(x, y, size.Width, size.Height);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            IsHover = TextBounds.Contains(e.X, e.Y);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            IsHover = false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (IsHover)
            {
                IsMouseDown = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (IsMouseDown)
            {
                IsMouseDown = false;

                if (LinkUrl != null && TextBounds.Contains(e.X, e.Y) && e.Button == MouseButtons.Left)
                {
                    Helper.OpenUrl(LinkUrl);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (BackColor.A < 255)
            {
                InvokePaintBackground(this, e);
            }
            else
            {
                e.Graphics.Clear(BackColor);
            }

            Color color = IsHover ? HoverColor : ForeColor;
            e.Graphics.DrawString(Text, Font, new SolidBrush(color), ClientRectangle, PaintHelper.TranslateContentAlignment(TextAlign));
        }
    }
}
