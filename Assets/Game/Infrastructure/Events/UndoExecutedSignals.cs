
namespace Assets.Game.Infrastructure.Signals
{

    public class UndoExecutedSignals
    {
        public MoveRecord MoveRecord { get; }

        public UndoExecutedSignals(MoveRecord moveRecord)
        {
            MoveRecord = moveRecord;
        }
    }
}