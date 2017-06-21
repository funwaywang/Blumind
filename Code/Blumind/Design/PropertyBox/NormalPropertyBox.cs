using System.Drawing;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Design
{
    class NormalPropertyBox : PropertyBox
    {
        PropertyControl propertyControl1;

        public NormalPropertyBox()
        {
            propertyControl1 = new PropertyControl();
            propertyControl1.Text = Lang._("Property");
            propertyControl1.HelpVisible = HelpVisible;
            propertyControl1.ToolbarVisible = ToolbarVisible;
            propertyControl1.BackColor = ContentBackColor;
            propertyControl1.ShowBorder = false;
            propertyControl1.Dock = DockStyle.Fill;
            propertyControl1.Font = SystemFonts.MessageBoxFont;
            //AddPage(PropertyGrid, Properties.Resources.property);
            Controls.Add(propertyControl1);
            propertyControl1.BringToFront();
        }

        protected override void OnSelectedObjectsChanged()
        {
            propertyControl1.SelectedObjects = SelectedObjects;

            base.OnSelectedObjectsChanged();
        }

        protected override void OnCurrentLanguageChanged()
        {
            base.OnCurrentLanguageChanged();

            propertyControl1.Text = LanguageManage.GetText("Property");
        }
    }
}
