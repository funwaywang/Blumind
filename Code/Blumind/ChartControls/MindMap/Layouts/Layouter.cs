using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Blumind.Configuration;
using Blumind.Core;
using Blumind.Model;
using Blumind.Model.MindMaps;
using Blumind.Model.Widgets;

namespace Blumind.Controls.MapViews
{
    abstract class Layouter
    {
        protected class TopicLayoutInfo
        {
            public Size FullSize;
            public Size ChildrenSize;

            public TopicLayoutInfo()
            {
            }

            public TopicLayoutInfo(Size fullSize)
            {
                FullSize = fullSize;
            }
        }

        //protected const int NodeSpace_H = 60;
        //protected const int NodeSpaceRoot_V = 50;
        //protected const int NodeSpace_V = 10;
        //protected const int IconSpace = 5;
        //public const int FoldingButtonSize = 13;
        protected readonly Size MinNodeSize = new Size(20, 10);

        public static readonly int LineAnchorSize = 4;

        public Size LayoutMap(MindMap map, MindMapLayoutArgs e)
        {
            if (map == null)
            {
                throw new ArgumentNullException();
            }

            if (map.Root != null)
            {
                Rectangle bounds = Layout(map.Root, e);

                Rectangle linesBounds = GetLinksFullBounds(map);
                if (!linesBounds.IsEmpty)
                    bounds = Rectangle.Union(bounds, linesBounds);

                // page size
                var psx = Math.Max(0, (map.PageSize.Width - (bounds.Width + map.Margin.Horizontal)) / 2);
                var psy = Math.Max(0, (map.PageSize.Height - (bounds.Height + map.Margin.Vertical)) / 2);

                Offset(map.Root, map.Margin.Left - bounds.X + psx, map.Margin.Top - bounds.Y + psy);

                // refresh line's layout
                Link[] lines = map.GetLinks(true);
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i].RefreshLayout();
                }

                bounds.Width +=  map.Margin.Horizontal;
                bounds.Height += map.Margin.Vertical;

                // page size
                bounds.Width = Math.Max(bounds.Width, map.PageSize.Width);
                bounds.Height = Math.Max(bounds.Height, map.PageSize.Height);

                map.LayoutInitialized = true;
                return bounds.Size;
            }
            else
            {
                return Size.Empty;
            }
        }

        protected abstract Rectangle Layout(Topic root, MindMapLayoutArgs e);

        public abstract Topic GetNextNode(Topic from, MoveVector vector);

        protected int FoldingButtonSize
        {
            get { return CommonOptions.Charts.FoldingButtonSize; }
        }

        //protected int Zoom(int value, float zoom)
        //{
        //    return (int)Math.Ceiling(value * zoom);
        //}

        protected virtual Size CalculateNodeSize(Topic topic, MindMapLayoutArgs e)
        {
            //if (topic.CustomWidth.HasValue && topic.CustomHeight.HasValue)
            //    return new Size(topic.CustomWidth.Value, topic.CustomHeight.Value);

            //
            Size proposedSize = Size.Empty;
            if (topic.CustomWidth.HasValue && topic.CustomWidth.Value > 0)
                proposedSize.Width = topic.CustomWidth.Value - topic.Style.Padding.Horizontal;
            if (topic.CustomHeight.HasValue && topic.CustomHeight.Value > 0)
                proposedSize.Height = topic.CustomHeight.Value - topic.Style.Padding.Vertical;

            // Icon Size
            //Rectangle iconBounds = Rectangle.Empty;
            //if (topic.Icon != null)
            //{
            //    iconBounds = new Rectangle(0, 0, topic.Icon.Width, topic.Icon.Height);
            //    if (proposedSize.Width > 0)
            //        proposedSize.Width -= topic.IconBounds.Width + +topic.Style.IconPadding;
            //}

            // Text Size
            Rectangle rectText = Rectangle.Empty;
            Font font = topic.Style.Font != null ? topic.Style.Font : e.Font;
            Size textSize;
            if (e.Graphics == null)
                textSize = TextRenderer.MeasureText(topic.Text, font, proposedSize);
            else
                textSize = Size.Ceiling(e.Graphics.MeasureString(topic.Text, font, new SizeF(proposedSize.Width, proposedSize.Height)));
            rectText = new Rectangle(Point.Empty, textSize);

            // Widgets Size
            //var widgetAligns = new WidgetAlignment[] { WidgetAlignment.Left, WidgetAlignment.Top, WidgetAlignment.Right, WidgetAlignment.Bottom };
            var rectLeft = CalculateWidgets(topic, WidgetAlignment.Left, e);
            var rectTop = CalculateWidgets(topic, WidgetAlignment.Top, e);
            var rectRight = CalculateWidgets(topic, WidgetAlignment.Right, e);
            var rectBottom = CalculateWidgets(topic, WidgetAlignment.Bottom, e);
            int maxWidth = Helper.GetMax(rectText.Width, rectTop.Width, rectBottom.Width) + rectLeft.Width + rectRight.Width;
            int totalHeight = Helper.GetMax(rectText.Height + rectTop.Height + rectBottom.Height, rectLeft.Height, rectRight.Height);
            rectText.Width = Helper.GetMax(rectText.Width, rectTop.Width, rectBottom.Width);

            // Desc Size
            if (e.ShowRemarkIcon && topic.HaveRemark)
            {
                maxWidth += 16;
            }
            
            // Calculate Size
            Size size = new Size(maxWidth + topic.Padding.Horizontal, totalHeight + topic.Padding.Vertical);
            //if (!iconBounds.IsEmpty)
            //{
            //    size.Width += topic.IconPadding + iconBounds.Width;
            //    size.Height = Math.Max(topic.IconPadding + iconBounds.Height, size.Height);
            //}
            size.Width = Math.Max(MinNodeSize.Width, size.Width);
            size.Height = Math.Max(MinNodeSize.Height, size.Height);
            if (topic.CustomWidth.HasValue)
                size.Width = topic.CustomWidth.Value;
            else if (topic.CustomHeight.HasValue)
                size.Height = topic.CustomHeight.Value;

            // 
            var rect = new Rectangle(0, 0, size.Width, size.Height);
            rect = rect.Inflate(topic.Style.Padding);

            // Text Location

            // Widgets Location
            int hh1 = rectText.Height + rectTop.Height + rectBottom.Height;
            if (hh1 < rect.Height)
            {
                int hh = (rect.Height - hh1) / 3;
                rectText.Height += hh;
                rectTop.Height += hh;
                rectBottom.Height += hh;
            }
            if (!rectText.IsEmpty)
            {
                rectText = new Rectangle(
                    rect.X + rectLeft.Width, 
                    rect.Y + rectTop.Height,
                    rect.Width - rectLeft.Width - rectRight.Width,
                    rect.Height - rectTop.Height - rectBottom.Height);
            }
            rectLeft.Height = rectRight.Height = Helper.GetMax(rectLeft.Height, rectRight.Height, rectText.Height + rectTop.Height + rectBottom.Height);
            rectLeft.Y = rectRight.Y = rect.Y;
            rectLeft.X = rect.X;
            rectRight.X = rectLeft.Right + rectText.Width;
            rectTop.X = rectBottom.X = rectText.Left;
            rectTop.Width = rectBottom.Width = rectText.Width;
            rectTop.Y = rect.Y;
            rectBottom.Y = rectText.Bottom;
            topic.TextBounds = rectText;
            ResetWidgetLocation(e, topic, WidgetAlignment.Left, rectLeft);
            ResetWidgetLocation(e, topic, WidgetAlignment.Top, rectTop);
            ResetWidgetLocation(e, topic, WidgetAlignment.Right, rectRight);
            ResetWidgetLocation(e, topic, WidgetAlignment.Bottom, rectBottom);
            
            //
            return size;
        }

        protected virtual Size LayoutAttachments(Topic topic, MindMapLayoutArgs e)
        {
            var size = topic.Size;
            if (e.ShowRemarkIcon && topic.HaveRemark)
            {
                topic.RemarkIconBounds = new Rectangle(0, size.Height + 2, e.RemarkIconSize, e.RemarkIconSize);
                size.Height += 2 + e.RemarkIconSize;
                size.Width = Math.Max(size.Width, e.RemarkIconSize);
            }

            return size;
        }

        Rectangle CalculateWidgets(Topic topic, WidgetAlignment alignment, MindMapLayoutArgs e)
        {
            var widgets = topic.FindWidgets(alignment);
            if (widgets.Length == 0)
                return Rectangle.Empty;

            var rect = Rectangle.Empty;
            var fitSize = Size.Empty;
            foreach (var widget in widgets)
            {
                var rectW = new Rectangle(Point.Empty, widget.CalculateSize(e));
                rectW.Inflate(widget.Padding, widget.Padding);
                if (widget.CustomWidth.HasValue)
                    rectW.Width = widget.CustomWidth.Value;
                if (widget.CustomHeight.HasValue)
                    rectW.Height = widget.CustomHeight.Value;
                widget.Bounds = rectW;

                rectW.Width += e.Chart.WidgetMargin;
                rectW.Height += e.Chart.WidgetMargin;

                //
                switch (alignment)
                {
                    case WidgetAlignment.Left:
                    case WidgetAlignment.Right:
                        if (widget.FitContainer)
                        {
                            fitSize.Width += rectW.Width;
                            fitSize.Height = Math.Max(fitSize.Height, rectW.Height);
                        }
                        else
                        {
                            rect.Width = Math.Max(rect.Width, rectW.Width);
                            rect.Height += rectW.Height;
                        }
                        break;
                    case WidgetAlignment.Top:
                    case WidgetAlignment.Bottom:
                        if (widget.FitContainer)
                        {
                            fitSize.Height += rectW.Height;
                            fitSize.Width = Math.Max(fitSize.Width, rectW.Width);
                        }
                        else
                        {
                            rect.Width += rectW.Width;
                            rect.Height = Math.Max(rect.Height, rectW.Height);
                        }
                        break;
                }
            }

            switch (alignment)
            {
                case WidgetAlignment.Left:
                case WidgetAlignment.Right:
                    rect.Width += fitSize.Width;
                    rect.Height = Math.Max(rect.Height, fitSize.Height);
                    break;
                case WidgetAlignment.Top:
                case WidgetAlignment.Bottom:
                    rect.Width = Math.Max(rect.Width, fitSize.Width);
                    rect.Height += fitSize.Height;
                    break;
            }

            rect.Width += e.Chart.WidgetMargin;
            rect.Height += e.Chart.WidgetMargin;

            return rect;
        }

        void ResetWidgetLocation(MindMapLayoutArgs e, Topic topic, WidgetAlignment alignment, Rectangle rect)
        {
            var widgets = topic.FindWidgets(alignment);
            if (widgets.Length == 0)
                return;

            var dynamicWidgets = widgets.Where(w => !w.FitContainer).ToArray();
            if (alignment == WidgetAlignment.Left || alignment == WidgetAlignment.Right)
            {
                var totalHeight = dynamicWidgets.Sum(w => w.Bounds.Height);
                var maxWidth = dynamicWidgets.Length > 0 ? dynamicWidgets.Max(w => w.Bounds.Width) : 0;
                var widgetMargin = Math.Max(e.Chart.WidgetMargin, (rect.Height - totalHeight) / (dynamicWidgets.Length + 1));

                int y = rect.Y + widgetMargin;
                int x = rect.X + e.Chart.WidgetMargin;
                int dx = -1;
                int dy = y;
                foreach (var w in widgets)
                {
                    var rw = w.Bounds;

                    if (w.FitContainer)
                    {
                        rw.X = x;
                        rw.Y = rect.Y + e.Chart.WidgetMargin;
                        rw.Height = rect.Height - e.Chart.WidgetMargin * 2;
                        x += rw.Width + e.Chart.WidgetMargin;
                    }
                    else
                    {
                        if (dx < 0)
                        {
                            dx = x;
                            x += maxWidth + e.Chart.WidgetMargin;
                        }
                        rw.X = dx + (maxWidth - w.Bounds.Width) / 2;
                        rw.Y = dy;
                        dy += rw.Height + widgetMargin;
                    }

                    w.Bounds = rw;
                }
            }
            else if (alignment == WidgetAlignment.Top || alignment == WidgetAlignment.Bottom)
            {
                var totalWidth = dynamicWidgets.Sum(w => w.Bounds.Width);
                var maxHeight = dynamicWidgets.Length > 0 ? dynamicWidgets.Max(w => w.Bounds.Height) : 0;
                var widgetPadding = Math.Max(e.Chart.WidgetMargin, (rect.Width - totalWidth) / (dynamicWidgets.Length + 1));

                int y = rect.Y + e.Chart.WidgetMargin;
                int x = rect.X + widgetPadding;
                int dx = x;
                int dy = -1;
                foreach (var w in widgets)
                {
                    var rw = w.Bounds;

                    if (w.FitContainer)
                    {
                        rw.X = rect.X + e.Chart.WidgetMargin;
                        rw.Y = y;
                        rw.Width = rect.Width - e.Chart.WidgetMargin * 2;
                        y += rw.Height + e.Chart.WidgetMargin;
                    }
                    else
                    {
                        if (dy < 0)
                        {
                            dy = y;
                            y += maxHeight + e.Chart.WidgetMargin;
                        }
                        rw.X = dx;
                        rw.Y = dy + (maxHeight - w.Bounds.Height) / 2;
                        dx += rw.Width + widgetPadding;
                    }

                    w.Bounds = rw;
                }
            }
        }
        
        protected Vector4 GetReverseVector(HorizontalVector vector)
        {
            switch (vector)
            {
                case HorizontalVector.Left:
                    return Vector4.Right;
                case HorizontalVector.Right:
                default:
                    return Vector4.Left;
            }
        }

        protected Vector4 GetReverseVector(Vector4 vector)
        {
            switch (vector)
            {
                case Vector4.Left:
                    return Vector4.Right;
                case Vector4.Top:
                    return Vector4.Bottom;
                case Vector4.Right:
                    return Vector4.Left;
                case Vector4.Bottom:
                default:
                    return Vector4.Top;
            }
        }

        protected void Offset(Topic topic, int x, int y)
        {
            if (topic == null)
                throw new ArgumentNullException("Topic");

            if (x == 0 && y == 0)
                return;

            Rectangle rect = topic.Bounds;
            rect.Offset(x, y);
            topic.Bounds = rect;

            rect = topic.FoldingButton;
            rect.Offset(x, y);
            topic.FoldingButton = rect;

            foreach (var line in topic.Lines)
            {
                line.Offset(x, y);
            }

            foreach (Topic subTopic in topic.Children)
            {
                Offset(subTopic, x, y);
            }
        }

        //public virtual void AdjustLineRect(TopicLine line, Topic fromTopic, ref Rectangle rectFrom, ref Rectangle rectTo)
        //{
        //}

        protected virtual TopicLine CreateTopicLine(MindMapLayoutArgs e, Topic beginTopic, Topic endTopic, Vector4 beginSide, Vector4 endSide)
        {
            return null;
        }

        protected Rectangle GetLinksFullBounds(MindMap map)
        {
            if (map == null)
                throw new ArgumentNullException();

            Rectangle result = Rectangle.Empty;
            Link[] lines = map.GetLinks(true);
            for (int i = 0; i < lines.Length; i++)
            {
                Link line = lines[i];
                if (line.Visible)
                {
                    lines[i].RefreshLayout();
                    Rectangle rect = line.LayoutData.GetFullBounds();
                    rect.Inflate(10, 10);

                    if (i == 0)
                        result = rect;
                    else
                        result = Rectangle.Union(result, rect);
                }
            }

            return result;
        }
    }
}
