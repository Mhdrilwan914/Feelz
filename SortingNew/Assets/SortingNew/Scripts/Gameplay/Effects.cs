using UnityEngine;
public static class Effects
{
    static Font font; public static Font F => font ? font : (font = Font.CreateDynamicFontFromOSFont("Arial", 16));
    static Material Mat(string tex) { var m = new Material(Shader.Find("Sprites/Default")); m.mainTexture = Resources.Load<Texture2D>("Sprites/" + tex); return m; }
    public static TextMesh Text(string s, Vector3 p, float size, Color c, TextAnchor a = TextAnchor.MiddleCenter, Transform parent = null)
    {
        var go = new GameObject("text"); if (parent) go.transform.SetParent(parent, false); go.transform.localPosition = p; var t = go.AddComponent<TextMesh>();
        t.font = F; t.text = s; t.characterSize = size; t.fontSize = 64; t.anchor = a; t.color = c; var r = go.GetComponent<MeshRenderer>(); r.sharedMaterial = F.material; r.sortingOrder = 300; return t;
    }
    // Sparkles live inside the liquid only (masked by the bottle's SpriteMask, shape resized to the fill level).
    public static ParticleSystem Sparkles(Transform parent)
    {
        var go = new GameObject("sparkles"); go.transform.SetParent(parent, false); var ps = go.AddComponent<ParticleSystem>(); ps.Stop();
        var m = ps.main; m.loop = true; m.startLifetime = .8f; m.startSpeed = 0; m.startSize = new ParticleSystem.MinMaxCurve(.05f, .1f); m.startColor = Color.white; m.simulationSpace = ParticleSystemSimulationSpace.Local; m.maxParticles = 12;
        var e = ps.emission; e.rateOverTime = 5; var sh = ps.shape; sh.shapeType = ParticleSystemShapeType.Box; sh.scale = new Vector3(.55f, .1f, .01f);
        var sz = ps.sizeOverLifetime; sz.enabled = true; sz.size = new ParticleSystem.MinMaxCurve(1, new AnimationCurve(new Keyframe(0, 0), new Keyframe(.5f, 1), new Keyframe(1, 0)));
        var r = go.GetComponent<ParticleSystemRenderer>(); r.material = Mat("sparkle_star"); r.maskInteraction = SpriteMaskInteraction.VisibleInsideMask; r.sortingOrder = 2; ps.Play(); return ps;
    }
    public static void Confetti(Vector3 pos)
    {
        var go = new GameObject("confetti"); go.transform.position = pos; go.transform.rotation = Quaternion.Euler(-90, 0, 0); var ps = go.AddComponent<ParticleSystem>(); ps.Stop();
        var m = ps.main; m.duration = 1; m.loop = false; m.startLifetime = 1.4f; m.startSpeed = new ParticleSystem.MinMaxCurve(4, 9); m.startSize = new ParticleSystem.MinMaxCurve(.1f, .2f); m.gravityModifier = 1.2f; m.startRotation = new ParticleSystem.MinMaxCurve(0, 6.28f);
        var g = new Gradient(); var cs = GameConfig.Colors; var keys = new GradientColorKey[8]; for (int i = 0; i < 8; i++) keys[i] = new GradientColorKey(cs[i], i / 7f);
        g.mode = GradientMode.Fixed; g.SetKeys(keys, new[] { new GradientAlphaKey(1, 0) }); m.startColor = new ParticleSystem.MinMaxGradient(g) { mode = ParticleSystemGradientMode.RandomColor };
        var e = ps.emission; e.rateOverTime = 0; e.SetBursts(new[] { new ParticleSystem.Burst(0, 44) });
        var sh = ps.shape; sh.shapeType = ParticleSystemShapeType.Cone; sh.angle = 35; sh.radius = .1f;
        var rot = ps.rotationOverLifetime; rot.enabled = true; rot.z = new ParticleSystem.MinMaxCurve(-5, 5);
        var col = ps.colorOverLifetime; col.enabled = true; var fade = new Gradient(); fade.SetKeys(new[] { new GradientColorKey(Color.white, 0) }, new[] { new GradientAlphaKey(1, .7f), new GradientAlphaKey(0, 1) }); col.color = fade;
        var r = go.GetComponent<ParticleSystemRenderer>(); r.material = Mat("confetti_piece"); r.sortingOrder = 100; ps.Play(); Object.Destroy(go, 3f);
    }
}
