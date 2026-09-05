using System.Collections.Generic;
using Assets.Game.Domain.Models;
using Assets.Game.Presentation.Views;

public class BoardContext
{
    public BoardView BoardView { get; }

    public BoardModel BoardModel { get; }

    public Dictionary<HookModel, HookView> HookViews { get; }


}
