using System.Collections.Generic;
using UnityEngine;
// Glass order panel at the top: up to 3 simultaneous orders, each with jar progress + status.
public class OrderPanelView : MonoBehaviour
{
    class Slot { public GameObject go; public SpriteRenderer bg, fill; public TextMesh name, status; public int color = -1; public float pop = 1; }
    List<Slot> slots = new List<Slot>(); float sc, step;
    public void Build(float width, float y)
    {
        transform.position = new Vector3(0, y, 0);
        var p = new GameObject("panel").AddComponent<SpriteRenderer>(); p.transform.SetParent(transform, false); p.sprite = GameConfig.Sprite("order_panel"); p.drawMode = SpriteDrawMode.Sliced; p.size = new Vector2(width, 1f); p.sortingOrder = -40;
        step = (width - .5f) / 3f; sc = (step - .1f) / 2.2f;
        for (int i = 0; i < 3; i++)
        {
            var s = new Slot { go = new GameObject("slot" + i) }; s.go.transform.SetParent(transform, false); s.go.transform.localPosition = new Vector3((i - 1) * step, 0, 0); s.go.transform.localScale = Vector3.one * sc;
            s.bg = Add("order_slot", s.go.transform, 0, 0, -30); Add("juice_jar_icon", s.go.transform, -.78f, 0, -27);
            s.fill = Add("liquid_cherry", s.go.transform, -.78f, 0, -28);
            s.name = Effects.Text("", new Vector3(-.55f, .14f, 0), .09f, Color.white, TextAnchor.MiddleLeft, s.go.transform); s.status = Effects.Text("", new Vector3(-.55f, -.14f, 0), .065f, new Color(1, 1, 1, .8f), TextAnchor.MiddleLeft, s.go.transform);
            slots.Add(s);
        }
    }
    SpriteRenderer Add(string n, Transform p, float x, float y, int ord) { var g = new GameObject(n); g.transform.SetParent(p, false); g.transform.localPosition = new Vector3(x, y, 0); var r = g.AddComponent<SpriteRenderer>(); r.sprite = GameConfig.Sprite(n); r.sortingOrder = ord; return r; }
    public Vector3 SlotPos(int i) => slots[i].go.transform.position;
    public void Refresh(BoardModel m)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            var s = slots[i]; bool on = i < m.active.Count; s.go.SetActive(on); if (!on) { s.color = -1; continue; }
            int c = m.active[i]; if (c != s.color) { s.color = c; s.pop = 0; s.fill.sprite = GameConfig.Sprite("liquid_" + GameConfig.Names[c]); }
            int p = m.Progress(c); bool ready = p == GameConfig.Capacity; float h = p / 4f * .4f;
            s.fill.transform.localScale = new Vector3(.24f, Mathf.Max(.001f, h / .44f), 1); s.fill.transform.localPosition = new Vector3(-.78f, -.2f + h / 2, 0);
            s.bg.sprite = GameConfig.Sprite(ready ? "order_slot_ready" : "order_slot");
            string n = GameConfig.Names[c]; s.name.text = char.ToUpper(n[0]) + n.Substring(1); s.status.text = ready ? "Serving…" : p + "/4 ready";
        }
    }
    void Update() { foreach (var s in slots) if (s.pop < 1) { s.pop = Mathf.Min(1, s.pop + Time.deltaTime * 2.2f); float t = s.pop - 1, k = 1 + 2.7f * t * t * t + 1.7f * t * t; s.go.transform.localScale = Vector3.one * sc * k; } }
}
