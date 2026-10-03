using System;
using System.Collections;
using UnityEngine;
using SauceSort.Core;

namespace SauceSort.Gameplay
{
    /// <summary>
    /// Handles only the visual presentation of a pour. BottleState validation and
    /// mutation are supplied by the caller through callbacks.
    /// </summary>
    public sealed class BottlePourAnimator : MonoBehaviour
    {
        [SerializeField] private float _tiltDegrees = 24f;
        [SerializeField] private float _tiltDuration = 0.18f;
        [SerializeField] private float _approachDuration = 0.16f;
        [SerializeField] private float _returnDuration = 0.16f;
        [SerializeField] private float _destinationClearance = 0.1f;
        [SerializeField] private float _invalidShakeDuration = 0.2f;
        [SerializeField] private float _invalidShakeDistance = 0.08f;
        [SerializeField] private float _pourDuration = 0.32f;
        [SerializeField] private float _settleDuration = 0.12f;
        [SerializeField] private float _pourArcHeight = 0.45f;
        [SerializeField] private float _settleScale = 1.04f;

        private bool _isPlaying;

        public bool IsPlaying
        {
            get { return _isPlaying; }
        }

        /// <summary>
        /// Starts the presentation for an already validated group transfer.
        /// The arrival callback applies the model move; the completion callback
        /// releases the caller's input lock.
        /// </summary>
        public bool PlayPour(
            BottleController source,
            BottleController destination,
            SauceType sauce,
            int groupSize,
            Func<bool> onSauceArrived,
            Action onComplete)
        {
            if (_isPlaying || source == null || destination == null
                || groupSize <= 0 || onSauceArrived == null)
            {
                return false;
            }

            _isPlaying = true;
            StartCoroutine(PlayPourRoutine(
                source, destination, sauce, groupSize, onSauceArrived, onComplete));
            return true;
        }

private IEnumerator PlayPourRoutine(
            BottleController source,
            BottleController destination,
            SauceType sauce,
            int groupSize,
            Func<bool> onSauceArrived,
            Action onComplete)
        {
            Vector3 sourceStartPosition = source.transform.position;
            Quaternion sourceStartRotation = source.transform.rotation;
            Vector3 destinationStartScale = destination.transform.localScale;
            Vector3 temporaryPourPosition = CalculateTemporaryPourPosition(
                source, destination, sourceStartPosition);
            float direction = Mathf.Sign(
                destination.transform.position.x - sourceStartPosition.x);
            if (direction == 0f)
            {
                direction = 1f;
            }

            Quaternion sourceTiltRotation = Quaternion.AngleAxis(
                -direction * _tiltDegrees, Vector3.forward) * sourceStartRotation;
            GameObject pouringSauce = null;

            try
            {
                yield return AnimatePosition(
                    source.transform,
                    sourceStartPosition,
                    temporaryPourPosition,
                    _approachDuration);

                yield return AnimateRotation(
                    source.transform, sourceStartRotation, sourceTiltRotation, _tiltDuration);

                int topLayerIndex = source.State.Layers.Count - 1;
                int destinationSlotIndex = destination.State.Layers.Count;
                Vector3 startPosition = source.GetLayerWorldPosition(topLayerIndex)
                    - source.transform.up
                    * (source.GetLayerVisualSize(1).y * (groupSize - 1) * 0.5f);
                Vector3 destinationPosition = destination.GetLayerWorldPosition(destinationSlotIndex);
                float layerHeight = destination.GetLayerVisualSize(1).y;
                destinationPosition += destination.transform.up
                    * (layerHeight * (groupSize - 1) * 0.5f);

                pouringSauce = CreatePouringSauce(source, sauce, groupSize);
                if (pouringSauce != null)
                {
                    pouringSauce.transform.position = startPosition;
                    yield return AnimateSauce(
                        pouringSauce.transform,
                        startPosition,
                        destinationPosition,
                        _pourDuration);
                }

                bool moveApplied = onSauceArrived();
                if (pouringSauce != null)
                {
                    Destroy(pouringSauce);
                    pouringSauce = null;
                }

                if (moveApplied)
                {
                    yield return AnimateSettle(
                        destination.transform, destinationStartScale, _settleDuration);
                }

                yield return AnimateRotation(
                    source.transform, sourceTiltRotation, sourceStartRotation, _tiltDuration);
                yield return AnimatePosition(
                    source.transform,
                    temporaryPourPosition,
                    sourceStartPosition,
                    _returnDuration);
            }
            finally
            {
                if (pouringSauce != null)
                {
                    Destroy(pouringSauce);
                }

                source.transform.SetPositionAndRotation(
                    sourceStartPosition, sourceStartRotation);
                destination.transform.localScale = destinationStartScale;
                _isPlaying = false;

                if (onComplete != null)
                {
                    onComplete();
                }
            }
        }

        private GameObject CreatePouringSauce(
            BottleController source,
            SauceType sauce,
            int groupSize)
        {
            Material sauceMaterial = source.GetSauceMaterialForPresentation(sauce);
            if (sauceMaterial == null)
            {
                Debug.LogError("The source bottle has no material for " + sauce + ".", source);
                return null;
            }

            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visual.name = "PouringSauce";
            Collider visualCollider = visual.GetComponent<Collider>();
            if (visualCollider != null)
            {
                Destroy(visualCollider);
            }

            Renderer visualRenderer = visual.GetComponent<Renderer>();
            visualRenderer.sharedMaterial = sauceMaterial;
            visual.transform.localScale = source.GetLayerVisualSize(groupSize);
            return visual;
        }

private static IEnumerator AnimateRotation(
            Transform target,
            Quaternion from,
            Quaternion to,
            float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
                target.rotation = Quaternion.Slerp(from, to, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }

            target.rotation = to;
        }

        private IEnumerator AnimateSauce(
            Transform sauce,
            Vector3 from,
            Vector3 to,
            float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
                Vector3 position = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t));
                position += Vector3.up * (Mathf.Sin(t * Mathf.PI) * _pourArcHeight);
                sauce.position = position;
                yield return null;
            }

            sauce.position = to;
        }

        private IEnumerator AnimateSettle(
            Transform target,
            Vector3 originalScale,
            float duration)
        {
            Vector3 raisedScale = originalScale * _settleScale;
            yield return AnimateScale(target, originalScale, raisedScale, duration * 0.5f);
            yield return AnimateScale(target, raisedScale, originalScale, duration * 0.5f);
        }

        private static IEnumerator AnimateScale(
            Transform target,
            Vector3 from,
            Vector3 to,
            float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
                target.localScale = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }

            target.localScale = to;
        }
    

private Vector3 CalculateTemporaryPourPosition(
            BottleController source,
            BottleController destination,
            Vector3 sourceStartPosition)
        {
            float direction = Mathf.Sign(
                destination.transform.position.x - sourceStartPosition.x);
            if (direction == 0f)
            {
                direction = 1f;
            }

            Collider sourceCollider = source.GetComponent<Collider>();
            Collider destinationCollider = destination.GetComponent<Collider>();
            float sourceHalfWidth = sourceCollider == null
                ? 0.5f
                : CalculateTiltedHalfWidth(sourceCollider.bounds.extents);
            float destinationHalfWidth = destinationCollider == null
                ? 0.5f
                : destinationCollider.bounds.extents.x;
            float centerSpacing = sourceHalfWidth + destinationHalfWidth + _destinationClearance;

            Vector3 pourPosition = destination.transform.position;
            pourPosition.x -= direction * centerSpacing;
            pourPosition.y = sourceStartPosition.y;
            pourPosition.z = sourceStartPosition.z;
            return pourPosition;
        }

        private static IEnumerator AnimatePosition(
            Transform target,
            Vector3 from,
            Vector3 to,
            float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
                target.position = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }

            target.position = to;
        }


private float CalculateTiltedHalfWidth(Vector3 sourceHalfExtents)
        {
            float tiltRadians = _tiltDegrees * Mathf.Deg2Rad;
            return Mathf.Abs(sourceHalfExtents.x * Mathf.Cos(tiltRadians))
                + Mathf.Abs(sourceHalfExtents.y * Mathf.Sin(tiltRadians));
        }


public bool PlayInvalidDestination(BottleController destination, Action onComplete)
        {
            if (_isPlaying || destination == null)
            {
                return false;
            }

            _isPlaying = true;
            StartCoroutine(ShakeInvalidDestination(destination, onComplete));
            return true;
        }

        private IEnumerator ShakeInvalidDestination(
            BottleController destination,
            Action onComplete)
        {
            Vector3 startPosition = destination.transform.position;

            try
            {
                float elapsed = 0f;
                while (elapsed < _invalidShakeDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = _invalidShakeDuration <= 0f
                        ? 1f
                        : Mathf.Clamp01(elapsed / _invalidShakeDuration);
                    float envelope = 1f - t;
                    float offset = Mathf.Sin(t * Mathf.PI * 6f)
                        * _invalidShakeDistance * envelope;
                    destination.transform.position = startPosition + Vector3.right * offset;
                    yield return null;
                }
            }
            finally
            {
                destination.transform.position = startPosition;
                _isPlaying = false;
                if (onComplete != null)
                {
                    onComplete();
                }
            }
        }
}
}