using UnityEngine;

namespace Assets.Game.Presentation.Views
{
    public sealed class HookView : MonoBehaviour
    {
        [SerializeField] private Transform[] _slots;

        public Transform GetSlot(int index)
        {
            if (index < 0 || index >= _slots.Length)
            {
                DevLog.LogError($"Invalid slot index: {index}");
                return null;
            }
            return _slots[index];
        }

        public int SlotCount => _slots.Length;
    }
}