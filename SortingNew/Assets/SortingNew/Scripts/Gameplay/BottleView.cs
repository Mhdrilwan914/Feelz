using System.Collections.Generic;
using UnityEngine;
// One bottle. Layers (bottom to top): glass back, liquid units (masked by bottle shape), glass front, lid. Pivot = bottom centre.
public class BottleView : MonoBehaviour
{
    public const float BottleHeight = 2.9f, LayerHeight = .435f, BottleLipWidth = .31f;
    public int bottleIndex;
    public Vector2 homePosition;
    public float bottleScale = 1;
    public bool isSelected;
    public bool isFlying;
    public float wobbleAmount;
    public float hintHighlightEndTime;

    private Transform visual;
    private Transform liquidRoot;
    private SpriteRenderer back;
    private SpriteRenderer front;
    private SpriteRenderer lid;
    private SpriteRenderer glow;
    private SpriteMask mask;
    private SpriteRenderer[] units = new SpriteRenderer[6];
    private ParticleSystem sparkle;
    private Bottle data;
    private float rem;
    private float extraAmt;
    private int extraColor = -1;
    private int order;
    private float sel;
    private float lidT;
    private Color[] cols;
    public static BottleView Create(int i, Transform parent)
    {
        var go = new GameObject("Bottle" + i);
        go.transform.SetParent(parent, false);
        var view = go.AddComponent<BottleView>();
        view.bottleIndex = i;
        view.Build();
        return view;
    }
    SpriteRenderer Spr(string n, Transform p, float y)
    {
        var gameObject = new GameObject(n);
        gameObject.transform.SetParent(p, false);
        gameObject.transform.localPosition = new Vector3(0, y, 0);
        var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = GameConfig.Sprite(n);
        return spriteRenderer;
    }
    void Build()
    {
        cols = GameConfig.Colors;
        var bottleCollider = gameObject.AddComponent<BoxCollider2D>();
        bottleCollider.size = new Vector2(1.15f, 3.1f);
        bottleCollider.offset = new Vector2(0, 1.55f);
        visual = new GameObject("visual").transform; visual.SetParent(transform, false);
        glow = Spr("bottle_selected_glow", visual, 1.45f);
        back = Spr("bottle_glass_back", visual, 1.45f);
        front = Spr("bottle_glass_front", visual, 1.45f);
        lid = Spr("lid_gold", visual, BottleHeight);

        var maskObject = new GameObject("mask");
        maskObject.transform.SetParent(visual, false);
        maskObject.transform.localPosition = new Vector3(0, 1.45f, 0);
        mask = maskObject.AddComponent<SpriteMask>();
        mask.sprite = GameConfig.Sprite("bottle_mask");
        mask.isCustomRangeActive = true;

        liquidRoot = new GameObject("liquid").transform;
        liquidRoot.SetParent(transform, false);
        for (int i = 0; i < units.Length; i++)
        {
            var layerObject = new GameObject("u" + i);
            layerObject.transform.SetParent(liquidRoot, false);
            units[i] = layerObject.AddComponent<SpriteRenderer>();
            units[i].maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
        sparkle = Effects.Sparkles(liquidRoot); SetOrder(0);
    }
    public void SetOrder(int b)
    {
        order = b;
        glow.sortingOrder = b - 1;
        back.sortingOrder = b;
        front.sortingOrder = b + 2;
        lid.sortingOrder = b + 3;
        foreach (var unit in units)
        {
            unit.sortingOrder = b + 1;
        }
        mask.backSortingOrder = b;
        mask.frontSortingOrder = b + 1;
        sparkle.GetComponent<ParticleSystemRenderer>().sortingOrder = b + 2;
    }
    // removeAmt/extra* are used while pouring so liquid moves smoothly between bottles.
    public void SetState(Bottle bottle, float removeAmt = 0, int extraC = -1, float extraA = 0)
    {
        data = bottle;
        rem = removeAmt;
        extraColor = extraC;
        extraAmt = extraA;
    }

    public void Wobble(float amount)
    {
        wobbleAmount = Mathf.Max(wobbleAmount, amount);
    }
    void LateUpdate()
    {
        if (data == null)
        {
            return;
        }
        sel += ((isSelected ? 1 : 0) - sel) * Mathf.Min(1, Time.deltaTime * 15);
        wobbleAmount *= Mathf.Pow(.965f, Time.deltaTime * 60);
        lidT += ((data.Sealed ? 1 : 0) - lidT) * Mathf.Min(1, Time.deltaTime * 8);
        visual.localPosition = new Vector3(0, sel * .35f, 0);
        float glowAlpha = Mathf.Max(
            sel * .9f,
            Time.time < hintHighlightEndTime ? .5f + .4f * Mathf.Sin(Time.time * 8) : 0);
        glow.color = new Color(1, .89f, .47f, glowAlpha);
        lid.color = new Color(1, 1, 1, Mathf.Min(1, lidT * 1.5f));
        lid.transform.localPosition = new Vector3(
            0,
            BottleHeight + .07f + (1 - lidT) * .22f,
            0);
        // liquid stays level while the bottle tilts: pin its root to the lowest bottom corner in world space
        float bottleTiltCosine = Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad);
        float layerHeight = LayerHeight * (.35f + .65f * Mathf.Abs(bottleTiltCosine));
        float lowestBottleY = Mathf.Min(
            visual.TransformPoint(new Vector3(-.5f, 0)).y,
            visual.TransformPoint(new Vector3(.5f, 0)).y);
        liquidRoot.rotation = Quaternion.identity;
        liquidRoot.position = new Vector3(
            visual.position.x,
            lowestBottleY + .015f * bottleScale,
            0);
        var list = new List<(int c, bool hd, float m)>(); for (int i = 0; i < data.layers.Count; i++) list.Add((data.layers[i], data.IsHidden(i), 1f));
        float remainingRemovedLayers = rem;
        for (int i = list.Count - 1; i >= 0 && remainingRemovedLayers > 0; i--)
        {
            float removedAmount = Mathf.Min(1, remainingRemovedLayers);
            list[i] = (list[i].c, list[i].hd, 1 - removedAmount);
            remainingRemovedLayers -= removedAmount;
        }

        if (extraColor >= 0 && extraAmt > 0)
        {
            list.Add((extraColor, false, extraAmt));
        }

        float accumulatedLayerHeight = 0;
        int lastVisibleLayerIndex = -1;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].m > .001f)
            {
                lastVisibleLayerIndex = i;
            }
        }
        for (int i = 0; i < units.Length; i++)
        {
            var unit = units[i];
            if (i >= list.Count || list[i].m <= .001f)
            {
                unit.enabled = false;
                continue;
            }

            unit.enabled = true;
            unit.sprite = GameConfig.Sprite(
                list[i].hd ? "liquid_hidden" : "liquid_" + GameConfig.Names[list[i].c]);
            float renderedHeight = layerHeight * list[i].m;
            if (i == lastVisibleLayerIndex)
            {
                renderedHeight *= 1 + Mathf.Sin(Time.time * 7) * wobbleAmount * .03f;
            }

            unit.transform.localScale = new Vector3(1.2f, renderedHeight / .44f, 1);
            unit.transform.localPosition = new Vector3(
                0,
                accumulatedLayerHeight + renderedHeight / 2,
                0);
            accumulatedLayerHeight += renderedHeight;
        }
        sparkle.transform.localPosition = new Vector3(0, accumulatedLayerHeight / 2, 0);
        var sh = sparkle.shape;
        sh.scale = new Vector3(
            .55f,
            Mathf.Max(.02f, accumulatedLayerHeight - .1f),
            .01f);
        var em = sparkle.emission;
        em.enabled = accumulatedLayerHeight > .2f && !data.isDelivered;
    }
}
