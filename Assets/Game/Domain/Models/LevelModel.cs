using System.Collections.Generic;
using Assets.Game.Domain.Models;

public class LevelModel
{
    public IReadOnlyList<HookModel> Hooks { get; }

    private readonly List<HookModel> _hooks;

    public LevelModel(List<HookModel> hooks)
    {
        _hooks = hooks;

    }

    public HookModel GetHook(int index)
    {
        if (index < 0 || index >= _hooks.Count)
        {
            DevLog.Log("Index is out of range."); return null;
        }
        return _hooks[index];
    }
}
