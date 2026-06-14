using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch.Touch;


public class InputSystem : MonoBehaviour
{
    public event Action _onTouchInvoke;


    private void Awake()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnEnable()
    {
        ETouch.onFingerDown += HandleFingerDown;
    }

    private void HandleFingerDown(Finger finger)
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(finger.screenPosition);

        RaycastHit2D hit = Physics2D.CircleCast(worldPoint, 0.1f, Vector2.zero);
        if (hit.collider == null) return;

        if (hit.collider.TryGetComponent(out ClayPlateMovement clayPlateMovement))
        {
            clayPlateMovement.gameObject.SetActive(false);
            DevLog.Log($"On Touch Game Object : {hit.collider.gameObject.name}");
        }

    }

    private void OnDisable()
    {
        ETouch.onFingerDown -= HandleFingerDown;
    }


}
