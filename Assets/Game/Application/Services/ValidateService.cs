using System.Collections.Generic;

public class ValidateService
{
    private readonly List<IMovable> _rules;

    public ValidateService(List<IMovable> rules)
    {
        _rules = rules;
    }

    public MoveValidationResult ValidateMove(MoveRequest moveRequest, LevelModel levelModel)
    {
        foreach (var rule in _rules)
        {
            var result = rule.ValidateMove(moveRequest, levelModel);
            if (!result.IsValid)
            {
                return result;
            }
        }
        return MoveValidationResult.Valid();
    }
}
