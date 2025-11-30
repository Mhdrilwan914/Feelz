using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GridItem : MonoBehaviour
{
    private  SpriteRenderer _spriteRenderer; // Visual imge of the tile

    public GridPosition GridPosition { get; private set; } // Position on the grid

    public int TypeID { get; private set; } // gem type {0,1,2}

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public GridItem(GridPosition gridPosition, int typeID, Sprite sprite)
    {
        GridPosition = gridPosition;
        TypeID = typeID;
        _spriteRenderer.sprite = sprite;
    }

    public void SetWorldPosition(Vector3 worldPosition) // sanpping title to exact world position
    {
        transform.localPosition = worldPosition;
    }   
}
