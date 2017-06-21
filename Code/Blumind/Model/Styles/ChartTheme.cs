using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using Blumind.Controls;
using Blumind.Controls.MapViews;
using Blumind.Globalization;
using Blumind.Model;

namespace Blumind.Model.Styles
{
    [DefaultProperty("Name")]
    class ChartTheme
    {
        public const int DefaultLayerSpace = 80;
        public const int DefaultItemsSpace = 10;

        string _Name;
        string _Filename;
        Image _Icon;
        bool _IsInternal;
        ChartThemeFolder _Folder;
        string _Description;
        Font _Font;
        int _LineWidth = 1;
        int _BorderWidth = 1;
        int _LayerSpace = DefaultLayerSpace;
        int _ItemsSpace = DefaultItemsSpace;

        Color _BackColor = Color.White;
        Color _ForeColor = Color.Black;
        Color _LineColor = Color.Black;
        Color _BorderColor = Color.DarkGray;
        Color _NodeBackColor = Color.White;
        Color _NodeForeColor = Color.Black;
        Color _SelectColor = SystemColors.Highlight;
        Color _HoverColor = Color.MediumSlateBlue;
        Color _LinkColor = Color.Green;

        Color _RootBackColor = Color.White;
        Color _RootForeColor = Color.Black;
        Color _RootBorderColor = Color.Black;

        public event System.EventHandler NameChanged;
        public event EventHandler ValueChanged;

        public ChartTheme()
        {
        }

        public ChartTheme(string name)
        {
            Name = name;
        }

        public ChartTheme(ChartTheme copyTheme)
        {
            if (copyTheme != null)
            {
                Copy(copyTheme);
            }
        }

        [DefaultValue(null), Browsable(false)]
        //[LocalDisplayName("Name"), LocalCategory("General")]
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
        public string Filename
        {
            get { return _Filename; }
            set { _Filename = value; }
        }

        [DefaultValue(typeof(Color), "White")]
        [LocalDisplayName("Back Color"), LocalCategory("Color")]
        public Color BackColor
        {
            get { return _BackColor; }
            set
            {
                if (_BackColor != value)
                {
                    _BackColor = value;
                    OnValueChanged();
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
                    _ForeColor = value;
                    OnValueChanged();
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
                    _LineColor = value;
                    OnValueChanged();
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
                    _BorderColor = value;
                    OnValueChanged();
                }
            }
        }

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

        [DefaultValue(typeof(Color), "Highlight")]
        [LocalDisplayName("Selection Color"), LocalCategory("Color")]
        public Color SelectColor
        {
            get { return _SelectColor; }
            set
            {
                if (_SelectColor != value)
                {
                    _SelectColor = value;
                    OnValueChanged();
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
                    _HoverColor = value;
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

        [DefaultValue(typeof(Color), "White")]
        [LocalDisplayName("Center Topic Back Color"), LocalCategory("Color")]
        public Color RootBackColor
        {
            get { return _RootBackColor; }
            set { _RootBackColor = value; }
        }

        [DefaultValue(typeof(Color), "Black")]
        [LocalDisplayName("Center Topic Fore Color"), LocalCategory("Color")]
        public Color RootForeColor
        {
            get { return _RootForeColor; }
            set { _RootForeColor = value; }
        }

        [DefaultValue(typeof(Color), "Black")]
        [LocalDisplayName("Center Topic Border Color"), LocalCategory("Color")]
        public Color RootBorderColor
        {
            get { return _RootBorderColor; }
            set { _RootBorderColor = value; }
        }

        [Browsable(false)]
        public Image Icon
        {
            get
            {
                if (_Icon == null)
                {
                    _Icon = CreateIcon();
                }

                return _Icon; 
            }
        }

        [Browsable(false)]
        public bool IsInternal
        {
            get { return _IsInternal; }

            private set { _IsInternal = value; }
        }

        [Browsable(false)]
        public ChartThemeFolder Folder
        {
            get { return _Folder; }
            internal set { _Folder = value; }
        }

        [Browsable(false)]
        public bool IsDefault
        {
            get { return this == ChartThemeManage.Default.DefaultTheme; }
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
                    _LineWidth = value;
                    OnValueChanged();
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
                    _BorderWidth = value;
                    OnValueChanged();
                }
            }
        }

        [Browsable(false), DefaultValue(null), LocalDisplayName("Notes"), LocalCategory("Data")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        [DefaultValue(null)]
        [LocalDisplayName("Font"), LocalCategory("Appearance")]
        public Font Font
        {
            get { return _Font; }
            set
            {
                if (_Font != value)
                {
                    _Font = value;
                    OnValueChanged();
                }
            }
        }

        [DefaultValue(DefaultLayerSpace)]
        [LocalDisplayName("Layer Space"), LocalCategory("Layout")]
        public int LayerSpace
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
        public int ItemsSpace
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

        public void Copy(ChartTheme theme)
        {
            if (theme == null)
                throw new ArgumentNullException();

            //Copy((Style)theme);

            Name = theme.Name;
            RootBackColor = theme.RootBackColor;
            RootForeColor = theme.RootForeColor;
            RootBorderColor = theme.RootBorderColor;
            IsInternal = theme.IsInternal;

            BackColor = theme.BackColor;
            ForeColor = theme.ForeColor;
            LineColor = theme.LineColor;
            BorderColor = theme.BorderColor;
            Font = theme.Font;
            LineWidth = theme.LineWidth;
            BorderWidth = theme.BorderWidth;
            NodeBackColor = theme.NodeBackColor;
            NodeForeColor = theme.NodeForeColor;
            SelectColor = theme.SelectColor;
            HoverColor = theme.HoverColor;
            LinkLineColor = theme.LinkLineColor;
            ItemsSpace = theme.ItemsSpace;
            LayerSpace = theme.LayerSpace;
        }

        public override string ToString()
        {
            return Name;
        }

        Image CreateIcon()
        {
            const int width = 16;
            const int height = 16;
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics grf = Graphics.FromImage(bmp))
            {
                PaintHelper.SetHighQualityRender(grf);

                // background
                GraphicsPath path = PaintHelper.GetRoundRectangle(new Rectangle(0, 0, width, height), 1);
                grf.FillPath(new SolidBrush(BackColor), path);
                //grf.FillEllipse(new SolidBrush(BackColor), 0, 0, width, height);

                grf.DrawLine(new Pen(LineColor, 0.3f), 5, 5, 13, 13);

                // root
                grf.FillEllipse(new SolidBrush(RootBackColor), 0, 0, 9, 9);
                grf.DrawEllipse(new Pen(RootBorderColor, 0.3f), 0, 0, 8, 8);

                // node
                grf.FillEllipse(new SolidBrush(NodeBackColor), 9, 9, 7, 7);
                grf.DrawEllipse(new Pen(BorderColor, 0.3f), 9, 9, 6, 6);

                grf.Dispose();
            }

            return bmp;
        }

        internal void SetIsInternal(bool isInternal)
        {
            IsInternal = isInternal;
        }

        void OnNameChanged()
        {
            if (NameChanged != null)
                NameChanged(this, EventArgs.Empty);
        }

        void OnValueChanged()
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, EventArgs.Empty);
            }
        }
    }
}
