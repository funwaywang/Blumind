using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Blumind.Core;
using Blumind.Dialogs;
using Blumind.Model;
using Blumind.Model.MindMaps;

namespace Blumind.Controls.MapViews
{
    partial class MindMapView
    {
        [Browsable(false)]
        public Topic[] SelectedTopics { get; private set; }

        [Browsable(false), DefaultValue(null)]
        public Topic SelectedTopic
        {
            get 
            {
                if (SelectedTopics != null && SelectedTopics.Length > 0)
                    return SelectedTopics[0];
                else
                    return null;
            }
        }

        protected override void OnSelectionChanged()
        {
            SelectedTopics = Selection.GetSelectedObjects<Topic>();

            //if (Created)
            //{
            //    bool sdlg = false;
            //    if (!Selection.IsEmpty() && RemarkDialog.Global.Visible)
            //    {
            //        var remarkObjects = Selection.Where(s => s is IRemark).ToArray();
            //        if (remarkObjects.Length == 1)
            //        {
            //            RemarkDialog.Global.ShowDialog(remarkObjects[0], this.ReadOnly);
            //            sdlg = true;
            //        }
            //    }

            //    if (!sdlg && RemarkDialog.Global.Visible)
            //        RemarkDialog.Global.Close();
            //}

            base.OnSelectionChanged();
        }

        void SelectNextTopic(MoveVector vector)
        {
            Topic topic = SelectedTopic;
            if (topic == null)
                return;

            if (topic != null && ChartLayouter != null)
            {
                Topic next = ChartLayouter.GetNextNode(topic, vector);

                if (next != null)
                {
                    //SelectTopic(next, true);
                    Select(next, true);
                }
            }
        }

        void TrySelectFolded(Topic topic)
        {
            if (topic.Folded && topic.IsDescent(SelectedTopic))
            {
                Select(topic);
            }
        }
    }
}
