using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Blumind.Controls;

namespace Blumind.Design
{
    abstract class ListEditor<T> : UITypeEditor
    {
        ListEditUI EditUI;
        protected abstract T[] GetStandardValues();

        protected virtual IEnumerable<ListItem<T>> GetStandardItems()
        {
            foreach (var t in GetStandardValues())
                yield return new ListItem<T>(t.ToString(), t);
        }

        protected virtual int ListControlMinWidth 
        {
            get { return 100; } 
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context.PropertyDescriptor.PropertyType == typeof(T))
            {
                var edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc != null)
                {
                    if (EditUI == null)
                    {
                        EditUI = new ListEditUI(this);
                    }

                    EditUI.Initialize(edSvc, GetStandardItems());
                    if(value is T)
                        EditUI.CurrentObject = (T)value;                    
                    edSvc.DropDownControl(EditUI);
                    return EditUI.CurrentObject;
                }
            }

            return base.EditValue(context, provider, value);
        }

        protected virtual void DrawListItem(DrawItemEventArgs e, Rectangle rect, ListItem<T> listItem)
        {
            if (listItem != null)
            {
                DrawItemText(e, rect, listItem.Text);
            }
        }

        protected void DrawItemText(DrawItemEventArgs e, Rectangle rect, string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Brush brushFore = ((e.State & DrawItemState.Selected) == DrawItemState.Selected) ? SystemBrushes.HighlightText : SystemBrushes.WindowText;
                StringFormat sf = PaintHelper.SFLeft;
                sf.FormatFlags |= StringFormatFlags.NoWrap;
                sf.Trimming = StringTrimming.None;
                e.Graphics.DrawString(text, e.Font, brushFore, rect, sf);
            }
        }

        protected void DrawIcon(Graphics graphics, Image icon, ref Rectangle rect)
        {
            if (icon != null)
            {
                graphics.DrawImage(icon,
                    new Rectangle(rect.X, rect.Y + (rect.Height - icon.Height) / 2, icon.Width, icon.Height),
                    new Rectangle(0, 0, icon.Width, icon.Height),
                    GraphicsUnit.Pixel);
            }
            rect.X += 20;
            rect.Width -= 20;
        }

        class ListEditUI : ListBox
        {
            ListEditor<T> Owner;
            T _CurrentObject;
            IWindowsFormsEditorService Service;

            public ListEditUI(ListEditor<T> owner)
            {
                Owner = owner;
                DrawMode = DrawMode.OwnerDrawFixed;
                ItemHeight += 10;
                BorderStyle = BorderStyle.None;
            }

            public T CurrentObject
            {
                get { return _CurrentObject; }
                set { _CurrentObject = value; }
            }

            public void Initialize(IWindowsFormsEditorService service, T[] standardValues)
            {
                Service = service;

                Items.Clear();
                foreach (T obj in standardValues)
                {
                    Items.Add(obj);
                }

                //
                Size = new Size(Math.Max(Width, Owner.ListControlMinWidth), ItemHeight * Math.Min(12, Items.Count) + 20);
            }

            public void Initialize(IWindowsFormsEditorService service, IEnumerable<ListItem<T>> items)
            {
                Service = service;

                Items.Clear();
                foreach (var item in items)
                {
                    Items.Add(item);
                }

                //
                Size = new Size(Math.Max(Width, Owner.ListControlMinWidth), ItemHeight * Math.Min(12, Items.Count) + 20);
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);

                if (SelectedItem is T)
                    CurrentObject = (T)SelectedItem;
                else if (SelectedItem is ListItem<T>)
                    CurrentObject = ((ListItem<T>)SelectedItem).Value;
                else if (SelectedItem is ListItem && ((ListItem)SelectedItem).Value is T)
                    CurrentObject = (T)((ListItem)SelectedItem).Value;

                if (Service != null)
                    Service.CloseDropDown();
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
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    PaintHelper.DrawHoverBackgroundFlat(e.Graphics, rect, SystemColors.Highlight);
                }
                else
                {
                    e.Graphics.DrawLine(SystemPens.ControlLight, rect.Left + 2, rect.Bottom - 1, rect.Right - 2, rect.Bottom - 1);
                }

                if (Owner != null)
                {
                    var item = Items[e.Index];
                    if(item is ListItem<T>)
                        Owner.DrawListItem(e, rect, (ListItem<T>)item);
                }
            }
        }
    }
}
