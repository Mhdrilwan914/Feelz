using System;
using SauceSort.Core;

namespace SauceSort.Levels
{
    /// <summary>Immutable initial contents and capacity for one prototype level.</summary>
    [Serializable]
    public sealed class LevelData
    {
        private readonly SauceType[][] _initialBottleContents;

        public int LevelNumber { get; private set; }
        public string DisplayName { get; private set; }
        public int BottleCapacity { get; private set; }
        public int BottleCount
        {
            get { return _initialBottleContents.Length; }
        }

        public LevelData(
            int levelNumber,
            string displayName,
            int bottleCapacity,
            SauceType[][] initialBottleContents)
        {
            if (levelNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(levelNumber));
            }

            if (bottleCapacity < 1 || initialBottleContents == null
                || initialBottleContents.Length == 0)
            {
                throw new ArgumentException("A level needs a positive capacity and at least one bottle.");
            }

            LevelNumber = levelNumber;
            DisplayName = displayName;
            BottleCapacity = bottleCapacity;
            _initialBottleContents = new SauceType[initialBottleContents.Length][];

            for (int i = 0; i < initialBottleContents.Length; i++)
            {
                SauceType[] layers = initialBottleContents[i] ?? new SauceType[0];
                if (layers.Length > bottleCapacity)
                {
                    throw new ArgumentException("Bottle " + (i + 1) + " exceeds level capacity.");
                }

                _initialBottleContents[i] = new SauceType[layers.Length];
                for (int j = 0; j < layers.Length; j++)
                {
                    if (layers[j] == SauceType.None)
                    {
                        throw new ArgumentException("Empty slots must be represented by an empty layer array.");
                    }

                    _initialBottleContents[i][j] = layers[j];
                }
            }
        }

        /// <summary>Returns a copy ordered from bottom to top.</summary>
        public SauceType[] GetBottleContents(int bottleIndex)
        {
            SauceType[] contents = _initialBottleContents[bottleIndex];
            SauceType[] copy = new SauceType[contents.Length];
            Array.Copy(contents, copy, contents.Length);
            return copy;
        }
    }
}