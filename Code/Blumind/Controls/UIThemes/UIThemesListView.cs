using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Blumind.Controls.UIThemes
{
    class UIThemesListView : ListView
    {
        IEnumerable<UIColorTheme> _Themes;

        public UIThemesListView()
        {
            CellSize = new Size(96, 96);
        }

        [DefaultValue(typeof(Size), "128, 128")]
        public Size CellSize { get; set; }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (!DesignMode)
            {
                View = System.Windows.Forms.View.LargeIcon;
            }
        }

        public IEnumerable<UIColorTheme> Themes
        {
            get { return _Themes; }
            set 
            {
                if (_Themes != value)
                {
                    _Themes = value;
                    OnThemesChanged();
                }
            }
        }

        protected virtual void OnThemesChanged()
        {
            RefreshItems();
        }

        void RefreshItems()
        {
            Items.Clear();

            LargeImageList = new ImageList();
            LargeImageList.ImageSize = CellSize;

            if (Themes != null)
            {
                int index = 0;
                foreach (var t in Themes)
                {
                    var thumb = t.CreateThumb(CellSize);
                    LargeImageList.Images.Add(thumb);

                    var lvi = new ListViewItem();
                    lvi.Text = t.Name;
                    lvi.SubItems.Add(t.Description);
                    lvi.ToolTipText = t.Description;
                    lvi.ImageIndex = index;
                    Items.Add(lvi);

                    index++;
                }
            }
        }
    }
}
