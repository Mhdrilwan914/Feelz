using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ordered collection of level assets and the lookup point for level requests.
/// </summary>
[CreateAssetMenu(
    fileName = "LevelCatalog",
    menuName = "Sorting New/Level Catalog",
    order = 2)]
public sealed class LevelCatalog : ScriptableObject
{
    [SerializeField]
    private List<LevelDefinition> levels = new List<LevelDefinition>();

    private Dictionary<int, LevelDefinition> levelsByNumber;
    private Dictionary<string, LevelDefinition> levelsById;
    private HashSet<int> duplicateLevelNumbers;
    private HashSet<string> duplicateLevelIds;
    private bool lookupTablesBuilt;

    public IReadOnlyList<LevelDefinition> Levels => levels;

    public bool TryGetLevel(int levelNumber, out LevelDefinition levelDefinition)
    {
        EnsureLookupTables();
        levelDefinition = null;

        if (duplicateLevelNumbers.Contains(levelNumber))
        {
            Debug.LogError(
                $"[LevelCatalog] Level number {levelNumber} is duplicated. " +
                "Give each level a unique number before loading it.",
                this);
            return false;
        }

        if (levelsByNumber.TryGetValue(levelNumber, out levelDefinition))
        {
            return true;
        }

        Debug.LogWarning(
            $"[LevelCatalog] No level is configured with number {levelNumber}.",
            this);
        return false;
    }

    public bool TryGetLevel(string levelId, out LevelDefinition levelDefinition)
    {
        EnsureLookupTables();
        levelDefinition = null;

        if (string.IsNullOrWhiteSpace(levelId))
        {
            Debug.LogWarning("[LevelCatalog] Cannot look up a level with an empty ID.", this);
            return false;
        }

        if (duplicateLevelIds.Contains(levelId))
        {
            Debug.LogError(
                $"[LevelCatalog] Level ID '{levelId}' is duplicated. " +
                "Give each level a unique ID before loading it.",
                this);
            return false;
        }

        if (levelsById.TryGetValue(levelId, out levelDefinition))
        {
            return true;
        }

        Debug.LogWarning($"[LevelCatalog] No level is configured with ID '{levelId}'.", this);
        return false;
    }

    /// <summary>
    /// Checks catalog references and identifier uniqueness without modifying assets.
    /// </summary>
    public bool ValidateCatalog()
    {
        bool isValid = true;
        var seenNumbers = new HashSet<int>();
        var seenIds = new HashSet<string>(StringComparer.Ordinal);

        for (int index = 0; index < levels.Count; index++)
        {
            LevelDefinition levelDefinition = levels[index];
            if (levelDefinition == null)
            {
                Debug.LogError($"[LevelCatalog] Entry at index {index} has no Level Definition assigned.", this);
                isValid = false;
                continue;
            }

            if (levelDefinition.LevelNumber < 1)
            {
                Debug.LogError(
                    $"[LevelCatalog] '{levelDefinition.name}' has invalid level number " +
                    $"{levelDefinition.LevelNumber}. Level numbers must be positive.",
                    levelDefinition);
                isValid = false;
            }
            else if (!seenNumbers.Add(levelDefinition.LevelNumber))
            {
                Debug.LogError(
                    $"[LevelCatalog] Duplicate level number {levelDefinition.LevelNumber} " +
                    $"found at catalog entry {index} ('{levelDefinition.name}').",
                    this);
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(levelDefinition.LevelId))
            {
                Debug.LogError($"[LevelCatalog] '{levelDefinition.name}' has an empty level ID.", levelDefinition);
                isValid = false;
            }
            else if (!seenIds.Add(levelDefinition.LevelId))
            {
                Debug.LogError(
                    $"[LevelCatalog] Duplicate level ID '{levelDefinition.LevelId}' " +
                    $"found at catalog entry {index} ('{levelDefinition.name}').",
                    this);
                isValid = false;
            }
        }

        return isValid;
    }

    private void EnsureLookupTables()
    {
        if (lookupTablesBuilt)
        {
            return;
        }

        levelsByNumber = new Dictionary<int, LevelDefinition>();
        levelsById = new Dictionary<string, LevelDefinition>(StringComparer.Ordinal);
        duplicateLevelNumbers = new HashSet<int>();
        duplicateLevelIds = new HashSet<string>(StringComparer.Ordinal);

        for (int index = 0; index < levels.Count; index++)
        {
            LevelDefinition levelDefinition = levels[index];
            if (levelDefinition == null)
            {
                continue;
            }

            int levelNumber = levelDefinition.LevelNumber;
            if (levelNumber > 0 && !levelsByNumber.TryAdd(levelNumber, levelDefinition))
            {
                levelsByNumber.Remove(levelNumber);
                duplicateLevelNumbers.Add(levelNumber);
            }

            string levelId = levelDefinition.LevelId;
            if (!string.IsNullOrWhiteSpace(levelId) && !levelsById.TryAdd(levelId, levelDefinition))
            {
                levelsById.Remove(levelId);
                duplicateLevelIds.Add(levelId);
            }
        }

        lookupTablesBuilt = true;
    }

    private void OnValidate()
    {
        lookupTablesBuilt = false;
        ValidateCatalog();
    }
}
