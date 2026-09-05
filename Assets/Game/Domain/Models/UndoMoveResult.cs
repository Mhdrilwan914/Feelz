public sealed class UndoMoveResult
{
    public bool Success { get; }
    public string Message { get; }

    public UndoMoveResult(bool success, string message = null)
    {
        Success = success;
        Message = message;
    }

    public static UndoMoveResult SuccessResult()
    {
        return new UndoMoveResult(true, string.Empty);
    }

    public static UndoMoveResult FailureResult(string message)
    {
        return new UndoMoveResult(false, message);
    }
}
