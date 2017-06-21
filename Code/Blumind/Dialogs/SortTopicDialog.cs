using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Globalization;

namespace Blumind.Dialogs
{
    partial class SortTopicDialog : StandardDialog
    {
        private object[] _Items;

        public SortTopicDialog()
        {
            InitializeComponent();

            AfterInitialize();
        }

        public SortTopicDialog(object[] items)
            : this()
        {
            Items = items;
        }

        public object[] Items
        {
            get { return _Items; }
            set 
            {
                if (_Items != value)
                {
                    _Items = value;
                    OnItemsChanged();
                }
            }
        }

        private void OnItemsChanged()
        {
            sortBox1.Items = Items;
        }

        public int[] GetNewIndices()
        {
            return sortBox1.GetNewIndices();
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            Text = Lang._("Custom Sort");
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (sortBox1 != null)
            {
                sortBox1.Bounds = this.ControlsRectangle;
            }
        }

        public override void ApplyTheme(UITheme theme)
        {
            base.ApplyTheme(theme);

            if (sortBox1 != null)
            {
                sortBox1.Font = theme.DefaultFont;
            }
        }
    }
}
