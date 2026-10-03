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

        private BottleState _state;
        private BottleSelectionController _selectionController;
        private Vector3 _normalScale;
        private Renderer[] _layerRenderers;

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

public bool TryPourTo(BottleController destination, out int pouredLayers)
        {
            pouredLayers = 0;
            if (_state == null || destination == null || destination._state == null)
            {
                return false;
            }

            return _state.TryPourTo(destination._state, out pouredLayers);
        }


        /// <summary>Refreshes each visible slot from the model's bottom-to-top layers.</summary>
        public void RefreshVisuals()
        {
            if (_state == null || _layerRenderers == null)
            {
                return;
            }

            for (int i = 0; i < _layerRenderers.Length; i++)
            {
                bool hasSauce = i < _state.Layers.Count;
                Renderer layerRenderer = _layerRenderers[i];
                layerRenderer.gameObject.SetActive(hasSauce);

                if (hasSauce)
                {
                    layerRenderer.sharedMaterial = GetSauceMaterial(_state.Layers[i]);
                }
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
}
}
