using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Blumind.Controls.OS;

namespace Blumind.Controls
{
    class ToolTipControl : ToolTip
    {
        enum TipAction
        {
            Show,
            Hide,
        }

        class TipFrame : Form
        {
            Timer MyTimer;
            int _InitialDelay = 500;
            int _HideDelay = 500;
            int TipX;
            int TipY;
            TipAction NextAction;

            public TipFrame()
            {
                SetStyle(ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.OptimizedDoubleBuffer, true);

                FormBorderStyle = FormBorderStyle.None;
                ShowInTaskbar = false;
                Opacity = 0.8;

                MyTimer = new Timer();
                MyTimer.Tick += new EventHandler(MyTimer_Tick);
            }

            public int InitialDelay
            {
                get { return _InitialDelay; }
                set { _InitialDelay = value; }
            }

            public int HideDelay
            {
                get { return _HideDelay; }
                set { _HideDelay = value; }
            }

            protected override void OnTextChanged(EventArgs e)
            {
                if (!string.IsNullOrEmpty(Text))
                {
                    Size size = TextRenderer.MeasureText(Text, Font);
                    size.Width += 10;
                    size.Height += 10;
                    this.Size = size;
                }
                else
                {
                    this.Size = Size.Empty;
                }

                base.OnTextChanged(e);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Rectangle rect = ClientRectangle;
                Graphics grf = e.Graphics;
                LinearGradientBrush brush = new LinearGradientBrush(rect, PaintHelper.GetLightColor(BackColor), BackColor, 90.0f);
                grf.FillRectangle(brush, rect);

                if (!string.IsNullOrEmpty(Text))
                {
                    grf.DrawString(Text, Font, new SolidBrush(ForeColor), rect, PaintHelper.SFCenter);
                }

                grf.DrawRectangle(new Pen(PaintHelper.GetDarkColor(BackColor)), rect.Left, rect.Top, rect.Width - 1, rect.Height - 1);
            }

            private void MyTimer_Tick(object sender, EventArgs e)
            {
                switch(NextAction)
                {
                    case TipAction.Show:
                        if (!User32.IsWindowVisible(Handle))
                        {
                            Opacity = 0.2;
                            ShowInternal();
                        }
                        else
                        {
                            Opacity = Math.Min(1, Opacity + 0.3);
                        }

                        if (Opacity >= 1.0)
                        {
                            MyTimer.Enabled = false;
                        }
                        break;
                    case TipAction.Hide:
                        Opacity = Math.Max(0, Opacity - 0.1);
                        if (Opacity <= 0.2)
                        {
                            HideInternal();
                            MyTimer.Enabled = false;
                        }
                        break;
                    default:
                        MyTimer.Enabled = false;
                        break;
                }
            }

            public void ShowTip(string text, int x, int y)
            {
                Text = text;
                TipX = x;
                TipY = y;
                NextAction = TipAction.Show;

                MyTimer.Interval = InitialDelay / 3;
                MyTimer.Enabled = true;
            }

            public void HideTip()
            {
                NextAction = TipAction.Hide;
                MyTimer.Interval = HideDelay / 3;
                MyTimer.Enabled = true;
            }

            private void ShowInternal()
            {
                if (!User32.IsWindow(Handle))
                {
                    CreateControl();
                }

                User32.SetWindowPos(Handle, WindowHandle.HWND_TOP, TipX, TipY, Width, Height,
                    SetWindowPosFlags.SWP_NOACTIVATE
                    | SetWindowPosFlags.SWP_NOSIZE
                    | SetWindowPosFlags.SWP_SHOWWINDOW);
                //User32.ShowWindow(TipBox.Handle, ShowWindowFlags.SW_SHOWNOACTIVATE);
            }

            private void HideInternal()
            {
                if (User32.IsWindowVisible(Handle))
                {
                    User32.ShowWindow(Handle, Blumind.Controls.OS.ShowWindowFlags.SW_HIDE);
                }
            }
        }

        TipFrame TipBox;

        public void Hide()
        {
            if (TipBox != null)
            {
                TipBox.HideTip();
            }
        }

        public new void Show(string text, IWin32Window control, int x, int y, int duration)
        {
            if (!string.IsNullOrEmpty(text))
            {
                if (TipBox == null || TipBox.IsDisposed)
                {
                    TipBox = new TipFrame();
                }
                TipBox.ShowTip(text, x, y);
            }
            //base.Show(text, control, x, y, duration);
        }
    }
}
