using System.Drawing;
using System.Windows.Forms;

namespace Blumind.Resources
{
    public static class RS
    {
        public static Icon GetIcon(string name)
        {
            return new Icon(typeof(RS), name + ".ico");
        }

        public static Image GetImage(string name)
        {
            return new Bitmap(typeof(RS), name + ".png");
        }

        public static Cursor GetCursor(string name)
        {
            return new Cursor(typeof(RS), name + ".cur");
        }
    }
}