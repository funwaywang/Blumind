using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Blumind.Controls.MapViews;
using Blumind.Core;
using Blumind.Globalization;
using Blumind.Model.Documents;
using Blumind.Model.MindMaps;

namespace Blumind.Model.Widgets
{
    abstract class Widget : ChartObject
        , IHyperlink
        , IColorToolTip
    {
        IWidgetContainer _WidgetContainer;
        bool _Selectable = true;
        bool _Hover;
        //bool _ResponseMouse;
        int? _CustomWidth;
        int? _CustomHeight;
        WidgetAlignment _Alignment = WidgetAlignment.Bottom;
        int _DisplayIndex;
        int _Padding;

        public Widget()
        {
        }

        [Browsable(false)]
        public IWidgetContainer WidgetContainer
        {
            get { return _WidgetContainer; }
            set 
            {
                if (_WidgetContainer != value)
                {
                    var old = _WidgetContainer;
                    _WidgetContainer = value;
                    OnWidgetContainerChanged(old);
                }
            }
        }

        public override ChartObject Container
        {
            get
            {
                return _WidgetContainer as ChartObject;
            }
            set
            {
                if (value is IWidgetContainer)
                    WidgetContainer = (IWidgetContainer)value;
                base.Container = value;
            }
        }

        [Browsable(false)]
        public bool Hover
        {
            get { return _Hover; }
            set 
            {
                if (_Hover != value)
                {
                    bool old = _Hover;
                    _Hover = value;
                    OnPropertyChanged("Hover", old, _Hover, ChangeTypes.Visual, false);
                }
            }
        }

        [Browsable(false)]
        public virtual bool ResponseMouse
        {
            get { return false; }
        }

        [DefaultValue(0), LocalDisplayName("Display Index"), LocalCategory("Layout")]
        public int DisplayIndex
        {
            get { return _DisplayIndex; }
            set
            {
                if (_DisplayIndex != value)
                {
                    var old = _DisplayIndex;
                    _DisplayIndex = value;
                    OnPropertyChanged("DisplayIndex", old, _DisplayIndex, ChangeTypes.Layout | ChangeTypes.Visual);
                }
            }
        }

        [DefaultValue(0), LocalDisplayName("Padding"), LocalCategory("Layout")]
        public int Padding
        {
            get { return _Padding; }
            set 
            {
                if (_Padding != value)
                {
                    var old = _Padding;
                    _Padding = value;
                    OnPropertyChanged("Padding", old, _Padding, ChangeTypes.Layout | ChangeTypes.Visual);
                }
            }
        }

        [Browsable(false)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
            //get
            //{
            //    return ST.HtmlToText(Remark);
            //    //return base.Text;
            //}
            //set
            //{
            //    if (Text != value)
            //    {
            //        Remark = value;
            //    }
            //}
        }

        [DefaultValue(WidgetAlignment.Bottom), LocalDisplayName("Alignment"), LocalCategory("Layout")]
        public virtual WidgetAlignment Alignment
        {
            get { return _Alignment; }
            set 
            {
                if (_Alignment != value)
                {
                    WidgetAlignment old = _Alignment;
                    _Alignment = value;
                    OnPropertyChanged("Alignment", old, _Alignment, ChangeTypes.All);
                }
            }
        }

        [DefaultValue(null), LocalDisplayName("Custom Width"), LocalCategory("Layout")]
        public int? CustomWidth
        {
            get { return _CustomWidth; }
            set
            {
                if (_CustomWidth != value)
                {
                    int? old = _CustomWidth;
                    _CustomWidth = value;
                    OnPropertyChanged("CustomWidth", old, _CustomWidth, ChangeTypes.All);
                }
            }
        }

        [DefaultValue(null), LocalDisplayName("Custom Height"), LocalCategory("Layout")]
        public int? CustomHeight
        {
            get { return _CustomHeight; }
            set
            {
                if (_CustomHeight != value)
                {
                    int? old = _CustomHeight;
                    _CustomHeight = value;
                    OnPropertyChanged("CustomHeight", old, _CustomHeight, ChangeTypes.All);
                }
            }
        }

        [Browsable(false)]
        public virtual bool FitContainer
        {
            get { return false; }
        }

        [Browsable(false)]
        protected Rectangle DisplayRectangle
        {
            get
            {
                var rect = Bounds;
                rect.Inflate(-Padding, -Padding);
                return rect;
            }
        }

        //[Browsable(false)]
        //public virtual bool VerticalFit
        //{
        //    get { return false; }
        //}

        //[DefaultValue(true), LocalDisplayName("Visible")]
        //[TypeConverter(typeof(Blumind.Design.BoolConverter))]
        //public bool Visible
        //{
        //    get { return _Visible; }
        //    set 
        //    {
        //        if (_Visible != value)
        //        {
        //            _Visible = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        [DefaultValue(true), Browsable(false)]
        public bool Selectable
        {
            get { return _Selectable; }
            set { _Selectable = value; }
        }

        [Browsable(false)]
        public virtual Cursor Cursor
        {
            get { return null; }
        }

        [Browsable(false)]
        public virtual bool CanCopy
        {
            get { return false; }
        }

        #region IHyperlink
        private string _Url = null;

        public event EventHandler HyperlinkChanged;

        [DefaultValue(null), LocalDisplayName("Hyperlink"), LocalCategory("Data")]
        [Editor(typeof(Blumind.Design.HyperlinkEditor), typeof(System.Drawing.Design.UITypeEditor))]
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

            OnPropertyChanged("Hyperlink", old, Hyperlink, ChangeTypes.Data | ChangeTypes.Visual);
        }

        #endregion

        public virtual void Copy()
        {
        }

        public abstract Size CalculateSize(MindMapLayoutArgs e);

        public abstract string GetTypeID();

        protected virtual string GetTooltip()
        {
            return null;
        }

        #region I/O
        public const string _XmlElementName = "widget";

        public override string XmlElementName
        {
            get { return _XmlElementName; }
        }

        public override void Serialize(XmlDocument dom, XmlElement node)
        {
            //
            node.SetAttribute("type", GetTypeID());
            node.SetAttribute("x", this.Bounds.X.ToString());
            node.SetAttribute("y", this.Bounds.Y.ToString());
            node.SetAttribute("width", this.Bounds.Width.ToString());
            node.SetAttribute("height", this.Bounds.Height.ToString());

            if (CustomWidth.HasValue)
                node.SetAttribute("custom_width", CustomWidth.Value.ToString());
            if (CustomHeight.HasValue)
                node.SetAttribute("custom_height", CustomHeight.Value.ToString());

            node.SetAttribute("align", Alignment.ToString());
            node.SetAttribute("hyperlink", Hyperlink);
            node.SetAttribute("display_index", DisplayIndex.ToString());
            node.SetAttribute("padding", Padding.ToString());
            if (!string.IsNullOrEmpty(Text))
                ST.WriteTextNode(node, "text", Text);
        }

        public override void Deserialize(Version documentVersion, XmlElement node)
        {
            //
            if (documentVersion >= Document.DV_3) // 新
            {
                this.Bounds = new Rectangle(
                    ST.GetIntDefault(node.GetAttribute("x")),
                    ST.GetIntDefault(node.GetAttribute("y")),
                    ST.GetIntDefault(node.GetAttribute("width")),
                    ST.GetIntDefault(node.GetAttribute("height")));

                CustomWidth = ST.GetInt(node.GetAttribute("custom_width"));
                CustomHeight = ST.GetInt(node.GetAttribute("custom_height"));
            }
            else
            {
                CustomWidth = ST.GetInt(node.GetAttribute("width"));
                CustomHeight = ST.GetInt(node.GetAttribute("height"));
            }

            if (node.HasAttribute("align"))
                Alignment = ST.GetEnumValue<WidgetAlignment>(node.GetAttribute("align"), Alignment);
            Hyperlink = node.GetAttribute("hyperlink");
            DisplayIndex = ST.GetIntDefault(node.GetAttribute("display_index"));
            Padding = ST.GetIntDefault(node.GetAttribute("padding"));
            Text = ST.ReadTextNode(node, "text");
        }

        public static Widget DeserializeWidget(Version documentVersion, XmlElement node)
        {
            if (node == null)
                throw new ArgumentNullException();

            string typeId = node.GetAttribute("type");
            Widget widget = Widget.Create(typeId);
            if (widget != null)
            {
                widget.Deserialize(documentVersion, node);
                return widget;
            }
            else
            {
                return null;
            }
        }
        #endregion

        public virtual void Paint(RenderArgs e)
        {
        }

        public virtual void CopyTo(Widget widget)
        {
            widget.Alignment = Alignment;
            widget.Text = Text;
        }

        public virtual void OnMouseClick(MouseEventArgs e)
        {
        }

        public virtual  void OnDoubleClick(HandledEventArgs e)
        {
        }

        protected virtual void OnWidgetContainerChanged(IWidgetContainer old)
        {
        }

        public static Widget Create(string typeId)
        {
            switch (typeId)
            {
                case ProgressBarWidget.TypeID:
                    return new ProgressBarWidget();
                case PictureWidget.TypeID:
                    return new PictureWidget();
                case NoteWidget.TypeID:
                    return new NoteWidget();
                //case HyperlinkWidget.TypeID:
                //    return new HyperlinkWidget();
                default:
                    return null;
            }
        }

        public virtual IWidgetEditDialog CreateEditDialog()
        {
            return null;
        }

        public void Remove()
        {
            if (WidgetContainer != null)
                WidgetContainer.Remove(this);
        }

        public virtual void OnAddByCommand(Topic parent)
        {
        }

        public virtual Widget Clone()
        {
            Widget widget = Create(GetTypeID());
            if (widget != null)
                CopyTo(widget);

            return widget;
        }

        #region IColorToolTip 成员

        [Browsable(false)]
        public string ToolTip
        {
            get
            {
                return GetTooltip();
            }
        }

        [Browsable(false)]
        public virtual bool ToolTipShowAlway
        {
            get { return true; }
        }

        [Browsable(false)]
        public virtual string ToolTipHyperlinks
        {
            get { return Hyperlink; }
        }

        #endregion
    }
}
