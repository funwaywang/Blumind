using System.ComponentModel;
using System.Drawing.Design;
using Blumind.Design;

namespace Blumind.Model.Styles
{
    [Editor(typeof(TopicShapeEditor), typeof(UITypeEditor))]
    [TypeConverter(typeof(TopicShapeConverter))]
    public enum TopicShape
    {
        Default,
        Rectangle,
        Ellipse,
        BaseLine,
        None,
    }
}
