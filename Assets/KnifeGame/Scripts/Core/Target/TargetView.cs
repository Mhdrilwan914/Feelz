using UnityEngine;

namespace Assets.KnifeGame.Scripts.Core.Target
{
    public class TargetView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _SpriteRenderer;



        // <summary>
        // Initializes the target view with the specified target data.
        // Parameter data: The target data to initialize the view with.
        public void Initialize(TargetData data)
        {
            _SpriteRenderer.sprite = data.Sprite;
        }



        // <summary>
        // Rotates the target around the Z-axis at a specified speed.
        // Parameter speed: The speed at which to rotate the target.
        public void Rotate(float speed)
        {
            transform.Rotate(Vector3.forward, speed * Time.deltaTime);

        }
    }
}