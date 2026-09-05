using UnityEngine;

namespace Assets.Game
{
    public sealed class KeyModel
    {
        public int Id { get; set; }

        public KeyColorEnum Color { get; set; }

        public KeyModel(int id, KeyColorEnum keyColor)
        {
            Id = id;
            Color = keyColor;
        }

        public override string ToString()
        {
            return $"KeyModel: Id={Id}, KeyColor={Color}";
        }
    }
}