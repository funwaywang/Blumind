using System;

namespace Blumind.Core
{
    public class XListEventArgs<T> : EventArgs
    {
        public int Index { get; private set; }
        public T Item { get; private set; }

        public XListEventArgs(int index, T item)
        {
            Index = index;
            Item = item;
        }
    }

    public class XListValueEventArgs<T> : EventArgs
    {
        public int Index { get; private set; }
        public T OldValue { get; private set; }
        public T NewValue { get; private set; }

        public XListValueEventArgs(int index, T oldValue, T newValue)
        {
            Index = index;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
