using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Blumind.Controls
{
    partial class PaintHelper
    {
        public class ConvMatrix
        {
            public int TopLeft = 0, TopMid = 0, TopRight = 0;
            public int MidLeft = 0, Pixel = 1, MidRight = 0;
            public int BottomLeft = 0, BottomMid = 0, BottomRight = 0;
            public int Factor = 1;
            public int Offset = 0;

            public void SetAll(int nVal)
            {
                TopLeft = TopMid = TopRight = MidLeft = Pixel = MidRight = BottomLeft = BottomMid = BottomRight = nVal;
            }
        }

        public static class BitmapFilter
        {
            public static bool Invert(Bitmap bitmap)
            {
                // GDI+ still lies to us - the return format is BGR, NOT RGB.
                BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;

                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;

                    int nOffset = stride - bitmap.Width * 3;
                    int nWidth = bitmap.Width * 3;

                    for (int y = 0; y < bitmap.Height; ++y)
                    {
                        for (int x = 0; x < nWidth; ++x)
                        {
                            p[0] = (byte)(255 - p[0]);
                            ++p;
                        }
                        p += nOffset;
                    }
                }

                bitmap.UnlockBits(bmData);

                return true;
            }

            public static bool GrayScale(Bitmap bitmap)
            {
                // GDI+ still lies to us - the return format is BGR, NOT RGB.
                BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;

                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;

                    int nOffset = stride - bitmap.Width * 3;

                    byte red, green, blue;

                    for (int y = 0; y < bitmap.Height; ++y)
                    {
                        for (int x = 0; x < bitmap.Width; ++x)
                        {
                            blue = p[0];
                            green = p[1];
                            red = p[2];

                            p[0] = p[1] = p[2] = (byte)(.299 * red + .587 * green + .114 * blue);

                            p += 3;
                        }
                        p += nOffset;
                    }
                }

                bitmap.UnlockBits(bmData);

                return true;
            }

            public static bool Brightness(Bitmap bitmap, int nBrightness)
            {
                if (nBrightness < -255 || nBrightness > 255)
                    return false;

                // GDI+ still lies to us - the return format is BGR, NOT RGB.
                BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;

                int nVal = 0;

                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;

                    int nOffset = stride - bitmap.Width * 3;
                    int nWidth = bitmap.Width * 3;

                    for (int y = 0; y < bitmap.Height; ++y)
                    {
                        for (int x = 0; x < nWidth; ++x)
                        {
                            nVal = (int)(p[0] + nBrightness);

                            if (nVal < 0) nVal = 0;
                            if (nVal > 255) nVal = 255;

                            p[0] = (byte)nVal;

                            ++p;
                        }
                        p += nOffset;
                    }
                }

                bitmap.UnlockBits(bmData);

                return true;
            }

            public static bool Contrast(Bitmap bitmap, sbyte nContrast)
            {
                if (nContrast < -100) return false;
                if (nContrast > 100) return false;

                double pixel = 0, contrast = (100.0 + nContrast) / 100.0;

                contrast *= contrast;

                int red, green, blue;

                // GDI+ still lies to us - the return format is BGR, NOT RGB.
                BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;

                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;

                    int nOffset = stride - bitmap.Width * 3;

                    for (int y = 0; y < bitmap.Height; ++y)
                    {
                        for (int x = 0; x < bitmap.Width; ++x)
                        {
                            blue = p[0];
                            green = p[1];
                            red = p[2];

                            pixel = red / 255.0;
                            pixel -= 0.5;
                            pixel *= contrast;
                            pixel += 0.5;
                            pixel *= 255;
                            if (pixel < 0) pixel = 0;
                            if (pixel > 255) pixel = 255;
                            p[2] = (byte)pixel;

                            pixel = green / 255.0;
                            pixel -= 0.5;
                            pixel *= contrast;
                            pixel += 0.5;
                            pixel *= 255;
                            if (pixel < 0) pixel = 0;
                            if (pixel > 255) pixel = 255;
                            p[1] = (byte)pixel;

                            pixel = blue / 255.0;
                            pixel -= 0.5;
                            pixel *= contrast;
                            pixel += 0.5;
                            pixel *= 255;
                            if (pixel < 0) pixel = 0;
                            if (pixel > 255) pixel = 255;
                            p[0] = (byte)pixel;

                            p += 3;
                        }
                        p += nOffset;
                    }
                }

                bitmap.UnlockBits(bmData);

                return true;
            }

            public static bool Gamma(Bitmap bitmap, double red, double green, double blue)
            {
                if (red < .2 || red > 5) return false;
                if (green < .2 || green > 5) return false;
                if (blue < .2 || blue > 5) return false;

                byte[] redGamma = new byte[256];
                byte[] greenGamma = new byte[256];
                byte[] blueGamma = new byte[256];

                for (int i = 0; i < 256; ++i)
                {
                    redGamma[i] = (byte)Math.Min(255, (int)((255.0 * Math.Pow(i / 255.0, 1.0 / red)) + 0.5));
                    greenGamma[i] = (byte)Math.Min(255, (int)((255.0 * Math.Pow(i / 255.0, 1.0 / green)) + 0.5));
                    blueGamma[i] = (byte)Math.Min(255, (int)((255.0 * Math.Pow(i / 255.0, 1.0 / blue)) + 0.5));
                }

                // GDI+ still lies to us - the return format is BGR, NOT RGB.
                BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;

                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;

                    int nOffset = stride - bitmap.Width * 3;

                    for (int y = 0; y < bitmap.Height; ++y)
                    {
                        for (int x = 0; x < bitmap.Width; ++x)
                        {
                            p[2] = redGamma[p[2]];
                            p[1] = greenGamma[p[1]];
                            p[0] = blueGamma[p[0]];

                            p += 3;
                        }
                        p += nOffset;
                    }
                }

                bitmap.UnlockBits(bmData);

                return true;
            }

            public static bool Color(Bitmap bitmap, int red, int green, int blue)
            {
                if (red < -255 || red > 255) return false;
                if (green < -255 || green > 255) return false;
                if (blue < -255 || blue > 255) return false;

                // GDI+ still lies to us - the return format is BGR, NOT RGB.
                BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;

                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;

                    int nOffset = stride - bitmap.Width * 3;
                    int nPixel;

                    for (int y = 0; y < bitmap.Height; ++y)
                    {
                        for (int x = 0; x < bitmap.Width; ++x)
                        {
                            nPixel = p[2] + red;
                            nPixel = Math.Max(nPixel, 0);
                            p[2] = (byte)Math.Min(255, nPixel);

                            nPixel = p[1] + green;
                            nPixel = Math.Max(nPixel, 0);
                            p[1] = (byte)Math.Min(255, nPixel);

                            nPixel = p[0] + blue;
                            nPixel = Math.Max(nPixel, 0);
                            p[0] = (byte)Math.Min(255, nPixel);

                            p += 3;
                        }
                        p += nOffset;
                    }
                }

                bitmap.UnlockBits(bmData);

                return true;
            }

            public static bool Conv3x3(Bitmap bitmap, ConvMatrix m)
            {
                // Avoid divide by zero errors
                if (0 == m.Factor) return false;

                Bitmap bSrc = (Bitmap)bitmap.Clone();

                // GDI+ still lies to us - the return format is BGR, NOT RGB.
                BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                int stride = bmData.Stride;
                int stride2 = stride * 2;
                System.IntPtr Scan0 = bmData.Scan0;
                System.IntPtr SrcScan0 = bmSrc.Scan0;

                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;
                    byte* pSrc = (byte*)(void*)SrcScan0;

                    int nOffset = stride + 6 - bitmap.Width * 3;
                    int nWidth = bitmap.Width - 2;
                    int nHeight = bitmap.Height - 2;

                    int nPixel;

                    for (int y = 0; y < nHeight; ++y)
                    {
                        for (int x = 0; x < nWidth; ++x)
                        {
                            nPixel = ((((pSrc[2] * m.TopLeft) + (pSrc[5] * m.TopMid) + (pSrc[8] * m.TopRight) +
                                (pSrc[2 + stride] * m.MidLeft) + (pSrc[5 + stride] * m.Pixel) + (pSrc[8 + stride] * m.MidRight) +
                                (pSrc[2 + stride2] * m.BottomLeft) + (pSrc[5 + stride2] * m.BottomMid) + (pSrc[8 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

                            if (nPixel < 0) nPixel = 0;
                            if (nPixel > 255) nPixel = 255;

                            p[5 + stride] = (byte)nPixel;

                            nPixel = ((((pSrc[1] * m.TopLeft) + (pSrc[4] * m.TopMid) + (pSrc[7] * m.TopRight) +
                                (pSrc[1 + stride] * m.MidLeft) + (pSrc[4 + stride] * m.Pixel) + (pSrc[7 + stride] * m.MidRight) +
                                (pSrc[1 + stride2] * m.BottomLeft) + (pSrc[4 + stride2] * m.BottomMid) + (pSrc[7 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

                            if (nPixel < 0) nPixel = 0;
                            if (nPixel > 255) nPixel = 255;

                            p[4 + stride] = (byte)nPixel;

                            nPixel = ((((pSrc[0] * m.TopLeft) + (pSrc[3] * m.TopMid) + (pSrc[6] * m.TopRight) +
                                (pSrc[0 + stride] * m.MidLeft) + (pSrc[3 + stride] * m.Pixel) + (pSrc[6 + stride] * m.MidRight) +
                                (pSrc[0 + stride2] * m.BottomLeft) + (pSrc[3 + stride2] * m.BottomMid) + (pSrc[6 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

                            if (nPixel < 0) nPixel = 0;
                            if (nPixel > 255) nPixel = 255;

                            p[3 + stride] = (byte)nPixel;

                            p += 3;
                            pSrc += 3;
                        }

                        p += nOffset;
                        pSrc += nOffset;
                    }
                }

                bitmap.UnlockBits(bmData);
                bSrc.UnlockBits(bmSrc);

                return true;
            }

            public static bool Smooth(Bitmap bitmap)
            {
                return Smooth(bitmap, 1);
            }

            public static bool Smooth(Bitmap bitmap, int nWeight)
            {
                ConvMatrix m = new ConvMatrix();
                m.SetAll(1);
                m.Pixel = nWeight;
                m.Factor = nWeight + 8;

                return BitmapFilter.Conv3x3(bitmap, m);
            }

            public static bool GaussianBlur(Bitmap bitmap)
            {
                return GaussianBlur(bitmap, 4);
            }

            public static bool GaussianBlur(Bitmap bitmap, int nWeight)
            {
                ConvMatrix m = new ConvMatrix();
                m.SetAll(1);
                m.Pixel = nWeight;
                m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = 2;
                m.Factor = nWeight + 12;

                return BitmapFilter.Conv3x3(bitmap, m);
            }

            public static bool MeanRemoval(Bitmap bitmap)
            {
                return MeanRemoval(bitmap, 9);
            }

            public static bool MeanRemoval(Bitmap bitmap, int nWeight)
            {
                ConvMatrix m = new ConvMatrix();
                m.SetAll(-1);
                m.Pixel = nWeight;
                m.Factor = nWeight - 8;

                return BitmapFilter.Conv3x3(bitmap, m);
            }

            public static bool Sharpen(Bitmap bitmap)
            {
                return Sharpen(bitmap, 11);
            }

            public static bool Sharpen(Bitmap bitmap, int nWeight)
            {
                ConvMatrix m = new ConvMatrix();
                m.SetAll(0);
                m.Pixel = nWeight;
                m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = -2;
                m.Factor = nWeight - 8;

                return BitmapFilter.Conv3x3(bitmap, m);
            }

            public static bool EmbossLaplacian(Bitmap bitmap)
            {
                ConvMatrix m = new ConvMatrix();
                m.SetAll(-1);
                m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = 0;
                m.Pixel = 4;
                m.Offset = 127;

                return BitmapFilter.Conv3x3(bitmap, m);
            }

            public static bool EdgeDetectQuick(Bitmap bitmap)
            {
                ConvMatrix m = new ConvMatrix();
                m.TopLeft = m.TopMid = m.TopRight = -1;
                m.MidLeft = m.Pixel = m.MidRight = 0;
                m.BottomLeft = m.BottomMid = m.BottomRight = 1;

                m.Offset = 127;

                return BitmapFilter.Conv3x3(bitmap, m);
            }
        }

        public static Size SizeInSize(Size size, Size outSize)
        {
            if (size.Width > outSize.Width || size.Height > outSize.Height)
            {
                if (size.Width > size.Height)
                {
                    size.Height = size.Height * outSize.Width / size.Width;
                    size.Width = outSize.Width;
                }
                else
                {
                    size.Width = size.Width * outSize.Height / size.Height;
                    size.Height = outSize.Height;
                }
            }

            return size;
        }

        public static float GetZoom(Size sourceSize, Size targetSize)
        {
            if (sourceSize.Width == 0 || sourceSize.Height == 0)
                return 0.0f;

            return Math.Min((float)targetSize.Width / sourceSize.Width, (float)targetSize.Height / sourceSize.Height);
        }
    }
}
