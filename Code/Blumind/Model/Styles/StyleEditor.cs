using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Blumind.Model.Styles
{
    class StyleEditor : UITypeEditor
    {
        StyleEditDialog editDialog;
        object Value;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            this.Value = value;
            if ((provider != null) && (((IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService))) != null))
            {
                if (this.editDialog == null)
                {
                    this.editDialog = new StyleEditDialog();
                }

                Style style = value as Style;
                if (style != null)
                {
                    this.editDialog.Style = style.Clone();
                }

                if (this.editDialog.ShowDialog() == DialogResult.OK)
                {
                    if (style != null)
                        style.Copy(editDialog.Style);
                    else
                        this.Value = editDialog.Style;
                }
            }

            value = this.Value;
            this.Value = null;
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
