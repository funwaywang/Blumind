using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Blumind.Canvas.Svg
{
    class TextLine
    {
        public TextLine(string text)
        {
            Text = text;
        }

        public Rectangle Bounds { get; set; }

        public string Text { get; set; }

        public Size Size
        {
            get { return Bounds.Size; }
            set
            {
                Bounds = new Rectangle(Bounds.Location, value);
            }
        }

        public Point Location
        {
            get { return Bounds.Location; }
            set
            {
                Bounds = new Rectangle(value, Bounds.Size);
            }
        }

        public int X
        {
            get { return Bounds.X; }
            set { Bounds = new Rectangle(value, Bounds.Y, Bounds.Width, Bounds.Height); }
        }

        public int Y
        {
            get { return Bounds.Y; }
            set { Bounds = new Rectangle(Bounds.X, value, Bounds.Width, Bounds.Height); }
        }

        public int Width
        {
            get { return Bounds.Width; }
            set { Bounds = new Rectangle(Bounds.X, Bounds.Y, value, Bounds.Height); }
        }

        public int Height
        {
            get { return Bounds.Height; }
            set { Bounds = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, value); }
        }
    }
}
