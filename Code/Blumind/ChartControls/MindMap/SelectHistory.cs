using System.Collections.Generic;
using Blumind.Core;
using System;
using Blumind.Model;

namespace Blumind.Controls.MapViews
{
    class SelectHistory
    {
        private List<ChartObject[]> Records;
        private int MaxSize;

        public SelectHistory(int maxSize)
        {
            MaxSize = maxSize;
            Records = new List<ChartObject[]>(maxSize);
        }

        public int Count
        {
            get { return Records.Count; }
        }

        public void Push(ChartObject[] topic)
        {
            //if (Records.Contains(topic))
            //{
            //    Records.Remove(topic);
            //}

            Records.Add(topic);

            while (Records.Count > MaxSize)
            {
                Records.RemoveAt(0);
            }
        }

        public ChartObject[] Pop()
        {
            if (Records.Count > 0)
            {
                ChartObject[] result = Records[Records.Count - 1];
                Records.RemoveAt(Records.Count - 1);
                return result;
            }
            else
            {
                return null;
            }
        }

        public void Remove(ChartObject mapObject)
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                ChartObject[] objs = Records[i];
                if (objs != null)
                {
                    int index = Array.IndexOf(objs, mapObject);
                    if (index > -1)
                    {
                        List<ChartObject> list = new List<ChartObject>(objs);
                        list.Remove(mapObject);
                        if (list.Count > 0)
                            Records[i] = list.ToArray();
                        else
                            Records.RemoveAt(i);
                    }
                }
            }
        }
    }
}
