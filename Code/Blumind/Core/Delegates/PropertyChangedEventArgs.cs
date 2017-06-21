using System;
using System.Collections.Generic;
using System.Text;

namespace Blumind.Core
{
    public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

    [Flags]
    public enum ChangeTypes
    {
        None        = 0,

        Data        = 2,
        Visual      = 4,
        Layout      = 8,
        ViewPort    = 16,
        NoData      = 32,

        AllVisual   = Visual | Layout | ViewPort,
        All         = Data | Visual | Layout | ViewPort,
    }

    public class PropertyChangedEventArgs : EventArgs
    {
        string _PropertyName;
        object _OldValue;
        object _NewValue;
        ChangeTypes _Changes = ChangeTypes.Data;
        bool _Rollbackable = true;

        public PropertyChangedEventArgs(string propertyName, object oldValue, object newValue)
        {
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
        }

        //public PropertyChangedEventArgs(string propertyName, object oldValue, object newValue, ChangeTypes changes)
        //    : this(propertyName, oldValue, newValue)
        //{
        //    Changes = changes;
        //}

        public PropertyChangedEventArgs(string propertyName, object oldValue, object newValue, ChangeTypes changes, bool rollbackable)
            : this(propertyName, oldValue, newValue)
        {
            Changes = changes;
            Rollbackable = rollbackable;
        }

        public ChangeTypes Changes
        {
            get { return _Changes; }
            private set { _Changes = value; }
        }

        public object OldValue
        {
            get { return _OldValue; }
            private set { _OldValue = value; }
        }

        public object NewValue
        {
            get { return _NewValue; }
            private set { _NewValue = value; }
        }

        public string PropertyName
        {
            get { return _PropertyName; }
            private set { _PropertyName = value; }
        }

        public bool Rollbackable
        {
            get { return _Rollbackable; }
            set { _Rollbackable = value; }
        }

        public bool HasChanges(ChangeTypes changes)
        {
            return (Changes & changes) == changes;
        }
    }

    public interface INotifyPropertyChanged
    {
        event PropertyChangedEventHandler PropertyChanged;

        bool PropertyChangeSuspending { get; set; }
    }
}
