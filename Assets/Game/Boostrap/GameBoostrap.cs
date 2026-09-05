using UnityEngine;
using Zenject;

public class GameBoostrap : MonoBehaviour
{
    [SerializeField] private LevelDefinitionSO _levelDefinitionSO;

    private ILevelLoader _levelLoader;

    [Inject]
    public void Construct(ILevelLoader levelLoader)
    {
        this._levelLoader = levelLoader;
    }

    private void Start()
    {
        _levelLoader.LoadLevel(_levelDefinitionSO);
    }
}
