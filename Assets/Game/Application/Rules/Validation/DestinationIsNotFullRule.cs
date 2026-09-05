using Assets.Game.Domain.Models;

namespace Assets.Game.Application.Rules.Validation
{
    public sealed class DestinationIsNotFullRule : IMovable
    {
        public MoveValidationResult ValidateMove(MoveRequest moveRequest, LevelModel levelModel)
        {
            HookModel destinationHook = levelModel?.GetHook(moveRequest.TargetHookIndex);
            string errorMessage = null;

            if (destinationHook == null)
            {
                errorMessage = "Destination hook not found.";
                return MoveValidationResult.Invalid(MoveValidationError.HookNotFound);
            }

            if (destinationHook.IsFull)
            {
                errorMessage = "Destination hook is full.";
                return MoveValidationResult.Invalid(MoveValidationError.DestinationHookFull);
            }

            return MoveValidationResult.Valid();
        }
    }
}