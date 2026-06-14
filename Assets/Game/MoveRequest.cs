public struct MoveRequest
{
    public int sourceHookIndex { get; }
    public int targetHookIndex { get; }

    public MoveRequest(int sourceHookIndex, int targetHookIndex)
    {
        this.sourceHookIndex = sourceHookIndex;
        this.targetHookIndex = targetHookIndex;
    }
}
