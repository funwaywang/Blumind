using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Blumind.Core
{
    interface IModifyObject
    {
        event EventHandler ModifiedChanged;

        bool Modified { get; set; }
    }

    class ModifyObject : IModifyObject
    {
        bool _Modified;
        int ModifySuspendCount;

        public event EventHandler ModifiedChanged;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Modified
        {
            get
            {
                return _Modified;
            }
            set
            {
                if (_Modified != value)
                {
                    _Modified = value;
                    OnModifiedChanged();
                }
            }
        }

        protected bool ModifySuspend { get; private set; }

        protected virtual void OnModifiedChanged()
        {
            if (ModifySuspend)
            {
                ModifySuspendCount++;
            }
            else
            {
                if (ModifiedChanged != null)
                    ModifiedChanged(this, EventArgs.Empty);
            }
        }

        public void SuspendMofity()
        {
            if (!ModifySuspend)
            {
                ModifySuspend = true;
                ModifySuspendCount = 0;
            }
        }

        public void ResumeModify()
        {
            ModifySuspend = false;

            if (ModifySuspendCount > 0)
            {
                ModifySuspendCount = 0;
                OnModifiedChanged();
            }
        }
    }
}
