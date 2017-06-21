using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Blumind.Controls.OS;

namespace Blumind.Controls
{
    class DropDownFrame : BaseForm
    {
        private bool _IsDroped = false;
        private bool _AllowActive = false;
        private bool _AllowResize = false;
        private Rectangle ResizeGripBounds = Rectangle.Empty;

        public DropDownFrame()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;

            this.CreateHandle();

            this.ResizeRedraw = true;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                this.DestroyHandle();
            }
        }

        protected override Padding DefaultPadding
        {
            get
            {
                return new Padding(2);
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new System.Drawing.Size(100, 100);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                if (System.Environment.OSVersion.Version > new Version(5, 1, 0, 0))
                {
                    const int CS_DROPSHADOW = 0x00020000;
                    CreateParams cp = base.CreateParams;
                    cp.ClassStyle |= CS_DROPSHADOW;
                    return cp;
                }
                else
                {
                    return base.CreateParams;
                }
            }
        }

        [Browsable(false)]
        public bool IsDroped
        {
            get { return _IsDroped; }
            private set
            {
                //System.Diagnostics.Debug.WriteLine(string.Format("IsDroped:{0}", value));
                _IsDroped = value;
            }
        }

        [DefaultValue(false), Browsable(false)]
        protected bool AllowActive
        {
            get { return _AllowActive; }
            set { _AllowActive = value; }
        }

        [DefaultValue(false), Browsable(false)]
        protected bool AllowResize
        {
            get { return _AllowResize; }
            set
            {
                if (_AllowResize != value)
                {
                    _AllowResize = value;
                    this.OnAllowResizeChanged();
                }
            }
        }

        private void OnAllowResizeChanged()
        {
            this.Invalidate();
        }

        public void DropDown(Control control, DropDownDirection direction)
        {
            if (control != null)
            {
                Point pt;
                switch (direction)
                {
                    case DropDownDirection.Left:
                        pt = new Point(-this.Width, 0);
                        break;
                    case DropDownDirection.Right:
                        pt = new Point(control.Width, 0);
                        break;
                    case DropDownDirection.AboveRight:
                        pt = new Point(control.Width, -this.Height);
                        break;
                    case DropDownDirection.AboveLeft:
                        pt = new Point(-this.Width, -this.Height);
                        break;
                    case DropDownDirection.BelowRight:
                        pt = new Point(control.Width - this.Width, control.Height);
                        break;
                    case DropDownDirection.BelowLeft:
                        pt = new Point(-this.Width, control.Height);
                        break;
                    case DropDownDirection.Above:
                        pt = new Point(0, -this.Height);
                        break;
                    case DropDownDirection.Below:
                    default:
                        pt = new Point(0, control.Height);
                        break;
                }

                DropDown(control, pt);
            }
            else
            {
                throw new ArgumentNullException("control");
            }
        }

        public void DropDown(Control control, Point pt)
        {
            if (control != null)
            {
                this.Owner = control.TopLevelControl as Form;
                pt = control.PointToScreen(pt);
                this.DropDown(pt);
            }
            else
            {
                throw new ArgumentNullException("control");
            }
        }

        public void DropDown(Point pt)
        {
            if (this.IsDroped)
                return;

            this.Location = pt;

            this.CalculateSize();

            if (AllowActive)
            {
                User32.ShowWindow(this.Handle, ShowWindowFlags.SW_SHOW);
            }
            else
            {
                User32.ShowWindow(this.Handle, ShowWindowFlags.SW_SHOWNA);
                this.Capture = true;
            }

            //User32.ShowWindow(this.Handle, ShowWindowFlags.SW_SHOWNA);

            //if (!this.AllowActive)
            //    this.Capture = true;

            this.IsDroped = true;
        }

        protected virtual void CalculateSize()
        {
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            this.ResizeGripBounds = new Rectangle(this.Width - 10, this.Height - 10, 10, 10);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            DrawBackground(e);

            if (this.AllowResize)
            {
                ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, ResizeGripBounds);
            }
        }

        protected virtual void DrawBackground(PaintEventArgs e)
        {
            Rectangle rect = ClientRectangle;

            e.Graphics.FillRectangle(SystemBrushes.Control, rect);
            e.Graphics.DrawRectangle(SystemPens.ControlDark, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_MOUSEACTIVATE = 0x0021;
            //const int WM_CHILDACTIVATE = 0x0022;
            const int WM_NCHITTEST = 0x0084;
            //const int WM_NCACTIVATE = 0x0086;

            if (!this.DesignMode)
            {
                this.OnWndProc(ref m);

                if (!this.AllowActive)
                {
                    if (this.IsDroped && !IsRightHandle(User32.GetCapture()))
                    {
                        this.EndDropDown();
                        return;
                    }
                }

                switch (m.Msg)
                {
                    case WM_MOUSEACTIVATE:
                        if (OnWmMouseActivate(ref m))
                            return;
                        else
                            break;
                    case WM_NCHITTEST:
                        if (OnWmNcHitTest(ref m))
                            return;
                        else
                            break;
                }
            }

            base.WndProc(ref m);
        }

        private bool OnWmMouseActivate(ref Message m)
        {
            //const int MA_ACTIVATE       = 1;
            //const int MA_ACTIVATEANDEAT = 2;
            //const int MA_NOACTIVATE     = 3;
            const int MA_NOACTIVATEANDEAT = 4;

            if (!this.AllowActive)
            {
                m.Result = (IntPtr)MA_NOACTIVATEANDEAT;
                return true;
            }

            return false;
        }

        private bool OnWmNcHitTest(ref Message m)
        {
            //const int  HTERROR = (-2);
            //const int  HTTRANSPARENT = (-1);
            //const int  HTNOWHERE = 0;
            //const int  HTCLIENT = 1;
            //const int  HTCAPTION = 2;
            //const int  HTSYSMENU = 3;
            //const int  HTGROWBOX = 4;
            //const int  HTSIZE = HTGROWBOX;
            //const int  HTMENU = 5;
            //const int  HTHSCROLL = 6;
            //const int  HTVSCROLL = 7;
            //const int  HTMINBUTTON = 8;
            //const int  HTMAXBUTTON = 9;
            //const int  HTLEFT = 10;
            //const int  HTRIGHT = 11;
            //const int  HTTOP = 12;
            //const int  HTTOPLEFT = 13;
            //const int  HTTOPRIGHT = 14;
            //const int  HTBOTTOM = 15;
            //const int  HTBOTTOMLEFT = 16;
            const int HTBOTTOMRIGHT = 17;
            //const int  HTBORDER = 18;
            //const int  HTREDUCE = HTMINBUTTON;
            //const int  HTZOOM = HTMAXBUTTON;
            //const int  HTSIZEFIRST = HTLEFT;
            //const int  HTSIZELAST = HTBOTTOMRIGHT;
            //const int  HTOBJECT = 19;
            //const int  HTCLOSE = 20;
            //const int  HTHELP = 21;

            if (this.AllowResize)
            {
                Point pt = GetPointFromInt(m.LParam.ToInt32());
                pt = this.PointToClient(pt);
                if (this.ResizeGripBounds.Contains(pt))
                {
                    m.Result = (IntPtr)HTBOTTOMRIGHT;
                    return true;
                }
            }

            return false;
        }

        protected virtual bool IsRightHandle(IntPtr hwnd)
        {
            if (this.Created)
                return hwnd == this.Handle || User32.IsChild(this.Handle, hwnd);
            else
                return false;
        }

        protected virtual void OnWndProc(ref Message m)
        {
        }

        public void EndDropDown()
        {
            if (this.IsDroped)
            {
                this.IsDroped = false;
                if (!this.AllowActive)
                    this.Capture = false;
                this.Hide();
            }
        }

        protected Point GetPointFromInt(int p)
        {
            return new Point(p & 0xffff, p >> 16);
        }
    }
}
