// Didnt Get this why exactly this is used
public struct MoveValidationResult
{
    public bool IsValid { get; }
    public MoveValidationError Error { get; }

    private MoveValidationResult(bool isValid, MoveValidationError error)
    {
        IsValid = isValid;
        Error = error;
    }

    public static MoveValidationResult Valid()
    {
        return new MoveValidationResult(true, MoveValidationError.None);
    }

    public static MoveValidationResult Invalid(MoveValidationError error)
    {
        return new MoveValidationResult(false, error);
    }
}
