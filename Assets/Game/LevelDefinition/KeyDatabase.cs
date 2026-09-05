using System.Collections.Generic;
using Assets.Game;
using UnityEngine;

[CreateAssetMenu(fileName = "KeyDatabase", menuName = "Scriptable Objects/KeyDatabase")]
public class KeyDatabase : ScriptableObject
{
    public List<KeyDefinitionSO> Keys;

    private Dictionary<KeyColorEnum, KeyDefinitionSO> _lookUpKey;


    public void Initialize()
    {
        _lookUpKey = new Dictionary<KeyColorEnum, KeyDefinitionSO>();
        foreach (var key in Keys)
        {
            _lookUpKey[key.KeyColorEnum] = key;
        }
    }


    public KeyDefinitionSO GetKeyDefinition(KeyColorEnum keyColor)
    {
        if (_lookUpKey == null)
        {
            Initialize();
        }
        if (_lookUpKey.TryGetValue(keyColor, out var keyDefinition))
        {
            return keyDefinition;
        }
        else
        {
            Debug.LogError($"Key color {keyColor} not found in the database.");
            return null;
        }
    }
}

