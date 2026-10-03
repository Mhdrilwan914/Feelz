using UnityEngine;
using UnityEngine.EventSystems;

namespace SauceSort.Gameplay
{
    /// <summary>
    /// Handles pointer/touch dragging for one world-space object.
    /// Destination acceptance is handled by DropDestination.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Vector3 _startPosition;
        private Vector3 _pointerOffset;
        private float _screenDepth;
        private Camera _eventCamera;
        private Collider _collider;
        private bool _dragStarted;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _eventCamera = eventData.pressEventCamera != null
                ? eventData.pressEventCamera
                : Camera.main;

            if (_eventCamera == null)
            {
                return;
            }

            _startPosition = transform.position;
            _screenDepth = _eventCamera.WorldToScreenPoint(_startPosition).z;
            _pointerOffset = _startPosition - PointerToWorldPosition(eventData.position);
            _dragStarted = true;

            // Let the PhysicsRaycaster see the destination behind this object.
            if (_collider != null)
            {
                _collider.enabled = false;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_dragStarted || _eventCamera == null)
            {
                return;
            }

            transform.position = PointerToWorldPosition(eventData.position) + _pointerOffset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_dragStarted)
            {
                return;
            }

            ReturnToStartPosition();
            _dragStarted = false;

            if (_collider != null)
            {
                _collider.enabled = true;
            }
        }

        internal void SnapTo(Vector3 targetPosition)
        {
            transform.position = new Vector3(targetPosition.x, targetPosition.y, _startPosition.z);
        }

        internal void ReturnToStartPosition()
        {
            transform.position = _startPosition;
        }

        private Vector3 PointerToWorldPosition(Vector2 screenPosition)
        {
            return _eventCamera.ScreenToWorldPoint(
                new Vector3(screenPosition.x, screenPosition.y, _screenDepth));
        }
    }
}
