using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Blumind.Controls.MapViews;
using Blumind.Core;
using Blumind.Model;
using Blumind.Model.Documents;
using Blumind.Model.MindMaps;
using Blumind.Model.Styles;

namespace Blumind.Controls
{
    abstract class ChartControl : Chart, ICommandHost, IChartControl
    {
        public event EventHandler ModifiedChanged;
        public event ChartObjectEventHandler ChartObjectAdded;

        public ChartControl()
        {
            Selection = new ChartSelection();
            Selection.BeforeClear += new EventHandler(Selection_BeforeClear);
            Selection.AfterClear += new EventHandler(Selection_AfterClear);

            MultiSelect = true;
        }

        public abstract ChartPage ChartPage { get; }

        [Browsable(false), DefaultValue(false)]
        public bool Modified
        {
            get { return ChartPage != null && ChartPage.Modified; }
            set
            {
                if (ChartPage != null)
                {
                    ChartPage.Modified = value;
                }
            }
        }

        protected void OnModifiedChanged()
        {
            if (ModifiedChanged != null)
            {
                ModifiedChanged(this, EventArgs.Empty);
            }
        }

        protected void OnChartObjectAdded(ChartObject chartObject)
        {
            if (ChartObjectAdded != null)
                ChartObjectAdded(this, new ChartObjectEventArgs(chartObject));
        }

        #region ICommandHost Members
        Stack<Command> CommandHistory = new Stack<Command>();
        Stack<Command> UndoCommandHistory = new Stack<Command>();

        public event EventHandler CommandHistoryChanged;

        public void Undo()
        {
            Undo(1);
        }

        public void Redo()
        {
            Redo(1);
        }

        protected virtual void OnCommandHistoryChanged()
        {
            if (CommandHistory.Count > 0)
                ChartPage.Modified = true;

            if (CommandHistoryChanged != null)
            {
                CommandHistoryChanged(this, EventArgs.Empty);
            }
        }

        protected void ClearCommandHistory()
        {
            CommandHistory.Clear();
            UndoCommandHistory.Clear();
            OnCommandHistoryChanged();
        }

        [Browsable(false)]
        public bool CanUndo
        {
            get { return CommandHistory.Count > 0; }
        }

        [Browsable(false)]
        public bool CanRedo
        {
            get { return UndoCommandHistory.Count > 0; }
        }

        public bool ExecuteCommand(Command command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            if (command.Execute())
            {
                if (command.NoteHistory)
                {
                    CommandHistory.Push(command);
                    UndoCommandHistory.Clear();
                    OnCommandHistoryChanged();
                }

                if (command.AfterSelection != null)
                    Select(command.AfterSelection);
            }

            return true;
        }

        public void Undo(int step)
        {
            Command lastSelectionCommand = null;
            bool changed = false;
            for (int i = 0; i < step; i++)
            {
                if (CommandHistory.Count > 0)
                {
                    Command comd = CommandHistory.Pop();
                    comd.Rollback();
                    UndoCommandHistory.Push(comd);
                    changed = true;

                    if (comd.AfterSelection != null)
                        lastSelectionCommand = comd;
                }
                else
                {
                    break;
                }
            }

            if (changed)
            {
                OnCommandHistoryChanged();
            }

            if (lastSelectionCommand != null && lastSelectionCommand.AfterSelection != null)
                Select(lastSelectionCommand.AfterSelection);
        }

        public void Redo(int step)
        {
            Command lastSelectionCommand = null;
            bool changed = false;
            for (int i = 0; i < step; i++)
            {
                if (UndoCommandHistory.Count > 0)
                {
                    Command comd = UndoCommandHistory.Pop();
                    comd.Execute();
                    CommandHistory.Push(comd);
                    changed = true;

                    if (comd.AfterSelection != null)
                        lastSelectionCommand = comd;
                }
                else
                {
                    break;
                }
            }

            if (changed)
            {
                OnCommandHistoryChanged();
            }

            if (lastSelectionCommand != null && lastSelectionCommand.AfterSelection != null)
                Select(lastSelectionCommand.AfterSelection);
        }

        #endregion

        #region IChartControl Members
        public void OnChartObjectPropertyChanged(ChartObject chartObject, Blumind.Core.PropertyChangedEventArgs e)
        {
        }

        #endregion

        #region Selection
        public event System.EventHandler SelectionChanged;

        protected ChartSelection Selection { get; private set; }

        [DefaultValue(true)]
        public bool MultiSelect { get; set; }

        [Browsable(false)]
        public ChartObject[] SelectedObjects { get; private set; }

        [Browsable(false)]
        public ChartObject SelectedObject
        {
            get
            {
                if (Selection.Count > 0)
                    return Selection[0];
                else
                    return null;
            }
        }

        public void Unselect(ChartObject mapObject)
        {
            if (mapObject != null && Selection.Contains(mapObject))
            {
                BeginUpdateView();
                Selection.Remove(mapObject);
                mapObject.Selected = false;
                OnSelectionChanged();
                EndUpdateView(ChangeTypes.Visual);
            }
        }

        public void ClearSelection()
        {
            Selection.Clear();
        }

        public void SelectAll()
        {
            var list = GetAllObjects(true);
            Select(list, true);
        }

        public void Select(ChartObject mapObject)
        {
            Select(mapObject, true);
        }

        public void Select(ChartObject mapObject, bool exclusive)
        {
            Select(new ChartObject[] { mapObject }, exclusive);
        }

        public void Select(ChartObject[] mapObjects)
        {
            Select(mapObjects, true);
        }

        public void Select(ChartObject[] mapObjects, bool exclusive)
        {
            if (mapObjects == null || mapObjects.Length == 0)
            {
                if (exclusive)
                    ClearSelection();
                return;
            }

            BeginUpdateView();

            Selection.Update(mapObjects, exclusive);
            OnSelectionChanged();

            EndUpdateView(ChangeTypes.Visual);
        }

        public bool HasSelected()
        {
            return Selection.Count > 0;
        }

        void Selection_BeforeClear(object sender, EventArgs e)
        {
            foreach (ChartObject obj in Selection)
            {
                if (obj.Selected)
                    obj.Selected = false;

                InvalidateObject(obj);
            }
        }

        void Selection_AfterClear(object sender, EventArgs e)
        {
            OnSelectionChanged();
        }

        protected virtual void OnSelectionChanged()
        {
            if (Selection.Count > 0)
            {
                EnsureVisible(Selection[0]);
            }

            SelectedObjects = Selection.ToArray();
            Selection.PushHistory();

            if (SelectionChanged != null)
            {
                SelectionChanged(this, EventArgs.Empty);
            }
        }

        public virtual void EnsureVisible(ChartObject chartObject)
        {
        }
        #endregion

        #region Zoom
        public void ZoomIn()
        {
            Zoom = (int)(Zoom / 0.25f + 1) * 0.25f;
        }

        public void ZoomOut()
        {
            Zoom = (int)(Zoom / 0.25f - 1) * 0.25f;
        }

        public void ZoomAs(ZoomType zoomType)
        {
            if (OriginalContentSize.IsEmpty)
                return;

            Rectangle rect = ViewPort;
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            switch (zoomType)
            {
                case ZoomType.FitPage:
                    Zoom = Math.Min((float)rect.Width / OriginalContentSize.Width, (float)rect.Height / OriginalContentSize.Height);
                    break;
                case ZoomType.FitWidth:
                    Zoom = (float)rect.Width / OriginalContentSize.Width;
                    break;
                case ZoomType.FitHeight:
                    Zoom = (float)rect.Height / OriginalContentSize.Height;
                    break;
            }
        }
        #endregion

        #region Edit

        [Browsable(false)]
        public abstract bool CanCopy { get; }

        [Browsable(false)]
        public abstract bool CanCut { get; }

        [Browsable(false)]
        public abstract bool CanPaste { get; }

        [Browsable(false)]
        public abstract bool CanPasteAsRemark { get; }

        [Browsable(false)]
        public abstract bool CanDelete { get; }

        [Browsable(false)]
        public abstract bool CanEdit { get; }

        public virtual void DeleteObject()
        {
        }

        public virtual void Paste()
        {
        }

        public virtual void Copy()
        {
        }

        public virtual void Cut()
        {
        }
        #endregion

        #region Editor Integration
        public virtual IEnumerable<ToolStripItem> GetToolStripItems()
        {
            return null;
        }

        #endregion

        public virtual void InvalidateObject(ChartObject obj)
        {
        }

        protected virtual ChartObject[] GetAllObjects(bool onlyVisible)
        {
            return null;
        }

        public virtual void ApplyChartTheme(ChartTheme chartTheme)
        {
        }

        internal void ChangeObjectText(ChartObject topic, string newText)
        {
            throw new NotImplementedException();
        }

        public virtual void ExportImage(string filename, string typeMime)
        {
            throw new NotImplementedException();
        }

        //public virtual System.Drawing.Printing.PrintDocument ReadyPrint()
        //{
        //    return null;
        //}

        public virtual void CopyStyle(bool holdOn)
        {
        }

        public virtual void ResetControlStatus()
        {
        }

        public void NotifyCurrentLanguageChanged()
        {
            OnCurrentLanguageChanged();
        }

        protected virtual void OnCurrentLanguageChanged()
        {
        }

        public virtual ChartObject FindNext(MindMapFinder Finder, string findWhat)
        {
            return null;
        }
    }
}
