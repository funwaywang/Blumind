using System;
using System.Windows.Forms;
using Blumind.Core;
using Blumind.Core.Documents;
using Blumind.Model;
using Blumind.Model.MindMaps;

namespace Blumind.Controls.MapViews
{
    class CopyCommand : Command
    {
        private ChartObject[] MapObjects;
        private bool Recursive;

        public CopyCommand(ChartObject[] objects, bool recursive)
        {
            MapObjects = objects;
            Recursive = recursive;

            if (MapObjects == null || MapObjects.Length == 0)
                throw new ArgumentNullException();
        }

        public override string Name
        {
            get { return "Copy"; }
        }

        public override bool NoteHistory
        {
            get
            {
                return false;
            }
        }

        public override bool Rollback()
        {
            return true;
        }

        public override bool Execute()
        {
            if (MapObjects != null)
            {
                CopyObjects(MapObjects);
            }

            return true;
        }

        void CopyObjects(ChartObject[] mapObjects)
        {
            var obj = new DataObject();

            TextSerializer ts = new TextSerializer();
            string text = ts.SerializeObjects(mapObjects, Recursive);
            if(!string.IsNullOrEmpty(text))
                obj.SetText(text);

            if(mapObjects.Length == 1 && mapObjects[0] != null)
                mapObjects[0].CopyExtendContent(obj);

            obj.SetData(typeof(MapClipboardData), new MapClipboardData(mapObjects));

            Clipboard.SetDataObject(obj);
        }
    }
}
