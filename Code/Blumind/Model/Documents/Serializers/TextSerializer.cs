using System;
using System.Collections.Generic;
using System.Text;
using Blumind.Model;
using Blumind.Model.MindMaps;
using Blumind.Model.Widgets;

namespace Blumind.Core.Documents
{
    class TextSerializer
    {
        //public string SerializeObjects(ChartObject[] mapObjects)
        //{
        //    return SerializeObjects(mapObjects, false);
        //}

        public string SerializeObjects(ChartObject[] mapObjects, bool recursive)
        {
            var accessedObjects = new List<ChartObject>();
            var sb = new StringBuilder();
            foreach (var co in mapObjects)
            {
                if (accessedObjects.Contains(co))
                    continue;

                if (co is Topic)
                {
                    SerializeTopic(sb, (Topic)co, 0, recursive, mapObjects, accessedObjects);
                }
                else
                {
                    sb.AppendLine(co.SerializeText(recursive, accessedObjects));
                }
            }
            //for (int i = 0; i < mapObjects.Length; i++)
            //{
            //    if (mapObjects[i] is Topic)
            //    {
            //        SerializeTopic(sb, (Topic)mapObjects[i], 0);
            //    }
            //}
            return sb.ToString();
        }

        public Topic[] DeserializeTopic(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new Topic[0];

            List<int> levels = new List<int>();
            List<Topic> topics = new List<Topic>();
            string[] lines = text.Split('\r', '\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                int level;
                Topic topic = DeserializeSingleTopic(line, out level);

                if (topic != null)
                {
                    levels.Add(level);
                    topics.Add(topic);
                }
            }
            
            // reset levels
            if (topics.Count > 1)
            {
                for (int i = topics.Count - 1; i >= 1; i--)
                {
                    int j = i-1;
                    while (j >= 0 && levels[i] <= levels[j])
                    {
                        j--;
                    }

                    if (j >= 0)
                    {
                        topics[j].Children.Insert(0, topics[i]);
                        topics.RemoveAt(i);
                        levels.RemoveAt(i);
                    }
                }
            }

            return topics.ToArray();
        }

        Topic DeserializeSingleTopic(string line, out int level)
        {
            level = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '\t')
                    level++;
                else
                    break;
            }

            string text = line.Trim();
            if (string.IsNullOrEmpty(text))
                return null;
            else
                return new Topic(text);
        }

        void SerializeTopic(StringBuilder sb, Topic topic, int level, bool recursive, ChartObject[] allObjects, List<ChartObject> accessedObjects)
        {
            sb.Append('\t', level);
            sb.AppendLine(topic.Text);

            foreach (var subTopic in topic.Children)
            {
                if (accessedObjects.Contains(subTopic))
                    continue;
                accessedObjects.Add(subTopic);

                if (recursive || allObjects.Contains(subTopic))
                {
                    SerializeTopic(sb, subTopic, level + 1, recursive, allObjects, accessedObjects);
                }
            }
        }
    }
}
