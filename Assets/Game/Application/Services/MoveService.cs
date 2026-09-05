using Assets.Game.Infrastructure.Signals;
using Zenject;



namespace Assets.Game.Application.Services
{
    public sealed class MoveService
    {
        private readonly ValidateService _validateService;

        private readonly IMoveHistory _moveHistory;

        private SignalBus _signalBus;

        public MoveService(ValidateService validateService, IMoveHistory moveHistory, SignalBus signalBus)
        {
            _validateService = validateService;
            _moveHistory = moveHistory;
            _signalBus = signalBus;
        }

        public MoveResult ExecuteMove(MoveRequest moveRequest, LevelModel levelModel)
        {
            MoveValidationResult validationResponse = _validateService.ValidateMove(moveRequest, levelModel);
            if (!validationResponse.IsValid)
            {
                return MoveResult.FailureResult("Destination hook not found.");
            }
            var sourceHook = levelModel?.GetHook(moveRequest.SourceHookIndex);
            var targetHook = levelModel?.GetHook(moveRequest.TargetHookIndex);
            if (!sourceHook.TryPop(out KeyModel key))
            {
                return MoveResult.FailureResult("Source hook is empty.");
            }
            if (!targetHook.TryPush(key))
            {
                sourceHook.TryPush(key);
                return MoveResult.FailureResult("Target hook is full.");
            }

            // create a move record to store the move details
            var moveRecord = new MoveRecord(moveRequest.SourceHookIndex, moveRequest.TargetHookIndex, key);

            // record the move in the move history
            _moveHistory.RecordMove(moveRecord);

            // fire a signal to notify other parts of the system that a move has been executed
            _signalBus.Fire(new MoveExecutedSignals(moveRecord));

            return MoveResult.SuccessResult();
        }

    }
}