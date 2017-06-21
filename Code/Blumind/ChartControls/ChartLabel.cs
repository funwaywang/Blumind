using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Blumind.Controls
{
    public class ChartLabel
    {
        private string _Name = null;
        private string _Text = null;
        private Point _Location = Point.Empty;
        private Size _Size = Size.Empty;
        private ContentAlignment _Alignment = ContentAlignment.TopLeft;
        private bool _AutoSize = true;
        private Font _Font = null;
        private bool _Visible = true;
        private Nullable<Color> _ForeColor = null;
        private Nullable<Color> _BackColor = null;

        public event System.EventHandler NeedPaint;

        public ChartLabel()
        {
        }

        public ChartLabel(string text)
        {
            Text = text;
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public string Text
        {
            get { return _Text; }
            set 
            {
                if (_Text != value)
                {
                    _Text = value;
                    OnTextChanged();
                }
            }
        }

        public Point Location
        {
            get { return _Location; }
            set
            {
                if (_Location != value)
                {
                    _Location = value;
                    OnLocationChanged();
                }
            }
        }

        public Size Size
        {
            get { return _Size; }
            set
            {
                if (_Size != value)
                {
                    _Size = value;
                    OnSizeChanged();
                }
            }
        }

        public ContentAlignment Alignment
        {
            get { return _Alignment; }
            set 
            {
                if (_Alignment != value)
                {
                    _Alignment = value;
                    OnAlignmentChanged();
                }
            }
        }

        public bool AutoSize
        {
            get { return _AutoSize; }
            set 
            {
                if (_AutoSize != value)
                {
                    _AutoSize = value;
                    OnAutoSizeChanged();
                }
            }
        }

        public Font Font
        {
            get { return _Font; }
            set 
            {
                if (_Font != value)
                {
                    _Font = value;
                    OnFontChanged();
                }
            }
        }

        public Nullable<Color> ForeColor
        {
            get { return _ForeColor; }
            set 
            {
                if (_ForeColor != value)
                {
                    _ForeColor = value;
                    OnForeColorChanged();
                }
            }
        }

        public Nullable<Color> BackColor
        {
            get { return _BackColor; }
            set 
            {
                if (_BackColor != value)
                {
                    _BackColor = value;
                    OnBackColorChanged();
                }
            }
        }

        public bool Visible
        {
            get { return _Visible; }
            set 
            {
                if (_Visible != value)
                {
                    _Visible = value;
                    OnVisibleChanged();
                }
            }
        }

        public void Invalidate()
        {
            if (NeedPaint != null)
            {
                NeedPaint(this, EventArgs.Empty);
            }
        }

        #region method of properties changed
        private void OnTextChanged()
        {
            Invalidate();
        }

        private void OnLocationChanged()
        {
            Invalidate();
        }

        private void OnSizeChanged()
        {
            Invalidate();
        }

        private void OnAlignmentChanged()
        {
            Invalidate();
        }

        private void OnAutoSizeChanged()
        {
            Invalidate();
        }

        private void OnFontChanged()
        {
            Invalidate();
        }

        private void OnForeColorChanged()
        {
            Invalidate();
        }

        private void OnBackColorChanged()
        {
            Invalidate();
        }

        private void OnGlobalSpaceChanged()
        {
            Invalidate();
        }

        private void OnVisibleChanged()
        {
            Invalidate();
        }

        #endregion
    }
}
