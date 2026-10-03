using UnityEngine;
using UnityEngine.EventSystems;

namespace SauceSort.Gameplay
{
    /// <summary>
    /// Validates a drop and snaps the configured object to this destination.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class DropDestination : MonoBehaviour, IDropHandler
    {
        [SerializeField] private DraggableObject _acceptedObject;

        public bool CanAccept(DraggableObject draggableObject)
        {
            return _acceptedObject != null && draggableObject == _acceptedObject;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null)
            {
                return;
            }

            DraggableObject draggableObject = eventData.pointerDrag.GetComponent<DraggableObject>();
            if (!CanAccept(draggableObject))
            {
                if (draggableObject != null)
                {
                    draggableObject.ReturnToStartPosition();
                }

                return;
            }

            draggableObject.SnapTo(transform.position);
        }
    }
}
