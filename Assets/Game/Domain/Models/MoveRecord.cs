using Assets.Game;

public readonly struct MoveRecord
{
    public int SourceHookIndex { get; }
    public int DestinationHookIndex { get; }
    public KeyModel MovedKey { get; }

    public MoveRecord(int sourceHookIndex, int destinationHookIndex, KeyModel movedKey)
    {
        SourceHookIndex = sourceHookIndex;
        DestinationHookIndex = destinationHookIndex;
        MovedKey = movedKey;
    }
}
