using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Linq;
using Blumind.Core.Documents;
using Blumind.Model;
using Blumind.Model.Documents;
using Blumind.Model.Widgets;

namespace Blumind.Model.MindMaps
{
    [Serializable]
    struct MapClipboardData
    {
        string Data;

        public MapClipboardData(ChartObject[] mapObjects)
        {
            Data = null;

            SetChartObjects(mapObjects);
        }

        void SetChartObjects(ChartObject[] mapObjects)
        {
            var dom = new XmlDocument();
            dom.LoadXml("<?xml version='1.0' encoding='utf-8'?><nodes/>");

            foreach (var mapObject in mapObjects)
            {
                if (mapObject is Topic)
                {
                    dom.DocumentElement.AppendChild(Topic.SerializeTopic(dom, (Topic)mapObject));
                }
                else if(!string.IsNullOrEmpty(mapObject.XmlElementName))
                {
                    var node = dom.CreateElement(mapObject.XmlElementName);
                    mapObject.Serialize(dom, node);
                    dom.DocumentElement.AppendChild(node);
                }
            }

            Data = dom.OuterXml;
        }

        public Topic[] GetTopics()
        {
            var objs = GetChartObjects();
            if (objs == null)
                return null;
            else
                return objs.OfType<Topic>().ToArray();
        }

        public ChartObject[] GetChartObjects()
        {
            if (string.IsNullOrEmpty(Data))
                return null;

            XmlDocument dom = new XmlDocument();
            try
            {
                dom.LoadXml(Data);
            }
            catch(System.Exception ex)
            {
                Helper.WriteLog(ex);
                return null;
            }

            if (dom.DocumentElement != null)
            {
                var mapObjects = new List<ChartObject>();
                var xmlNode = dom.DocumentElement.FirstChild;
                while (xmlNode != null)
                {
                    if (xmlNode is XmlElement)
                    {
                        var xmlElement = (XmlElement)xmlNode;
                        switch (xmlElement.Name)
                        {
                            case Topic._XmlElementName:
                                var topic = Topic.DeserializeTopic(Document.DocumentVersion, xmlElement);
                                if(topic != null)
                                    mapObjects.Add(topic);
                                break;
                            case Widget._XmlElementName:
                                var widget = Widget.DeserializeWidget(Document.DocumentVersion, xmlElement);
                                if(widget != null)
                                    mapObjects.Add(widget);
                                break;
                            case Link._XmlElementName:
                                var link = new Link();
                                link.Deserialize(Document.DocumentVersion, xmlElement);
                                mapObjects.Add(link);
                                break;
                        }
                    }

                    xmlNode = xmlNode.NextSibling;
                }

                //XmlNodeList nodes = dom.DocumentElement.SelectNodes(Topic._XmlElementName);
                //foreach (XmlNode node in dom.DocumentElement.FirstChild)
                //{
                //    if (node is XmlElement)
                //    {


                //        var topic = Topic.DeserializeTopic(Document.DocumentVersion, (XmlElement)node);
                //        mapObjects.Add(topic);
                //    }
                //}

                return mapObjects.ToArray();
            }
            else
            {
                return null;
            }
        }
    }
}
