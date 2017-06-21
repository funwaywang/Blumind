using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Blumind.Core;
using Blumind.Model;
using Blumind.Model.MindMaps;

namespace Blumind.Controls.MapViews
{
    interface IMindMapRender
    {
        //Size Layout(MindMap map, RenderArgs e);

        void Paint(MindMap map, RenderArgs e);

        void PaintNavigationMap(MindMap map, float zoom, PaintEventArgs e);

        void PaintTopic(Topic topic, RenderArgs e);

        void PaintTopics(IEnumerable<Topic> topics, RenderArgs e);
    }
}
