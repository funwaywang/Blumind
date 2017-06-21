using System;
using System.Collections;
using System.Drawing;
using Blumind.Core;
using Blumind.Model;
using Blumind.Model.MindMaps;

namespace Blumind.Controls.MapViews
{
    class OrganizationLayout : Layouter
    {
        private MindMapLayoutType Vector;

        public OrganizationLayout(MindMapLayoutType vector)
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
            if(!layoutInfos.Contains(topic))
                return Rectangle.Empty;

            var tli = (TopicLayoutInfo)layoutInfos[topic];
            int y;
            int x = 0;
            if (Vector == MindMapLayoutType.OrganizationUp)
            {
                y = tli.FullSize.Height - e.LayerSpace;
            }
            else
            {
                y = 0;
            }

            LayoutTopic(topic, layoutInfos, x, y, e); 
            return new Rectangle(0, 0, tli.FullSize.Width, tli.FullSize.Height);
        }

        void LayoutTopic(Topic topic, Hashtable layoutInfos, int x, int y, MindMapLayoutArgs e)
        {
            var tli = (TopicLayoutInfo)layoutInfos[topic];
            int y2;
            int foldBtnSize = FoldingButtonSize;
            if (Vector == MindMapLayoutType.OrganizationUp)
            {
                topic.Location = new Point(x + (tli.FullSize.Width - topic.Width) / 2, y - topic.Height);
                topic.FoldingButton = new Rectangle(
                    topic.Left + (int)Math.Round((topic.Width - foldBtnSize) / 2.0f, MidpointRounding.AwayFromZero),
                    topic.Top - foldBtnSize, 
                    foldBtnSize, 
                    foldBtnSize);
                y2 = topic.Top- e.LayerSpace;
            }
            else
            {
                topic.Location = new Point(x + (tli.FullSize.Width - topic.Width) / 2, y);
                topic.FoldingButton = new Rectangle(
                    topic.Left + (int)Math.Round((topic.Width - foldBtnSize) / 2.0f, MidpointRounding.AwayFromZero),
                    topic.Bottom, 
                    foldBtnSize,
                    foldBtnSize);
                y2 = topic.Bottom + e.LayerSpace;
            }

            topic.Lines.Clear();
            if (!topic.Children.IsEmpty && !topic.Folded)
            {
                x = x + (tli.FullSize.Width - tli.ChildrenSize.Width) / 2;
                foreach (Topic subTopic in topic.Children)
                {
                    if (subTopic != topic.Children[0])
                    {
                        x += e.ItemsSpace;
                    }

                    LayoutTopic(subTopic, layoutInfos, x, y2, e);

                    if (layoutInfos.Contains(subTopic))
                    {
                        var tli2 = (TopicLayoutInfo)layoutInfos[subTopic];
                        x += tli2.FullSize.Width;
                    }

                    // line
                    var line = CreateTopicLine(e, topic, subTopic);
                    if (line != null)
                    {
                        topic.Lines.Add(line);
                    }
                    
                    //if (Vector == MindMapLayoutType.OrganizationUp)
                    //{
                    //    topic.Lines.Add(new TopicLine(subTopic, Vector4.Top, Vector4.Bottom));
                    //}
                    //else
                    //{
                    //    topic.Lines.Add(new TopicLine(subTopic, Vector4.Bottom, Vector4.Top));
                    //}
                }
            }
        }

        void CalculateSizes(Topic topic, Hashtable layoutInfos, MindMapLayoutArgs e)
        {
            if (topic == null)
                return;

            Size size = CalculateNodeSize(topic, e);
            if (size.Width % 2 == 0)
                size = new Size(size.Width + 1, size.Height);
            topic.Size = size;
            var topicFullSize = LayoutAttachments(topic, e);

            int w = 0;
            int h = 0;
            if (!topic.Children.IsEmpty && !topic.Folded)
            {
                foreach (var subTopic in topic.Children)
                {
                    CalculateSizes(subTopic, layoutInfos, e);
                    if (layoutInfos.Contains(subTopic))
                    {
                        TopicLayoutInfo tli = (TopicLayoutInfo)layoutInfos[subTopic];
                        w += tli.FullSize.Width;
                        h = Math.Max(h, tli.FullSize.Height);
                    }

                    if (subTopic != topic.Children[0])
                    {
                        w += e.ItemsSpace;
                    }
                }
            }

            TopicLayoutInfo tlf =  new TopicLayoutInfo();
            tlf.ChildrenSize = new Size(w, h);
            tlf.FullSize = new Size(Math.Max(w, topicFullSize.Width), h + topicFullSize.Height + e.LayerSpace);
            layoutInfos[topic] = tlf;
        }

        TopicLine CreateTopicLine(MindMapLayoutArgs e, Topic topic, Topic subTopic)
        {
            switch (Vector)
            {
                case MindMapLayoutType.OrganizationUp:
                    return CreateTopicLine(e, topic, subTopic, Vector4.Top, Vector4.Bottom);
                case MindMapLayoutType.OrganizationDown:
                default:
                    return CreateTopicLine(e, topic, subTopic, Vector4.Bottom, Vector4.Top);
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

            if (!beginTopic.IsRoot)
            {
                int foldBtnSize = beginTopic.FoldingButton.Height;
                switch (Vector)
                {
                    case MindMapLayoutType.OrganizationUp:
                        beginRect.Y -= foldBtnSize;
                        beginRect.Height += foldBtnSize;
                        break;
                    case MindMapLayoutType.OrganizationDown:
                    default:
                        beginRect.Height += foldBtnSize;
                        break;
                }
            }

            return new TopicLine(endTopic, beginSide, endSide, beginRect, endRect);
        }

        public override Topic GetNextNode(Topic from, MoveVector vector)
        {
            if (from == null)
                throw new ArgumentNullException("from");

            switch (Vector)
            {
                case MindMapLayoutType.OrganizationUp:
                    return GetNextNodeUp(from, vector);
                case MindMapLayoutType.OrganizationDown:
                    return GetNextNodeDown(from, vector);
                default:
                    throw new Exception("Invalid Layout Vector");
            }
        }

        private Topic GetNextNodeUp(Topic from, MoveVector vector)
        {
            switch (vector)
            {
                case MoveVector.Left:
                    return from.GetSibling(false, false, true);
                case MoveVector.Right:
                    return from.GetSibling(true, false, true);
                case MoveVector.Up:
                    return (from.HasChildren) ? from.Children[0] : null;
                case MoveVector.Down:
                    return (from.ParentTopic != null) ? from.ParentTopic : null;
            }

            return null;
        }

        private Topic GetNextNodeDown(Topic from, MoveVector vector)
        {
            switch (vector)
            {
                case MoveVector.Left:
                    return from.GetSibling(false, false, true);
                case MoveVector.Right:
                    return from.GetSibling(true, false, true);
                case MoveVector.Up:
                    return (from.ParentTopic != null) ? from.ParentTopic : null;
                case MoveVector.Down:
                    return (from.HasChildren) ? from.Children[0] : null;
            }

            return null;
        }
    }
}
