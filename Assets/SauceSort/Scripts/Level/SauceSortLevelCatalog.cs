using SauceSort.Core;

namespace SauceSort.Levels
{
    /// <summary>Three fixed, hand-authored Water Sort prototype levels.</summary>
    public static class SauceSortLevelCatalog
    {
        private static readonly LevelData[] Levels =
        {
            new LevelData(1, "Tutorial", 4, new[]
            {
                new[] { SauceType.Red, SauceType.Red, SauceType.Blue, SauceType.Blue },
                new[] { SauceType.Blue, SauceType.Blue, SauceType.Red, SauceType.Red },
                new SauceType[0],
                new SauceType[0]
            }),
            new LevelData(2, "Three Sauces", 4, new[]
            {
                new[] { SauceType.Red, SauceType.Red, SauceType.Green, SauceType.Green },
                new[] { SauceType.Green, SauceType.Green, SauceType.Blue, SauceType.Blue },
                new[] { SauceType.Blue, SauceType.Blue, SauceType.Red, SauceType.Red },
                new SauceType[0],
                new SauceType[0]
            }),
            new LevelData(3, "Four Sauces", 4, new[]
            {
                new[] { SauceType.Red, SauceType.Blue, SauceType.Green, SauceType.Yellow },
                new[] { SauceType.Blue, SauceType.Green, SauceType.Yellow, SauceType.Red },
                new[] { SauceType.Green, SauceType.Yellow, SauceType.Red, SauceType.Blue },
                new[] { SauceType.Yellow, SauceType.Red, SauceType.Blue, SauceType.Green },
                new SauceType[0],
                new SauceType[0]
            })
        };

        public static int Count
        {
            get { return Levels.Length; }
        }

        public static LevelData GetLevel(int levelNumber)
        {
            if (levelNumber < 1 || levelNumber > Levels.Length)
            {
                throw new System.ArgumentOutOfRangeException(nameof(levelNumber));
            }

            return Levels[levelNumber - 1];
        }
    }
}