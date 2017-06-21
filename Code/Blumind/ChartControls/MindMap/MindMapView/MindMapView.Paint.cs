using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Windows.Forms;
using Blumind.Configuration;
using Blumind.Core;
using Blumind.Model;

namespace Blumind.Controls.MapViews
{
    partial class MindMapView
    {
        public override bool CustomDoubleBuffer
        {
            get
            {
                return true;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (ShowBorder && Padding.Left > 0)
            {
                Rectangle rect = ClientRectangle;
                /*PT.SetHighQualityRender(e.Graphics);
                e.Graphics.Clear(this.BackColor);
                GlobalBackground.Draw(e.Graphics);
                rect.Inflate(0, 1);
                GraphicsPath gp = PT.GetRoundRectangle(rect, Padding.Left * 2);

                e.Graphics.FillPath(new SolidBrush(ChartBackColor), gp);
                e.Graphics.DrawPath(new Pen(BorderColor), gp);*/
                e.Graphics.Clear(ChartBackColor);
                e.Graphics.DrawRectangle(new Pen(BorderColor), 0, 0, rect.Width - 1, rect.Height - 1);
            }
            else
            {
                base.OnPaint(e);
            }
        }

        protected override void DrawChart(ChartPaintEventArgs e)
        {
            e.Graphics.Clear(ChartBackColor);

            // transfter
            Rectangle viewPort = ViewPort;
            Point ptTran = Point.Empty;
            SizeF size = ContentSize;// new SizeF(ContentSize.Width * Zoom, ContentSize.Height * Zoom);
            if (!HorizontalScroll.Enabled && viewPort.Width > ContentSize.Width)
            {
                ptTran.X = (int)Math.Ceiling((viewPort.Width - size.Width) / 2);
            }
            if (!VerticalScroll.Enabled && viewPort.Height > ContentSize.Height)
            {
                ptTran.Y = (int)Math.Ceiling((viewPort.Height - size.Height) / 2);
            }

            if (!ptTran.IsEmpty && Zoom > 0)
            {
                e.Graphics.TranslateTransform(ptTran.X / Zoom, ptTran.Y / Zoom);
                TranslatePoint = ptTran;
            }
            else
            {
                TranslatePoint = Point.Empty;
            }

            if (Render != null && Map != null)
            {
                RenderArgs args = new RenderArgs(RenderMode.UserInface, e.Graphics, this, e.Font);
                Render.Paint(Map, args);
            }
        }

        public override void DrawNavigationMap(PaintEventArgs e)
        {
            base.DrawNavigationMap(e);

            Rectangle rect = e.ClipRectangle;
            float zoom = Math.Min((float)rect.Width / ContentSize.Width,
                (float)rect.Height / ContentSize.Height) * Zoom;

            e.Graphics.TranslateTransform(Margin.Left * zoom, Margin.Top * zoom);
            PaintHelper.SetHighQualityRender(e.Graphics);

            Render.PaintNavigationMap(Map, zoom, e);
        }

        protected override void OnAfterPaint(PaintEventArgs e)
        {
            base.OnAfterPaint(e);

            if (DragBox.Visible)
                DragBox.Draw(e);
        }

        //private void InvalidateTopic(Topic topic)
        //{
        //    if (topic != null)
        //    {
        //        Rectangle rect = GetObjectRect(topic);
        //        rect.Inflate(10, 10);
        //        InvalidateChart(rect);
        //    }
        //}

        public override void InvalidateObject(ChartObject obj)
        {
            if (obj != null)
            {
                Rectangle rect = obj.Bounds;
                rect.Inflate(10, 10);
                rect = RectangleToReal(rect);
                InvalidateChart(rect);
            }
        }

        /*void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (Render != null && Map != null)
            {
                float zoom = Math.Min(1, Math.Min((float)e.MarginBounds.Width / OriginalContentSize.Width, (float)e.MarginBounds.Height / OriginalContentSize.Height));
                zoom = (int)(zoom * 1000) / 1000.0f;

                Rectangle rectMargin = PaintHelper.Zoom(e.MarginBounds, zoom);
                RenderArgs args = new RenderArgs(RenderMode.Print, e.Graphics, this, ChartBox.DefaultChartFont);
                //Size size = Render.Layout(Map, args);
                e.Graphics.Clip = new Region(e.MarginBounds);

                // print document title
                if (Options.Current.GetBool(Blumind.Configuration.OptionNames.PageSettigs.PrintDocumentTitle))
                {
                    Point ptTitle = e.MarginBounds.Location;
                    var brush = new SolidBrush(Color.Black);
                    e.Graphics.DrawString(Map.Name, (Font)args.Font.Raw, brush, ptTitle.X, ptTitle.Y);
                }

                // print page context
                SizeF size = new SizeF(OriginalContentSize.Width * zoom, OriginalContentSize.Height * zoom);
                PointF pt = new PointF(e.MarginBounds.Left + Math.Max(0, (e.MarginBounds.Width - size.Width) / 2),
                    e.MarginBounds.Top + Math.Max(0, (e.MarginBounds.Height - size.Height) / 2));

                e.Graphics.TranslateTransform(pt.X, pt.Y);
                e.Graphics.ScaleTransform(zoom, zoom);

                Render.Paint(Map, args);
            }

            e.HasMorePages = false;
        }*/
    }
}
