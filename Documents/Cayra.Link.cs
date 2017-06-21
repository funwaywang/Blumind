public class Link : UserControl, IComponentConnector
{
    // Fields
    private bool _contentLoaded;
    private Path arrow;
    private double ArrowBase;
    internal Canvas canvas;
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(Link), new PropertyMetadata(new PropertyChangedCallback(Link.OnColorChanged)));
    public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(MapData.LinksRow), typeof(Link), new PropertyMetadata(new PropertyChangedCallback(Link.OnDataChanged)));
    public Color fromColor;
    private FrameworkElement fromElement;
    public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(Point), typeof(Link), new PropertyMetadata(new PropertyChangedCallback(Link.OnFromChanged)));
    public static readonly DependencyProperty FromThicknessProperty = DependencyProperty.Register("FromThickness", typeof(Thickness), typeof(Link), new PropertyMetadata(new PropertyChangedCallback(Link.OnFromThicknessChanged)));
    private double rand1;
    private double rand2;
    private Random random;
    private Text2Path textControl;
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Link), new PropertyMetadata(new PropertyChangedCallback(Link.OnTextChanged)));
    public Color toColor;
    private FrameworkElement toElement;
    public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(Point), typeof(Link), new PropertyMetadata(new PropertyChangedCallback(Link.OnToChanged)));
    public static readonly DependencyProperty ToThicknessProperty = DependencyProperty.Register("ToThickness", typeof(Thickness), typeof(Link), new PropertyMetadata(new PropertyChangedCallback(Link.OnToThicknessChanged)));

    // Methods
    public Link()
    {
        this.fromColor = Colors.Transparent;
        this.toColor = Colors.Transparent;
        this.ArrowBase = 7.0;
        this.random = new Random();
        this.InitializeComponent();
        this.rand1 = this.random.NextDouble();
        this.rand2 = this.random.NextDouble();
        this.arrow = new Path();
        this.arrow.Stroke = Brushes.Black;
        this.arrow.Fill = Brushes.White;
        this.arrow.StrokeThickness = 2.0;
        this.arrow.HorizontalAlignment = HorizontalAlignment.Left;
        this.arrow.VerticalAlignment = VerticalAlignment.Top;
        this.canvas.Children.Add(this.arrow);
        this.textControl = new Text2Path();
        this.textControl.Text = "No text";
        this.textControl.PathDistance = 5M;
        this.canvas.Children.Add(this.textControl);
    }

    public Link(FrameworkElement from, FrameworkElement to, MapData.NodesRow fromNode, MapData.NodesRow toNode) : this()
    {
        this.fromElement = from;
        if (fromNode != null)
        {
            this.fromColor = Style.ConvertColor(fromNode.NodeStyle.FillColor);
        }
        this.toElement = to;
        if (toNode != null)
        {
            this.toColor = Style.ConvertColor(toNode.NodeStyle.FillColor);
        }
    }

    public void DrowArrow()
    {
        Color toColor;
        Point org = (Point) base.GetValue(FromProperty);
        Point to = (Point) base.GetValue(ToProperty);
        StreamGeometry geometry = new StreamGeometry();
        StreamGeometryContext context = geometry.Open();
        Point startPoint = GetOffsetPoint(org, to, this.ArrowBase);
        Point point4 = GetRandomPoint(org, to, 0.5, this.rand1);
        Point point5 = GetRandomPoint(to, org, 0.5, this.rand2);
        Point point6 = GetOffsetPoint(org, to, -this.ArrowBase);
        Point point7 = GetOffsetPoint(point4, to, -5.0);
        context.BeginFigure(startPoint, true, true);
        context.BezierTo(point4, point5, GetOffsetPoint(to, org, -2.0), true, true);
        context.LineTo(GetOffsetPoint(to, org, 1.0), true, true);
        context.BezierTo(point5, point7, point6, true, true);
        context.Close();
        this.arrow.Data = geometry;
        if ((this.toColor == Colors.Transparent) && (this.fromColor == Colors.Transparent))
        {
            toColor = (Color) base.GetValue(ColorProperty);
            toColor.A = 0x7d;
            this.arrow.Stroke = new SolidColorBrush(toColor);
            toColor.A = 60;
            this.arrow.Fill = new SolidColorBrush(toColor);
        }
        else
        {
            Point point8 = new Point(0.0, 0.0);
            Point point9 = new Point(1.0, 1.0);
            if (point4.X < point6.X)
            {
                point8.X = 1.0;
                point9.X = 0.0;
            }
            if (point4.Y < point6.Y)
            {
                point8.Y = 1.0;
                point9.Y = 0.0;
            }
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint = point8;
            brush.EndPoint = point9;
            LinearGradientBrush brush2 = brush.Clone();
            toColor = this.toColor;
            toColor.A = 0x7d;
            brush.GradientStops.Add(new GradientStop(toColor, 0.6));
            toColor.A = 60;
            brush2.GradientStops.Add(new GradientStop(toColor, 0.6));
            toColor = this.fromColor;
            toColor.A = 0x7d;
            brush.GradientStops.Add(new GradientStop(toColor, 0.2));
            toColor.A = 60;
            brush2.GradientStops.Add(new GradientStop(toColor, 0.2));
            this.arrow.Stroke = brush;
            this.arrow.Fill = brush2;
        }
        if (((this.Data != null) && this.Data.IsInTable) && (this.Data.Type == "$EMPTY_LINK_TYPE"))
        {
            this.Text = null;
        }
        if ((this.Text != null) && (this.Text.Length > 0))
        {
            geometry = new StreamGeometry();
            geometry.FillRule = FillRule.EvenOdd;
            context = geometry.Open();
            if (org.X < to.X)
            {
                context.BeginFigure(point6, true, false);
                context.BezierTo(point7, point5, GetOffsetPoint(to, org, 2.0), true, true);
            }
            else
            {
                context.BeginFigure(GetOffsetPoint(to, org, -2.0), true, false);
                context.BezierTo(point5, point4, startPoint, true, true);
            }
            context.Close();
            this.textControl.Color = this.Color;
            this.textControl.Text = this.Text;
            this.textControl.Geometry = geometry;
            this.textControl.Visibility = Visibility.Visible;
        }
        else
        {
            this.textControl.Geometry = null;
            this.textControl.Visibility = Visibility.Collapsed;
        }
    }

    private static Point GetElementCenter(FrameworkElement elem)
    {
        Point point = new Point();
        if (elem.ActualWidth != 0.0)
        {
            point.X = elem.ActualWidth / 2.0;
        }
        else
        {
            point.X = elem.DesiredSize.Width / 2.0;
        }
        if (elem.ActualHeight != 0.0)
        {
            point.Y = elem.ActualHeight / 2.0;
            return point;
        }
        point.Y = elem.DesiredSize.Height / 2.0;
        return point;
    }

    private static Point GetOffsetPoint(Point org, Point to, double dist)
    {
        double d = Math.Atan2(to.Y - org.Y, to.X - org.X) + 1.5707963267948966;
        double num2 = Math.Cos(d) * dist;
        double num3 = Math.Sin(d) * dist;
        return new Point(org.X + num2, org.Y + num3);
    }

    private static Point GetRandomPoint(Point from, Point to, double scale, double rand)
    {
        double x = to.X - from.X;
        double y = to.Y - from.Y;
        double num3 = Math.Sqrt((x * x) + (y * y));
        double d = Math.Atan2(y, x) + (0.52359877559829882 * (rand - 0.5));
        double num6 = Math.Cos(d) * num3;
        double num7 = Math.Sin(d) * num3;
        return new Point(from.X + (num6 * scale), from.Y + (num7 * scale));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
        if (!this._contentLoaded)
        {
            this._contentLoaded = true;
            Uri resourceLocator = new Uri("/View;component/controls/link.xaml", UriKind.Relative);
            Application.LoadComponent(this, resourceLocator);
        }
    }

    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Link) d).DrowArrow();
    }

    private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Link) d).DrowArrow();
    }

    private static void OnFromChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Link) d).DrowArrow();
    }

    private static void OnFromThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Thickness newValue = (Thickness) e.NewValue;
        Link link = (Link) d;
        Point elementCenter = GetElementCenter(link.fromElement);
        link.SetValue(FromProperty, new Point(newValue.Left + elementCenter.X, newValue.Top + elementCenter.Y));
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Link) d).DrowArrow();
    }

    private static void OnToChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Link) d).DrowArrow();
    }

    private static void OnToThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Thickness newValue = (Thickness) e.NewValue;
        Link link = (Link) d;
        Point elementCenter = GetElementCenter(link.toElement);
        link.SetValue(ToProperty, new Point(newValue.Left + elementCenter.X, newValue.Top + elementCenter.Y));
    }

    [EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        if (connectionId == 1)
        {
            this.canvas = (Canvas) target;
        }
        else
        {
            this._contentLoaded = true;
        }
    }

    // Properties
    public Color Color
    {
        get
        {
            return (Color) base.GetValue(ColorProperty);
        }
        set
        {
            base.SetValue(ColorProperty, value);
        }
    }

    public MapData.LinksRow Data
    {
        get
        {
            return (MapData.LinksRow) base.GetValue(DataProperty);
        }
        set
        {
            base.SetValue(DataProperty, value);
        }
    }

    public Point From
    {
        get
        {
            return (Point) base.GetValue(FromProperty);
        }
        set
        {
            base.SetValue(FromProperty, value);
        }
    }

    public Thickness FromThickness
    {
        get
        {
            return (Thickness) base.GetValue(FromThicknessProperty);
        }
        set
        {
            base.SetValue(FromThicknessProperty, value);
        }
    }

    public string Text
    {
        get
        {
            return (string) base.GetValue(TextProperty);
        }
        set
        {
            base.SetValue(TextProperty, value);
        }
    }

    public Point To
    {
        get
        {
            return (Point) base.GetValue(ToProperty);
        }
        set
        {
            base.SetValue(ToProperty, value);
        }
    }

    public Thickness ToThickness
    {
        get
        {
            return (Thickness) base.GetValue(ToThicknessProperty);
        }
        set
        {
            base.SetValue(ToThicknessProperty, value);
        }
    }
}


