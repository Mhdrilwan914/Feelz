using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static configuration for one order-delivery liquid sorting level.
/// Runtime gameplay state belongs in the board model, not in this asset.
/// </summary>
[CreateAssetMenu(
    fileName = "LevelDefinition",
    menuName = "Sorting New/Level Definition",
    order = 1)]
public sealed class LevelDefinition : ScriptableObject
{
    [SerializeField, Min(1)]
    private int levelNumber = 1;

    [SerializeField]
    private string levelId = "level-001";

    [SerializeField, Min(1)]
    private int bottleCapacity = 4;

    [SerializeField]
    private List<BottleConfiguration> bottles = new();

    [SerializeField]
    private List<int> orderColorIds = new();

    [SerializeField, Min(0)]
    private int moveLimit;

    [SerializeField]
    private LevelDifficulty difficulty = LevelDifficulty.Normal;

    public int LevelNumber => levelNumber;
    public string LevelId => levelId;
    public int BottleCapacity => bottleCapacity;
    public IReadOnlyList<BottleConfiguration> Bottles => bottles;
    public IReadOnlyList<int> OrderColorIds => orderColorIds;
    public int MoveLimit => moveLimit;
    public LevelDifficulty Difficulty => difficulty;

    public int TotalBottleCount => bottles.Count;


    //Empty bottles present in the level, which is useful for determining how many bottles are available for gameplay.
    public int EmptyBottleCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < bottles.Count; i++)
            {
                if (bottles[i].ColorIds.Count == 0)
                {
                    count++;
                }
            }

            return count;
        }
    }

    // This will let us know how many unique colors are present in the level, which is useful for determining the number of orders and other gameplay elements.
    public int LiquidColorCount
    {
        get
        {
            var uniqueColorIds = new HashSet<int>();
            for (int bottleIndex = 0; bottleIndex < bottles.Count; bottleIndex++)
            {
                IReadOnlyList<int> colorIds = bottles[bottleIndex].ColorIds;
                for (int layerIndex = 0; layerIndex < colorIds.Count; layerIndex++)
                {
                    uniqueColorIds.Add(colorIds[layerIndex]);
                }
            }

            return uniqueColorIds.Count;
        }
    }
}

/// <summary>
/// The initial contents of one bottle. Color IDs are ordered bottom to top.
/// </summary>
[Serializable]
public sealed class BottleConfiguration
{
    [SerializeField]
    private List<int> colorIds = new();

    [SerializeField, Min(0)]
    private int hiddenLayerCount;

    public IReadOnlyList<int> ColorIds => colorIds;
    public int HiddenLayerCount => hiddenLayerCount;
}

public enum LevelDifficulty
{
    Easy,
    Normal,
    Hard
}
