using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Blumind.Controls.MapViews;
using Blumind.Core;

namespace Blumind.Model.Widgets
{
    [DefaultProperty("Hyperlink")]
    public class HyperlinkWidget : Widget
    {
        public const string TypeID = "HYPERLINK";
        private string _Url;
        private string _Title;

        public HyperlinkWidget()
        {
            Alignment = WidgetAlignment.Right;
            ResponseMouse = true;
        }

        [DefaultValue(null), LocalCategory("Hyperlink"), LocalDisplayName("Hyperlink")]
        public string Url
        {
            get { return _Url; }
            set 
            {
                if (_Url != value)
                {
                    string old = _Url;
                    _Url = value;
                    OnPropertyChanged("Hyperlink", old, _Url, PropertyChangeTypes.Data | PropertyChangeTypes.Visual);
                }
            }
        }

        [DefaultValue(null), LocalDisplayName("Title")]
        public string Title
        {
            get { return _Title; }
            set 
            {
                if (_Title != value)
                {
                    string old = _Title;
                    _Title = value;
                    OnPropertyChanged("Title", old, _Title, PropertyChangeTypes.All);
                }
            }
        }

        [DefaultValue(WidgetAlignment.Right)]
        public override WidgetAlignment Alignment
        {
            get
            {
                return base.Alignment;
            }
            set
            {
                base.Alignment = value;
            }
        }

        public override void CopyTo(Widget widget)
        {
            if (widget is HyperlinkWidget)
            {
                HyperlinkWidget hl = (HyperlinkWidget)widget;
                hl.Title = Title;
                hl.Url = Url;
            }

            base.CopyTo(widget);
        }

        public override System.Windows.Forms.Cursor Cursor
        {
            get
            {
                if (Helper.TestModifierKeys(Keys.Shift))
                    return Cursors.Hand;
                else
                    return null;
            }
        }

        protected override string GetTooltip()
        {
            string text = Title;
            if (!string.IsNullOrEmpty(Url))
            {
                if (!string.IsNullOrEmpty(text))
                    text += System.Environment.NewLine;
                text += string.Format("Shift + Click here to visit \"{0}\"", Url);
            }

            return text;
        }

        public override Size CalculateSize(RenderArgs e)
        {
            //string text = Text;
            //if (string.IsNullOrEmpty(Text))
            //    text = Url;

            //if (!string.IsNullOrEmpty(text))
            //{
            //    Size size = Size.Ceiling(e.Graphics.MeasureString(text, e.Font));
            //    return size;
            //}
            //else
            //{
            //    return Size.Empty;
            //}
            return new Size(16, 16);
        }

        public override void Paint(RenderArgs e)
        {
            //base.Paint(e);
            Image icon = Properties.Resources.hyperlink;
            Rectangle rect = Bounds;
            rect.X += Math.Max(0, (rect.Width - icon.Width) / 2);
            rect.Y += Math.Max(0, (rect.Height - icon.Height) / 2);
            rect.Width = Math.Min(rect.Width, icon.Width);
            rect.Height = Math.Min(rect.Height, icon.Height);
            e.Graphics.DrawImage(icon, rect, 0, 0, icon.Width, icon.Height, GraphicsUnit.Pixel);

            if (Hover)
            {
                e.Graphics.DrawLine(Pens.Blue, Bounds.X, Bounds.Bottom, Bounds.Right, Bounds.Bottom);
            }
        }

        public override string GetTypeID()
        {
            return TypeID;
        }

        public override bool Serialize(XmlElement node)
        {
            ST.WriteTextNode(node, "url", Url);
            return base.Serialize(node);
        }

        public override bool Deserialize(XmlElement node)
        {
            Url = ST.ReadTextNode(node, "url");
            return base.Deserialize(node);
        }

        public override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Helper.TestModifierKeys(Keys.Shift))
            {
                if (!string.IsNullOrEmpty(Url))
                {
                    Helper.OpenUrl(Url);
                }
            }
            base.OnMouseClick(e);
        }
    }
}
