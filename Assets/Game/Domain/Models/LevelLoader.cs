using Assets.Game.Infrastructure.Signals;
using Zenject;

public sealed class LevelLoader : ILevelLoader
{

    private readonly GameBoard _board;
    private readonly ILevelBuilder _levelBuilder;
    private readonly SignalBus _signalBus;

    public LevelLoader(GameBoard board, ILevelBuilder levelBuilder, SignalBus signalBus)
    {
        _board = board;
        _levelBuilder = levelBuilder;
        _signalBus = signalBus;
    }
    public void LoadLevel(LevelDefinitionSO levelDefinition)
    {
        var levelModel = _levelBuilder.BuildLevel(levelDefinition.LevelDefinition);

        _board.SetLevel(levelModel);

        _signalBus.Fire(new LevelLoadedSignals(levelModel));
    }
}
