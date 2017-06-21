using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Blumind.Core;
using Blumind.Design;
using Blumind.Globalization;
using Blumind.Model.Styles;

namespace Blumind.Model.MindMaps
{
    //Design Time Only, Style Members
    partial class Topic
    {
        [DefaultValue(typeof(Color), "Empty")]
        [DesignOnly(true), LocalDisplayName("Back Color"), LocalCategory("Color")]
        public Color BackColor
        {
            get { return Style.BackColor; }
            set { Style.BackColor = value; }
        }

        [DefaultValue(typeof(Color), "Empty")]
        [DesignOnly(true), LocalDisplayName("Fore Color"), LocalCategory("Color")]
        public Color ForeColor
        {
            get { return Style.ForeColor; }
            set { Style.ForeColor = value; }
        }

        [DefaultValue(typeof(Color), "Empty")]
        [DesignOnly(true), LocalDisplayName("Line Color"), LocalCategory("Color")]
        public Color LineColor
        {
            get { return Style.LineColor; }
            set { Style.LineColor = value; }
        }

        [DefaultValue(typeof(Color), "Empty")]
        [DesignOnly(true), LocalDisplayName("Border Color"), LocalCategory("Color")]
        public Color BorderColor
        {
            get { return Style.BorderColor; }
            set { Style.BorderColor = value; }
        }

        [DefaultValue(null)]
        [DesignOnly(true), LocalDisplayName("Font"), LocalCategory("Appearance")]
        public Font Font
        {
            get { return Style.Font; }
            set { Style.Font = value; }
        }

        [DefaultValue(TopicShape.Default)]
        [DesignOnly(true), LocalDisplayName("Shape"), LocalCategory("Appearance")]
        public TopicShape Shape
        {
            get { return Style.Shape; }
            set { Style.Shape = value; }
        }
        
        [DefaultValue(null)]
        [DesignOnly(true), LocalDisplayName("Fill Type"), LocalCategory("Appearance")]
        [Editor(typeof(FillTypeEditor), typeof(UITypeEditor))]
        public string FillType
        {
            get { return Style.FillType; }
            set { Style.FillType = value; }
        }

        [DefaultValue(typeof(Padding), "2, 2, 2, 2")]
        [DesignOnly(true), LocalDisplayName("Padding"), LocalCategory("Layout")]
        [Editor(typeof(PaddingEditor), typeof(UITypeEditor)), TypeConverter(typeof(Blumind.Design.PaddingConverter))]
        public Padding Padding
        {
            get { return Style.Padding; }
            set { Style.Padding = value; }
        }

        [DefaultValue(TopicStyle.DefaultRoundRadius)]
        [DesignOnly(true), LocalDisplayName("Round Radius"), LocalCategory("Appearance")]
        public int RoundRadius
        {
            get { return Style.RoundRadius; }
            set { Style.RoundRadius = value; }
        }

        [DefaultValue(HorizontalAlignment.Center)]
        [DesignOnly(true), LocalDisplayName("Text Alignment"), LocalCategory("Appearance")]
        [Editor(typeof(HorizontalAlignmentEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(HorizontalAlignmentConverter))]
        public HorizontalAlignment TextAlignment
        {
            get { return Style.TextAlignment; }
            set { Style.TextAlignment = value; }
        }

        private Padding DefaultPadding
        {
            get
            {
                if (IsRoot)
                    return new Padding(TopicStyle.DefaultRootPadding);
                else
                    return new Padding(TopicStyle.DefaultNodePadding);
            }
        }

        internal void ResetPadding()
        {
            Padding = DefaultPadding;
        }
    }
}
