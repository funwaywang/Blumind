using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Design
{
    class LineStyleEditor : ListEditor<DashStyle>
    {
        protected override DashStyle[] GetStandardValues()
        {
            return new DashStyle[]{
                    DashStyle.Solid,
                    DashStyle.Dash,
                    DashStyle.Dot,
                    DashStyle.DashDot,
                    DashStyle.DashDotDot,
                };
        }

        protected override IEnumerable<ListItem<DashStyle>> GetStandardItems()
        {
            foreach (var ds in GetStandardValues())
            {
                yield return new ListItem<DashStyle>(LineStyleConverter._ConvertToString(ds), ds);
            }
        }

        protected override int ListControlMinWidth
        {
            get
            {
                return 160;
            }
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.White, e.Bounds);

            bool isend = e.Context.PropertyDescriptor.Name == "EndCap";
            Pen pen = new Pen(Color.Black, 2);
            pen.DashStyle = (DashStyle)e.Value;
            Point pt = PaintHelper.CenterPoint(e.Bounds);
            e.Graphics.DrawLine(pen, e.Bounds.Left + 2, pt.Y, e.Bounds.Right - 2, pt.Y);
        }

        protected override void DrawListItem(DrawItemEventArgs e, Rectangle rect, ListItem<DashStyle> listItem)
        {
            var value = listItem.Value;

            //base.DrawListItem(e, rect, value);
            Color foreColor = ((e.State & DrawItemState.Selected) == DrawItemState.Selected) ? SystemColors.HighlightText : SystemColors.WindowText;
            Point pt = PaintHelper.CenterPoint(rect);
            rect.Inflate(-4, 0);
            Pen pen = new Pen(foreColor, 2);
            pen.DashStyle = value;
            e.Graphics.DrawLine(pen, rect.Left, pt.Y, rect.Left + 60, pt.Y);
            rect.X += 70;
            rect.Width -= 70;

            DrawItemText(e, rect, listItem.Text);
        }
    }

    class LineStyleConverter : BaseTypeConverter
    {
        protected override object ConvertValueToString(object value)
        {
            if (value is DashStyle)
            {
                return _ConvertToString((DashStyle)value);
            }

            return base.ConvertValueToString(value);
        }

        internal static string _ConvertToString(DashStyle dashStyle)
        {
            switch (dashStyle)
            {
                case DashStyle.Solid:
                    return Lang._("Solid");
                case DashStyle.Dash:
                    return Lang._("Dash");
                case DashStyle.Dot:
                    return Lang._("Dot");
                case DashStyle.DashDot:
                    return Lang._("Dash Dot");
                case DashStyle.DashDotDot:
                    return Lang._("Dash Dot Dot");
                default:
                    return dashStyle.ToString();
            }
        }
    }
}
