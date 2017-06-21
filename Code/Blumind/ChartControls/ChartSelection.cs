using System;
using System.Collections;
using System.Collections.Generic;
using Blumind.Core;
using Blumind.Model;

namespace Blumind.Controls.MapViews
{
    class ChartSelection : XList<ChartObject>
    {
        internal SelectHistory SelectHistory;

        public ChartSelection()
        {
            SelectHistory = new SelectHistory(30);
        }

        public ChartSelection(ChartObject[] objects)
            : this()
        {
            AddRange(objects);
        }

        public void Update(ChartObject[] mapObjects, bool exclusive)
        {
            if (exclusive)
            {
                for (int i = Count - 1; i >= 0; i--)
                {
                    ChartObject mapObject = this[i];
                    if (Array.IndexOf(mapObjects, mapObject) < 0)
                    {
                        mapObject.Selected = false;
                        RemoveAt(i);
                    }
                }

                foreach (ChartObject mapObject in mapObjects)
                {
                    if (!Contains(mapObject))
                    {
                        mapObject.Selected = true;
                        Add(mapObject);
                    }
                }
            }
            else
            {
                foreach (ChartObject mapObject in mapObjects)
                {
                    mapObject.Selected = true;
                    if (!Contains(mapObject))
                    {
                        Add(mapObject);
                    }
                }
            }
        }

        public void PushHistory()
        {
            SelectHistory.Push(this.ToArray());
        }

        public void RemoveInHistory(ChartObject mapObject)
        {
            SelectHistory.Remove(mapObject);
        }

        public ChartObject[] PopHistory()
        {
            return SelectHistory.Pop();
        }

        public T[] GetSelectedObjects<T>()
            where T : ChartObject
        {
            List<T> list = new List<T>();
            foreach (var co in this)
            {
                if (co is T)
                    list.Add((T)co);
            }

            return list.ToArray();
        }
    }
}
