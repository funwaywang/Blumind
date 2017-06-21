using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Globalization;
using Blumind.Model.Documents;
using Blumind.Model.Styles;
using Blumind.Model.Widgets;

namespace Blumind.Model.MindMaps
{
    [DefaultProperty("Text")]
    partial class Topic : ChartObject
        , ITextObject
        , IWidgetContainer
        , IHyperlink
        , Blumind.Core.INotifyPropertyChanged
        , IColorToolTip
    {
        public Topic()
        {
            Style = new TopicStyle();
            Lines = new List<TopicLine>();
            Links = new XList<Link>();
            Children = new XList<Topic>();
            Widgets = new XList<Widget>();
        }

        public Topic(string text)
            : this()
        {
            Text = text;
        }

        #region properties
        bool _Folded;
        object _Tag;
        Topic _ParentTopic;
        int _Index = -1;
        int _Level;
        XList<Topic> _Children;
        //Image _Icon;
        Vector4 _Vector;
        //string _Description;
        TopicStyle _Style;
        List<TopicLine> _Lines;
        XList<Link> _Links;
        XList<Widget> _Widgets;

        public event EventHandler StyleChanged;
        //public event Blumind.Model.PropertyChangedEventHandler WidgetChanged;
        public event EventHandler FoldedChanged;
        public event EventHandler DescriptionChanged;
        public event EventHandler IconChanged;
        public event EventHandler CustomWidthChanged;
        public event EventHandler CustomHeightChanged;
        public event CancelEventHandler BeforeExpand;
        public event CancelEventHandler BeforeCollapse;
        
        [Browsable(false)]
        public MindMap MindMap
        {
            get { return Chart as MindMap; }
        }

        [DefaultValue(false), Browsable(false)]
        public bool Folded
        {
            get { return _Folded; }
            set 
            {
                if (_Folded != value)
                {
                    CancelEventArgs cea = new CancelEventArgs();
                    if (value)
                        OnBeforeCollapse(cea);
                    else
                        OnBeforeExpand(cea);

                    if (cea.Cancel)
                        return;

                    _Folded = value;
                    OnFoldedChanged();
                }
            }
        }

        [DefaultValue(null), Browsable(false)]
        public object Tag
        {
            get { return _Tag; }
            set { _Tag = value; }
        }

        [DefaultValue(null), Browsable(false)]
        public Topic ParentTopic
        {
            get { return _ParentTopic; }
            set 
            {
                if (_ParentTopic != value)
                {
                    Topic old = _ParentTopic;
                    _ParentTopic = value;
                    OnParentTopicChanged(old);
                }
            }
        }

        public override ChartObject Parent
        {
            get
            {
                return _ParentTopic;
            }
            set
            {
                if (value is Topic)
                {
                    ParentTopic = value as Topic;
                }
            }
        }

        [DefaultValue(-1), Browsable(false)]
        public int Index
        {
            get { return _Index; }
            private set { _Index = value; }
        }

        [DefaultValue(0), Browsable(false)]
        public int Level
        {
            get { return _Level; }
            private set 
            {
                if (_Level != value)
                {
                    _Level = value;
                    OnLevelChanged();
                }
            }
        }

        [Browsable(false)]
        public XList<Topic> Children
        {
            get { return _Children; }
            private set 
            {
                if (_Children != value)
                {
                    var old = _Children;
                    _Children = value;
                    OnChildrenChanged(old);
                }
            }
        }

        [DefaultValue(null)]
        [LocalDisplayName("Icon"), LocalCategory("Data")]
        public Blumind.Model.Widgets.PictureWidget.PictureDesign Icon
        {
            get 
            {
                PictureWidget pw = FindWidget<PictureWidget>();
                if (pw != null)
                    return pw.Image;
                else
                    return null;
            }

            set 
            {
                PictureWidget pw = FindWidget<PictureWidget>();
                if (pw != null)
                {
                    pw.Image = value;
                }
                else
                {
                    pw = new PictureWidget();
                    pw.Image = value;
                    Add(pw);
                }
                //if (_Icon != value)
                //{
                //    _Icon = value;
                //    OnIconChanged();
                //}
            }
        }

        [DefaultValue(Vector4.Left)]
        internal Vector4 Vector
        {
            get { return _Vector; }
            set { _Vector = value; }
        }

        [Browsable(false)]
        public bool IsRoot
        {
            get
            {
                return ParentTopic == null;
            }
        }

        [Browsable(false)]
        public Topic PreviousSibling
        {
            get
            {
                if (ParentTopic != null && Index > 0)
                    return ParentTopic.Children[Index - 1];
                else
                    return null;
            }
        }

        [Browsable(false)]
        public Topic NextSibling
        {
            get
            {
                if (ParentTopic != null && Index < ParentTopic.Children.Count - 1)
                    return ParentTopic.Children[Index + 1];
                else
                    return null;
            }
        }

        [Browsable(false)]
        public Topic LastChild
        {
            get
            {
                if (HasChildren)
                    return Children[Children.Count - 1];
                else
                    return null;
            }
        }

        [Browsable(false)]
        public Topic FirstChild
        {
            get
            {
                if (HasChildren)
                    return Children[0];
                else
                    return null;
            }
        }

        [Browsable(false)]
        public bool HasChildren
        {
            get
            {
                return Children.Count > 0;
            }
        }

        [DefaultValue(null)]
        [Browsable(false), LocalDisplayName("Style")]
        public TopicStyle Style
        {
            get { return _Style; }
            private set 
            {
                if (_Style != value && value != null)
                {
                    _Style = value;
                    OnStyleChanged();
                }
            }
        }

        [Browsable(false)]
        public List<TopicLine> Lines
        {
            get { return _Lines; }
            private set { _Lines = value; }
        }
        
        [Browsable(false)]
        public XList<Link> Links
        {
            get { return _Links; }
            private set
            {
                if (_Links != value)
                {
                    var old = _Links;
                    _Links = value;
                    OnLinksChanged(old);
                }
            }
        }

        [Browsable(false)]
        internal XList<Widget> Widgets
        {
            get { return _Widgets; }
            private set
            {
                if (_Widgets != value)
                {
                    var old = _Widgets;
                    _Widgets = value;
                    OnWidgetsChanged(old);
                }
            }
        }

        [Browsable(false)]
        public bool Visible
        {
            get
            {
                if (ParentTopic == null)
                    return true;
                else if (ParentTopic.Folded)
                    return false;
                else
                    return ParentTopic.Visible;
            }
        }

        [Browsable(false)]
        public Color RealBackColor
        {
            get
            {
                if (!BackColor.IsEmpty)
                    return BackColor;
                else if (ParentTopic != null && !ParentTopic.IsRoot)
                    return ParentTopic.RealBackColor;
                else if (MindMap != null)
                    return this.MindMap.NodeBackColor;
                else
                    return Color.Empty;
            }
        }

        void OnStyleChanged()
        {
            if (Style != null)
            {
                Style.ValueChanged += new EventHandler(Style_ValueChanged);
            }

            Style_ValueChanged(this, EventArgs.Empty);
        }

        void Style_ValueChanged(object sender, EventArgs e)
        {
            if (StyleChanged != null)
            {
                StyleChanged(this, EventArgs.Empty);
            }

            if (MindMap != null)
            {
                //MindMap.OnTopicStyleChanged(this);
                MindMap.OnObjectStyleChanged(this, null, ChangeTypes.Layout | ChangeTypes.Visual);
            }
        }

        void OnFoldedChanged()
        {
            if (FoldedChanged != null)
            {
                FoldedChanged(this, EventArgs.Empty);
            }

            if (MindMap != null)
            {
                MindMap.OnTopicFoldedChanged(this);
            }
        }

        void OnIconChanged()
        {
            if (IconChanged != null)
            {
                IconChanged(this, EventArgs.Empty);
            }

            if (MindMap != null)
            {
                MindMap.OnTopicIconChanged(this);
            }
        }

        void OnDescriptionChanged()
        {
            if (DescriptionChanged != null)
            {
                DescriptionChanged(this, EventArgs.Empty);
            }

            if (MindMap != null)
            {
                MindMap.OnTopicDescriptionChanged(this);
            }
        }

        protected override void OnChartChanged()
        {
            base.OnChartChanged();

            foreach (var subTopic in Children)
            {
                subTopic.Chart = this.Chart;
            }

            foreach (var widget in Widgets)
            {
                widget.Chart = this.Chart;
            }

            foreach (var link in Links)
            {
                link.Chart = this.Chart;
            }
        }

        void OnLevelChanged()
        {
            foreach (Topic subTopic in Children)
            {
                subTopic.Level = Level + 1;
            }
        }

        void OnParentTopicChanged(Topic old)
        {
            if (old != null && old.Children.Contains(this))
            {
                old.Children.Remove(this);
            }

            if (ParentTopic != null && !ParentTopic.Children.Contains(this))
            {
                ParentTopic.Children.Add(this);
            }

            FoldingButtonVisible = !IsRoot && Children.Count > 0;
            Parent = ParentTopic;
        }

        void OnBeforeExpand(CancelEventArgs e)
        {
            if (BeforeExpand != null)
                BeforeExpand(this, e);

            if (!e.Cancel && MindMap != null)
            {
                TopicCancelEventArgs ce = new TopicCancelEventArgs(this);
                MindMap.OnTopicBeforeExpand(ce);
                e.Cancel = ce.Cancel;
            }
        }

        void OnBeforeCollapse(CancelEventArgs e)
        {
            if (BeforeCollapse != null)
                BeforeCollapse(this, e);

            if (!e.Cancel && MindMap != null)
            {
                TopicCancelEventArgs ce = new TopicCancelEventArgs(this);
                MindMap.OnTopicBeforeCollapse(ce);
                e.Cancel = ce.Cancel;
            }
        }

        void OnWidthChanged()
        {
            if (CustomWidthChanged != null)
            {
                CustomWidthChanged(this, EventArgs.Empty);
            }

            if (MindMap != null)
            {
                MindMap.OnTopicWidthChanged(this);
            }
        }

        void OnHeightChanged()
        {
            if (CustomHeightChanged != null)
            {
                CustomHeightChanged(this, EventArgs.Empty);
            }

            if (MindMap != null)
            {
                MindMap.OnTopicHeightChanged(this);
            }
        }

        private void OnWidgetChanged(object sender, Blumind.Core.PropertyChangedEventArgs e)
        {
            //if (WidgetChanged != null)
            //{
            //    WidgetChanged(sender, new ChangeEventArgs(types));
            //}

            if (MindMap != null)
            {
                MindMap.OnTopicWidgetChanged(this, sender as Widget, e);
            }
        }

        #endregion

        #region IHyperlink 
        private string _Url = null;

        public event EventHandler HyperlinkChanged;

        [DefaultValue(null), LocalDisplayName("Hyperlink"), LocalCategory("Data")]
        [Editor(typeof(Blumind.Design.HyperlinkEditor), typeof(UITypeEditor))]
        public string Hyperlink
        {
            get
            {
                return _Url;
            }
            set
            {
                if (_Url != value)
                {
                    string old = _Url;
                    _Url = value;
                    OnHyperlinkChanged(old);
                }
            }
        }

        [DefaultValue(null)]
        protected virtual void OnHyperlinkChanged(string old)
        {
            if (HyperlinkChanged != null)
            {
                HyperlinkChanged(this, EventArgs.Empty);
            }

            OnPropertyChanged(new Blumind.Core.PropertyChangedEventArgs("Hyperlink", old, Hyperlink, ChangeTypes.Data | ChangeTypes.Visual, true));
        }

        #endregion

        #region IColorToolTip 成员

        public int MaxToopTipLength = 128;

        [Browsable(false)]
        public string ToolTip
        {
            get
            {
                //StringBuilder sb = new StringBuilder();
                //NotesWidget[] notes = FindWidgets<NotesWidget>();
                //foreach (NotesWidget note in notes)
                //{
                //    if (!string.IsNullOrEmpty(note.ToolTip))
                //    {
                //        if (sb.Length > 0)
                //            sb.AppendLine();
                //        sb.Append(note.ToolTip);
                //    }
                //}

                ////
                //if (sb.Length > 0)
                //    return sb.ToString();
                //else
                //    return null; 

                var remark = ST.HtmlToText(Remark);
                if (remark != null && remark.Length > MaxToopTipLength)
                {
                    remark = remark.Substring(0, MaxToopTipLength) + "...";
                }

                return ST.HtmlToText(remark);
            }
        }

        bool IColorToolTip.ToolTipShowAlway
        {
            get { return false; }
        }

        string IColorToolTip.ToolTipHyperlinks
        {
            get { return Hyperlink; }
        }

        #endregion

        #region paint
        public event InvalidateEventHandler Invalidated;

        public void Invalidate()
        {
            OnInvalidate(Rectangle.Empty);
        }

        public void InvalidateRect(Rectangle rect)
        {
            OnInvalidate(rect);
        }

        private void OnInvalidate(Rectangle rectangle)
        {
            if (Invalidated != null)
            {
                Invalidated(this, new InvalidateEventArgs(rectangle));
            }
        }

        #endregion

        void Children_ItemRemoved(object sender, XListEventArgs<Topic> e)
        {
            if (e.Item != null && e.Item.ParentTopic == this)
            {
                e.Item.ParentTopic = null;
            }

            if (MindMap != null)
            {
                MindMap.OnTopicRemoved(e.Item);
            }

            OnItemsChanged();
        }

        void Children_ItemAdded(object sender, XListEventArgs<Topic> e)
        {
            if (e.Item != null && e.Item.ParentTopic != this)
            {
                e.Item.ParentTopic = this;
                e.Item.Chart = this.MindMap;
                e.Item.Level = this.Level + 1;
            }

            if (MindMap != null)
            {
                MindMap.OnTopicAdded(e.Item);
            }

            OnItemsChanged();
        }

        void Children_AfterSort(object sender, EventArgs e)
        {
            if (MindMap != null)
            {
                MindMap.OnTopicAfterSort();
            }

            OnItemsChanged();
        }

        void Links_ItemAdded(object sender, XListEventArgs<Link> e)
        {
            if (e.Item != null)
            {
                e.Item.From = this;
                e.Item.Chart = this.Chart;
            }

            if (MindMap != null)
            {
                MindMap.OnLineAdded(e.Item);
            }
        }

        void Links_ItemRemoved(object sender, XListEventArgs<Link> e)
        {
            if (e.Item != null && e.Item.From == this)
            {
                e.Item.From = null;
            }

            if (MindMap != null)
            {
                MindMap.OnLineRemoved(e.Item);
            }
        }

        void Links_BeforeClear(object sender, EventArgs e)
        {
            foreach (Link line in Links)
            {
                if (line != null && line.From == this)
                {
                    line.From = null;
                }
            }
        }

        void OnChildrenChanged(XList<Topic> old)
        {
            if (old != null)
            {
                old.ItemAdded -= new XListEventHandler<Topic>(Children_ItemAdded);
                old.ItemRemoved -= new XListEventHandler<Topic>(Children_ItemRemoved);
                old.AfterSort -= new EventHandler(Children_AfterSort);
            }

            if (Children != null)
            {
                Children.ItemAdded += new XListEventHandler<Topic>(Children_ItemAdded);
                Children.ItemRemoved += new XListEventHandler<Topic>(Children_ItemRemoved);
                Children.AfterSort += new EventHandler(Children_AfterSort);
            }
        }

        void OnWidgetsChanged(XList<Widget> old)
        {
            if (old != null)
            {
                Widgets.ItemAdded -= new XListEventHandler<Widget>(Widgets_ItemAdded);
                Widgets.ItemRemoved -= new XListEventHandler<Widget>(Widgets_ItemRemoved);
            }

            if (Widgets != null)
            {
                Widgets.ItemAdded += new XListEventHandler<Widget>(Widgets_ItemAdded);
                Widgets.ItemRemoved += new XListEventHandler<Widget>(Widgets_ItemRemoved);
            }
        }

        void OnLinksChanged(XList<Link> old)
        {
            if (old != null)
            {
                old.ItemAdded -= new XListEventHandler<Link>(Links_ItemAdded);
                old.ItemRemoved -= new XListEventHandler<Link>(Links_ItemRemoved);
                old.BeforeClear -= new EventHandler(Links_BeforeClear);
            }

            if (Links != null)
            {
                Links.ItemAdded += new XListEventHandler<Link>(Links_ItemAdded);
                Links.ItemRemoved += new XListEventHandler<Link>(Links_ItemRemoved);
                Links.BeforeClear += new EventHandler(Links_BeforeClear);
            }
        }

        void OnItemsChanged()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Index = i;
            }

            FoldingButtonVisible = !IsRoot && Children.Count > 0;
        }

        public int GetMaxLevelSize()
        {
            if (Children.IsEmpty)
            {
                return 1;
            }
            else
            {
                int mls = 0;
                foreach (Topic subTopic in Children)
                {
                    mls += subTopic.GetMaxLevelSize();
                }

                return mls;
            }
        }

        public override string ToString()
        {
            return Text;
        }

        public Topic GetSibling(bool forward, bool nested, bool wrap)
        {
            if (ParentTopic == null)
                return null;

            if (nested)
            {
                return GetNestedSibling(forward, wrap);
            }
            else
            {
                Topic next = GetNestedSibling(forward, false);
                if (next != null)
                    return next;

                next = ParentTopic.GetSibling(this, forward, Level, Vector);
                if (next != null)
                    return next;

                if (wrap && MindMap != null && MindMap.Root != null)
                    next = MindMap.Root.GetFirstTopic(forward, Level, Vector);

                if (next == this)
                    return null;
                else
                    return next;
            }
        }

        Topic GetSibling(Topic from, bool forward, int level, Vector4 vector)
        {
            if (this.Level == level && this.Vector == vector)
                return this;

            if (forward)
            {
                if (HasChildren)
                {
                    if (Children.Contains(from) && from != LastChild)
                        return from.NextSibling.GetSibling(this, forward, level, vector);
                    else if(from == ParentTopic)
                        return FirstChild.GetSibling(this, forward, level, vector);
                }
            }
            else
            {
                if (HasChildren)
                {
                    if(Children.Contains(from) && from != FirstChild)
                        return from.PreviousSibling.GetSibling(this, forward, level, vector);
                    else if(from == ParentTopic)
                        return LastChild.GetSibling(this, forward, level, vector);
                }
            }

            if (ParentTopic == null)
                return null;
            else
                return ParentTopic.GetSibling(this, forward, level, vector);
        }

        Topic GetNestedSibling(bool forward, bool wrap)
        {
            if(ParentTopic == null)
                return null;

            if (forward)
            {
                if (Index < ParentTopic.Children.Count - 1)
                    return ParentTopic.Children[Index + 1];
                else if (wrap && Index != 0)
                    return ParentTopic.Children[0];
            }
            else
            {
                if (Index > 0)
                    return ParentTopic.Children[Index - 1];
                else if (wrap && Index != ParentTopic.Children.Count - 1)
                    return ParentTopic.Children[ParentTopic.Children.Count - 1];
            }

            return null;
        }

        Topic GetFirstTopic(bool front, int level, Vector4 vector)
        {
            if (this.Level == level && Vector != vector)
                return this;

            if (HasChildren)
            {
                if (front)
                    return FirstChild.GetSibling(this, true, level, vector);
                else
                    return LastChild.GetSibling(this, false, level, vector);
            }
            else
            {
                return null;
            }
        }

        public void Toggle()
        {
            Folded = !Folded;
        }

        public void Expand()
        {
            Folded = false;
        }

        public void Collapse()
        {
            Folded = true;
        }

        public void CollapseAll()
        {
            foreach (Topic topic in Children)
            {
                topic.CollapseAll();
            }

            Collapse();
        }

        public void ExpandAll()
        {
            Expand();

            foreach (Topic topic in Children)
            {
                topic.ExpandAll();
            }
        }

        public bool IsDescent(Topic topic)
        {
            if (topic == null)
                return false;

            foreach (Topic subTopic in Children)
            {
                if (subTopic == topic || subTopic.IsDescent(topic))
                    return true;
            }

            return false;
        }

        public bool IsAncestor(Topic topic)
        {
            if (topic == null || ParentTopic == null)
                return false;

            if (ParentTopic == topic || ParentTopic.IsAncestor(topic))
                return true;

            return false;
        }

        public bool Remove(Topic topic)
        {
            if (Children.Contains(topic))
                return Children.Remove(topic);

            return false;
        }

        public bool Remove(Link link)
        {
            if (Links.Contains(link))
                return Links.Remove(link);

            return false;
        }

        public bool TryRemove(ChartObject chartObject)
        {
            if (chartObject == null)
                throw new ArgumentNullException();

            if (chartObject is Topic)
            {
                return Remove((Topic)chartObject);
            }
            else if (chartObject is Widget)
            {
                return Remove((Widget)chartObject);
            }
            else if (chartObject is Link)
            {
                return Remove((Link)chartObject);
            }

            return false;
        }

        #region Clone
        public Topic Clone()
        {
            var exchangeTable = new Dictionary<Topic, Topic>();
            var topic = Clone(exchangeTable);
            CloneLinks(exchangeTable);

            return topic;
        }

        Topic Clone(Dictionary<Topic, Topic> exchangeTable)
        {
            var topic = (Topic)this.MemberwiseClone();
            if (MindMap != null)
                topic.ID = this.MindMap.GetNextObjectID();
            else
                topic.ID = null;
            topic.Chart = null;
            topic.Style = (TopicStyle)Style.Clone();
            topic.Children = new XList<Topic>();
            topic.Links = new XList<Link>();
            topic.Widgets = new XList<Widget>();
            topic.Lines = new List<TopicLine>();
            exchangeTable.Add(this, topic);

            foreach (Topic subTopic in this.Children)
            {
                topic.Children.Add(subTopic.Clone(exchangeTable));
            }

            foreach (var widget in this.Widgets)
            {
                var newWidget = widget.Clone();
                if(newWidget != null)
                    topic.Widgets.Add(widget.Clone());
            }

            return topic;
        }

        void CloneLinks(Dictionary<Topic, Topic> exchangeTable)
        {
            foreach (var link in Links)
            {
                if (exchangeTable.ContainsKey(link.From))
                {
                    var lk = link.Clone();
                    lk.From = exchangeTable[link.From];
                    if (exchangeTable.ContainsKey(link.Target))
                        lk.Target = exchangeTable[link.Target];
                }
            }

            foreach (var subTopic in Children)
            {
                subTopic.CloneLinks(exchangeTable);
            }
        }

        public void CutFromMap()
        {
            ParentTopic = null;

            var nodes = this.GetDescendants().ToList();
            nodes.Add(this);

            this.ClearOutputLinks(nodes);
        }

        void ClearOutputLinks(List<Topic> nodes)
        {
            Links.RemoveAll(link => link == null || !nodes.Contains(link.Target));

            foreach (var sub in Children)
            {
                sub.ClearOutputLinks(nodes);
            }
        }

        #endregion

        public IEnumerable<Topic> GetDescendants()
        {
            foreach (var sub in Children)
            {
                yield return sub;

                foreach (var des in sub.GetDescendants())
                    yield return des;
            }
        }

        public Topic FindChildByText(string text)
        {
            foreach (Topic topic in Children)
            {
                if (StringComparer.OrdinalIgnoreCase.Equals(topic.Text, text))
                    return topic;
            }

            return null;
        }

        /// <summary>
        /// 不要直接在代码里使用, 自定义顺序请使用CustomSortCommand
        /// </summary>
        /// <param name="newIndices"></param>
        internal void SortChildren(int[] newIndices)
        {
            Children.SortAs(newIndices);
        }

        public Topic GetRoot()
        {
            Topic topic = this;
            while (topic.ParentTopic != null)
            {
                topic = topic.ParentTopic;
            }

            return topic;
        }

        public void SetIcon(Image image)
        {
            PictureWidget pw = FindWidget<PictureWidget>();
            if (pw == null)
            {
                pw = new PictureWidget();
                pw.Data = image;
                Widgets.Add(pw);
            }
            else
            {
                pw.Data = image;
            }
        }

        #region Widgets

        internal T FindWidget<T>()
            where T : Widget
        {
            foreach (Widget widget in Widgets)
            {
                if (widget is T)
                    return (T)widget;
            }

            return null;
        }

        internal T[] FindWidgets<T>()
            where T : Widget
        {
            List<T> list = new List<T>();
            foreach (Widget widget in Widgets)
            {
                if (widget is T)
                    list.Add((T)widget);
            }

            return list.ToArray();
        }

        public Widget[] FindWidgets(WidgetAlignment alignment)
        {
            return (from w in Widgets
                    where w.Alignment == alignment
                    orderby w.DisplayIndex
                    select w).ToArray();
        }

        public void Add(Widget widget)
        {
            Widgets.Add(widget);
        }

        public bool Remove(Widget widget)
        {
            if (Widgets.Contains(widget))
                return Widgets.Remove(widget);

            return false;
        }

        public int IndexOf(Widget widget)
        {
            return Widgets.IndexOf(widget);
        }

        public void Insert(int index, Widget widget)
        {
            if (index > -1 && index < Widgets.Count)
                Widgets.Insert(index, widget);
            else
                throw new ArgumentOutOfRangeException();
        }

        [Browsable(false)]
        public int WidgetsCount
        {
            get { return Widgets.Count; }
        }

        private void Widgets_ItemAdded(object sender, XListEventArgs<Widget> e)
        {
            if (e.Item != null)
            {
                e.Item.WidgetContainer = this;
                e.Item.Chart = this.Chart;
                e.Item.PropertyChanged += new Blumind.Core.PropertyChangedEventHandler(Widget_Changed);
            }

            Blumind.Core.PropertyChangedEventArgs arg = new Blumind.Core.PropertyChangedEventArgs("Items", null, null, ChangeTypes.All, false);
            OnPropertyChanged(arg);
            //OnWidgetChanged(sender, PropertyChangeTypes.All);
        }

        void Widgets_ItemRemoved(object sender, XListEventArgs<Widget> e)
        {
            if (e.Item != null)
            {
                if (e.Item.WidgetContainer == this)
                    e.Item.WidgetContainer = null;
                e.Item.PropertyChanged -= new Blumind.Core.PropertyChangedEventHandler(Widget_Changed);
            }

            if (MindMap != null)
            {
                MindMap.OnWidgetRemoved(e.Item);
            }
        }

        void Widget_Changed(object sender, Blumind.Core.PropertyChangedEventArgs e)
        {
            OnWidgetChanged(sender as Widget, e);
        }

        #endregion

        public IEnumerable<Topic> GetPathArray()
        {
            List<Topic> path = new List<Topic>();
            path.Add(this);

            var p = this.ParentTopic;
            while (p != null)
            {
                path.Insert(0, p);
                p = p.ParentTopic;
            }

            return path;
        }

        public Topic GetChildByText(string text)
        {
            foreach (Topic t in Children)
            {
                if (t.Text.Equals(text, StringComparison.OrdinalIgnoreCase))
                    return t;
            }

            return null;
        }

        #region I/O
        public const string _XmlElementName = "node";

        public override string XmlElementName
        {
            get { return _XmlElementName; }
        }

        public override void Serialize(XmlDocument dom, XmlElement node)
        {
            base.Serialize(dom, node);

            if (!string.IsNullOrEmpty(this.Hyperlink))
                node.SetAttribute("hyperlink", this.Hyperlink);
            if (this.Folded)
                node.SetAttribute("folded", this.Folded.ToString());

            if (this.CustomWidth.HasValue)
                node.SetAttribute("custom_width", this.CustomWidth.Value.ToString());
            if (this.CustomHeight.HasValue)
                node.SetAttribute("custom_height", this.CustomHeight.Value.ToString());

            //
            if(!TextBounds.IsEmpty)
                node.SetAttribute("text_b", ST.ToString(TextBounds));
            if(!RemarkIconBounds.IsEmpty)
                node.SetAttribute("remark_b", ST.ToString(RemarkIconBounds));
            if (FoldingButtonVisible && !FoldingButton.IsEmpty)
                node.SetAttribute("fold_btn_b", ST.ToString(FoldingButton));

            var styleNode = dom.CreateElement("style");
            this.Style.Serialize(dom, styleNode);
            if (styleNode.Attributes.Count > 0 || styleNode.ChildNodes.Count > 0)
            {
                node.AppendChild(styleNode);
            }

            // links
            if (!this.Links.IsEmpty)
            {
                var links = dom.CreateElement("links");
                foreach (Link link in this.Links)
                {
                    if (link.Target == null || link.Target == link.From)
                        continue;

                    var linkNode = dom.CreateElement(Link._XmlElementName);
                    link.Serialize(dom, linkNode);
                    links.AppendChild(linkNode);
                }

                if (links.ChildNodes.Count != 0)
                    node.AppendChild(links);
            }

            // widgets
            if (!this.Widgets.IsEmpty)
            {
                var widgets = dom.CreateElement("widgets");
                foreach (var widget in this.Widgets)
                {
                    var widgetNode = dom.CreateElement(Widget._XmlElementName);
                    //widgetNode.SetAttribute("type", widget.GetTypeID());
                    widget.Serialize(dom, widgetNode);
                    widgets.AppendChild(widgetNode);
                }

                if (widgets.ChildNodes.Count != 0)
                    node.AppendChild(widgets);
            }

            // sub-nodes
            if (!this.Children.IsEmpty)
            {
                var nodes = dom.CreateElement("nodes");
                node.AppendChild(nodes);
                foreach (var subTopic in this.Children)
                {
                    var subNode = dom.CreateElement(XmlElementName);//
                    subTopic.Serialize(dom, subNode);
                    nodes.AppendChild(subNode);
                }

                //
                var lines = dom.CreateElement("lines");
                node.AppendChild(lines);
                foreach (var line in this.Lines)
                {
                    var lineNode = dom.CreateElement("line");
                    line.Serialize(dom, lineNode);
                    lines.AppendChild(lineNode);
                }
            }
        }

        public override void Deserialize(Version documentVersion, System.Xml.XmlElement node)
        {
            base.Deserialize(documentVersion, node);

            Hyperlink = node.GetAttribute("hyperlink");
            Folded = ST.GetBoolDefault(node.GetAttribute("folded"));

            //
            if (documentVersion >= Document.DV_3) // 新
            {
                CustomWidth = ST.GetInt(node.GetAttribute("custom_width"));
                CustomHeight = ST.GetInt(node.GetAttribute("custom_height"));
            }
            else
            {
                CustomWidth = ST.GetInt(node.GetAttribute("width"));
                CustomHeight = ST.GetInt(node.GetAttribute("height"));
            }

            TextBounds = ST.GetRectangle(node.GetAttribute("text_b"), TextBounds);
            RemarkIconBounds = ST.GetRectangle(node.GetAttribute("remark_b"), RemarkIconBounds);
            FoldingButton = ST.GetRectangle(node.GetAttribute("fold_btn_b"), FoldingButton);

            XmlElement styleNode = node.SelectSingleNode("style") as XmlElement;
            if (styleNode != null)
            {
                Style.Deserialize(documentVersion, styleNode);
            }

            //
            var linkNodes = node.SelectNodes("links/link");
            foreach (XmlElement linkNode in linkNodes)
            {
                Link link = new Link();
                link.Deserialize(documentVersion, linkNode);
                Links.Add(link);
            }

            //
            var widgetNodes = node.SelectNodes("widgets/widget");
            foreach (XmlElement widgetNode in widgetNodes)
            {
                var widget = Widget.DeserializeWidget(documentVersion, widgetNode);
                if (widget != null)
                    Widgets.Add(widget);
            }

            //
            XmlNodeList nodes = node.SelectNodes("nodes/node");
            foreach (XmlElement subNode in nodes)
            {
                Topic subTopic = new Topic();
                subTopic.Deserialize(documentVersion, subNode);
                Children.Add(subTopic);
            }

            //
            if (!Children.IsNullOrEmpty())
            {
                XmlNodeList lines = node.SelectNodes("lines/line");
                foreach (XmlElement lineNode in lines)
                {
                    var line = new TopicLine();
                    line.Deserialize(documentVersion, lineNode);

                    var targetID = lineNode.GetAttribute("target");
                    var target = Children.Find(c => StringComparer.OrdinalIgnoreCase.Equals(c.ID, targetID));
                    if (target != null)
                    {
                        line.SetTarget(target);
                        Lines.Add(line);
                    }
                }
            }

            //
        }

        public static XmlNode SerializeTopic(XmlDocument dom, Topic topic)
        {
            var node = dom.CreateElement(_XmlElementName);
            topic.Serialize(dom, node);
            return node;
        }

        public static Topic DeserializeTopic(Version documentVersion, XmlElement node)
        {
            if (node == null)
                return null;

            Topic topic = new Topic();
            topic.Deserialize(documentVersion, node);
            return topic;
        }

        public override string SerializeText(bool recursive, List<ChartObject> accessedObjects)
        {
            if (recursive)
            {
                if (accessedObjects != null && !accessedObjects.Contains(this))
                    accessedObjects.Add(this);

                var sb = new StringBuilder();
                SerializeText(sb, this, 0, accessedObjects);
                return sb.ToString();
            }
            else
            {
                return base.SerializeText(recursive, accessedObjects);
            }
        }

        void SerializeText(StringBuilder sb, Topic topic, int level, List<ChartObject> accessedObjects)
        {
            sb.Append('\t', level);
            sb.AppendLine(topic.Text);

            foreach (var subTopic in topic.Children)
            {
                if (accessedObjects != null && accessedObjects.Contains(subTopic))
                    continue;
                accessedObjects.Add(subTopic);

                SerializeText(sb, subTopic, level + 1, accessedObjects);
            }
        }
        #endregion
    }
}
