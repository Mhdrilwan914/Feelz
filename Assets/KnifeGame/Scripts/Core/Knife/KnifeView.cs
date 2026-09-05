using System;
using UnityEngine;


namespace Assets.KnifeGame.Scripts.Core.Knife
{
    public class KnifeView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Rigidbody2D _rigidBody;

        private float _insertDepth;
        public event Action<HitInfo> OnHItDetected;
        public void Initialize(KnifeData knifeData)
        {
            _spriteRenderer.sprite = knifeData.Sprite;
            _insertDepth = knifeData.InsertDepth;
        }

        public void Throw(float throwForce)
        {
            _rigidBody.linearVelocity = Vector2.zero;
            _rigidBody.angularVelocity = 0f;
            _rigidBody.AddForce(Vector2.up * throwForce, ForceMode2D.Impulse);
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            ContactPoint2D contactPoint2D = collision.GetContact(0); // Get the first contact point of the collision

            HitInfo hitInfo = new(

                contactPoint: contactPoint2D.point,
                hitTransform: collision.transform

            );

            OnHItDetected?.Invoke(hitInfo);
        }

        public void Stick(HitInfo hitInfo)
        {

            _rigidBody.linearVelocity = Vector2.zero;
            _rigidBody.angularVelocity = 0f;
            _rigidBody.simulated = false;
            transform.position += transform.up * _insertDepth;
            transform.SetParent(hitInfo.HitTransform, true);
        }

    }
}