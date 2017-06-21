using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;
using System.Windows.Forms;
using Blumind.Globalization;

namespace Blumind.Model
{
    /// <summary>
    /// for design time
    /// </summary>
    class CodeObjectSite : ISite, IMenuCommandService
    {
        DesignerVerbCollection _Verbs;
        ChartObject ChartObject;

        public CodeObjectSite(ChartObject chartObject)
        {
            ChartObject = chartObject;
        }

        public IComponent Component
        {
            get { return null;/* ChartObject;*/ }
        }

        public IContainer Container
        {
            get { return null; }
        }

        public bool DesignMode
        {
            get { return true; }
        }

        public string Name { get; set; }

        public DesignerVerbCollection Verbs
        {
            get
            {
                if (_Verbs == null)
                {
                    _Verbs = new DesignerVerbCollection();
                    _Verbs.Add(new DesignerVerb(Lang._("Remark"), OnRemarkClick));
                }

                return _Verbs;
            }
            set { _Verbs = value; }
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IMenuCommandService))
                return this;
            return null;
        }

        public void AddCommand(MenuCommand command)
        {
        }

        public void AddVerb(DesignerVerb verb)
        {
        }

        public MenuCommand FindCommand(CommandID commandID)
        {
            return null;
        }

        public bool GlobalInvoke(CommandID commandID)
        {
            return true;
        }

        public void RemoveCommand(MenuCommand command)
        {
        }

        public void RemoveVerb(DesignerVerb verb)
        {
        }

        public void ShowContextMenu(CommandID menuID, int x, int y)
        {
        }

        public void OnRemarkClick(object sender, EventArgs e)
        {
            MessageBox.Show("Hello");
        }
    }
}
