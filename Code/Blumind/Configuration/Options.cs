using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;
using Blumind.Configuration.Formaters;
using Blumind.Configuration.Models;
using Blumind.Core;

namespace Blumind.Configuration
{
    abstract class Options : ModifyObject
    {
        public static Options Current { get; private set; }
        public static Options DefaultValue { get; private set; }
        protected Dictionary<string, object> Data { get; private set; }
        Options DataSource { get; set; }
        Dictionary<string, object> ChangedItems { get; set; }

        public event EventHandler OpitonsChanged;
        public event Blumind.Core.PropertyChangedEventHandler OptionChanged;

        static Options()
        {
            DefaultValue = new MemoryOptions();
            Current = new FileSystemOptions(DefaultValue);
        }

        public Options()
        {
            Data = new Dictionary<string, object>();
            ChangedItems = new Dictionary<string, object>();
        }

        public Options(Options dataSource)
            : this()
        {
            DataSource = dataSource;
        }

        public abstract void Load(string[] args);

        public abstract bool Save();

        public IEnumerable<string> Changes
        {
            get { return ChangedItems.Select(c => c.Key); }
        }

        public object GetValue(string key)
        {
            if (key == null)
                throw new ArgumentNullException();

            if (Data.ContainsKey(key))
                return Data[key];
            else if (DataSource != null)
                return DataSource.GetValue(key);
            else
                return null;
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            if (key == null)
                throw new ArgumentNullException();

            if (Data.ContainsKey(key))
                return ST.GetValue<T>(Data[key], defaultValue);
            else if (DataSource != null)
                return DataSource.GetValue<T>(key, defaultValue);
            else
                return defaultValue;
        }

        public T GetValue<T>(string key)
        {
            return GetValue<T>(key, default(T));
        }

        public string GetString(string key)
        {
            return GetValue<string>(key);
        }

        public int GetInt(string key)
        {
            return GetValue<int>(key);
        }

        public bool GetBool(string key)
        {
            return GetValue<bool>(key);
        }

        public bool GetBool(string key, bool defaultValue)
        {
            return GetValue(key, defaultValue);
        }

        public void SetValue(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException();

            object old = GetValue(key);
            if (old != value)
            {
                if (!ChangedItems.ContainsKey(key))
                    ChangedItems.Add(key, old);
                Data[key] = value;
                OnOptionChanged(key, old, value);
                Modified = true;
            }
        }

        public void AcceptChanges()
        {
            ChangedItems.Clear();
            Modified = false;
        }

        public void Clear()
        {
            this.Data.Clear();
        }

        protected virtual object DeserializeValue(XmlElement node)
        {
            if (node == null)
                throw new ArgumentNullException();

            var type = node.GetAttribute("type");
            if (!string.IsNullOrEmpty(type))
            {
                IObjectFormater formater = ObjectFormaters.GetFormater(type);
                if (formater != null)
                    return formater.DeserializeValue(node);
            }

            return node.GetAttribute("value");
        }

        protected virtual object DeserializeValue(string type, string value)
        {
            if (string.IsNullOrEmpty(type))
                return value;

            switch (type)
            {
                case "int":
                    return ST.GetIntDefault(value);
                case "decimal":
                    return ST.GetDecimalDefault(value);
                case "float":
                    return ST.GetFloatDefault(value);
                default:
                    return value;
            }
        }

        protected virtual void SerializeValue(XmlElement node, object value)
        {
            IObjectFormater formater = null;

            if(value != null)
                formater = ObjectFormaters.GetFormater(value.GetType());

            if (formater != null)
            {
                formater.SerializeValue(node, value);
                node.SetAttribute("type", formater.Name);
            }
            else if (value != null)
            {
                node.SetAttribute("value", ST.ToString(value, false));
            }
        }

        public void InvokeChanged()
        {
            if (OpitonsChanged != null)
                OpitonsChanged(this, EventArgs.Empty);
        }

        void OnOptionChanged(string key, object old, object value)
        {
            if (OptionChanged != null)
            {
                OptionChanged(this, new Core.PropertyChangedEventArgs(key, old, value));
            }
        }

        public bool Contains(string key)
        {
            return Data != null && Data.ContainsKey(key);
        }
    }
}
