using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Blumind.Model;
using Blumind.Model.Documents;
using Blumind.Model.MindMaps;

namespace Blumind.Core.Imports
{
    class CsvEngine : DocumentImportEngine
    {
        public override DocumentType DocumentType
        {
            get { return DocumentType.Csv; }
        }

        public override Document ImportFile(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException();

            List<string> lines = new List<string>();
            using (StreamReader sw = new StreamReader(filename, Encoding.Default))
            {
                string line = sw.ReadLine();
                while (line != null)
                {
                    int qmc = GetQuotationMarks(line);
                    while (qmc % 2 == 1)
                    {
                        string line2 = sw.ReadLine();
                        if (line2 == null)
                            break;
                        qmc += GetQuotationMarks(line2);
                        line = line + "\n" + line2;
                    }

                    lines.Add(line);
                    line = sw.ReadLine();
                }

                sw.Close();
            }

            Regex regex = new Regex("(?:^|,)(\\\"(?:[^\\\"]+|\\\"\\\")*\\\"|[^,]*)", 
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

            Topic root = new Topic();
            foreach (string line in lines)
            {
                MatchCollection matchs = regex.Matches(line);
                List<string> items = new List<string>();
                foreach (Match m in matchs)
                {
                    string str = m.Groups[1].Value;
                    if (str.StartsWith("\"") && str.EndsWith("\""))
                        str = str.Substring(1, str.Length - 2);
                    str = str.Replace("\"\"", string.Empty);
                    items.Add(str);
                }

                CreateTopic(root, items);
            }

            //
            if (root.Children.Count == 1)
            {
                root = root.Children[0];
                root.ParentTopic = null;
            }
            MindMap map = new MindMap(Path.GetFileNameWithoutExtension(filename));
            map.Root = root;
            Document doc = new Document();
            doc.Name = Path.GetFileNameWithoutExtension(filename);
            doc.Charts.Add(map);

            return doc;
        }

        void CreateTopic(Topic root, IEnumerable<string> items)
        {
            Topic parent = root;
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item))
                    continue;

                Topic t = parent.GetChildByText(item);
                if (t == null)
                {
                    t = new Topic(item);
                    parent.Children.Add(t);
                }
                parent = t;
            }
        }

        int GetQuotationMarks(string line)
        {
            int count = 0;
            foreach (char c in line)
            {
                if (c == '"')
                    count++;
            }

            return count;
        }
    }
}
