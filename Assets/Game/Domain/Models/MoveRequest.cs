public readonly struct MoveRequest
{
    public int SourceHookIndex { get; }
    public int TargetHookIndex { get; }

    public MoveRequest(int sourceHookIndex, int targetHookIndex)
    {
        this.SourceHookIndex = sourceHookIndex;
        this.TargetHookIndex = targetHookIndex;
    }
}
