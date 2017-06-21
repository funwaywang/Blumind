using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Blumind.Controls
{
    class ChartLayer
    {
        Chart _Owner;

        public ChartLayer(Chart owner)
        {
            Owner = owner;
        }

        public Chart Owner
        {
            get { return _Owner; }
            private set { _Owner = value; }
        }

        public Font Font
        {
            get
            {
                if (Owner != null)
                    return Owner.Font;
                else
                    return null;
            }
        }

        public virtual void OnMouseMove(ExMouseEventArgs e)
        {
        }

        public virtual void OnMouseDown(ExMouseEventArgs e)
        {
        }

        public virtual void OnMouseUp(ExMouseEventArgs e)
        {
        }

        public virtual void OnDoubleClick(HandledEventArgs e)
        {
        }

        public virtual void OnKeyDown(KeyEventArgs e)
        {
        }

        public virtual void OnKeyUp(KeyEventArgs e)
        {
        }

        public virtual void Draw(PaintEventArgs e)
        {
        }

        public virtual void DrawRealTime(PaintEventArgs e)
        {
        }

        public void Invalidate(Rectangle rect)
        {
            if (Owner != null)
            {
                Owner.InvalidateChart(rect, true);
                //Owner.InvalidateChart();
            }
        }

        public void InvalidateChart(Region region)
        {
            if (Owner != null)
            {
                Owner.InvalidateChart(region, true);
            }
        }

        public void InvalidateChart(Region region, bool realTime)
        {
            if (Owner != null)
            {
                Owner.InvalidateChart(region, realTime);
            }
        }
    }
}
