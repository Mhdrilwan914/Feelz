using Assets.Game;
using UnityEngine;

[CreateAssetMenu(fileName = "KeyDefinitionSO", menuName = "Scriptable Objects/KeyDefinitionSO")]
public class KeyDefinitionSO : ScriptableObject
{
    public KeyColorEnum KeyColorEnum;
    public Sprite Sprite;
}
