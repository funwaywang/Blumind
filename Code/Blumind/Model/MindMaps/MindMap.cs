using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Blumind.Controls;
using Blumind.Controls.MapViews;
using Blumind.Core;
using Blumind.Globalization;
using Blumind.Model;
using Blumind.Model.Documents;
using Blumind.Model.Styles;
using Blumind.Model.Widgets;

namespace Blumind.Model.MindMaps
{
    partial class MindMap : ChartPage
    {
        public MindMap()
        {
            NamedTopics = new Dictionary<string, Topic>();
            ExtendAttributes = new Dictionary<string, string>();
            _PictureLibrary = new PictureLibrary();

            Root = new Topic();
            Root.ResetPadding();
        }

        public MindMap(string name)
            : this()
        {
            Name = name;
        }

        public MindMap(Topic root, string name)
        {
            NamedTopics = new Dictionary<string, Topic>();
            ExtendAttributes = new Dictionary<string, string>();
            _PictureLibrary = new PictureLibrary();
            Name = name;

            if (root != null)
                Root = root;
            else
                Root = new Topic();
            Root.ResetPadding();
        }

        #region Properties
        Topic _Root;
        string _Filename;
        //MindMapStyle _Style;
        MindMapLayoutType _LayoutType = MindMapLayoutType.MindMap;
        //string _Description;
        string _Author;
        string _Company;
        string _Version = "1.0";
        Dictionary<string, Topic> NamedTopics;
        Dictionary<string, string> _ExtendAttributes;
        PictureLibrary _PictureLibrary;

        public event EventHandler FilenameChanged;
        public event EventHandler RootChanged;
        //public event EventHandler StyleChanged;
        public event EventHandler LayoutTypeChanged;
        public event EventHandler DescriptionChanged;

        [DefaultValue(null), Browsable(false)]
        public Topic Root
        {
            get { return _Root; }
            set 
            {
                if (_Root != value)
                {
                    _Root = value;
                    OnRootChanged();
                }
            }
        }

        [Browsable(false), DefaultValue(null)]
        public string Filename
        {
            get { return _Filename; }
            set
            {
                if (_Filename != value)
                {
                    _Filename = value;
                    OnFilenameChanged();
                }
            }
        }

        /*[DefaultValue(null), Browsable(false)]
        [LocalDisplayName("Style")]
        public MindMapStyle Style
        {
            get { return _Style; }
            private set 
            {
                if (_Style != value)
                {
                    MindMapStyle old = _Style;
                    _Style = value;
                    OnStyleChanged(old);
                }
            }
        }*/

        [Browsable(false)]
        public override ChartType Type
        {
            get { return ChartType.MindMap; }
        }

        [DefaultValue(MindMapLayoutType.MindMap), LocalDisplayName("Layout Type"), LocalCategory("Layout")]
        public MindMapLayoutType LayoutType
        {
            get { return _LayoutType; }
            set
            {
                if (_LayoutType != value)
                {
                    _LayoutType = value;
                    OnLayoutTypeChanged();
                }
            }
        }

        //[DefaultValue(null), LocalDisplayName("Notes"), LocalCategory("Description")]
        //[Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor))]
        //public string Description
        //{
        //    get { return _Description; }
        //    set
        //    {
        //        if (_Description != value)
        //        {
        //            _Description = value;
        //            OnDescriptionChanged();
        //        }
        //    }
        //}

        [Browsable(false)]//[DefaultValue(null), LocalDisplayName("Author"), LocalCategory("Description")]
        public string Author
        {
            get { return _Author; }
            set 
            {
                if (_Author != value)
                {
                    _Author = value;
                    OnAuthorChanged();
                }
            }
        }

        [Browsable(false)]//[DefaultValue(null), LocalDisplayName("Company"), LocalCategory("Description")]
        public string Company
        {
            get { return _Company; }
            set 
            {
                if (_Company != value)
                {
                    _Company = value;
                    OnCompanyChanged();
                }
            }
        }

        [Browsable(false)]//[DefaultValue("1.0"), LocalDisplayName("Version"), LocalCategory("Description")]
        public string Version
        {
            get { return _Version; }
            set
            {
                if (_Version != value)
                {
                    _Version = value;
                    OnVersionChanged();
                }
            }
        }         

        void OnAuthorChanged()
        {
            Modified = true;
        }

        void OnCompanyChanged()
        {
            Modified = true;
        }

        void OnVersionChanged()
        {
            Modified = true;
        }

        [Browsable(false)]
        public Dictionary<string, string> ExtendAttributes
        {
            get { return _ExtendAttributes; }
            private set { _ExtendAttributes = value; }
        }

        void OnFilenameChanged()
        {
            if (FilenameChanged != null)
            {
                FilenameChanged(this, EventArgs.Empty);
            }
        }

        void OnRootChanged()
        {
            Modified = true;

            if (Root != null)
            {
                Root.Chart = this;
                RefreshID(Root);
            }

            if (RootChanged != null)
            {
                RootChanged(this, EventArgs.Empty);
            }
        }

        void OnLayoutTypeChanged()
        {
            Modified = true;

            if (LayoutTypeChanged != null)
            {
                LayoutTypeChanged(this, EventArgs.Empty);
            }
        }

        /*void OnStyleChanged(MindMapStyle old)
        {
            if (old != null)
            {
                old.ValueChanged -= new EventHandler(Style_ValueChanged);
            }

            if (Style != null)
            {
                Style.ValueChanged += new EventHandler(Style_ValueChanged);
            }

            if (StyleChanged != null)
            {
                StyleChanged(this, EventArgs.Empty);
            }
        }*/

        void OnDescriptionChanged()
        {
            Modified = true;

            if (DescriptionChanged != null)
            {
                DescriptionChanged(this, EventArgs.Empty);
            }
        }

        //void Style_ValueChanged(object sender, EventArgs e)
        //{
        //    Modified = true;

        //    if (StyleChanged != null)
        //    {
        //        StyleChanged(this, EventArgs.Empty);
        //    } 
        //}

        #endregion

        #region Topic Events
        //public event TopicEventHandler TopicStyleChanged;
        //public event TopicEventHandler TopicTextChanged;
        public event Blumind.Core.PropertyChangedEventHandler TopicWidgetChanged;
        public event TopicEventHandler TopicDescriptionChanged;
        public event TopicEventHandler TopicIconChanged;
        public event TopicEventHandler TopicFoldedChanged;
        public event TopicEventHandler TopicWidthChanged;
        public event TopicEventHandler TopicHeightChanged;
        public event TopicEventHandler TopicAdded;
        public event TopicEventHandler TopicRemoved;
        public event EventHandler TopicAfterSort;
        public event TopicCancelEventHandler TopicBeforeExpand;
        public event TopicCancelEventHandler TopicBeforeCollapse;
        public event LinkEventHandler LinkAdded;
        public event LinkEventHandler LinkRemoved;
        public event WidgetEventHandler WidgetRemoved;

        //internal void OnTopicStyleChanged(Topic topic)
        //{
        //    Modified = true;

        //    if (TopicStyleChanged != null)
        //    {
        //        TopicStyleChanged(this, new TopicEventArgs(topic));
        //    }
        //}

        //internal void OnTopicTextChanged(Topic topic)
        //{
        //    Modified = true;

        //    if (TopicTextChanged != null)
        //    {
        //        TopicTextChanged(this, new TopicEventArgs(topic));
        //    }
        //}

        internal void OnTopicWidgetChanged(Topic topic, Widget widget, Blumind.Core.PropertyChangedEventArgs e)
        {
            if(e.HasChanges(ChangeTypes.Data))
                Modified = true;

            if (TopicWidgetChanged != null)
                TopicWidgetChanged(widget, e);
        }

        internal void OnTopicDescriptionChanged(Topic topic)
        {
            Modified = true;

            if (TopicDescriptionChanged != null)
            {
                TopicDescriptionChanged(this, new TopicEventArgs(topic));
            }
        }

        internal void OnTopicIconChanged(Topic topic)
        {
            Modified = true;

            if (TopicIconChanged != null)
            {
                TopicIconChanged(this, new TopicEventArgs(topic));
            }
        }

        internal void OnTopicFoldedChanged(Topic topic)
        {
            Modified = true;

            if (TopicFoldedChanged != null)
            {
                TopicFoldedChanged(this, new TopicEventArgs(topic));
            }
        }

        internal void OnTopicAdded(Topic item)
        {
            Modified = true;
            RefreshID(item);

            if (TopicAdded != null)
            {
                TopicAdded(this, new TopicEventArgs(item));
            }

            OnChartObjectAdded(item);
        }

        internal void OnTopicRemoved(Topic item)
        {
            if (!string.IsNullOrEmpty(item.ID) && NamedTopics.ContainsKey(item.ID))
            {
                Modified = true;
                NamedTopics.Remove(item.ID);
            }

            if (TopicRemoved != null)
            {
                TopicRemoved(this, new TopicEventArgs(item));
            }
        }

        internal void OnTopicBeforeExpand(TopicCancelEventArgs e)
        {
            if (TopicBeforeExpand != null)
            {
                TopicBeforeExpand(this, e);
            }
        }

        internal void OnTopicBeforeCollapse(TopicCancelEventArgs e)
        {
            if (TopicBeforeCollapse != null)
            {
                TopicBeforeCollapse(this, e);
            }
        }

        internal void OnTopicWidthChanged(Topic topic)
        {
            Modified = true;

            if (TopicWidthChanged != null)
            {
                TopicWidthChanged(this, new TopicEventArgs(topic));
            }
        }

        internal void OnTopicHeightChanged(Topic topic)
        {
            Modified = true;

            if (TopicHeightChanged != null)
            {
                TopicHeightChanged(this, new TopicEventArgs(topic));
            }
        }

        internal void OnTopicAfterSort()
        {
            Modified = true;

            if (TopicAfterSort != null)
            {
                TopicAfterSort(this, EventArgs.Empty);
            }
        }

        internal void OnLineAdded(Link link)
        {
            Modified = true;

            if (LinkAdded != null)
            {
                LinkAdded(this, new LinkEventArgs(link));
            }
        }

        internal void OnLineRemoved(Link link)
        {
            Modified = true;

            if (LinkRemoved != null)
            {
                LinkRemoved(this, new LinkEventArgs(link));
            }
        }

        internal void OnWidgetRemoved(Widget widget)
        {
            Modified = true;

            if (WidgetRemoved != null)
            {
                WidgetRemoved(this, new WidgetEventArgs(widget));
            }
        }

        #endregion

        #region Links Events
        [Category("Link Events")]
        public event Blumind.Core.PropertyChangedEventHandler LinkPropertyChanged;

        [Category("Link Events")]
        public event LinkEventHandler LinkVisibleChanged;

        public void OnLinkPropertyChanged(object sender, Blumind.Core.PropertyChangedEventArgs e)
        {
            Modified = true;

            if (LinkPropertyChanged != null)
            {
                LinkPropertyChanged(sender, e);
            }
        }

        public void OnLinkVisibleChanged(Link link)
        {
            Modified = true;

            if (LinkVisibleChanged != null)
            {
                LinkVisibleChanged(this, new LinkEventArgs(link));
            }
        }
        
        #endregion

        #region Style
        public const int DefaultLayerSpace = 80;
        public const int DefaultItemsSpace = 10;

        Color _NodeBackColor = Color.White;
        Color _NodeForeColor = Color.Black;
        Color _LinkColor = Color.Green;

        int _LayerSpace = DefaultLayerSpace;
        int _ItemsSpace = DefaultItemsSpace;

        [DefaultValue(typeof(Color), "White")]
        [LocalDisplayName("Node Back Color"), LocalCategory("Color")]
        public Color NodeBackColor
        {
            get { return _NodeBackColor; }
            set
            {
                if (_NodeBackColor != value)
                {
                    var old = _NodeBackColor;
                    _NodeBackColor = value;
                    OnPropertyChanged("NodeBackColor", old, NodeBackColor, ChangeTypes.Visual);
                }
            }
        }

        [DefaultValue(typeof(Color), "Black")]
        [LocalDisplayName("Node Fore Color"), LocalCategory("Color")]
        public Color NodeForeColor
        {
            get { return _NodeForeColor; }
            set
            {
                if (_NodeForeColor != value)
                {
                    var old = _NodeForeColor;
                    _NodeForeColor = value;
                    OnPropertyChanged("NodeForeColor", old, NodeForeColor, ChangeTypes.Visual);
                }
            }
        }

        [DefaultValue(typeof(Color), "Green")]
        [LocalDisplayName("Link Line Color"), LocalCategory("Color")]
        public Color LinkLineColor
        {
            get { return _LinkColor; }
            set
            {
                if (_LinkColor != value)
                {
                    var old = _LinkColor;
                    _LinkColor = value;
                    OnPropertyChanged("LinkLineColor", old, LinkLineColor, ChangeTypes.Visual);
                }
            }
        }

        [DefaultValue(DefaultLayerSpace)]
        [LocalDisplayName("Layer Space"), LocalCategory("Layout")]
        public virtual int LayerSpace
        {
            get { return _LayerSpace; }
            set
            {
                if (_LayerSpace != value)
                {
                    var old = _LayerSpace;
                    _LayerSpace = value;
                    OnPropertyChanged("LayerSpace", old, _LayerSpace, ChangeTypes.Visual | ChangeTypes.Layout);
                }
            }
        }

        [DefaultValue(DefaultItemsSpace)]
        [LocalDisplayName("Items Space"), LocalCategory("Layout")]
        public virtual int ItemsSpace
        {
            get { return _ItemsSpace; }
            set
            {
                if (_ItemsSpace != value)
                {
                    var old = _ItemsSpace;
                    _ItemsSpace = value;
                    OnPropertyChanged("ItemsSpace", old, _ItemsSpace, ChangeTypes.Visual | ChangeTypes.Layout);
                }
            }
        }

        public override string StyleToString()
        {
            var sb = new StringBuilder();
            sb.Append(base.ToString());

            BuildStyleString(sb, "Node Back Color", !NodeBackColor.IsEmpty, ST.ToString(NodeBackColor));
            BuildStyleString(sb, "Node Fore Color", !NodeForeColor.IsEmpty, ST.ToString(NodeForeColor));
            BuildStyleString(sb, "Link Line Color", !LinkLineColor.IsEmpty, ST.ToString(LinkLineColor));
            BuildStyleString(sb, "Layer Space", LayerSpace != DefaultLayerSpace, LayerSpace);
            BuildStyleString(sb, "Items Space", ItemsSpace != DefaultItemsSpace, ItemsSpace);

            return sb.ToString();
        }

        protected override void OnPictureThumbSizeChanged(Size old)
        {
            base.OnPictureThumbSizeChanged(old);

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                foreach (var topic in GetTopics(false))
                {

                    foreach (var widget in topic.Widgets)
                    {
                        if (widget is PictureWidget)
                        {
                            var pw = (PictureWidget)widget;
                            pw.CreateThumbImage();
                        }
                    }
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        #endregion

        public void CollapseAll()
        {
            if (Root != null)
            {
                Root.CollapseAll();
            }
        }

        public void ExpandAll()
        {
            if (Root != null)
            {
                Root.ExpandAll();
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public MindMap Clone()
        {
            var map = (MindMap)this.MemberwiseClone();
            //map.Style = (MindMapStyle)this.Style.Clone();
            var root = map.Root.Clone();
            root.CutFromMap();
            map.Root = root;

            return map;
        }

        void RefreshID(Topic topic)
        {
            var accessedObjects = new List<Topic>();
            RefreshID(topic, accessedObjects);
        }

        void RefreshID(Topic topic, List<Topic> accessedObjects)
        {
            accessedObjects.Add(topic);
            //if (string.IsNullOrEmpty(topic.ID) || (Topics.ContainsKey(topic.ID) && Topics[topic.ID] != topic))
            //{
            //    topic.ID = GenerateNextID();
            //}

            if (!string.IsNullOrEmpty(topic.ID) && !NamedTopics.ContainsKey(topic.ID))
            {
                NamedTopics.Add(topic.ID, topic);
            }

            foreach (var subtopic in topic.Children)
            {
                if (accessedObjects.Contains(subtopic))
                    continue;

                RefreshID(subtopic);
            }
        }

        public Topic GetTopicByID(string id)
        {
            if (string.IsNullOrEmpty(id) || !NamedTopics.ContainsKey(id))
                return null;
            else
                return NamedTopics[id];
        }
        
        public Link[] GetLinks(bool onlyVisible)
        {
            List<Link> lines = new List<Link>();
            GetLinks(Root, lines, onlyVisible);
            return lines.ToArray();
        }

        private void GetLinks(Topic topic, List<Link> lines, bool onlyVisible)
        {
            foreach (Link line in topic.Links)
            {
                lines.Add(line);
            }

            if (!topic.Folded || !onlyVisible)
            {
                foreach (Topic subTopic in topic.Children)
                {
                    GetLinks(subTopic, lines, onlyVisible);
                }
            }
        }

        public Topic[] GetTopics(bool onlyVisible)
        {
            var topics = new List<Topic>();
            if (Root != null)
            {
                GetTopics(Root, topics, onlyVisible);
            }

            return topics.ToArray();
        }

        void GetTopics(Topic topic, List<Topic> topics, bool onlyVisible)
        {
            topics.Add(topic);

            if (!topic.Folded || !onlyVisible)
            {
                foreach (Topic subTopic in topic.Children)
                {
                    GetTopics(subTopic, topics, onlyVisible);
                }
            }
        }

        public Topic[] GetLeafTopics()
        {
            List<Topic> topics = new List<Topic>();
            if (Root != null)
            {
                GetLeafTopics(Root, topics);
            }

            return topics.ToArray();
        }

        void GetLeafTopics(Topic topic, List<Topic> topics)
        {
            if (topic.Children.IsEmpty)
            {
                topics.Add(topic);
            }
            else
            {
                foreach (var subTopic in topic.Children)
                {
                    GetLeafTopics(subTopic, topics);
                }
            }
        }

        public override void ApplyTheme(ChartTheme theme)
        {
            //int itemsSpace = ItemsSpace;
            //int layerSpace = LayerSpace;
            //Font font = Font;

            //Style.Copy(theme);
            BackColor = theme.BackColor;
            ForeColor = theme.ForeColor;
            LineColor = theme.LineColor;
            BorderColor = theme.BorderColor;
            LineWidth = theme.LineWidth;
            BorderWidth = theme.BorderWidth;
            NodeBackColor = theme.NodeBackColor;
            NodeForeColor = theme.NodeForeColor;
            SelectColor = theme.SelectColor;
            HoverColor = theme.HoverColor;
            LinkLineColor = theme.LinkLineColor;
            ItemsSpace = theme.ItemsSpace;
            LayerSpace = theme.LayerSpace;

            if (Root != null)
            {
                Root.Style.BackColor = theme.RootBackColor;
                Root.Style.ForeColor = theme.RootForeColor;
                Root.Style.BorderColor = theme.RootBorderColor;
            }
        }

        public override ChartTheme GetChartTheme()
        {
            var theme = new ChartTheme();
            //theme.Copy(this.Style);

            theme.BackColor = this.BackColor;
            theme.ForeColor = this.ForeColor;
            theme.LineColor = this.LineColor;
            theme.BorderColor = this.BorderColor;
            theme.Font = this.Font;
            theme.LineWidth = this.LineWidth;
            theme.BorderWidth = this.BorderWidth;
            theme.NodeBackColor = this.NodeBackColor;
            theme.NodeForeColor = this.NodeForeColor;
            theme.SelectColor = this.SelectColor;
            theme.HoverColor = this.HoverColor;
            theme.LinkLineColor = this.LinkLineColor;
            theme.ItemsSpace = this.ItemsSpace;
            theme.LayerSpace = this.LayerSpace;

            if (Root != null)
            {
                theme.RootBackColor = Root.Style.BackColor;
                theme.RootForeColor = Root.Style.ForeColor;
                theme.RootBorderColor = Root.Style.BorderColor;
            }

            return theme;
        }

        public override Size GetContentSize()
        {
            var rect = new Rectangle();

            foreach (var topic in GetTopics(true))
            {
                rect = Rectangle.Union(rect, topic.Bounds);
            }

            foreach (var link in GetLinks(true))
            {
                rect = Rectangle.Union(rect, link.Bounds);
            }

            rect.Width += Margin.Right;
            rect.Height += Margin.Bottom;
            rect.Width = Math.Max(rect.Width, PageSize.Width);
            rect.Height = Math.Max(rect.Height, PageSize.Height);

            return rect.Size;
        }

        public override Image CreateThumbImage(Size thumbSize)
        {
            if (!EnsureChartLayouted())
                return null;

            Size contentSize = GetContentSize();
            if (contentSize.Width <= 0 || contentSize.Height <= 0)
                return null;

            Bitmap bmp = new Bitmap(thumbSize.Width, thumbSize.Height);
            using (Graphics grf = Graphics.FromImage(bmp))
            {
                PaintHelper.SetHighQualityRender(grf);

                var render = new GeneralRender();
                var args = new PaintEventArgs(grf, new Rectangle(0, 0, thumbSize.Width, thumbSize.Height));
                var zoom = PaintHelper.GetZoom(contentSize, thumbSize);

                //
                var cs = PaintHelper.Zoom(contentSize, zoom);
                if (cs.Width < thumbSize.Width || cs.Height < thumbSize.Height)
                    grf.TranslateTransform(Math.Max(0, (thumbSize.Width - cs.Width) / 2), Math.Max(0, (thumbSize.Height - cs.Height) / 2));

                render.PaintNavigationMap(this, zoom, args);
            }

            return bmp;
            //return base.CreateThumbImage();
        }

        public override bool EnsureChartLayouted()
        {
            if (LayoutInitialized)
                return true;

            var layouter = LayoutManage.GetLayouter(this.LayoutType);
            if (layouter != null)
            {
                var args = new MindMapLayoutArgs(this, ChartBox.DefaultChartFont);
                layouter.LayoutMap(this, args);
                return true;
            }

            return base.EnsureChartLayouted();
        }

        #region I/O
        public override void Serialize(XmlDocument dom, XmlElement node)
        {
            base.Serialize(dom, node);

            //
            if (Document != null)
            {
                foreach (var topic in GetTopics(false))
                {
                    if (string.IsNullOrEmpty(topic.ID))
                        topic.ID = Document.GetNextObjectID();
                }
            }

            //Link[] lines = GetLinks(false);
            //foreach (Link line in lines)
            //{
            //    if (line.Target != null && string.IsNullOrEmpty(line.Target.ID))
            //        line.Target.ID = Topic.NewID();
            //}

            // layout
            ST.WriteTextNode(node, "layout", ST.ToString(LayoutType));

            // desc
            //if (!string.IsNullOrEmpty(Description))
            //    ST.WriteCDataNode(node, "description", Description);

            // style
            var styleNode = SerializeMapStyle(dom);
            if (styleNode.Attributes.Count > 0 || styleNode.ChildNodes.Count > 0)
                node.AppendChild(styleNode);

            // topics
            XmlElement topics = dom.CreateElement("nodes");
            topics.AppendChild(Topic.SerializeTopic(dom, Root));
            node.AppendChild(topics);
        }

        XmlElement SerializeMapStyle(XmlDocument dom)
        {
            XmlElement node = dom.CreateElement("style");

            ST.SerializeColor(node, "back_color", BackColor);
            ST.SerializeColor(node, "fore_color", ForeColor);
            ST.SerializeColor(node, "line_color", LineColor);
            ST.SerializeColor(node, "border_color", BorderColor);
            ST.SerializeColor(node, "node_back_color", NodeBackColor);
            ST.SerializeColor(node, "node_fore_color", NodeForeColor);
            ST.SerializeColor(node, "select_color", SelectColor);
            ST.SerializeColor(node, "hover_color", HoverColor);
            ST.SerializeColor(node, "link_line_color", LinkLineColor);

            ST.WriteTextNode(node, "widget_margin", WidgetMargin.ToString());
            ST.WriteTextNode(node, "picture_thumb_size", ST.ToString(PictureThumbSize));

            if (LayerSpace != MindMapStyle.DefaultLayerSpace)
                ST.WriteTextNode(node, "layer_space", LayerSpace.ToString());

            if (ItemsSpace != MindMapStyle.DefaultItemsSpace)
                ST.WriteTextNode(node, "items_space", ItemsSpace.ToString());

            if (Font != null)
            {
                XmlElement font = node.OwnerDocument.CreateElement("font");
                ST.WriteFontNode(font, Font);
                node.AppendChild(font);
            }

            return node;
        }

        public static MindMap LoadAsXml(Version documentVersion, XmlElement node)
        {
            if (node == null)
                return null;

            MindMap map = new MindMap(node.GetAttribute("name"));
            map.Deserialize(documentVersion, node);
            return map;
        }

        public override void Deserialize(Version documentVersion, XmlElement node)
        {
            base.Deserialize(documentVersion, node);

            LayoutType = ST.GetLayoutType(ST.ReadTextNode(node, "layout"));

            // 向后兼容
            if (string.IsNullOrEmpty(Remark))
            {
                Remark = ST.ReadCDataNode(node, "description");
                if (Remark == string.Empty)
                    Remark = ST.ReadTextNode(node, "description");
            }

            // style
            XmlElement styleNode = node.SelectSingleNode("style") as XmlElement;
            if (styleNode != null)
            {
                DeserializeMapStyle(styleNode);
            }

            // extend attributes (向后兼容)
            XmlElement attributeNode = node.SelectSingleNode("attributes") as XmlElement;
            if (attributeNode != null)
            {
                LoadExtendAttributes(attributeNode);
            }

            // nodes
            XmlElement rootNode = node.SelectSingleNode("nodes/node") as XmlElement;
            if (rootNode != null)
            {
                var topic = Topic.DeserializeTopic(documentVersion, rootNode);
                Root = topic;
            }
        }

        void DeserializeMapStyle(XmlElement node)
        {
            BackColor = ST.DeserializeColor(node, "back_color", BackColor);
            ForeColor = ST.DeserializeColor(node, "fore_color", ForeColor);
            LineColor = ST.DeserializeColor(node, "line_color", LineColor);
            BorderColor = ST.DeserializeColor(node, "border_color", BorderColor);
            NodeBackColor = ST.DeserializeColor(node, "node_back_color", NodeBackColor);
            NodeForeColor = ST.DeserializeColor(node, "node_fore_color", NodeForeColor);
            SelectColor = ST.DeserializeColor(node, "select_color", SelectColor);
            HoverColor = ST.DeserializeColor(node, "hover_color", HoverColor);
            LinkLineColor = ST.DeserializeColor(node, "link_line_color", LinkLineColor);
            LayerSpace = ST.GetInt(ST.ReadTextNode(node, "layer_space"), MindMapStyle.DefaultLayerSpace);
            ItemsSpace = ST.GetInt(ST.ReadTextNode(node, "items_space"), MindMapStyle.DefaultItemsSpace);
            WidgetMargin = ST.GetInt(ST.ReadTextNode(node, "widget_margin"), WidgetMargin);
            PictureThumbSize = ST.GetSize(ST.ReadTextNode(node, "picture_thumb_size"), PictureThumbSize);

            var fontNode = node.SelectSingleNode("font") as XmlElement;
            if (fontNode != null)
            {
                Font = ST.ReadFontNode(fontNode);
            }
        }

        void LoadExtendAttributes(XmlElement node)
        {
            if (node == null)
                return;

            XmlNodeList list = node.SelectNodes("item");
            foreach (XmlElement attNode in list)
            {
                ExtendAttributes[attNode.GetAttribute("name")] = attNode.InnerText;
            }
        }
        #endregion

        public ChartObject[] GetAllObjects(bool onlyVisible)
        {
            if (Root == null)
                return new ChartObject[0];

            var topics = GetAllTopics(Root, onlyVisible);
            var list = new List<ChartObject>();
            foreach (var t in topics)
            {
                list.Add(t);

                foreach (var link in t.Links)
                {
                    if (!onlyVisible || topics.Contains(link.Target))
                        list.Add(link);
                }
            }

            return list.ToArray();
        }

        IEnumerable<Topic> GetAllTopics(Topic parent, bool onlyVisible)
        {
            if (parent == null)
                yield break;
            yield return parent;

            if (!onlyVisible || !parent.Folded)
            {
                foreach (var n in parent.Children)
                {
                    yield return n;
                    foreach (var sn in GetAllTopics(n, onlyVisible))
                        yield return sn;
                }
            }
        }
    }
}
