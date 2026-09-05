

namespace Assets.KnifeGame.Scripts.Core.Target
{
    public class TargetPresenter : ITarget
    {
        private TargetModel _model;
        private TargetView _view;


        public TargetPresenter(TargetModel model, TargetView view)
        {
            _model = model;
            _view = view;

        }

        public void Initialize()
        {
            _view.Initialize(_model.Data);
        }

        public void OnHit(HitInfo hitInfo)
        {
            DevLog.Log($"Target Hit! at {hitInfo.HitTransform.name}");
        }

        public void Tick()
        {
            float direction = _model.IsRotatingClockwise ? -1f : 1f;

            _view.Rotate(_model.RotationSpeed * direction);
        }
    }
}