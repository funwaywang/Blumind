using Blumind.Model;
using Blumind.Model.Documents;

namespace Blumind.Controls.MapViews
{
    abstract class Command
    {
        public abstract string Name { get; }

        public virtual bool NoteHistory
        {
            get { return true; }
        }

        public ChartObject[] AfterSelection { get; set; }

        public abstract bool Rollback();

        public abstract bool Execute();

        public override string ToString()
        {
            return Name;
        }
    }
}
