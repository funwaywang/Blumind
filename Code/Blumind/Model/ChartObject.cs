using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Blumind.Core;
using Blumind.Globalization;
using Blumind.Model.Documents;

namespace Blumind.Model
{
    abstract class ChartObject : Blumind.Core.INotifyPropertyChanged, IRemark, Blumind.Model.ISerializable//, IComponent
    {
        ChartPage _Chart;
        string _Text;
        string _Remark;

        public event EventHandler TextChanged;
        public event EventHandler RemarkChanged;

        public ChartObject()
        {
        }

        [Browsable(false)]
        public string ID { get; set; }

        [Browsable(false)]
        public ChartPage Chart
        {
            get { return _Chart; }
            set
            {
                if (_Chart != value)
                {
                    _Chart = value;
                    OnChartChanged();
                }
            }
        }

        [Browsable(false)]
        public virtual ChartObject Parent { get; set; }

        [Browsable(false)]
        public virtual ChartObject Container { get; set; }

        [DefaultValue(null), LocalDisplayName("Text"), LocalCategory("Data")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor))]
        public virtual string Text
        {
            get { return _Text; }
            set
            {
                if (_Text != value)
                {
                    var old = _Text;
                    _Text = value;
                    OnTextChanged(old);
                }
            }
        }

        [DefaultValue(null), LocalDisplayName("Remark"), LocalCategory("Data")]
        [Editor(typeof(Blumind.Design.RemarkDesignEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(Blumind.Design.RemarkTypeConverter))]
        public string Remark
        {
            get { return _Remark; }
            set 
            {
                if (_Remark != value)
                {
                    var old = _Remark;
                    _Remark = value;
                    OnRemarkChanged(old);
                }
            }
        }

        [Browsable(false)]
        public bool HaveRemark
        {
            get { return !string.IsNullOrEmpty(Remark); }
        }

        [Browsable(false)]
        public Point PointToChart
        {
            get
            {
                if (Container != null)
                {
                    var pt = Container.PointToChart;
                    pt.Offset(this.Left, this.Top);
                    return pt;
                }

                return this.Location;
            }
        }

        protected virtual void OnRemarkChanged(string oldValue)
        {
            if (RemarkChanged != null)
            {
                RemarkChanged(this, EventArgs.Empty);
            }

            var ct = ChangeTypes.Data;
            if (string.IsNullOrEmpty(oldValue) != string.IsNullOrEmpty(Remark))
                ct |= ChangeTypes.Layout | ChangeTypes.Visual;

            OnPropertyChanged(new Blumind.Core.PropertyChangedEventArgs("Remark", oldValue, Remark, ct, true));
        }

        protected virtual void OnTextChanged(string oldValue)
        {
            if (TextChanged != null)
            {
                TextChanged(this, EventArgs.Empty);
            }

            OnPropertyChanged(new Blumind.Core.PropertyChangedEventArgs("Text", oldValue, Text, ChangeTypes.All, true));
        }

        protected virtual void OnChartChanged()
        {
        }

        /// <summary>
        /// 提供一个机会, 让对象可以将自己独特的内容复制到剪切板, 比如图片
        /// </summary>
        /// <param name="dataObject"></param>
        public virtual void CopyExtendContent(DataObject dataObject)
        {
        }

        #region Property Change
        public event Blumind.Core.PropertyChangedEventHandler PropertyChanged;

        [Browsable(false), DefaultValue(false)]
        public bool PropertyChangeSuspending { get; set; }

        public void SuspendPropertyChange()
        {
            PropertyChangeSuspending = true;
        }

        public void ResumePropertyChange()
        {
            PropertyChangeSuspending = false;
        }

        protected virtual void OnPropertyChanged(Blumind.Core.PropertyChangedEventArgs e)
        {
            if (PropertyChangeSuspending)
                return;

            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }

            if (Chart != null)
            {
                Chart.OnChartObjectPropertyChanged(this, e);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue, ChangeTypes changes)
        {
            OnPropertyChanged(new Blumind.Core.PropertyChangedEventArgs(propertyName, oldValue, newValue, changes, true));
        }

        protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue, ChangeTypes changes, bool rollbackable)
        {
            OnPropertyChanged(new Blumind.Core.PropertyChangedEventArgs(propertyName, oldValue, newValue, changes, rollbackable));
        }

        #endregion

        #region Selection
        bool _Selected;

        public event EventHandler SelectedChanged;

        [Browsable(false)]
        public bool Selected
        {
            get { return _Selected; }
            set
            {
                if (_Selected != value)
                {
                    bool old = _Selected;
                    _Selected = value;
                    OnSelectedChanged();

                    var e = new Blumind.Core.PropertyChangedEventArgs("Selected", old, _Selected, ChangeTypes.Visual, false);
                    e.Rollbackable = false;
                    OnPropertyChanged(e);
                }
            }
        }

        protected virtual void OnSelectedChanged()
        {
            if (SelectedChanged != null)
                SelectedChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Bounds
        Rectangle _Bounds;

        public event EventHandler BoundsChanged;

        [Browsable(false)]
        public Rectangle Bounds
        {
            get { return _Bounds; }
            set
            {
                if (_Bounds != value)
                {
                    _Bounds = value;
                    OnBoundsChanged();
                }
            }
        }

        [Browsable(false)]
        public int Left
        {
            get { return Bounds.Left; }
        }

        [Browsable(false)]
        public int Top
        {
            get { return Bounds.Top; }
        }

        [Browsable(false)]
        public int Width
        {
            get { return Bounds.Width; }
        }

        [Browsable(false)]
        public int Height
        {
            get { return Bounds.Height; }
        }

        [Browsable(false)]
        public int Right
        {
            get { return Bounds.Right; }
        }

        [Browsable(false)]
        public int Bottom
        {
            get { return Bounds.Bottom; }
        }

        [Browsable(false)]
        public Point Location
        {
            get { return Bounds.Location; }
            set
            {
                Bounds = new Rectangle(value, Size);
            }
        }

        [Browsable(false)]
        public Size Size
        {
            get { return Bounds.Size; }
            set
            {
                Bounds = new Rectangle(Location, value);
            }
        }

        protected virtual void OnBoundsChanged()
        {
            if (BoundsChanged != null)
                BoundsChanged(this, EventArgs.Empty);
        }

        #endregion

        #region I/O
        [Browsable(false)]
        public abstract string XmlElementName
        {
            get;
        }

        public virtual void Serialize(XmlDocument dom, XmlElement node)
        {
            if (!string.IsNullOrEmpty(this.ID))
                node.SetAttribute("id", this.ID);
            if (!string.IsNullOrEmpty(this.Text))
                node.SetAttribute("text", this.Text);

            //
            node.SetAttribute("x", this.Bounds.X.ToString());
            node.SetAttribute("y", this.Bounds.Y.ToString());
            node.SetAttribute("width", this.Bounds.Width.ToString());
            node.SetAttribute("height", this.Bounds.Height.ToString());

            if (!string.IsNullOrEmpty(this.Remark))
            {
                ST.WriteTextNode(node, "remark", this.Remark);
            }
        }

        public virtual void Deserialize(Version documentVersion, XmlElement node)
        {
            this.ID = node.GetAttribute("id");
            this.Text = node.GetAttribute("text");

            //
            if (node.HasAttribute("x"))
            {
                this.Bounds = new Rectangle(
                    ST.GetIntDefault(node.GetAttribute("x")),
                    ST.GetIntDefault(node.GetAttribute("y")),
                    ST.GetIntDefault(node.GetAttribute("width")),
                    ST.GetIntDefault(node.GetAttribute("height")));
            }

            //
            this.Remark = ST.ReadTextNode(node, "remark");
            if (string.IsNullOrEmpty(this.Remark)) // 向后兼容
            {
                string notes = ST.ReadCDataNode(node, "description");
                if (string.IsNullOrEmpty(notes))
                    notes = ST.ReadTextNode(node, "description");
                this.Remark = notes;
            }
        }

        public virtual string SerializeText(bool recursive, List<ChartObject> accessedObjects)
        {
            if (accessedObjects != null && !accessedObjects.Contains(this))
                accessedObjects.Add(this);
            return Text;
        }
        #endregion

        #region IComponent
        /*ISite _Site;

        public event EventHandler Disposed;

        [Browsable(false)]
        public ISite Site
        {
            get
            {
                if (_Site == null)
                    _Site = new CodeObjectSite(this);
                return _Site;
            }
            set
            {
                _Site = value;
            }
        }

        public void Dispose()
        {
            if (Disposed != null)
                Disposed(this, EventArgs.Empty);
        }*/
        #endregion
    }
}
