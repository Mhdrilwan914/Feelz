using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class LandScapeSafeAreaScaler : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Rect _lastSafeArea = Rect.zero;
    private Vector2 _lastResolution = Vector2.zero;


    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }


    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
            ApplySafeArea();
    }

    private void OnRectTransformDimensionsChange()
    {
        if (Screen.safeArea != _lastSafeArea ||
           _lastResolution.x != Screen.width ||
           _lastResolution.y != Screen.height)

        {
            Debug.Log($"Screen Width {Screen.width} and Screen Height {Screen.height}");
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        Debug.Log($"Safe Area: {safeArea}");
        Vector2 anchorMin = safeArea.position;
        Debug.Log($"Anchor Min: {anchorMin}");
        Vector2 anchorMax = safeArea.position + safeArea.size;
        Debug.Log($"Anchor Max: {anchorMax}");

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;

        _rectTransform.offsetMin = Vector2.zero;
        _rectTransform.offsetMax = Vector2.zero;

        _lastSafeArea = safeArea;
        _lastResolution = new Vector2(Screen.width, Screen.height);

    }
}

