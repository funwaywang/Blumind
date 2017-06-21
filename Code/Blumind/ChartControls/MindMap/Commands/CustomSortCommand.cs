using System;
using System.Collections.Generic;
using System.Text;
using Blumind.Core;
using Blumind.Model;
using Blumind.Model.MindMaps;

namespace Blumind.Controls.MapViews
{
    class CustomSortCommand : Command
    {
        private Topic Parent;
        private int[] NewIndices;

        public CustomSortCommand(Topic[] topics, int step)
        {
            if (topics == null || topics.Length == 0 || step == 0)
                return;

            Parent = topics[0].ParentTopic;
            if (Parent == null)
                return;

            // get topics them have a same parent
            List<int> moveIndices = new List<int>();
            foreach (Topic topic in topics)
            {
                if (topic.ParentTopic == Parent)
                    moveIndices.Add(topic.Index);
            }

            NewIndices = GetNewIndices(Parent.Children.Count, moveIndices.ToArray(), step);
        }

        public CustomSortCommand(Topic parent, int[] newIndices)
        {
            Parent = parent;
            NewIndices = newIndices;
        }

        public override string Name
        {
            get { return "Custom Sort"; }
        }

        public override bool Rollback()
        {
            if (Parent == null || NewIndices == null || NewIndices.Length > Parent.Children.Count)
                return false;

            // 重构旧索引
            int[] oldIndices = new int[NewIndices.Length];
            for (int i = 0; i < NewIndices.Length; i++)
            {
                int index = NewIndices[i];
                if (index > -1 && index < oldIndices.Length)
                    oldIndices[index] = i;
                else
                    return false;
            }

            Parent.SortChildren(oldIndices);

            return true;
        }

        public override bool Execute()
        {
            if (Parent == null || NewIndices == null || NewIndices.Length > Parent.Children.Count)
                return false;

            Parent.SortChildren(NewIndices);

            return true;
        }

        public static int[] GetNewIndices(int count, int[] moveIndices, int step)
        {
            // build old indices
            int[] oldIndices = new int[count];
            for (int i = 0; i < oldIndices.Length; i++)
                oldIndices[i] = i;

            // sort move indices
            Array.Sort(moveIndices);

            // 
            if (step > 0)
                step = Math.Min(step, count - moveIndices[moveIndices.Length - 1] - 1);
            else
                step = Math.Max(step, 0 - moveIndices[0]);
            if (step == 0)
                return oldIndices;

            //
            if (step > 0)
            {
                for (int i1 = moveIndices.Length - 1; i1 >= 0; i1--)
                {
                    int index = moveIndices[i1];
                    int min = Math.Min(index + step, index);
                    int max = Math.Max(index + step, index);

                    for (int i2 = 0; i2 < oldIndices.Length; i2++)
                    {
                        int oldIndex = oldIndices[i2];
                        if (oldIndex < min || oldIndex > max || oldIndex == index)
                            continue;
                        oldIndices[i2] = oldIndex - 1;
                    }
                    oldIndices[index] += step;
                }
            }
            else if (step < 0)
            {
                for (int i1 = 0; i1 < moveIndices.Length; i1++)
                {
                    int index = moveIndices[i1];
                    int min = Math.Min(index + step, index);
                    int max = Math.Max(index + step, index);

                    for (int i2 = 0; i2 < oldIndices.Length; i2++)
                    {
                        int oldIndex = oldIndices[i2];
                        if (oldIndex < min || oldIndex > max || oldIndex == index)
                            continue;
                        oldIndices[i2] = oldIndex + 1;
                    }
                    oldIndices[index] += step;
                }
            }

            return oldIndices;
        }

#if DEBUG
        public static void UnitTest()
        {
            int[] mi = new int[] { 4, 6 };
            int[] list = new int[10];
            for (int i = 0; i < list.Length; i++)
                list[i] = i;

            int[] nis1 = GetNewIndices(10, mi, 3);
            int[] nis2 = GetNewIndices(10, mi, -3);
            int[] nis3 = GetNewIndices(10, mi, 20);
            int[] nis4 = GetNewIndices(10, mi, -20);

            ShowIntArray(list);
            Console.WriteLine("-------------------------------------------------------------------");
            ShowIntArray(nis1);
            ShowIntArray(nis2);
            ShowIntArray(nis3);
            ShowIntArray(nis4);
            Console.ReadKey();
        }

        private static void ShowIntArray(int[] nis)
        {
            StringBuilder sb = new StringBuilder();
            foreach (int i in nis)
                sb.AppendFormat("{0}", i.ToString().PadLeft(4));
            Console.WriteLine(sb);
        }
#endif
    }
}
