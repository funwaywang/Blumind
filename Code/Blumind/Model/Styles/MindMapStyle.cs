using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using Blumind.Core;
using Blumind.Globalization;
using Blumind.Model.Styles;

namespace Blumind.Model.Styles
{
    class MindMapStyle : ChartStyle
    {
        public const int DefaultLayerSpace = 80;
        public const int DefaultItemsSpace = 10;

        Color _NodeBackColor = Color.White;
        Color _NodeForeColor = Color.Black;
        Color _LinkColor = Color.Green;

        int _LayerSpace = DefaultLayerSpace;
        int _ItemsSpace = DefaultItemsSpace;
        //private Nullable<HatchStyle> _BackgroundGrain = null;
        //private String _BackgroundImage;

        public MindMapStyle()
        {
        }

        //[DefaultValue(null)]
        //public Nullable<HatchStyle> BackgroundGrain
        //{
        //    get { return _BackgroundGrain; }
        //    set
        //    {
        //        if (_BackgroundGrain != value)
        //        {
        //            _BackgroundGrain = value;
        //            OnValueChanged();
        //        }
        //    }
        //}

        [DefaultValue(typeof(Color), "White")]
        [LocalDisplayName("Node Back Color"), LocalCategory("Color")]
        public Color NodeBackColor
        {
            get { return _NodeBackColor; }
            set
            {
                if (_NodeBackColor != value)
                {
                    _NodeBackColor = value;
                    OnValueChanged();
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
                    _NodeForeColor = value;
                    OnValueChanged();
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
                    _LinkColor = value;
                    OnValueChanged();
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
                    _LayerSpace = value;
                    OnValueChanged();
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
                    _ItemsSpace = value;
                    OnValueChanged();
                }
            }
        }

        [Browsable(false)]
        public override bool IsEmpty
        {
            get
            {
                return base.IsEmpty
                    && NodeBackColor.IsEmpty 
                    && NodeForeColor.IsEmpty 
                    && LinkLineColor.IsEmpty
                    && LayerSpace != DefaultLayerSpace
                    && ItemsSpace != DefaultItemsSpace;
            }
        }

        //[DefaultValue(null)]
        //public String BackgroundImage
        //{
        //    get { return _BackgroundImage; }
        //    set { _BackgroundImage = value; }
        //}

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());

            BuildStyleString(sb, "Node Back Color", !NodeBackColor.IsEmpty, ST.ToString(NodeBackColor));
            BuildStyleString(sb, "Node Fore Color", !NodeForeColor.IsEmpty, ST.ToString(NodeForeColor));
            BuildStyleString(sb, "Link Line Color", !LinkLineColor.IsEmpty, ST.ToString(LinkLineColor));
            BuildStyleString(sb, "Layer Space", LayerSpace != DefaultLayerSpace, LayerSpace);
            BuildStyleString(sb, "Items Space", ItemsSpace != DefaultItemsSpace, ItemsSpace);

            return sb.ToString();
        }

        public override void Copy(Style style)
        {
            base.Copy(style);

            if (style is MindMapStyle)
            {
                var ms = (MindMapStyle)style;
                
                NodeBackColor = ms.NodeBackColor;
                NodeForeColor = ms.NodeForeColor;
                LinkLineColor = ms.LinkLineColor;
                ItemsSpace = ms.ItemsSpace;
                LayerSpace = ms.LayerSpace;
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}
