using System.Linq;
using Assets.Game.Domain.Models;
using Game.Application.Rules.Interfaces;

namespace Assets.Game.Application.Services
{
    public class WinConditionService : IWinConditionService
    {
        public bool IsLevelCompleted(LevelModel levelModel)
        {
            return levelModel.Hooks.All(IsHookCompleted);
        }

        private bool IsHookCompleted(HookModel hookModel)
        {
            if (hookModel.IsEmpty) return false;
            return hookModel.IsComplete();

        }
    }
}