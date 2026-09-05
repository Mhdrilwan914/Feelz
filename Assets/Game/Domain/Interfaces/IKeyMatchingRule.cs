namespace Assets.Game.Domain.Interfaces
{
    public interface IKeyMatchingRule
    {
        public bool IsMatch(KeyModel sourceKey, KeyModel targetKey);
    }
}