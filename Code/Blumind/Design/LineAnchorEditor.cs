using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Blumind.Canvas;
using Blumind.Canvas.GdiPlus;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Design
{
    class LineAnchorEditor : ListEditor<LineAnchor>
    {
        private bool IsEndCap = false;

        protected override LineAnchor[] GetStandardValues()
        {
            return new LineAnchor[]{
                LineAnchor.None,
                LineAnchor.Arrow,
                LineAnchor.Round,
                LineAnchor.Square,
                LineAnchor.Diamond};
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value is LineAnchor)
            {
                IsEndCap = context.PropertyDescriptor.Name == "EndCap";
            }

            return base.EditValue(context, provider, value);
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /*public override void PaintValue(PaintValueEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.White, e.Bounds);
            if (e.Value is LineAnchor && ((LineCap)e.Value) != LineCap.Flat)
            {
                Pen pen = new Pen(Color.Black, 5);
                Point pt = PaintHelper.CenterPoint(e.Bounds);
                if (IsEndCap)
                {
                    pen.EndCap = (LineCap)e.Value;
                    e.Graphics.DrawLine(pen, e.Bounds.Left + 2, pt.Y, e.Bounds.Right - pen.Width - 2, pt.Y);
                }
                else
                {
                    pen.StartCap = (LineCap)e.Value;
                    e.Graphics.DrawLine(pen, e.Bounds.Left + pen.Width + 2, pt.Y, e.Bounds.Right - 2, pt.Y);
                }
            }
        }*/

        protected override void DrawListItem(DrawItemEventArgs e, Rectangle rect, ListItem<LineAnchor> listItem)
        {
            var value = listItem.Value;

            //base.DrawItem(e, rect, value);
            if (value != LineAnchor.None)
            {
                Color foreColor = ((e.State & DrawItemState.Selected) == DrawItemState.Selected) ? SystemColors.HighlightText : SystemColors.WindowText;
                Point pt = PaintHelper.CenterPoint(rect);
                rect.Inflate(-10, 0);
                rect.X += 5;
                rect.Width -= 5;

                IGraphics grf = new GdiGraphics(e.Graphics);
                var gs = grf.Save();
                grf.SetHighQualityRender();
                var pen = grf.Pen(foreColor, 5);
                if (IsEndCap)
                    grf.DrawLine(pen, rect.Left, pt.Y, rect.Right, pt.Y, LineAnchor.None, (LineAnchor)value);
                else
                    grf.DrawLine(pen, rect.Left, pt.Y, rect.Right, pt.Y, (LineAnchor)value, LineAnchor.None);
                grf.Restore(gs);
            }
        }
    }

    class LineCapConverter : BaseTypeConverter
    {
        protected override object ConvertValueToString(object value)
        {
            if (value is LineCap)
            {
                switch ((LineCap)value)
                {
                    case LineCap.Flat:
                        return Lang._("None");
                    case LineCap.Square:
                        return string.Empty;// LanguageManage.GetText("Square");
                    case LineCap.Round:
                        return string.Empty;// LanguageManage.GetText("Round");
                    case LineCap.Triangle:
                        return string.Empty;// LanguageManage.GetText("Triangle");
                    //case LineCap.NoAnchor:
                    //    break;
                    case LineCap.SquareAnchor:
                        return string.Empty;// LanguageManage.GetText("Square Anchor");
                    case LineCap.RoundAnchor:
                        return string.Empty;// LanguageManage.GetText("Round Anchor");
                    case LineCap.DiamondAnchor:
                        return string.Empty;// LanguageManage.GetText("Diamond Anchor");
                    case LineCap.ArrowAnchor:
                        return string.Empty;// LanguageManage.GetText("Arrow Anchor");
                    //case LineCap.AnchorMask:
                    //    break;
                    //case LineCap.Custom:
                }
            }

            return base.ConvertValueToString(value);
        }
    }
}
