using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blumind.Core;
using Blumind.Core.Documents;
using Blumind.Model;
using Blumind.Model.Documents;
using Blumind.Model.MindMaps;

namespace Blumind.Controls.MapViews
{
    class DragDropCommand : Command
    {
        Document Document;
        ChartObject[] DragObjects;
        Model.MindMaps.Topic Target;
        DragTopicsMethod DragDropMethod;
        Dictionary<ChartObject, object> Parents;
        Dictionary<ChartObject, int> Indices;
        ChartObject[] NewObjects;

        public DragDropCommand(Document document, IEnumerable<ChartObject> objects, Topic target, DragTopicsMethod method)
        {
            Document = document;
            if (objects != null)
                this.DragObjects = objects.ToArray();
            this.Target = target;
            this.DragDropMethod = method;
        }

        public override string Name
        {
            get { return "Drag-Drop"; }
        }

        public override bool Rollback()
        {
            if (DragObjects.IsNullOrEmpty() || Target == null || DragDropMethod == DragTopicsMethod.None)
                return false;

            switch (DragDropMethod)
            {
                case DragTopicsMethod.Copy:
                    if (Target != null && !NewObjects.IsNullOrEmpty())
                    {
                        foreach (var no in NewObjects)
                            Target.TryRemove(no);
                    }
                    NewObjects = null;
                    return true;
                case DragTopicsMethod.Move:
                    return DeleteCommand.UndeleteObjects(DragObjects, Parents, Indices);
                default:
                    return false;
            }
        }

        public override bool Execute()
        {
            if (DragObjects.IsNullOrEmpty() || Target == null || DragDropMethod == DragTopicsMethod.None)
                return false;

            Parents = new Dictionary<ChartObject, object>();
            Indices = new Dictionary<ChartObject, int>();
                        
            switch (DragDropMethod)
            {
                case DragTopicsMethod.Copy:
                    var data = new MapClipboardData(DragObjects);
                    NewObjects = PasteCommand.PasteObjectsTo(data, Document, Target);
                    AfterSelection = NewObjects;
                    return true;
                case DragTopicsMethod.Move:
                    if (DeleteCommand.DeleteObjects(DragObjects, Parents, Indices))
                    {
                        foreach (var co in DragObjects)
                        {
                            co.Parent = Target;
                        }
                        AfterSelection = DragObjects;
                        return true;
                    }

                    return false;
                default:
                    return false;
            }
        }
    }
}
