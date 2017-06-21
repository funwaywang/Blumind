using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Blumind.Controls
{
    public class ListItem : ListItem<object>
    {
        public static T GetSelectedValue<T>(ComboBox combox)
        {
            if (combox.SelectedItem is ListItem<T>)
            {
                return ((ListItem<T>)combox.SelectedItem).Value;
            }
            else if (combox.SelectedItem is T)
            {
                return (T)combox.SelectedItem;
            }
            else if (combox.SelectedItem is ListItem)
            {
                if (((ListItem)combox.SelectedItem).Value is T)
                    return (T)((ListItem)combox.SelectedItem).Value;
            }

            return default(T);
        }
    }

    public class ListItem<T>
    {
        public Image Image { get; set; }
        public string Text { get; set; }
        public T Value { get; set; }

        public ListItem()
        {
        }

        public ListItem(string text, T value)
        {
            Text = text;
            Value = value;
        }

        public ListItem(string text, T value, Image image)
            :this(text, value)
        {
            Image = image;
        }

        public override string ToString()
        {
            if (Text == null)
                return string.Empty;
            else
                return Text;
        }

        public bool Select(ComboBox combox, T value)
        {
            return Select<T>(combox, value, null);
        }

        public bool Select(ComboBox combox, T value, IEqualityComparer<T> comparer)
        {
            return Select<T>(combox, value, comparer);
        }

        public static bool Select<TValue>(ComboBox combox, TValue value)
        {
            return Select<TValue>(combox, value, null);
        }

        public static bool Select<TValue>(ComboBox combox, TValue value, IEqualityComparer<TValue> comparer)
        {
            if (comparer == null)
                comparer = EqualityComparer<TValue>.Default;

            foreach (object item in combox.Items)
            {
                if (item is ListItem<TValue>)
                {
                    var li = (ListItem<TValue>)item;
                    if (comparer.Equals(li.Value, value))
                    {
                        combox.SelectedItem = item;
                        return true;
                    }
                }
            }

            combox.SelectedIndex = -1;
            return false;
        }
    }
}
