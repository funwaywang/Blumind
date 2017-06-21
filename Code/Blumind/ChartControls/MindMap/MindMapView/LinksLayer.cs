using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Blumind.Canvas;
using Blumind.ChartControls.Shapes;
using Blumind.Controls.Paint;
using Blumind.Core;
using Blumind.Model;
using Blumind.Model.MindMaps;

namespace Blumind.Controls.MapViews
{
    class LinesLayer : ChartLayer
    {
        const int ControlHandleSize = 4;
        MindMapView _View;
        HitTestResult _HoverObject;
        HitTestResult _SelectedObject;
        HitTestResult PressedObject;
        MouseButtons PressMouseButton;
        BezierLayoutInfo TempLayout;
        BezierPoint DragingControlHandle;
        Pen PenLine = Pens.Gray;
        Color ControlPointColor = Color.Yellow;
        Color EndPointColor = Color.Lime;
        Topic _NewLineFrom;
        Point MousePoint;

        public LinesLayer(MindMapView chart)
            : base(chart)
        {
            View = chart;
        }

        public MindMap Map
        {
            get
            {
                if (View != null)
                    return View.Map;
                else
                    return null;
            }
        }

        public MindMapView View
        {
            get { return _View; }
            private set { _View = value; }
        }

        private HitTestResult HoverObject
        {
            get { return _HoverObject; }
            set 
            {
                if (_HoverObject != value)
                {
                    HitTestResult old = _HoverObject;
                    _HoverObject = value;
                    OnHoverObjectChanged(old);
                }
            }
        }

        private HitTestResult SelectedObject
        {
            get { return _SelectedObject; }
            set 
            {
                if (_SelectedObject != value)
                {
                    HitTestResult old = _SelectedObject;
                    _SelectedObject = value;
                    OnSelectedObjectChanged(old);
                }
            }
        }

        public Topic NewLineFrom
        {
            get { return _NewLineFrom; }
            set
            {
                if (_NewLineFrom != value)
                {
                    _NewLineFrom = value;
                    OnNewLineFromChanged();
                }
            }
        }

        private void OnHoverObjectChanged(HitTestResult old)
        {
            if (!old.IsEmpty)
            {
                InvalidateLink(old.Link);
            }

            if (!HoverObject.IsEmpty)
            {
                InvalidateLink(HoverObject.Link);

                /*if (!string.IsNullOrEmpty(HoverObject.Link.ToolTip))
                {
                    View.ShowToolTip(HoverObject.Link);
                }
                else
                {
                    View.HideToolTip();
                }*/
            }
            else
            {
                View.HideToolTip();
            }
        }

        private void OnSelectedObjectChanged(HitTestResult old)
        {
            if (!old.IsEmpty)
            {
                old.Link.PropertyChanged -= new Blumind.Core.PropertyChangedEventHandler(Line_Changed);
                InvalidateLink(old.Link, true);
            }

            if (!SelectedObject.IsEmpty)
            {
                TempLayout = SelectedObject.Link.LayoutData.Clone();
                View.Select(SelectedObject.Link);

                SelectedObject.Link.PropertyChanged += new Blumind.Core.PropertyChangedEventHandler(Line_Changed);
                InvalidateLink(SelectedObject.Link, true);
            }
            else
            {
                TempLayout = null;
            }
        }

        public override void OnDoubleClick(HandledEventArgs e)
        {
            base.OnDoubleClick(e);

            if (!SelectedObject.IsEmpty)
            {
                View.EditObject(SelectedObject.Link);
                e.Handled = true;
            }
        }

        public override void OnMouseDown(ExMouseEventArgs e)
        {
            base.OnMouseDown(e);

            PressMouseButton = e.Button;
            PressedObject = HitTest(e.X, e.Y);
            if (e.Button == MouseButtons.Left && PressedObject.Link != null && Helper.TestModifierKeys(Keys.Shift))
            {
                e.Suppress = true;
            }
            else
            {
                SelectedObject = HitTest(e.X, e.Y);

                DragingControlHandle = BezierPoint.None;
                if (!SelectedObject.IsEmpty || NewLineFrom != null)
                {
                    e.Suppress = true;
                }
            }
        }

        public override void OnMouseUp(ExMouseEventArgs e)
        {
            base.OnMouseUp(e);

            PressMouseButton = MouseButtons.None;
            //MouseDownObject = HitTestResult.Empty;

            if (NewLineFrom != null)
            {
                EndNewLineTest(e);
                e.Suppress = true;
            }
            else if (e.Button == MouseButtons.Left && PressedObject.Link != null && Helper.TestModifierKeys(Keys.Shift))
            {
                if (!string.IsNullOrEmpty(PressedObject.Link.Hyperlink))
                    Helper.OpenUrl(PressedObject.Link.Hyperlink);
            }
            else
            {
                if (DragingControlHandle != BezierPoint.None && TempLayout != null && SelectedObject.Link != null)
                {
                    Link line = SelectedObject.Link;
                    var oldPoints = line.GetBezierPoints();
                    //Rectangle rect = line.Bounds;

                    Point pt = View.PointToLogic(new Point(e.X, e.Y));
                    Point pts;
                    Topic topic;
                    switch (DragingControlHandle)
                    {
                        case BezierPoint.StartPoint:
                            topic = View.GetTopicAt(e.X, e.Y);
                            if (line.CanStartFrom(topic))
                                line.From = topic;
                            break;
                        case BezierPoint.EndPoint:
                            topic = View.GetTopicAt(e.X, e.Y);
                            if (line.CanEndTo(topic))
                                line.Target = topic;
                            break;
                        case BezierPoint.ControlPoint1:
                            pts = PaintHelper.CenterPoint(line.LayoutData.StartBounds);
                            //line.LayoutData.CPLength1 = PaintHelper.GetDistance(pts, pt);
                            //line.LayoutData.CPAngle1 = PaintHelper.GetAngle(pts, pt);
                            line.LayoutData.CP1 = new BezierControlPoint(PaintHelper.GetAngle(pts, pt), PaintHelper.GetDistance(pts, pt));
                            break;
                        case BezierPoint.ControlPoint2:
                            pts = PaintHelper.CenterPoint(line.LayoutData.EndBounds);
                            //line.LayoutData.CPLength2 = PaintHelper.GetDistance(pts, pt);
                            //line.LayoutData.CPAngle2 = PaintHelper.GetAngle(pts, pt);
                            line.LayoutData.CP2 = new BezierControlPoint(PaintHelper.GetAngle(pts, pt), PaintHelper.GetDistance(pts, pt));
                            break;
                    }

                    line.RefreshLayout();
                    line.SetChanged();
                    TempLayout = line.LayoutData.Clone();

                    InvalidateLink(oldPoints, line.Width, true);
                    InvalidateLink(line, false);
                    //InvalidateLinkRegion(oldPoints, line.GetBezierPoints(), line.LineWidth);

                    /*if (rect.IsEmpty)
                        rect = line.Bounds;
                    else
                        rect = Rectangle.Union(rect, line.Bounds);
                    rect.Location = View.PointToReal(rect.Location);
                    InvalidateChart(rect, true);*/
                }

                DragingControlHandle = BezierPoint.None;
            }
        }

        public override void OnMouseMove(ExMouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (NewLineFrom != null)
            {
                Point pt = PaintHelper.CenterPoint(NewLineFrom.Bounds);
                pt = View.PointToReal(pt);
                Rectangle rect = PaintHelper.GetRectangle(pt, MousePoint);
                rect = Rectangle.Union(rect, PaintHelper.GetRectangle(pt, new Point(e.X, e.Y)));

                MousePoint.X = e.X;
                MousePoint.Y = e.Y;

                InvalidateChart(rect, false);
            }
            else
            {
                // test - is drag control handle
                if (PressMouseButton == MouseButtons.Left
                    && SelectedObject.ControlHandle != BezierPoint.None)
                {
                    DragControlHandle(SelectedObject.ControlHandle, new Point(e.X, e.Y));

                    e.Suppress = true;
                    if (View.Cursor != Cursors.SizeAll)
                        View.Cursor = Cursors.SizeAll;
                }
                else
                {
                    HoverObject = HitTest(e.X, e.Y);
                    // test - is open url
                    if (HoverObject.Link != null && Helper.TestModifierKeys(Keys.Shift) && !string.IsNullOrEmpty(HoverObject.Link.Hyperlink))
                    {
                        e.Suppress = true;
                        if (View.Cursor != Cursors.Hand)
                            View.Cursor = Cursors.Hand;
                    }
                    else if (HoverObject.ControlHandle != BezierPoint.None)
                    {
                        if (View.Cursor != Cursors.SizeAll)
                            View.Cursor = Cursors.SizeAll;
                        e.Suppress = true;
                    }
                    //else
                    //View.Cursor = Cursors.Default;
                }
            }
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Escape)
            {
                if (NewLineFrom != null)
                {
                    NewLineFrom = null;
                }
            }
        }

        void OnNewLineFromChanged()
        {
            View.InvalidateChart(true);
        }

        void EndNewLineTest(ExMouseEventArgs e)
        {
            Point pt = new Point(e.X - View.TranslatePoint.X, e.Y - View.TranslatePoint.Y);
            Topic topic = View.GetTopicAt(e.X, e.Y);
            if (topic != null && topic != NewLineFrom)
            {
                Link link = Link.CreateNew(NewLineFrom, topic);
                if (link != null)
                {
                    link.RefreshLayout();
                    View.Select(link);
                    SelectedObject = new HitTestResult(link, BezierPoint.None);
                }
            }

            NewLineFrom = null;
        }

        void DragControlHandle(BezierPoint controlHandle, Point pt)
        {
            if (TempLayout != null && SelectedObject.Link != null)
            {
                Rectangle rect = TempLayout.MaxRectangle;

                Link line = SelectedObject.Link;
                var oldPoints = line.GetBezierPoints();
                pt = View.PointToLogic(pt);
                switch (controlHandle)
                {
                    case BezierPoint.StartPoint:
                        TempLayout.StartPoint = pt;
                        TempLayout.ControlPoint1 = BezierHelper.GetControlPoint(TempLayout.StartPoint, TempLayout.CP1);
                        break;
                    case BezierPoint.EndPoint:
                        TempLayout.EndPoint = pt;
                        TempLayout.ControlPoint2 = BezierHelper.GetControlPoint(TempLayout.EndPoint, TempLayout.CP2);
                        break;
                    case BezierPoint.ControlPoint1:
                        TempLayout.ControlPoint1 = pt;
                        Shape sShape = Shape.GetShaper(line.From);
                        TempLayout.StartPoint = sShape.GetBorderPoint(line.From.Bounds, TempLayout.ControlPoint1);
                        break;
                    case BezierPoint.ControlPoint2:
                        TempLayout.ControlPoint2 = pt;
                        Shape dShape = Shape.GetShaper(line.Target);
                        TempLayout.EndPoint = dShape.GetBorderPoint(line.Target.Bounds, TempLayout.ControlPoint2);
                        break;
                    default:
                        break;
                }

                //rect = Rectangle.Union(rect, TempLayout.MaxRectangle);
                //InvalidateChart(rect, true);
                InvalidateLink(oldPoints, line.Width, true);
                InvalidateLink(TempLayout.GetPoints(), line.Width, true);
            }

            DragingControlHandle = controlHandle;
        }

        public override void DrawRealTime(PaintEventArgs e)
        {
            base.DrawRealTime(e);

            if (Map == null)
                return;

            if (NewLineFrom != null)
            {
                DrawNewLineTest(NewLineFrom, e);
            }
            else
            {
                GraphicsState gs = View.TranslateGraphics(e.Graphics);

                if (!HoverObject.IsEmpty && HoverObject.Link.Visible)
                {
                    Color color = HoverObject.Link.Color.IsEmpty ? Map.LinkLineColor : HoverObject.Link.Color;
                    Pen pen = new Pen(Color.FromArgb(100, color), HoverObject.Link.LineWidth + 2);
                    //pen.DashStyle = line.LineStyle;
                    HoverObject.Link.StartCap.TrySetStart(pen);
                    HoverObject.Link.EndCap.TrySetEnd(pen);

                    var layout = HoverObject.Link.LayoutData;

                    e.Graphics.DrawBezier(pen, layout.StartPoint, layout.ControlPoint1, layout.ControlPoint2, layout.EndPoint);
                }

                if (!SelectedObject.IsEmpty && TempLayout != null && SelectedObject.Link.Visible)
                {
                    var layout = TempLayout;
                    Link line = SelectedObject.Link;

                    e.Graphics.DrawLine(PenLine, layout.ControlPoint1, layout.StartPoint);
                    e.Graphics.DrawLine(PenLine, layout.ControlPoint2, layout.EndPoint);

                    if (DragingControlHandle != BezierPoint.None)
                    {
                        Color color = line.Color.IsEmpty ? Map.LinkLineColor : line.Color;
                        Pen pen = new Pen(Color.FromArgb(100, color), line.LineWidth + 2);
                        //pen.DashStyle = DashStyle.DashDot;
                        line.StartCap.TrySetStart(pen);
                        line.EndCap.TrySetEnd(pen);
                        e.Graphics.DrawBezier(pen, layout.StartPoint, layout.ControlPoint1, layout.ControlPoint2, layout.EndPoint);
                    }

                    // control-handles
                    DrawControlHandle(e.Graphics, layout.StartPoint, BezierPoint.StartPoint);
                    DrawControlHandle(e.Graphics, layout.EndPoint, BezierPoint.EndPoint);
                    DrawControlHandle(e.Graphics, layout.ControlPoint1, BezierPoint.ControlPoint1);
                    DrawControlHandle(e.Graphics, layout.ControlPoint2, BezierPoint.ControlPoint2);
                }

                e.Graphics.Restore(gs);
            }
        }

        void DrawNewLineTest(Topic from, PaintEventArgs e)
        {
            Point pt = PaintHelper.CenterPoint(from.Bounds);
            pt = View.PointToReal(pt);

            Pen pen = new Pen(Color.FromArgb(100, Map.LinkLineColor), 3);
            pen.StartCap = LineCap.RoundAnchor;
            pen.EndCap = LineCap.ArrowAnchor;

            e.Graphics.DrawLine(pen, pt, MousePoint);
        }

        void DrawControlHandle(Graphics graphics, Point point, BezierPoint controlHandle)
        {
            Color color = Color.Black;

            switch (controlHandle)
            {
                case BezierPoint.StartPoint:
                case BezierPoint.EndPoint:
                    color = EndPointColor;
                    break;
                case BezierPoint.ControlPoint1:
                case BezierPoint.ControlPoint2:
                    color = ControlPointColor;
                    break;
            }

            bool hover = controlHandle == HoverObject.ControlHandle;
            Brush brush =  new SolidBrush(hover ? color : Color.FromArgb(180, color));
            PaintHelper.FillDot(graphics, brush, point, ControlHandleSize);
            PaintHelper.DrawDot(graphics, PenLine, point, ControlHandleSize);
        }

        void Line_Changed(object sender, Blumind.Core.PropertyChangedEventArgs e)
        {
            if (!SelectedObject.IsEmpty)
            {
                if (e.HasChanges(ChangeTypes.Layout) || e.HasChanges(ChangeTypes.Visual))
                {
                    TempLayout = SelectedObject.Link.LayoutData;
                    InvalidateChart(SelectedObject.Link.Bounds, true);
                }
            }
        }

        HitTestResult HitTest(int x, int y)
        {
            if (Map != null)
            {
                Point pt = View.PointToLogic(x, y);

                Link[] links = Map.GetLinks(true);
                foreach(Link line in links)
                {
                    if (!line.Visible)
                        continue;

                    var layout = line.LayoutData;
                    if (line == SelectedObject.Link)
                    {
                        if (InDotTest(layout.ControlPoint1, ControlHandleSize, pt))
                            return new HitTestResult(line, BezierPoint.ControlPoint1);
                        else if (InDotTest(layout.ControlPoint2, ControlHandleSize, pt))
                            return new HitTestResult(line, BezierPoint.ControlPoint2);
                        else if (InDotTest(layout.StartPoint, ControlHandleSize, pt))
                            return new HitTestResult(line, BezierPoint.StartPoint);
                        else if (InDotTest(layout.EndPoint, ControlHandleSize, pt))
                            return new HitTestResult(line, BezierPoint.EndPoint);
                    }

                    if ((layout.Bounds.Contains(pt) && layout.Region.IsVisible(pt)) || layout.TextBounds.Contains(pt))
                    {
                        return new HitTestResult(line, BezierPoint.None);
                    }
                }
            }

            return HitTestResult.Empty;
        }

        bool InDotTest(Point ptCenter, int dotSize, Point ptTest)
        {
            return ptTest.X > (ptCenter.X - dotSize) 
                && ptTest.X < (ptCenter.X + dotSize) 
                && ptTest.Y > (ptCenter.Y - dotSize) 
                && ptTest.Y < (ptCenter.Y + dotSize);
        }

        public void InvalidateChart(Rectangle rect, bool toreal)
        {
            const int inflate = 10;
            rect.Inflate(inflate, inflate);
            if (toreal)
            {
                rect = View.RectangleToReal(rect);
            }

            Invalidate(rect);
        }

        protected void InvalidateLink(Link link, bool realTime)
        {
            if (link == null)
                return;

            var region = BezierHelper.GetBezierUpdateRegionWidthHandles(
                View.PointToReal(link.LayoutData.StartPoint),
                View.PointToReal(link.LayoutData.ControlPoint1),
                View.PointToReal(link.LayoutData.ControlPoint2),
                View.PointToReal(link.LayoutData.EndPoint),
                link.Width);
            if (region != null)
            {
                InvalidateChart(region, realTime);
            }
        }

        protected void InvalidateLink(Link link)
        {
            InvalidateLink(link, false);
        }

        protected void InvalidateLink(Point[] points, int width, bool realTime)
        {
            if (points == null || points.Length != 4)
                return;

            var region = BezierHelper.GetBezierUpdateRegion(
                View.PointToReal(points[0]),
                View.PointToReal(points[1]),
                View.PointToReal(points[2]),
                View.PointToReal(points[3]),
                width);

            InvalidateChart(region, realTime);
        }

        struct HitTestResult
        {
            public Link Link;
            public BezierPoint ControlHandle;
            public static readonly HitTestResult Empty = new HitTestResult();

            public HitTestResult(Link link, BezierPoint controlHandle)
            {
                Link = link;
                ControlHandle = controlHandle;
            }

            public bool IsEmpty
            {
                get
                {
                    return Link == null && ControlHandle == BezierPoint.None;
                }
            }

            public static bool operator !=(HitTestResult htr1, HitTestResult htr2)
            {
                return htr1.Link != htr2.Link
                    || htr1.ControlHandle != htr2.ControlHandle ;
            }

            public static bool operator ==(HitTestResult htr1, HitTestResult htr2)
            {
                return htr1.Link == htr2.Link
                    && htr1.ControlHandle == htr2.ControlHandle;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is HitTestResult))
                    return false;
                else
                    return this == (HitTestResult)obj;
            }

            public override int GetHashCode()
            {
                if (Link != null)
                    return Link.GetHashCode() + ControlHandle.GetHashCode();
                else
                    return 0;
            }
        }
    }
}
