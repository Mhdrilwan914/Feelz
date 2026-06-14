using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundScaleToCamera : MonoBehaviour
{
    [SerializeField] private float _scaleMultiplier = 2f;
    private void Start()
    {
        ScaleBgImage();
    }



    private void ScaleBgImage()
    {
        Camera cam = Camera.main;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        float camSize = cam.orthographicSize * _scaleMultiplier;

        DevLog.Log($"Camera Aspect: {cam.aspect}");

        float scale = Mathf.Max(camSize * cam.aspect / sr.sprite.bounds.size.x, camSize / sr.sprite.bounds.size.y);

        transform.localScale = Vector3.one * scale;
    }
}
