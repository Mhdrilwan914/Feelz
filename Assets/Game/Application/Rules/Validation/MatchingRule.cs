using Assets.Game;
using Assets.Game.Domain.Interfaces;
using Assets.Game.Domain.Models;

public class MatchingRule : IMovable
{
    private readonly IKeyMatchingRule _matchingRule;
    public MatchingRule(IKeyMatchingRule matchingRule)
    {
        _matchingRule = matchingRule;
    }
    public MoveValidationResult ValidateMove(MoveRequest moveRequest, LevelModel levelModel)
    {
        HookModel sourceHook = levelModel?.GetHook(moveRequest.SourceHookIndex);
        HookModel targetHook = levelModel?.GetHook(moveRequest.TargetHookIndex);
        string errorMessage = null;

        if (sourceHook == null || targetHook == null)
        {
            errorMessage = "One or both hooks not found.";
            return MoveValidationResult.Invalid(MoveValidationError.HookNotFound);
        }

        if (sourceHook.IsEmpty)
        {
            errorMessage = "Source hook is empty.";
            return MoveValidationResult.Invalid(MoveValidationError.SourceHookEmpty);
        }

        KeyModel sourceKey = sourceHook.TopKey;
        KeyModel destinationKey = targetHook.TopKey;

        if (destinationKey != null && !_matchingRule.IsMatch(sourceKey, destinationKey))
        {
            errorMessage = "Keys do not match.";
            return MoveValidationResult.Invalid(MoveValidationError.KeysDoNotMatch);
        }

        return MoveValidationResult.Valid();
    }
}
