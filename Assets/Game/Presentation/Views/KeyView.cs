using UnityEngine;

namespace Assets.Game.Presentation.Views
{
    public sealed class KeyView : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        public void SetSprite(Sprite sprite)
        {
            if (sprite == null)
            {
                DevLog.LogError("Sprite is null. Cannot set sprite.");
                return;
            }
            _spriteRenderer.sprite = sprite;
        }
    }
}