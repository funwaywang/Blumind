using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Blumind.ChartControls.Shapes;
using Blumind.Controls;
using Blumind.Controls.MapViews;
using Blumind.Core;
using Blumind.Globalization;
using Blumind.Model.Styles;

namespace Blumind.Design
{
    class TopicShapeEditor : ListEditor<TopicShape> 
    {
        protected override TopicShape[] GetStandardValues()
        {
            return (TopicShape[])Enum.GetValues(typeof(TopicShape));
        }

        protected override IEnumerable<ListItem<TopicShape>> GetStandardItems()
        {
            foreach (TopicShape ts in Enum.GetValues(typeof(TopicShape)))
            {
                yield return new ListItem<TopicShape>(TopicShapeConverter._ConvertToString(ts), ts);
            }
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.White, e.Bounds);
            if (e.Value is TopicShape)
            {
                TopicShape ts = (TopicShape)e.Value;
                Shape shape = Shape.GetShaper(ts, 3);
                PaintEventArgs pea = new PaintEventArgs(e.Graphics, e.Bounds);
                shape.DrawIcon(pea);
            }
        }

        private static void DrawIcon(Graphics graphics, TopicShape value, Rectangle rect)
        {
            Shape shape = Shape.GetShaper(value, 3);
            PaintEventArgs pea = new PaintEventArgs(graphics, rect);
            shape.DrawIcon(pea);
        }

        protected override void DrawListItem(DrawItemEventArgs e, Rectangle rect, ListItem<TopicShape> listItem)
        {
            var value = listItem.Value;

            Brush brushFore = ((e.State & DrawItemState.Selected) == DrawItemState.Selected) ? SystemBrushes.HighlightText : SystemBrushes.WindowText;
            bool isdefault = value == TopicShape.Default;
            rect.Inflate(-1, -1);

            // draw icon
            Rectangle rectImage = new Rectangle(rect.Left, rect.Y + (rect.Height - 16) / 2, 16, 16);
            TopicShapeEditor.DrawIcon(e.Graphics, value, rectImage);
            rect.X += 20;
            rect.Width -= 20;

            // draw text
            StringFormat sf = new StringFormat(PaintHelper.SFLeft);
            sf.FormatFlags |= StringFormatFlags.NoWrap;
            Font font = e.Font;
            //string str = TopicShapeConverter._ConvertToString(value);
            if (isdefault)
            {
                font = new Font(font, font.Style | FontStyle.Bold);
            }
            e.Graphics.DrawString(listItem.Text, font, brushFore, rect, sf);
        }
    }

    class TopicShapeConverter : BaseTypeConverter
    {
        protected override object ConvertValueToString(object value)
        {
            if (value is TopicShape)
            {
                return _ConvertToString((TopicShape)value);
            }

            return base.ConvertValueToString(value);
        }

        internal static string _ConvertToString(TopicShape shape)
        {
            switch (shape)
            {
                case TopicShape.Default:
                    return Lang._("Default");
                case TopicShape.BaseLine:
                    return Lang._("Base Line");
                case TopicShape.Ellipse:
                    return Lang._("Ellipse");
                case TopicShape.Rectangle:
                    return Lang._("Rectangle");
                case TopicShape.None:
                    return Lang._("None");
                default:
                    return shape.ToString();
            }
        }
    }
}
