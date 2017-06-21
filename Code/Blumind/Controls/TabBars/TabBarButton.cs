using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Blumind.Controls
{
    class TabBarButton
    {
        TabBar _Owner;
        string _Text;
        Rectangle _Bounds;
        string _ToolTipText;
        Image _Icon;
        int _Index;
        ToolStripItemDisplayStyle _DisplayStyle = ToolStripItemDisplayStyle.Image;
        bool _Visible = true;

        public event System.EventHandler Click;
        public event System.EventHandler MouseDown;
        public event System.EventHandler MouseUp;
        public event UIPaintEventHandler Paint;

        static TabBarButton()
        {
        }

        public TabBarButton()
        {
            OwnerDraw = true;
        }

        public TabBarButton(string text, Image icon)
            : this()
        {
            Text = text;
            Icon = icon;
        }

        public TabBar Owner
        {
            get { return _Owner; }
            internal set { _Owner = value; }
        }

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

        public Rectangle Bounds
        {
            get { return _Bounds; }
            set { _Bounds = value; }
        }

        public string ToolTipText
        {
            get { return _ToolTipText; }
            set { _ToolTipText = value; }
        }

        public Image Icon
        {
            get { return _Icon; }
            set 
            {
                if (_Icon != value)
                {
                    _Icon = value;
                    OnIconChanged();
                }
            }
        }

        public int Index
        {
            get { return _Index; }
            set { _Index = value; }
        }

        public bool Visible
        {
            get { return _Visible; }
            set
            {
                if (_Visible != value)
                {
                    _Visible = value;
                    OnVisibleChanged();
                }
            }
        }

        public float CustomSize { get; set; }

        public float CustomSpace { get; set; }

        [DefaultValue(true)]
        public bool OwnerDraw { get; set; }

        [DefaultValue(ToolStripItemDisplayStyle.Image)]
        public ToolStripItemDisplayStyle DisplayStyle
        {
            get { return _DisplayStyle; }
            set { _DisplayStyle = value; }
        }

        public override string ToString()
        {
            return Text;
        }

        internal void OnClick()
        {
            if (Click != null)
            {
                Click(this, EventArgs.Empty);
            }
        }

        internal void OnMouseDown()
        {
            if (IsDroppingDown)
                IsDroppingDown = false;

            if (MouseDown != null)
            {
                MouseDown(this, EventArgs.Empty);
            }
        }

        internal void OnMouseUp()
        {
            if (MouseUp != null)
            {
                MouseUp(this, EventArgs.Empty);
            }
        }

        internal void Draw(PaintEventArgs e, UIControlStatus ucs)
        {
            OnPaint(e, ucs);
        }

        protected virtual void OnPaint(PaintEventArgs e, UIControlStatus ucs)
        {
            if (Paint != null)
                Paint(this, e, ucs);
        }
        
        protected virtual void OnTextChanged()
        {
            Invalidate();
        }

        protected virtual void OnIconChanged()
        {
            Invalidate();
        }

        protected virtual void OnVisibleChanged()
        {
            if (Owner != null)
            {
                Owner.NotifyItemVisibleChanged();
            }
        }

        public void Invalidate()
        {
            if (Owner != null)
            {
                Owner.Invalidate(Bounds);
            }
        }

        #region drop down menu
        bool _IsDroppingDown;

        public bool IsDroppingDown
        {
            get { return _IsDroppingDown; }
            private set
            {
                if (_IsDroppingDown != value)
                {
                    _IsDroppingDown = value;
                    OnIsDroppingDownChanged();
                }
            }
        }

        protected virtual void OnIsDroppingDownChanged()
        {
            Invalidate();
        }

        public void ShowMenu(ToolStripDropDown menu)
        {
            ShowMenu(menu, new Point(Bounds.Left, Bounds.Bottom), ToolStripDropDownDirection.BelowRight);
        }

        public void ShowMenu(ToolStripDropDown menu, Point location, ToolStripDropDownDirection direction)
        {
            if (menu == null)
                throw new ArgumentNullException();

            IsDroppingDown = true;
            try
            {
                menu.Show(Owner, location, direction);
                menu.Closed += menu_Closed;
            }
            catch
            {
                IsDroppingDown = false;
            }
        }

        void menu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            IsDroppingDown = false;

            if (sender is ContextMenuStrip)
            {
                var menu = (ContextMenuStrip)sender;
                menu.Closed -= menu_Closed;
            }
        }
        #endregion
    }
}
