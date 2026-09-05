using Assets.Game.Application.Services;
using Game.Application.Rules.Interfaces;
using UnityEngine;
using Zenject;

public class BoardController : MonoBehaviour
{
    private MoveService _moveService;
    private UndoService _undoService;
    private IWinConditionService _winConditionService;


    [Inject]
    public void Construct(MoveService moveService, UndoService undoService, IWinConditionService winConditionService)
    {
        _moveService = moveService;
        _undoService = undoService;
        _winConditionService = winConditionService;
    }
}
