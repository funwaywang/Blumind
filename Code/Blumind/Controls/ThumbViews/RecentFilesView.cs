using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blumind.Configuration;

namespace Blumind.Controls
{
    class RecentFilesView : ThumbView
    {
        bool ViewNeedUpdate;

        public RecentFilesView()
        {
            RecentFilesManage.Default.FilesChanged += Default_FilesChanged;
            ViewNeedUpdate = true;
        }

        void Default_FilesChanged(object sender, EventArgs e)
        {
            ViewNeedUpdate = true;
            RefreshView();
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (ViewNeedUpdate)
            {
                RefreshView();
            }
        }

        void RefreshView()
        {
            Items.Clear();
            foreach (var file in RecentFilesManage.Default.Reverse())
            {
                var fti = new FileThumbItem(file);
                fti.CanClose = true;
                Items.Add(fti);
            }

            ViewNeedUpdate = true;
        }

        protected override void OnItemClosed(ThumbViewItemEventArgs e)
        {
            base.OnItemClosed(e);

            if (e.Item is FileThumbItem)
            {
                var fti = (FileThumbItem)e.Item;
                RecentFilesManage.Default.Remove(fti.Filename);
            }
        }
    }
}
