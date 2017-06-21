using System;
using System.Drawing;
using System.Windows.Forms;
using Blumind.Controls.OS;
using Blumind.Core;

namespace Blumind.Controls
{
    class DragManager : IDisposable
    {
        bool _InnerDrag = true;
        Control _Owner = null;
        Rectangle MoveRange = Rectangle.Empty;
        Rectangle SourceRange = Rectangle.Empty;
        Bitmap BmpSource = null;
        bool _IsDraging = false;
        DragBmpWin DrawBox = null;
        Point RelativeMousePos = Point.Empty;
        object _DragObject = null;
        DragBmpWin DragginBox = null;

        public DragManager(Control owner)
        {
            Owner = owner;
        }

        public DragManager(Control owner, DragFeedbackType draggingFeedback)
        {
            Owner = owner;
            DraggingFeedback = draggingFeedback;
        }

        ~DragManager()
        {
            Dispose();
        }

        private Control Owner
        {
            get { return _Owner; }
            set { _Owner = value; }
        }

        public bool IsDraging
        {
            get { return _IsDraging; }
            private set { _IsDraging = value; }
        }

        public bool InnerDrag
        {
            get { return _InnerDrag; }
            set { _InnerDrag = value; }
        }

        public object DragObject
        {
            get { return _DragObject; }
            private set { _DragObject = value; }
        }

        public DragFeedbackType DraggingFeedback { get; private set; }

        public void Drag(Point pt)
        {
            pt.X += RelativeMousePos.X;
            pt.Y += RelativeMousePos.Y;

            //
            if (!MoveRange.IsEmpty)
            {
                pt.X = Math.Max(MoveRange.Left, Math.Min(MoveRange.Right - SourceRange.Width, pt.X));
                pt.Y = Math.Max(MoveRange.Top, Math.Min(MoveRange.Bottom - SourceRange.Height, pt.Y));
            }

            if(this.Owner != null)
                pt = this.Owner.PointToScreen(pt);
            DrawBox.Show(pt);
        }

        public void BeginDrag(object dragObject, Rectangle sourceRange, Point mousePos)
        {
            BeginDrag(dragObject, sourceRange, Rectangle.Empty, mousePos);
        }

        public void BeginDrag(object data, Rectangle sourceRange, Rectangle moveRange, Point mousePos)
        {
            SourceRange = sourceRange;
            MoveRange = moveRange;
            DragObject = data;
            IsDraging = false;

            if (Owner == null)
                return;
            BmpSource = WinHelper.CopyImage(this.Owner, sourceRange.X, sourceRange.Y, sourceRange.Width, sourceRange.Height);
            RelativeMousePos = new Point(sourceRange.X - mousePos.X, sourceRange.Y - mousePos.Y);
            IsDraging = true;

            if (DrawBox == null)
            {
                DrawBox = new DragBmpWin();
            }
            DrawBox.Show(BmpSource, new Point(sourceRange.X, sourceRange.Y));
        }

        public void BeginDrag(object data, Rectangle sourceRange, Rectangle moveRange, Point mousePos, GenerateImageCallBack dragImageCreator)
        {
            SourceRange = sourceRange;
            MoveRange = moveRange;
            DragObject = data;
            IsDraging = false;

            if (Owner == null)
                return;

            RelativeMousePos = new Point(sourceRange.X - mousePos.X, sourceRange.Y - mousePos.Y);
            IsDraging = true;

            if (DraggingFeedback != DragFeedbackType.None)
            {
                if (DragginBox == null)
                {
                    DragginBox = new DragBmpWin(this, DraggingFeedback == DragFeedbackType.SubControl ? Owner : null);
                }
                BuildBmpSource(data, sourceRange, dragImageCreator);
                DragginBox.Show(BmpSource, new Point(sourceRange.X, sourceRange.Y));
            }
        }

        public void EndDrag()
        {
            DrawBox.Hide();
            IsDraging = false;
        }

        public void CancelDraging()
        {
            if (DragginBox != null)
                DragginBox.Hide();
            IsDraging = false;
        }

        void BuildBmpSource(object data, Rectangle sourceRange, GenerateImageCallBack dragImageCreator)
        {
            if (BmpSource != null)
            {
                BmpSource.Dispose();
                BmpSource = null;
            }

            if (dragImageCreator != null)
            {
                BmpSource = dragImageCreator(data);
            }
            else
            {
                if (Owner == null || !Owner.Created || sourceRange.Width <= 0 || sourceRange.Height <= 0)
                    return;

                BmpSource = new Bitmap(sourceRange.Width, sourceRange.Height);
                Owner.DrawToBitmap(BmpSource, sourceRange, 0, 0);
            }
        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (BmpSource != null)
            {
                BmpSource.Dispose();
                BmpSource = null;
            }
        }

        #endregion

        #region Drag Bmp Win
        class DragBmpWin : Form
        {
            int _Width = 0;
            int _Height = 0;
            bool ImageChanged = false;
            Control ParentWin;
            DragManager OwnerManager;

            public DragBmpWin()
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.Opacity = 0.8;
                this.ShowInTaskbar = false;
            }

            public DragBmpWin(DragManager owner, Control parent)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.Opacity = 0.8;
                this.ShowInTaskbar = false;
                OwnerManager = owner;

                if (parent != null)
                {
                    TopLevel = false;
                    ParentWin = parent;
                }
            }

            public void Show(Bitmap bmp, Point pt)
            {
                this.BackgroundImage = bmp;
                ImageChanged = true;
                //this.Size = new Size(bmp.Width, bmp.Height);
                _Width = bmp.Width;
                _Height = bmp.Height;

                Show(pt);
                ImageChanged = false;
            }

            public void Show(Point pt)
            {
                if (!User32.IsWindow(this.Handle))
                {
                    CreateHandle();
                }

                if (!ImageChanged && User32.IsWindowVisible(this.Handle))
                {
                    User32.SetWindowPos(this.Handle, WindowHandle.HWND_TOP, pt.X, pt.Y, 0, 0,
                        SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOSIZE);
                }
                else
                {
                    User32.SetWindowPos(this.Handle, WindowHandle.HWND_TOP, pt.X, pt.Y, _Width, _Height,
                        SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_SHOWWINDOW);
                }
            }
        }
        #endregion

        public enum DragFeedbackType
        {
            None,
            TopWindow,
            SubControl,
        }
    }
}
