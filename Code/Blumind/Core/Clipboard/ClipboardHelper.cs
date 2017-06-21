using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Blumind.Controls.OS;

namespace Blumind.Core
{
    static class ClipboardHelper
    {
        public static string GetHtml()
        {
            if (!Clipboard.ContainsText())
                return null;
            
            if (Clipboard.ContainsText(TextDataFormat.Html))
            {
                var html = ClipboardHtmlOutput.FromClipboard();
                if (html != null)
                {
                    return html.Fragment;
                }
            }

            if (Clipboard.GetDataObject().GetDataPresent("text/html"))
            {
                var data = Clipboard.GetData("text/html");
                if (data is Stream)
                {
                    var stream = (Stream)data;
                    var buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    return Encoding.Unicode.GetString(buffer);
                }
            }
            
            return Clipboard.GetText();
        }

        public static byte[] GetClipboardData(string format)
        {
            uint formatHandle = User32.RegisterClipboardFormat(format);
            if (formatHandle != 0)
            {
                User32.OpenClipboard(IntPtr.Zero);

                //Get pointer to clipboard data in the selected format
                var clipboardDataPointer = User32.GetClipboardData(formatHandle);

                //Do a bunch of crap necessary to copy the data from the memory
                //the above pointer points at to a place we can access it.
                var length = Kernel32.GlobalSize(clipboardDataPointer);
                var gLock = Kernel32.GlobalLock(clipboardDataPointer);

                //Init a buffer which will contain the clipboard data
                var buffer = new byte[(int)length];

                //Copy clipboard data to buffer
                Marshal.Copy(gLock, buffer, 0, (int)length);
                User32.CloseClipboard();

                return buffer;
            }

            return new byte[0];
        }

        public static void SetHtml(DataObject dataObject, string html)
        {
            dataObject.SetData(DataFormats.Html, ClipboardHtmlOutput.Package(html));
            //dataObject.SetText(html, TextDataFormat.Html);
        }
    }
}
