using System.Drawing;
using Blumind.Controls;
using System.Xml;
using Blumind.Canvas;
using Blumind.Model.Styles;

namespace Blumind.ChartControls.MindMap.Lines
{
    interface ILine
    {
        void DrawLine(IGraphics graphics, IPen pen,
            TopicShape shapeFrom, TopicShape shapeTo, 
            Rectangle rectFrom, Rectangle rectTo, Vector4 vectorFrom, Vector4 vectorTo, 
            LineAnchor startAnchor, LineAnchor endAnchor);
    }
}
