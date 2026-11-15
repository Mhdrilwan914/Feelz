using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ButtonTag : MonoBehaviour { public Action onClick; public SpriteRenderer spriteRenderer; }
// Attach to an empty GameObject in an empty scene (the scene needs only the Main Camera). Builds everything from Resources.
public class GameController : MonoBehaviour
{
    const float BoardTop = 5.3f, BoardBot = -5.6f;
    BoardModel _boardModel; List<BottleView> _bottleViews = new List<BottleView>(); OrderPanelView _orderPanel; Camera _camera; TextMesh _titleText, _messageText; ButtonTag _undoButton, _perfectUndoButton, _extraBottleButton, _hintButton;
    int _currentLevelNumber, _selectedBottleIndex = -1; bool _isAnimatingMove, _isDeliveringOrder, _isGameOver, _didLevelFail; float _worldWidth;
    void Awake() { if (FindObjectOfType<AudioManager>() == null) new GameObject("Audio").AddComponent<AudioManager>(); }
    void Start()
    {
        _camera = Camera.main; if (_camera == null) _camera = new GameObject("Cam").AddComponent<Camera>(); _camera.orthographic = true; _camera.orthographicSize = 8; _camera.backgroundColor = new Color(.08f, .23f, .3f); _camera.clearFlags = CameraClearFlags.SolidColor; _camera.transform.position = new Vector3(0, 0, -10);
        _worldWidth = 16f * _camera.aspect; BuildScene(); Load(PlayerPrefs.GetInt("ph", 1));
    }
    SpriteRenderer Spr(string n, Vector3 p, int ord) { var g = new GameObject(n); g.transform.position = p; var r = g.AddComponent<SpriteRenderer>(); r.sprite = GameConfig.Sprite(n); r.sortingOrder = ord; return r; }
    ButtonTag Btn(string n, Vector3 p, float s, Action a) { var r = Spr(n, p, 10); r.transform.localScale = Vector3.one * s; var c = r.gameObject.AddComponent<BoxCollider2D>(); c.size = new Vector2(1.2f, 1.2f); var t = r.gameObject.AddComponent<ButtonTag>(); t.spriteRenderer = r; t.onClick = () => { AudioManager.Play("button_tap"); a(); }; return t; }
    void BuildScene()
    {
        var bg = Spr("background", Vector3.zero, -100); bg.transform.localScale = Vector3.one * Mathf.Max(_worldWidth / 7.2f, 16f / 12.8f);
        var pn = Spr("glass_panel", new Vector3(0, (BoardTop + BoardBot) / 2, 0), -50); pn.drawMode = SpriteDrawMode.Sliced; pn.size = new Vector2(_worldWidth - .4f, BoardTop - BoardBot);
        _orderPanel = new GameObject("Orders").AddComponent<OrderPanelView>(); _orderPanel.Build(_worldWidth - .4f, 6.5f);
        _titleText = Effects.Text("", new Vector3(0, 7.55f, 0), .12f, Color.white); _messageText = Effects.Text("", new Vector3(0, 0, 0), .2f, new Color(1, .89f, .47f)); _messageText.GetComponent<MeshRenderer>().sortingOrder = 400;
        float x0 = -1.5f * 1.5f, y = -6.9f;
        _undoButton = Btn("btn_undo", new Vector3(x0, y), .8f, DoUndo); _perfectUndoButton = Btn("btn_undo_perfect", new Vector3(x0 + 1.5f, y), .8f, DoUndoPerfect);
        _extraBottleButton = Btn("btn_extra_bottle", new Vector3(x0 + 3f, y), .8f, DoExtra); _hintButton = Btn("btn_hint", new Vector3(x0 + 4.5f, y), .8f, DoHint);
        Btn("btn_settings", new Vector3(_worldWidth / 2 - .7f, 7.55f), .45f, () => { var audioManager = AudioManager.Instance; bool audioEnabled = !(audioManager.SoundOn || audioManager.MusicOn); audioManager.SoundOn = audioEnabled; audioManager.MusicOn = audioEnabled; });
    }
    void Load(int n)
    {
        StopAllCoroutines(); foreach (var v in _bottleViews) Destroy(v.gameObject); _bottleViews.Clear(); _currentLevelNumber = n; _boardModel = new BoardModel(LevelGenerator.Make(n)); _selectedBottleIndex = -1; _isAnimatingMove = _isDeliveringOrder = _isGameOver = _didLevelFail = false; _messageText.text = "";
        var tips = new Dictionary<int, string> { { 1, "Tap a bottle, then tap where to pour" }, { 3, "Fill an order's colour to serve it" }, { 5, "Mystery bottles hide their colours" }, { 8, "Moves are limited now" } };
        if (tips.TryGetValue(n, out var tip)) StartCoroutine(Tip(tip)); Sync();
    }
    IEnumerator Tip(string s) { _messageText.text = s; _messageText.characterSize = .1f; yield return new WaitForSeconds(2.2f); if (!_isGameOver) _messageText.text = ""; _messageText.characterSize = .2f; }
    void Sync()
    {
        while (_bottleViews.Count < _boardModel.bottles.Count) _bottleViews.Add(BottleView.Create(_bottleViews.Count, transform));
        for (int i = 0; i < _bottleViews.Count; i++) { _bottleViews[i].SetState(_boardModel.bottles[i]); if (!_bottleViews[i].isFlying) _bottleViews[i].gameObject.SetActive(!_boardModel.bottles[i].isDelivered); }
        Layout(); _orderPanel.Refresh(_boardModel); Hud();
    }
    void Hud()
    {
        int done = _boardModel.totalOrders - _boardModel.active.Count - _boardModel.queue.Count; _titleText.text = "Level " + _currentLevelNumber + "   Orders " + done + "/" + _boardModel.totalOrders + (_boardModel.moveLimit > 0 ? "   Moves " + Mathf.Max(0, _boardModel.MovesLeft) : "");
        bool free = !_isAnimatingMove && !_isDeliveringOrder; Dim(_undoButton, free && _boardModel.CanUndo); Dim(_perfectUndoButton, free && _boardModel.CanUndoPerfect); Dim(_extraBottleButton, !_boardModel.extraUsed && !_isAnimatingMove); Dim(_hintButton, !_isAnimatingMove);
    }
    void Dim(ButtonTag b, bool on) { b.spriteRenderer.color = new Color(1, 1, 1, on ? 1 : .35f); }
    void Layout()
    {
        int n = _bottleViews.Count, rows = n <= 5 ? 1 : n <= 12 ? 2 : 3, cols = Mathf.CeilToInt(n / (float)rows); float aw = _worldWidth - .8f, cw = aw / cols, ah = BoardTop - BoardBot - .4f, rh = ah / rows;
        float s = Mathf.Min(1.25f, cw * .78f, rh / 3.4f);
        for (int i = 0; i < n; i++)
        {
            int row = i / cols, cnt = Mathf.Min(cols, n - row * cols), j = i - row * cols; var v = _bottleViews[i];
            v.bottleScale = s; v.homePosition = new Vector2(-cnt * cw / 2 + (j + .5f) * cw, BoardTop - .2f - (row + .5f) * rh - BottleView.BottleHeight * s / 2);
            if (!v.isFlying && !(_isAnimatingMove && v.transform.rotation != Quaternion.identity)) { v.transform.position = v.homePosition; v.transform.localScale = Vector3.one * s; v.transform.rotation = Quaternion.identity; }
        }
    }
    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (_isGameOver) { Load(_didLevelFail ? _currentLevelNumber : Mathf.Min(_currentLevelNumber + 1, GameConfig.MaxLevel)); return; }
        var c = Physics2D.OverlapPoint(_camera.ScreenToWorldPoint(Input.mousePosition)); if (c == null) { if (_selectedBottleIndex >= 0) Deselect(); return; }
        var bt = c.GetComponent<ButtonTag>(); if (bt) { bt.onClick(); return; }
        var bv = c.GetComponent<BottleView>(); if (bv && !_isAnimatingMove) Tap(bv.bottleIndex);
    }
    void Deselect() { _bottleViews[_selectedBottleIndex].isSelected = false; _bottleViews[_selectedBottleIndex].Wobble(2); _selectedBottleIndex = -1; AudioManager.Play("deselect"); }
    void Tap(int i)
    {
        var b = _boardModel.bottles[i]; if (b.isDelivered || b.Sealed) { AudioManager.Play("invalid_move"); return; }
        if (_selectedBottleIndex < 0) { if (b.layers.Count == 0) { AudioManager.Play("invalid_move"); return; } _selectedBottleIndex = i; _bottleViews[i].isSelected = true; _bottleViews[i].Wobble(2.4f); AudioManager.Play("select"); }
        else if (_selectedBottleIndex == i) Deselect();
        else { int s = _selectedBottleIndex; _bottleViews[s].isSelected = false; _selectedBottleIndex = -1; if (_boardModel.PourCount(s, i) == 0) { AudioManager.Play("invalid_move"); return; } StartCoroutine(Pour(s, i)); }
    }
    IEnumerator Pour(int s, int d)
    {
        _isAnimatingMove = true; Hud(); var sv = _bottleViews[s]; var dv = _bottleViews[d]; int n = _boardModel.PourCount(s, d), color = _boardModel.bottles[s].Top; float dur = .9f + .18f * n, t = 0, sc = sv.bottleScale; int sg = dv.homePosition.x >= sv.homePosition.x ? 1 : -1; bool played = false;
        sv.SetOrder(50); var lr = new GameObject("stream").AddComponent<LineRenderer>(); lr.material = new Material(Shader.Find("Sprites/Default")); lr.startColor = lr.endColor = GameConfig.Colors[color]; lr.positionCount = 2; lr.sortingOrder = 60; lr.enabled = false;
        var lipL = new Vector2(sg * BottleView.BottleLipWidth * sc, BottleView.BottleHeight * sc);
        while (t < 1)
        {
            t += Time.deltaTime / dur; float e, pf = 0; if (t < .25f) e = t / .25f; else if (t < .75f) { e = 1; pf = (t - .25f) / .5f; } else e = Mathf.Max(0, 1 - (t - .75f) / .25f);
            e = Mathf.SmoothStep(0, 1, e); pf = Mathf.SmoothStep(0, 1, pf); if (t >= .75f) pf = 1; float p = pf * n;
            if (t >= .25f && !played) { played = true; AudioManager.Play("liquid_pour"); }
            var rot = Quaternion.Euler(0, 0, -sg * (1 + .18f * pf) * e * Mathf.Rad2Deg);
            Vector2 cur = Vector2.Lerp(sv.homePosition + lipL, dv.homePosition + new Vector2(-sg * BottleView.BottleLipWidth * sc * .25f, (BottleView.BottleHeight + .3f) * sc), e);
            sv.transform.rotation = rot; sv.transform.position = cur - (Vector2)(rot * lipL);
            sv.SetState(_boardModel.bottles[s], p); dv.SetState(_boardModel.bottles[d], 0, color, p);
            bool stream = t >= .27f && t < .78f; lr.enabled = stream;
            if (stream) { float surf = dv.homePosition.y + (.04f + (_boardModel.bottles[d].layers.Count + p) * BottleView.LayerHeight) * sc; lr.widthMultiplier = .06f * sc; lr.SetPosition(0, cur); lr.SetPosition(1, new Vector3(cur.x, Mathf.Min(cur.y, surf), 0)); dv.Wobble(2.6f); }
            yield return null;
        }
        Destroy(lr.gameObject); sv.transform.rotation = Quaternion.identity; sv.SetOrder(0);
        bool destinationBottleSealed = _boardModel.Apply(s, d, out _); dv.Wobble(3); _isAnimatingMove = false; Sync();
    if (destinationBottleSealed) { AudioManager.Play("bottle_complete_lid_close"); AudioManager.Play("confetti"); Effects.Confetti(dv.transform.position + Vector3.up* BottleView.BottleHeight * dv.bottleScale); StartCoroutine(Deliver());
}
    else AudioManager.Play("liquid_land");
if (_boardModel.MovesLeft <= 0) StartCoroutine(CheckFail());
  }
  IEnumerator Deliver()
{
    if (_isDeliveringOrder) yield break; _isDeliveringOrder = true; Hud(); yield return new WaitForSeconds(.5f); int i;
    while ((i = _boardModel.ReadyToDeliver()) >= 0)
    {
        var v = _bottleViews[i]; int slot = _boardModel.active.IndexOf(_boardModel.bottles[i].layers[0]); Vector3 a = v.transform.position, b = _orderPanel.SlotPos(slot) + Vector3.down * .3f;
        v.isFlying = true; v.SetOrder(60); AudioManager.Play("bottle_fly"); float t = 0, s0 = v.bottleScale;
        while (t < 1) { t += Time.deltaTime / .95f; float e = Mathf.SmoothStep(0, 1, Mathf.Min(1, t)); v.transform.position = Vector3.Lerp(a, b, e) + Vector3.up * Mathf.Sin(Mathf.Min(1, t) * Mathf.PI) * 1.2f; v.transform.localScale = Vector3.one * Mathf.Lerp(s0, s0 * .38f, e); v.transform.rotation = Quaternion.Euler(0, 0, 35 * e); yield return null; }
        _boardModel.Deliver(i); v.isFlying = false; v.SetOrder(0); v.gameObject.SetActive(false); Effects.Confetti(b); AudioManager.Play("order_delivery"); Sync();
    }
    _isDeliveringOrder = false; Hud();
    if (_boardModel.Won) { _isGameOver = true; _didLevelFail = false; AudioManager.Play("level_complete"); PlayerPrefs.SetInt("ph", Mathf.Max(PlayerPrefs.GetInt("ph", 1), Mathf.Min(_currentLevelNumber + 1, GameConfig.MaxLevel))); _messageText.text = "All orders served!\nTap to continue"; Effects.Confetti(new Vector3(-2, -2)); Effects.Confetti(new Vector3(2, -2)); }
}
IEnumerator CheckFail() { yield return new WaitForSeconds(1.3f); if (!_isGameOver && !_isAnimatingMove && !_isDeliveringOrder && _boardModel.MovesLeft <= 0 && !_boardModel.Won && _boardModel.ReadyToDeliver() < 0) { _isGameOver = _didLevelFail = true; _messageText.text = "Out of moves\nTap to retry"; } }
// ---- power-ups ----
void DoUndo() { if (_isAnimatingMove || _isDeliveringOrder || !_boardModel.CanUndo) return; ClearSel(); _boardModel.Undo(); AudioManager.Play("undo"); Sync(); }
void DoUndoPerfect() { if (_isAnimatingMove || _isDeliveringOrder || !_boardModel.CanUndoPerfect) return; ClearSel(); _boardModel.UndoPerfect(); AudioManager.Play("undo"); Sync(); }
void DoExtra() { if (_isAnimatingMove || !_boardModel.AddExtraBottle()) return; AudioManager.Play("power_up"); Sync(); }
void DoHint() { if (_isAnimatingMove || !_boardModel.FindHint(out int s, out int d)) { AudioManager.Play("invalid_move"); return; } _bottleViews[s].hintHighlightEndTime = _bottleViews[d].hintHighlightEndTime = Time.time + 2.2f; AudioManager.Play("hint_ping"); }
void ClearSel() { if (_selectedBottleIndex >= 0) { _bottleViews[_selectedBottleIndex].isSelected = false; _selectedBottleIndex = -1; } }
}
