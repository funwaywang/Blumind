using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Blumind.Core
{
    public delegate void XListEventHandler<T>(object sender, XListEventArgs<T> e);
    public delegate void XListValueEventHandler<T>(object sender, XListValueEventArgs<T> e);

    public class XList<T> : Collection<T>
    {
        public event XListEventHandler<T> ItemAdded;
        public event XListEventHandler<T> ItemRemoved;
        public event XListValueEventHandler<T> ItemChanged;
        public event EventHandler AfterClear;
        public event EventHandler BeforeClear;
        public event EventHandler AfterSort;
        public event ListChangedEventHandler ListChanged;

        public bool IsEmpty
        {
            get { return this.Count <= 0; }
        }

        [Browsable(false)]
        public bool IsReadOnly
        {
            get { return Items.IsReadOnly; }
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException();
            }

            foreach(var item in items)
            {
                Add(item);
            }
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            if (ItemAdded != null)
                ItemAdded(this, new XListEventArgs<T>(index, item));

            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
        }

        protected override void RemoveItem(int index)
        {
            T item = Items[index];
            base.RemoveItem(index);

            if (ItemRemoved != null)
                ItemRemoved(this, new XListEventArgs<T>(index, item));

            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
        }

        protected override void SetItem(int index, T item)
        {
            T old = Items[index];
            base.SetItem(index, item);
            if (ItemChanged != null)
                ItemChanged(this, new XListValueEventArgs<T>(index, old, item));

            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
        }

        protected override void ClearItems()
        {
            if (BeforeClear != null)
                BeforeClear(this, EventArgs.Empty);

            base.ClearItems();

            if (AfterClear != null)
                AfterClear(this, EventArgs.Empty);

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
        }

        protected virtual void OnListChanged(ListChangedEventArgs e)
        {
            if (ListChanged != null)
                ListChanged(this, e);
        }

        public T[] ToArray()
        {
            T[] result = new T[Count];
            for (int i = 0; i < Count; i++)
            {
                result[i] = this[i];
            }

            return result;
        }

        internal void SortAs(int[] newIndices)
        {
            if (IsReadOnly)
                throw new NotSupportedException();

            if (newIndices == null || newIndices.Length != this.Count)
                throw new ArgumentException();

            T[] items = ToArray();
            int i = 0;
            foreach (int index in newIndices)
            {
                Items[index] = items[i++];
            }

            OnAfterSort();
        }

        protected virtual void OnAfterSort()
        {
            if (AfterSort != null)
                AfterSort(this, EventArgs.Empty);
        }

        public int RemoveAll(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException();

            int count = 0;
            for (int i = Count - 1; i >= 0; i--)
            {
                if (match(this[i]))
                {
                    this.RemoveAt(i);
                    count++;
                }
            }

            return count;
        }
    }
}
