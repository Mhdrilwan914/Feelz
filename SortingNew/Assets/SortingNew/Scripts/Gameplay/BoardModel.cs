using System.Collections.Generic;
using System.Linq;
public class Bottle
{
    public List<int> layers = new List<int>();
    public int hiddenLayerCount;
    public bool isDelivered;
    public bool Sealed => layers.Count == GameConfig.Capacity
        && layers.All(color => color == layers[0]);
    public int Top => layers[layers.Count - 1];
    public bool IsHidden(int i) => i < hiddenLayerCount && i < layers.Count - 1;
}
public class Snap
{
    public List<int>[] bottleLayers;
    public bool[] deliveredStates;
    public int[] hiddenLayerCounts;
    public List<int> active;
    public List<int> queue;
}
public class BoardModel
{
    public List<Bottle> bottles = new List<Bottle>();
    public List<int> active = new List<int>();
    public List<int> queue = new List<int>();
    public List<Snap> history = new List<Snap>();
    public List<(Snap snap, int idx)> checkpoints = new List<(Snap, int)>();
    public int moveLimit;
    public int totalOrders;
    public bool extraUsed;
    public BoardModel(LevelData d)
    {
        for (int i = 0; i < d.bottles.Count; i++)
        {
            bottles.Add(new Bottle
            {
                layers = d.bottles[i].ToList(),
                hiddenLayerCount = d.hidden[i]
            });
        }

        active = d.orders.Take(3).ToList();
        queue = d.orders.Skip(3).ToList();
        moveLimit = d.moveLimit;
        totalOrders = d.orders.Count;
    }
    Snap Snapshot() => new Snap
    {
        bottleLayers = bottles.Select(bottle => bottle.layers.ToList()).ToArray(),
        deliveredStates = bottles.Select(bottle => bottle.isDelivered).ToArray(),
        hiddenLayerCounts = bottles.Select(bottle => bottle.hiddenLayerCount).ToArray(),
        active = active.ToList(),
        queue = queue.ToList()
    };
    void Restore(Snap s)
    {
        for (int i = 0; i < bottles.Count; i++)
        {
            bool hasSnapshot = i < s.bottleLayers.Length;
            bottles[i].layers = hasSnapshot
                ? s.bottleLayers[i].ToList()
                : new List<int>();
            bottles[i].isDelivered = hasSnapshot && s.deliveredStates[i];
            bottles[i].hiddenLayerCount = hasSnapshot ? s.hiddenLayerCounts[i] : 0;
        }

        active = s.active.ToList();
        queue = s.queue.ToList();
    }
    public int PourCount(int s, int d)
    {
        var a = bottles[s];
        var b = bottles[d];
        int cap = GameConfig.Capacity;
        if (s == d || a.isDelivered || b.isDelivered || a.Sealed || b.Sealed
            || a.layers.Count == 0 || b.layers.Count >= cap)
        {
            return 0;
        }

        if (b.layers.Count > 0 && b.Top != a.Top)
        {
            return 0;
        }

        int n = 0;
        for (int i = a.layers.Count - 1; i >= 0 && a.layers[i] == a.Top; i--)
        {
            n++;
        }

        return System.Math.Min(n, cap - b.layers.Count);
    }
    public bool Apply(int s, int d, out int moved)
    { // returns true when destination became sealed
        moved = PourCount(s, d);
        if (moved == 0) return false;

        var pre = Snapshot();
        for (int k = 0; k < moved; k++)
        {
            bottles[d].layers.Add(bottles[s].Top);
            bottles[s].layers.RemoveAt(bottles[s].layers.Count - 1);
        }

        history.Add(pre);
        if (bottles[d].Sealed)
        {
            checkpoints.Add((pre, history.Count - 1));
            return true;
        }
        return false;
    }
    public int MovesLeft => moveLimit == 0 ? int.MaxValue : moveLimit - history.Count;
    public bool AnySealed => bottles.Any(b => b.Sealed);
    public bool CanUndo => !AnySealed && history.Count > 0;
    public bool CanUndoPerfect => checkpoints.Count > 0;
    public void Undo()
    {
        if (!CanUndo) return;
        var snapshot = history[history.Count - 1];
        history.RemoveAt(history.Count - 1);
        Restore(snapshot);
    }

    public void UndoPerfect()
    {
        if (!CanUndoPerfect) return;
        var checkpoint = checkpoints[checkpoints.Count - 1];
        checkpoints.RemoveAt(checkpoints.Count - 1);
        history.RemoveRange(checkpoint.idx, history.Count - checkpoint.idx);
        Restore(checkpoint.snap);
    }

    public bool AddExtraBottle()
    {
        if (extraUsed) return false;
        extraUsed = true;
        bottles.Add(new Bottle());
        return true;
    }
    public bool FindHint(out int src, out int dst)
    {
        int best = -1;
        src = dst = -1;
        for (int i = 0; i < bottles.Count; i++)
        {
            for (int j = 0; j < bottles.Count; j++)
            {
                if (PourCount(i, j) == 0) continue;
                int sc = (bottles[j].layers.Count > 0 ? 2 : 0)
                    + (bottles[i].layers.All(c => c == bottles[i].layers[0]) ? 0 : 1);
                if (sc > best)
                {
                    best = sc;
                    src = i;
                    dst = j;
                }
            }
        }
        return best >= 0;
    }
    public int Progress(int color)
    {
        int m = 0;
        foreach (var b in bottles)
        {
            if (b.isDelivered) continue;
            int n = 0;
            for (int i = b.layers.Count - 1; i >= 0 && b.layers[i] == color; i--) n++;
            if (n > m) m = n;
        }
        return m;
    }

    public int ReadyToDeliver()
    {
        for (int i = 0; i < bottles.Count; i++)
        {
            if (!bottles[i].isDelivered
                && bottles[i].Sealed
                && active.Contains(bottles[i].layers[0]))
            {
                return i;
            }
        }
        return -1;
    }
    public int Deliver(int i)
    { // returns the slot index that was served; slot gets a new order or is removed
        var b = bottles[i];
        b.isDelivered = true;
        int slot = active.IndexOf(b.layers[0]);
        if (queue.Count > 0)
        {
            active[slot] = queue[0];
            queue.RemoveAt(0);
        }
        else
        {
            active.RemoveAt(slot);
        }
        return slot;
    }
    public bool Won => active.Count == 0 && queue.Count == 0;
}
