using Assets.Game;

public sealed class DestinationIsNotFullRule : IMovable
{
    public bool ValidateMove(MoveRequest moveRequest, LevelModel levelModel, out string errorMessage)
    {
        HookModel destinationHook = levelModel?.GetHook(moveRequest.targetHookIndex);
        errorMessage = null;

        if (destinationHook == null)
        {
            errorMessage = "Destination hook not found.";
            return false;
        }

        if (destinationHook.IsFull)
        {
            errorMessage = "Destination hook is full.";
            return false;
        }

        return true;
    }
}
