using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Blumind.Controls
{
    public static class IconExtractor
    {
        [Flags]
        public enum DrawIconExFlags
        {
            DI_MASK = 0x0001,
            DI_IMAGE = 0x0002,
            DI_NORMAL = 0x0003,
            DI_COMPAT = 0x0004,
            DI_DEFAULTSIZE = 0x0008,
            DI_NOMIRROR = 0x0010,
        }

        [Flags]
        public enum SHGFI
        {
            ICON = 0x100,
            LARGEICON = 0x0,
            SMALLICON = 0x1,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes,
                             ref SHFILEINFO psfi, uint cbSizeFileInfo, SHGFI uFlags);

        public static Icon ExtractAssociatedIcon(string filePath, bool small)
        {
            IntPtr handle = small ? GetSmallAssociatedIcon(filePath) : GetLargeAssociatedIcon(filePath);
            //IntPtr handle = ExtractAssociatedIcon(filePath, 0);
            if (handle != IntPtr.Zero)
            {
                return System.Drawing.Icon.FromHandle(handle);
            }
            else
            {
                return null;
            }
        }

        static IntPtr GetSmallAssociatedIcon(string filePath)
        {
            SHFILEINFO sif = new SHFILEINFO();
            if (SHGetFileInfo(filePath, 0, ref sif, (uint)Marshal.SizeOf(sif), SHGFI.ICON | SHGFI.SMALLICON) != IntPtr.Zero)
            {
                return sif.hIcon;
            }

            return IntPtr.Zero;
        }

        static IntPtr GetLargeAssociatedIcon(string filePath)
        {
            SHFILEINFO sif = new SHFILEINFO();
            if (SHGetFileInfo(filePath, 0, ref sif, (uint)Marshal.SizeOf(sif), SHGFI.ICON | SHGFI.LARGEICON) != IntPtr.Zero)
            {
                return sif.hIcon;
            }

            return IntPtr.Zero;
        }

        static IntPtr ExtractAssociatedIcon(string filePath, ushort index)
        {
            Uri uri;
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException();

            try
            {
                uri = new Uri(filePath);
            }
            catch (UriFormatException)
            {
                filePath = Path.GetFullPath(filePath);
                uri = new Uri(filePath);
            }

            if (uri.IsUnc)
            {
                throw new ArgumentException();
            }

            if (uri.IsFile)
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException(filePath);
                }

                StringBuilder iconPath = new StringBuilder(260);
                iconPath.Append(filePath);
                IntPtr handle = ExtractAssociatedIcon(IntPtr.Zero, iconPath, out index);
                return handle;
            }

            return IntPtr.Zero;
        }

        [DllImport("shell32.dll")]
        public static extern IntPtr ExtractAssociatedIcon(IntPtr hInst, StringBuilder lpIconPath, out ushort lpiIcon);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DrawIconEx(IntPtr hdc, int xLeft, int yTop
            , IntPtr hIcon, int cxWidth, int cyHeight
            , int istepIfAniCur, IntPtr hbrFlickerFreeDraw, DrawIconExFlags diFlags);

        public static Bitmap ExtractAssociatedIconImage(string filename)
        {
            return ExtractAssociatedIconImage(filename, true);
        }

        public static Bitmap ExtractAssociatedIconImage(string filename, bool small)
        {
            Bitmap bmp = null;
            using (Icon icon = ExtractAssociatedIcon(filename, small))
            {
                if (icon != null)
                {
                    bmp = icon.ToBitmap();
                    icon.Dispose();
                }
            }
            return bmp;

            //return ExtractIcon(ExtractAssociatedIcon(filename), size);
        }

        // Based on: http://www.codeproject.com/KB/cs/IconExtractor.aspx
        // And a hint from: http://www.codeproject.com/KB/cs/IconLib.aspx
        public static Bitmap ExtractIcon(Icon icoIcon, Size size)
        {
            Bitmap bmpPngExtracted = null;
            try
            {
                byte[] srcBuf = null;
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    icoIcon.Save(stream);
                    srcBuf = stream.ToArray();
                }

                const int SizeICONDIR = 6;
                const int SizeICONDIRENTRY = 16;
                int iCount = BitConverter.ToInt16(srcBuf, 4);
                for (int iIndex = 0; iIndex < iCount; iIndex++)
                {
                    int iWidth = srcBuf[SizeICONDIR + SizeICONDIRENTRY * iIndex];
                    int iHeight = srcBuf[SizeICONDIR + SizeICONDIRENTRY * iIndex + 1];
                    int iBitCount = BitConverter.ToInt16(srcBuf, SizeICONDIR + SizeICONDIRENTRY * iIndex + 6);
                    if (iWidth == size.Width && iHeight == size.Height)// && iBitCount == 32)
                    {
                        int iImageSize = BitConverter.ToInt32(srcBuf, SizeICONDIR + SizeICONDIRENTRY * iIndex + 8);
                        int iImageOffset = BitConverter.ToInt32(srcBuf, SizeICONDIR + SizeICONDIRENTRY * iIndex + 12);
                        System.IO.MemoryStream destStream = new System.IO.MemoryStream();
                        System.IO.BinaryWriter writer = new System.IO.BinaryWriter(destStream);
                        writer.Write(srcBuf, iImageOffset, iImageSize);
                        destStream.Seek(0, System.IO.SeekOrigin.Begin);
                        bmpPngExtracted = new Bitmap(destStream); // This is PNG! :)
                        break;
                    }
                }
            }
            catch
            {
                return null;
            }

            return bmpPngExtracted;
        }

        public static Bitmap ExtractIconByExtension(string extension, bool small)
        {
            Bitmap icon = null;
            if (!string.IsNullOrEmpty(extension))
            {
                string tempFile = Path.Combine(Path.GetTempPath(), string.Format("blumind_test{0}", extension));
                try
                {
                    if (!File.Exists(tempFile))
                    {
                        using (FileStream stream = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                        {
                            stream.Close();
                        }
                    }

                    icon = IconExtractor.ExtractAssociatedIconImage(tempFile, small);
                }
                finally
                {
                    if (File.Exists(tempFile))
                        File.Delete(tempFile);
                }
            }

            return icon;
        }

        public static Bitmap ExtractSmallIconByExtension(string extension)
        {
            return ExtractIconByExtension(extension, true);
        }

        public static Bitmap ExtractLargeIconByExtension(string extension)
        {
            return ExtractIconByExtension(extension, false);
        }
    }
}
