using System;
using System.Collections.Generic;
using System.ComponentModel;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Model.Styles
{
    class ChartThemeFolder
    {
        private string _Name;
        private XList<ChartTheme> _Themes;
        private XList<ChartThemeFolder> _Folders;

        public event EventHandler NameChanged;

        public ChartThemeFolder()
        {
            Themes = new XList<ChartTheme>();
            Themes.ItemAdded += new XListEventHandler<ChartTheme>(Themes_ItemAdded);
            Themes.ItemRemoved += new XListEventHandler<ChartTheme>(Themes_ItemRemoved);

            Folders = new XList<ChartThemeFolder>();
        }

        public ChartThemeFolder(string name)
            : this()
        {
            Name = name;
        }

        [DefaultValue(null)]
        [LocalDisplayName("Name"), LocalCategory("General")]
        public string Name
        {
            get { return _Name; }
            set 
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnNameChanged();
                }
            }
        }

        public XList<ChartTheme> Themes
        {
            get { return _Themes; }
            private set { _Themes = value; }
        }

        public XList<ChartThemeFolder> Folders
        {
            get { return _Folders; }
            private set { _Folders = value; }
        }

        public IEnumerable<ChartTheme> AllThemes
        {
            get
            {
                foreach (var f in Folders)
                    foreach (var c in f.AllThemes)
                        yield return c;

                foreach (var t in Themes)
                    yield return t;
            }
        }

        private void Themes_ItemAdded(object sender, XListEventArgs<ChartTheme> e)
        {
            if (e.Item != null)
            {
                e.Item.Folder = this;
            }
        }

        private void Themes_ItemRemoved(object sender, XListEventArgs<ChartTheme> e)
        {
            if (e.Item != null && e.Item.Folder == this)
            {
                e.Item.Folder = null;
            }
        }

        private void OnNameChanged()
        {
            if (NameChanged != null)
                NameChanged(this, EventArgs.Empty);
        }

        public bool HasTheme(string name)
        {
            foreach (ChartTheme them in Themes)
            {
                if (them != null && StringComparer.OrdinalIgnoreCase.Equals(them.Name, name))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
