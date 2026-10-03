using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SauceSort.Core;

namespace Assets.SauceSort.Scripts.Core
{
    /// <summary>
    /// Pure logical bottle state. Layers are ordered from bottom to top, so the
    /// last stored layer is the top sauce.
    /// </summary>
    public sealed class BottleState
    {
        public const int Capacity = 4;

        private readonly List<SauceType> layers = new List<SauceType>(Capacity);
        private readonly ReadOnlyCollection<SauceType> readOnlyLayers;

        public BottleState()
        {
            readOnlyLayers = layers.AsReadOnly();
        }

        /// <summary>
        /// Creates a bottle from bottom-to-top layers. None is not a layer and
        /// invalid or over-capacity contents are rejected.
        /// </summary>
        public BottleState(IEnumerable<SauceType> initialLayers) : this()
        {
            if (initialLayers == null)
            {
                throw new ArgumentNullException(nameof(initialLayers));
            }

            foreach (SauceType sauce in initialLayers)
            {
                if (!IsStorableSauce(sauce))
                {
                    throw new ArgumentException(
                        "Bottle layers must be a real sauce type; SauceType.None cannot be stored.",
                        nameof(initialLayers));
                }

                if (layers.Count >= Capacity)
                {
                    throw new ArgumentException(
                        "A bottle cannot contain more than " + Capacity + " layers.",
                        nameof(initialLayers));
                }

                layers.Add(sauce);
            }
        }

        /// <summary>Read-only layers listed from bottom to top.</summary>
        public IReadOnlyList<SauceType> Layers
        {
            get { return readOnlyLayers; }
        }

        public bool IsEmpty
        {
            get { return layers.Count == 0; }
        }

        public bool IsFull
        {
            get { return layers.Count == Capacity; }
        }

        /// <summary>Returns None when the bottle is empty.</summary>
        public SauceType GetTopSauce()
        {
            return IsEmpty ? SauceType.None : layers[layers.Count - 1];
        }

        /// <summary>
        /// Counts the contiguous same-sauce layers at the top, or zero when empty.
        /// </summary>
        public int CountTopGroup()
        {
            if (IsEmpty)
            {
                return 0;
            }

            SauceType topSauce = GetTopSauce();
            int count = 0;

            for (int i = layers.Count - 1; i >= 0 && layers[i] == topSauce; i--)
            {
                count++;
            }

            return count;
        }

        /// <summary>
        /// Determines whether this bottle can receive the complete top group
        /// from source under standard Water Sort rules.
        /// </summary>
        public bool CanReceiveTopGroupFrom(BottleState source)
        {
            if (source == null || ReferenceEquals(source, this) || source.IsEmpty || IsFull)
            {
                return false;
            }

            SauceType incomingSauce = source.GetTopSauce();
            if (!IsEmpty && GetTopSauce() != incomingSauce)
            {
                return false;
            }

            int incomingGroupSize = source.CountTopGroup();
            int availableSpace = Capacity - layers.Count;

            // A pour moves the entire contiguous group; it never splits it.
            return incomingGroupSize <= availableSpace;
        }

        /// <summary>Returns true when this bottle can pour its top group to destination.</summary>
        public bool CanPourTo(BottleState destination)
        {
            return destination != null && destination.CanReceiveTopGroupFrom(this);
        }

        /// <summary>
        /// Moves the whole contiguous top group, or leaves both bottles unchanged
        /// when the pour is invalid.
        /// </summary>
        public bool TryPourTo(BottleState destination, out int pouredLayers)
        {
            pouredLayers = 0;
            if (!CanPourTo(destination))
            {
                return false;
            }

            pouredLayers = CountTopGroup();
            int firstMovedLayer = layers.Count - pouredLayers;

            for (int i = firstMovedLayer; i < layers.Count; i++)
            {
                destination.layers.Add(layers[i]);
            }

            layers.RemoveRange(firstMovedLayer, pouredLayers);
            return true;
        }

        private static bool IsStorableSauce(SauceType sauce)
        {
            return sauce == SauceType.Red
                || sauce == SauceType.Blue
                || sauce == SauceType.Green
                || sauce == SauceType.Yellow;
        }
    }
}
