public sealed class GameBoard
{
    public LevelModel CurrentLevel { get; private set; }

    public void SetLevel(LevelModel levelModel)
    {
        CurrentLevel = levelModel;
    }
}
