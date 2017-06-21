using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Blumind.Controls
{
    public class UIStatusImage
    {
        public UIStatusImage()
        {
            SlicesLocation = new Dictionary<UIControlStatus, Point>();
        }

        public UIStatusImage(Image image)
            :this()
        {
            Image = image;
        }

        public Size SliceSize { get; set; }

        public Image Image { get; set; }

        public Dictionary<UIControlStatus, Point> SlicesLocation { get; private set; }

        public static UIStatusImage FromHorizontal(Image image, UIControlStatus[] status)
        {
            if (image != null && status != null && status.Length > 0)
                return FromHorizontal(image, image.Width / status.Length, status);
            else
                return FromHorizontal(image, 0, status);
        }

        public static UIStatusImage FromHorizontal(Image image, int sliceSize, UIControlStatus[] status)
        {
            var img = new UIStatusImage(image);
            img.SliceSize = new Size(sliceSize, 0);

            if (status != null)
            {
                int p = 0;
                foreach (var s in status)
                {
                    img.SlicesLocation[s] = new Point(p, 0);
                    p += sliceSize;
                }
            }

            return img;
        }
        
        public static UIStatusImage FromVertical(Image image, UIControlStatus[] status)
        {
            if (image != null && status != null && status.Length > 0)
                return FromVertical(image, image.Height / status.Length, status);
            else
                return FromVertical(image, 0, status);
        }

        public static UIStatusImage FromVertical(Image image, int sliceSize, UIControlStatus[] status)
        {
            var img = new UIStatusImage(image);
            img.SliceSize = new Size(0, sliceSize);

            if (status != null)
            {
                int p = 0;
                foreach (var s in status)
                {
                    img.SlicesLocation[s] = new Point(0, p);
                    p += sliceSize;
                }
            }

            return img;
        }

        public Rectangle GetBounds(UIControlStatus status)
        {
            Rectangle rect;
            if (SlicesLocation.ContainsKey(status))
            {
                var location = SlicesLocation[status];
                rect = new Rectangle(location, SliceSize);
            }
            else
            {
                rect = Rectangle.Empty;
            }

            if (rect.Width == 0 && Image != null)
                rect.Width = Image.Width;

            if (rect.Height == 0 && Image != null)
                rect.Height = Image.Height;

            return rect;
        }

        public bool Draw(Graphics graphics, UIControlStatus status, Rectangle targetRectangle)
        {
            if (this.Image == null)
                return false;

            var rectSource = GetBounds(status);
            if (rectSource.Width <= 0 || rectSource.Height <= 0)
                return false;

            return PaintHelper.DrawImageInRange(graphics, Image, targetRectangle, rectSource);
        }

        public bool DrawExpand(Graphics graphics, UIControlStatus status, Rectangle targetRectangle, int padding)
        {
            return DrawExpand(graphics, status, targetRectangle, new Padding(padding));
        }

        public bool DrawExpand(Graphics graphics, UIControlStatus status, Rectangle targetRectangle, Padding padding)
        {
            if (this.Image == null)
                return false;

            var rectSource = GetBounds(status);
            if (rectSource.Width <= 0 || rectSource.Height <= 0)
                return false;

            return PaintHelper.ExpandImage(graphics, Image, padding, targetRectangle, rectSource);
        }
    }
}
