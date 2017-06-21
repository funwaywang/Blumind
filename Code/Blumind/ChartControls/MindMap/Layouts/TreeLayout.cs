using System;
using System.Collections;
using System.Drawing;
using Blumind.Core;
using Blumind.Model;
using Blumind.Model.MindMaps;

namespace Blumind.Controls.MapViews
{
    class TreeLayout : Layouter
    {
        MindMapLayoutType Vector;

        public TreeLayout(MindMapLayoutType vector)
        {
            Vector = vector;
        }

        protected override Rectangle Layout(Topic root, MindMapLayoutArgs e)
        {
            if (root == null)
                throw new ArgumentNullException();

            return LayoutRoot(root, e);
        }

        Rectangle LayoutRoot(Topic topic, MindMapLayoutArgs e)
        {
            Hashtable layoutInfos = new Hashtable();
            CalculateSizes(topic, layoutInfos, e);
            if (!layoutInfos.Contains(topic))
                return Rectangle.Empty;

            var tli = (TopicLayoutInfo)layoutInfos[topic];
            int y = 0;
            int x;
            if (Vector == MindMapLayoutType.TreeLeft)
            {
                x = tli.FullSize.Width;
            }
            else
            {
                x = 0;
            }

            LayoutTopic(topic, layoutInfos, x, y, e);
            return new Rectangle(0, 0, tli.FullSize.Width, tli.FullSize.Height);
        }

        void LayoutTopic(Topic topic, Hashtable layoutInfos, int x, int y, MindMapLayoutArgs e)
        {
            var tli = (TopicLayoutInfo)layoutInfos[topic];
            int foldBtnSize = FoldingButtonSize;
            int x2;
            if (Vector == MindMapLayoutType.TreeLeft)
            {
                topic.Location = new Point(x - topic.Width, y);
                topic.FoldingButton = new Rectangle(topic.Right, 
                    topic.Top + (int)Math.Round((topic.Height - foldBtnSize) / 2.0f, MidpointRounding.AwayFromZero),
                    foldBtnSize, 
                    foldBtnSize);
                x2 = topic.Left + topic.Width / 2 - e.LayerSpace;
            }
            else
            {
                topic.Location = new Point(x, y);
                topic.FoldingButton = new Rectangle(topic.Left - foldBtnSize,
                    topic.Top + (int)Math.Round((topic.Height - foldBtnSize) / 2.0f, MidpointRounding.AwayFromZero),
                    foldBtnSize, 
                    foldBtnSize);
                x2 = topic.Left + topic.Width / 2 + e.LayerSpace;
            }

            topic.Lines.Clear();
            if (!topic.Children.IsEmpty && !topic.Folded)
            {
                y += topic.Height + e.ItemsSpace;
                foreach (Topic subTopic in topic.Children)
                {
                    LayoutTopic(subTopic, layoutInfos, x2, y, e);

                    if (layoutInfos.Contains(subTopic))
                    {
                        TopicLayoutInfo tli2 = (TopicLayoutInfo)layoutInfos[subTopic];
                        y += tli2.FullSize.Height + e.ItemsSpace;
                    }

                    // line
                    var line = CreateTopicLine(e, topic, subTopic);
                    if (line != null)
                    {
                        topic.Lines.Add(line);
                    }
                }
            }
        }

        TopicLine CreateTopicLine(MindMapLayoutArgs e, Topic topic, Topic subTopic)
        {
            switch (Vector)
            {
                case MindMapLayoutType.TreeLeft:
                    return CreateTopicLine(e, topic, subTopic, Vector4.Bottom, Vector4.Right);
                case MindMapLayoutType.TreeRight:
                default:
                    return CreateTopicLine(e, topic, subTopic, Vector4.Bottom, Vector4.Left);
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

            if (endTopic != null && !endTopic.Children.IsEmpty)
            {
                int foldBtnSize = endTopic.FoldingButton.Width;
                switch (endSide)
                {
                    case Vector4.Left:
                        endRect.X -= foldBtnSize;
                        endRect.Width += foldBtnSize;
                        break;
                    case Vector4.Right:
                        endRect.Width += foldBtnSize;
                        break;
                }
            }

            return new TopicLine(endTopic, beginSide, endSide, beginRect, endRect);

            //if (Vector == MindMapLayoutType.TreeLeft)
            //{
            //    beginTopic.Lines.Add(new TopicLine(endTopic, Vector4.Bottom, Vector4.Right));
            //}
            //else
            //{
            //    beginTopic.Lines.Add(new TopicLine(endTopic, Vector4.Bottom, Vector4.Left));
            //}
        }

        void CalculateSizes(Topic topic, Hashtable layoutInfos, MindMapLayoutArgs e)
        {
            if (topic == null)
                return;

            topic.Size = CalculateNodeSize(topic, e);
            var topicFullSize = LayoutAttachments(topic, e);

            TopicLayoutInfo tlf = new TopicLayoutInfo();
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

                tlf.ChildrenSize = new Size(w, h);
                tlf.FullSize = new Size(Math.Max(topicFullSize.Width , w + topicFullSize.Width / 2 + e.LayerSpace),
                    topicFullSize.Height + e.ItemsSpace + h);
            }
            else
            {
                tlf.FullSize = new Size(topicFullSize.Width, topicFullSize.Height);
            }
            //if (!topic.Children.IsEmpty)
                //tlf.FullSize.Height += FoldingButtonSize;

            layoutInfos[topic] = tlf;
        }

        //public override void AdjustLineRect(TopicLine line, Topic fromTopic, ref Rectangle rectFrom, ref Rectangle rectTo)
        //{
        //    base.AdjustLineRect(line, fromTopic, ref rectFrom, ref rectTo);

        //    if (line.Target != null && !line.Target.Children.IsEmpty)
        //    {
        //        int foldBtnSize = line.Target.FoldingButton.Width;
        //        switch (line.EndSide)
        //        {
        //            case Vector4.Left:
        //                rectTo.X -= foldBtnSize;
        //                rectTo.Width += foldBtnSize;
        //                break;
        //            case Vector4.Right:
        //                rectTo.Width += foldBtnSize;
        //                break;
        //        }
        //    }
        //}

        public override Topic GetNextNode(Topic from, MoveVector vector)
        {
            if (from == null)
                throw new ArgumentNullException("from");

            switch (Vector)
            {
                case MindMapLayoutType.TreeLeft:
                    return GetNextNodeLeft(from, vector);
                case MindMapLayoutType.TreeRight:
                    return GetNextNodeRight(from, vector);
                default:
                    throw new Exception("Invalid Layout Vector");
            }
        }

        Topic GetNextNodeLeft(Topic from, MoveVector vector)
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

        Topic GetNextNodeRight(Topic from, MoveVector vector)
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
                    case MindMapLayoutType.TreeLeft:
                        x = -2 - e.RemarkIconSize;
                        break;
                    case MindMapLayoutType.TreeRight:
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
