using System;
using System.Collections;
using System.Drawing;
using Blumind.Core;
using Blumind.Model;
using Blumind.Model.MindMaps;
using Blumind.Model.Styles;

namespace Blumind.Controls.MapViews
{
    class MindMapLayout : Layouter
    {
        protected override Rectangle Layout(Topic root, MindMapLayoutArgs e)
        {
            if (root == null)
                throw new ArgumentNullException();

            return LayoutRoot(root, e);
        }

        Rectangle LayoutRoot(Topic root, MindMapLayoutArgs args)
        {
            Size size = CalculateNodeSize(root, args);
            Point pt = new Point(0, 0);
            root.Bounds = new Rectangle(pt.X - size.Width / 2, pt.Y - size.Height / 2, size.Width, size.Height);
            var rootFullSize = LayoutAttachments(root, args);

            Vector4[] vectors = new Vector4[] { Vector4.Left, Vector4.Right };
            int def = 0;
            int sideCount = Math.DivRem(root.Children.Count, vectors.Length, out def);

            Rectangle allBounds = root.Bounds;
            int subTopicIndex = 0;
            root.Lines.Clear();
            Hashtable layoutInfos = new Hashtable();
            for (int vi = 0; vi < vectors.Length; vi++)
            {
                int mySideCount = sideCount;
                if (vi < def)
                    mySideCount++;

                int[] subSizes = new int[mySideCount];
                Topic[] subTopics = new Topic[mySideCount];
                for (int ti = 0; ti < mySideCount; ti++)
                {
                    subTopics[ti] = root.Children[subTopicIndex + ti];
                }

                Size fullSize = CalculateSizes(root, rootFullSize, subTopics, args, vectors[vi], layoutInfos);
                Rectangle rectFull = LayoutSubTopics(root, subTopics, vectors[vi], layoutInfos, args);
                //Rectangle fullRectangle;
                //switch(vectors[vi])
                //{
                //    case HorizontalVector.Left:
                //        fullRectangle = new Rectangle(pt.X - fullSize.Width, pt.Y - fullSize.Height/2, fullSize.Width, fullSize.Height);
                //        break;
                //    case HorizontalVector.Right:
                //    default:
                //        fullRectangle = new Rectangle(pt.X, pt.Y - fullSize.Height/2, fullSize.Width, fullSize.Height);
                //        break;
                //}

                subTopicIndex += mySideCount;
                if (!rectFull.IsEmpty)
                    allBounds = Rectangle.Union(allBounds, rectFull);
            }

            return allBounds;
        }

        Size CalculateSizes(Topic parent, Size parentFullSize, Topic[] subTopics, MindMapLayoutArgs e, Vector4 vector, Hashtable layoutInfos)
        {
            Size fullSize = parentFullSize;// parent.Size;

            if (subTopics != null && subTopics.Length > 0)
            {
                bool first = true;
                foreach (Topic subTopic in subTopics)
                {
                    subTopic.Size = CalculateNodeSize(subTopic, e);
                    var subTopicFullSize = LayoutAttachments(subTopic, e);
                    Size subSize = CalculateSizes(subTopic, subTopicFullSize, e, vector, layoutInfos);
                    if (first)
                    {
                        fullSize.Height = Math.Max(parent.Size.Height, subSize.Height);
                        first = false;
                    }
                    else
                    {
                        fullSize.Width = Math.Max(fullSize.Width, subSize.Width);
                        fullSize.Height += subSize.Height + e.ItemsSpace;
                        //fullSize.Height += subSize.Height + (int)((parent.IsRoot ? NodeSpaceRoot_V : NodeSpace_V) * args.Zoom);
                    }
                }

                fullSize.Width += e.LayerSpace + parent.Size.Width / 2;
            }

            layoutInfos[parent] = new TopicLayoutInfo(fullSize);
            return fullSize;
        }

        Size CalculateSizes(Topic parent, Size parentFullSize, MindMapLayoutArgs e, Vector4 vector, Hashtable layoutInfos)
        {
            if (parent.Folded)
                return CalculateSizes(parent, parentFullSize, null, e, vector, layoutInfos);
            else
                return CalculateSizes(parent, parentFullSize, parent.Children.ToArray(), e, vector, layoutInfos);
        }

        Rectangle LayoutSubTopics(Topic parent, Topic[] subTopics, Vector4 vector, Hashtable layoutInfos, MindMapLayoutArgs e)
        {
            if (parent == null)
                throw new ArgumentNullException();

            if (!layoutInfos.Contains(parent))
                return Rectangle.Empty;

            if (parent.Folded || parent.Children.IsEmpty)
                return Rectangle.Empty;

            int nodeSpace = e.ItemsSpace;
            if (parent.IsRoot)
            {
                nodeSpace = GetRootItemsSpace(parent, subTopics, layoutInfos, e);
            }

            // get full height
            //int nodeSpace = (int)((parent.IsRoot ? NodeSpaceRoot_V : NodeSpace_V) * args.Zoom);
            int fullHeight = 0;
            for (int i = 0; i < subTopics.Length; i++)
            {
                if (i > 0)
                    fullHeight += nodeSpace;
                fullHeight += ((TopicLayoutInfo)layoutInfos[subTopics[i]]).FullSize.Height;
            }

            //
            Rectangle rectFull = Rectangle.Empty;
            Point pp = PaintHelper.CenterPoint(parent.Bounds);
            int y = pp.Y - (fullHeight / 2);
            //int space = (int)(NodeSpace_H * args.Zoom);
            int space = e.LayerSpace;
            foreach (var subTopic in subTopics)
            {
                var subTif = (TopicLayoutInfo)layoutInfos[subTopic];
                subTopic.Vector = vector == Vector4.Left ? Vector4.Left : Vector4.Right;
                Point pt;
                switch (vector)
                {
                    case Vector4.Left:
                        pt = new Point(parent.Bounds.Left - space - subTopic.Bounds.Width, y + (subTif.FullSize.Height) / 2);
                        break;
                    case Vector4.Right:
                    default:
                        pt = new Point(parent.Bounds.Right + space, y + (subTif.FullSize.Height) / 2);
                        break;
                }
                subTopic.Location = new Point(pt.X, pt.Y - subTopic.Size.Height / 2);

                int foldBtnSize = FoldingButtonSize;
                switch (vector)
                {
                    case Vector4.Left:
                        subTopic.FoldingButton = new Rectangle(subTopic.Left - foldBtnSize - 1,
                            subTopic.Top + (int)Math.Round((subTopic.Height - foldBtnSize) / 2.0f, MidpointRounding.AwayFromZero), 
                            foldBtnSize,
                            foldBtnSize);
                        break;
                    case Vector4.Right:
                    default:
                        subTopic.FoldingButton = new Rectangle(subTopic.Right + 1,
                            subTopic.Top + (int)Math.Round((subTopic.Height - foldBtnSize) / 2.0f, MidpointRounding.AwayFromZero), 
                            foldBtnSize, 
                            foldBtnSize);
                        break;
                }

                // line
                var line = CreateTopicLine(e, parent, subTopic, vector, GetReverseVector(vector));
                if (line != null)
                {
                    parent.Lines.Add(line);
                }
                //parent.Lines.Add(new TopicLine(subTopic, vector, GetReverseVector(vector)));

                subTopic.Lines.Clear();
                Rectangle rectFullSub = LayoutSubTopics(subTopic, subTopic.Children.ToArray(), vector, layoutInfos, e);

                rectFull = Rectangle.Union(rectFull, subTopic.Bounds);
                if (!rectFullSub.IsEmpty)
                    rectFull = Rectangle.Union(rectFull, rectFullSub);

                y += subTif.FullSize.Height + nodeSpace;
            }

            return rectFull;
        }

        /// <summary>
        /// 计算跟节点下的各个子节点之间的合适距离
        /// </summary>
        /// <param name="subTopics">子节点集合</param>
        /// <param name="layerSpace"></param>
        /// <param name="minSpace">最小距离</param>
        /// <returns></returns>
        int GetRootItemsSpace(Topic parent, Topic[] subTopics, Hashtable layoutInfos, MindMapLayoutArgs e)
        {
            if (subTopics == null || subTopics.Length == 0)
                return 0;

            int layerSpace = e.LayerSpace;

            int count = subTopics.Length;
            double angle = Math.PI / (count + 1) * (count - 1);
            int c1 = (int)Math.Ceiling(Math.Sin(angle / 2) * layerSpace * 2);

            for (int i = 0; i < count; i++)
            {
                int h = subTopics[i].Height;// ((TopicLayoutInfo)layoutInfos[subTopics[i]]).FullSize.Height;
                if (i == 0 || i == count - 1)
                    c1 -= h / 2;
                else
                    c1 -= h;
            }

            return Math.Max(c1, e.ItemsSpace);
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
                switch (beginSide)
                {
                    case Vector4.Left:
                        beginRect.X -= foldBtnSize;
                        beginRect.Width += foldBtnSize;
                        break;
                    case Vector4.Right:
                        beginRect.Width += foldBtnSize;
                        break;
                    case Vector4.Top:
                        beginRect.Y -= foldBtnSize;
                        beginRect.Height += foldBtnSize;
                        break;
                    case Vector4.Bottom:
                        beginRect.Height += foldBtnSize;
                        break;
                }
            }

            if (endTopic != null && endTopic.Style.Shape == TopicShape.BaseLine)
            {
                endRect.Y = endRect.Bottom;
                endRect.Height = 0;
            }

            return new TopicLine(endTopic, beginSide, endSide, beginRect, endRect);
        }

        //public override void AdjustLineRect(TopicLine line, Topic fromTopic, ref Rectangle rectFrom, ref Rectangle rectTo)
        //{
        //    base.AdjustLineRect(line, fromTopic, ref rectFrom, ref rectTo);

        //    if (fromTopic != null && !fromTopic.IsRoot)
        //    {
        //        int foldBtnSize = line.Target.FoldingButton.Width;
        //        switch (line.BeginSide)
        //        {
        //            case Vector4.Left:
        //                rectFrom.X -= foldBtnSize;
        //                rectFrom.Width += foldBtnSize;
        //                break;
        //            case Vector4.Right:
        //                rectFrom.Width += foldBtnSize;
        //                break;
        //            case Vector4.Top:
        //                rectFrom.Y -= foldBtnSize;
        //                rectFrom.Height += foldBtnSize;
        //                break;
        //            case Vector4.Bottom:
        //                rectFrom.Height += foldBtnSize;
        //                break;
        //        }
        //    }

        //    if (line.Target != null && line.Target.Style.Shape == Core.Styles.TopicShape.BaseLine)
        //    {
        //        rectTo.Y = rectTo.Bottom;
        //        rectTo.Height = 0;
        //    }
        //}

        public override Topic GetNextNode(Topic from, MoveVector vector)
        {
            Topic next = null;

            if (from.IsRoot)
            {
                switch (vector)
                {
                    case MoveVector.Left:
                    case MoveVector.Up:
                        foreach (Topic subTopic in from.Children)
                        {
                            if (subTopic.Vector == Vector4.Left)
                            {
                                next = subTopic;
                                break;
                            }
                        }
                        break;
                    case MoveVector.Right:
                    case MoveVector.Down:
                        foreach (Topic subTopic in from.Children)
                        {
                            if (subTopic.Vector == Vector4.Right)
                            {
                                next = subTopic;
                                break;
                            }
                        }
                        break;
                }
            }
            else
            {
                switch (vector)
                {
                    case MoveVector.Left:
                        if (from.Vector == Vector4.Left)
                            next = from.HasChildren ? from.Children[0] : null;
                        else
                            next = from.ParentTopic;
                        break;
                    case MoveVector.Right:
                        if (from.Vector == Vector4.Right)
                            next = from.HasChildren ? from.Children[0] : null;
                        else
                            next = from.ParentTopic;
                        break;
                    case MoveVector.Up:
                        next = from.GetSibling(false, false, true);
                        break;
                    case MoveVector.Down:
                        next = from.GetSibling(true, false, true);
                        break;
                }
            }

            return next;
        }
    }
}
