using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Dialogs;
using Blumind.Model;
using Blumind.Model.Widgets;

namespace Blumind.Design
{
    class PictureEditor : UITypeEditor
    {
        PictureEditUI EditUI;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override bool IsDropDownResizable
        {
            get
            {
                return true;
            }
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value is PictureWidget.PictureDesign || value == null)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc != null)
                {
                    if (EditUI == null)
                    {
                        EditUI = new PictureEditUI();
                        EditUI.Size = new Size(200, 300);
                    }

                    EditUI.Initialize(edSvc);
                    EditUI.CurrentObject = value as PictureWidget.PictureDesign;
                    edSvc.DropDownControl(EditUI);
                    return EditUI.CurrentObject;
                }
            }

            return base.EditValue(context, provider, value);
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            if (e.Value is PictureWidget.PictureDesign)
            {
                PictureWidget.PictureDesign pd = (PictureWidget.PictureDesign)e.Value;
                if (pd.Data != null)
                {
                    PaintHelper.DrawImageInRange(e.Graphics, pd.Data, e.Bounds);
                }
            }
        }

        
    }
}
