using System;
using System.Collections.Generic;
using System.Text;
using Blumind.Core;
using Blumind.Model;
using Blumind.Model.Documents;
using Blumind.Model.MindMaps;

#if DEBUG
namespace Blumind.Test
{
    class BigMap
    {
        private int Count;
        private int LayerCount;
        
        public BigMap(int count, int layerCount)
        {
            Count = count;
            LayerCount = Math.Max(1, layerCount);
        }

        public Document CreateDocument()
        {
            Document doc = new Document();
            doc.Name = "Big Map";
            doc.Charts.Add(Create());
            return doc;
        }

        public MindMap Create()
        {
            MindMap map = new MindMap("Big Map");

            Topic root = new Topic("Root");
            map.Root = root;

            CreateTopics(root, 1, Count);

            return map;
        }

        private void CreateTopics(Topic parent, int layer, int count)
        {
            count -= LayerCount;
            int batch = count / LayerCount;
            if (count % LayerCount > 0)
                batch += 1;

            int c = 0;
            for (int i = 0; i < LayerCount; i++)
            {
                Topic topic = new Topic(string.Format("Topic {0}-{1}", layer, i));
                topic.Collapse();
                parent.Children.Add(topic);
                batch = Math.Min(count - c, batch);
                if (batch > 0)
                {
                    CreateTopics(topic, layer+1, batch);
                    count += batch;
                }
            }
        }
    }
}
#endif
