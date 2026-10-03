using UnityEngine;

public class SortObject : MonoBehaviour
{

    [SerializeField] private string objectColor;
    private Vector3 dragOffset;

    [SerializeField] private Camera _mainCamera;
    private bool isDragging;
    private float objectDepth;



    private void OnMouseDown()
    {
        isDragging = true;

        objectDepth = Mathf.Abs(transform.position.z - _mainCamera.transform.position.z);

        dragOffset = transform.position - GetMousePosAndSetDepth();
    }

    private void OnMouseDrag()
    {
        if (!isDragging) return;

        transform.position = GetMousePosAndSetDepth() + dragOffset;
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }

    private Vector3 GetMousePosAndSetDepth()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = objectDepth;

        return _mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }
}
