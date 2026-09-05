using System;
using UnityEngine.InputSystem;
namespace KnifeGame.Services.Input
{
    public class InputReader
    {
        private PlayerInputAction _inputActions;

        public event Action ThrowRequested;


        public InputReader(PlayerInputAction inputActions)
        {
            _inputActions = inputActions;
        }
        public void Enable()
        {
            _inputActions.Enable();
            _inputActions.GamePlay.Throw.performed += OnThrowPerformed;
        }

        private void OnThrowPerformed(InputAction.CallbackContext context)
        {
            DevLog.Log("Throw requested!");
            ThrowRequested?.Invoke();
        }


        public void Disable()
        {
            _inputActions.GamePlay.Throw.performed -= OnThrowPerformed;
            _inputActions.Disable();
        }
    }
}

