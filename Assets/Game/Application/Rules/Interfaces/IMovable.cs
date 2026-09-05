public interface IMovable
{
    public MoveValidationResult ValidateMove(MoveRequest moveRequest, LevelModel levelModel);
}
