public sealed class MoveResult
{
    public bool Success { get; }
    public string Message { get; }

    public MoveResult(bool success, string message = null)
    {
        Success = success;
        Message = message;
    }

    public static MoveResult SuccessResult()
    {
        return new MoveResult(true);
    }

    public static MoveResult FailureResult(string message)
    {
        return new MoveResult(false, message);
    }
}
