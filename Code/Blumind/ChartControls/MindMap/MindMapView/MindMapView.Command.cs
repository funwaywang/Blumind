using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Linq;
using Blumind.Configuration;
using Blumind.Core;
using Blumind.Model;
using Blumind.Model.MindMaps;
using Blumind.Model.Styles;
using Blumind.Model.Widgets;
using System.Collections.Generic;

namespace Blumind.Controls.MapViews
{
    partial class MindMapView 
    {
        #region Common Commands

        [Browsable(false)]
        public override bool CanCopy
        {
            get
            {
                if (SelectedTopics != null && SelectedTopics.Length > 0)
                    return true;

                if (SelectedObject is Widget)// && ((Widget)SelectedObject).CanCopy)
                    return true;

                return false;
            }
        }

        [Browsable(false)]
        public override bool CanCut
        {
            get
            {
                if (ReadOnly)
                    return false;

                if (!SelectedTopics.IsNullOrEmpty() && !SelectedTopics.Exists(t=>t.IsRoot))
                    return true;

                if (SelectedObject is Widget)
                    return true;

                return false;
            }
        }

        [Browsable(false)]
        public override bool CanPaste
        {
            get
            {
                return !ReadOnly && Selection.Count == 1 && SelectedTopic != null;
            }
        }

        [Browsable(false)]
        public override bool CanPasteAsRemark
        {
            get
            {
                return !ReadOnly && SelectedObject is IRemark;
            }
        }

        [Browsable(false)]
        public override bool CanDelete
        {
            get
            {
                return !ReadOnly && Selection.Count > 0 && (SelectedTopic == null || !SelectedTopic.IsRoot);
            }
        }

        [Browsable(false)]
        public override bool CanEdit
        {
            get
            {
                return !ReadOnly && SelectedTopic != null;
            }
        }

        public override void DeleteObject()
        {
            if (Selection.Count > 0)
            {
                Delete(Selection.ToArray());
            }
        }

        private void Delete(ChartObject[] mapObjects)
        {
            if (mapObjects != null && mapObjects.Length > 0)
            {
                foreach (ChartObject mapObject in mapObjects)
                {
                    if (mapObject is Topic && ((Topic)mapObject).IsRoot)
                    {
                        return;
                    }
                }

                DeleteCommand command = new DeleteCommand(mapObjects);
                ExecuteCommand(command);
            }
        }

        public override void Copy()
        {
            if (SelectedTopics != null && SelectedTopics.Length > 0)
            {
                var topics = SelectedTopics.OrderBy(t => t.Level).ToArray();
                Copy(topics, true);
            }
            else if (SelectedObject is Widget)// && ((Widget)SelectedObject).CanCopy)
            {
                var widgets = SelectedObjects.Where(o => o is Widget).ToArray();
                Copy(widgets, false);
            }
        }

        void Copy(ChartObject[] objects, bool recursive)
        {
            if (objects != null && objects.Length > 0)
            {
                var command = new CopyCommand(objects, recursive);
                ExecuteCommand(command);
            }
        }

        public override void Cut()
        {
            if (SelectedTopics != null && SelectedTopics.Length > 0)
            {
                var topics = SelectedTopics.OrderBy(t => t.Level).ToArray();
                var command = new CutCommand(topics);
                ExecuteCommand(command);
            }
            else if (SelectedObject is Widget)
            {
                var widgets = SelectedObjects.Where(o => o is Widget).ToArray();
                var command = new CutCommand(widgets);
                ExecuteCommand(command);
            }
        }

        public override void Paste()
        {
            if (SelectedTopic != null)
            {
                var command = new PasteCommand(Map.Document, SelectedTopic);
                ExecuteCommand(command);
            }
        }

        void DargDropTo(IEnumerable<ChartObject> chartObjects, Topic target, DragTopicsMethod dragMethod)
        {
            if (chartObjects.IsNullOrEmpty() || target == null || dragMethod == DragTopicsMethod.None)
                return;

            var comd = new DragDropCommand(Map.Document, chartObjects, target, dragMethod);
            this.ExecuteCommand(comd);
        }

        public void PasteAsRemark()
        {
            if (SelectedObject is IRemark && Clipboard.ContainsText())
            {
                if (!string.IsNullOrEmpty(SelectedObject.Remark))
                {
                    if (this.ShowMessage("Whether to replace the current remark?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                        return;
                }

                SelectedObject.Remark = ClipboardHelper.GetHtml();
            }
        }

        public void PasteAsImage()
        {
            if (SelectedTopic != null && Clipboard.ContainsImage())
            {
                PictureWidget template = new PictureWidget();
                template.Image = Blumind.Model.Widgets.PictureWidget.PictureDesign.FromClipboard();
                if (template.Image.Data != null)
                {
                    Size size = template.Image.Data.Size;
                    if (size.Width > 128 || size.Height > 128)
                    {
                        size = PaintHelper.SizeInSize(size, new Size(128, 128));
                        template.CustomWidth = size.Width;
                        template.CustomHeight = size.Height;
                    }

                    AddWidget(PictureWidget.TypeID, template, Helper.TestModifierKeys(Keys.Shift));
                }
            }
        }

        public void AddTopic(Topic parentTopic, Topic subTopic, int index)
        {
            var command = new AddTopicCommand(parentTopic, subTopic, index);
            ExecuteCommand(command);
        }

        public void AddTopics(Topic parentTopic, Topic[] subTopics, int index)
        {
            var command = new AddTopicCommand(parentTopic, subTopics, index);
            ExecuteCommand(command);
        }

        public void ChangeObjectText(ITextObject tobj, string newText)
        {
            ChangeTextCommand command = new ChangeTextCommand(tobj, newText);
            ExecuteCommand(command);
        }

        public override void CopyStyle(bool holdOn)
        {
            if (FormatPainter.Default.IsEmpty)
            {
                if (SelectedTopic != null)
                {
                    FormatPainter.Default.Copy(SelectedTopic);
                    FormatPainter.Default.HoldOn = holdOn;
                }
            }
            else
            {
                FormatPainter.Default.Clear();
            }
        }

        public void CustomSort(int step)
        {
            if (SelectedTopics.Length > 0)
            {
                CustomSortCommand comd = new CustomSortCommand(SelectedTopics, step);
                ExecuteCommand(comd);
            }
        }

        public void CustomSort(Topic parent, int[] newIndices)
        {
            if (parent == null || newIndices == null || newIndices.Length == 0)
                return;

            CustomSortCommand comd = new CustomSortCommand(parent, newIndices);
            ExecuteCommand(comd);
        }

        #endregion
    }
}
