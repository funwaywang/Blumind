using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Blumind.Controls
{
    class SelectionLayer : ChartLayer
    {
        private Point _StartPoint;
        private Point _EndPoint;
        private Rectangle _Bounds;
        private Color _Color = SystemColors.Highlight;
        private bool _Active;

        public SelectionLayer(Chart owner)
            : base(owner)
        {
        }

        public Rectangle Bounds
        {
            get { return _Bounds; }
            private set { _Bounds = value; }
        }

        public Color Color
        {
            get { return _Color; }
            set
            {
                if (_Color != value)
                {
                    _Color = value;
                    OnColorChanged();
                }
            }
        }

        public bool Active
        {
            get { return _Active; }
            set 
            {
                if (_Active != value)
                {
                    _Active = value;
                    OnActiveChanged();
                }
            }
        }

        public Point StartPoint
        {
            get { return _StartPoint; }
            private set { _StartPoint = value; }
        }

        public Point EndPoint
        {
            get { return _EndPoint; }
            private set { _EndPoint = value; }
        }

        private void OnColorChanged()
        {
            if (Active)
            {
                Invalidate(Bounds);
            }
        }

        private void OnActiveChanged()
        {
            Invalidate(Bounds);
            if (Active)
            {
                StartPoint = GetMousePoint();
                Bounds = Rectangle.Empty;
            }
            else
            {
                Bounds = GetBounds();
                StartPoint = Point.Empty;
            }

            EndPoint = StartPoint;
        }

        private Point GetMousePoint()
        {
            return Owner.PointToChartBox(Control.MousePosition);
        }

        private Rectangle GetBounds()
        {
            return new Rectangle(
                Math.Min(StartPoint.X, EndPoint.X),
                Math.Min(StartPoint.Y, EndPoint.Y),
                Math.Abs(StartPoint.X - EndPoint.X),
                Math.Abs(StartPoint.Y - EndPoint.Y));
        }

        public override void DrawRealTime(PaintEventArgs e)
        {
            //base.DrawRealTime(e);
            if (Active)
            {
                PixelOffsetMode pom = e.Graphics.PixelOffsetMode;
                SmoothingMode sm = e.Graphics.SmoothingMode;
                e.Graphics.SmoothingMode = SmoothingMode.Default;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.Default;
                
                //
                Rectangle rect = GetBounds();
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, Color)), rect);
                e.Graphics.DrawRectangle(new Pen(Color), rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

                e.Graphics.SmoothingMode = sm;
                e.Graphics.PixelOffsetMode = pom;
            }
        }

        public override void OnMouseMove(ExMouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (Active)
            {
                Rectangle rect = GetBounds();
                EndPoint = new Point(e.X, e.Y);
                rect = Rectangle.Union(rect, GetBounds());
                rect.Inflate(10, 10);
                Invalidate(rect);
            }
        }

        public override void OnMouseUp(ExMouseEventArgs e)
        {
            base.OnMouseUp(e);

            Active = false;
        }
    }
}
