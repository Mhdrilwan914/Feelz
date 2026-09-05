using System.Collections.Generic;
using System.Linq;

namespace Assets.Game.Domain.Models
{
    public sealed class HookModel
    {
        private readonly Stack<KeyModel> _keys;

        public int Capacity { get; }
        public int Count => _keys?.Count ?? 0;


        public bool IsEmpty => _keys?.Count == 0;
        public bool IsFull => _keys?.Count >= Capacity;



        public KeyModel TopKey => IsEmpty ? null : _keys?.Peek();

        public IReadOnlyCollection<KeyModel> Keys => _keys;

        public HookModel(int capacity)
        {
            Capacity = capacity;
            _keys = new Stack<KeyModel>();
        }

        public bool TryPush(KeyModel key)
        {
            if (IsFull)
            {
                return false;
            }
            _keys?.Push(key);
            return true;
        }



        public bool TryPop(out KeyModel key)
        {
            if (IsEmpty)
            {
                key = null;
                return false;
            }
            key = _keys?.Pop();
            return true;
        }


        public bool IsUniform()
        {
            if (_keys?.Count == 0)
                return false;

            KeyColorEnum firstColor = _keys.Peek().Color;

            return _keys.All(k => k.Color == firstColor);
        }

        public bool IsComplete()
        {
            return IsFull && IsUniform();
        }
    }
}

