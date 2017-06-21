using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Blumind.Model.Documents;

namespace Blumind.Core
{
    abstract class DocumentImportEngine
    {
        public static IEnumerable<DocumentImportEngine> GetEngines()
        {
            return new DocumentImportEngine[]
            {
                new Blumind.Core.Imports.CsvEngine(),
                new Blumind.Core.Imports.FreeMindEngine()
            };
        }

        public abstract DocumentType DocumentType { get; }

        public abstract Document ImportFile(string filename);
    }
}
