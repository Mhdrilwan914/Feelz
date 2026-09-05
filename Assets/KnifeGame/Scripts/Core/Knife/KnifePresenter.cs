namespace Assets.KnifeGame.Scripts.Core.Knife
{
    public class KnifePresenter//Presenter Connects model and view 
    {
        private readonly KnifeModel _model;
        private readonly KnifeView _view;

        public KnifePresenter(KnifeModel knifeModel, KnifeView knifeView)
        {
            _model = knifeModel;
            _view = knifeView;
        }

        public void Initialize()
        {
            _view?.Initialize(_model.KnifeData);
            _view.OnHItDetected += HitDetected;
        }

        private void HitDetected(HitInfo hitInfo)
        {
            DevLog.Log($"Knife Hit! at {hitInfo.HitTransform.name}");
            if (!hitInfo.HitTransform.gameObject.TryGetComponent(out HitReciver reciver))
            {
                DevLog.Log("No target found!");
                return;
            }
            reciver.target.OnHit(hitInfo);
            DevLog.Log("Knife Hit!");
            Stick(hitInfo);
        }

        public void Throw()
        {
            if (!_model.CanThrow())
            {
                return;
            }

            _model?.Throw();

            _view?.Throw(_model.KnifeData.ThrowForce);
        }


        public void Stick(HitInfo hitInfo)
        {
            DevLog.Log($"Knife Stick! {_model.State}");

            if (_model.CanThrow())
            {
                return;
            }

            _model?.Stick();
            _view?.Stick(hitInfo);
        }

        public void Dispose()
        {
            _view.OnHItDetected -= HitDetected;
        }
    }
}