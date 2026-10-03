using System;
using UnityEngine;
using SauceSort.Core;

namespace SauceSort.Gameplay
{
    /// <summary>
    /// Coordinates the two-click source/destination selection flow for bottles
    /// in one sorting area. Pour legality remains in BottleState.
    /// </summary>
    public sealed class BottleSelectionController : MonoBehaviour
    {
        private BottleController _selectedSource;
        [SerializeField] private BottlePourAnimator _pourAnimator;
        private bool _inputLocked;

        public event Action ValidPourPresentationCompleted;

        public bool IsInputLocked
        {
            get { return _inputLocked; }
        }

        private void Awake()
        {
            if (_pourAnimator == null)
            {
                _pourAnimator = GetComponent<BottlePourAnimator>();
            }
        }

public void HandleBottleSelection(BottleController clickedBottle)
        {
            if (_inputLocked || clickedBottle == null || clickedBottle.State == null)
            {
                return;
            }

            if (_selectedSource == null)
            {
                _selectedSource = clickedBottle;
                _selectedSource.SetSelected(true);
                return;
            }

            if (_selectedSource == clickedBottle)
            {
                ClearSelection();
                return;
            }

            BottleController source = _selectedSource;
            ClearSelection();

            if (!source.State.CanPourTo(clickedBottle.State))
            {
                _inputLocked = true;
                bool shakeStarted = _pourAnimator != null
                    && _pourAnimator.PlayInvalidDestination(
                        clickedBottle,
                        delegate { _inputLocked = false; });
                if (!shakeStarted)
                {
                    _inputLocked = false;
                }

                return;
            }

            SauceType sauce = source.State.GetTopSauce();
            int groupSize = source.State.CountTopGroup();
            bool moveApplied = false;
            _inputLocked = true;

            bool pourStarted = _pourAnimator != null && _pourAnimator.PlayPour(
                source,
                clickedBottle,
                sauce,
                groupSize,
                delegate
                {
                    moveApplied = ApplyPour(source, clickedBottle);
                    return moveApplied;
                },
                delegate
                {
                    _inputLocked = false;
                    if (moveApplied && ValidPourPresentationCompleted != null)
                    {
                        ValidPourPresentationCompleted();
                    }
                });

            if (!pourStarted)
            {
                _inputLocked = false;
                Debug.LogError("A valid pour could not start because BottlePourAnimator is missing or busy.", this);
            }
        }

        private void ClearSelection()
        {
            if (_selectedSource != null)
            {
                _selectedSource.SetSelected(false);
                _selectedSource = null;
            }
        }
    

private static bool ApplyPour(BottleController source, BottleController destination)
        {
            int movedLayers;
            if (!source.TryPourTo(destination, out movedLayers))
            {
                return false;
            }

            source.RefreshVisuals();
            destination.RefreshVisuals();
            return true;
        }


public void CancelSelection()
        {
            if (!_inputLocked)
            {
                ClearSelection();
            }
        }
}
}
