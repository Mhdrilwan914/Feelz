using System;
using System.Collections;
using System.Collections.Generic;
using Assets.SauceSort.Scripts.Core;
using SauceSort.Core;
using SauceSort.Levels;
using UnityEngine;

namespace SauceSort.Gameplay
{
    /// <summary>Moves full, single-sauce bottles to matching conveyor targets.</summary>
    public sealed class ConveyorController : MonoBehaviour
    {
        [SerializeField] private float _beltCenterY = -4.1f;
        [SerializeField] private float _bottleParkY = -3f;
        [SerializeField] private float _slotSpacing = 1.55f;
        [SerializeField] private float _travelDuration = 0.55f;
        [SerializeField] private float _parkedScale = 0.78f;
        [SerializeField] private float _beltWidth = 8.5f;

        private const int SauceCount = (int)SauceType.Yellow;
        private readonly int[] _targets = new int[SauceCount + 1];
        private readonly int[] _reserved = new int[SauceCount + 1];
        private readonly HashSet<BottleController> _accepted = new HashSet<BottleController>();
        private readonly List<GameObject> _markers = new List<GameObject>();

        private int _targetTotal;
        private int _delivered;
        private bool _completionRaised;

        public event Action AllBottlesMatched;

        private void Awake() { BuildBelt(); }

        public void BeginLevel(LevelData level, BottleController[] bottles)
        {
            if (level == null) throw new ArgumentNullException(nameof(level));

            StopAllCoroutines();
            Array.Clear(_targets, 0, _targets.Length);
            Array.Clear(_reserved, 0, _reserved.Length);
            _accepted.Clear();
            _targetTotal = _delivered = 0;
            _completionRaised = false;
            ClearMarkers();
            ResetBottles(bottles);

            for (int i = 0; i < level.BottleCount; i++)
                foreach (SauceType sauce in level.GetBottleContents(i))
                    _targets[(int)sauce]++;

            // Each complete bottle needs one target; incomplete remainders are ignored.
            for (int i = (int)SauceType.Red; i <= SauceCount; i++)
            {
                _targets[i] /= level.BottleCapacity;
                _targetTotal += _targets[i];
            }

            BuildMarkers(bottles);
        }

        public void CollectCompletedBottles(BottleController[] bottles, int activeCount)
        {
            if (bottles == null) return;

            for (int i = 0; i < Mathf.Min(activeCount, bottles.Length); i++)
            {
                BottleController bottle = bottles[i];
                if (bottle == null || _accepted.Contains(bottle)
                    || !TryGetCompletedSauce(bottle, out SauceType sauce))
                    continue;

                int index = (int)sauce;
                if (_reserved[index] >= _targets[index]) continue;

                int slot = GetSlot(index, _reserved[index]++);
                _accepted.Add(bottle);
                bottle.SetSelected(false);
                SetBottleEnabled(bottle, false);
                StartCoroutine(DeliverBottle(bottle, sauce, slot));
            }
        }

        private IEnumerator DeliverBottle(BottleController bottle, SauceType sauce, int slot)
        {
            Transform item = bottle.transform;
            Vector3 start = item.position;
            Vector3 destination = new Vector3(
                transform.position.x + SlotOffset(slot),
                transform.position.y + _bottleParkY,
                start.z);
            Vector3 scale = item.localScale;

            float elapsed = 0f;
            while (elapsed < _travelDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / _travelDuration);
                item.position = Vector3.Lerp(start, destination, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }

            item.position = destination;
            item.localScale = scale * _parkedScale;
            _delivered++;
            Debug.Log("Conveyor matched " + sauce + " (" + _delivered + "/" + _targetTotal + ").", bottle);
            if (slot >= 0 && slot < _markers.Count) _markers[slot].SetActive(false);

            if (!_completionRaised && _targetTotal > 0 && _delivered >= _targetTotal)
            {
                _completionRaised = true;
                AllBottlesMatched?.Invoke();
            }
        }

        private bool TryGetCompletedSauce(BottleController bottle, out SauceType sauce)
        {
            sauce = SauceType.None;
            BottleState state = bottle.State;
            if (state == null || state.Layers.Count != BottleState.Capacity) return false;

            sauce = state.Layers[0];
            for (int i = 1; i < state.Layers.Count; i++)
                if (state.Layers[i] != sauce)
                {
                    sauce = SauceType.None;
                    return false;
                }

            return true;
        }

        private int GetSlot(int sauce, int occurrence)
        {
            int slot = 0;
            for (int i = (int)SauceType.Red; i < sauce; i++) slot += _targets[i];
            return slot + occurrence;
        }

        private float SlotOffset(int slot)
        {
            return (slot - (_targetTotal - 1) * 0.5f) * _slotSpacing;
        }

        private void ResetBottles(BottleController[] bottles)
        {
            if (bottles == null) return;
            foreach (BottleController bottle in bottles)
            {
                if (bottle == null) continue;
                SetBottleEnabled(bottle, true);
                bottle.SetSelected(false);
            }
        }

        private static void SetBottleEnabled(BottleController bottle, bool enabled)
        {
            Collider collider = bottle.GetComponent<Collider>();
            if (collider != null) collider.enabled = enabled;
            bottle.enabled = enabled;
        }

        private void BuildBelt()
        {
            GameObject belt = GameObject.CreatePrimitive(PrimitiveType.Cube);
            belt.name = "ConveyorBeltVisual";
            belt.transform.SetParent(transform, false);
            belt.transform.localPosition = new Vector3(0f, _beltCenterY, 0.5f);
            belt.transform.localScale = new Vector3(_beltWidth, 0.28f, 0.85f);
            RemoveCollider(belt);
        }

        private void BuildMarkers(BottleController[] bottles)
        {
            int slot = 0;
            for (int sauce = (int)SauceType.Red; sauce <= SauceCount; sauce++)
            {
                for (int n = 0; n < _targets[sauce]; n++, slot++)
                {
                    GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    marker.name = ((SauceType)sauce) + "MatchTarget_" + (n + 1);
                    marker.transform.SetParent(transform, false);
                    marker.transform.localPosition = new Vector3(
                        SlotOffset(slot), _beltCenterY - 0.48f, 0.5f);
                    marker.transform.localScale = new Vector3(0.72f, 0.06f, 0.72f);
                    RemoveCollider(marker);

                    Renderer renderer = marker.GetComponent<Renderer>();
                    Material material = FindSauceMaterial(bottles, (SauceType)sauce);
                    if (renderer != null && material != null) renderer.sharedMaterial = material;
                    _markers.Add(marker);
                }
            }
        }

        private static Material FindSauceMaterial(BottleController[] bottles, SauceType sauce)
        {
            if (bottles == null) return null;
            foreach (BottleController bottle in bottles)
            {
                if (bottle == null) continue;
                Material material = bottle.GetSauceMaterialForPresentation(sauce);
                if (material != null) return material;
            }

            return null;
        }

        private void ClearMarkers()
        {
            foreach (GameObject marker in _markers)
                if (marker != null) Destroy(marker);
            _markers.Clear();
        }

        private static void RemoveCollider(GameObject target)
        {
            Collider collider = target.GetComponent<Collider>();
            if (collider != null) Destroy(collider);
        }
    }
}
