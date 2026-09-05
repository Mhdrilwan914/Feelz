
namespace Game.Application.Rules.Interfaces
{

    public interface IWinConditionService
    {
        public bool IsLevelCompleted(LevelModel levelModel);
    }
}