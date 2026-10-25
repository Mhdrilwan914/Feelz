using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets.SauceSort.Scripts.Core;
using SauceSort.Core;

namespace SauceSort.Gameplay
{
    /// <summary>
    /// Connects one bottle's logical state to its four visible sauce slots and
    /// forwards player clicks to the scene's selection controller.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class BottleController : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject[] _layerObjects = new GameObject[BottleState.Capacity];
        [SerializeField] private SauceType _startingBottomLayer = SauceType.None;
        [SerializeField] private SauceType _startingLayer1 = SauceType.None;
        [SerializeField] private SauceType _startingLayer2 = SauceType.None;
        [SerializeField] private SauceType _startingTopLayer = SauceType.None;
        [SerializeField] private Material _redSauceMaterial;
        [SerializeField] private Material _blueSauceMaterial;
        [SerializeField] private Material _greenSauceMaterial;
        [SerializeField] private Material _yellowSauceMaterial;
        [SerializeField] private float _selectedScaleMultiplier = 1.1f;
        [SerializeField] private Color _redSurfaceColor = new Color(1f, 0.34f, 0.27f, 1f);
        [SerializeField] private Color _blueSurfaceColor = new Color(0.34f, 0.62f, 1f, 1f);
        [SerializeField] private Color _greenSurfaceColor = new Color(0.35f, 1f, 0.48f, 1f);
        [SerializeField] private Color _yellowSurfaceColor = new Color(1f, 0.9f, 0.32f, 1f);

        private BottleState _state;
        private BottleSelectionController _selectionController;
        private Vector3 _normalScale;
        private Renderer[] _layerRenderers;
        private BottleLiquidRenderer _liquidRenderer;
        private readonly float[] _layerFillFractions = new float[BottleState.Capacity];

        private const int GradientTextureWidth = 32;
        private const int GradientTextureHeight = 64;
        private const float LiquidBottomShade = 0.54f;
        private const float LiquidSideShade = 0.24f;
        private const float LiquidPrimaryHighlightCenter = 0.17f;
        private const float LiquidPrimaryHighlightWidth = 0.04f;
        private const float LiquidPrimaryHighlightStrength = 0.72f;
        private const float LiquidHighlightLowerFadeStart = 0.04f;
        private const float LiquidHighlightLowerFadeEnd = 0.18f;
        private const float LiquidHighlightUpperFadeStart = 0.82f;
        private const float LiquidHighlightUpperFadeEnd = 0.96f;
        private const float LiquidSecondaryHighlightCenter = 0.83f;
        private const float LiquidSecondaryHighlightWidth = 0.07f;
        private const float LiquidSecondaryHighlightStrength = 0.2f;
        private const float LiquidSurfaceHighlightCenter = 0.96f;
        private const float LiquidSurfaceHighlightWidth = 0.055f;
        private const float LiquidSurfaceHighlightCenterStrength = 0.78f;
        private const float LiquidSurfaceHighlightEdgeStrength = 0.38f;
        private static readonly int BaseMapId = Shader.PropertyToID("_BaseMap");
        private static Texture2D _sideGradientTexture;
        private static Texture2D _surfaceGradientTexture;
        private static readonly Dictionary<Material, Material> GlossMaterials =
            new Dictionary<Material, Material>();
        private static readonly Dictionary<KeyValuePair<Material, Color>, Material> SurfaceMaterials =
            new Dictionary<KeyValuePair<Material, Color>, Material>();

        /// <summary>The logical state used by gameplay systems.</summary>
        public BottleState State
        {
            get { return _state; }
        }

        private void Awake()
        {
            _normalScale = transform.localScale;
            _selectionController = GetComponentInParent<BottleSelectionController>();
            CacheLayerRenderers();
            CacheLiquidMeshes();

            var initialLayers = new List<SauceType>(BottleState.Capacity);
            AddIfPresent(initialLayers, _startingBottomLayer);
            AddIfPresent(initialLayers, _startingLayer1);
            AddIfPresent(initialLayers, _startingLayer2);
            AddIfPresent(initialLayers, _startingTopLayer);
            Initialize(new BottleState(initialLayers));
        }

        /// <summary>Connects this view to a logical bottle state.</summary>
        public void Initialize(BottleState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            _state = state;
            RefreshVisuals();
        }

        public bool TryPourTo(BottleController destination)
        {
            if (_state == null || destination == null || destination._state == null)
            {
                return false;
            }

            return _state.TryPourTo(destination._state, out _);
        }


        /// <summary>Refreshes each visible slot from the model's bottom-to-top layers.</summary>
        public void RefreshVisuals()
        {
            if (_state == null || _layerRenderers == null)
            {
                return;
            }

            _liquidRenderer.SetMotion(0f);
            for (int i = 0; i < _layerRenderers.Length; i++)
            {
                bool hasSauce = i < _state.Layers.Count;
                Renderer layerRenderer = _layerRenderers[i];
                layerRenderer.gameObject.SetActive(hasSauce);
                layerRenderer.enabled = hasSauce;
                if (hasSauce)
                {
                    layerRenderer.sharedMaterial = GetSauceMaterial(_state.Layers[i]);
                    ApplyLiquidGradient(layerRenderer, _state.Layers[i]);
                }
            }

            if (_state.Layers.Count > 0)
            {
                _liquidRenderer.SetSurfaceMaterial(
                    GetSurfaceMaterial(_state.Layers[_state.Layers.Count - 1]));
            }
            RefreshLiquidMeshes();
        }

        /// <summary>Updates the exposed surface while the bottle is moving.</summary>
        public void SetLiquidMotion(float motionAmount)
        {
            if (_liquidRenderer != null)
            {
                _liquidRenderer.SetMotion(motionAmount);
            }
        }

        /// <summary>Changes only the selection highlight, not the logical state.</summary>
        public void SetSelected(bool selected)
        {
            transform.localScale = selected
                ? _normalScale * _selectedScaleMultiplier
                : _normalScale;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left
                && _selectionController != null)
            {
                _selectionController.HandleBottleSelection(this);
            }
        }

        private void CacheLayerRenderers()
        {
            if (_layerObjects == null || _layerObjects.Length != BottleState.Capacity)
            {
                Debug.LogError(
                    name + " must reference exactly " + BottleState.Capacity + " layer objects.",
                    this);
                return;
            }

            _layerRenderers = new Renderer[BottleState.Capacity];
            for (int i = 0; i < _layerObjects.Length; i++)
            {
                if (_layerObjects[i] == null)
                {
                    Debug.LogError(name + " has a missing layer object at slot " + i + ".", this);
                    _layerRenderers = null;
                    return;
                }

                _layerRenderers[i] = _layerObjects[i].GetComponent<Renderer>();
                if (_layerRenderers[i] == null)
                {
                    Debug.LogError(name + " layer slot " + i + " needs a Renderer.", this);
                    _layerRenderers = null;
                    return;
                }

            }
        }

        private void CacheLiquidMeshes()
        {
            if (_layerObjects == null || _layerObjects.Length != BottleState.Capacity)
            {
                return;
            }

            _liquidRenderer = new BottleLiquidRenderer(transform, _layerObjects);
        }

        private void RefreshLiquidMeshes()
        {
            if (_liquidRenderer == null || _layerObjects == null || _state == null)
            {
                return;
            }

            int topLayerIndex = _state.Layers.Count - 1;
            for (int layerIndex = 0; layerIndex < _layerObjects.Length; layerIndex++)
            {
                if (layerIndex >= _state.Layers.Count)
                {
                    continue;
                }

                _liquidRenderer.SetSegment(layerIndex, 1f, layerIndex == topLayerIndex);
            }
        }

        /// <summary>
        /// Shrinks the source's top units during a pour without changing its logical contents.
        /// </summary>
        public void SetPourVisualProgress(int pouredLayers, float normalizedProgress)
        {
            if (_state == null || _layerRenderers == null || pouredLayers <= 0)
            {
                return;
            }

            int visiblePourLayers = Mathf.Min(pouredLayers, _state.Layers.Count);
            float transferredLayers = Mathf.Clamp01(normalizedProgress) * visiblePourLayers;
            int topLayerIndex = _state.Layers.Count - 1;
            Array.Clear(_layerFillFractions, 0, _layerFillFractions.Length);
            for (int layerIndex = 0; layerIndex < _state.Layers.Count; layerIndex++)
            {
                int offsetFromTop = topLayerIndex - layerIndex;
                float filledFraction = offsetFromTop < visiblePourLayers
                    ? 1f - Mathf.Clamp01(transferredLayers - offsetFromTop)
                    : 1f;
                _layerFillFractions[layerIndex] = filledFraction;
            }

            _liquidRenderer.SetMotion(Mathf.Sin(Mathf.Clamp01(normalizedProgress) * Mathf.PI));
            int surfaceLayer = FindTopVisibleLayer(_layerFillFractions);
            if (surfaceLayer >= 0)
            {
                _liquidRenderer.SetSurfaceMaterial(GetSurfaceMaterial(_state.Layers[surfaceLayer]));
            }
            for (int layerIndex = 0; layerIndex < _state.Layers.Count; layerIndex++)
            {
                SetLayerFill(layerIndex, _layerFillFractions[layerIndex], layerIndex == surfaceLayer);
            }
        }

        /// <summary>
        /// Shows incoming liquid in the destination while the matching source units are in flight.
        /// </summary>
        public void SetIncomingPourVisualProgress(
            SauceType sauce,
            int incomingLayers,
            float normalizedProgress)
        {
            if (_state == null || _layerRenderers == null || incomingLayers <= 0)
            {
                return;
            }

            int availableSpace = BottleState.Capacity - _state.Layers.Count;
            int previewLayerCount = Mathf.Min(incomingLayers, availableSpace);
            float transferredLayers = Mathf.Clamp01(normalizedProgress) * previewLayerCount;
            int firstPreviewLayer = _state.Layers.Count;
            int lastVisibleLayer = firstPreviewLayer + Mathf.Min(
                Mathf.CeilToInt(transferredLayers), previewLayerCount) - 1;
            int surfaceLayerIndex = Mathf.Clamp(lastVisibleLayer, firstPreviewLayer,
                firstPreviewLayer + previewLayerCount - 1);
            _liquidRenderer.SetSurfaceMaterial(GetSurfaceMaterial(sauce));

            for (int previewOffset = 0; previewOffset < previewLayerCount; previewOffset++)
            {
                int layerIndex = firstPreviewLayer + previewOffset;
                float filledFraction = Mathf.Clamp01(transferredLayers - previewOffset);
                if (filledFraction <= 0f)
                {
                    continue;
                }

                Renderer layerRenderer = _layerRenderers[layerIndex];
                layerRenderer.gameObject.SetActive(true);
                layerRenderer.enabled = true;
                ApplyLiquidGradient(layerRenderer, sauce);
                SetLayerFill(layerIndex, filledFraction, layerIndex == surfaceLayerIndex);
            }

            _liquidRenderer.SetMotion(Mathf.Sin(Mathf.Clamp01(normalizedProgress) * Mathf.PI));
        }

        private static int FindTopVisibleLayer(float[] fillFractions)
        {
            for (int i = fillFractions.Length - 1; i >= 0; i--)
            {
                if (fillFractions[i] > 0f)
                {
                    return i;
                }
            }

            return -1;
        }

        private void SetLayerFill(int layerIndex, float filledFraction, bool exposedSurface)
        {
            float clampedFraction = Mathf.Clamp01(filledFraction);
            _layerRenderers[layerIndex].enabled = clampedFraction > 0f;
            _liquidRenderer.SetSegment(layerIndex, clampedFraction, exposedSurface);
        }

        private void ApplyLiquidGradient(Renderer renderer, SauceType sauce)
        {
            if (renderer == null)
            {
                return;
            }

            Material material = GetSauceMaterial(sauce);
            if (material == null)
            {
                return;
            }

            renderer.sharedMaterial = GetGlossMaterial(material);
        }

        public void ApplySauceAppearance(Renderer renderer, SauceType sauce)
        {
            if (renderer == null)
            {
                return;
            }

            Material material = GetSauceMaterial(sauce);
            if (material == null)
            {
                return;
            }

            renderer.sharedMaterial = GetGlossMaterial(material);
        }

        private static Material GetGlossMaterial(Material sauceMaterial)
        {
            if (GlossMaterials.TryGetValue(sauceMaterial, out Material cached))
            {
                return cached;
            }

            Texture2D gradient = GetGradientTexture(false);
            Material glossyMaterial = new Material(sauceMaterial)
            {
                name = sauceMaterial.name + "_Gloss",
                hideFlags = HideFlags.DontSave
            };
            glossyMaterial.SetTexture(BaseMapId, gradient);
            GlossMaterials.Add(sauceMaterial, glossyMaterial);
            return glossyMaterial;
        }

        private static Texture2D GetGradientTexture(bool includeSurfaceHighlight)
        {
            Texture2D cachedGradient = includeSurfaceHighlight
                ? _surfaceGradientTexture
                : _sideGradientTexture;
            if (cachedGradient != null)
            {
                return cachedGradient;
            }

            Texture2D texture = new Texture2D(
                GradientTextureWidth,
                GradientTextureHeight,
                TextureFormat.RGBA32,
                false,
                true)
            {
                name = "SauceLiquidGlossGradient",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.DontSave
            };

            for (int y = 0; y < GradientTextureHeight; y++)
            {
                float verticalPosition = y / (float)(GradientTextureHeight - 1);
                float verticalShade = Mathf.Lerp(
                    LiquidBottomShade,
                    1f,
                    Mathf.SmoothStep(0f, 1f, verticalPosition));
                Color verticalColor = Color.white * verticalShade;

                for (int x = 0; x < GradientTextureWidth; x++)
                {
                    float horizontalPosition = x / (float)(GradientTextureWidth - 1);
                    float centeredHorizontal = horizontalPosition * 2f - 1f;
                    float sideShade = Mathf.Lerp(
                        1f - LiquidSideShade,
                        1f,
                        1f - centeredHorizontal * centeredHorizontal);
                    Color shaded = verticalColor * sideShade;

                    float highlightRange = Mathf.SmoothStep(
                        LiquidHighlightLowerFadeStart,
                        LiquidHighlightLowerFadeEnd,
                        verticalPosition)
                        * (1f - Mathf.SmoothStep(
                            LiquidHighlightUpperFadeStart,
                            LiquidHighlightUpperFadeEnd,
                            verticalPosition));
                    float primaryHighlight = CalculateGaussian(
                        horizontalPosition,
                        LiquidPrimaryHighlightCenter,
                        LiquidPrimaryHighlightWidth)
                        * LiquidPrimaryHighlightStrength
                        * highlightRange;
                    float secondaryHighlight = CalculateGaussian(
                        horizontalPosition,
                        LiquidSecondaryHighlightCenter,
                        LiquidSecondaryHighlightWidth)
                        * LiquidSecondaryHighlightStrength
                        * highlightRange;
                    float surfaceHighlight = includeSurfaceHighlight
                        ? CalculateGaussian(
                            verticalPosition,
                            LiquidSurfaceHighlightCenter,
                            LiquidSurfaceHighlightWidth)
                            * Mathf.Lerp(
                                LiquidSurfaceHighlightCenterStrength,
                                LiquidSurfaceHighlightEdgeStrength,
                                Mathf.Abs(centeredHorizontal))
                        : 0f;
                    float highlight = Mathf.Clamp01(
                        primaryHighlight + secondaryHighlight + surfaceHighlight);
                    Color pixel = Color.Lerp(shaded, Color.white, highlight);
                    texture.SetPixel(x, y, pixel);
                }
            }

            texture.Apply(false, true);
            if (includeSurfaceHighlight)
            {
                _surfaceGradientTexture = texture;
                return _surfaceGradientTexture;
            }

            _sideGradientTexture = texture;
            return _sideGradientTexture;
        }

        private static float CalculateGaussian(float position, float center, float width)
        {
            float normalizedDistance = (position - center) / width;
            return Mathf.Exp(-normalizedDistance * normalizedDistance);
        }

        private Material GetSauceMaterial(SauceType sauce)
        {
            switch (sauce)
            {
                case SauceType.Red:
                    return _redSauceMaterial;
                case SauceType.Blue:
                    return _blueSauceMaterial;
                case SauceType.Green:
                    return _greenSauceMaterial;
                case SauceType.Yellow:
                    return _yellowSauceMaterial;
                default:
                    return null;
            }
        }

        private Color GetSurfaceColor(SauceType sauce)
        {
            switch (sauce)
            {
                case SauceType.Red:
                    return _redSurfaceColor;
                case SauceType.Blue:
                    return _blueSurfaceColor;
                case SauceType.Green:
                    return _greenSurfaceColor;
                case SauceType.Yellow:
                    return _yellowSurfaceColor;
                default:
                    return Color.white;
            }
        }

        private Material GetSurfaceMaterial(SauceType sauce)
        {
            Material sideMaterial = GetSauceMaterial(sauce);
            if (sideMaterial == null)
            {
                return null;
            }

            Color surfaceColor = GetSurfaceColor(sauce);
            var key = new KeyValuePair<Material, Color>(sideMaterial, surfaceColor);
            if (SurfaceMaterials.TryGetValue(key, out Material cached))
            {
                return cached;
            }

            Material surfaceMaterial = new Material(sideMaterial)
            {
                name = sideMaterial.name + "_Surface",
                color = surfaceColor,
                hideFlags = HideFlags.DontSave
            };
            Texture2D gradient = GetGradientTexture(true);
            surfaceMaterial.SetTexture(BaseMapId, gradient);
            SurfaceMaterials.Add(key, surfaceMaterial);
            return surfaceMaterial;
        }

        private static void AddIfPresent(ICollection<SauceType> layers, SauceType sauce)
        {
            if (sauce != SauceType.None)
            {
                layers.Add(sauce);
            }
        }
    

        public Vector3 GetLayerWorldPosition(int layerIndex)
        {
            if (_layerRenderers == null || layerIndex < 0 || layerIndex >= _layerRenderers.Length)
            {
                return transform.position;
            }

            return _layerRenderers[layerIndex].transform.position;
        }

        public Vector3 GetLayerVisualSize(int layerCount)
        {
            if (_layerRenderers == null || _layerRenderers.Length == 0)
            {
                return Vector3.one;
            }

            Vector3 size = _layerRenderers[0].transform.localScale;
            size.y *= Mathf.Max(1, layerCount);
            return size;
        }

        public Material GetSauceMaterialForPresentation(SauceType sauce)
        {
            return GetSauceMaterial(sauce);
        }

        private void OnDestroy()
        {
            if (_liquidRenderer != null)
            {
                _liquidRenderer.Dispose();
            }
        }
}
}
