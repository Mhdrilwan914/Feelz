using UnityEngine;


namespace Assets.KnifeGame.Scripts.Core.Target
{
    [CreateAssetMenu(fileName = "TargetData", menuName = "Scriptable Objects/TargetData")]
    public class TargetData : ScriptableObject
    {
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private bool _isRotatingClockwise = true;
        [SerializeField] private Sprite _targetVisual;

        public float RotationSpeed => _rotationSpeed;
        public bool IsRotatingClockwise => _isRotatingClockwise;
        public Sprite Sprite => _targetVisual;
    }
}