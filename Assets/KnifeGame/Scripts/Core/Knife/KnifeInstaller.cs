using KnifeGame.Services.Input;
using UnityEngine;

namespace Assets.KnifeGame.Scripts.Core.Knife
{
    public class KnifeInstaller : MonoBehaviour
    {
        [SerializeField] private KnifeData _knifeData;
        [SerializeField] private KnifeView _knifeView;

        private InputReader _inputReader;
        private KnifePresenter _knifePresenter;

        private void Awake()
        {
            PlayerInputAction inputActions = new();

            _inputReader = new InputReader(inputActions);

            KnifeModel knifeModel = new(_knifeData);

            _knifePresenter = new KnifePresenter(knifeModel, _knifeView);

            _knifePresenter.Initialize();
        }


        private bool IsNull => _inputReader == null || _knifePresenter == null;
        private void OnEnable()
        {
            if (!IsNull)
            {
                _inputReader.Enable();
                _inputReader.ThrowRequested += _knifePresenter.Throw;
            }
        }

        private void OnDisable()
        {
            if (!IsNull)
            {
                _inputReader.ThrowRequested -= _knifePresenter.Throw;
                _inputReader.Disable();
            }
        }
    }
}