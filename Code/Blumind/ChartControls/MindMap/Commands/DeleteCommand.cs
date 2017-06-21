using System;
using Blumind.Core;
using Blumind.Model.Widgets;
using Blumind.Model;
using Blumind.Model.MindMaps;
using System.Collections.Generic;

namespace Blumind.Controls.MapViews
{
    class DeleteCommand : Command
    {
        ChartObject[] MapObjects;
        Dictionary<ChartObject, object> Parents;
        Dictionary<ChartObject, int> Indices;

        public DeleteCommand(ChartObject[] mapObjects)
        {
            MapObjects = mapObjects;

            if (MapObjects == null || MapObjects.Length == 0)
            {
                throw new ArgumentNullException();
            }
        }

        public override string Name
        {
            get { return "Delete"; }
        }

        public override bool Rollback()
        {
            return UndeleteObjects(MapObjects, Parents, Indices);
        }

        public override bool Execute()
        {
            Parents = new Dictionary<ChartObject, object>();
            Indices = new Dictionary<ChartObject, int>();
            return DeleteObjects(MapObjects, Parents, Indices);
        }

        public static bool DeleteObjects(ChartObject[] mapObjects,
            Dictionary<ChartObject, object> parents, Dictionary<ChartObject, int> indices)
        {
            bool changed = false;

            for (int i = 0; i < mapObjects.Length; i++)
            {
                ChartObject mo = mapObjects[i];
                if (mo is Link)
                {
                    Link line = (Link)mo;
                    if (line.From != null)
                    {
                        parents[mo] = line.From;
                        line.From.Links.Remove(line);
                        changed = true;
                    }
                }
                else if (mo is Topic)
                {
                    Topic topic = (Topic)mapObjects[i];
                    if (topic.ParentTopic != null)
                    {
                        parents[mo] = topic.ParentTopic;
                        indices[mo] = topic.Index;

                        topic.ParentTopic.Children.Remove(topic);
                        changed = true;
                    }
                }
                else if (mo is Widget)
                {
                    Widget widget = (Widget)mapObjects[i];
                    if (widget.WidgetContainer != null)
                    {
                        parents[mo] = widget.WidgetContainer;
                        indices[mo] = widget.WidgetContainer.IndexOf(widget);

                        widget.WidgetContainer.Remove(widget);
                        changed = true;
                    }
                }
            }

            return changed;
        }

        public static bool UndeleteObjects(ChartObject[] MapObjects, Dictionary<ChartObject, object> Parents, Dictionary<ChartObject, int> Indices)
        {
            if (MapObjects.Length > 0)
            {
                for (int i = 0; i < MapObjects.Length; i++)
                {
                    ChartObject mo = MapObjects[i];
                    object parent = null;
                    int index = -1;
                    Parents.TryGetValue(mo, out parent);
                    Indices.TryGetValue(mo, out index);

                    if (mo is Topic)
                    {
                        Topic topic = (Topic)mo;

                        if (parent is Topic)
                        {
                            Topic parentTopic = (Topic)parent;
                            if (index > -1 && index < parentTopic.Children.Count)
                                parentTopic.Children.Insert(index, topic);
                            else
                                parentTopic.Children.Add(topic);
                        }
                    }
                    else if (mo is Link)
                    {
                        if (parent is Topic)
                        {
                            Topic parentTopic = (Topic)parent;
                            parentTopic.Links.Add((Link)mo);
                        }
                    }
                    else if (mo is Widget)
                    {
                        if (parent is IWidgetContainer)
                        {
                            var container = (IWidgetContainer)parent;
                            if (index > -1 && index < container.WidgetsCount)
                                container.Insert(index, (Widget)mo);
                            else
                                container.Add((Widget)mo);
                        }
                    }
                }
            }

            return true;
        }
    }
}
