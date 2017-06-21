using System.Drawing;
using Blumind.Controls.MapViews;
using Blumind.Model.Documents;
using Blumind.Model.MindMaps;

namespace Blumind.Core.Exports
{
    class FreeMindEngine : ChartsExportEngine
    {
        public override string TypeMime
        {
            get { return DocumentType.FreeMind.TypeMime; }
        }

        /*public override bool Export(MindMapView view, string filename)
        {
            if (view.Map != null)
            {
                FreeMindFile.SaveFile(view.Map, filename);
                return true;
            }

            return false;
        }*/

        protected override bool ExportChartToFile(Document document, ChartPage chart, string filename)
        {
            if (chart is MindMap)
            {
                FreeMindFile.SaveFile((MindMap)chart, filename);
                return true;
            }

            return false;
            //return base.ExportChartToFile(document, chart, filename);
        }
    }
}
