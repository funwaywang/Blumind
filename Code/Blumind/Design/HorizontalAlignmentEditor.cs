using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Globalization;

namespace Blumind.Design
{
    class HorizontalAlignmentEditor : EnumEditor<HorizontalAlignment>
    {
        protected override void DrawListItem(DrawItemEventArgs e, Rectangle rect, ListItem<HorizontalAlignment> listItem)
        {
            //base.DrawListItem(e, rect, listItem);            
            Image icon = null;
            switch (listItem.Value)
            {
                case HorizontalAlignment.Left:
                    icon = Properties.Resources.edit_alignment;
                    break;
                case HorizontalAlignment.Center:
                    icon = Properties.Resources.edit_alignment_center;
                    break;
                case HorizontalAlignment.Right:
                    icon = Properties.Resources.edit_alignment_right;
                    break;
            }

            DrawIcon(e.Graphics, icon, ref rect);

            DrawItemText(e, rect, Lang._(listItem.Value.ToString()));
        }
    }

    class HorizontalAlignmentConverter : BaseTypeConverter
    {
        protected override object ConvertValueToString(object value)
        {
            if (value is HorizontalAlignment)
            {
                switch ((HorizontalAlignment)value)
                {
                    case HorizontalAlignment.Left:
                        return Lang._("Left");
                    case HorizontalAlignment.Center:
                        return Lang._("Center");
                    case HorizontalAlignment.Right:
                        return Lang._("Right");
                }
            }

            return base.ConvertValueToString(value);
        }
    }
}
