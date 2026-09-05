using UnityEngine;

namespace Assets.KnifeGame.Scripts.Core.Knife
{
    [CreateAssetMenu(fileName = "KnifeData", menuName = "Scriptable Objects/KnifeData")]
    public class KnifeData : ScriptableObject
    {
        [Header("GamePlay")]
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _throwForce = 10f;
        [SerializeField] private float _insertDepth = 0.5f;

        [Header("Visual")]
        [SerializeField] private Sprite _sprite;
        [SerializeField] private string _knifeName;


        public string KnifeName => _knifeName;

        public float Damage => _damage;

        public float ThrowForce => _throwForce;

        public Sprite Sprite => _sprite;

        public float InsertDepth => _insertDepth;
    }
}