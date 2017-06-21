using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Blumind.Controls.OS;
using Blumind.Core;

namespace Blumind.Controls.MapViews
{
    public class TopicEditControl : Control
    {
        private TextBox InternalTextBox;
        private const int BorderSize = 3;
        private SizeGripper Gripper;
        private ShortcutKeysTable ShortcutKeys;

        public event System.EventHandler Cancel;
        public event System.EventHandler Apply;

        public TopicEditControl()
        {
            InternalTextBox = new TextBox();
            InternalTextBox.Multiline = true;
            InternalTextBox.BorderStyle = BorderStyle.None;
            InternalTextBox.TextAlign = HorizontalAlignment.Center;
            //InternalTextBox.ImeMode = ImeMode.On;
            InternalTextBox.KeyDown += new KeyEventHandler(InternalTextBox_KeyDown);

            //
            Gripper = new SizeGripper();

            Controls.AddRange(new Control[] { InternalTextBox, Gripper });

            //
            ResizeRedraw = true;
            DoubleBuffered = true;

            InitializeKeyBoard();
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                InternalTextBox.Text = value;
            }
        }

        //protected override void OnCreateControl()
        //{
        //    base.OnCreateControl();

        //    LayoutControls();
        //}

        //protected override void OnResize(EventArgs e)
        //{
        //    base.OnResize(e);

        //    LayoutControls();
        //}
        
        private void InitializeKeyBoard()
        {
            ShortcutKeys = new ShortcutKeysTable();
            ShortcutKeys.Register(KeyMap.SelectAll, delegate() { InternalTextBox.SelectAll(); });
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            LayoutControls();
        }

        private void LayoutControls()
        {
            if (!Created)
                return;

            Rectangle rect = ClientRectangle;
            rect.Inflate(-BorderSize, -BorderSize);

            //
            InternalTextBox.Bounds = new Rectangle(rect.X, rect.Y, rect.Width - Gripper.Width / 2, rect.Height - Gripper.Height / 2);
            //
            Gripper.Location = new Point(rect.Right - Gripper.Width, rect.Bottom - Gripper.Height);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            InternalTextBox.Font = this.Font;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(InternalTextBox.BackColor);
            //base.OnPaint(e);
            Graphics grf = e.Graphics;
            //grf.FillRectangle(SystemBrushes.Window, ClientRectangle);
            //grf.DrawRectangle(SystemPens.ActiveCaption, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
            ControlPaint.DrawVisualStyleBorder(grf, new Rectangle(0, 0, Width - 1, Height - 1));

            //Rectangle rect = Tb.Bounds;
            //grf.FillRectangle(SystemBrushes.Window, rect);
            //ControlPaint.DrawVisualStyleBorder(grf, new Rectangle(rect.Left - 1, rect.Top - 1, rect.Width + 1, rect.Height + 1));
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            if (InternalTextBox.CanFocus)
            {
                InternalTextBox.Focus();
            }
        }

        private void InternalTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (ShortcutKeys.Haldle(e.KeyData))
            {
                e.SuppressKeyPress = true;
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    CancelEdit(true);
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Enter:
                    if (!e.Control)
                    {
                        ApplyEdit(true);
                        e.SuppressKeyPress = true;
                    }
                    break;
            }
        }

        public void EndEdit(bool acceptChange, bool invokeEvent)
        {
            if (acceptChange)
                ApplyEdit(invokeEvent);
            else
                CancelEdit(invokeEvent);
        }

        private void ApplyEdit(bool invokeEvent)
        {
            Text = InternalTextBox.Text;

            if (invokeEvent && Apply != null)
            {
                Apply(this, EventArgs.Empty);
            }
        }

        private void CancelEdit(bool invokeEvent)
        {
            InternalTextBox.Text = Text;

            if (invokeEvent && Cancel != null)
            {
                Cancel(this, EventArgs.Empty);
            }
        }

        internal void Send_WM_CHAR(char c)
        {
            if (Visible && InternalTextBox.Created)
            {
                if (c < 0x80)
                {
                    User32.PostMessage(InternalTextBox.Handle, WinMessages.WM_CHAR, c, 1);
                }
                else
                {
                    byte[] bytes = System.Text.Encoding.Default.GetBytes(new char[] { c });
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        User32.PostMessage(InternalTextBox.Handle, WinMessages.WM_CHAR, bytes[i], 1);
                    }
                }
            }
        }

        class SizeGripper : Control
        {
            private MouseButtons MouseDownButton = MouseButtons.None;
            private Size ParentSize;
            private Point MouseDownPos;

            public SizeGripper()
            {
                Size = new Size(16, 16);
                Cursor = Cursors.SizeNWSE;

                GraphicsPath gp = new GraphicsPath();
                gp.AddLine(0, Height, Width, Height);
                gp.AddLine(Width, Height, Width, 0);
                gp.CloseFigure();
                Region = new Region(gp);

                SetStyle(ControlStyles.UserPaint
                    | ControlStyles.AllPaintingInWmPaint
                    | ControlStyles.OptimizedDoubleBuffer
                    | ControlStyles.SupportsTransparentBackColor, true);

                BackColor = Color.Transparent;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                InvokePaintBackground(this, e);

                //
                if (VisualStyleRenderer.IsSupported)
                {
                    VisualStyleRenderer renderer = new VisualStyleRenderer(VisualStyleElement.Status.Gripper.Normal);
                    renderer.DrawBackground(e.Graphics, ClientRectangle);
                }
            }

            //protected override void OnMouseHover(EventArgs e)
            //{
            //    base.OnMouseHover(e);

            //    Visible = true;
            //}

            //protected override void OnMouseLeave(EventArgs e)
            //{
            //    base.OnMouseLeave(e);

            //    if (Parent != null)
            //    {
            //        Point pt = Parent.PointToClient(MousePosition);
            //        Visible = Bounds.Contains(pt);
            //    }
            //    else
            //    {
            //        Visible = false;
            //    }
            //}

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);

                MouseDownButton = e.Button;
                MouseDownPos = PointToScreen(new Point(e.X, e.Y));
                if (Parent != null)
                    ParentSize = Parent.Size;
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);

                MouseDownButton = System.Windows.Forms.MouseButtons.None;
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);

                if (MouseDownButton == System.Windows.Forms.MouseButtons.Left)
                {
                    Point pt = PointToScreen(new Point(e.X, e.Y));
                    Size size = new Size(ParentSize.Width + (pt.X - MouseDownPos.X)
                    , ParentSize.Height + (pt.Y - MouseDownPos.Y));
                    if (Parent != null)
                        Parent.Size = size;
                }
            }
        }
    }
}
