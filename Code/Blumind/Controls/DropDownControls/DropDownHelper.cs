using System;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace Blumind.Controls
{
    public class DropDownHelper : System.ComponentModel.Component
    {
        private Control _Control = null;
        private ToolStripDropDown DropDownContainer = null;
        private bool _IsDropDown = false;

        public event CancelEventHandler Opening;
        public event EventHandler Opened;
        public event CancelEventHandler Closing;
        public event EventHandler Closed;

        public DropDownHelper()
        {
        }

        public DropDownHelper(Control control)
        {
            Control = control;
        }

        [DefaultValue(null)]
        public Control Control
        {
            get { return _Control; }
            set 
            {
                if (_Control != value)
                {
                    _Control = value;
                    OnControlChanged();
                }
            }
        }

        [DefaultValue(false), Browsable(false)]
        public bool IsDropDown
        {
            get { return _IsDropDown; }
            private set { _IsDropDown = value; }
        }

        public void DropDown(Control owner, int x, int y)
        {
            DropDown(owner, x, y, false);
        }

        public void DropDown(Control owner, int x, int y, bool resizable)
        {
            if (Control == null)
                return;

            if (DropDownContainer == null)
            {
                DropDownContainer = new ToolStripDropDown();
                //DropDownContainer.Stretch = resizable;
                DropDownContainer.Opened += new EventHandler(DropDownContainer_Opened);
                DropDownContainer.Opening += new CancelEventHandler(DropDownContainer_Opening);
                DropDownContainer.Closed += new ToolStripDropDownClosedEventHandler(DropDownContainer_Closed);
                DropDownContainer.Closing += new ToolStripDropDownClosingEventHandler(DropDownContainer_Closing);

                ToolStripControlHost tsch = new ToolStripControlHost(Control);
                DropDownContainer = new ToolStripDropDown();
                DropDownContainer.Items.Add(tsch);
            }

            IsDropDown = true;
            DropDownContainer.Show(owner, x, y);
        }

        public void Close()
        {
            IsDropDown = false;
            if (DropDownContainer != null)
            {
                DropDownContainer.Close();
            }
        }

        private void OnControlChanged()
        {
            if (DropDownContainer != null)
            {
                DropDownContainer.Items.Clear();

                ToolStripControlHost tsch = new ToolStripControlHost(Control);
                DropDownContainer = new ToolStripDropDown();
                DropDownContainer.Items.Add(tsch);
            }
        }

        private void DropDownContainer_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            bool cancel = e.Cancel;
            OnClosing(ref cancel);
            e.Cancel = cancel;
        }

        private void DropDownContainer_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            OnClosed();
        }

        private void DropDownContainer_Opening(object sender, CancelEventArgs e)
        {
            bool cancel = e.Cancel;
            OnOpening(ref cancel);
            e.Cancel = cancel;
        }

        private void DropDownContainer_Opened(object sender, EventArgs e)
        {
            OnOpened();
        }

        private void OnOpened()
        {
            IsDropDown = true;

            if (Opened != null)
            {
                Opened(this, EventArgs.Empty);
            }
        }

        private void OnClosed()
        {
            IsDropDown = false;

            if (Closed != null)
            {
                Closed(this, EventArgs.Empty);
            }
        }

        private void OnClosing(ref bool cancel)
        {
            CancelEventArgs ce = new CancelEventArgs(cancel);
            if (Closing != null)
            {
                Closing(this, ce);
            }
        }

        private void OnOpening(ref bool cancel)
        {
            CancelEventArgs ce = new CancelEventArgs(cancel);
            if (Opening != null)
            {
                Opening(this, ce);
            }
        }
    }
}
