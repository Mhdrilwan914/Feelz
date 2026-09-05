using System.Collections.Generic;

// LevelDefeinition contains the definition of a level,
// including the hook capacity and a list of hook definitions.
[System.Serializable]
public class LevelDefinition
{
    public int HookCapacity { get; set; }

    public List<HookDefinition> Hooks;
}
