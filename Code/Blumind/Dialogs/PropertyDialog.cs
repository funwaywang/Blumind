using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Blumind.Configuration;
using Blumind.Globalization;

namespace Blumind.Controls
{
    class PropertyDialog : StandardDialog
    {
        const string PropertyDialogSize = "PropertyDialogSize";
        PropertyControl PropertyGrid;

        public PropertyDialog()
        {
            InitializeComponent();

            AfterInitialize();
        }

        public PropertyDialog(object selectedObject)
            : this()
        {
            SelectedObject = selectedObject;
        }

        [Browsable(false)]
        public object SelectedObject
        {
            get { return PropertyGrid.SelectedObject; }
            set { PropertyGrid.SelectedObject = value; }
        }

        [Browsable(false)]
        public PropertySort PropertySort
        {
            get { return PropertyGrid.PropertySort; }
            set { PropertyGrid.PropertySort = value; }
        }

        void InitializeComponent()
        {
            PropertyGrid = new PropertyControl();
            SuspendLayout();

            // PropertyGrid
            PropertyGrid.HelpVisible = false;
            PropertyGrid.ShowBorder = false;
            PropertyGrid.TabIndex = 0;
            PropertyGrid.ToolbarVisible = false;
            PropertyGrid.Font = SystemFonts.MessageBoxFont;

            // PropertyDialog
            Controls.Add(this.PropertyGrid);
            Text = "Property";
            Icon = Properties.Resources.property_icon;
            MinimumSize = new Size(200, 200);
            Size = Options.Current.GetValue(PropertyDialogSize, Size);
            ResumeLayout(false);
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            Text = Lang._("Property");
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (PropertyGrid != null)
            {
                PropertyGrid.Bounds = this.ControlsRectangle;
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            Options.Current.SetValue(PropertyDialogSize, Size);
        }
    }
}
