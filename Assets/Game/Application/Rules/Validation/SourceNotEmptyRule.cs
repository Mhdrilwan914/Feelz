using Assets.Game.Domain.Models;

public sealed class SourceNotEmptyRule : IMovable
{
    public MoveValidationResult ValidateMove(MoveRequest moveRequest, LevelModel levelModel)
    {
        HookModel sourceHook = levelModel?.GetHook(moveRequest.SourceHookIndex);
        string errorMessage = null;

        if (sourceHook == null)
        {
            errorMessage = "Source hook not found.";
            return MoveValidationResult.Invalid(MoveValidationError.SourceHookEmpty);
        }

        if (sourceHook.IsEmpty)
        {
            errorMessage = "Source hook is empty.";
            return MoveValidationResult.Invalid(MoveValidationError.SourceHookEmpty);
        }

        return MoveValidationResult.Valid();
    }
}
