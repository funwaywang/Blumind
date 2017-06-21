using System;
using System.Collections;
using System.Drawing;
using Blumind.Core;
using Blumind.Model;
using Blumind.Model.MindMaps;

namespace Blumind.Controls.MapViews
{
    class LogicLayout : Layouter
    {
        private MindMapLayoutType Vector;

        public LogicLayout(MindMapLayoutType vector)
        {
            Vector = vector;
        }

        protected override Rectangle Layout(Topic root, MindMapLayoutArgs e)
        {
            if (root == null)
                throw new ArgumentNullException();

            return LayoutRoot(root, e);
        }

        private Rectangle LayoutRoot(Topic topic, MindMapLayoutArgs e)
        {
            Hashtable layoutInfos = new Hashtable();
            CalculateSizes(topic, layoutInfos, e);
            if (!layoutInfos.Contains(topic))
                return Rectangle.Empty;

            TopicLayoutInfo tli = (TopicLayoutInfo)layoutInfos[topic];
            int y = 0;
            int x;
            if (Vector == MindMapLayoutType.LogicLeft)
            {
                x = tli.FullSize.Width - e.LayerSpace;
            }
            else
            {
                x = 0;
            }

            LayoutTopic(topic, layoutInfos, x, y, e);
            return new Rectangle(0, 0, tli.FullSize.Width, tli.FullSize.Height);
        }

        private void LayoutTopic(Topic topic, Hashtable layoutInfos, int x, int y, MindMapLayoutArgs e)
        {
            var tli = (TopicLayoutInfo)layoutInfos[topic];
            int x2;
            int foldBtnSize = FoldingButtonSize;
            if (Vector == MindMapLayoutType.LogicLeft)
            {
                topic.Location = new Point(x - topic.Width, y + (tli.FullSize.Height - topic.Height) / 2);
                topic.FoldingButton = new Rectangle(topic.Left - foldBtnSize, 
                    topic.Top + (int)Math.Round((topic.Height - foldBtnSize) / 2.0f, MidpointRounding.AwayFromZero), 
                    foldBtnSize, 
                    foldBtnSize);
                x2 = topic.Location.X - e.LayerSpace;
            }
            else
            {
                topic.Location = new Point(x, y + (tli.FullSize.Height - topic.Height) / 2);
                topic.FoldingButton = new Rectangle(topic.Right,
                    topic.Top + (int)Math.Round((topic.Height - foldBtnSize) / 2.0f, MidpointRounding.AwayFromZero),
                    foldBtnSize,
                    foldBtnSize);
                x2 = topic.Right + e.LayerSpace;
            }

            topic.Lines.Clear();
            if (!topic.Children.IsEmpty && !topic.Folded)
            {
                y = y + (tli.FullSize.Height - tli.ChildrenSize.Height) / 2;
                foreach (Topic subTopic in topic.Children)
                {
                    if (subTopic != topic.Children[0])
                    {
                        y += e.ItemsSpace;
                    }

                    LayoutTopic(subTopic, layoutInfos, x2, y, e);

                    if (layoutInfos.Contains(subTopic))
                    {
                        var tli2 = (TopicLayoutInfo)layoutInfos[subTopic];
                        y += tli2.FullSize.Height;
                    }

                    // line
                    var line = CreateTopicLine(e, topic, subTopic);
                    if (line != null)
                    {
                        topic.Lines.Add(line);
                    }

                    //if (Vector == MindMapLayoutType.LogicLeft)
                    //{
                    //    topic.Lines.Add(new TopicLine(subTopic, Vector4.Left, Vector4.Right));
                    //}
                    //else
                    //{
                    //    topic.Lines.Add(new TopicLine(subTopic, Vector4.Right, Vector4.Left));
                    //}
                }
            }
        }

        TopicLine CreateTopicLine(MindMapLayoutArgs e, Topic topic, Topic subTopic)
        {
            switch (Vector)
            {
                case MindMapLayoutType.LogicLeft:
                    return CreateTopicLine(e, topic, subTopic, Vector4.Left, Vector4.Right);
                case MindMapLayoutType.LogicRight:
                default:
                    return CreateTopicLine(e, topic, subTopic, Vector4.Right, Vector4.Left);
            }
        }

        protected override TopicLine CreateTopicLine(MindMapLayoutArgs e, Topic beginTopic, Topic endTopic, Vector4 beginSide, Vector4 endSide)
        {
            var beginRect = beginTopic.Bounds;
            var endRect = endTopic.Bounds;

            if (e.ShowLineArrowCap)
            {
                endRect.Inflate(LineAnchorSize, LineAnchorSize);
            }

            if (beginTopic != null && !beginTopic.IsRoot)
            {
                int foldBtnSize = endTopic.FoldingButton.Width;
                switch (Vector)
                {
                    case MindMapLayoutType.LogicLeft:
                        beginRect.X -= foldBtnSize;
                        beginRect.Width += foldBtnSize;
                        break;
                    case MindMapLayoutType.LogicRight:
                    default:
                        beginRect.Width += foldBtnSize;
                        break;
                }
            }

            return new TopicLine(endTopic, beginSide, endSide, beginRect, endRect);
        }

        //public override void AdjustLineRect(TopicLine line, Topic fromTopic, ref Rectangle rectFrom, ref Rectangle rectTo)
        //{
        //    base.AdjustLineRect(line, fromTopic, ref rectFrom, ref rectTo);

        //    if (fromTopic != null && !fromTopic.IsRoot)
        //    {
        //        int foldBtnSize = line.Target.FoldingButton.Width;
        //        if (Vector == MindMapLayoutType.LogicLeft)
        //        {
        //            rectFrom.X -= foldBtnSize;
        //            rectFrom.Width += foldBtnSize;
        //        }
        //        else
        //        {
        //            rectFrom.Width += foldBtnSize;
        //        }
        //    }
        //}

        void CalculateSizes(Topic topic, Hashtable layoutInfos, MindMapLayoutArgs e)
        {
            if (topic == null)
                return;

            topic.Size = CalculateNodeSize(topic, e);
            var topicFullSize = LayoutAttachments(topic, e);

            int w = 0;
            int h = 0;
            if (!topic.Children.IsEmpty && !topic.Folded)
            {
                foreach (Topic subTopic in topic.Children)
                {
                    CalculateSizes(subTopic, layoutInfos, e);
                    if (layoutInfos.Contains(subTopic))
                    {
                        TopicLayoutInfo tli = (TopicLayoutInfo)layoutInfos[subTopic];
                        h += tli.FullSize.Height;
                        w = Math.Max(w, tli.FullSize.Width);
                    }

                    if (subTopic != topic.Children[0])
                    {
                        h += e.ItemsSpace;
                    }
                }
            }

            var tlf = new TopicLayoutInfo();
            tlf.ChildrenSize = new Size(w, h);
            tlf.FullSize = new Size(w + topicFullSize.Width + e.LayerSpace, Math.Max(h, topicFullSize.Height));
            layoutInfos[topic] = tlf;
        }

        public override Topic GetNextNode(Topic from, MoveVector vector)
        {
            if (from == null)
                throw new ArgumentNullException("from");

            switch (Vector)
            {
                case MindMapLayoutType.LogicLeft:
                    return GetNextNodeLeft(from, vector);
                case MindMapLayoutType.LogicRight:
                    return GetNextNodeRight(from, vector);
                default:
                    throw new Exception("Invalid Layout Vector");
            }
        }

        private Topic GetNextNodeLeft(Topic from, MoveVector vector)
        {
            switch (vector)
            {
                case MoveVector.Left:
                    return (from.HasChildren) ? from.Children[0] : null;
                case MoveVector.Right:
                    return (from.ParentTopic != null) ? from.ParentTopic : null;
                case MoveVector.Up:
                    return from.GetSibling(false, false, true);
                case MoveVector.Down:
                    return from.GetSibling(true, false, true);
            }

            return null;
        }

        private Topic GetNextNodeRight(Topic from, MoveVector vector)
        {
            switch (vector)
            {
                case MoveVector.Left:
                    return (from.ParentTopic != null) ? from.ParentTopic : null;
                case MoveVector.Right:
                    return (from.HasChildren) ? from.Children[0] : null;
                case MoveVector.Up:
                    return from.GetSibling(false, false, true);
                case MoveVector.Down:
                    return from.GetSibling(true, false, true);
            }

            return null;
        }

        protected override Size LayoutAttachments(Topic topic, MindMapLayoutArgs e)
        {
            var size = topic.Size;
            if (e.ShowRemarkIcon && topic.HaveRemark)
            {
                int x = 0;
                switch (Vector)
                {
                    case MindMapLayoutType.LogicLeft:
                        x = -2 - e.RemarkIconSize;
                        break;
                    case MindMapLayoutType.LogicRight:
                        x = size.Width + 2;
                        break;
                }

                topic.RemarkIconBounds = new Rectangle(x, 0, e.RemarkIconSize, e.RemarkIconSize);
                size.Width += 2 + e.RemarkIconSize;
                size.Height = Math.Max(size.Height, e.RemarkIconSize);
            }

            return size;
            //return base.LayoutAttachments(topic, e);
        }
    }
}
