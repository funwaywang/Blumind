using System.Drawing;

namespace Blumind.Controls
{
    class ListBoxControlItem<T>
    {
        public ListBoxControlItem()
        {
            Index = -1;
        }

        public ListBoxControlItem(string text)
        {
            Index = -1;
            Text = text;
        }

        public ListBoxControlItem(string text, object tag)
        {
            Index = -1;
            Text = text;
            Tag = tag;
        }

        public int Index { get; internal set; }

        public string Text { get; set; }

        public T Value { get; set; }

        public object Tag { get; set; }

        public Rectangle Bounds { get; internal set; }

        public Image Icon { get; set; }

        public string ToolTipText { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
