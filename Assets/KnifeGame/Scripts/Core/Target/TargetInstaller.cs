using Assets.KnifeGame.Scripts.Core.Knife;
using UnityEngine;

namespace Assets.KnifeGame.Scripts.Core.Target
{
    public class TargetInstaller : MonoBehaviour
    {
        [SerializeField] private TargetData _targetData;
        [SerializeField] private TargetView _targetView;
        [SerializeField] private HitReciver _hitReciver;
        private TargetPresenter _targetPresenter;

        private void Awake()
        {
            TargetModel model = new TargetModel(_targetData);
            _targetPresenter = new TargetPresenter(model, _targetView);
            _targetPresenter.Initialize();
            _hitReciver.target = _targetPresenter;

        }

        private void Update()
        {
            _targetPresenter.Tick();
        }
    }
}