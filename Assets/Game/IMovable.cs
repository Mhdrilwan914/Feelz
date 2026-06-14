public interface IMovable
{
    public bool ValidateMove(MoveRequest moveRequest, LevelModel levelModel, out string errorMessage);
}
