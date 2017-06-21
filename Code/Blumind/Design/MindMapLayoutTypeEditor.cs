using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Blumind.Controls;
using Blumind.Core;
using Blumind.Model.MindMaps;

namespace Blumind.Design
{
    class MindMapLayoutTypeEditor : ListEditor<MindMapLayoutType> 
    {
        protected override MindMapLayoutType[] GetStandardValues()
        {
            return (MindMapLayoutType[])Enum.GetValues(typeof(MindMapLayoutType));
        }

        protected override IEnumerable<ListItem<MindMapLayoutType>> GetStandardItems()
        {
            foreach (MindMapLayoutType lt in Enum.GetValues(typeof(MindMapLayoutType)))
            {
                yield return new ListItem<MindMapLayoutType>(
                    MindMapLayoutTypeConverter._ConvertToString(lt), lt, GetIcon(lt));
            }
            //return base.GetStandardItems();
        }

        protected override int ListControlMinWidth
        {
            get
            {
                return 200;
            }
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.White, e.Bounds);
            if (e.Value is MindMapLayoutType)
            {
                MindMapLayoutType lt = (MindMapLayoutType)e.Value;
                Image image = GetIcon(lt);
                if(image != null)
                {
                    PaintHelper.DrawImageInRange(e.Graphics, image, e.Bounds);
                }
            }
        }

        public static Image GetIcon(MindMapLayoutType layoutType)
        {
            switch (layoutType)
            {
                case MindMapLayoutType.MindMap:
                    return Properties.Resources.layout_mind_map;
                case MindMapLayoutType.OrganizationDown:
                    return Properties.Resources.layout_org_down;
                case MindMapLayoutType.OrganizationUp:
                    return Properties.Resources.layout_org_up;
                case MindMapLayoutType.TreeLeft:
                    return Properties.Resources.layout_tree_left;
                case MindMapLayoutType.TreeRight:
                    return Properties.Resources.layout_tree_right;
                case MindMapLayoutType.LogicLeft:
                    return Properties.Resources.layout_logic_left;
                case MindMapLayoutType.LogicRight:
                    return Properties.Resources.layout_logic_right;
                default:
                    return null;
            }
        }

        protected override void DrawListItem(DrawItemEventArgs e, Rectangle rect, ListItem<MindMapLayoutType> listItem)
        {
            var value = listItem.Value;

            Brush brushFore = ((e.State & DrawItemState.Selected) == DrawItemState.Selected) ? SystemBrushes.HighlightText : SystemBrushes.WindowText;
            bool isdefault = value == MindMapLayoutType.MindMap;
            rect.Inflate(-1, -1);

            // draw icon
            //Image image = MindMapLayoutTypeEditor.GetIcon(value);
            var image = listItem.Image;
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
            Font font = e.Font;
            string str = MindMapLayoutTypeConverter._ConvertToString(value);
            if (isdefault)
            {
                font = new Font(font, font.Style | FontStyle.Bold);
                //str += string.Format(" ({0})", LanguageManage.GetText("Default"));
            }
            e.Graphics.DrawString(str, font, brushFore, rect, sf);
        }
    }

    class MindMapLayoutTypeConverter : BaseTypeConverter
    {
        protected override object ConvertValueToString(object value)
        {
            if (value is MindMapLayoutType)
            {
                return _ConvertToString((MindMapLayoutType)value);
            }

            return base.ConvertValueToString(value);
        }

        internal static string _ConvertToString(MindMapLayoutType lt)
        {
            return ST.EnumToString(lt);
            //switch (lt)
            //{
            //    case MindMapLayoutType.MindMap:
            //        return LanguageManage.GetText("Mind Map Chart");
            //    case MindMapLayoutType.OrganizationDown:
            //        return LanguageManage.GetText("Organization Chart (Down)");
            //    case MindMapLayoutType.OrganizationUp:
            //        return LanguageManage.GetText("Organization Chart (Up)");
            //    case MindMapLayoutType.TreeLeft:
            //        return LanguageManage.GetText("Tree Chart (Left)");
            //    case MindMapLayoutType.TreeRight:
            //        return LanguageManage.GetText("Tree Chart (Right)");
            //    case MindMapLayoutType.LogicLeft:
            //        return LanguageManage.GetText("Logic Chart (Left)");
            //    case MindMapLayoutType.LogicRight:
            //        return LanguageManage.GetText("Logic Chart (Right)");
            //    default:
            //        return lt.ToString();
            //}
        }
    }
}
