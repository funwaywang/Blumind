using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Blumind.Canvas;
using Blumind.Canvas.GdiPlus;
using Blumind.ChartControls.FillTypes;
using Blumind.ChartControls.MindMap.Lines;
using Blumind.ChartControls.Shapes;
using Blumind.Configuration;
using Blumind.Controls.Paint;
using Blumind.Core;
using Blumind.Model;
using Blumind.Model.MindMaps;
using Blumind.Model.Widgets;

namespace Blumind.Controls.MapViews
{
    class GeneralRender : IMindMapRender
    {
        #region const params
        const int RoundDotSize = 6;
        public const int OutBoxSpace = 3;
        const int LineCapSpace = 5;
        #endregion

        IPen PenLine;
        IPen PenLineNav;
        IPen PenNodeBorder;
        IPen PenRootNodeBorder;
        IPen PenNodeBorderHover;
        IPen PenNodeBorderSelect;
        IPen PenNodeBorderActive;
        IBrush BrushNodeFore;
        IBrush BrushRootNodeBack;
        IBrush BrushRootNodeFore;
        IBrush BrushNodeBack;
        IBrush BrushNodeShadow;

        int ShadowSize = 2;
        ILine Liner;

        public GeneralRender()
        {
            //InitializeRender();
        }

        #region IRender Members

        public void Paint(MindMap map, RenderArgs args)
        {
            if (map == null)
                throw new ArgumentNullException();

            ResetObjects(args.Graphics, map);

            //PaintBackground(map, args);

            if (map.Root != null)
            {
                Liner = LayoutManage.GetLiner(map.LayoutType);

                //GenerateFoldingButtonImage(map);
                PaintNode(map.Root, args);
                PaintLinkLines(map.Root, args);
            }
        }

        public void PaintNavigationMap(MindMap map, float zoom, PaintEventArgs e)
        {
            if (map == null)
                throw new ArgumentNullException();

            var graphics = new GdiGraphics(e.Graphics);
            ResetObjects(graphics, map);

            if (!map.BackColor.IsEmpty)
            {
                e.Graphics.Clear(map.BackColor);
            }

            if (map.Root != null)
            {
                Liner = LayoutManage.GetLiner(map.LayoutType);

                Font font = ChartBox.DefaultChartFont;
                var font2 = new GdiFont(font.FontFamily, font.Size * zoom);

                //GenerateFoldingButtonImage(map, zoom);
                PaintNavigationMap(graphics, map.Root, zoom, font2);

                PaintNavigationMapLinkLines(graphics, map.Root, zoom, font2);
            }
        }

        public void PaintTopic(Topic topic, RenderArgs e)
        {
            Point pt = topic.Location;
            e.Graphics.TranslateTransform(pt.X, pt.Y);
            _PaintNode(topic, e);
            e.Graphics.TranslateTransform(-pt.X, -pt.Y);
        }

        public void PaintTopics(IEnumerable<Topic> topics, RenderArgs e)
        {
            if (topics.IsNullOrEmpty())
                return;

            foreach (var topic in topics)
            {
                var pt = topic.Location;
                e.Graphics.TranslateTransform(pt.X, pt.Y);
                _PaintNode(topic, e);
                e.Graphics.TranslateTransform(-pt.X, -pt.Y);
            }
        }

        #endregion

        #region Paint

        void PaintNode(Topic topic, RenderArgs e)
        {
            if (topic.Bounds.IsEmpty)
                return;

            var rect = topic.Bounds;
            if (topic.Lines.Count > 0)
            {
                if (Liner is BezierLine)
                    ((BezierLine)Liner).IsRoot = topic.IsRoot;
                else if(Liner is HandPaintLine)
                    ((HandPaintLine)Liner).IsRoot = topic.IsRoot;

                LineAnchor sla = LineAnchor.None;
                LineAnchor ela = e.ShowLineArrowCap ? LineAnchor.Arrow : LineAnchor.None;

                foreach (var line in topic.Lines)
                {
                    if (line.Target == null)
                        continue;
                    var rectFrom = line.BeginRectangle;
                    var rectTo = line.EndRectangle;// line.Target.Bounds;
                    //if (e.Layouter != null)
                    //    e.Layouter.AdjustLineRect(line, topic, ref rectFrom, ref rectTo);

                    var linePen = PenLine;
                    if (!line.Target.Style.LineColor.IsEmpty)
                        linePen = GetLinePen(e.Graphics, e.Chart.LineWidth, line.Target.Style.LineColor);

                    Liner.DrawLine(e.Graphics, linePen, topic.Style.Shape, line.Target.Style.Shape,
                        rectFrom, rectTo, line.BeginSide, line.EndSide, sla, ela);
                }
            }

            PaintTopic(topic, e);

            // draw children
            if (!topic.Children.IsEmpty)
            {
                if (!topic.Folded)
                {
                    foreach (Topic subTopic in topic.Children)
                    {
                        PaintNode(subTopic, e);
                    }
                }

                if (!topic.IsRoot)
                {
                    bool hover = e.Mode == RenderMode.UserInface
                        && e.View != null
                        && !e.View.HoverObject.IsEmpty
                        && e.View.HoverObject.IsFoldingButton 
                        && e.View.HoverObject.Topic == topic;
                    PaintFoldingButton(topic, e, topic.Folded, hover);
                }
            }
        }

        // 坐标已经转换, 原点为 Topic.Left/Top
        void _PaintNode(Topic topic, RenderArgs e)
        {
            var style = topic.Style;
            var font = style.Font != null ? e.Graphics.Font(style.Font) : e.Font;

            // draw background
            PaintNodeBackground(topic, e);

            // calculate paint bounds
            var rect = new Rectangle(Point.Empty, topic.Bounds.Size); // 修改参照原点
            rect = rect.Inflate(style.Padding);

            // widgets
            foreach(var widget in topic.Widgets)
            {
                //if (!widget.Visible)
                //    continue;
                widget.Paint(e);
                if (widget.Selectable && widget.Selected)
                {
                    DrawSelectRectangle(e.Graphics, e.Chart.SelectColor, widget.Bounds);
                }
            }

            // draw text
            if (!string.IsNullOrEmpty(topic.Text))
            {
                var rectText = topic.TextBounds;
                //rectText.Offset(topic.Left, topic.Top); // 原点已经在调用时转换
                IBrush brushFore = style.ForeColor.IsEmpty ? BrushNodeFore : (e.Graphics.SolidBrush(style.ForeColor));
                var sf = PaintHelper.SFCenter;
                if (e.Mode == RenderMode.Print && !topic.CustomWidth.HasValue)
                {
                    sf.FormatFlags |= StringFormatFlags.NoWrap;
                    sf.Trimming = StringTrimming.None;
                }
                switch (topic.TextAlignment)
                {
                    case HorizontalAlignment.Left:
                        sf.Alignment = StringAlignment.Near;
                        break;
                    case HorizontalAlignment.Right:
                        sf.Alignment = StringAlignment.Far;
                        break;
                }

                var font2 = font;
                if (!string.IsNullOrEmpty(topic.Hyperlink))
                    font2 = e.Graphics.Font(font, font.Style | FontStyle.Underline);

                e.Graphics.DrawString(topic.Text, font2, brushFore, rectText, sf);
            }

            // draw remark icon 
            if (e.ShowRemarkIcon && topic.HaveRemark)
            {
                var iconRemark = Properties.Resources.note_small;
                var rectRemark = topic.RemarkIconBounds;
                e.Graphics.DrawImage(iconRemark, rectRemark, 0, 0, iconRemark.Width, iconRemark.Height);
            }
        }

        void DrawSelectRectangle(IGraphics graphics, Color color, Rectangle rectangle)
        {
            //Pen pen = new Pen(color);
            rectangle.Inflate(1, 1);
            graphics.DrawRectangle(graphics.Pen(Color.White), rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            rectangle.Inflate(1, 1);
            graphics.DrawRectangle(graphics.Pen(Color.Black), rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        void PaintLinkLines(Topic topic, RenderArgs e)
        {
            // links
            foreach (Link link in topic.Links)
            {
                if (link.Visible)
                    PaintLinkLine(link, e);
            }

            if (!topic.Children.IsEmpty && !topic.Folded)
            {
                foreach (Topic subTopic in topic.Children)
                {
                    PaintLinkLines(subTopic, e);
                }
            }
        }

        // 坐标已经转换, 原点为Topic.Left/Top
        void PaintNodeBackground(Topic topic, RenderArgs e)
        {
            var grf = e.Graphics;
            var rect = new Rectangle(Point.Empty, topic.Bounds.Size);

            bool hover = false;
            bool select = false;
            bool active = false;
            if (e.Mode == RenderMode.UserInface && e.View != null)
            {
                hover = e.View.HoverObject != null && e.View.HoverObject.Topic == topic && !e.View.HoverObject.IsFoldingButton;
                select = topic.Selected;
                active = topic == e.View.SelectedTopic;
            }

            Shape shaper = Shape.GetShaper(topic.Style.Shape, topic.Style.RoundRadius);

            // draw shadow
            //if (ShadowSize != 0)// && hover)
            //    PaintNodeShadow(topic, e, round);

            // draw background
            Color backColor = topic.Style.BackColor;
            if (backColor.IsEmpty)
                backColor = e.Chart.NodeBackColor;
            var ft = FillType.GetFillType(topic.Style.FillType);
            IBrush brushBack = ft.CreateBrush(e.Graphics, backColor, rect);

            //IBrush brushBack;
            //if (!topic.Style.BackColor.IsEmpty)
            //{
            //    brushBack = e.Graphics.SolidBrush(topic.Style.BackColor);
            //}
            //else
            //{
            //    brushBack = topic.IsRoot ? BrushRootNodeBack : BrushNodeBack;
            //}
            shaper.Fill(e.Graphics, brushBack, rect);

            // draw border
            if (hover)
                shaper.DrawBorder(e.Graphics, PenNodeBorderHover, rect);
            else
            {
                if (topic.Style.BorderColor.IsEmpty)
                    shaper.DrawBorder(e.Graphics, PenNodeBorder, rect);
                else
                    shaper.DrawBorder(e.Graphics, e.Graphics.Pen(topic.Style.BorderColor, PenNodeBorder.Width), rect);
            }

            if (active || select)
            {
                //Pen pen = active ? PenNodeBorderActive : PenNodeBorderSelect;
                //Rectangle rectOut = rect;
                //rectOut.Inflate(OutBoxSpace, OutBoxSpace);
                //shaper.DrawOutBox(e.Graphics, pen, rect, OutBoxSpace);
                DrawSelectRectangle(e.Graphics, e.Chart.SelectColor, rect);
            }

            shaper.Dispose();
        }

        void PaintFoldingButton(Topic topic, RenderArgs e, bool folded, bool hover)
        {
            if (!topic.FoldingButtonVisible || topic.FoldingButton.IsEmpty)
                return;

            var rect = topic.FoldingButton;
            var hard = e.Mode != RenderMode.UserInface;

            // 非界面输出, 不显示收缩按钮
            //if (hard && !folded)
            //    return;

            //if (e.Mode == RenderMode.UserInface)
            //{
            //    if (FoldingButtonImage == null)
            //        return;

            //    Image image = FoldingButtonImage;//
            //    //Image image = Properties.Resources.folding_button;
            //    int w = image.Width / 2;
            //    int h = image.Height / 2;
            //    int y = folded ? 0 : h;
            //    int x = hover ? w : 0;
            //    e.Graphics.DrawImage(image, rect, x, y, w, h, GraphicsUnit.Pixel);
            //}
            //else // for print
            //{
                DrawFoldingButtonBack(e.Graphics, rect, hover, hard);
                e.Graphics.ClearHighQualityFlag();
                DrawFoldingButtonMark(e.Graphics, rect, folded, hover, hard);
                e.Graphics.SetHighQualityRender();
            //}
        }

        void PaintOutBox(IPen pen, Topic topic, RenderArgs e, int round)
        {
            Rectangle rect = topic.Bounds;
            rect.Inflate(OutBoxSpace, OutBoxSpace);

            //GraphicsPath gp = PaintHelper.GetRoundRectangle(rect, round);
            //e.Graphics.DrawPath(pen, gp);
            e.Graphics.DrawRoundRectangle(pen, rect, round);
        }

        void PaintNodeShadow(Topic topic, RenderArgs e, int round)
        {
            if (ShadowSize > 0)
            {
                Shape shaper = Shape.GetShaper(topic.Style.Shape, round);

                Rectangle rect = topic.Bounds;
                rect.Offset(ShadowSize, ShadowSize);
                //GraphicsPath gpShadow = shaper.GetShape(new Rectangle(rect.Left + ShadowSize, rect.Top + ShadowSize, rect.Width, rect.Height), 0);
                    //PT.GetRoundRectangle(new Rectangle(rect.Left + ShadowSize, rect.Top + ShadowSize, rect.Width, rect.Height), round);
                //args.Graphics.FillPath(BrushNodeShadow, gpShadow);

                Color color;
                if (e.Mode == RenderMode.UserInface)
                {
                    if (topic.Style.BackColor.IsEmpty)
                        color = PaintHelper.GetDarkColor(topic.MindMap.NodeBackColor, 0.5);
                    else
                        color = PaintHelper.GetDarkColor(topic.Style.BackColor, 0.5);

                    //e.Graphics.FillPath(new SolidBrush(Color.FromArgb(150, color)), gpShadow);
                    shaper.Fill(e.Graphics, e.Graphics.SolidBrush(Color.FromArgb(150, color)), rect);
                }
                else // for print
                {
                    if (topic.Style.BackColor.IsEmpty)
                        color = PaintHelper.GetDarkColor(topic.MindMap.NodeBackColor);
                    else
                        color = PaintHelper.GetDarkColor(topic.Style.BackColor);

                    //e.Graphics.FillPath(new SolidBrush(color), gpShadow);
                    shaper.Fill(e.Graphics, e.Graphics.SolidBrush(color), rect);
                }

                shaper.Dispose();
            }
        }

        void PaintNavigationMap(IGraphics graphics, Topic topic, float zoom, IFont font)
        {
            if (topic == null || zoom <= 0)
                return;

            IBrush brushBack;
            if (!topic.Style.BackColor.IsEmpty)
                brushBack = graphics.SolidBrush(topic.Style.BackColor);
            else if (topic.IsRoot)
                brushBack = BrushRootNodeBack;
            else
                brushBack = BrushNodeBack;

            var rect = PaintHelper.Zoom(topic.Bounds, zoom);
            Shape shaper = Shape.GetShaper(topic.Style.Shape, 0);// Zoom(topic.Style.RoundRadius, zoom));
            if (rect.Width > 0 && rect.Height > 0)
            {
                shaper.Fill(graphics, brushBack, rect);
                shaper.DrawBorder(graphics, PenNodeBorder, rect);
            }

            //
            if (!string.IsNullOrEmpty(topic.Text) && font.Size > 0.1)
            {
                IBrush brushFore;
                if (!topic.Style.ForeColor.IsEmpty)
                    brushFore = graphics.SolidBrush(topic.Style.ForeColor);
                else if (topic.IsRoot)
                    brushFore = BrushRootNodeFore;
                else
                    brushFore = BrushNodeFore;

                var sf = PaintHelper.SFCenter;
                switch (topic.TextAlignment)
                {
                    case HorizontalAlignment.Left:
                        sf.Alignment = StringAlignment.Near;
                        break;
                    case HorizontalAlignment.Right:
                        sf.Alignment = StringAlignment.Far;
                        break;
                }

                var rectText = topic.TextBounds;
                rectText.Offset(topic.Location);
                rectText = PaintHelper.Zoom(rectText, zoom);
                graphics.DrawString(topic.Text, font, brushFore, rectText, sf);
            }

            if (topic.Lines.Count > 0)
            {
                if (Liner is BezierLine)
                    ((BezierLine)Liner).IsRoot = topic.IsRoot;

                foreach (var line in topic.Lines)
                {
                    var rectTo = PaintHelper.Zoom(line.Target.Bounds, zoom);
                    Liner.DrawLine(graphics, PenLineNav, topic.Style.Shape, line.Target.Style.Shape, rect, rectTo, 
                        line.BeginSide, line.EndSide, LineAnchor.None, LineAnchor.None);
                }
            }

            //if (topic.Parent != null)
            //{
            //    Rectangle rectParent = PT.Zoom(topic.Parent.Bounds, zoom);
            //    Liner.DrawLine(e.Graphics, PenLineNav, rectParent, rect, topic.Vector, zoom);
            //}

            if (!topic.Folded && !topic.Children.IsEmpty)
            {
                foreach (Topic subTopic in topic.Children)
                {
                    PaintNavigationMap(graphics, subTopic, zoom, font);
                }
            }
        }

        void PaintNavigationMapLinkLines(IGraphics graphics, Topic topic, float zoom, IFont font)
        {
            // links
            foreach (Link link in topic.Links)
            {
                if (!link.Visible)
                    continue;

                var layout = link.LayoutData;
                Color color = (topic.MindMap != null && link.Color.IsEmpty) ? topic.MindMap.LinkLineColor : link.Color;
                IPen pen = graphics.Pen(color, link.LineStyle);
                Point[] pts = new Point[] { layout.StartPoint, layout.ControlPoint1, layout.ControlPoint2, layout.EndPoint };
                for (int i = 0; i < pts.Length; i++)
                    pts[i] = PaintHelper.Zoom(pts[i], zoom);
                graphics.DrawBezier(pen, pts[0], pts[1], pts[2], pts[3]);
            }

            if (!topic.Children.IsEmpty && !topic.Folded)
            {
                foreach (Topic subTopic in topic.Children)
                {
                    PaintNavigationMapLinkLines(graphics, subTopic, zoom, font);
                }
            }
        }

        /*void GenerateFoldingButtonImage(MindMap map)
        {
            if (FoldingButtonImage != null)
            {
                FoldingButtonImage.Dispose();
                FoldingButtonImage = null;
            }

            int BS = CommonOptions.Charts.FoldingButtonSize;
            if (BS % 2 == 0)
                BS++;
            if (BS <= 2)
                return;

            Bitmap bmp = new Bitmap(BS * 2, BS * 2);
            using (Graphics grf = Graphics.FromImage(bmp))
            {
                var rects = new Rectangle[] { 
                    new Rectangle(0, 0, BS, BS),
                    new Rectangle(BS, 0, BS, BS),
                    new Rectangle(0, BS, BS, BS),
                    new Rectangle(BS, BS, BS, BS)
                    };

                var gs = grf.Save();
                var graphics = new GdiGraphics(grf);
                PaintHelper.SetHighQualityRender(grf);
                for (int i = 0; i < 4; i++)
                {
                    bool hover = i % 2 == 1;
                    bool plusMark = i < 2;
                    //rects[i].Inflate(-1, -1);
                    Rectangle rect = rects[i];

                    DrawFoldingButtonBack(graphics, rect, hover);
                }
                grf.Restore(gs);

                for (int i = 0; i < 4; i++)
                {
                    bool hover = i % 2 == 1;
                    bool plusMark = i < 2;
                    Rectangle rect = rects[i];

                    DrawFoldingButtonMark(graphics, rect, plusMark, hover);
                }

                grf.Dispose();
            }
            FoldingButtonImage = bmp;
        }*/

        void DrawFoldingButtonBack(IGraphics grf, Rectangle rect, bool hover, bool hard)
        {
            IBrush brushBack;
            IPen penBorder;
            if (hover)
            {
                brushBack = grf.LinearGradientBrush(rect, Color.WhiteSmoke, Color.Lavender, LinearGradientMode.ForwardDiagonal);
                //brushBack = new LinearGradientBrush(rect, Color.FromArgb(128, Color.WhiteSmoke), Color.FromArgb(64, Color.WhiteSmoke), 45.0f);
                penBorder = grf.Pen(Color.DimGray);
            }
            else if (hard)
            {
                brushBack = grf.SolidBrush(Color.WhiteSmoke);
                penBorder = grf.Pen(Color.DimGray);
            }
            else
            {
                brushBack = grf.SolidBrush(Color.WhiteSmoke);
                penBorder = grf.Pen(Color.FromArgb(200, Color.DimGray));
            }

            grf.FillEllipse(brushBack, rect.Left, rect.Top, rect.Width, rect.Height);
            grf.DrawEllipse(penBorder, rect.Left, rect.Top, rect.Width, rect.Height);
            //grf.FillRectangle(brushBack, rect.Left, rect.Top, rect.Width, rect.Height);
            //grf.DrawRectangle(penBorder, rect.Left, rect.Top, rect.Width - 1, rect.Height - 1);

            //GraphicsPath gp = PT.GetRoundRectangle(rect, 3);
            //grf.FillPath(brushBack, gp);
            //grf.DrawPath(penBorder, gp);
        }

        void DrawFoldingButtonMark(IGraphics grf, Rectangle rect, bool plusMark, bool hover, bool hard)
        {
            rect.Inflate(-3, -3);
            //grf.DrawRectangle(Pens.Black, rect.Left, rect.Top, rect.Width - 1, rect.Height - 1);

            const int size = 1;

            IBrush brush;
            if (hover)
                brush = grf.SolidBrush(Color.DimGray);
            else if (hard)
                brush = grf.SolidBrush(Color.DimGray);
            else
                brush = grf.SolidBrush(Color.FromArgb(180, Color.DimGray));

            grf.FillRectangle(brush, new Rectangle(rect.Left, rect.Top + (rect.Height - size) / 2, rect.Width, size));

            if (plusMark)
            {
                int hh = (rect.Height - size) / 2;
                grf.FillRectangle(brush, new Rectangle(rect.Left + (rect.Width - size) / 2, rect.Y, size, hh));
                grf.FillRectangle(brush, new Rectangle(rect.Left + (rect.Width - size) / 2, rect.Bottom - hh, size, hh));
            }
        }

        void PaintLinkLine(Link link, RenderArgs e)
        {
            Rectangle rect1 = link.From.Bounds;
            Rectangle rect2 = link.Target.Bounds;

            // if bounds changed, rebuild layout info
            if (rect1 != link.LayoutData.StartBounds || rect2 != link.LayoutData.EndBounds)
            {
                link.RefreshLayout();
            }

            // draw line
            var layout = link.LayoutData;
            Color color = link.Color.IsEmpty ? e.Chart.LinkLineColor : link.Color;
            var pen = e.Graphics.Pen(color, link.LineWidth, link.LineStyle);
            e.Graphics.DrawBezier(pen, layout.StartPoint, layout.ControlPoint1, layout.ControlPoint2, layout.EndPoint,
                link.StartCap, link.EndCap);

            //text
            if (!string.IsNullOrEmpty(link.Text))
            {
                var pt = BezierHelper.GetPoint(layout.StartPoint, layout.ControlPoint1, layout.ControlPoint2, layout.EndPoint, 0.5f);
                var size = Size.Ceiling(e.Graphics.MeasureString(link.Text, e.Font));
                var rectText = new Rectangle(pt.X - size.Width / 2, pt.Y - size.Height / 2, size.Width, size.Height);

                if (!e.Chart.BackColor.IsEmpty)
                    e.Graphics.FillRectangle(e.Graphics.SolidBrush(Color.FromArgb(180, e.Chart.BackColor)), rectText);
                
                if (!e.Chart.ForeColor.IsEmpty)
                {
                    var font2 = e.Font;
                    if (!string.IsNullOrEmpty(link.Hyperlink))
                        font2 = e.Graphics.Font(e.Font, e.Font.Style | FontStyle.Underline);

                    var sf = PaintHelper.SFCenter;
                    e.Graphics.DrawString(link.Text, font2, e.Graphics.SolidBrush(e.Chart.ForeColor), rectText, sf);
                }

                link.LayoutData.TextBounds = rectText;
            }
            else
            {
                link.LayoutData.TextBounds = Rectangle.Empty;
            }

            // remark-icon
            if (e.ShowRemarkIcon && link.HaveRemark)
            {
                Image iconRemark = Properties.Resources.note_small;
                Point pt = BezierHelper.GetPoint(layout.StartPoint, layout.ControlPoint1, layout.ControlPoint2, layout.EndPoint, 0.5f);
                Rectangle rectRemark = new Rectangle(pt.X - iconRemark.Width / 2, pt.Y - iconRemark.Height / 2, iconRemark.Width, iconRemark.Height);
                e.Graphics.DrawImage(iconRemark, rectRemark, 0, 0, iconRemark.Width, iconRemark.Height);
            }
        }

        #endregion

        IPen GetLinePen(IGraphics graphics, int width, Color color)
        {
            var pen = graphics.Pen(color, width);

            if (Options.Current.GetBool(Blumind.Configuration.OptionNames.Charts.ShowLineArrowCap))
            {
                /*int arrowSize = width;
                int arrowSize2 = width * 2;

                if (arrowSize > 1 && arrowSize2 > 1)
                {
                    pen.EndCap = LineCap.Custom;
                    GraphicsPath hPath = new GraphicsPath();
                    hPath.AddLine(new Point(0, 0), new Point(-arrowSize, -arrowSize2));
                    hPath.AddLine(new Point(-arrowSize, -arrowSize2), new Point(arrowSize, -arrowSize2));
                    hPath.AddLine(new Point(arrowSize, -arrowSize2), new Point(0, 0));
                    pen.CustomEndCap = new CustomLineCap(hPath, null);
                }
                else
                {
                    pen.EndCap = LineCap.ArrowAnchor;
                }*/
            }

            return pen;
        }

        void ResetObjects(IGraphics graphics, MindMap map)
        {
            if (map != null)
            {
                PenLine = GetLinePen(graphics, map.LineWidth, map.LineColor);
                PenLineNav = graphics.Pen(PenLine.Color);

                PenNodeBorder = graphics.Pen(map.BorderColor, map.BorderWidth);
                PenRootNodeBorder = graphics.Pen(map.BorderColor, map.BorderWidth);
                PenNodeBorderHover = graphics.Pen(map.HoverColor, map.BorderWidth + 1);
                PenNodeBorderSelect = graphics.Pen(map.SelectColor, map.BorderWidth + 1);
                PenNodeBorderActive = graphics.Pen(map.SelectColor, map.BorderWidth + 2);

                BrushNodeBack = graphics.SolidBrush(map.NodeBackColor);
                BrushNodeFore = graphics.SolidBrush(map.NodeForeColor);
                BrushRootNodeBack = graphics.SolidBrush(Color.White);
                BrushRootNodeFore = graphics.SolidBrush(Color.Black);
                BrushNodeShadow = graphics.SolidBrush(Color.LightGray);
            }
        }

        int Zoom(int value, float zoom)
        {
            return (int)Math.Ceiling(value * zoom);
        }
    }
}
