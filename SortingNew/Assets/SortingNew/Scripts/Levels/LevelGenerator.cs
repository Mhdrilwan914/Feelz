using System.Collections.Generic;
using UnityEngine;
public class Rng
{ // mulberry32 – same seeds give same levels
    int s; public Rng(int seed) { s = seed; }
    public double Next() { unchecked { s += 0x6D2B79F5; uint z = (uint)s; uint t = (z ^ (z >> 15)) * (1u | z); t = (t + ((t ^ (t >> 7)) * (61u | t))) ^ t; return (t ^ (t >> 14)) / 4294967296.0; } }
}
public class LevelData { public List<int[]> bottles = new List<int[]>(); public int[] hidden; public List<int> orders = new List<int>(); public int moveLimit; public int level; }
public static class LevelGenerator
{
    public static LevelData Make(int n)
    {
        int k = Mathf.Min(3 + (n - 1) / 2, 10), cap = GameConfig.Capacity; var r = new Rng(n * 7919 + 3); int[] u; bool ok;
        do
        {
            u = new int[k * cap]; for (int c = 0; c < k; c++) for (int i = 0; i < cap; i++) u[c * cap + i] = c;
            for (int i = u.Length - 1; i > 0; i--) { int j = (int)(r.Next() * (i + 1)); (u[i], u[j]) = (u[j], u[i]); }
            ok = true; for (int b = 0; b < k; b++) { bool same = true; for (int i = 1; i < cap; i++) if (u[b * cap + i] != u[b * cap]) same = false; if (same) ok = false; }
        } while (!ok);
        var d = new LevelData { level = n, hidden = new int[k + 2] };
        for (int b = 0; b < k; b++) { var a = new int[cap]; System.Array.Copy(u, b * cap, a, 0, cap); d.bottles.Add(a); }
        d.bottles.Add(new int[0]); d.bottles.Add(new int[0]);
        int mm = n >= 5 ? Mathf.Min((n - 3) / 2, k - 1) : 0; for (int i = 0; i < mm; i++) d.hidden[(int)(r.Next() * k)] = 3; // mystery bottles
        var cs = new List<int>(); for (int c = 0; c < k; c++) cs.Add(c);
        for (int i = k - 1; i > 0; i--) { int j = (int)(r.Next() * (i + 1)); (cs[i], cs[j]) = (cs[j], cs[i]); }
        d.orders = cs; d.moveLimit = n >= 8 ? k * 14 : 0; return d;
    }
}
