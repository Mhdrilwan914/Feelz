
namespace Assets.Game.Infrastructure.Signals
{

    public class LevelLoadedSignals
    {
        public LevelModel Level { get; }

        public LevelLoadedSignals(LevelModel level)
        {
            Level = level;
        }
    }
}
