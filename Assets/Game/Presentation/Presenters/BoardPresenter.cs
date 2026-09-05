using Assets.Game.Infrastructure.Signals;
using Zenject;

namespace Assets.Game.Presentation.Presenters
{
    public sealed class BoardPresenter
    {
        private readonly GameBoard _boardView;
        private readonly SignalBus _signalBus;
        public BoardPresenter(GameBoard boardView, SignalBus signalBus)
        {
            _boardView = boardView;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<LevelLoadedSignals>(OnLevelLoaded);
            _signalBus.Subscribe<MoveExecutedSignals>(OnMoveExecuted);
        }


        public void Dispose()
        {
            _signalBus.Unsubscribe<LevelLoadedSignals>(OnLevelLoaded);
            _signalBus.Unsubscribe<MoveExecutedSignals>(OnMoveExecuted);
        }
        private void OnMoveExecuted(MoveExecutedSignals moveExecutedSignals)
        {
            // render baord state based on the moveExecutedSignals.MoveRecord
        }

        private void OnLevelLoaded(LevelLoadedSignals levelLoadedSignals)
        {
            // Animate key
        }
    }
}