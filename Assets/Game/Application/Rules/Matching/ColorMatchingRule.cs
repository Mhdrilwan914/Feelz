using Assets.Game;
using Assets.Game.Domain.Interfaces;

public class ColorMatchingRule : IKeyMatchingRule
{
    public bool IsMatch(KeyModel sourceKey, KeyModel targetKey)
    {
        return sourceKey.Color == targetKey.Color;
    }


}
