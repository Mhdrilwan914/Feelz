using Assets.Game;

public class MatchingRule : IMovable
{
    public bool ValidateMove(MoveRequest moveRequest, LevelModel levelModel, out string errorMessage)
    {
        HookModel sourceHook = levelModel?.GetHook(moveRequest.sourceHookIndex);
        HookModel targetHook = levelModel?.GetHook(moveRequest.targetHookIndex);
        errorMessage = null;

        if (sourceHook == null || targetHook == null)
        {
            errorMessage = "One or both hooks not found.";
            return false;
        }

        if (sourceHook.IsEmpty)
        {
            errorMessage = "Source hook is empty.";
            return false;
        }

        KeyModel sourceKey = sourceHook.TopKey;
        KeyModel destinationKey = targetHook.TopKey;

        if (destinationKey != null && sourceKey.Color != destinationKey.Color)
        {
            errorMessage = "Keys do not match.";
            return false;
        }

        return true;
    }
}
