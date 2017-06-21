using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Blumind.Controls
{
    class DropFilesEventArgs : HandledEventArgs
    {
        public DropFilesEventArgs(string[] files, Point mousePosition)
        {
            FileNames = files;
            MousePosition = mousePosition;
        }

        public string[] FileNames { get; private set; }

        public Point MousePosition { get; private set; }
    }

    /// <summary>
    /// 处理拖拽文件
    /// </summary>
    interface IDropFilesHandler
    {
        void OnFilesDrop(DropFilesEventArgs e);
    }

    static class DropFilesHandlerExtensions
    {
        public static void PostDropFiles(this Control control, DropFilesEventArgs e)
        {
            if (control == null)
                return;

            var c = control.GetChildAtPoint(e.MousePosition);
            if (c != null)
            {
                var cpt = new Point(e.MousePosition.X - c.Left, e.MousePosition.Y - c.Top);
                var de = new DropFilesEventArgs(e.FileNames, cpt);
                if (c is IDropFilesHandler)
                {
                    ((IDropFilesHandler)c).OnFilesDrop(de);
                }
                else
                {
                    PostDropFiles(c, de);
                }
                e.Handled = de.Handled;
            }
        }
    }
}
