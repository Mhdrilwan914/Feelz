using Assets.Game.Application.Rules.Validation;
using Assets.Game.Application.Services;
using Assets.Game.Infrastructure.Signals;
using Assets.Game.Presentation.Presenters;
using Assets.Game.Presentation.Views;
using Game.Application.Rules.Interfaces;
using UnityEngine;
using Zenject;


namespace Assets.Game.Installer
{
    public class GamePlayInstaller : MonoInstaller
    {
        [SerializeField]
        private BoardView _boardView;

        public override void InstallBindings()
        {
            //signals events

            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<MoveExecutedSignals>();
            Container.DeclareSignal<UndoExecutedSignals>();
            Container.DeclareSignal<LevelLoadedSignals>();


            Container.Bind<BoardView>().FromInstance(_boardView).AsSingle();

            // This Automatically binds the BoardPresenter to its interfaces and itself, 
            //allowing it to be injected wherever needed calls Initialize and Dispose methods 
            //to be called automatically when the object is created and destroyed.
            Container.BindInterfacesAndSelfTo<BoardPresenter>().AsSingle();

            // Move history
            Container.Bind<IMoveHistory>().To<MoveHistory>().AsSingle();

            // Services
            Container.Bind<ValidateService>().AsSingle();

            //Move service
            Container.Bind<MoveService>().AsSingle();

            //Undo service
            Container.Bind<UndoService>().AsSingle();


            //Win condition service
            Container.Bind<IWinConditionService>().To<WinConditionService>().AsSingle();


            // Validation rules
            Container?.Bind<IMovable>().To<SourceNotEmptyRule>().AsSingle();
            Container?.Bind<IMovable>().To<DestinationIsNotFullRule>().AsSingle();
            Container?.Bind<IMovable>().To<MatchingRule>().AsSingle();

            Container?.Bind<GameBoard>().AsSingle();

            Container?.Bind<ILevelBuilder>().To<LevelBuilder>().AsSingle();
            Container.Bind<ILevelLoader>().To<LevelLoader>().AsSingle();
        }
    }
}