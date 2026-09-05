
namespace Assets.Game.Infrastructure.Signals
{
    public class MoveExecutedSignals
    {
        public MoveRecord MoveRecord { get; }

        public MoveExecutedSignals(MoveRecord moveRecord)
        {
            MoveRecord = moveRecord;
        }
    }
}
