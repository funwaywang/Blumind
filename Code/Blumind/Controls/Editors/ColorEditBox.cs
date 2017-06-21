using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Controls
{
    [DefaultEvent("ColorChanged")]
    class ColorEditBox : ResizableTextBox
    {
        private TextBoxExDropDownButton DropDownButton;
        private ColorIconItem ColorIcon;

        //private string _Text = string.Empty;
        private Color? _ColorValue;
        private ColorPicker Picker;
        private List<Color> _CustomColors = new List<Color>();
        private bool _UseCustomColors = false;
        private bool _ShowDefaultColorButton = true;
        private bool _ShowMoreColorButton = true;
        private Color _DefaultColor = Color.Black;
        private string _DefaultColorText = "Default";
        private string _MoreColorText = "More...";
        static ColorDialog ShareColorDialog = null;
        private static List<Color> _CommonColors = new List<Color>();

        public event System.EventHandler ColorValueChanged;

        static ColorEditBox()
        {
            _CommonColors.AddRange(
                new Color[]{
                    Color.FromArgb(192, 0, 0), 
                    Color.FromArgb(255, 0, 0), 
                    Color.FromArgb(255, 192, 0), 
                    Color.FromArgb(255, 255, 0), 
                    Color.FromArgb(146, 208, 80), 
                    Color.FromArgb(0, 176, 80), 
                    Color.FromArgb(0, 176, 240), 
                    Color.FromArgb(0, 112, 192), 
                    Color.FromArgb(0, 32, 96), 
                    Color.FromArgb(112, 48, 160),
                    Color.FromArgb(255, 255, 255),

                    Color.FromArgb(0, 0, 0),
                    Color.FromArgb(238, 236, 225),
                    Color.FromArgb(31, 73, 125),
                    Color.FromArgb(79, 129, 189),
                    Color.FromArgb(192, 80, 77),
                    Color.FromArgb(155, 187, 89),
                    Color.FromArgb(128, 100, 162),
                    Color.FromArgb(255, 255, 255),
                    Color.FromArgb(247, 150, 70),

                    Color.FromArgb(242, 242, 242),
                    Color.FromArgb(127, 127, 127),
                    Color.FromArgb(221, 217, 195),
                    Color.FromArgb(198, 217, 240),
                    Color.FromArgb(219, 229, 241),
                    Color.FromArgb(242, 220, 219),
                    Color.FromArgb(235, 241, 221),
                    Color.FromArgb(229, 224, 236),
                    Color.FromArgb(219, 238, 243),
                    Color.FromArgb(253, 234, 218),

                    Color.FromArgb(216, 216, 216),
                    Color.FromArgb(89, 89, 89),
                    Color.FromArgb(196, 189, 151),
                    Color.FromArgb(141, 179, 226),
                    Color.FromArgb(184, 204, 228),
                    Color.FromArgb(229, 185, 183),
                    Color.FromArgb(215, 227, 188),
                    Color.FromArgb(204, 193, 217),
                    Color.FromArgb(183, 221, 232),
                    Color.FromArgb(251, 213, 181),

                    Color.FromArgb(191, 191, 191),
                    Color.FromArgb(63, 63, 63),
                    Color.FromArgb(147, 137, 83),
                    Color.FromArgb(84, 141, 212),
                    Color.FromArgb(149, 179, 215),
                    Color.FromArgb(217, 150, 148),
                    Color.FromArgb(195, 214, 155),
                    Color.FromArgb(178, 162, 199),
                    Color.FromArgb(146, 205, 220),
                    Color.FromArgb(250, 192, 143),

                    Color.FromArgb(165, 165, 165),
                    Color.FromArgb(38, 38, 38),
                    Color.FromArgb(73, 68, 41),
                    Color.FromArgb(23, 54, 93),
                    Color.FromArgb(54, 96, 146),
                    Color.FromArgb(149, 55, 52),
                    Color.FromArgb(118, 146, 60),
                    Color.FromArgb(95, 73, 122),
                    Color.FromArgb(49, 133, 155),
                    Color.FromArgb(227, 108, 9),

                    Color.FromArgb(127, 127, 127),
                    Color.FromArgb(12, 12, 12),
                    Color.FromArgb(29, 27, 16),
                    Color.FromArgb(15, 36, 62),
                    Color.FromArgb(36, 64, 97),
                    Color.FromArgb(99, 36, 35),
                    Color.FromArgb(79, 97, 40),
                    Color.FromArgb(63, 49, 81),
                    Color.FromArgb(32, 88, 103),
                    Color.FromArgb(151, 72, 6),

                    SystemColors.Window,
                    SystemColors.WindowText,
                    SystemColors.Control,
                    SystemColors.ControlText,
                    SystemColors.Highlight,
                    SystemColors.HighlightText,
                    SystemColors.ActiveCaption,
                    SystemColors.ActiveCaptionText,
                    SystemColors.InactiveCaption,
                    SystemColors.InactiveCaptionText
                }
            );
        }

        public ColorEditBox()
        {
            base.Text = string.Empty;
            InnerTextBox.TextAlign = HorizontalAlignment.Center;

            ColorIcon = new ColorIconItem();
            ColorIcon.BackColor = ColorValue;
            ColorIcon.RespondMouse = false;

            DropDownButton = new TextBoxExDropDownButton();
            DropDownButton.Click += new EventHandler(DropDownButton_Click);

            Items.Add(ColorIcon);
            Items.Add(DropDownButton);

            DefaultColorText = Lang._("Default");
            MoreColorText = Lang.GetTextWithEllipsis("More");
        }

        //public new string Text
        //{
        //    get { return _Text; }
        //    set { _Text = value; }
        //}

        [DefaultValue(null)]
        public Color? ColorValue
        {
            get { return _ColorValue; }
            set
            {
                if (_ColorValue != value)
                {
                    _ColorValue = value;
                    OnColorValueChanged();
                }
            }
        }

        [Browsable(false)]
        public List<Color> CustomColors
        {
            get { return _CustomColors; }
        }

        [DefaultValue(false)]
        public bool UseCustomColors
        {
            get { return _UseCustomColors; }
            set { _UseCustomColors = value; }
        }

        [DefaultValue(true)]
        public bool ShowDefaultColorButton
        {
            get { return _ShowDefaultColorButton; }
            set { _ShowDefaultColorButton = value; }
        }

        [DefaultValue(true)]
        public bool ShowMoreColorButton
        {
            get { return _ShowMoreColorButton; }
            set { _ShowMoreColorButton = value; }
        }

        [DefaultValue(typeof(Color), "Black")]
        public Color DefaultColor
        {
            get { return _DefaultColor; }
            set { _DefaultColor = value; }
        }

        [DefaultValue("Default")]
        private string DefaultColorText
        {
            get { return _DefaultColorText; }
            set { _DefaultColorText = value; }
        }

        [DefaultValue("More...")]
        private string MoreColorText
        {
            get { return _MoreColorText; }
            set { _MoreColorText = value; }
        }

        public static List<Color> CommonColors
        {
            get { return ColorEditBox._CommonColors; }
        }

        private void OnColorValueChanged()
        {
            ColorIcon.BackColor = ColorValue;
            if (ColorValue.HasValue)
                Text = ST.ToString(ColorValue.Value);
            else
                Text = string.Empty;
            //this.Invalidate();

            if (ColorValueChanged != null)
            {
                ColorValueChanged(this, EventArgs.Empty);
            }
        }

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    const int ArrowSize = 5;

        //    base.OnPaint(e);

        //    Graphics grf = e.Graphics;
        //    Rectangle rect = this.ClientRectangle;

        //    rect.Inflate(-5, -5);
        //    rect.Width--;
        //    rect.Height--;
        //    rect.Width -= ArrowSize + 6;
        //    grf.FillRectangle(new SolidBrush(this.Color), rect);
        //    grf.DrawRectangle(Pens.Black, rect);

        //    Rectangle rectArrow = new Rectangle(rect.Right + 5, rect.Y + (rect.Height - ArrowSize) / 2 + 2, ArrowSize, ArrowSize * 3 / 4);
        //    grf.DrawLine(SystemPens.ControlDark, rectArrow.X - 3, rect.Y + 2, rectArrow.X - 3, rect.Bottom - 2);
        //    grf.FillPolygon(SystemBrushes.ControlText, new Point[] { 
        //        new Point(rectArrow.X,rectArrow.Y),
        //        new Point(rectArrow.Right, rectArrow.Y),
        //        new Point(rectArrow.X+rectArrow.Width/2, rectArrow.Bottom)
        //        });
        //}

        private void DropDownButton_Click(object sender, EventArgs e)
        {
            DropDownPicker();
        }

        //protected override void OnClick(EventArgs e)
        //{
        //    base.OnClick(e);

        //}

        private void DropDownPicker()
        {
            if (this.Picker != null && this.Picker.IsDroped)
            {
                this.Picker.EndDropDown();
            }
            else
            {
                if (Picker == null)
                {
                    Picker = new ColorPicker();
                    Picker.ColorSelected += new EventHandler(Picker_ColorSelected);
                    Picker.NeedColorDialog += new EventHandler(Picker_NeedColorDialog);
                }

                Picker.Colors = this.UseCustomColors ? this.CustomColors : CommonColors;
                Picker.ShowDefaultColorButton = this.ShowDefaultColorButton;
                Picker.ShowMoreColorButton = this.ShowMoreColorButton;
                Picker.DefaultColor = this.DefaultColor;
                Picker.DefaultColorText = this.DefaultColorText;
                Picker.MoreColorText = this.MoreColorText;

                Picker.DropDown(this, DropDownDirection.Below);//, new Point(Math.Max(0, Width - Picker.Width), Height));
            }
        }

        private void ShowColorDialog()
        {
            if (ShareColorDialog == null)
            {
                ShareColorDialog = new ColorDialog();
                ShareColorDialog.FullOpen = true;
            }

            if(ColorValue.HasValue)
                ShareColorDialog.Color = ColorValue.Value;
            if (ShareColorDialog.ShowDialog(this) == DialogResult.OK)
            {
                ColorValue = ShareColorDialog.Color;
            }

            /*ToolStripButton buttonRed;
            ToolStripButton buttonBlue;
            ToolStripButton buttonYellow; 
            
            buttonRed = new ToolStripButton();
            buttonRed.ForeColor = Color.Red;
            buttonRed.Text = "A";

            buttonBlue = new ToolStripButton();
            buttonBlue.ForeColor = Color.Blue;
            buttonBlue.Text = "A";

            buttonYellow = new ToolStripButton();
            buttonYellow.ForeColor = Color.Yellow;
            buttonYellow.Text = "A";


            ToolStripDropDown tsdd = new ToolStripDropDown();
            tsdd.Items.Add(buttonRed);
            tsdd.Items.Add(buttonBlue);
            tsdd.Items.Add(buttonYellow);
            tsdd.Show(this, new Point(0, 30));*/
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Escape && Picker != null && Picker.IsDroped)
            {
                Picker.EndDropDown();
            }
            else if (e.KeyCode == Keys.Enter && (Picker == null || !Picker.IsDroped))
            {
                ColorValue = ST.GetColor(Text);
                e.SuppressKeyPress = true;
            }
        }

        private void Picker_NeedColorDialog(object sender, EventArgs e)
        {
            this.ShowColorDialog();
        }

        private void Picker_ColorSelected(object sender, EventArgs e)
        {
            ColorPicker picker = sender as ColorPicker;
            if (picker != null)
                this.ColorValue = picker.SelectedColor;
        }

        internal class ColorPicker : DropDownFrame
        {
            const int CellSize = 20;
            const int ButtonHeight = 23;
            const int DefaultColorButtonIndex = int.MaxValue - 1;
            const int MoreColorButtonIndex = int.MaxValue - 2;
            const int Columns = 10;
            const int CellPadding = 1;

            private int _MouseOverIndex = -1;
            public List<Color> Colors = null;
            public bool ShowDefaultColorButton = true;
            public bool ShowMoreColorButton = true;
            public Color DefaultColor = Color.Black;
            public string DefaultColorText = null;
            public string MoreColorText = null;
            private Color _SelectedColor = Color.Empty;

            public event System.EventHandler ColorSelected;
            public event System.EventHandler NeedColorDialog;

            public ColorPicker()
            {
                BackColor = SystemColors.Window;//.Menu;
                ForeColor = SystemColors.WindowText;//.MenuText;

                SetStyle(ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.UserPaint |
                    ControlStyles.OptimizedDoubleBuffer, true);
            }

            private int MouseOverIndex
            {
                get { return _MouseOverIndex; }
                set
                {
                    if (_MouseOverIndex != value)
                    {
                        int old = _MouseOverIndex;
                        _MouseOverIndex = value;
                        OnMouseOverIndexChanged(old, value);
                    }
                }
            }

            public Color SelectedColor
            {
                get { return _SelectedColor; }
                set
                {
                    _SelectedColor = value;
                    if (ColorSelected != null)
                        this.ColorSelected(this, EventArgs.Empty);
                }
            }

            private void OnMouseOverIndexChanged(int oi, int ni)
            {
                this.Invalidate();
                if (oi > -1)
                    this.Invalidate(GetItemBounds(oi));

                if (ni > -1)
                    this.Invalidate(GetItemBounds(ni));
            }

            protected override void CalculateSize()
            {
                int w = CellSize * Columns;
                int h = CellSize;
                if (this.Colors != null && this.Colors.Count > 0)
                {
                    h = this.Colors.Count / Columns * CellSize;
                    if (this.Colors.Count % Columns > 0)
                        h += CellSize;
                }

                if (this.ShowDefaultColorButton)
                    h += ButtonHeight;
                if (this.ShowMoreColorButton)
                    h += ButtonHeight;

                this.Size = new Size(w + this.Padding.Horizontal, h + this.Padding.Vertical);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                DrawBackground(e);

                Graphics grf = e.Graphics;
                Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
                rect.X += this.Padding.Left;
                rect.Y += this.Padding.Top;
                rect.Width -= this.Padding.Horizontal;
                rect.Height -= this.Padding.Vertical;

                // draw default button
                if (this.ShowDefaultColorButton)
                    this.DrawDefaultButton(grf, ref rect);

                if (this.ShowMoreColorButton)
                    this.DrawMoreButton(grf, ref rect);

                if (this.Colors == null || this.Colors.Count == 0)
                    return;

                Rectangle rectcell = new Rectangle(rect.X, rect.Y, CellSize, CellSize);
                for (int i = 0; i < this.Colors.Count; i++)
                {
                    this.DrawColorCell(grf, i, Colors[i], rectcell);

                    rectcell.X += CellSize;
                    if (rectcell.Right > rect.Right)
                    {
                        rectcell.X = rect.X;
                        rectcell.Y += CellSize;
                    }

                    if (rect.Bottom > rect.Bottom)
                        break;
                }
            }

            protected override void DrawBackground(PaintEventArgs e)
            {
                Rectangle rect = ClientRectangle;

                e.Graphics.FillRectangle(new SolidBrush(BackColor), rect);
                ControlPaint.DrawVisualStyleBorder(e.Graphics, new Rectangle(rect.X, rect.Y, rect.Width - 1, rect.Height - 1));
            }

            private void DrawColorCell(Graphics grf, int index, Color color, Rectangle rectcell)
            {
                if (index == this.MouseOverIndex)
                    grf.FillRectangle(SystemBrushes.Highlight, rectcell);

                Rectangle rect = rectcell;
                rect.Inflate(-CellPadding, -CellPadding);
                grf.FillRectangle(new SolidBrush(color), rect);
                grf.DrawRectangle(SystemPens.ControlDark, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }

            private void DrawMoreButton(Graphics grf, ref Rectangle rect)
            {
                Rectangle rectButton = rect;
                rectButton.Y = rect.Bottom - ButtonHeight;
                rectButton.Height = ButtonHeight;
                rectButton.Inflate(-2, -2);

                Brush forebrush = null;
                if (this.MouseOverIndex == MoreColorButtonIndex)
                {
                    grf.FillRectangle(SystemBrushes.Highlight, rectButton);
                    forebrush = SystemBrushes.HighlightText;
                    grf.DrawRectangle(SystemPens.MenuHighlight, rectButton);
                }
                else
                {
                    forebrush = new SolidBrush(this.ForeColor);
                }
                if (!string.IsNullOrEmpty(this.MoreColorText))
                {
                    StringFormat sf = PaintHelper.SFCenter;
                    sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Hide;
                    grf.DrawString(this.MoreColorText, SystemInformation.MenuFont, forebrush, rectButton, sf);
                }

                rect.Height -= ButtonHeight;
            }

            private void DrawDefaultButton(Graphics grf, ref Rectangle rect)
            {
                Rectangle rectButton = rect;
                rectButton.Height = ButtonHeight;
                rectButton.Inflate(-2, -2);

                Brush forebrush = null;
                if (this.MouseOverIndex == DefaultColorButtonIndex)
                {
                    grf.FillRectangle(SystemBrushes.Highlight, rectButton);
                    forebrush = SystemBrushes.HighlightText;
                }
                else
                {
                    forebrush = new SolidBrush(this.ForeColor);
                }
                grf.DrawRectangle(SystemPens.MenuHighlight, rectButton);
                if (!string.IsNullOrEmpty(this.DefaultColorText))
                {
                    StringFormat sf = PaintHelper.SFCenter;
                    sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Hide;
                    grf.DrawString(this.DefaultColorText, SystemInformation.MenuFont, forebrush, rectButton, sf);
                }

                Rectangle rectcell = rectButton;
                rectcell.Y += (rectButton.Height - CellSize) / 2;
                rectcell.Height = CellSize;
                rectcell.Width = CellSize;
                this.DrawColorCell(grf, DefaultColorButtonIndex, this.DefaultColor, rectcell);

                rect.Y += ButtonHeight;
                rect.Height -= ButtonHeight;
            }

            //protected override void OnWndProc(ref Message m)
            //{
            //    switch (m.Msg)
            //    {
            //        case WinMessages.WM_LBUTTONUP:
            //            OnLButtonDown(GetPointFromInt(m.LParam.ToInt32()));
            //            m.Result = IntPtr.Zero;
            //            break;
            //        default:
            //            base.OnWndProc(ref m);
            //            break;
            //    }
            //}

            protected override void OnMouseDown(MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    OnLButtonDown(new Point(e.X, e.Y));
                }
                else
                {
                    base.OnMouseDown(e);
                }
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);

                this.MouseOverIndex = this.HitTest(e.X, e.Y);
            }

            private void OnLButtonDown(Point pt)
            {
                int index = this.HitTest(pt.X, pt.Y);
                if (index > -1)
                {
                    if (index == DefaultColorButtonIndex)
                        this.SelectedColor = this.DefaultColor;
                    else if (this.Colors != null && index < this.Colors.Count)
                        this.SelectedColor = this.Colors[index];
                }

                this.EndDropDown();

                if (index == MoreColorButtonIndex)
                    this.ShowMoreColorDialog();
            }

            private void ShowMoreColorDialog()
            {
                if (this.NeedColorDialog != null)
                    this.NeedColorDialog(this, EventArgs.Empty);
            }

            private void OnColorSelected(Color color)
            {
                if (ColorSelected != null)
                    ColorSelected(this, EventArgs.Empty);
            }

            private int HitTest(int x, int y)
            {
                Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
                rect.X += this.Padding.Left;
                rect.Y += this.Padding.Top;
                rect.Width -= this.Padding.Horizontal;
                rect.Height -= this.Padding.Vertical;
                if (!rect.Contains(x, y))
                    return -1;

                if (this.ShowDefaultColorButton && y < ButtonHeight)
                    return DefaultColorButtonIndex;

                if (this.ShowMoreColorButton && y > rect.Bottom - ButtonHeight)
                    return MoreColorButtonIndex;

                Rectangle rectcell = new Rectangle(rect.X, rect.Y, CellSize, CellSize);
                if (this.ShowDefaultColorButton)
                    rectcell.Y += ButtonHeight;

                for (int i = 0; i < this.Colors.Count; i++)
                {
                    if (rectcell.Contains(x, y))
                        return i;

                    rectcell.X += CellSize;
                    if (rectcell.Right > rect.Right)
                    {
                        rectcell.X = rect.X;
                        rectcell.Y += CellSize;
                    }

                    if (rect.Bottom > rect.Bottom)
                        break;
                }

                return -1;
            }

            private Rectangle GetItemBounds(int oi)
            {
                return Rectangle.Empty;
            }
        }

        class ColorIconItem : TextBoxExItem
        {
            public ColorIconItem()
                : base(null, ItemAlignment.Left)
            {                
            }

            public override void OnPaint(PaintEventArgs e, Color backColor, Color foreColor, Font font, bool hover, bool pressed)
            {
                //base.OnPaint(e, backColor, foreColor, font, hover, pressed);
                Rectangle rect = Bounds;
                rect.Inflate(-1, -1);

                if (BackColor.HasValue)
                {
                    e.Graphics.FillRectangle(new SolidBrush(BackColor.Value), rect);
                }
            }
        }
    }
}
