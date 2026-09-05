using System.Collections.Generic;
using Assets.Game;
using Assets.Game.Domain.Models;

public class LevelBuilder : ILevelBuilder
{

    private int _nextKeyId;
    public LevelModel BuildLevel(LevelDefinition levelDefinition)
    {
        var hooks = new List<HookModel>();

        foreach (var hookDefinition in levelDefinition.Hooks)
        {
            var hook = new HookModel(levelDefinition.HookCapacity);
            foreach (var color in hookDefinition.Keys)
            {
                var key = new KeyModel(_nextKeyId++, color);
                hook.TryPush(key);
            }
            hooks.Add(hook);
        }
        return new LevelModel(hooks);
    }
}
