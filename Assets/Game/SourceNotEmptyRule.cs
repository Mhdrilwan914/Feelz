using Assets.Game;

public sealed class SourceNotEmptyRule : IMovable
{
    public bool ValidateMove(MoveRequest moveRequest, LevelModel levelModel, out string errorMessage)
    {
        HookModel sourceHook = levelModel?.GetHook(moveRequest.sourceHookIndex);
        errorMessage = null;

        if (sourceHook == null)
        {
            errorMessage = "Source hook not found.";
            return false;
        }

        if (sourceHook.IsEmpty)
        {
            errorMessage = "Source hook is empty.";
            return false;
        }

        return true;
    }
}
