using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Blumind.Controls
{
    class ControlFullScreenShell : BaseForm
    {
        private Control _AttachControl = null;
        private DockStyle _ControlDock = DockStyle.None;
        private Rectangle _ControlBounds = Rectangle.Empty;
        private Control _ControlParent = null;
        private bool TrueClose = false;

        static Hashtable Shells = new Hashtable();

        private ControlFullScreenShell(Control control)
        {
            AttachControl = control;
            if (control != null)
            {
                ControlDock = control.Dock;
                ControlBounds = control.Bounds;
                ControlParent = control.Parent;
            }
            KeyPreview = true;
            ShowInTaskbar = false;
        }

        public Control AttachControl
        {
            get { return _AttachControl; }
            private set { _AttachControl = value; }
        }

        public DockStyle ControlDock
        {
            get { return _ControlDock; }
            set { _ControlDock = value; }
        }

        public Rectangle ControlBounds
        {
            get { return _ControlBounds; }
            set { _ControlBounds = value; }
        }

        public Control ControlParent
        {
            get { return _ControlParent; }
            set { _ControlParent = value; }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (this.FullScreen && e.KeyCode == HotKeys.FullScreen)
                ExitFullScreen(this.AttachControl);
        }

        public static void EnterFullScreen(Control control)
        {
            if (control == null || Shells.Contains(control))
                return;
            ControlFullScreenShell shell = new ControlFullScreenShell(control);
            if (control.Parent != null)
            {
                control.Parent.Controls.Remove(control);
            }
            control.Dock = DockStyle.Fill;
            shell.Controls.Add(control);
            shell.Text = control.Text;
            Shells.Add(control, shell);

            shell.FullScreen = true;
            shell.EnterFullScreen();
            shell.Show(shell.ControlParent);
        }

        public static void ExitFullScreen(Control control)
        {
            if (control == null || !Shells.Contains(control))
                return;
            ControlFullScreenShell shell = Shells[control] as ControlFullScreenShell;
            if (shell != null && shell.AttachControl == control)
            {
                shell.Controls.Remove(control);
                if (shell.ControlParent != null)
                {
                    shell.ControlParent.Controls.Add(control);
                }
                control.Dock = shell.ControlDock;
                control.Bounds = shell.ControlBounds;
                shell.TrueClose = true;
                shell.Close();
            }

            Shells.Remove(control);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (!TrueClose)
            {
                ExitFullScreen(this.AttachControl);
            }
        }
    }
}
