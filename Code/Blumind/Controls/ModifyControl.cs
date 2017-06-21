using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Blumind.Core;

namespace Blumind.Controls
{
    class ModifyControl : Control, IModifyObject
    {
        bool _Modified;
        int ModifySuspendCount;
        bool _ReadOnly;

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

        [DefaultValue(false)]
        public virtual bool ReadOnly
        {
            get { return _ReadOnly; }
            set 
            {
                if (_ReadOnly != value)
                {
                    _ReadOnly = value;
                    OnReadOnlyChanged();
                }
            }
        }

        protected bool ModifySuspend { get; private set; }

        protected virtual void OnReadOnlyChanged()
        {
        }

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
