using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Xml;
using Blumind.Canvas;
using Blumind.ChartControls.Shapes;
using Blumind.Controls;
using Blumind.Controls.MapViews;
using Blumind.Controls.Paint;
using Blumind.Core;
using Blumind.Design;
using Blumind.Globalization;
using Blumind.Model;
using Blumind.Model.Documents;

namespace Blumind.Model.MindMaps
{
    class Link : ChartObject
        , ITextObject
        , IHyperlink
        , Blumind.Core.INotifyPropertyChanged
        //, IColorToolTip
    {
        //string _FromID;
        string _TargetID;
        Topic _From;
        Topic _Target;
        //string _Notes;
        int _LineWidth = 2;
        Color _Color = Color.Empty;
        BezierLayoutInfo _Layout;
        DashStyle _LineStyle = DashStyle.Dot;
        LineAnchor _StartCap = LineAnchor.None;
        LineAnchor _EndCap = LineAnchor.Arrow;

        //public event EventHandler VisibleChanged;

        public Link()
        {
            LayoutData = new BezierLayoutInfo();
        }

        public Link(Topic from, Topic to)
            : this()
        {
            From = from;
            Target = to;
        }

        [Browsable(false)]
        public MindMap Map
        {
            get
            {
                if (_From != null)
                    return _From.MindMap;
                else
                    return null;
            }

            //get { return _Owner; }
            //set 
            //{
            //    if (_Owner != value)
            //    {
            //        _Owner = value;
            //        OnOwnerChanged();
            //    }
            //}
        }

        //[Browsable(false)]
        //public string FromID
        //{
        //    get { return _FromID; }
        //    set { _FromID = value; }
        //}

        [Browsable(false)]
        public string TargetID
        {
            get { return _TargetID; }
            set { _TargetID = value; }
        }

        [Browsable(false)]
        public Topic From
        {
            get 
            {
                //if (_From == null && !string.IsNullOrEmpty(FromID) && Owner != null)
                //    _From = Owner.GetTopicByID(FromID);

                return _From; 
            }

            set 
            {
                if (_From != value)
                {
                    Topic old = _From;
                    _From = value;
                    OnFromChanged(old);
                }
            }
        }

        [Browsable(false)]
        public Topic Target
        {
            get 
            {
                if (_Target == null && !string.IsNullOrEmpty(TargetID) && Map != null)
                    _Target = Map.GetTopicByID(TargetID);

                return _Target; 
            }

            set
            {
                if (_Target != value)
                {
                    _Target = value;
                    OnTargetChanged();
                }
            }
        }

        [DefaultValue(2), LocalDisplayName("Width"), LocalCategory("Appearance")]
        [Editor(typeof(LineWidthEditor), typeof(UITypeEditor))]
        public int LineWidth
        {
            get { return _LineWidth; }
            set 
            {
                if (_LineWidth != value)
                {
                    int old = _LineWidth;
                    _LineWidth = value;
                    OnPropertyChanged("Width", old, _LineWidth, ChangeTypes.All);
                }
            }
        }

        [DefaultValue(typeof(Color), "Empty"), LocalDisplayName("Color"), LocalCategory("Appearance")]
        public Color Color
        {
            get { return _Color; }
            set 
            {
                if (_Color != value)
                {
                    Color old = _Color;
                    _Color = value;
                    OnPropertyChanged("Color", old, _Color, ChangeTypes.Data | ChangeTypes.Visual);
                }
            }
        }

        /*[DefaultValue(null), LocalDisplayName("Notes"), LocalCategory("Data")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor))]
        public string Notes
        {
            get { return _Notes; }
            set 
            {
                if (_Notes != value)
                {
                    string old = _Notes;
                    _Notes = value;
                    OnPropertyChanged("Notes", old, _Notes, ChangeTypes.Data);
                }
            }
        }*/

        [Browsable(false), Obsolete]
        public string Notes
        {
            get { return Remark; }
            set { Remark = value; }
        }

        [DefaultValue(DashStyle.Dot), LocalDisplayName("Line Style"), LocalCategory("Appearance")]
        [Editor(typeof(LineStyleEditor), typeof(UITypeEditor)), TypeConverter(typeof(LineStyleConverter))]
        public DashStyle LineStyle
        {
            get { return _LineStyle; }
            set
            {
                if (_LineStyle != value)
                {
                    DashStyle old = _LineStyle;
                    _LineStyle = value;
                    OnPropertyChanged("LineStyle", old, _LineStyle, ChangeTypes.Data | ChangeTypes.Visual);
                }
            }
        }

        [DefaultValue(LineAnchor.None), LocalDisplayName("Start Cap"), LocalCategory("Appearance")]
        [Editor(typeof(LineAnchorEditor), typeof(UITypeEditor)), TypeConverter(typeof(LineCapConverter))]
        public LineAnchor StartCap
        {
            get { return _StartCap; }
            set 
            {
                if (_StartCap != value)
                {
                    var old = _StartCap;
                    _StartCap = value;
                    OnPropertyChanged("StartCap", old, _StartCap, ChangeTypes.Data | ChangeTypes.Visual);
                }
            }
        }

        [DefaultValue(LineAnchor.Arrow), LocalDisplayName("End Cap"), LocalCategory("Appearance")]
        [Editor(typeof(LineAnchorEditor), typeof(UITypeEditor)), TypeConverter(typeof(LineCapConverter))]
        public LineAnchor EndCap
        {
            get { return _EndCap; }
            set
            {
                if (_EndCap != value)
                {
                    var old = _EndCap;
                    _EndCap = value;
                    OnPropertyChanged("EndCap", old, _EndCap, ChangeTypes.Data | ChangeTypes.Visual);
                }
            }
        }

        [Browsable(false)]
        public BezierLayoutInfo LayoutData
        {
            get { return _Layout; }
            private set { _Layout = value; }
        }

        [Browsable(false)]
        public bool Visible
        {
            get { return From != null && Target != null && From != Target && From.Visible && Target.Visible; }
        }

        public Point[] GetBezierPoints()
        {
            return LayoutData.GetPoints();
        }

        #region IHyperlink
        private string _Hyperlink = null;

        [DefaultValue(null), LocalDisplayName("Hyperlink"), LocalCategory("Data")]
        [Editor(typeof(Blumind.Design.HyperlinkEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Hyperlink
        {
            get
            {
                return _Hyperlink;
            }
            set
            {
                if (_Hyperlink != value)
                {
                    string old = _Hyperlink;
                    _Hyperlink = value;
                    OnHyperlinkChanged(old);
                }
            }
        }

        [DefaultValue(null)]
        protected virtual void OnHyperlinkChanged(string old)
        {
            OnPropertyChanged("Hyperlink", old, Hyperlink, ChangeTypes.Data | ChangeTypes.Visual, true);
        }

        #endregion
        
        private void OnFromChanged(Topic old)
        {
            if (old != null && old.Links.Contains(this))
            {
                old.Links.Remove(this);
            }

            if (From != null && !From.Links.Contains(this))
            {
                From.Links.Add(this);
            }

            if (old != null && old.MindMap != null)
            {
                old.MindMap.OnLinkVisibleChanged(this);
            }

            OnVisibleChanged();
        }

        private void OnTargetChanged()
        {
            //RefreshLayout();

            OnVisibleChanged();
        }

        //private void OnChanged()
        //{
        //    if (Map != null)
        //    {
        //        Map.OnLinkChanged(this);
        //    }

        //    if (Changed != null)
        //    {
        //        Changed(this, EventArgs.Empty);
        //    }
        //}

        private void OnVisibleChanged()
        {
            if (Map != null)
            {
                Map.OnLinkVisibleChanged(this);
            }

            //if (VisibleChanged != null)
            //{
            //    VisibleChanged(this, EventArgs.Empty);
            //}
        }

        private Point AdjustPoint(int quadrant, Point pts)
        {
            Point pt = pts;

            switch (quadrant % 4)
            {
                case 1:
                    pt.X = -pts.Y;
                    pt.Y = pts.X;
                    break;
                case 2:
                    pt.X = -pts.X;
                    pt.Y = -pts.Y;
                    break;
                case 3:
                    pt.X = pts.Y;
                    pt.Y = -pts.X;
                    break;
            }

            return pt;
        }

        public void RefreshLayout()
        {
            if (From == null || Target == null)
                return;

            LayoutData = GetLayoutInfo(LayoutData.CP1, LayoutData.CP2);
            Bounds = LayoutData.GetFullBounds();
        }

        BezierLayoutInfo GetLayoutInfo(BezierControlPoint controlPoint1, BezierControlPoint controlPoint2)
        {
            var layout = new BezierLayoutInfo();
            layout.CP1 = controlPoint1;
            layout.CP2 = controlPoint2;

            Point cp1, cp2;
            Rectangle rect1 = From.Bounds;
            Rectangle rect2 = Target.Bounds;
            Point pts = PaintHelper.CenterPoint(rect1);
            Point ptd = PaintHelper.CenterPoint(rect2);

            rect1.Inflate(2, 2);
            rect2.Inflate(2, 2);
            if(StartCap != LineAnchor.None)
                rect1.Inflate(Layouter.LineAnchorSize, Layouter.LineAnchorSize);
            if (EndCap != LineAnchor.None)
                rect2.Inflate(Layouter.LineAnchorSize, Layouter.LineAnchorSize);

            if (pts == ptd)
            {
                cp1 = pts;
                cp2 = ptd;

                layout.Bounds = Rectangle.Empty;
                layout.Region = null;
            }
            else
            {
                cp1 = BezierHelper.GetControlPoint(pts, controlPoint1);
                cp2 = BezierHelper.GetControlPoint(ptd, controlPoint2);

                //Point[] controlPoints = GetControlPoints(pts, ptd);

                Shape sShape = Shape.GetShaper(From);
                pts = sShape.GetBorderPoint(rect1, cp1);

                Shape dShape = Shape.GetShaper(Target);
                ptd = sShape.GetBorderPoint(rect2, cp2);

                GraphicsPath gp = new GraphicsPath();
                gp.AddBezier(pts, cp1, cp2, ptd);
                Pen penWiden = new Pen(Color.Black, LineWidth + 5);
                gp.Widen(penWiden);

                var rect = gp.GetBounds();
                rect.Inflate(LineWidth, LineWidth);
                layout.Bounds = rect;
                layout.Region = new Region(gp);
            }

            Point ptCenter = BezierHelper.GetPoint(pts, cp1, cp2, ptd, 0.5f);
            Rectangle textBounds = layout.TextBounds;
            textBounds.X = ptCenter.X - layout.TextBounds.Width / 2;
            textBounds.Y = ptCenter.Y - layout.TextBounds.Height / 2;

            // cache layout info
            layout.StartBounds = rect1;
            layout.EndBounds = rect2;
            layout.StartPoint = pts;
            layout.EndPoint = ptd;
            layout.ControlPoint1 = cp1;
            layout.ControlPoint2 = cp2;
            layout.TextBounds = textBounds;

            return layout;
        }

        public bool CanStartFrom(Topic topic)
        {
            if (topic == null || topic == Target)
                return false;
            else
                return true;
        }

        public bool CanEndTo(Topic topic)
        {
            if (topic == null || topic == From)
                return false;
            else
                return true;
        }

        public void SetChanged()
        {
            Blumind.Core.PropertyChangedEventArgs e = new Blumind.Core.PropertyChangedEventArgs(null, null, null, ChangeTypes.All, false);
            OnPropertyChanged(e);
        }

        public static Link CreateNew(Topic from, Topic to)
        {
            var link = new Link(from, to);
            link.Reset();

            return link;
        }

        public void Reset()
        {
            Point ptf = PaintHelper.CenterPoint(From.Bounds);
            Point ptt = PaintHelper.CenterPoint(Target.Bounds);
            int distance = PaintHelper.GetDistance(ptf, ptt);
            var len = distance * 20 / 100;
            var angle = PaintHelper.GetAngle(ptf, ptt);

            LayoutData.CP1 = new BezierControlPoint(angle, len);
            LayoutData.CP2 = new BezierControlPoint((180 + angle) % 360, len);

            RefreshLayout();
            SetChanged();
        }

        public Link Clone()
        {
            return (Link)this.MemberwiseClone();
        }

        #region IColorToolTip 成员
/*
        [Browsable(false)]
        public string ToolTip
        {
            get
            {
                return Remark;
            }
        }

        [Browsable(false)]
        public Color ToolTipBackColor
        {
            get { return Color.Empty; }
        }

        [Browsable(false)]
        public Color ToolTipForeColor
        {
            get { return Color.Empty; }
        }

        [Browsable(false)]
        public Font ToolTipFont
        {
            get { return null; }
        }

        [Browsable(false)]
        public bool ToolTipShowAlway
        {
            get { return false; }
        }

        [Browsable(false)]
        public string ToolTipHyperlinks
        {
            get { return Hyperlink; }
        }
        */
        #endregion

        #region ITextObject 成员

        Font ITextObject.Font
        {
            get
            {
                if (Map == null)
                    return null;
                else
                    return Map.Font; 
            }
        }

        Rectangle ITextObject.Bounds
        {
            get 
            {
                if (!string.IsNullOrEmpty(Text) && !LayoutData.TextBounds.IsEmpty)
                    return LayoutData.TextBounds;
                else
                {
                    Point pt = BezierHelper.GetPoint(LayoutData.StartPoint, LayoutData.ControlPoint1, LayoutData.ControlPoint2, LayoutData.EndPoint, 0.5f);
                    return new Rectangle(pt.X - 50, pt.Y - 5, 100, 10);
                }
            }
        }

        #endregion

        #region I/O
        public const string _XmlElementName = "link";

        public override string XmlElementName
        {
            get { return _XmlElementName; }
        }

        public override void Serialize(XmlDocument dom, XmlElement node)
        {
            base.Serialize(dom, node);

            node.SetAttribute("target", Target.ID);

            if (LayoutData != null)
            {
                node.SetAttribute("cp1_length", LayoutData.CP1.Length.ToString());
                node.SetAttribute("cp1_angle", LayoutData.CP1.Angle.ToString());
                node.SetAttribute("cp2_length", LayoutData.CP2.Length.ToString());
                node.SetAttribute("cp2_angle", LayoutData.CP2.Angle.ToString());
            }

            node.SetAttribute("width", LineWidth.ToString());
            if (!Color.IsEmpty)
                node.SetAttribute("color", ST.ToString(Color));
            node.SetAttribute("line_style", LineStyle.ToString());
            node.SetAttribute("start_cap", StartCap.ToString());
            node.SetAttribute("end_cap", EndCap.ToString());
            node.SetAttribute("text", Text);
            node.SetAttribute("hyperlink", Hyperlink);

            if (!string.IsNullOrEmpty(Remark))
            {
                ST.WriteTextNode(node, "remark", Remark);
            }
        }

        public override void Deserialize(Version documentVersion, XmlElement node)
        {
            base.Deserialize(documentVersion, node);

            //FromID = node.GetAttribute("from");
            TargetID = node.GetAttribute("target");
            //LayoutData.CPLength1 = ST.GetInt(node.GetAttribute("cp1_length"), 0);
            //LayoutData.CPAngle1 = ST.GetInt(node.GetAttribute("cp1_angle"), 0);
            LayoutData.CP1 = new BezierControlPoint(
                ST.GetInt(node.GetAttribute("cp1_angle"), 0),
                ST.GetInt(node.GetAttribute("cp1_length"), 0));
            //LayoutData.CPLength2 = ST.GetInt(node.GetAttribute("cp2_length"), 0);
            //LayoutData.CPAngle2 = ST.GetInt(node.GetAttribute("cp2_angle"), 0);
            LayoutData.CP2 = new BezierControlPoint(
                ST.GetInt(node.GetAttribute("cp2_angle"), 0),
                ST.GetInt(node.GetAttribute("cp2_length"), 0));
            Color = ST.GetColor(node.GetAttribute("color"), Color);
            LineWidth = ST.GetInt(node.GetAttribute("width"), LineWidth);
            LineStyle = ST.GetEnumValue(node.GetAttribute("line_style"), LineStyle);
            Text = node.GetAttribute("text");
            Hyperlink = node.GetAttribute("hyperlink");

            if (node.HasAttribute("start_cap"))
            {
                int c ;
                if (int.TryParse(node.GetAttribute("start_cap"), out c))
                    StartCap = ((LineCap)c).ToLineAnchor();
                else if (documentVersion >= Document.DV_3)
                    StartCap = LineAnchorExtensions.Parse(node.GetAttribute("start_cap"));
            }

            if (node.HasAttribute("end_cap"))
            {
                int c;
                if (int.TryParse(node.GetAttribute("end_cap"), out c))
                    EndCap = ((LineCap)c).ToLineAnchor();
                else if (documentVersion >= Document.DV_3)
                    EndCap = LineAnchorExtensions.Parse(node.GetAttribute("end_cap"));
            }

            if (documentVersion >= Document.DV_3)
            {
                Remark = ST.ReadTextNode(node, "remark");
            }
            else
            {
                Remark = ST.ReadTextNode(node, "description");
            }
        }
        #endregion
    }
}
