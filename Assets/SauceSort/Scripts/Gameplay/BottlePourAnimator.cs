using System;
using Assets.SauceSort.Scripts.Core;
using DG.Tweening;
using SauceSort.Core;
using UnityEngine;

namespace SauceSort.Gameplay
{
    /// <summary>
    /// Handles only the visual presentation of a pour. BottleState validation and
    /// mutation are supplied by the caller through callbacks.
    /// </summary>
    /* public sealed class BottlePourAnimator : MonoBehaviour
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
 */

    public sealed class BottlePourAnimator : MonoBehaviour
    {
        [Header("Pour")]
        [SerializeField] private float _approachDuration = 0.16f;
        [SerializeField] private float _tiltDuration = 0.18f;
        [SerializeField] private float _returnDuration = 0.16f;
        [SerializeField] private float _tiltDegrees = 24f;
        [SerializeField] private float _destinationClearance = 0.1f;
        [SerializeField] private float _pourDuration = 0.32f;
        [SerializeField] private float _streamCurveHeight = 0.08f;
        [SerializeField] private float _streamWidth = 0.1f;
        [SerializeField] private float _streamEndWidth = 0.07f;

        [Header("Feedback")]
        [SerializeField] private float _settleScale = 1.04f;
        [SerializeField] private float _settleDuration = 0.12f;
        [SerializeField] private float _shakeDuration = 0.2f;
        [SerializeField] private float _shakeDistance = 0.08f;

        private DG.Tweening.Sequence _sequence;
        private GameObject _pourVisual;
        private LineRenderer _pourStream;
        private bool _isPlaying;
        private const int PourStreamPointCount = 5;

        public bool IsPlaying => _isPlaying;

        /// <summary>Cancels the current pour or invalid-move feedback and restores bottle visuals.</summary>
        public void CancelActiveAnimation()
        {
            if (_sequence != null && _sequence.IsActive())
                _sequence.Kill(false);
        }

        public bool PlayPour(
            BottleController source,
            BottleController destination,
            SauceType sauce,
            int pouredLayers,
            Func<bool> onSauceArrived,
            Action onComplete)
        {
            if (_isPlaying || source == null || destination == null ||
                pouredLayers <= 0 || onSauceArrived == null)
                return false;

            _isPlaying = true;

            Transform sourceTransform = source.transform;
            Transform destinationTransform = destination.transform;

            Vector3 sourceStart = sourceTransform.position;
            Quaternion sourceRotation = sourceTransform.rotation;
            Vector3 destinationScale = destinationTransform.localScale;

            // The model may split a same-sauce group when the destination has
            // less room than the source group. Match the animated block to the
            // number of layers TryPourTo will actually transfer.
            int availableDestinationSpace = BottleState.Capacity - destination.State.Layers.Count;
            int animatedLayerCount = Mathf.Min(pouredLayers, availableDestinationSpace);
            if (animatedLayerCount <= 0)
            {
                _isPlaying = false;
                return false;
            }

            float direction = Mathf.Sign(destinationTransform.position.x - sourceStart.x);
            if (Mathf.Approximately(direction, 0f))
                direction = 1f;

            Vector3 approachPosition = GetApproachPosition(
                source, destination, sourceStart, direction);

            Quaternion tiltedRotation = Quaternion.AngleAxis(
                -direction * _tiltDegrees, Vector3.forward) * sourceRotation;

            BoxCollider sourceCollider = source.GetComponent<BoxCollider>();
            Vector3 sourceMouthLocalPosition = sourceCollider != null
                ? sourceCollider.center + Vector3.up * sourceCollider.size.y * 0.5f
                : Vector3.up * source.GetLayerVisualSize(BottleState.Capacity).y * 0.5f;
            Vector3 sauceStart = sourceTransform.TransformPoint(sourceMouthLocalPosition);

            float layerHeight = destination.GetLayerVisualSize(1).y * destinationTransform.lossyScale.y;
            Vector3 destinationSurfaceStart = destination.GetLayerWorldPosition(
                destination.State.Layers.Count) - destinationTransform.up * (layerHeight * 0.5f);

            _pourVisual = CreatePourVisual(source, sauce, animatedLayerCount);
            if (_pourVisual != null)
            {
                _pourVisual.transform.position = sauceStart;
                _pourStream.SetPosition(0, sauceStart);
                _pourStream.SetPosition(1, sauceStart);
            }

            _sequence = DOTween.Sequence();
            _sequence.Append(sourceTransform.DOMove(approachPosition, _approachDuration)
                .SetEase(Ease.InOutSine));
            _sequence.Append(sourceTransform.DORotateQuaternion(tiltedRotation, _tiltDuration)
                .SetEase(Ease.InOutSine)
                .OnUpdate(() =>
                {
                    if (source != null)
                    {
                        float rotatedDegrees = Mathf.Abs(Mathf.DeltaAngle(
                            sourceRotation.eulerAngles.z, sourceTransform.eulerAngles.z));
                        source.SetLiquidMotion(Mathf.Clamp01(rotatedDegrees / _tiltDegrees));
                    }
                }));

            if (_pourVisual != null)
            {
                Tween pourTween = DOVirtual.Float(0f, 1f, _pourDuration, progress =>
                {
                    if (source == null || destination == null || _pourVisual == null)
                        return;

                    if (!_pourVisual.activeSelf)
                        _pourVisual.SetActive(true);

                    Vector3 streamStart = sourceTransform.TransformPoint(sourceMouthLocalPosition);
                    Vector3 streamEnd = destinationSurfaceStart
                                        + destinationTransform.up * (layerHeight * progress * animatedLayerCount);
                    Vector3 control = (streamStart + streamEnd) * 0.5f
                                      + Vector3.up * _streamCurveHeight;
                    source.SetPourVisualProgress(animatedLayerCount, progress);
                    destination.SetIncomingPourVisualProgress(sauce, animatedLayerCount, progress);
                    if (_pourStream != null)
                    {
                        for (int pointIndex = 0; pointIndex < PourStreamPointCount; pointIndex++)
                        {
                            float pointProgress = pointIndex / (float)(PourStreamPointCount - 1);
                            _pourStream.SetPosition(pointIndex,
                                GetBezierPoint(streamStart, control, streamEnd, pointProgress));
                        }
                    }
                }).SetEase(Ease.InOutSine);
                pourTween.OnStart(() =>
                {
                    if (_pourVisual != null)
                        _pourVisual.SetActive(true);
                });
                _sequence.Append(pourTween);
            }
            else
            {
                _sequence.AppendInterval(_pourDuration);
            }

            // Apply the model change when the visual reaches the destination.
            _sequence.AppendCallback(() =>
            {
                bool moved = onSauceArrived();
                HidePourVisual();

                if (moved && destination != null)
                {
                    destinationTransform
                        .DOScale(destinationScale * _settleScale, _settleDuration * 0.5f)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            if (destination != null)
                            {
                                destinationTransform
                                    .DOScale(destinationScale, _settleDuration * 0.5f)
                                    .SetEase(Ease.InOutSine);
                            }
                        });
                }
            });

            _sequence.Append(sourceTransform.DORotateQuaternion(sourceRotation, _tiltDuration)
                .SetEase(Ease.InOutSine)
                .OnUpdate(() =>
                {
                    if (source != null)
                    {
                        float rotatedDegrees = Mathf.Abs(Mathf.DeltaAngle(
                            sourceRotation.eulerAngles.z, sourceTransform.eulerAngles.z));
                        source.SetLiquidMotion(Mathf.Clamp01(rotatedDegrees / _tiltDegrees));
                    }
                }));
            _sequence.Append(sourceTransform.DOMove(sourceStart, _returnDuration)
                .SetEase(Ease.InOutSine));

            _sequence.OnComplete(() => Finish(
                source, destination, sourceStart, sourceRotation, destinationScale, onComplete));
            _sequence.OnKill(() => Finish(
                source, destination, sourceStart, sourceRotation, destinationScale, onComplete));

            return true;
        }

        public bool PlayInvalidDestination(BottleController destination, Action onComplete)
        {
            if (_isPlaying || destination == null)
                return false;

            _isPlaying = true;
            Vector3 startPosition = destination.transform.position;

            _sequence = DOTween.Sequence();
            _sequence.Append(destination.transform.DOShakePosition(
                _shakeDuration,
                new Vector3(_shakeDistance, 0f, 0f),
                18, 0f, false, true));

            _sequence.OnComplete(() =>
            {
                destination.transform.position = startPosition;
                Finish(onComplete);
            });
            _sequence.OnKill(() =>
            {
                if (destination != null)
                    destination.transform.position = startPosition;
                Finish(onComplete);
            });
            return true;
        }

        private Vector3 GetApproachPosition(
            BottleController source,
            BottleController destination,
            Vector3 sourceStart,
            float direction)
        {
            Collider sourceCollider = source.GetComponent<Collider>();
            Collider destinationCollider = destination.GetComponent<Collider>();

            float sourceWidth = sourceCollider != null ? sourceCollider.bounds.extents.x : 0.5f;
            float destinationWidth = destinationCollider != null
                ? destinationCollider.bounds.extents.x
                : 0.5f;

            Vector3 result = destination.transform.position;
            result.x -= direction * (sourceWidth + destinationWidth + _destinationClearance);
            result.y = sourceStart.y;
            result.z = sourceStart.z;
            return result;
        }

        private GameObject CreatePourVisual(
            BottleController source,
            SauceType sauce,
            int pouredLayers)
        {
            Material material = source.GetSauceMaterialForPresentation(sauce);
            if (material == null)
            {
                Debug.LogError("Missing sauce material for " + sauce, source);
                return null;
            }

            GameObject visual = _pourVisual;
            if (visual == null)
            {
                visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                visual.name = "PouringSauce";
                Destroy(visual.GetComponent<Collider>());
                _pourStream = visual.AddComponent<LineRenderer>();
                _pourStream.positionCount = PourStreamPointCount;
                _pourStream.useWorldSpace = true;
                _pourStream.alignment = LineAlignment.View;
                _pourStream.textureMode = LineTextureMode.Stretch;
                _pourStream.numCapVertices = 4;
                _pourStream.sortingOrder = 20;
            }

            visual.SetActive(true);
            Renderer renderer = visual.GetComponent<Renderer>();
            renderer.enabled = false;
            source.ApplySauceAppearance(renderer, sauce);
            source.ApplySauceAppearance(_pourStream, sauce);
            _pourStream.enabled = true;
            _pourStream.startWidth = _streamWidth;
            _pourStream.endWidth = _streamEndWidth;
            _pourStream.startColor = Color.white;
            _pourStream.endColor = Color.white;

            Vector3 layerSize = source.GetLayerVisualSize(pouredLayers);
            visual.transform.localScale = new Vector3(
                layerSize.x * 0.34f,
                layerSize.y * 0.42f,
                Mathf.Max(0.08f, layerSize.z * 0.8f));
            visual.SetActive(false);
            return visual;
        }

        private static Vector3 GetBezierPoint(Vector3 start, Vector3 control, Vector3 end, float progress)
        {
            float inverse = 1f - progress;
            return inverse * inverse * start
                   + 2f * inverse * progress * control
                   + progress * progress * end;
        }

        private void HidePourVisual()
        {
            if (_pourVisual == null)
                return;

            Renderer renderer = _pourVisual.GetComponent<Renderer>();
            if (renderer != null)
                renderer.enabled = false;
            if (_pourStream != null)
                _pourStream.enabled = false;
            _pourVisual.SetActive(false);
        }

        private void Finish(
            BottleController source,
            BottleController destination,
            Vector3 sourcePosition,
            Quaternion sourceRotation,
            Vector3 destinationScale,
            Action onComplete)
        {
            if (!_isPlaying)
                return;

            if (source != null)
                source.transform.SetPositionAndRotation(sourcePosition, sourceRotation);

            if (destination != null)
                destination.transform.localScale = destinationScale;

            if (source != null)
                source.RefreshVisuals();
            if (destination != null)
                destination.RefreshVisuals();
            HidePourVisual();
            Finish(onComplete);
        }

        private void Finish(Action onComplete)
        {
            if (!_isPlaying)
                return;

            _isPlaying = false;
            _sequence = null;
            onComplete?.Invoke();
        }

        private void OnDestroy()
        {
            if (_sequence != null && _sequence.IsActive())
                _sequence.Kill(false);

            if (_pourVisual != null)
                Destroy(_pourVisual);
        }

        private void OnDisable()
        {
            CancelActiveAnimation();
        }
    }
}
