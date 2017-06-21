using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Dialogs;
using Blumind.Globalization;

namespace Blumind.Design
{
    class HyperlinkEditor : UITypeEditor
    {
        UrlEditUI EditUI;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null)
            {
                EditUI = new UrlEditUI();
                EditUI.Initialize(edSvc);
                EditUI.CurrentObject = value as string;
                edSvc.DropDownControl(EditUI);
                return EditUI.CurrentObject;
            }

            return base.EditValue(context, provider, value);
        }

        class UrlEditUI : ListBox
        {
            private string _CurrentObject;
            private IWindowsFormsEditorService Service;

            public UrlEditUI()
            {
                Items.Add(new ListItem<string>(LanguageManage.GetText("Cut"), "Cut", Properties.Resources.cut));
                Items.Add(new ListItem<string>(LanguageManage.GetText("Copy"), "Copy", Properties.Resources.copy));
                Items.Add(new ListItem<string>(LanguageManage.GetText("Paste"), "Paste", Properties.Resources.paste));
                Items.Add(new ListItem<string>(LanguageManage.GetTextWithEllipsis("Select Local File"), "Open", Properties.Resources.open));
                Items.Add(new ListItem<string>(LanguageManage.GetTextWithEllipsis("Internet"), "Internet", Properties.Resources.internet));

                DrawMode = DrawMode.OwnerDrawFixed;
                ItemHeight += 10;
                BorderStyle = BorderStyle.None;
                this.Size = new Size(Math.Max(Width, 200), ItemHeight * Math.Min(12, Items.Count) + 20);
            }

            public string CurrentObject
            {
                get { return _CurrentObject; }
                set
                {
                    _CurrentObject = value;
                    OnCurrentObjectChanged();
                }
            }

            public void Initialize(IWindowsFormsEditorService service)
            {
                Service = service;
            }

            protected virtual void OnCurrentObjectChanged()
            {
                if (!Clipboard.ContainsText())
                {
                    RemoveItem("Paste");
                }

                if (string.IsNullOrEmpty(CurrentObject))
                {
                    RemoveItem("Cut");
                    RemoveItem("Copy");
                }

                this.Size = new Size(Math.Max(Width, 200), ItemHeight * Math.Min(12, Items.Count) + 20);
            }

            private void RemoveItem(string name)
            {
                for (int i = Items.Count - 1; i >= 0; i--)
                {
                    if (((ListItem<string>)Items[i]).Value == name)
                    {
                        Items.RemoveAt(i);
                        return;
                    }
                }
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);

                if (SelectedItem is ListItem<string>)
                {
                    switch (((ListItem<string>)SelectedItem).Value)
                    {
                        case "Cut":
                            if (!string.IsNullOrEmpty(CurrentObject))
                                Clipboard.SetText(CurrentObject);
                            CurrentObject = null;
                            break;
                        case "Copy":
                            if (!string.IsNullOrEmpty(CurrentObject))
                                Clipboard.SetText(CurrentObject);
                            break;
                        case "Paste":
                            CurrentObject = Clipboard.GetText();
                            break;
                        case "Open":
                            OpenLocalFile();
                            break;
                        case "Internet":
                            InputInternetUrl();
                            break;
                    }
                }

                if (Service != null)
                    Service.CloseDropDown();
            }

            private void InputInternetUrl()
            {
                var dialog = new InputDialog(Lang._("Hyperlink"), Lang._("Please input Url below"));
                dialog.Icon = Properties.Resources.globe_network;
                dialog.ShowIcon = true;
                dialog.Value = CurrentObject;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    CurrentObject = dialog.Value;
                }
            }

            private void OpenLocalFile()
            {
                var dialog = new OpenFileDialog();
                dialog.Multiselect = false;
                dialog.Filter = string.Format("{0} (*.*)|*.*", LanguageManage.GetText("All Files"));
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    CurrentObject = dialog.FileName;
                }
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);

                SelectedIndex = IndexFromPoint(e.X, e.Y);
            }

            protected override void OnDrawItem(DrawItemEventArgs e)
            {
                if (e.Index < 0 || e.Index >= Items.Count)
                {
                    base.OnDrawItem(e);
                    return;
                }

                e.DrawBackground();

                Rectangle rect = GetItemRectangle(e.Index);
                Brush brushFore;
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    brushFore = SystemBrushes.HighlightText;

                    PaintHelper.DrawHoverBackgroundFlat(e.Graphics, rect, SystemColors.Highlight);
                }
                else
                {
                    brushFore = SystemBrushes.WindowText;
                    e.Graphics.DrawLine(SystemPens.ControlLight, rect.Left + 2, rect.Bottom - 1, rect.Right - 2, rect.Bottom - 1);
                }

                var lt = (ListItem<string>)Items[e.Index];
                rect.Inflate(-1, -1);

                // draw icon
                Image image = lt.Image;
                if (image != null)
                {
                    Rectangle rectImage = new Rectangle(rect.Left + 2, rect.Y + (rect.Height - 16) / 2, 16, 16);
                    e.Graphics.DrawImage(image, rectImage, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
                }
                rect.X += 20;
                rect.Width -= 20;

                // draw text
                StringFormat sf = new StringFormat(PaintHelper.SFLeft);
                sf.FormatFlags |= StringFormatFlags.NoWrap;
                Font font = Font;
                string str = lt.Text;
                e.Graphics.DrawString(str, font, brushFore, rect, sf);
            }
        }
    }
}
