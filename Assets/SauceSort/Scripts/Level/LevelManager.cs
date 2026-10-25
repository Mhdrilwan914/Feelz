using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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
        [SerializeField] private float _bottleRowSpacing = 4.8f;
        [SerializeField, Range(0.35f, 1f)] private float _portraitGridAspect = 0.65f;
        [SerializeField] private float _layoutPadding = 0.8f;

        [SerializeField] private ConveyorController _conveyor;
        [SerializeField] private float _bottleRowY = 5f;

        [SerializeField] private float _levelAdvanceDelay = 1.5f;

        private LevelData _currentLevel;
        private bool _allLevelsComplete;
        private Text _progressLabel;
        private Camera _gameCamera;
        private int _layoutScreenWidth;
        private int _layoutScreenHeight;

        public LevelData CurrentLevel
        {
            get { return _currentLevel; }
        }

        private void Start()
        {
            CreateProgressLabel();
            _gameCamera = Camera.main;

            if (_selectionController == null)
            {
                _selectionController = GetComponent<BottleSelectionController>();
            }

            if (_selectionController != null)
            {
                _selectionController.ValidPourPresentationCompleted += HandleValidPourCompleted;
            }

            if (_conveyor == null)
            {
                GameObject conveyorArea = GameObject.Find("ConveyorArea");
                if (conveyorArea != null)
                {
                    _conveyor = conveyorArea.GetComponent<ConveyorController>();
                }
            }

            if (_conveyor != null)
            {
                _conveyor.AllBottlesMatched += HandleAllBottlesMatched;
            }

            StartLevel(1);
        }

        private void OnDestroy()
        {
            if (_selectionController != null)
            {
                _selectionController.ValidPourPresentationCompleted -= HandleValidPourCompleted;
            }

            if (_conveyor != null)
            {
                _conveyor.AllBottlesMatched -= HandleAllBottlesMatched;
            }
        }

        private void Update()
        {
            if (_currentLevel == null
                || (Screen.width == _layoutScreenWidth && Screen.height == _layoutScreenHeight)
                || (_selectionController != null && _selectionController.IsInputLocked))
                return;

            _layoutScreenWidth = Screen.width;
            _layoutScreenHeight = Screen.height;
            for (int i = 0; i < _currentLevel.BottleCount; i++)
            {
                BottleController bottle = _bottles[i];
                if (bottle != null && bottle.enabled)
                    ApplyBottleLayout(bottle, i, _currentLevel.BottleCount);
            }

            FitCameraToGame(_currentLevel.BottleCount);
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
                _selectionController.SetInputEnabled(true);
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

            if (_progressLabel != null)
            {
                _progressLabel.text = "Level " + level.LevelNumber + " / " + SauceSortLevelCatalog.Count;
            }


            if (_conveyor != null)
            {
                _conveyor.BeginLevel(level, _bottles);
            }
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

                ApplyBottleLayout(bottle, i, level.BottleCount);
                bottle.transform.rotation = Quaternion.identity;
                bottle.SetSelected(false);
                bottle.Initialize(new BottleState(level.GetBottleContents(i)));
            }

            FitCameraToGame(level.BottleCount);

            Debug.Log("Started Level " + level.LevelNumber + ": " + level.DisplayName + ".", this);
            return true;
        }

        private void ApplyBottleLayout(BottleController bottle, int index, int bottleCount)
        {
            float aspect = GetScreenAspect();
            int columns = aspect < _portraitGridAspect
                ? Mathf.Min(bottleCount <= 4 ? 2 : 3, bottleCount)
                : bottleCount;
            int rows = Mathf.CeilToInt(bottleCount / (float)columns);
            int row = index / columns;
            int rowCount = Mathf.Min(columns, bottleCount - row * columns);
            int column = index % columns;
            float centeredColumn = column - (rowCount - 1) * 0.5f;
            float centeredRow = row - (rows - 1) * 0.5f;

            Vector3 position = bottle.transform.position;
            position.x = centeredColumn * _bottleSpacing;
            position.y = _bottleRowY - centeredRow * _bottleRowSpacing;
            bottle.transform.position = position;
        }

        private void FitCameraToGame(int bottleCount)
        {
            if (_gameCamera == null) _gameCamera = Camera.main;
            if (_gameCamera == null || !_gameCamera.orthographic) return;

            float aspect = GetScreenAspect();
            int columns = aspect < _portraitGridAspect
                ? Mathf.Min(bottleCount <= 4 ? 2 : 3, bottleCount)
                : bottleCount;
            int rows = Mathf.CeilToInt(bottleCount / (float)columns);
            float bottleWidth = 1.35f;
            float rowWidth = (Mathf.Min(columns, bottleCount) - 1) * _bottleSpacing + bottleWidth;
            // Four conveyor targets is the widest level layout. Leave room for parked bottles too.
            float conveyorWidth = 3 * 1.55f + bottleWidth * 0.78f;
            float requiredWidth = Mathf.Max(rowWidth, conveyorWidth) + _layoutPadding * 2f;
            float top = _bottleRowY + (rows - 1) * _bottleRowSpacing * 0.5f + 2.4f;
            float bottom = -4.9f;
            float requiredHeight = top - bottom + _layoutPadding * 2f;

            _gameCamera.orthographicSize = Mathf.Max(
                requiredHeight * 0.5f,
                requiredWidth / (2f * aspect));
            Vector3 cameraPosition = _gameCamera.transform.position;
            cameraPosition.y = (top + bottom) * 0.5f;
            _gameCamera.transform.position = cameraPosition;
            _layoutScreenWidth = Screen.width;
            _layoutScreenHeight = Screen.height;
        }

        private static float GetScreenAspect()
        {
            if (Screen.height <= 0) return 9f / 16f;
            return Mathf.Max(0.1f, Screen.width / (float)Screen.height);
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
            if (_currentLevel == null || _allLevelsComplete)
            {
                return;
            }

            if (_conveyor != null)
            {
                _conveyor.CollectCompletedBottles(_bottles, _currentLevel.BottleCount);
                return;
            }

            if (IsCurrentLevelSolved())
            {
                HandleAllBottlesMatched();
            }
        }

        private void HandleAllBottlesMatched()
        {
            if (_currentLevel == null || _allLevelsComplete)
            {
                return;
            }

            if (_selectionController != null)
            {
                _selectionController.SetInputEnabled(false);
            }

            if (_currentLevel.LevelNumber < SauceSortLevelCatalog.Count)
            {
                int completedLevel = _currentLevel.LevelNumber;
                int nextLevel = completedLevel + 1;
                if (_progressLabel != null)
                {
                    _progressLabel.text = "Level " + completedLevel + " complete!\nNext: Level " + nextLevel;
                }

                StartCoroutine(AdvanceToLevel(nextLevel));
                return;
            }

            _allLevelsComplete = true;
            if (_progressLabel != null)
            {
                _progressLabel.text = "All levels complete!";
            }
            Debug.Log("All three Sauce Sort prototype levels are complete!", this);
        }

        private IEnumerator AdvanceToLevel(int levelNumber)
        {
            yield return new WaitForSeconds(_levelAdvanceDelay);
            StartLevel(levelNumber);
        }

        private void CreateProgressLabel()
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogWarning("Level progress UI needs a Canvas in the scene.", this);
                return;
            }

            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1080f, 1920f);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
            }

            GameObject labelObject = new GameObject("SauceSortProgress", typeof(RectTransform), typeof(Text));
            labelObject.transform.SetParent(canvas.transform, false);

            RectTransform rect = labelObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.05f, 1f);
            rect.anchorMax = new Vector2(0.95f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -24f);
            rect.sizeDelta = new Vector2(0f, 100f);

            _progressLabel = labelObject.GetComponent<Text>();
            _progressLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _progressLabel.fontSize = 34;
            _progressLabel.fontStyle = FontStyle.Bold;
            _progressLabel.alignment = TextAnchor.MiddleCenter;
            _progressLabel.color = Color.white;
            _progressLabel.raycastTarget = false;

            Outline outline = labelObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.85f);
            outline.effectDistance = new Vector2(2f, -2f);
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
