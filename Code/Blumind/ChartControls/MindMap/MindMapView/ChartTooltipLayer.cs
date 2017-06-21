using System;
using System.Drawing;
using System.Windows.Forms;
using Blumind.Core;

namespace Blumind.Controls.MapViews
{
    class ChartTooltipLayer : ChartLayer
    {
        private XList<ChartToolTip> _ToolTips;
        private Timer TheTimer;

        public ChartTooltipLayer(Chart owner)
            : base(owner)
        {
            ToolTips = new XList<ChartToolTip>();
            ToolTips.ItemAdded += new XListEventHandler<ChartToolTip>(ToolTips_ItemAdded);
            ToolTips.ItemRemoved += new XListEventHandler<ChartToolTip>(ToolTips_ItemRemoved);

            TheTimer = new Timer();
            TheTimer.Interval = 100;
            TheTimer.Tick += new EventHandler(TheTimer_Tick);
        }

        public XList<ChartToolTip> ToolTips
        {
            get { return _ToolTips; }
            private set { _ToolTips = value; }
        }

        public ChartToolTip CreateToolTip()
        {
            return new ChartToolTip(this);
        }

        public ChartToolTip ShowToolTip(int x, int y, string text)
        {
            ChartHyperlinkToolTip toolTip = new ChartHyperlinkToolTip(this);
            toolTip.Text = text;
            ShowToolTip(x, y, toolTip);
            return toolTip;
        }

        public void ShowToolTip(int x, int y, ChartToolTip toolTip)
        {
            toolTip.Location = new Point(x, y);
            ToolTips.Add(toolTip);
        }

        private void ToolTips_ItemRemoved(object sender, XListEventArgs<ChartToolTip> e)
        {
            Invalidate(e.Item.Bounds);
            
            TheTimer.Enabled = HasAnyVisibleToolTips();
        }

        private void ToolTips_ItemAdded(object sender, XListEventArgs<ChartToolTip> e)
        {
            Invalidate(e.Item.Bounds);
            e.Item.VisibleChanged += new EventHandler(Item_VisibleChanged);

            TheTimer.Enabled = HasAnyVisibleToolTips(); ;
        }

        private void Item_VisibleChanged(object sender, EventArgs e)
        {
            TheTimer.Enabled = HasAnyVisibleToolTips();
        }

        private bool HasAnyVisibleToolTips()
        {
            foreach (ChartToolTip toolTip in ToolTips)
            {
                if (toolTip.Visible)
                {
                    return true;
                }
            }

            return false;
        }

        private void TheTimer_Tick(object sender, EventArgs e)
        {
            foreach (ChartToolTip toolTip in ToolTips)
            {
                if (toolTip.Visible)
                {
                    toolTip.OnTimerTick(TheTimer.Interval);
                }
            }
        }

        public override void DrawRealTime(PaintEventArgs e)
        {
            base.DrawRealTime(e);

            Draw(e);
        }

        public override void Draw(PaintEventArgs e)
        {
            base.Draw(e);

            foreach (ChartToolTip toolTip in ToolTips)
            {
                if (toolTip.Visible)
                {
                    toolTip.Draw(e);
                }
            }
        }

        internal Size MeasureString(string text, Size maximumSize, StringFormat stringFormat)
        {
            if (Owner != null && Owner.Created)
            {
                using (Graphics grf = Owner.CreateGraphics())
                {
                    return Size.Ceiling(grf.MeasureString(text, Owner.Font, maximumSize, stringFormat));
                }
            }

            return TextRenderer.MeasureText(text, Owner.Font, maximumSize);
        }

        public override void OnMouseDown(ExMouseEventArgs e)
        {
            base.OnMouseDown(e);

            foreach (ChartToolTip tooltip in ToolTips)
            {
                if (tooltip.Visible && tooltip.Bounds.Contains(e.X, e.Y))
                {
                    tooltip.OnMouseDown(e);
                    e.Suppress = true;
                }
            }
        }

        public override void OnMouseMove(ExMouseEventArgs e)
        {
            base.OnMouseMove(e);

            foreach (ChartToolTip tooltip in ToolTips)
            {
                if (tooltip.Visible && tooltip.Bounds.Contains(e.X, e.Y))
                {
                    tooltip.OnMouseMove(e);
                    e.Suppress = true;
                }
                else
                {
                    tooltip.IsMouseHover = false;
                }
            }
        }

        public override void OnMouseUp(ExMouseEventArgs e)
        {
            base.OnMouseUp(e);

            foreach (ChartToolTip tooltip in ToolTips)
            {
                if (tooltip.Visible && tooltip.Bounds.Contains(e.X, e.Y))
                {
                    tooltip.OnMouseUp(e);
                    e.Suppress = true;
                }
            }
        }

        public void HideAllToolTips()
        {
            foreach (ChartToolTip tooltip in ToolTips)
            {
                if (tooltip.Visible)
                {
                    tooltip.Hide(false);
                }
            }
        }
    }
}
