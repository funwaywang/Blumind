using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Blumind.Canvas;
using Blumind.Controls.OS;
using Blumind.Controls.Paint;

namespace Blumind.Controls
{
    static partial class PaintHelper
    {
        #region Static Members
        private static ImageAttributes disabledImageAttr = null;
        private static Random rand = new Random(DateTime.Now.Second);

        static PaintHelper()
        {
        }

        public static void Init()
        {
        }

        public static void Term()
        {
        }

        public static StringFormat SFCenter
        {
            get
            {
                return GetStringFormat(StringAlignment.Center, StringAlignment.Center);
            }
        }

        public static StringFormat SFLeft
        {
            get
            {
                return GetStringFormat(StringAlignment.Near, StringAlignment.Center);
            }
        }

        public static StringFormat SFRight
        {
            get
            {
                return GetStringFormat(StringAlignment.Far, StringAlignment.Center);
            }
        }

        public static StringFormat GetStringFormat(StringAlignment alignment, StringAlignment lineAlignment)
        {
            var sf = new StringFormat();
            sf.Alignment = alignment;
            sf.LineAlignment = lineAlignment;
            sf.Trimming = StringTrimming.EllipsisCharacter;
            //SfCenter.FormatFlags |= StringFormatFlags.NoWrap;
            //sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Hide;

            // for right-to-left language
            if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
                sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;

            return sf;
        }

        #endregion

        #region Draw Image

        public enum DrawImageMethod
        {
            Stretch,
            Expand,
            RepeatX,
            RepeatY,
            RepeatAll,
            InRange,
        }

        public static void BitBlt(Graphics grf, int dx, int dy, int width, int height, Image img, int sx, int sy)
        {
            if (width <= 0 || height <= 0)
                return;

            grf.DrawImage(img, new Rectangle(dx, dy, width, height), sx, sy, width, height, GraphicsUnit.Pixel);
        }

        public static void StretchBlt(Graphics grf, int dx, int dy, int dWidth, int dHeight, Image img, int sx, int sy, int sWidth, int sHeight)
        {
            if (dWidth <= 0 || dHeight <= 0 || sWidth <= 0 || sHeight <= 0)
                return;

            ImageAttributes ia = new ImageAttributes();
            ia.SetWrapMode(WrapMode.Tile);
            grf.DrawImage(img, new Rectangle(dx, dy, dWidth, dHeight), sx, sy, sWidth, sHeight, GraphicsUnit.Pixel, ia);
        }

        public static bool ExpandImage(Graphics grf, Image imageS, Padding padding, Rectangle rectD, Rectangle rectS)
        {
            if (rectD.Width <= 0 || rectD.Height <= 0 || rectS.Width <= 0 || rectS.Height <= 0)
                return false;

            if (padding.All == 0)
            {
                ImageAttributes ia = new ImageAttributes();
                ia.SetWrapMode(WrapMode.Tile);
                grf.DrawImage(imageS, rectD, rectS.Left, rectS.Top, rectS.Width, rectS.Height, GraphicsUnit.Pixel, ia);
            }
            else if (rectS.Height == rectD.Height) // 等高, 横向扩展
            {
                BitBlt(grf, rectD.X, rectD.Y, padding.Left, rectD.Height, imageS, rectS.X, rectS.Y);
                StretchBlt(grf,
                    rectD.X + padding.Left, rectD.Y, rectD.Width - padding.Horizontal, rectD.Height,
                    imageS,
                    rectS.X + padding.Left, rectS.Y, rectS.Width - padding.Horizontal, rectS.Height);
                BitBlt(grf, rectD.Right - padding.Right, rectD.Y, padding.Right, rectD.Height, imageS, rectS.Right - padding.Right, rectS.Y);
            }
            else if (rectS.Width == rectD.Width) // 等宽, 纵向扩展
            {
                BitBlt(grf, rectD.X, rectD.Y, rectD.Width, padding.Top, imageS, rectS.X, rectS.Y);
                StretchBlt(grf,
                    rectD.X, rectD.Y + padding.Top, rectD.Width, rectD.Height - padding.Vertical,
                    imageS,
                    rectS.X, rectS.Y + padding.Top, rectS.Width, rectS.Height - padding.Vertical);
                BitBlt(grf, rectD.X, rectD.Bottom - padding.Bottom, rectD.Width, padding.Bottom, imageS, rectS.X, rectS.Bottom - padding.Bottom);
            }
            else // 双向扩展
            {
                //left-top
                BitBlt(grf, rectD.X, rectD.Y, padding.Left, padding.Top, imageS, rectS.X, rectS.Y);
                //center-top
                StretchBlt(grf,
                    rectD.X + padding.Left, rectD.Y, rectD.Width - padding.Horizontal, padding.Top,
                    imageS,
                    rectS.X + padding.Left, rectS.Y, Math.Max(1, rectS.Width - padding.Horizontal - 1), padding.Top);
                //right-top
                BitBlt(grf, rectD.Right - padding.Right, rectD.Y, padding.Right, padding.Top, imageS, rectS.Right - padding.Right, rectS.Y);

                //left-middle
                StretchBlt(grf,
                    rectD.X, rectD.Y + padding.Top, padding.Left, rectD.Height - padding.Vertical,
                    imageS,
                    rectS.X, rectS.Y + padding.Top, padding.Left, Math.Max(1, rectS.Height - padding.Vertical - 1));
                //center-middle
                StretchBlt(grf,
                    rectD.X + padding.Left, rectD.Y + padding.Top, rectD.Width - padding.Horizontal, rectD.Height - padding.Vertical,
                    imageS,
                    rectS.X + padding.Left, rectS.Y + padding.Top, Math.Max(1, rectS.Width - padding.Horizontal - 1), Math.Max(1, rectS.Height - padding.Vertical - 1));
                //right-middle
                StretchBlt(grf,
                    rectD.Right - padding.Right, rectD.Y + padding.Top, padding.Right, rectD.Height - padding.Vertical,
                    imageS,
                    rectS.Right - padding.Right, rectS.Y + padding.Top, padding.Right, Math.Max(1, rectS.Height - padding.Vertical - 1));

                //left-bottom
                BitBlt(grf, rectD.X, rectD.Bottom - padding.Bottom, padding.Left, padding.Bottom, imageS, rectS.X, rectS.Bottom - padding.Bottom);
                //center-bottom
                StretchBlt(grf,
                    rectD.X + padding.Left, rectD.Bottom - padding.Bottom, rectD.Width - padding.Horizontal, padding.Bottom,
                    imageS,
                    rectS.X + padding.Left, rectS.Bottom - padding.Bottom, Math.Max(1, rectS.Width - padding.Horizontal - 1), padding.Bottom);
                //right-bottom
                BitBlt(grf, rectD.Right - padding.Right, rectD.Bottom - padding.Bottom, padding.Right, padding.Bottom, imageS, rectS.Right - padding.Right, rectS.Bottom - padding.Bottom);
            }

            return true;
        }

        public static bool ExpandImage(Graphics grf, Image imageS, int padding, Rectangle rectD, Rectangle rectS)
        {
            if (rectD.Width <= 0 || rectD.Height <= 0 || rectS.Width <= 0 || rectS.Height <= 0)
                return false;
            return ExpandImage(grf, imageS, new Padding(padding), rectD, rectS);
        }

        public static bool ExpandImage(ref Bitmap imageD, Image iamgeS, int padding, Rectangle rectD, Rectangle rectS)
        {
            if (imageD == null)
                return false;

            using (Graphics grf = Graphics.FromImage(imageD))
            {
                ExpandImage(grf, iamgeS, new Padding(padding), rectD, rectS);
                grf.Dispose();
            }

            return true;
        }

        public static void DrawImage(Graphics grf, Image image, Rectangle rectD, Rectangle rectS, DrawImageMethod method, Padding? padding)
        {
            switch (method)
            {
                case DrawImageMethod.Expand:
                    ExpandImage(grf, image, padding.HasValue ? padding.Value : Padding.Empty, rectD, rectS);
                    break;
                case DrawImageMethod.RepeatY:
                    DrawImageTiled(grf, image, rectD, DrawImageMethod.RepeatY);
                    break;
                case DrawImageMethod.RepeatX:
                    DrawImageTiled(grf, image, rectD, DrawImageMethod.RepeatX);
                    break;
                case DrawImageMethod.RepeatAll:
                    DrawImageTiled(grf, image, rectD, DrawImageMethod.RepeatAll);
                    break;
                case DrawImageMethod.InRange:
                    DrawImageInRange(grf, image, rectD, rectS);
                    break;
                case DrawImageMethod.Stretch:
                default:
                    grf.DrawImage(image, rectD, rectS.Left, rectS.Top, rectS.Width, rectS.Height, GraphicsUnit.Pixel);
                    break;
            }
        }

        public static void DrawImageTiled(Graphics grf, Image image, Rectangle rect)
        {
            //DrawImageTiled(grf, image, rect, DrawImageMethod.RepeatAll);

            if (grf == null || image == null || rect.Width <= 0 || rect.Height <= 0)
                return;

            Bitmap buffer = new Bitmap(rect.Width, rect.Height);
            Graphics grfBuffer = Graphics.FromImage(buffer);

            int x = rect.X;
            while (x < rect.Right)
            {
                int y = rect.Y;
                while (y < rect.Bottom)
                {
                    grfBuffer.DrawImage(image, new Rectangle(x, y, image.Width, image.Height)
                        , 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);

                    y += image.Height;
                }

                x += image.Width;
            }

            grf.DrawImage(buffer, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel);

            grfBuffer.Dispose();
            buffer.Dispose();
        }

        public static void DrawImageTiled(Graphics grf, Image image, Rectangle rect, DrawImageMethod repeatMode)
        {
            if (grf == null || image == null || rect.Width <= 0 || rect.Height <= 0)
                return;

            using (TextureBrush tb = new TextureBrush(image))
            {
                Rectangle rect_d;
                switch (repeatMode)
                {
                    case DrawImageMethod.RepeatX:
                        rect_d = new Rectangle(rect.Left, rect.Top + (rect.Height - image.Height) / 2, rect.Width, Math.Min(rect.Height, image.Height));
                        break;
                    case DrawImageMethod.RepeatY:
                        rect_d = new Rectangle(rect.Left + (rect.Width - image.Width) / 2, rect.Top, Math.Min(rect.Width, image.Width), rect.Height);
                        break;
                    default:
                        rect_d = rect;
                        break;
                }
                grf.FillRectangle(tb, rect_d);
            }
        }

        public static bool DrawImageInRange(Graphics grf, Image image, Rectangle rect)
        {
            return DrawImageInRange(grf, image, rect, new Rectangle(0, 0, image.Width, image.Height));
        }

        public static bool DrawImageInRange(Graphics grf, Image image, Rectangle rectD, Rectangle rectS)
        {
            if (grf == null || image == null || rectD.Width <= 0 || rectD.Height <= 0)
            {
                return false;
            }

            Rectangle rectImage = GetRectInBounds(rectD, rectS.Width, rectS.Height);
            if (rectImage.Width > 0 && rectImage.Height > 0)
            {
                grf.DrawImage(image, rectImage, rectS.Left, rectS.Top, rectS.Width, rectS.Height, GraphicsUnit.Pixel);
                return true;
            }

            return false;
        }

        public static void DrawImageDisabledInRect(Graphics grf, Image image, Rectangle rect, Color backColor)
        {
            if (grf == null || image == null || rect.Width <= 0 || rect.Height <= 0)
            {
                return;
            }

            Rectangle rectImage = GetRectInBounds(rect, image.Width, image.Height);
            if (rectImage.Width > 0 && rectImage.Height > 0)
            {
                DrawImageDisabled(grf, image, rectImage, new Rectangle(0, 0, image.Width, image.Height), backColor);
            }
        }

        public static void DrawImageDisabled(Graphics grf, Image image, int x, int y, int width, int height, Color background)
        {
            if (grf == null || image == null || width <= 0 || height <= 0)
                return;

            DrawImageDisabled(grf, image, new Rectangle(x, y, width, height)
                , new Rectangle(0, 0, image.Width, image.Height), background);
        }

        public static void DrawImageDisabled(Graphics graphics, Image image, Rectangle dRect, Rectangle sRect, Color background)
        {
            if (disabledImageAttr == null)
            {
                float[][] fs1 = new float[5][];
                fs1[0] = new float[] { 0.21250001F, 0.21250001F, 0.21250001F, 0.0F, 0.0F };
                fs1[1] = new float[] { 0.2577F, 0.2577F, 0.2577F, 0.0F, 0.0F };
                fs1[2] = new float[] { 3.61e-002F, 3.61e-002F, 3.61e-002F, 0.0F, 0.0F };
                float[] fs2 = new float[5];
                fs2[3] = 1.0F;
                fs1[3] = fs2;
                fs1[4] = new float[] { 0.38F, 0.38F, 0.38F, 0.0F, 1.0F };
                ColorMatrix colorMatrix = new ColorMatrix(fs1);
                disabledImageAttr = new ImageAttributes();
                disabledImageAttr.ClearColorKey();
                disabledImageAttr.SetColorMatrix(colorMatrix);
            }

            try
            {
                graphics.DrawImage(image, dRect, sRect.Left, sRect.Top, sRect.Width, sRect.Height, GraphicsUnit.Pixel, disabledImageAttr);
            }
            catch(System.Exception ex)
            {
                Helper.WriteLog(ex);
            }
        }
        #endregion

        #region Win32 GDI & DC

        public static uint GetCOLORREF(Color color)
        {
            return (uint)(color.B << 16 | color.G << 8 | color.R);
        }

        public static int RotateDc(IntPtr dc, int rotateAngle)
        {
            if (dc == IntPtr.Zero || rotateAngle == 0)
                return 0;
            return TransformDc(dc, rotateAngle, 0, 0);
        }

        public static int MoveDc(IntPtr dc, int moveX, int moveY)
        {
            if (dc == IntPtr.Zero || (moveX == 0 && moveY == 0))
                return 0;
            return TransformDc(dc, 0, moveX, moveY);
        }

        public static int TransformDc(IntPtr dc, int rotateAngle, int moveX, int moveY)
        {
            if (dc == IntPtr.Zero)
                return 0;
            int saveId = Gdi32.SaveDC(dc);
            Gdi32.SetGraphicsMode(dc, GraphicsModes.GM_ADVANCED);

            if (rotateAngle != 0)
            {
                double a = -rotateAngle * Math.PI / 180;
                XFORM xform = new XFORM((float)Math.Cos(a), -(float)Math.Sin(a), (float)Math.Sin(a), (float)Math.Cos(a), moveX, moveY);
                Gdi32.SetWorldTransform(dc, xform);
            }
            else
            {
                XFORM xform = new XFORM(0, 0, 0, 0, moveX, moveY);
                Gdi32.SetWorldTransform(dc, xform);
            }

            return saveId;
        }

        #endregion

        #region Image Data & Operate

        public static void TransparentImage(Bitmap bmp, float change)
        {
            AdjustImage(bmp, ColorByte.Alpha, change);
        }

        public static void AdjustImage(Bitmap bmp, ColorByte cb, byte value, bool replace)
        {
            if (bmp == null)
                return;
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            if (replace)
            {
                value = Math.Max((byte)0, Math.Min((byte)255, value));
            }

            int ci = 0;
            switch (cb)
            {
                case ColorByte.Alpha:
                    ci = 3;
                    break;
                case ColorByte.Red:
                    ci = 2;
                    break;
                case ColorByte.Green:
                    ci = 1;
                    break;
                case ColorByte.Blue:
                    ci = 0;
                    break;
            }

            int bytes = bd.Stride * bd.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(bd.Scan0, rgbValues, 0, bytes);

            for (int y = 0; y < bd.Height; y++)
            {
                int index = y * bd.Stride;
                for (int x = 0; x < bd.Stride; x += 4)
                {
                    rgbValues[index + x + ci] = replace ? value : (byte)Math.Min(255, rgbValues[index + x + ci] + value);
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, bd.Scan0, bytes);

            bmp.UnlockBits(bd);
        }

        public static void AdjustImage(Bitmap bmp, ColorByte cb, float change)
        {
            if (change < 0.0f || change > 1.0f)
                throw new ArgumentException();

            if (bmp == null)
                return;
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            int ci = 0;
            switch (cb)
            {
                case ColorByte.Alpha:
                    ci = 3;
                    break;
                case ColorByte.Red:
                    ci = 2;
                    break;
                case ColorByte.Green:
                    ci = 1;
                    break;
                case ColorByte.Blue:
                    ci = 0;
                    break;
            }

            int bytes = bd.Stride * bd.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(bd.Scan0, rgbValues, 0, bytes);

            for (int y = 0; y < bd.Height; y++)
            {
                int index = y * bd.Stride;
                for (int x = 0; x < bd.Stride; x += 4)
                {
                    rgbValues[index + x + ci] = (byte)Math.Min(255, rgbValues[index + x + ci] * change);
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, bd.Scan0, bytes);

            bmp.UnlockBits(bd);
        }

        public static Image GetImageFromButtfer(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return null;
            }

            Image image = null;
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                try
                {
                    image = Image.FromStream(stream);
                }
                finally
                {
                    stream.Close();
                }
            }

            return image;
        }

        public static byte[] GetBufferFromImage(Image image)
        {
            if (image == null)
            {
                return new byte[0];
            }

            byte[] buffer = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    image.Save(stream, ImageFormat.Png);
                    stream.Position = 0;
                    buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                }
                finally
                {
                    stream.Close();
                }
            }

            return buffer;
        }

        public static Image CreateThumbImage(Image image, Size size)
        {
            return CreateThumbImage(image, size.Width, size.Height);
        }

        public static Image CreateThumbImage(Image image, int width, int height)
        {
            if (image == null || width < 0 || height < 0)
            {
                return null;
            }

            try
            {
                Rectangle rect = GetRectInBounds(new Rectangle(0, 0, width, height), image.Width, image.Height);
                Bitmap bmp = new Bitmap(rect.Width, rect.Height);
                if (rect.Width > 0 && rect.Height > 0)
                {
                    using (Graphics grf = Graphics.FromImage(bmp))
                    {
                        grf.DrawImage(image, new Rectangle(0, 0, rect.Width, rect.Height)
                            , 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
                        grf.Dispose();
                    }
                }
                return bmp;
            }
            catch(System.Exception ex)
            {
                Helper.WriteLog(ex);
                return null;
            }
        }

        public static Image SynthesisImage(Image image1, Image image2, ContentAlignment align)
        {
            if (image1 == null || image2 == null)
            {
                return null;
            }

            Size size1 = new Size(image1.Width, image1.Height);
            Size size2 = new Size(image2.Width, image2.Height);
            Rectangle rect2 = new Rectangle(Point.Empty, size2);
            switch (align)
            {
                case ContentAlignment.TopLeft:
                    break;
                case ContentAlignment.TopCenter:
                    rect2.X += (size1.Width - size2.Width) / 2;
                    break;
                case ContentAlignment.TopRight:
                    rect2.X = size1.Width - size2.Width;
                    break;
                case ContentAlignment.MiddleLeft:
                    rect2.Y += (size1.Height - size2.Height) / 2;
                    break;
                case ContentAlignment.MiddleCenter:
                    rect2.X += (size1.Width - size2.Width) / 2;
                    rect2.Y += (size1.Height - size2.Height) / 2;
                    break;
                case ContentAlignment.MiddleRight:
                    rect2.X = size1.Width - size2.Width;
                    rect2.Y += (size1.Height - size2.Height) / 2;
                    break;
                case ContentAlignment.BottomLeft:
                    rect2.Y = size1.Height - size2.Height;
                    break;
                case ContentAlignment.BottomCenter:
                    rect2.X += (size1.Width - size2.Width) / 2;
                    rect2.Y = size1.Height - size2.Height;
                    break;
                case ContentAlignment.BottomRight:
                    rect2.X = size1.Width - size2.Width;
                    rect2.Y = size1.Height - size2.Height;
                    break;
            }

            using (Graphics grf = Graphics.FromImage(image1))
            {
                //grf.FillRectangle(Brushes.Red, rect2);
                //grf.FillRectangle(Brushes.White, rect2);
                //grf.DrawRectangle(Pens.Gray, rect2.X, rect2.Y, rect2.Width - 1, rect2.Height - 1);
                grf.DrawImage(image2, rect2, 0, 0, size2.Width, size2.Height, GraphicsUnit.Pixel);
                grf.Dispose();
            }

            return image1;
        }

        public static Bitmap IconToImage(Icon icon)
        {
            Bitmap bmp = new Bitmap(icon.Width, icon.Height);
            using (Graphics grf = Graphics.FromImage(bmp))
            {
                grf.DrawIcon(icon, new Rectangle(0, 0, bmp.Width, bmp.Height));
            }

            return bmp;
        }

        public static Image CopyImage(Image imageS)
        {
            if (imageS == null)
            {
                throw new ArgumentNullException();
            }

            Image imageD = new Bitmap(imageS.Width, imageS.Height);
            CopyImage(imageS, imageD);
            return imageD;
        }

        public static void CopyImage(Image imageS, Image imageD)
        {
            if (imageS == null || imageD == null)
            {
                throw new ArgumentNullException();
            }

            int w = Math.Min(imageS.Width, imageD.Width);
            int h = Math.Min(imageS.Height, imageD.Height);
            if (w <= 0 || h <= 0)
                return;

            using (Graphics grf = Graphics.FromImage(imageD))
            {
                grf.DrawImage(imageS, new Rectangle(0, 0, w, h), 0, 0, w, h, GraphicsUnit.Pixel);
            }
        }

        public static Bitmap CutImage(Image image, int x, int y, int width, int height)
        {
            if (image == null)
                throw new ArgumentNullException();
            if (width <= 0 || height <= 0)
                throw new ArgumentException();

            Bitmap bmp = new Bitmap(width, height);
            using(var grf = Graphics.FromImage(bmp))
            {
                grf.DrawImage(image,
                    new Rectangle(0, 0, width, height),
                    new Rectangle(x, y, width, height),
                    GraphicsUnit.Pixel);
            }
            return bmp;
        }

        #endregion

        #region Colors

        public static Color GetRandomColor()
        {
            return Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255));
        }

        public static Color GetRandomColor(int min)
        {
            return GetRandomColor(min, 255);
        }

        public static Color GetRandomColor(int min, int max)
        {
            min = Math.Max(0, min);
            int mm = Math.Min(255, max) - min;
            return Color.FromArgb(min + rand.Next(mm), min + rand.Next(mm), min + rand.Next(mm));
        }

        public static Color AdjustColor(Color color, int minS, int maxS, int minL, int maxL)
        {
            int h, l, s;
            RgbToHsl(color, out h, out s, out l);
            s = Math.Min(maxS, Math.Max(minS, s));
            l = Math.Min(maxL, Math.Max(minL, l));
            return HslToRgb(h, s, l);
        }

        public static Color AdjustColorS(Color color, int minS, int maxS)
        {
            int h, l, s;
            RgbToHsl(color, out h, out s, out l);
            s = Math.Min(maxS, Math.Max(minS, s));
            return HslToRgb(h, s, l);
        }

        public static Color AdjustColorSLess(Color color, int sless)
        {
            int h, l, s;
            RgbToHsl(color, out h, out s, out l);
            s = Math.Min(sless, s);
            return HslToRgb(h, s, l);
        }

        public static Color ReduceSaturation(Color color, int diff)
        {
            int h, l, s;
            RgbToHsl(color, out h, out s, out l);
            s = Math.Max(s - diff, 0);
            return HslToRgb(h, s, l);
        }

        public static Color GetLightColor(Color c)
        {
            return GetLightColor(c, 0.3f);
        }

        public static Color GetLightColor(Color c, double change)
        {
            int h, l, s;
            RgbToHsl(c, out h, out s, out l);
            if(l == 0)
                l = Math.Max(0, Math.Min(100, (int)(100 * change)));
            else
                l = Math.Max(0, Math.Min(100, (int)(l * (1 + change))));
            return HslToRgb(h, s, l, c.A);
        }

        public static Color GetDarkColor(Color c)
        {
            return GetDarkColor(c, 0.3f);
        }

        public static Color GetDarkColor(Color c, double change)
        {
            int h, l, s;
            RgbToHsl(c, out h, out s, out l);
            l = Math.Max(0, Math.Min(100, (int)(l * (1 - change))));
            return HslToRgb(h, s, l, c.A);
        }

        public static void RgbToHsl(Color color, out int h, out int s, out int l)
        {
            RgbToHsl(color.R, color.G, color.B, out h, out s, out l);
        }

        public static Color WithoutAlpha(Color color)
        {
            return Color.FromArgb(color.R, color.G, color.B);
        }

        public static void RgbToHsl(byte red, byte green, byte blue, out int h, out int s, out int l)
        {
            byte minval = Math.Min(red, Math.Min(green, blue));
            byte maxval = Math.Max(red, Math.Max(green, blue));

            float mdiff = (float)(maxval - minval);
            float msum = (float)(maxval + minval);

            float hue = 0;
            float luminance = msum / 510.0f;
            float saturation = 0;

            if (maxval == minval)
            {
                saturation = 0.0f;
                hue = 0.0f;
            }
            else
            {
                float rnorm = (maxval - red) / mdiff;
                float gnorm = (maxval - green) / mdiff;
                float bnorm = (maxval - blue) / mdiff;

                saturation = (luminance <= 0.5f) ? (mdiff / msum) : (mdiff / (510.0f - msum));

                if (red == maxval)
                {
                    hue = 60.0f * (6.0f + bnorm - gnorm);
                }
                if (green == maxval)
                {
                    hue = 60.0f * (2.0f + rnorm - bnorm);
                }
                if (blue == maxval)
                {
                    hue = 60.0f * (4.0f + gnorm - rnorm);
                }
                if (hue > 360.0f)
                {
                    hue = hue - 360.0f;
                }
            }

            h = (int)hue;
            l = (int)(luminance * 100);
            s = (int)(saturation * 100);
        }

        public static void HslToRgb(int h, int s, int l, out byte red, out byte green, out byte blue)
        {
            double hue = h;
            double saturation = s / 100.0f;
            double luminance = l / 100.0f;

            if (saturation == 0.0)
            {
                red = (byte)(luminance * 255.0F);
                green = red;
                blue = red;
            }
            else
            {
                double rm1;
                double rm2;

                if (luminance <= 0.5f)
                {
                    rm2 = luminance + luminance * saturation;
                }
                else
                {
                    rm2 = luminance + saturation - luminance * saturation;
                }
                rm1 = 2.0f * luminance - rm2;
                red = ToRGB1(rm1, rm2, hue + 120.0f);
                green = ToRGB1(rm1, rm2, hue);
                blue = ToRGB1(rm1, rm2, hue - 120.0f);
            }

        }

        private static byte ToRGB1(double rm1, double rm2, double rh)
        {
            if (rh > 360.0f)
            {
                rh -= 360.0f;
            }
            else if (rh < 0.0f)
            {
                rh += 360.0f;
            }

            if (rh < 60.0f)
            {
                rm1 = rm1 + (rm2 - rm1) * rh / 60.0f;
            }
            else if (rh < 180.0f)
            {
                rm1 = rm2;
            }
            else if (rh < 240.0f)
            {
                rm1 = rm1 + (rm2 - rm1) * (240.0f - rh) / 60.0f;
            }

            return (byte)(rm1 * 255);
        }

        public static Color HslToRgb(int h, int s, int l)
        {
            byte r, g, b;
            HslToRgb(h, s, l, out r, out g, out b);
            return Color.FromArgb(r, g, b);
        }

        public static Color HslToRgb(int h, int s, int l, int alpha)
        {
            byte r, g, b;
            HslToRgb(h, s, l, out r, out g, out b);
            return Color.FromArgb(alpha, r, g, b);
        }

        public static Color FarthestColor(Color source, Color color1, Color color2)
        {
            int h, l1, l2, l3, s;
            RgbToHsl(source, out h, out s, out l1);
            RgbToHsl(color1, out h, out s, out l2);
            RgbToHsl(color2, out h, out s, out l3);

            if (Math.Abs(l1 - l2) > Math.Abs(l1 - l3))
                return color1;
            else
                return color2;
        }

        public static Color NearestColor(Color source, Color color1, Color color2)
        {
            int h, l1, l2, l3, s;
            RgbToHsl(source, out h, out s, out l1);
            RgbToHsl(color1, out h, out s, out l2);
            RgbToHsl(color2, out h, out s, out l3);

            if (Math.Abs(l1 - l2) < Math.Abs(l1 - l3))
                return color1;
            else
                return color2;
        }

        public static Color FarthestColor(Color source, Color color1, Color color2, int minDiff)
        {
            int h, l, s;
            int h1, l1, s1;
            int h2, l2, s2;
            RgbToHsl(source, out h, out s, out l);
            RgbToHsl(color1, out h1, out s1, out l1);
            RgbToHsl(color2, out h2, out s2, out l2);

            int ld1 = Math.Abs(l - l1);
            int ld2 = Math.Abs(l - l2);
            if (ld1 > minDiff && ld2 > minDiff)
            {
                if (Math.Abs(l - l1) > Math.Abs(l - l2))
                    return color1;
                else
                    return color2;
            }
            else if (ld1 > minDiff)
            {
                return color1;
            }
            else if (ld2 > minDiff)
            {
                return color2;
            }
            else
            {
                l1 = (l > l1) ? Math.Max(0, l - minDiff) : Math.Min(100, l + minDiff);
                l2 = (l > l2) ? Math.Max(0, l - minDiff) : Math.Min(100, l + minDiff);
                if (Math.Abs(l - l1) > Math.Abs(l - l2))
                    return HslToRgb(h1, s1, l1);
                else
                    return HslToRgb(h2, s2, l2);
            }
        }

        public static Color Darker(Color color1, Color color2)
        {
            int h, s, l1, l2;
            RgbToHsl(color1, out h, out s, out l1);
            RgbToHsl(color2, out h, out s, out l2);
            if (l1 > l2)
                return color2;
            else
                return color1;
        }
        #endregion

        public static GraphicsPath GetRoundRectangle(Rectangle rect, int rounds)
        {
            return GetRoundRectangle(rect, rounds, rounds, rounds, rounds);
        }

        public static GraphicsPath GetRoundRectangle(Rectangle rect, int leftTop, int rightTop, int rightBottom, int leftBottom)
        {
            // init params
            int min = Math.Min(rect.Width / 2, rect.Height / 2) - 1;
            leftTop = Math.Min(min, leftTop);
            rightTop = Math.Min(min, rightTop);
            rightBottom = Math.Min(min, rightBottom);
            leftBottom = Math.Min(min, leftBottom);

            //
            GraphicsPath gp = new GraphicsPath();

            //
            if (leftTop > 0 || rightTop > 0 || rightBottom > 0 || leftBottom > 0)
            {
                if (leftTop > 0)
                    gp.AddArc(new Rectangle(rect.Left, rect.Top, leftTop * 2, leftTop * 2), 180, 90);
                else
                    gp.AddLine(rect.Left, rect.Bottom - leftBottom - 1, rect.Left, rect.Top);

                if (rightTop > 0)
                    gp.AddArc(new Rectangle(rect.Right - rightTop * 2 - 1, rect.Top, rightTop * 2, rightTop * 2), 270, 90);
                else
                    gp.AddLine(rect.Left + leftTop, rect.Top, rect.Right - 1, rect.Top);

                if (rightBottom > 0)
                    gp.AddArc(new Rectangle(rect.Right - rightBottom * 2 - 1, rect.Bottom - rightBottom * 2 - 1, rightBottom * 2, rightBottom * 2), 0, 90);
                else
                    gp.AddLine(rect.Right - 1, rect.Top + rightTop, rect.Right - 1, rect.Bottom - 1);

                if (leftBottom > 0)
                    gp.AddArc(new Rectangle(rect.Left, rect.Bottom - leftBottom * 2 - 1, leftBottom * 2, leftBottom * 2), 90, 90);
                else
                    gp.AddLine(rect.Right - rightBottom - 1, rect.Bottom - 1, rect.Left, rect.Bottom - 1);

                gp.CloseFigure();
            }
            else
            {
                gp.AddRectangle(rect);
            }

            return gp;
        }

        private static PointF[] GetTrianglePoints(Rectangle rect, Vector4 vector)
        {
            PointF[] pts = new PointF[3];
            switch (vector)
            {
                case Vector4.Left:
                    pts[0] = new PointF(rect.Right, rect.Top);
                    pts[1] = new PointF(rect.Right, rect.Bottom);
                    pts[2] = new PointF(rect.Left, rect.Top + rect.Height / 2.0f);
                    break;
                case Vector4.Top:
                    pts[0] = new PointF(rect.Left, rect.Bottom);
                    pts[1] = new PointF(rect.Right, rect.Bottom);
                    pts[2] = new PointF(rect.Left + rect.Width / 2.0f, rect.Top);
                    break;
                case Vector4.Right:
                    pts[0] = new PointF(rect.Left, rect.Top);
                    pts[1] = new PointF(rect.Left, rect.Bottom);
                    pts[2] = new PointF(rect.Right, rect.Top + rect.Height / 2.0f);
                    break;
                case Vector4.Bottom:
                    pts[0] = new PointF(rect.Left, rect.Top);
                    pts[1] = new PointF(rect.Right, rect.Top);
                    pts[2] = new PointF(rect.Left + rect.Width / 2.0f, rect.Bottom);
                    break;
            }

            return pts;
        }

        public static Bitmap GetTriangleImage(Color brushColor, Size size, Vector4 vector)
        {
            Bitmap bmp = new Bitmap(size.Width, size.Height);
            using (Graphics grf = Graphics.FromImage(bmp))
            {
                DrawTriangle(grf, brushColor, new Rectangle(0, 0, size.Width, size.Height), vector);
            }

            return bmp;
        }

        public static void DrawTriangle(Graphics grf, Color brushColor, Rectangle rect, Vector4 vector)
        {
            PointF[] pts = GetTrianglePoints(rect, vector);
            grf.FillPolygon(new SolidBrush(brushColor), pts);
        }

        public static void DrawReversibleFrame(Rectangle rectangle, Color backColor)
        {
            IntPtr dc = User32.GetDC(IntPtr.Zero);
            if (dc == IntPtr.Zero)
                return;

            IntPtr brush = Gdi32.SelectObject(dc, Gdi32.CreateSolidBrush(GetCOLORREF(backColor)));
            IntPtr pen = Gdi32.SelectObject(dc, Gdi32.GetStockObject(StockObjects.NULL_PEN));
            RopModes rp = Gdi32.SetROP2(dc, RopModes.R2_NOT);
            Gdi32.Rectangle(dc, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);

            Gdi32.DeleteObject(Gdi32.SelectObject(dc, brush));
            User32.ReleaseDC(IntPtr.Zero, dc);
        }

        public static void DrawReversibleLine(int x1, int y1, int x2, int y2, Color colorLine)
        {
            //ControlPaint.DrawReversibleLine(new Point(x1, y1), new Point(x2, y2), colorLine);
            IntPtr dc = User32.GetDC(IntPtr.Zero);
            if (dc == IntPtr.Zero)
                return;

            Color c = Color.FromArgb(255 - colorLine.R, 255 - colorLine.G, 255 - colorLine.B);
            IntPtr pen = Gdi32.SelectObject(dc, Gdi32.CreatePen(PenStyles.PS_SOLID, 1, Gdi32.RGB(c)));
            RopModes rp = Gdi32.SetROP2(dc, RopModes.R2_XORPEN);

            Gdi32.MoveToEx(dc, x1, y1, IntPtr.Zero);
            Gdi32.LineTo(dc, x2, y2);

            Gdi32.DeleteObject(Gdi32.SelectObject(dc, pen));
            User32.ReleaseDC(IntPtr.Zero, dc);
        }

        public static Rectangle GetRectInBounds(Rectangle bounds, int width, int height)
        {
            int w;
            int h;

            if (width <= bounds.Width && height <= bounds.Height)
            {
                w = width;
                h = height;
            }
            else
            {
                decimal r = Math.Min((decimal)bounds.Width / width, (decimal)bounds.Height / height);
                w = (int)Math.Ceiling(width * r);
                h = (int)Math.Ceiling(height * r);
            }

            return new Rectangle(bounds.Left + (bounds.Width - w) / 2
                , bounds.Top + (bounds.Height - h) / 2, w, h);
        }

        public static void DrawString(Graphics grf, string text, Font font, Brush brush, Rectangle rect, StringFormat stringFormat, Orientation orientation)
        {
            GraphicsState gs = null;
            Rectangle rect_text = rect;
            if (orientation == Orientation.Vertical)
            {
                gs = grf.Save();
                grf.RotateTransform(90);
                rect_text = new Rectangle(rect.Y, -rect.X - rect.Width, rect.Height, rect.Width);
                //e.Graphics.DrawRectangle(Pens.Blue, rect_text);
            }
            grf.DrawString(text, font, brush, rect_text, stringFormat);

            if (orientation == Orientation.Vertical)
            {
                grf.Restore(gs);
            }
        }
        
        public static void SetHighQualityRender(Graphics graphics)
        {
            if (graphics != null)
            {
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;//.AntiAlias;// GridFit;
                //graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            }
        }

        public static void ClearHighQualityFlag(Graphics graphics)
        {
            if (graphics != null)
            {
                graphics.SmoothingMode = SmoothingMode.Default;
                graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
                //graphics.PixelOffsetMode = PixelOffsetMode.Default;
            }
        }

        public static void FillDot(Graphics grf, Brush brush, Point pt, int r)
        {
            grf.FillEllipse(brush, new Rectangle(pt.X - r, pt.Y - r, r * 2, r * 2));
        }

        public static void DrawDot(Graphics grf, Pen pen, Point pt, int r)
        {
            grf.DrawEllipse(pen, new Rectangle(pt.X - r, pt.Y - r, r * 2, r * 2));
        }

        public static void DrawButton(Graphics graphics, Rectangle rect, UIControlStatus ucs, Color color)
        {
            graphics.FillRectangle(new SolidBrush(color), rect);

            rect.Width -= 1;
            rect.Height -= 1;
            switch (ucs)
            {
                case UIControlStatus.Hover:
                    graphics.DrawLines(new Pen(GetLightColor(color)), new Point[] { new Point(rect.Left, rect.Bottom), new Point(rect.Left, rect.Top), new Point(rect.Right, rect.Top) });
                    graphics.DrawLines(new Pen(GetDarkColor(color, 0.5)), new Point[] { new Point(rect.Left, rect.Bottom), new Point(rect.Right, rect.Bottom), new Point(rect.Right, rect.Top) });
                    graphics.DrawLines(new Pen(GetDarkColor(color)), new Point[] { new Point(rect.Left - 2, rect.Bottom - 1), new Point(rect.Right - 1, rect.Bottom - 1), new Point(rect.Right - 1, rect.Top - 2) });
                    break;
                case UIControlStatus.Selected:
                    graphics.DrawLines(new Pen(GetDarkColor(color)), new Point[] { new Point(rect.Left, rect.Bottom), new Point(rect.Left, rect.Top), new Point(rect.Right, rect.Top) });
                    graphics.DrawLines(new Pen(GetLightColor(color)), new Point[] { new Point(rect.Left, rect.Bottom), new Point(rect.Right, rect.Bottom), new Point(rect.Right, rect.Top) });
                    break;
                default:
                    graphics.DrawLines(new Pen(GetLightColor(color)), new Point[] { new Point(rect.Left, rect.Bottom), new Point(rect.Left, rect.Top), new Point(rect.Right, rect.Top) });
                    graphics.DrawLines(new Pen(GetDarkColor(color)), new Point[] { new Point(rect.Left, rect.Bottom), new Point(rect.Right, rect.Bottom), new Point(rect.Right, rect.Top) });
                    break;
            }
        }

        public static void DrawControlSelection(Graphics graphics, Rectangle rect, Color color)
        {
            graphics.FillRectangle(Brushes.White, rect);
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, color)), rect);
            graphics.DrawRectangle(new Pen(color), rect);
        }

        public static void DrawHorizontalGradientLine(Graphics grf, int x1, int y, int x2, Color color)
        {
            Color c = PaintHelper.GetDarkColor(color, 0.1);
            Rectangle rect = new Rectangle(x1, y, x2 - x1, 1);
            LinearGradientBrush brush = new LinearGradientBrush(rect, c, c, 0.0f);
            ColorBlend cb = new ColorBlend(4);
            cb.Colors = new Color[] { Color.FromArgb(0, c), c, c, Color.FromArgb(0, c) };
            cb.Positions = new float[] { 0.0f, 0.2f, 0.8f, 1.0f };
            brush.InterpolationColors = cb;
            grf.FillRectangle(brush, rect);

            c = PaintHelper.GetLightColor(color, 0.1);
            cb.Colors = new Color[] { Color.FromArgb(0, c), c, c, Color.FromArgb(0, c) };
            brush.InterpolationColors = cb;
            rect.Y += 1;
            grf.FillRectangle(brush, rect);

            //grf.DrawLine(new Pen(PT.GetDarkColor(color)), x1, y, x2, y);
            //grf.DrawLine(new Pen(PT.GetLightColor(color)), x1, y + 1, x2, y + 1);
        }

        public static void DrawHorizontal3DLine(Graphics grf, int x1, int y, int x2, Color color)
        {
            grf.DrawLine(new Pen(PaintHelper.GetDarkColor(color)), x1, y, x2, y);
            grf.DrawLine(new Pen(PaintHelper.GetLightColor(color)), x1, y + 1, x2, y + 1);
        }

        public static void DrawVertical3DLine(Graphics grf, int x, int y1, int y2, Color color)
        {
            grf.DrawLine(new Pen(PaintHelper.GetDarkColor(color)), x, y1, x, y2);
            grf.DrawLine(new Pen(PaintHelper.GetLightColor(color)), x + 1, y1, x + 1, y2);
        }

        public static Point CenterPoint(Rectangle rect)
        {
            return new Point(rect.Left + rect.Width / 2, rect.Y + rect.Height / 2);
        }

        public static Rectangle Zoom(Rectangle rect, float zoom)
        {
            rect.X = (int)Math.Round(rect.Left * zoom);
            rect.Y = (int)Math.Round(rect.Top * zoom);
            rect.Width = Math.Max(1, (int)Math.Round(rect.Width * zoom));
            rect.Height = Math.Max(1, (int)Math.Round(rect.Height * zoom));

            return rect;
        }

        public static Point Zoom(Point point, float zoom)
        {
            point.X = (int)Math.Round(point.X * zoom);
            point.Y = (int)Math.Round(point.Y * zoom);
            return point;
        }

        public static Size Zoom(Size size, float zoom)
        {
            size.Width = (int)Math.Round(size.Width * zoom);
            size.Height = (int)Math.Round(size.Height * zoom);
            return size;
        }

        public static Padding Zoom(Padding padding, float zoom)
        {
            return new Padding(
                (int)Math.Round(padding.Left * zoom),
                (int)Math.Round(padding.Top * zoom),
                (int)Math.Round(padding.Right * zoom),
                (int)Math.Round(padding.Bottom * zoom));
        }

        public static Rectangle DeZoom(Rectangle rect, float zoom)
        {
            if (zoom <= 0)
            {
                throw new ArgumentOutOfRangeException("zoom");
            }
            else if (zoom == 1)
            {
                return rect;
            }
            else
            {
                rect.X = (int)Math.Round(rect.Left / zoom);
                rect.Y = (int)Math.Round(rect.Top / zoom);
                rect.Width = Math.Max(1, (int)Math.Round(rect.Width / zoom));
                rect.Height = Math.Max(1, (int)Math.Round(rect.Height / zoom));

                return rect;
            }
        }

        public static Point DeZoom(Point point, float zoom)
        {
            if (zoom <= 0)
            {
                throw new ArgumentOutOfRangeException("zoom");
            }
            else if (zoom == 1)
            {
                return point;
            }
            else
            {
                point.X = (int)Math.Round(point.X / zoom);
                point.Y = (int)Math.Round(point.Y / zoom);
                return point;
            }
        }

        public static StringFormat TranslateContentAlignment(ContentAlignment align)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = TranslateAlignment(align);
            sf.LineAlignment = TranslateLineAlignment(align);

            // for right-to-left language
            if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
                SFCenter.FormatFlags |= StringFormatFlags.DirectionRightToLeft;

            return sf;
        }

        public static StringAlignment TranslateLineAlignment(ContentAlignment alignment)
        {
            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.TopCenter:
                case ContentAlignment.TopRight:
                    return StringAlignment.Near;
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleRight:
                    return StringAlignment.Center;
                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomRight:
                    return StringAlignment.Far;
                default:
                    throw new ArgumentException();
            }
        }

        public static StringAlignment TranslateAlignment(ContentAlignment alignment)
        {
            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    return StringAlignment.Near;
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    return StringAlignment.Center;
                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    return StringAlignment.Far;
                default:
                    throw new ArgumentException();
            }
        }

        public static Image AdjustImageColor(Image image, Color fromColor, Color toColor)
        {
            float[] tran = new float[] {
                (toColor.R - fromColor.R)/255.0f
                ,(toColor.G - fromColor.G)/255.0f
                ,(toColor.B - fromColor.B)/255.0f
                ,(toColor.A - fromColor.A)/255.0f};

            ImageAttributes imageAttributes = new ImageAttributes();
            float[][] colorMatrixElements = { 
               new float[] {1, 0, 0, 0, 0},
               new float[] {0, 1, 0, 0, 0},
               new float[] {0, 0, 1, 0, 0},
               new float[] {0, 0, 0, 1, 0},
               new float[] {tran[0], tran[1], tran[2], tran[3], 1}};

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(
               colorMatrix,
               ColorMatrixFlag.Default,
               ColorAdjustType.Bitmap);

            Bitmap bmp = new Bitmap(image.Width, image.Height);
            using (Graphics grf = Graphics.FromImage(bmp))
            {
                grf.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
                grf.Dispose();
            }

            return bmp;
        }

        public static Bitmap GetImage(Image image, int width, int height, DrawImageMethod method)
        {
            if (image == null)
                throw new ArgumentNullException();

            if (width <= 0 || height <= 0)
                throw new ArithmeticException();

            Bitmap bmp = new Bitmap(width, height);
            using (Graphics grf = Graphics.FromImage(bmp))
            {
                DrawImage(grf, image, new Rectangle(0, 0, bmp.Width, bmp.Height), new Rectangle(0, 0, image.Width, image.Height), method, null);
            }

            return bmp;
        }

        public static int GetAngle(Point pts, Point ptd)
        {
            int angle = 0;

            if (pts != ptd)
            {
                if (pts.X == ptd.X)
                {
                    angle = (pts.Y > ptd.Y) ? 270 : 90;
                }
                else if (pts.Y == ptd.Y)
                {
                    angle = (pts.X > ptd.X) ? 180 : 0;
                }
                else
                {
                    angle = (int)Math.Floor(Math.Atan((double)Math.Abs(ptd.Y - pts.Y) / Math.Abs(ptd.X - pts.X)) * 180 / Math.PI);
                    if (ptd.X < pts.X)
                    {
                        if (ptd.Y < pts.Y)
                            angle += 180;
                        else
                            angle = 180 - angle;
                    }
                    else
                    {
                        if (ptd.Y < pts.Y)
                        {
                            angle = 360 - angle;
                        }
                    }
                }
            }

            return angle;
        }

        public static int GetDistance(Point pts, Point ptd)
        {
            if (pts == ptd)
                return 0;
            else if (pts.X == ptd.X)
                return Math.Abs(pts.Y - ptd.Y);
            else if (pts.Y == ptd.Y)
                return Math.Abs(pts.X - ptd.X);
            else
                return (int)Math.Ceiling(Math.Sqrt(Math.Pow(Math.Abs(pts.X - ptd.X), 2) + Math.Pow(Math.Abs(pts.Y - ptd.Y), 2)));
        }

        public static Rectangle GetRectangle(Point pt1, Point pt2)
        {
            return new Rectangle(
                Math.Min(pt1.X, pt2.X),
                Math.Min(pt1.Y, pt2.Y),
                Math.Abs(pt1.X - pt2.X),
                Math.Abs(pt1.Y - pt2.Y));
        }

        public static RectangleF GetRectangle(PointF pt1, PointF pt2)
        {
            return new RectangleF(
                Math.Min(pt1.X, pt2.X),
                Math.Min(pt1.Y, pt2.Y),
                Math.Abs(pt1.X - pt2.X),
                Math.Abs(pt1.Y - pt2.Y));
        }

        public static void DrawHoverBackground(Graphics graphics, Rectangle rect, Color color)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            Rectangle rect2 = rect;
            rect2.Height = rect.Height / 2;
            graphics.FillRectangle(new LinearGradientBrush(rect2, PaintHelper.GetLightColor(color, .1f), color, 90.0f), rect2);
            rect2.Y = rect2.Bottom;
            rect2.Height = rect.Height - rect2.Height;
            graphics.FillRectangle(new LinearGradientBrush(rect2, PaintHelper.GetDarkColor(color, .1f), color, 90.0f), rect2);

            GraphicsPath path = PaintHelper.GetRoundRectangle(rect, 2);
            graphics.DrawPath(new Pen(PaintHelper.GetDarkColor(color, .2f)), path);
            rect.Inflate(-1, -1);
            path = PaintHelper.GetRoundRectangle(rect, 2);
            graphics.DrawPath(new Pen(PaintHelper.GetLightColor(color)), path);
        }

        public static void DrawHoverBackground(IGraphics graphics, Rectangle rect, Color color)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
                return;
            graphics.FillRectangle(graphics.SolidBrush(color), rect);

            /*Rectangle rect2 = rect;
            rect2.Height = rect.Height / 2;
            graphics.FillRectangle(graphics.LinearGradientBrush(rect2, PaintHelper.GetLightColor(color, .1f), color, LinearGradientMode.Vertical), rect2);
            rect2.Y = rect2.Bottom;
            rect2.Height = rect.Height - rect2.Height;
            graphics.FillRectangle(graphics.LinearGradientBrush(rect2, PaintHelper.GetDarkColor(color, .1f), color, LinearGradientMode.Vertical), rect2);

            //GraphicsPath path = PaintHelper.GetRoundRectangle(rect, 2);
            //graphics.DrawPath(graphics.Pen(PaintHelper.GetDarkColor(color, .2f)), path);
            graphics.DrawRoundRectangle(graphics.Pen(PaintHelper.GetDarkColor(color, .2f)), rect, 2);
            rect.Inflate(-1, -1);
            //path = PaintHelper.GetRoundRectangle(rect, 2);
            //graphics.DrawPath(graphics.Pen(PaintHelper.GetLightColor(color)), path);
            graphics.DrawRoundRectangle(graphics.Pen(PaintHelper.GetLightColor(color, .2f)), rect, 2);*/
        }

        public static void DrawHoverBackgroundFlat(Graphics graphics, Rectangle rect, Color color)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            graphics.FillRectangle(new SolidBrush(color), rect);
        }

        public static PointF GetPointOnLine(Point startPoint, Point endPoint, float t)
        {
            return BezierHelper.GetPointF(startPoint, endPoint, t);
        }

        public static Point? GetPointForLineIntersectLine(Point start1, Point end1, Point start2, Point end2)
        {
            var A1 = end1.Y - start1.Y;
            var B1 = start1.X - end1.X;
            var C1 = A1 * start1.X + B1 * start1.Y;

            var A2 = end2.Y - start2.Y;
            var B2 = start2.X - end2.X;
            var C2 = A2 * start2.X + B2 * start2.Y;

            double det = A1 * B2 - A2 * B1;
            if (det == 0)
            {
                //Lines are parallel
                return null;
            }
            else
            {
                double x = (B2 * C1 - B1 * C2) / det;
                double y = (A1 * C2 - A2 * C1) / det;

                if (x > Math.Min(Math.Max(start1.X, end1.X), Math.Max(start2.X, end2.X))
                    || x < Math.Max(Math.Min(start1.X, end1.X), Math.Min(start2.X, end2.X))
                    || y > Math.Min(Math.Max(start1.Y, end1.Y), Math.Max(start2.Y, end2.Y))
                    || y < Math.Max(Math.Min(start1.Y, end1.Y), Math.Min(start2.Y, end2.Y)))
                    return null;

                return new Point((int)Math.Round(x), (int)Math.Round(y));
            }
        }

        public static Point? GetPointForLineIntersectRectangle(Rectangle rect, Point point)
        {
            var cp = CenterPoint(rect);
            Point? r = GetPointForLineIntersectLine(new Point(rect.X, rect.Y), new Point(rect.Right, rect.Y), cp, point);
            if (!r.HasValue)
                r = GetPointForLineIntersectLine(new Point(rect.Right, rect.Y), new Point(rect.Right, rect.Bottom), cp, point);
            if (!r.HasValue)
                r = GetPointForLineIntersectLine(new Point(rect.X, rect.Bottom), new Point(rect.Right, rect.Bottom), cp, point);
            if (!r.HasValue)
                r = GetPointForLineIntersectLine(new Point(rect.X, rect.Y), new Point(rect.X, rect.Bottom), cp, point);

            return r;            
        }

        public static Point? GetPointForPointIntersectEllipse(Rectangle rect, Point point)
        {
            if (TestPointInEllipse(point, rect))
                return null;

            Point pt1;
            Point pt2;

            GetPointsForPointIntersectEllipse(
                new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2),
                rect.Size, point, out pt1, out pt2);
            return pt1;
        }

        public static void GetPointsForPointIntersectEllipse(Point originPoint, Size size, Point startPoint,
            out Point startIntersectPoint, out Point endIntersectPoint)
        {
            startIntersectPoint = Point.Empty;
            endIntersectPoint = Point.Empty;

            long a = size.Width / 2;
            long b = size.Height / 2;
            long x0 = startPoint.X - originPoint.X;
            long y0 = startPoint.Y - originPoint.Y;

            var d = (a * b) / Math.Sqrt((a * a) * (y0 * y0) + (b * b) * (x0 * x0));

            int xx = (int)Math.Round(d * x0);
            int yy = (int)Math.Round(d * y0);
            startIntersectPoint = new Point(xx + originPoint.X, yy + originPoint.Y);
            endIntersectPoint = new Point(-xx + originPoint.X, -yy + originPoint.Y);
        }

        public static bool GetPointsForLineIntersectEllipse(Point originPoint, Size size, Point startPoint, Point endPoint,
            out Point? startIntersectPoint, out Point? endIntersectPoint)
        {
            // Intersection points not necessarily bounded to the line segment.
            var intersectPoint1 = Point.Empty;
            var intersectPoint2 = Point.Empty;

            bool result = true;
            // intersection = Point.Zero;

            // Used for Quadratic equation
            double aa = 0.0;
            double bb = 0.0;
            double cc = 0.0;
            double m = 0.0;

            // Readability enhancement
            double a = size.Width / 2;
            double b = size.Height / 2;

            // Non Vertical line
            if (startPoint.X != endPoint.X)
            {
                var vector = new PointF(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
                m = vector.Y / vector.X;
                double c = startPoint.Y - m * startPoint.X;

                aa = b * b + a * a * m * m;
                bb = 2 * a * a * c * m - 2 * a * a * originPoint.Y * m - 2 * originPoint.X * b * b;
                cc = b * b * originPoint.X * originPoint.X + a * a * c * c - 2 * a * a * originPoint.Y * c + a * a * originPoint.Y * originPoint.Y - a * a * b * b;
            }
            else //Vertical Line
            {
                aa = a * a;
                bb = -2.0f * originPoint.Y * a * a;
                cc = -a * a * b * b + b * b * (startPoint.X - originPoint.X) * (startPoint.X - originPoint.X);
            }

            double d = bb * bb - 4 * aa * cc;

            // If the determinant in greater than 0 we have intersections.
            if (d > 0.0f)
            {
                if (startPoint.X != endPoint.X)
                {
                    var x1 = (-bb + Math.Sqrt(d)) / (2 * aa);
                    var y1 = startPoint.Y + m * (x1 - startPoint.X);
                    intersectPoint1 = new Point((int)Math.Round(x1), (int)Math.Round(y1));

                    var x2 = (-bb - Math.Sqrt(d)) / (2 * aa);
                    var y2 = startPoint.Y + m * (x2 - startPoint.X);
                    intersectPoint2 = new Point((int)Math.Round(x2), (int)Math.Round(y2));
                }
                else
                {
                    var y1 = (-bb + Math.Sqrt(d)) / (2 * aa);
                    intersectPoint1 = new Point(startPoint.X, (int)Math.Round(y1));

                    var y2 = (-bb - Math.Sqrt(d)) / (2 * aa);
                    intersectPoint2 = new Point(startPoint.X, (int)Math.Round(y2));
                }
            }
            else
            {
                result = false;
            }

            if (result)
            {
                startIntersectPoint = intersectPoint1;
                endIntersectPoint = intersectPoint2;
            }
            else
            {
                startIntersectPoint = null;
                endIntersectPoint = null;
            }

            return result;
        }

        public static bool TestPointInEllipse(Point point, Rectangle ellipseRange)
        {
            return TestPointInEllipse(point,
                new Point(ellipseRange.X + ellipseRange.Width / 2, ellipseRange.Y + ellipseRange.Height / 2),
                ellipseRange.Size);
        }

        public static bool TestPointInEllipse(Point point, Point originPoint, Size size)
        {
            double a = size.Width / 2;
            double b = size.Height / 2;
            if (a <= 0 || b <= 0)
                return false;

            double x2 = (point.X - originPoint.X) * (point.X - originPoint.X);
            double y2 = (point.Y - originPoint.Y) * (point.Y - originPoint.Y);
            return x2 / (a * a) + y2 / (b * b) <= 1;
        }

        public static Region GetLineUpdateRegion(Point startPoint, Point endPoint, int width = 1, int samples = 10)
        {
            return BezierHelper.GetBezierUpdateRegion(startPoint, endPoint, width, samples);
        }

        public static void DrawTransparentBackground(Graphics graphics, Rectangle rectangle)
        {
            var bmp = new Bitmap(20, 20);
            using (var grf = Graphics.FromImage(bmp))
            {
                grf.Clear(Color.White);
                grf.FillRectangle(Brushes.Gainsboro, 0, 0, 10, 10);
                grf.FillRectangle(Brushes.Gainsboro, 10, 10, 10, 10);
            }
            var brush = new TextureBrush(bmp);
            graphics.FillRectangle(brush, rectangle);
        }
    }
}
