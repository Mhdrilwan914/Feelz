using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ScreenResolution : MonoBehaviour
{
    public float MinX { get; private set; }
    public float MaxX { get; private set; }
    private void Awake()
    {
        SetScreenResoution();
    }

    private void Start()
    {
        CalculateBounds();
    }

    private void SetScreenResoution()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Rect safeArea = Screen.safeArea;
        Vector2 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(safeArea.xMin, safeArea.yMin)); //bottom left corner of the screen of safe area
        Vector2 topRight = Camera.main.ScreenToWorldPoint(new Vector3(safeArea.xMax, safeArea.yMax)); //top right corner of the screen of safe area

        float width = topRight.x - bottomLeft.x;  //width of the screen of safe area
        float height = topRight.y - bottomLeft.y;   //height of the screen of safe area
     
        float spriteWidth = spriteRenderer.sprite.bounds.size.x; //width of the sprite
        float spriteHeight = spriteRenderer.sprite.bounds.size.y; //height of the sprite

        float scaleX = width / spriteWidth;
        float scaleY = height / spriteHeight;

        float scale = Mathf.Min(scaleX, scaleY);

        transform.localScale = new Vector3(scale, scale, 1f);
    }

    public float GetScreenBottomY()
    {
        Camera cam = Camera.main;
        float screenBottomY = cam.transform.position.y - cam.orthographicSize;
        return screenBottomY;
    }


    private void CalculateBounds()
    {
        Camera cam = Camera.main;

        float screenHalfHeight = cam.orthographicSize;
        float screenHalfWidth = screenHalfHeight * cam.aspect;

        MinX = -screenHalfWidth;
        MaxX = screenHalfWidth;

        DevLog.Log("MMinx" + MinX);
        DevLog.Log("MMaxX" + MaxX);
    }
}
