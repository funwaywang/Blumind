using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Blumind.Controls
{
    class Tip : Component
    {
        const int BorderSize = 1;
        const int TitleButtonSize = 16;
        const int AngularSize = 12;
        const int ResizeGripSize = 16;

        Control _Control;
        Color _BackColor;
        Color _ForeColor;
        Color _TitleColor;
        Color _BorderColor;
        Font _Font;
        Control _CustomControl;
        bool _ShowCloseButton;
        bool _ShowPinButton;
        string _Title;
        string _Text;
        Size _Size;
        TipForm tipWin;
        int _AutomaticDelay;
        TipTimer timer;
        bool _ShowAlways;
        TipButton tbClose;
        TipButton tbPin;
        bool _ShowAngular;
        Image _TitleIcon;
        Padding _Padding;
        Size _MinimumSize;
        bool _AutoSize;
        bool _Resizable;

        public Tip()
        {
            _BackColor = SystemColors.Info;
            _ForeColor = SystemColors.InfoText;
            _TitleColor = SystemColors.ActiveCaption;
            _BorderColor = SystemColors.ControlDarkDark;
            _ShowAngular = true;
            _AutoSize = true;
            AutomaticDelay = 500;

            Buttons = new List<TipButton>();
        }

        public Tip(Control control)
            : this()
        {
            Control = control;
        }

        [DefaultValue(typeof(Color), "Info")]
        public Color BackColor
        {
            get { return _BackColor; }
            set
            {
                if (_BackColor != value)
                {
                    _BackColor = value;
                    OnChanged();
                }
            }
        }

        [DefaultValue(typeof(Color), "InfoText")]
        public Color ForeColor
        {
            get { return _ForeColor; }
            set
            {
                if (_ForeColor != value)
                {
                    _ForeColor = value;
                    OnChanged();
                }
            }
        }

        [DefaultValue(typeof(Color), "ActiveCaption")]
        public Color TitleColor
        {
            get { return _TitleColor; }
            set
            {
                if (_TitleColor != value)
                {
                    _TitleColor = value;
                    OnChanged();
                }
            }
        }

        [DefaultValue(typeof(Color), "ControlDarkDark")]
        public Color BorderColor
        {
            get { return _BorderColor; }
            set { _BorderColor = value; }
        }

        [DefaultValue(null)]
        public Font Font
        {
            get { return _Font; }
            set
            {
                if (_Font != value)
                {
                    _Font = value;
                    OnChanged();
                }
            }
        }

        [DefaultValue(null)]
        public Control CustomControl
        {
            get { return _CustomControl; }
            set
            {
                if (_CustomControl != value)
                {
                    _CustomControl = value;
                    OnChanged();
                }
            }
        }

        [DefaultValue(false)]
        public bool ShowCloseButton
        {
            get { return _ShowCloseButton; }
            set 
            {
                if (_ShowCloseButton != value)
                {
                    _ShowCloseButton = value;
                    OnShowCloseButtonChanged();
                }
            }
        }

        [DefaultValue(false)]
        public bool ShowPinButton
        {
            get { return _ShowPinButton; }
            set 
            {
                if (_ShowPinButton != value)
                {
                    _ShowPinButton = value;
                    OnShowPinButtonChanged();
                }
            }
        }

        [DefaultValue(null)]
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    OnChanged();
                }
            }
        }

        [DefaultValue(null)]
        public Image TitleIcon
        {
            get { return _TitleIcon; }
            set 
            {
                if (_TitleIcon != value)
                {
                    _TitleIcon = value;
                    OnTitleIconChanged();
                }
            }
        }

        [DefaultValue(typeof(Padding), "0")]
        public Padding Padding
        {
            get { return _Padding; }
            set 
            {
                if (_Padding != value)
                {
                    _Padding = value;
                    OnPaddingChanged();
                }
            }
        }

        [DefaultValue(typeof(Size), "0, 0")]
        public Size MinimumSize
        {
            get { return _MinimumSize; }
            set 
            {
                if (_MinimumSize != value)
                {
                    _MinimumSize = value;
                    OnMinimumSizeChanged();
                }
            }
        }

        [DefaultValue(true)]
        public bool AutoSize
        {
            get { return _AutoSize; }
            set 
            {
                if (_AutoSize != value)
                {
                    _AutoSize = value;
                    OnAutoSizeChanged();
                }
            }
        }

        [DefaultValue(false)]
        public bool Resizable
        {
            get { return _Resizable; }
            set 
            {
                if (_Resizable != value)
                {
                    _Resizable = value;
                    OnResizableChanged();
                }
            }
        }

        [DefaultValue(typeof(Size), "0, 0")]
        public Size Size
        {
            get { return _Size; }
            set 
            {
                if (_Size != value)
                {
                    _Size = value;
                    OnSizeChanged();
                }
            }
        }

        [Browsable(false)]
        public bool WindowHasPin { get; private set; }

        [DefaultValue(0)]
        public int CaptionHeight { get; set; }

        [DefaultValue(null)]
        public string Text
        {
            get { return _Text; }
            set
            {
                if (_Text != value)
                {
                    _Text = value;
                    OnTextChanged();
                }
            }
        }

        [RefreshProperties(RefreshProperties.All)]
        public int AutoPopDelay { get; set; }

        [RefreshProperties(RefreshProperties.All)]
        public int InitialDelay { get; set; }

        [RefreshProperties(RefreshProperties.All)]
        public int ReshowDelay { get; set; }

        [RefreshProperties(RefreshProperties.All)]
        public int HideDelay { get; set; }

        [DefaultValue(500)]
        [RefreshProperties(RefreshProperties.All)]
        public int AutomaticDelay
        {
            get
            {
                return _AutomaticDelay;
            }

            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();

                if (_AutomaticDelay != value)
                {
                    _AutomaticDelay = value;
                    OnAutomaticDelayChanged();
                }
            }
        }

        [DefaultValue(false)]
        public bool ShowAlways
        {
            get { return _ShowAlways; }
            set { _ShowAlways = value; }
        }

        [DefaultValue(false)]
        public bool ShowOnUserFocused { get; set; }

        Rectangle TargetRectangle { get; set; }

        [DefaultValue(true)]
        public bool ShowAngular
        {
            get { return _ShowAngular; }
            set 
            {
                if (_ShowAngular != value)
                {
                    _ShowAngular = value;
                    OnShowAngularChanged();
                }
            }
        }

        protected bool NeedShowCaption
        {
            get { return Buttons.Count > 0 || !string.IsNullOrEmpty(Title) || TitleIcon != null; }
        }

        protected List<TipButton> Buttons { get; private set; }

        [Browsable(false), DefaultValue(false)]
        public bool Visible
        {
            get
            {
                return tipWin != null && !tipWin.IsDisposed && IsWindow(tipWin.Handle);
            }

            set
            {
                if (value)
                    Show();
                else
                    Hide();
            }
        }

        [Browsable(false), DefaultValue(null)]
        public Control Control
        {
            get { return _Control; }
            set 
            {
                if (_Control != value)
                {
                    var old = _Control;
                    _Control = value;
                    OnControlChanged(old);
                }
            }
        }

        protected virtual void OnAutomaticDelayChanged()
        {
            AutoPopDelay = AutomaticDelay * 10;
            InitialDelay = AutomaticDelay;
            ReshowDelay = AutomaticDelay / 5;
            HideDelay = AutomaticDelay;
        }

        protected virtual void OnShowCloseButtonChanged()
        {
            if (ShowCloseButton)
            {
                if(tbClose != null && Buttons.Contains(tbClose))
                    return;

                if (tbClose == null)
                {
                    tbClose = new TipButton()
                    {
                        Image = Properties.Resources.tip_close_button,
                        SortIndex = 999,
                    };
                    tbClose.Click += (s, a) => { HideInternal(); };
                }

                if(!Buttons.Contains(tbClose))
                    Buttons.Add(tbClose);
            }
            else
            {
                if (tbClose == null || !Buttons.Contains(tbClose))
                    return;

                Buttons.Remove(tbClose);
            }

            if (tipWin != null && tipWin.IsHandleCreated)
                tipWin.PerformLayout();
        }

        protected virtual void OnShowPinButtonChanged()
        {
            if (ShowPinButton)
            {
                if (tbPin != null && Buttons.Contains(tbPin))
                    return;

                if (tbPin == null)
                {
                    tbPin = new TipButton()
                    {
                        Image = WindowHasPin ? Properties.Resources.tip_pin_pressed_button : Properties.Resources.tip_pin_button,
                        SortIndex = 888,
                    };
                    tbPin.Click += (s, a) => { TogglePinStatus(); };
                }

                if (!Buttons.Contains(tbPin))
                    Buttons.Add(tbPin);
            }
            else
            {
                if (tbPin == null || !Buttons.Contains(tbPin))
                    return;

                Buttons.Remove(tbPin);
            }

            if (tipWin != null && tipWin.IsHandleCreated)
                tipWin.PerformLayout();
        }

        protected virtual void OnShowAngularChanged()
        {
            if (tipWin != null && tipWin.IsHandleCreated)
            {
                tipWin.ShowAngular = this.ShowAngular;
                LayoutTipWin();
            }
        }

        protected virtual void OnTitleIconChanged()
        {
            OnChanged();
        }

        protected virtual void OnPaddingChanged()
        {
            OnChanged();
        }

        protected virtual void OnMinimumSizeChanged()
        {
            OnChanged();
        }

        protected virtual void OnAutoSizeChanged()
        {
            OnChanged();
        }

        protected virtual void OnSizeChanged()
        {
            OnChanged();
        }

        protected virtual void OnResizableChanged()
        {
            if (tipWin != null && !tipWin.IsDisposed)
                tipWin.Resizable = this.Resizable && !AutoSize;
        }

        protected virtual void OnTextChanged()
        {
        }

        void OnChanged()
        {
            if (tipWin != null && !tipWin.IsDisposed && IsWindowVisible(tipWin.Handle))
            {
                tipWin.Invalidate();
            }
        }

        protected virtual void OnControlChanged(System.Windows.Forms.Control old)
        {
            if (old != null)
            {
                old.VisibleChanged -= Control_VisibleChanged;
                old.Disposed -= Control_Disposed;
            }

            if (Control != null)
            {
                Control.VisibleChanged += Control_VisibleChanged;
                Control.Disposed += Control_Disposed;
            }

            if (tipWin != null && !tipWin.IsDisposed)
            {
                var topForm = this.Control != null ? this.Control.TopLevelControl as Form : null;
                tipWin.Owner = topForm;
            }
        }

        void Control_Disposed(object sender, EventArgs e)
        {
            HideInternal();
        }

        void Control_VisibleChanged(object sender, EventArgs e)
        {
            HideInternal();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (tipWin != null && !tipWin.IsDisposed && !tipWin.Disposing)
                    tipWin.Dispose();
            }
        }

        void ExecAction(TipAction action)
        {
            int delay;
            switch (action)
            {
                case TipAction.Initial:
                    delay = InitialDelay;
                    break;
                case TipAction.Show:
                    delay = AutoPopDelay;
                    break;
                case TipAction.Hide:
                    delay = HideDelay;
                    break;
                default:
                    return;
            }

            if (timer != null && timer.Enabled)
                timer.Stop();
            if (delay <= 0)
            {
                ExecActionInternal(action);
            }
            else
            {
                timer = new TipTimer();
                timer.Interval = HideDelay;
                timer.Tick += (s, a) =>
                {
                    //if (tipWin != null && action == TipAction.Hide)
                    //{
                    //    System.Diagnostics.Debug.WriteLine(string.Format("TipWin.UserFocused:{0}", tipWin.UserFocused));
                    //    System.Diagnostics.Debug.WriteLine(string.Format("timer.Paused:{0}", timer.Paused));
                    //}
                    ExecActionInternal(action);
                };
                timer.Start();
            }
        }

        void ExecActionInternal(TipAction action)
        {
            switch (action)
            {
                case TipAction.Initial:
                    ShowInternal();
                    break;
                case TipAction.Hide:
                    HideInternal();
                    break;
            }
        }

        void LayoutTipWin()
        {
            Size sizeText, sizeTitle, sizeControl;
            Size size;
            if (AutoSize)
            {
                size = MeasureSize(Text, out sizeText, out sizeTitle, out sizeControl);
            }
            else
            {
                size = Size;
                LayoutByFixedSize(Size, out sizeText, out sizeTitle, out sizeControl);
            }

            var screenBounds = Screen.GetWorkingArea(tipWin);
            var angularSize = ShowAngular ? new Size(AngularSize, AngularSize) : Size.Empty;
            TipSide side;
            Rectangle rectAngular;
            tipWin.Location = LocateTipWin(screenBounds, TargetRectangle, size, out side, angularSize, out rectAngular);
            tipWin.SideOfTarget = side;
            tipWin.ShowAngular = ShowAngular;
            tipWin.AngularRectangle = rectAngular;
            tipWin.Size = size;

            var rect = tipWin.ClientRectangle;
            rect.Inflate(-BorderSize, -BorderSize);
            switch (tipWin.SideOfTarget)
            {
                case TipSide.Left:
                    rect.Width -= AngularSize;
                    break;
                case TipSide.Top:
                    rect.Height -= AngularSize;
                    break;
                case TipSide.Right:
                    rect.X += AngularSize;
                    rect.Width -= AngularSize;
                    break;
                case TipSide.Bottom:
                    rect.Y += AngularSize;
                    rect.Height -= AngularSize;
                    break;
            }

            tipWin.TitleBounds = new Rectangle(rect.X, rect.Y, size.Width, sizeTitle.Height);
            rect.X += Padding.Left;
            rect.Y += Padding.Top;
            rect.Width -= Padding.Horizontal;
            rect.Height -= Padding.Vertical;
            tipWin.TextBounds = new Rectangle(rect.X, tipWin.TitleBounds.Bottom + Padding.Top, sizeText.Width, sizeText.Height);
            tipWin.ControlBounds = new Rectangle(rect.X, tipWin.TextBounds.Bottom, sizeControl.Width, sizeControl.Height);

            size = tipWin.Size;
            if (ShowAngular)
            {
                switch (side)
                {
                    case TipSide.Left:
                    case TipSide.Right:
                        size.Width += AngularSize;
                        break;
                    case TipSide.Top:
                    case TipSide.Bottom:
                        size.Height += AngularSize;
                        break;
                }
            }
            size.Width++;    // 绘制边框时一个像素的偏移
            size.Height++;   // 绘制边框时一个像素的偏移
            tipWin.Size = size;
            tipWin.Region = GetTipWinRegion(tipWin.Size, ShowAngular, tipWin.SideOfTarget, tipWin.AngularRectangle);
        }

        Region GetTipWinRegion(Size size, bool showAngular, TipSide sideOfTarget, Rectangle angularRectangle)
        {
            if (!showAngular || angularRectangle.Width <= 0 || angularRectangle.Height <= 0)
                return null;

            var path = GetAngularPath(new Rectangle(Point.Empty, size), showAngular, sideOfTarget, angularRectangle, false);
            if (path != null)
                return new Region(path);
            else
                return null;
        }

        Point LocateTipWin(Rectangle screenRectangle, Rectangle targetRectangle, Size size, out TipSide tipSide,
            Size angularSize, out Rectangle angularRectangle)
        {
            var sides = new TipSide[] { TipSide.Right, TipSide.Bottom, TipSide.Left, TipSide.Top };
            int[] values = new int[] { 0, 0, 0, 0 };

            // right
            if (screenRectangle.Right - targetRectangle.Right >= size.Width + angularSize.Width)
                values[0] += 1;
            else
                values[0] -= 1;
            if (screenRectangle.Height >= size.Height)
                values[0] += 1;
            else
                values[0] -= 1;

            // bottom
            if (screenRectangle.Bottom - targetRectangle.Bottom >= size.Height + angularSize.Height)
                values[1] += 1;
            else
                values[1] -= 1;
            if (screenRectangle.Width >= size.Width)
                values[1] += 1;
            else
                values[1] -= 1;

            // left
            if (targetRectangle.Left - screenRectangle.Left >= size.Width + angularSize.Width)
                values[2] += 1;
            else
                values[2] -= 1;
            if (screenRectangle.Height >= size.Height)
                values[2] += 1;
            else
                values[2] -= 1;

            // top
            if (targetRectangle.Top - screenRectangle.Top >= size.Height + angularSize.Height)
                values[3] += 1;
            else
                values[3] -= 1;
            if (screenRectangle.Width >= size.Width)
                values[3] += 1;
            else
                values[3] -= 1;

            //
            var max = values.Max();
            var index = Array.IndexOf(values, max);
            tipSide = sides[index];

            //
            Point pos;
            var targetCenter = new Point(targetRectangle.X + targetRectangle.Width / 2, targetRectangle.Y + targetRectangle.Height / 2);
            switch (tipSide)
            {
                case TipSide.Left:
                    pos = new Point(targetRectangle.Left - angularSize.Width - size.Width, targetCenter.Y - size.Height / 2);
                    pos.Y = Math.Max(screenRectangle.Top, Math.Min(screenRectangle.Bottom - size.Height, pos.Y));
                    break;
                case TipSide.Top:
                    pos = new Point(targetCenter.X - size.Width / 2, targetRectangle.Top - angularSize.Height - size.Height);
                    pos.X = Math.Max(screenRectangle.Left, Math.Min(screenRectangle.Right - size.Width, pos.X));
                    break;
                case TipSide.Right:
                    pos = new Point(targetRectangle.Right, targetCenter.Y - size.Height / 2);
                    pos.Y = Math.Max(screenRectangle.Top, Math.Min(screenRectangle.Bottom - size.Height, pos.Y));
                    break;
                case TipSide.Bottom:
                    pos = new Point(targetCenter.X - size.Width / 2, targetRectangle.Bottom);
                    pos.X = Math.Max(screenRectangle.Left, Math.Min(screenRectangle.Right - size.Width, pos.X));
                    break;
                default:
                    throw new NotSupportedException();
            }

            //
            angularRectangle = Rectangle.Empty;
            if (angularSize.Width > 0 || angularSize.Height > 0)
            {
                var angularPos = Point.Empty; // 相对与 Pos 的位置
                switch (tipSide)
                {
                    case TipSide.Left:
                        angularPos = new Point(size.Width/* - angularSize.Width*/, targetCenter.Y - pos.Y - angularSize.Height / 2);
                        break;
                    case TipSide.Right:
                        angularPos = new Point(0, targetCenter.Y - pos.Y - angularSize.Height / 2);
                        break;
                    case TipSide.Top:
                        angularPos = new Point(targetCenter.X - pos.X - angularSize.Width / 2, size.Height/* - angularSize.Height*/);
                        break;
                    case TipSide.Bottom:
                        angularPos = new Point(targetCenter.X - pos.X - angularSize.Width / 2, 0);
                        break;
                }
                angularRectangle = new Rectangle(angularPos, angularSize);
            }

            //
            return pos;
        }

        protected virtual Size MeasureSize(string text, out Size textSize, out Size titleSize, out Size controlSize)
        {
            var width = Math.Max(0, MinimumSize.Width);
            var height = Math.Max(0, MinimumSize.Height);

            var font = this.Font ?? SystemFonts.MessageBoxFont;
            textSize = Size.Empty;
            titleSize = Size.Empty;
            if (!string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(Title))
            {
                if (tipWin.IsHandleCreated)
                {
                    using (var grf = tipWin.CreateGraphics())
                    {
                        if (!string.IsNullOrEmpty(text))
                        {
                            if (width > 0)
                                textSize = Size.Ceiling(grf.MeasureString(text, font, width));
                            else
                                textSize = Size.Ceiling(grf.MeasureString(text, font));
                        }

                        if (!string.IsNullOrEmpty(Title))
                            titleSize = Size.Ceiling(grf.MeasureString(Title, font));
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        if (width > 0)
                            textSize = TextRenderer.MeasureText(text, font, new Size(width, int.MaxValue));
                        else
                            textSize = TextRenderer.MeasureText(text, font);
                    }

                    if (!string.IsNullOrEmpty(Title))
                        titleSize = TextRenderer.MeasureText(Title, font);
                }
            }

            width = textSize.Width;
            height = textSize.Height;

            //
            if (NeedShowCaption)
            {
                titleSize.Width += 4;// margin of title
                titleSize.Height = Math.Max(TitleButtonSize, titleSize.Height);
                if (TitleIcon != null)
                {
                    titleSize.Width += TitleIcon.Width;
                    titleSize.Height = Math.Max(TitleIcon.Height, titleSize.Height);
                }
                var captionHeight = CaptionHeight > 0 ? CaptionHeight : titleSize.Height;
                height += captionHeight;
                width = Math.Max(width, titleSize.Width);
            }

            //
            controlSize = Size.Empty;
            if (CustomControl != null)
            {
                controlSize = CustomControl.Size;
                width = Math.Max(width, controlSize.Width);
                height += controlSize.Height;
            }

            width += Padding.Horizontal;
            height += Padding.Vertical;

            //
            width += BorderSize * 2;
            height += BorderSize * 2;
            width = Math.Max(MinimumSize.Width, width);
            height = Math.Max(MinimumSize.Height, height);
            return new Size(width, height);
        }

        protected virtual void LayoutByFixedSize(Size size, out Size sizeText, out Size sizeTitle, out Size sizeControl)
        {
            size.Width -= BorderSize * 2;
            size.Height -= BorderSize * 2;

            var font = this.Font ?? SystemFonts.MessageBoxFont;

            sizeTitle = Size.Empty;
            if (NeedShowCaption)
            {
                if (CaptionHeight > 0)
                {
                    sizeTitle = new Size(size.Width, CaptionHeight);
                }
                else
                {
                    var titleSize = Size.Empty;
                    var width = size.Width;
                    if (TitleIcon != null)
                        width -= TitleIcon.Width;
                    if (!string.IsNullOrEmpty(Title))
                    {
                        if (tipWin.IsHandleCreated)
                        {
                            using (var grf = tipWin.CreateGraphics())
                            {
                                titleSize = Size.Ceiling(grf.MeasureString(Title, font, width));
                            }
                        }
                        else
                        {
                            titleSize = TextRenderer.MeasureText(Title, font, new Size(width, int.MaxValue));
                        }
                    }

                    if(TitleIcon != null)
                        titleSize.Height = Math.Max(TitleIcon.Height, titleSize.Height);
                    if (Buttons.Count > 0)
                        titleSize.Height = Math.Max(titleSize.Height, TitleButtonSize);

                    sizeTitle = new Size(size.Width, titleSize.Height);
                }
            }

            //
            var rect = new Rectangle(Point.Empty, size);
            rect.Y += sizeTitle.Height;
            rect.Height -= sizeTitle.Height;
            rect.X += Padding.Left;
            rect.Y += Padding.Top;
            rect.Width -= Padding.Horizontal;
            rect.Height -= Padding.Vertical;

            if (CustomControl == null || string.IsNullOrEmpty(Text))
            {
                if (!string.IsNullOrEmpty(Text))
                {
                    sizeText = rect.Size;
                    sizeControl = Size.Empty;
                }
                else
                {
                    if (Resizable)
                        rect.Height -= ResizeGripSize;

                    sizeText = Size.Empty;
                    sizeControl = rect.Size;
                }
            }
            else
            {
                if (Resizable)
                    rect.Height -= ResizeGripSize;

                Size textSize;
                if (tipWin.IsHandleCreated)
                {
                    using (var grf = tipWin.CreateGraphics())
                    {
                        textSize = Size.Ceiling(grf.MeasureString(Text, font, size.Width));
                    }
                }
                else
                {
                    textSize = TextRenderer.MeasureText(Text, font, new Size(size.Width, int.MaxValue));
                }

                sizeText = new Size(rect.Width, Math.Min(textSize.Height, rect.Height));
                sizeControl = new Size(rect.Width, rect.Height - sizeText.Height);
            }
        }

        public void Show()
        {
            Show(null, new Rectangle(Control.MousePosition, Size.Empty));
        }

        public void Show(string text)
        {
            Show(text, new Rectangle(Control.MousePosition, Size.Empty));
        }

        public void Show(string text, Point targetPoint)
        {
            Show(text, new Rectangle(targetPoint, Size.Empty));
        }

        public void Show(string text, Rectangle targetRectangle)
        {
            if (string.IsNullOrEmpty(text) && CustomControl == null && !ShowAlways)
            {
                Hide();
                return;
            }

            Text = text;
            TargetRectangle = targetRectangle;
            ExecAction(TipAction.Initial);
        }

        void ShowInternal()
        {
            if (tipWin == null || tipWin.IsDisposed)
            {
                tipWin = new TipForm(this);
                tipWin.Buttons = this.Buttons;
                tipWin.UserFocusedChanged += tipWin_UserFocusedChanged;
                tipWin.Paint += tipWin_Paint;
                tipWin.Layout += tipWin_Layout;

                if (CustomControl != null)
                {
                    tipWin.Controls.Add(CustomControl);
                }

                WindowHasPin = false;
            }
            tipWin.HideWhenClick = !ShowOnUserFocused;
            tipWin.Resizable = this.Resizable && !AutoSize;
            //if (!tipWin.Created)
            //    tipWin.CreateControl();
            if (!tipWin.IsHandleCreated)
                tipWin._CreateHandle();
            var topForm = this.Control != null ? this.Control.TopLevelControl as Form : null;
            tipWin.Owner = topForm;

            LayoutTipWin();

            var hWnd = tipWin.Handle;
            if (!IsWindowVisible(hWnd))
            {
                tipWin.Show();
                SetWindowPos(hWnd, HWND.TopMost, tipWin.Left, tipWin.Top, tipWin.Width, tipWin.Height, SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_SHOWWINDOW);
            }
            else
            {
                SetWindowPos(hWnd, HWND.TopMost, tipWin.Left, tipWin.Top, tipWin.Width, tipWin.Height, SetWindowPosFlags.SWP_NOACTIVATE);
            }
        }

        public void HideForce()
        {
            HideInternal();
        }

        public void Hide()
        {
            if (WindowHasPin)
                return;

            if (ShowOnUserFocused && tipWin != null && !tipWin.IsDisposed && tipWin.Visible && tipWin.UserFocused)
                return;

            ExecAction(TipAction.Hide);
        }

        void HideInternal()
        {
            Text = null;

            if (tipWin != null && IsWindow(tipWin.Handle) && IsWindowVisible(tipWin.Handle))
            {
                ShowWindow(tipWin.Handle, ShowWindowFlags.SW_HIDE);
            }
        }

        void TogglePinStatus()
        {
            WindowHasPin = !WindowHasPin;
            if (tbPin != null)
                tbPin.Image = WindowHasPin ? Properties.Resources.tip_pin_pressed_button : Properties.Resources.tip_pin_button;
        }

        void tipWin_UserFocusedChanged(object sender, EventArgs e)
        {
            if (!ShowOnUserFocused)
                return;

            if (tipWin != null && timer != null)
            {
                timer.Paused = tipWin.UserFocused;
                //System.Diagnostics.Debug.WriteLine(string.Format("timer.Paused:{0}, tipWin.UserFocused:{1}", timer.Paused, tipWin.UserFocused));
            }
        }

        void tipWin_Layout(object sender, LayoutEventArgs e)
        {
            if (CustomControl != null)
            {
                if (CustomControl.Parent != tipWin || !tipWin.Controls.Contains(CustomControl))
                {
                    if (CustomControl.Parent != null)
                        CustomControl.Parent.Controls.Remove(CustomControl);
                    tipWin.Controls.Add(CustomControl);
                }

                if (!CustomControl.Visible)
                    CustomControl.Visible = true;
                //CustomControl.Location = tipWin.ControlBounds.Location;
                CustomControl.Bounds = tipWin.ControlBounds;
            }

            if (Buttons != null && Buttons.Count > 0)
            {
                var rect = tipWin.TitleBounds;
                int space = 2;
                int x = rect.Right - Buttons.Count * (TitleButtonSize + space) - space; // 
                int y = tipWin.TitleBounds.Y + (tipWin.TitleBounds.Height - TitleButtonSize) / 2;
                var buttons = Buttons.OrderBy(b => b.SortIndex).ToArray();
                foreach (var btn in buttons)
                {
                    btn.Bounds = new Rectangle(x, y, TitleButtonSize, TitleButtonSize);
                    x += TitleButtonSize + space;
                }
            }

            //
            if (Resizable)
            {
                var rectGrip = tipWin.ClientRectangle;
                switch (tipWin.SideOfTarget)
                {
                    case TipSide.Left:
                        rectGrip.Width -= tipWin.AngularRectangle.Width;
                        break;
                    case TipSide.Top:
                        rectGrip.Height -= tipWin.AngularRectangle.Height;
                        break;
                }
                tipWin.ResizeGripRectangle = new Rectangle(rectGrip.Right - ResizeGripSize, rectGrip.Bottom - ResizeGripSize, ResizeGripSize, ResizeGripSize);
            }
        }

        void tipWin_Paint(object sender, PaintEventArgs e)
        {
            OnPaint(e);
        }

        protected virtual void OnPaint(PaintEventArgs e)
        {
            var grf = e.Graphics;
            grf.Clear(BackColor);

            var rect = tipWin.ClientRectangle;
            var font = this.Font ?? SystemFonts.MessageBoxFont;

            if (NeedShowCaption)
            {
                var rectCaption = tipWin.TitleBounds;
                rectCaption.Inflate(-2, 0);
                if (TitleIcon != null)
                {
                    e.Graphics.DrawImage(TitleIcon,
                        new Rectangle(rectCaption.Left, rectCaption.Y + (rectCaption.Height - TitleIcon.Height) / 2, TitleIcon.Width, TitleIcon.Height),
                        0, 0, TitleIcon.Width, TitleIcon.Height,
                        GraphicsUnit.Pixel);
                    rectCaption.X += TitleIcon.Width;
                    rectCaption.Width -= TitleIcon.Width;
                }
                if (!string.IsNullOrEmpty(Title))
                {
                    var sfCaption = new StringFormat();
                    sfCaption.Alignment = StringAlignment.Near;
                    sfCaption.LineAlignment = StringAlignment.Center;
                    e.Graphics.DrawString(Title, font, new SolidBrush(TitleColor), rectCaption, sfCaption);
                }

                foreach (var btn in Buttons)
                {
                    PaintButton(e, btn, tipWin.HoverButton == btn, tipWin.PressedButton == btn);
                }
            }

            if (!string.IsNullOrEmpty(Text))
            {
                var sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisCharacter;

                var rectText = tipWin.TextBounds;
                grf.DrawString(Text, font, new SolidBrush(this.ForeColor), rectText, sf);
            }

            //
            if (Resizable)
            {
                PaintResizeHandle(e, tipWin.ResizeGripRectangle);
            }

            //
            Pen penBorder = new Pen(BorderColor);
            GraphicsPath path = null;
            if (tipWin.ShowAngular && tipWin.AngularRectangle.Width > 0 && tipWin.AngularRectangle.Height > 0)
                path = GetAngularPath(rect, tipWin.ShowAngular, tipWin.SideOfTarget, tipWin.AngularRectangle, true);
            if (path == null)
                grf.DrawRectangle(penBorder, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            else
                grf.DrawPath(penBorder, path);
        }

        void PaintButton(PaintEventArgs e, TipButton button, bool isHover, bool isPressed)
        {
            if (button == null)
                return;

            var rect = button.Bounds;
            if (isPressed)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Gainsboro)), rect);
                e.Graphics.DrawRectangle(Pens.SlateGray, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }
            else if (isHover)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Gainsboro)), rect);
                e.Graphics.DrawRectangle(Pens.CadetBlue, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }

            if (button.Image != null)
            {
                var drect = new Rectangle(
                    Math.Max(0, rect.X + (rect.Width - button.Image.Width )/2), 
                    Math.Max(0, rect.Y + (rect.Height - button.Image.Height)/2), 
                    Math.Min(rect.Width, button.Image.Width),
                    Math.Min(rect.Height, button.Image.Height));
                e.Graphics.DrawImage(button.Image, drect, 0, 0, button.Image.Width, button.Image.Height, GraphicsUnit.Pixel);
            }
            //button.OnPaint(e, isHover, isPressed);
        }

        void PaintResizeHandle(PaintEventArgs e, Rectangle rect)
        {
            if (VisualStyleRenderer.IsSupported)
            {
                var r = new VisualStyleRenderer(VisualStyleElement.Status.Gripper.Normal);
                r.DrawBackground(e.Graphics, rect);
            }
            else
            {
                ControlPaint.DrawSizeGrip(e.Graphics, BackColor, rect);
            }
        }

        protected virtual GraphicsPath GetAngularPath(Rectangle rect, bool showAngular, TipSide sideOfTarget, Rectangle angularRectangle, bool forPaint)
        {
            if (!showAngular || angularRectangle.Width <= 0 || angularRectangle.Height <= 0)
                return null;
            if (rect.Width <= 0 || rect.Height <= 0)
                return null;

            var ar = angularRectangle;
            if (forPaint) // 绘制时, 一个像素的偏移
            {
                rect.Width--;
                rect.Height--;
                switch (sideOfTarget)
                {
                    case TipSide.Left:
                        ar.Offset(-1, 0);
                        break;
                    case TipSide.Top:
                        ar.Offset(0, -1);
                        break;
                }
            }

            Point[] pts;
            var adg = new Size(angularRectangle.Width / 2, angularRectangle.Height / 2);
            switch (sideOfTarget)
            {
                case TipSide.Right:
                    rect.X += ar.Width;
                    rect.Width -= ar.Width;

                    pts = new Point[] {
                        new Point(ar.Right, ar.Bottom),
                        new Point(ar.Left+adg.Width, ar.Top + ar.Height/2),
                        new Point(ar.Right, ar.Top),

                        new Point(rect.X, rect.Top),
                        new Point(rect.Right, rect.Top),
                        new Point(rect.Right, rect.Bottom),
                        new Point(rect.Left, rect.Bottom),
                    };
                    break;
                case TipSide.Bottom:
                    rect.Y += ar.Height;
                    rect.Height -= ar.Height;
                    pts = new Point[] {
                        new Point(ar.Left, ar.Bottom),
                        new Point(ar.Left + ar.Width /2, ar.Top+adg.Height),
                        new Point(ar.Right, ar.Bottom),
                                                    
                        new Point(rect.Right, rect.Top),
                        new Point(rect.Right, rect.Bottom),
                        new Point(rect.Left, rect.Bottom),
                        new Point(rect.Left, rect.Top)
                    };
                    break;
                case TipSide.Left:
                    rect.Width -= ar.Width;
                    pts = new Point[] {
                        new Point(rect.Right, ar.Top),
                        new Point(ar.Right-adg.Width+1, ar.Top + ar.Height/2),
                        new Point(rect.Right, ar.Bottom),

                        new Point(rect.Right, rect.Bottom),
                        new Point(rect.Left, rect.Bottom),
                        new Point(rect.Left, rect.Top),
                        new Point(rect.Right, rect.Top)
                    };
                    break;
                case TipSide.Top:
                    rect.Height -= ar.Height;
                    pts = new Point[] {
                        new Point(ar.Right, rect.Bottom),
                        new Point(ar.Left + ar.Width /2, ar.Bottom-adg.Height+1),
                        new Point(ar.Left, rect.Bottom),

                        new Point(rect.Left, rect.Bottom),
                        new Point(rect.Left, rect.Top),
                        new Point(rect.Right, rect.Top),
                        new Point(rect.Right, rect.Bottom)
                    };
                    break;
                default:
                    return null;
            }

            var gp = new GraphicsPath();
            gp.AddLines(pts);
            gp.CloseFigure();
            return gp;
        }
        
        protected virtual void ResizeTipWin(Size size)
        {
            if (tipWin == null || tipWin.IsDisposed)
                return;

            //SetWindowPos(tipWin.Handle, HWND.None, 0, 0, size.Width, size.Height,
            //    SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOZORDER);
            switch (tipWin.SideOfTarget)
            {
                case TipSide.Left:
                case TipSide.Right:
                    size.Width -= tipWin.AngularRectangle.Width;
                    break;
                case TipSide.Top:
                case TipSide.Bottom:
                    size.Height -= tipWin.AngularRectangle.Height;
                    break;
            }

            this._Size = size;
            LayoutTipWin();
            tipWin.Invalidate();
        }

        public enum TipSide
        {
            Left,
            Top,
            Bottom,
            Right,
        }

        #region TipAction
        enum TipAction
        {
            Initial,
            Show,
            Hide,
        }
        #endregion

        #region Timer
        class TipTimer : Timer
        {
            bool _Paused;
            bool LastEnabled;

            public TipTimer()
            {
            }

            public bool Paused
            {
                get { return _Paused; }
                set
                {
                    if (_Paused != value)
                    {
                        _Paused = value;
                        OnPausedChanged();
                    }
                }
            }

            void OnPausedChanged()
            {
                if (Paused)
                {
                    if (Enabled)
                    {
                        LastEnabled = true;
                        Enabled = false;
                    }
                }
                else
                {
                    if (LastEnabled)
                    {
                        Enabled = true;
                        LastEnabled = false;
                    }
                }
            }

            protected override void OnTick(EventArgs e)
            {
                base.OnTick(e);

                Stop();
                LastEnabled = false;
            }
        }
        #endregion

        #region Tip Form
        public class TipButton
        {
            public event EventHandler Click;

            public TipButton()
            {
            }

            public Rectangle Bounds { get; set; }

            public bool Visible { get; set; }

            public Image Image { get; set; }

            public int SortIndex { get; set; }

            internal void OnClick()
            {
                if (Click != null)
                    Click(this, EventArgs.Empty);
            }
        }

        public class TipButtonEventArgs : EventArgs
        {
            public TipButtonEventArgs(TipButton button)
            {
                Button = button;
            }

            public TipButton Button { get; private set; }
        }

        public delegate void TipButtonEventHandler(object sender, TipButtonEventArgs e);

        class TipForm : Form, IMessageFilter
        {
            public const int CloseButtonID = 0x9999;

            Tip OwnerTip;
            bool _UserFocused;
            TipButton _HoverButton;
            TipButton _PressedButton;
            bool HoldResizeGrip;
            Point MouseDownPos;
            Size MouseDownSize;
            Rectangle? lastReversibleFrame;

            public event EventHandler UserFocusedChanged;
            public event TipButtonEventHandler ButtonClick;

            public TipForm(Tip ownerTip)
            {
                this.OwnerTip = ownerTip;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                this.ShowInTaskbar = false;
                this.StartPosition = FormStartPosition.Manual;
                //this.TopMost = true;
                this.Font = SystemFonts.MessageBoxFont;
                this.BackColor = SystemColors.Info;
                this.ForeColor = SystemColors.InfoText;

                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            }

            public bool UserFocused
            {
                get
                {
                    return _UserFocused;
                }

                set
                {
                    if (_UserFocused != value)
                    {
                        _UserFocused = value;
                        OnUserFocusedChanged();
                    }
                }
            }

            public bool HideWhenClick { get; set; }

            public Rectangle TextBounds { get; set; }

            public Rectangle TitleBounds { get; set; }

            public Rectangle ControlBounds { get; set; }

            public TipButton HoverButton
            {
                get { return _HoverButton; }
                private set 
                {
                    if (_HoverButton != value)
                    {
                        var old = _HoverButton;
                        _HoverButton = value;
                        OnHoverButtonChanged(old);
                    }
                }
            }

            public TipButton PressedButton
            {
                get { return _PressedButton; }
                private set 
                {
                    if (_PressedButton != value)
                    {
                        var old = _PressedButton;
                        _PressedButton = value;
                        OnPressedButtonChanged(old);
                    }
                }
            }

            public TipSide SideOfTarget { get; set; }

            public bool ShowAngular { get; set; }

            public IEnumerable<TipButton> Buttons { get; set; }

            public Rectangle AngularRectangle { get; set; }

            public bool Resizable { get; set; }

            public Rectangle ResizeGripRectangle { get; set; }

            void OnUserFocusedChanged()
            {
                if (UserFocusedChanged != null)
                {
                    UserFocusedChanged(this, EventArgs.Empty);
                }
            }

            bool TestUserFocus()
            {
                if (this.IsDisposed || !IsWindow(Handle) || !IsWindowVisible(Handle))
                    return false;

                return ClientRectangle.Contains(PointToClient(Control.MousePosition))
                    || this.Focused
                    || this.ContainsFocus;
            }

            protected override void OnLoad(EventArgs e)
            {
                base.OnLoad(e);

                if (!DesignMode)
                {
                    Application.AddMessageFilter(this);
                }
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                Application.RemoveMessageFilter(this);
            }

            protected override void OnClosed(EventArgs e)
            {
                base.OnClosed(e);

                Application.RemoveMessageFilter(this);
            }

            protected override void OnMouseHover(EventArgs e)
            {
                base.OnMouseHover(e);

                UserFocused = TestUserFocus();
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);

                UserFocused = TestUserFocus();
                HoverButton = null;
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);

                HoverButton = ButtonHitTest(e.X, e.Y);
                UserFocused = TestUserFocus();

                if (Resizable && (HoldResizeGrip || ResizeGripRectangle.Contains(e.X, e.Y)))
                {
                    if (HoldResizeGrip)
                    {
                        var size = new Size(
                            Math.Max(MouseDownSize.Width + (e.X - MouseDownPos.X), MinimumSize.Width),
                            Math.Max(MouseDownSize.Height + (e.Y - MouseDownPos.Y), MinimumSize.Height));
                        var rect = new Rectangle(this.Left, this.Top, size.Width, size.Height);
                        DrawReversibleFrame(rect);
                    }
                    Cursor = Cursors.SizeNWSE;
                }
                else
                {
                    Cursor = Cursors.Default;
                }
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);

                PressedButton = ButtonHitTest(e.X, e.Y);
                MouseDownPos = new Point(e.X, e.Y);
                MouseDownSize = Size;
                if (Resizable && ResizeGripRectangle.Contains(e.X, e.Y))
                {
                    HoldResizeGrip = true;
                    Capture = true;
                }
                else
                {
                    HoldResizeGrip = false;
                }
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);

                if (HoldResizeGrip)
                {
                    DrawReversibleFrame(null);

                    var size = new Size(
                        Math.Max(MouseDownSize.Width + (e.X - MouseDownPos.X), MinimumSize.Width),
                        Math.Max(MouseDownSize.Height + (e.Y - MouseDownPos.Y), MinimumSize.Height));
                    if (OwnerTip != null)
                        OwnerTip.ResizeTipWin(size);
                }
                else
                    if (ButtonHitTest(e.X, e.Y) == PressedButton && e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        var btn = PressedButton;
                        PressedButton = null;
                        OnButtonClick(btn);
                    }

                PressedButton = null;
                HoldResizeGrip = false;
                Capture = false;
            }

            void DrawReversibleFrame(Rectangle? rectangle)
            {
                if (lastReversibleFrame.HasValue)
                    ControlPaint.DrawReversibleFrame(lastReversibleFrame.Value, this.BackColor, FrameStyle.Dashed);

                if (rectangle.HasValue)
                    ControlPaint.DrawReversibleFrame(rectangle.Value, this.BackColor, FrameStyle.Dashed);

                lastReversibleFrame = rectangle;
            }

            TipButton ButtonHitTest(int x, int y)
            {
                if (this.Buttons == null)
                    return null;

                foreach (var btn in this.Buttons)
                    if (btn.Bounds.Contains(x, y))
                        return btn;

                return null;
            }

            void OnButtonClick(TipButton button)
            {
                if (ButtonClick != null)
                {
                    ButtonClick(this, new TipButtonEventArgs(button));
                }

                if (button != null)
                    button.OnClick();
            }

            protected override void OnVisibleChanged(EventArgs e)
            {
                base.OnVisibleChanged(e);

                UserFocused = TestUserFocus();
            }

            void OnHoverButtonChanged(TipButton old)
            {
                if (old != null)
                    Invalidate(old.Bounds);

                if (HoverButton != null)
                    Invalidate(HoverButton.Bounds);
            }

            void OnPressedButtonChanged(TipButton old)
            {
                if (old != null)
                    Invalidate(old.Bounds);

                if (PressedButton != null)
                    Invalidate(PressedButton.Bounds);
            }

            protected override void WndProc(ref Message m)
            {
                if (HideWhenClick)
                {
                    switch (m.Msg)
                    {
                        case WM_LBUTTONDOWN:
                        case WM_MBUTTONDOWN:
                        case WM_RBUTTONDOWN:
                            ShowWindow(this.Handle, ShowWindowFlags.SW_HIDE);
                            m.Result = IntPtr.Zero;
                            return;
                    }
                }

                base.WndProc(ref m);

                switch (m.Msg)
                {
                    case WM_SHOWWINDOW:
                        if (m.WParam.ToInt32() == 0)
                            UserFocused = TestUserFocus();
                        break;
                }
            }

            protected override bool ShowWithoutActivation
            {
                get
                {
                    return true;
                }
            }

            public void TryShow(IWin32Window ownerControl)
            {
                if (!this.Created || !this.Visible)
                    this.Show(ownerControl);
                this.Invalidate();
            }

            public void TryHide()
            {
                if (this.Visible)
                    this.Hide();
            }

            public void _CreateHandle()
            {
                this.CreateHandle();
                this.CreateControl();
                this.CreateControlsInstance();
            }

            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg == WM_MOUSEMOVE && m.HWnd != this.Handle 
                    && this.OwnerTip != null && this.OwnerTip.ShowOnUserFocused)
                {
                    if (!this.IsDisposed && IsWindowVisible(this.Handle))
                    {
                        UserFocused = TestUserFocus();
                        //System.Diagnostics.Debug.WriteLine(string.Format("UserFocused:{0}", UserFocused));
                    }
                }

                return false;
            }
        }
        #endregion

        #region win apis
        const int WM_SHOWWINDOW     = 0x0018;
        const int WM_LBUTTONDOWN    = 0x0201;
        const int WM_RBUTTONDOWN    = 0x0204;
        const int WM_MBUTTONDOWN    = 0x0207;
        const int WM_MOUSEMOVE      = 0x0200;

        enum HWND
        {
            None = 0,
            NoTopMost = -2,
            TopMost = -1,
            Top = 0,
            Bottom = 1
        }

        enum ShowWindowFlags
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11,
        }

        enum SetWindowPosFlags : uint
        {
            SWP_NOSIZE = 0x0001,
            SWP_NOMOVE = 0x0002,
            SWP_NOZORDER = 0x0004,
            SWP_NOREDRAW = 0x0008,
            SWP_NOACTIVATE = 0x0010,
            SWP_FRAMECHANGED = 0x0020,  /* The frame changed: send WM_NCCALCSIZE */
            SWP_SHOWWINDOW = 0x0040,
            SWP_HIDEWINDOW = 0x0080,
            SWP_NOCOPYBITS = 0x0100,
            SWP_NOOWNERZORDER = 0x0200,  /* Don't do owner Z ordering */
            SWP_NOSENDCHANGING = 0x0400,  /* Don't send WM_WINDOWPOSCHANGING */
            SWP_DRAWFRAME = SWP_FRAMECHANGED,
            SWP_NOREPOSITION = SWP_NOOWNERZORDER,
            SWP_DEFERERASE = 0x2000,
            SWP_ASYNCWINDOWPOS = 0x4000,
        }

        [DllImport("User32")]
        static extern int ShowWindow(IntPtr hWnd, ShowWindowFlags nCmdShow);

        [DllImport("User32")]
        static extern int SetWindowPos(IntPtr hWnd, HWND hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("User32")]
        static extern bool IsWindow(IntPtr hWnd);

        [DllImport("User32")]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("User32")]
        static extern bool IsWindowEnabled(IntPtr hWnd);
        #endregion
    }
}
