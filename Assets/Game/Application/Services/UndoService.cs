public sealed class UndoService : IUndoService
{
    private readonly IMoveHistory moveHistory;

    public UndoService(IMoveHistory history)
    {
        moveHistory = history;
    }


    public UndoMoveResult Undo(LevelModel levelModel)
    {
        if (!moveHistory.TryGetLastMove(out MoveRecord move))
        {
            return UndoMoveResult.FailureResult("No Moves To Undo");
        }

        var source = levelModel.GetHook(move.SourceHookIndex);

        var destination = levelModel.GetHook(move.DestinationHookIndex);

        if (!destination.TryPop(out var key))
        {
            return UndoMoveResult.FailureResult("Failed to undo move.");
        }

        if (!source.TryPush(key))
        {
            destination.TryPush(key);

            return UndoMoveResult.FailureResult("Failed to restore move.");
        }

        return UndoMoveResult.SuccessResult();
    }
}
