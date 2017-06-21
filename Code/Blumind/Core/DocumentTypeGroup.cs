using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Blumind.Core
{
    class DocumentTypeGroup
    {
        DocumentType[] _Types;
        string _Name;

        public DocumentTypeGroup(string name, DocumentType[] types)
        {
            Name = name;
            Types = types;
        }

        public DocumentType[] Types
        {
            get { return _Types; }
            private set { _Types = value; }
        }

        public string Name
        {
            get { return _Name; }
            private set { _Name = value; }
        }

        /*public static void BuildExportMenus(DocumentTypeGroup[] groups, ToolStripMenuItem menuExport, EventHandler clickEvent)
        {
            menuExport.DropDownItems.Clear();
            if(!groups.IsNullOrEmpty())
            {
                foreach (var group in groups)
                {
                    if (group.Types.IsNullOrEmpty())
                        continue;

                    if (group != groups[0])
                        menuExport.DropDownItems.Add(new ToolStripSeparator());

                    foreach (var dt in group.Types)
                    {
                        var miExport = new ToolStripMenuItem();
                        miExport.Text = string.Format("{0} ...", dt.Name);
                        if(string.IsNullOrEmpty(dt.Description))
                            miExport.ToolTipText = Lang._(dt.Name);
                        else
                            miExport.ToolTipText = Lang._(dt.Description);
                        if (dt.Icon != null)
                            miExport.Image = dt.Icon;
                        miExport.Tag = dt;
                        miExport.Click += clickEvent;
                        menuExport.DropDownItems.Add(miExport);
                    }
                }
            }

            menuExport.Enabled = menuExport.DropDownItems.Count > 0;
        }*/
    }
}
