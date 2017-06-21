using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Controls
{
    static class ControlExtensions
    {
        public static bool IsSubControl(this Control owner, Control child)
        {
            if (owner == null || child == null)
                throw new NullReferenceException();

            Control c = child;
            while (c != null)
            {
                if (c == owner)
                    return true;
                c = c.Parent;
            }

            return false;
        }

        public static void MoveToCenter(this Control owner)
        {
            if (owner == null)
                throw new ArgumentNullException();

            if (owner.Parent == null)
                return;

            MoveToCenter(owner, owner.Parent);
        }

        public static void MoveToCenter(this Control owner, Control parentControl)
        {
            if (parentControl == null)
                throw new ArgumentNullException();

            Point pt = new Point(
                (parentControl.ClientSize.Width - owner.Width) / 2,
                (parentControl.ClientSize.Height - owner.Height) / 2);
            owner.Location = new Point(Math.Max(0, pt.X), Math.Max(0, pt.Y));
        }

        public static void EnsureVisiable(this Control control)
        {
            if (control == null)
                throw new ArgumentNullException();

            if (control.Parent == null)
                return;

            var cr = control.Parent.ClientRectangle;
            var r = control.Bounds;

            Point pt = r.Location;
            pt.Y = Math.Max(cr.Top, Math.Min(pt.Y, cr.Bottom - r.Height));
            pt.X = Math.Max(cr.Left, Math.Min(pt.X, cr.Right - r.Width));
            control.Location = pt;
        }

        public static void DrawToBitmap(this Control control, Bitmap bitmap, Rectangle sourceBounds, int targetX, int targetY)
        {
            if (sourceBounds.Width < control.Width || sourceBounds.Height < control.Height)
            {
                if (sourceBounds.Width <= 0 || sourceBounds.Height <= 0)
                    return;
                Bitmap temp = new Bitmap(control.ClientSize.Width, control.ClientSize.Height);
                control.DrawToBitmap(temp, control.ClientRectangle);

                using (Graphics grf = Graphics.FromImage(bitmap))
                {
                    int width = Math.Min(sourceBounds.Width, temp.Width);
                    int height = Math.Min(sourceBounds.Height, temp.Height);

                    grf.DrawImage(temp,
                        new Rectangle(targetX, targetY, width, height),
                        sourceBounds.X, sourceBounds.Y, width, height,
                        GraphicsUnit.Pixel);
                }
            }
            else
            {
                control.DrawToBitmap(bitmap, new Rectangle(targetX, targetY, sourceBounds.Width, sourceBounds.Height));
            }
        }

        public static void SetAutoScaleMode(this Control control, AutoScaleMode autoScaleMode)
        {
            if (control == null)
                return;

            foreach (Control c in control.Controls)
            {
                SetAutoScaleMode(c, autoScaleMode);
            }

            if (control is ContainerControl)
            {
                ((ContainerControl)control).AutoScaleMode = autoScaleMode;
            }
        }
        
        public static void SetFontNotScale(this ContainerControl control, Font font)
        {
            if (control == null)
                throw new NullReferenceException();

            if (control.Font != font)
            {
                var sm = control.AutoScaleMode;
                control.AutoScaleMode = AutoScaleMode.None;
                control.Font = font;
                control.AutoScaleMode = sm;
            }
        }

        [Obsolete]
        public static int GetLayoutSuspendCount(this Control control)
        {
            if (control == null)
                throw new ArgumentNullException();

            var type = typeof(Control);
            var fif = type.GetField("layoutSuspendCount",
                System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.GetField);
            if (fif != null)
                return ST.GetIntDefault(fif.GetValue(control));
            else
                return 0;
        }

        public static bool IsDesignMode(this Control control)
        {
            if (control == null)
                throw new ArgumentNullException();

            if (control.Site != null && control.Site.DesignMode)
                return true;

            if (control.Parent != null)
                return IsDesignMode(control.Parent);
            else
                return false;
        }

        #region MessageBox
        public static DialogResult ShowMessage(this Control control, string message, MessageBoxIcon icon)
        {
            return ShowMessage(control, message, MessageBoxButtons.OK, icon);
        }

        public static DialogResult ShowMessage(this Control control, string message, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            message = Lang._(message);

            if (control != null && !control.IsDisposed && !control.InvokeRequired)
                return MessageBox.Show(control, message, control.Text, buttons, icon);
            else
                return MessageBox.Show(message, control.Text, buttons, icon);
        }

        public static DialogResult ShowMessage(this Control control, string message, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            message = Lang._(message);

            if (control != null && !control.IsDisposed && !control.InvokeRequired)
                return MessageBox.Show(control, message, control.Text, buttons, icon, defaultButton);
            else
                return MessageBox.Show(message, control.Text, buttons, icon, defaultButton);
        }

        public static void ShowMessage(this Control control, Exception ex)
        {
            if (control != null && !control.IsDisposed && !control.InvokeRequired)
                ExceptionDialog.Show(control, ex);
            else
                ExceptionDialog.Show(ex);
            //ShowMessage(ex.Message, MessageBoxIcon.Error);
        }
        #endregion
    }
}
