using Assets.Game.Domain.Models;

public class BoardModel
{
    public int Id { get; set; }

    public HookModel HookModel { get; set; }

    public BoardModel(int id, HookModel hookModel)
    {
        Id = id;
        HookModel = hookModel;
    }
}
