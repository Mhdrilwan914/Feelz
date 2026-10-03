using UnityEngine;
using SauceSort.Gameplay;
using SauceSort.Core;
using Assets.SauceSort.Scripts.Core;

namespace SauceSort.Levels
{
    /// <summary>Loads fixed level data and advances after a solved board.</summary>
    public sealed class LevelManager : MonoBehaviour
    {
        [SerializeField] private BottleController[] _bottles = new BottleController[6];
        [SerializeField] private BottleSelectionController _selectionController;
        [SerializeField] private float _bottleSpacing = 1.6f;
        [SerializeField] private float _bottleRowY = 5f;

        private LevelData _currentLevel;
        private bool _allLevelsComplete;

        public LevelData CurrentLevel
        {
            get { return _currentLevel; }
        }

        private void Start()
        {
            if (_selectionController == null)
            {
                _selectionController = GetComponent<BottleSelectionController>();
            }

            if (_selectionController != null)
            {
                _selectionController.ValidPourPresentationCompleted += HandleValidPourCompleted;
            }

            StartLevel(1);
        }

        private void OnDestroy()
        {
            if (_selectionController != null)
            {
                _selectionController.ValidPourPresentationCompleted -= HandleValidPourCompleted;
            }
        }

        /// <summary>Loads a level in the existing Game scene.</summary>
        public bool StartLevel(int levelNumber)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Enter Play Mode, then use the LevelManager context menu to start a level.", this);
                return false;
            }

            if (_selectionController == null)
            {
                _selectionController = GetComponent<BottleSelectionController>();
            }

            if (_selectionController != null && _selectionController.IsInputLocked)
            {
                Debug.LogWarning("Wait for the current bottle animation before starting another level.", this);
                return false;
            }

            if (_selectionController != null)
            {
                _selectionController.CancelSelection();
            }

            
if (_bottles == null || _bottles.Length < 6)
            {
                Debug.LogError("LevelManager needs six BottleController references.", this);
                return false;
            }

            LevelData level = SauceSortLevelCatalog.GetLevel(levelNumber);
            if (level.BottleCount > _bottles.Length)
            {
                Debug.LogError("The scene does not have enough bottle instances for " + level.DisplayName + ".", this);
                return false;
            }

            _currentLevel = level;
            _allLevelsComplete = false;

            for (int i = 0; i < _bottles.Length; i++)
            {
                BottleController bottle = _bottles[i];
                if (bottle == null)
                {
                    Debug.LogError("LevelManager has a missing bottle reference at index " + i + ".", this);
                    return false;
                }

                bool used = i < level.BottleCount;
                bottle.gameObject.SetActive(used);
                if (!used)
                {
                    continue;
                }

                float centeredIndex = i - (level.BottleCount - 1) * 0.5f;
                Vector3 position = bottle.transform.position;
                position.x = centeredIndex * _bottleSpacing;
                position.y = _bottleRowY;
                bottle.transform.position = position;
                bottle.transform.rotation = Quaternion.identity;
                bottle.SetSelected(false);
                bottle.Initialize(new BottleState(level.GetBottleContents(i)));
            }

            Debug.Log("Started Level " + level.LevelNumber + ": " + level.DisplayName + ".", this);
            return true;
        }

        [ContextMenu("Start Level 1")]
        private void StartLevelOne()
        {
            StartLevel(1);
        }

        [ContextMenu("Start Level 2")]
        private void StartLevelTwo()
        {
            StartLevel(2);
        }

        [ContextMenu("Start Level 3")]
        private void StartLevelThree()
        {
            StartLevel(3);
        }

        private void HandleValidPourCompleted()
        {
            if (_currentLevel == null || _allLevelsComplete || !IsCurrentLevelSolved())
            {
                return;
            }

            if (_currentLevel.LevelNumber < SauceSortLevelCatalog.Count)
            {
                int completedLevel = _currentLevel.LevelNumber;
                Debug.Log("Level " + completedLevel + " complete. Loading Level "
                    + (completedLevel + 1) + ".", this);
                StartLevel(completedLevel + 1);
                return;
            }

            _allLevelsComplete = true;
            Debug.Log("All three Sauce Sort prototype levels are complete!", this);
        }

        private bool IsCurrentLevelSolved()
        {
            for (int i = 0; i < _currentLevel.BottleCount; i++)
            {
                BottleState state = _bottles[i].State;
                if (state.IsEmpty)
                {
                    continue;
                }

                if (!state.IsFull)
                {
                    return false;
                }

                SauceType firstSauce = state.Layers[0];
                for (int layer = 1; layer < state.Layers.Count; layer++)
                {
                    if (state.Layers[layer] != firstSauce)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}