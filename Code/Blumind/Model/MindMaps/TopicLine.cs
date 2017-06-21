using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml;
using Blumind.Controls;
using Blumind.Core;

namespace Blumind.Model.MindMaps
{
    class TopicLine : ISerializable
    {
        public TopicLine()
        {
        }

        public TopicLine(Topic target, Vector4 beginSide, Vector4 endSide, Rectangle beginRect, Rectangle endRect)
        {
            Target = target;
            BeginSide = beginSide;
            EndSide = endSide;
            BeginRectangle = beginRect;
            EndRectangle = endRect;
        }

        public Topic Target { get; private set; }

        public Vector4 BeginSide { get; private set; }

        public Vector4 EndSide { get; private set; }

        public Rectangle BeginRectangle { get; private set; }

        public Rectangle EndRectangle { get; private set; }

        public void SetTarget(Topic topic)
        {
            Target = topic;
        }

        public void Offset(int x, int y)
        {
            var br = BeginRectangle;
            br.Offset(x, y);
            BeginRectangle = br;

            var er = EndRectangle;
            er.Offset(x, y);
            EndRectangle = er;
        }

        public void Serialize(XmlDocument dom, XmlElement node)
        {
            node.SetAttribute("target", Target.ID);
            node.SetAttribute("begin_side", BeginSide.ToString());
            node.SetAttribute("end_side", EndSide.ToString());
            node.SetAttribute("begin_rect", ST.ToString(BeginRectangle));
            node.SetAttribute("end_rect", ST.ToString(EndRectangle));
        }

        public void Deserialize(Version documentVersion, XmlElement node)
        {
            BeginSide = ST.GetEnumValue(node.GetAttribute("begin_side"), BeginSide);
            EndSide = ST.GetEnumValue(node.GetAttribute("end_side"), EndSide);
            BeginRectangle = ST.GetRectangle(node.GetAttribute("begin_rect"), BeginRectangle);
            EndRectangle = ST.GetRectangle(node.GetAttribute("end_rect"), EndRectangle);
        }
    }
}
