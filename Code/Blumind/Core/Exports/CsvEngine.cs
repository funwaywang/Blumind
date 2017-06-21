using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Blumind.Controls.MapViews;
using Blumind.Model.Documents;
using Blumind.Model.MindMaps;

namespace Blumind.Core.Exports
{
    class CsvEngine : ChartsExportEngine
    {
        public override string TypeMime
        {
            get { return "text/csv"; }
        }

        /*public override bool Export(MindMapView view, string filename)
        {
            string str = string.Empty;

            var topics = view.Map.GetLeafTopics();
            using (var sw = new StreamWriter(filename, false, Encoding.Default))
            {
                foreach (var t in topics)
                {
                    var path = t.GetPathArray();
                    bool first = true;
                    foreach (var pt in path)
                    {
                        if (!first)
                            sw.Write(",");
                        sw.Write(FormatCsvField(pt.Text));
                        first = false;
                    }
                    sw.WriteLine();
                }

                sw.Write(str);
                sw.Close();
            }

            return true;
        }*/

        protected override bool ExportChartToFile(Document document, ChartPage chart, string filename)
        {
            string str = string.Empty;

            var mindMap = chart as MindMap;
            if (mindMap == null)
                return false;

            var topics = mindMap.GetLeafTopics();
            using (var sw = new StreamWriter(filename, false, Encoding.Default))
            {
                foreach (var t in topics)
                {
                    var path = t.GetPathArray();
                    bool first = true;
                    foreach (var pt in path)
                    {
                        if (!first)
                            sw.Write(",");
                        sw.Write(FormatCsvField(pt.Text));
                        first = false;
                    }
                    sw.WriteLine();
                }

                sw.Write(str);
                sw.Close();
            }

            return true;

            //return base.ExportChartToFile(document, chart, filename);
        }

        string FormatCsvField(string value)
        {
            if(string.IsNullOrEmpty(value))
                return "\"\"";

            if (value.Contains("\""))
                value = value.Replace("\"", "\"\"");
            if (value.Contains(",") || value.Contains("\n") || value.Contains("\r"))
                value = "\"" + value + "\"";
            return value;
        }
    }
}
