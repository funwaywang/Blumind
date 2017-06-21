using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Blumind.Core;
using Blumind.Globalization;
using Blumind.Model.Styles;

namespace Blumind.Model.Documents
{
    abstract class ChartPage : ModifyObject, Blumind.Core.INotifyPropertyChanged, IRemark, ISerializable
    {
        string _Name;
        string _Remark;
        Size _PageSize;
        Padding _Margin = new Padding(20);

        public event ChartObjectEventHandler ChartObjectAdded;
        public event Blumind.Core.PropertyChangedEventHandler ChartObjectPropertyChanged;
        public event EventHandler NameChanged;
        public event EventHandler RemarkChanged;
        public event ChartObjectPropertyEventHandler ObjectStyleChanged;

        public ChartPage()
        {
            PictureLibrary = new PictureLibrary();
            _PageSize = new Size(1124, 796);
        }

        [Browsable(false)]
        public abstract ChartType Type { get; }

        [Browsable(false)]
        public Document Document { get; set; }

        [Browsable(false)]
        public Size PageSize
        {
            get{return _PageSize;}
            set
            {
                if (_PageSize != value)
                {
                    var old = _PageSize;
                    _PageSize = value;
                    OnPageSizeChanged(old);
                }
            }
        }

        [DefaultValue(typeof(Padding), "20, 20, 20, 20"), Browsable(false)]
        public Padding Margin
        {
            get { return _Margin; }
            set { _Margin = value; }
        }

        [Browsable(false)]
        public bool LayoutInitialized { get; set; }

        public abstract void ApplyTheme(ChartTheme chartTheme);

        [DefaultValue(null), LocalDisplayName("Name"), Browsable(false)]
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnNameChanged();
                }
            }
        }

        [Browsable(false)]
        public PictureLibrary PictureLibrary { get; private set; }

        [Browsable(false), DefaultValue(null)]
        public string Remark
        {
            get { return _Remark; }
            set
            {
                if (_Remark != value)
                {
                    _Remark = value;
                    OnRemarkChanged();
                }
            }
        }

        protected virtual void OnPageSizeChanged(Size old)
        {
            OnPropertyChanged("PageSize", old, PageSize, ChangeTypes.Layout | ChangeTypes.Data);
        }

        protected virtual void OnRemarkChanged()
        {
            if (RemarkChanged != null)
            {
                RemarkChanged(this, EventArgs.Empty);
            }
        }

        protected virtual void OnChartObjectAdded(ChartObject chartObject)
        {
            if (ChartObjectAdded != null)
                ChartObjectAdded(this, new ChartObjectEventArgs(chartObject));
        }

        public virtual void OnChartObjectPropertyChanged(object sender, Blumind.Core.PropertyChangedEventArgs e)
        {
            if (e.HasChanges(ChangeTypes.Data))
            {
                this.Modified = true;
            }

            if (ChartObjectPropertyChanged != null)
                ChartObjectPropertyChanged(this, e);
        }

        protected virtual void OnNameChanged()
        {
            Modified = true;

            if (NameChanged != null)
            {
                NameChanged(this, EventArgs.Empty);
            }
        }

        public virtual ChartTheme GetChartTheme()
        {
            return null;
        }

        public virtual Size GetContentSize()
        {
            return Size.Empty;
        }

        internal void OnObjectStyleChanged(ChartObject chartObject, string propertyName, ChangeTypes changeTypes)
        {
            OnObjectStyleChanged(new ChartObjectPropertyEventArgs(chartObject, propertyName, changeTypes));
        }

        protected virtual void OnObjectStyleChanged(ChartObjectPropertyEventArgs args)
        {
            Modified = true;

            if (ObjectStyleChanged != null)
            {
                ObjectStyleChanged(this, args);
            }
        }

        public virtual Image CreateThumbImage(Size thumbSize)
        {
            return null;
        }

        public virtual bool EnsureChartLayouted()
        {
            if (LayoutInitialized)
                return true;

            return false;
        }

        public string GetNextObjectID()
        {
            if (Document != null)
                return Document.GetNextObjectID();
            else
                return null;
        }

        #region Style
        public const int DefaultWidgetMargin = 1;
        Color _BackColor = Color.White;
        Color _ForeColor = Color.Black;
        Color _LineColor = Color.Black;
        Color _BorderColor = Color.DarkGray;
        Color _SelectColor = SystemColors.Highlight;
        Color _HoverColor = Color.MediumSlateBlue;
        Font _Font;
        int _LineWidth = 1;
        int _BorderWidth = 1;
        int _WidgetMargin = 1;
        Size _PictureThumbSize = new Size(100, 100);

        [DefaultValue(typeof(Color), "White")]
        [LocalDisplayName("Back Color"), LocalCategory("Color")]
        public Color BackColor
        {
            get { return _BackColor; }
            set 
            {
                if (_BackColor != value)
                {
                    var old = _BackColor;
                    _BackColor = value;
                    OnPropertyChanged("BackColor", old, BackColor, ChangeTypes.Visual);
                }
            }
        }

        [DefaultValue(typeof(Color), "Black")]
        [LocalDisplayName("Fore Color"), LocalCategory("Color")]
        public Color ForeColor
        {
            get { return _ForeColor; }
            set 
            {
                if (_ForeColor != value)
                {
                    var old = _ForeColor;
                    _ForeColor = value;
                    OnPropertyChanged("ForeColor", old, ForeColor, ChangeTypes.Visual);
                }
            }
        }

        [DefaultValue(null)]
        [LocalDisplayName("Font"), LocalCategory("Appearance")]
        public virtual Font Font
        {
            get { return _Font; }
            set
            {
                if (_Font != value)
                {
                    var old = _Font;
                    _Font = value;
                    OnPropertyChanged("Font", old, Font, ChangeTypes.Visual | ChangeTypes.Layout);
                }
            }
        }

        [DefaultValue(typeof(Color), "Black")]
        [LocalDisplayName("Line Color"), LocalCategory("Color")]
        public Color LineColor
        {
            get { return _LineColor; }
            set
            {
                if (_LineColor != value)
                {
                    var old = _LineColor;
                    _LineColor = value;
                    OnPropertyChanged("LineColor", old, _LineColor, ChangeTypes.Visual);
                }
            }
        }

        [DefaultValue(typeof(Color), "DarkGray")]
        [LocalDisplayName("Border Color"), LocalCategory("Color")]
        public Color BorderColor
        {
            get { return _BorderColor; }
            set
            {
                if (_BorderColor != value)
                {
                    var old = _BorderColor;
                    _BorderColor = value;
                    OnPropertyChanged("BorderColor", old, _BorderColor, ChangeTypes.Visual);
                }
            }
        }

        [DefaultValue(typeof(Color), "Highlight")]
        [LocalDisplayName("Selection Color"), LocalCategory("Color")]
        public Color SelectColor
        {
            get { return _SelectColor; }
            set
            {
                if (_SelectColor != value)
                {
                    var old = _SelectColor;
                    _SelectColor = value;
                    OnPropertyChanged("SelectColor", old, _SelectColor, ChangeTypes.Visual);
                }
            }
        }

        [DefaultValue(typeof(Color), "MediumSlateBlue")]
        [LocalDisplayName("Hover Color"), LocalCategory("Color")]
        public Color HoverColor
        {
            get { return _HoverColor; }
            set
            {
                if (_HoverColor != value)
                {
                    var old = _HoverColor;
                    _HoverColor = value;
                    OnPropertyChanged("_HoverColor", old, _HoverColor, ChangeTypes.Visual);
                }
            }
        }

        [DefaultValue(1)]
        [LocalDisplayName("Line Width"), LocalCategory("Appearance")]
        public int LineWidth
        {
            get { return _LineWidth; }
            set
            {
                if (value <= 0)
                    return;

                if (_LineWidth != value)
                {
                    var old = _LineWidth;
                    _LineWidth = value;
                    OnPropertyChanged("LineWidth", old, _LineWidth, ChangeTypes.Layout | ChangeTypes.Visual);
                }
            }
        }

        [DefaultValue(1)]
        [LocalDisplayName("Border Width"), LocalCategory("Appearance")]
        public int BorderWidth
        {
            get { return _BorderWidth; }
            set
            {
                if (value <= 0)
                    return;

                if (_BorderWidth != value)
                {
                    var old = _BorderWidth;
                    _BorderWidth = value;
                    OnPropertyChanged("BorderWidth", old, _BorderWidth, ChangeTypes.Layout | ChangeTypes.Visual);
                }
            }
        }

        [DefaultValue(DefaultWidgetMargin)]
        [DesignOnly(true), LocalDisplayName("Widget Margin"), LocalCategory("Layout")]
        public int WidgetMargin
        {
            get { return _WidgetMargin; }
            set 
            {
                if (_WidgetMargin != value)
                {
                    var old = _WidgetMargin;
                    _WidgetMargin = value;
                    OnPropertyChanged("WidgetMargin", old, _WidgetMargin, ChangeTypes.Layout | ChangeTypes.Visual);
                }
            }
        }

        [DefaultValue(typeof(Size), "100, 100")]
        [DesignOnly(true), LocalDisplayName("Picture Thumb Size"), LocalCategory("Layout")]
        public Size PictureThumbSize
        {
            get { return _PictureThumbSize; }
            set
            {
                if (_PictureThumbSize != value)
                {
                    var old = _PictureThumbSize;
                    _PictureThumbSize = value;
                    OnPictureThumbSizeChanged(old);
                }
            }
        }

        protected virtual void OnPictureThumbSizeChanged(Size old)
        {
            OnPropertyChanged("PictureThumbSize", old, _PictureThumbSize, ChangeTypes.Layout | ChangeTypes.Visual);
        }

        public virtual string StyleToString()
        {
            var sb = new StringBuilder();

            BuildStyleString(sb, "Back Color", !BackColor.IsEmpty, ST.ToString(BackColor));
            BuildStyleString(sb, "Fore Color", !ForeColor.IsEmpty, ST.ToString(ForeColor));
            BuildStyleString(sb, "Line Color", !LineColor.IsEmpty, ST.ToString(LineColor));
            BuildStyleString(sb, "Border Color", !BorderColor.IsEmpty, ST.ToString(BorderColor));
            BuildStyleString(sb, "Select Color", !SelectColor.IsEmpty, ST.ToString(SelectColor));
            BuildStyleString(sb, "Hover Color", !HoverColor.IsEmpty, ST.ToString(HoverColor));
            BuildStyleString(sb, "Font", Font != null, Font);
            BuildStyleString(sb, "Line Width", LineWidth != 1, LineWidth);
            BuildStyleString(sb, "Border Width", BorderWidth != 1, BorderWidth);

            return sb.ToString();
        }

        protected void BuildStyleString(StringBuilder sb, string name, bool validate, object value)
        {
            if (validate)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(LanguageManage.GetText(name));
                sb.Append(":");
                sb.Append(value.ToString());
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event Blumind.Core.PropertyChangedEventHandler PropertyChanged;

        [Browsable(false)]
        public bool PropertyChangeSuspending { get; set; }

        protected virtual void OnPropertyChanged(Blumind.Core.PropertyChangedEventArgs e)
        {
            if (PropertyChangeSuspending)
                return;

            if (!e.HasChanges(ChangeTypes.NoData))
                Modified = true;

            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
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

        #region I/O
        public virtual void Serialize(XmlDocument dom, XmlElement node)
        {
            if (node == null)
                return;

            node.SetAttribute("name", this.Name);
            node.SetAttribute("type", this.Type.ToString());
            ST.WriteTextNode(node, "remark", Remark);
        }

        public virtual void Deserialize(Version documentVersion, XmlElement node)
        {
            if (node == null)
                return;

            this.Name = node.GetAttribute("name");
            this.Remark = ST.ReadTextNode(node, "remark");
        }

        #endregion
    }
}
