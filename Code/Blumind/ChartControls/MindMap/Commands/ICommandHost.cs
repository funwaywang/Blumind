
namespace Blumind.Controls.MapViews
{
    interface ICommandHost
    {
        bool CanUndo { get; }

        bool CanRedo { get; }

        bool ExecuteCommand(Command command);

        void Undo(int step);

        void Redo(int step);
    }
}
