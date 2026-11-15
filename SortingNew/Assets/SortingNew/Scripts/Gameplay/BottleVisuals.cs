using System.Collections.Generic;
using UnityEngine;

// Builds and draws the bottle's sprite layers. BottleView sends it state and animation values.
public class BottleVisuals : MonoBehaviour
{
    private static readonly Dictionary<string, Sprite> SpriteCache = new();

    private static readonly string[] ColorSpriteNames =
    {
        "blueberry", "cherry", "coffee", "grape", "guava",
        "kiwi", "lagoon", "lemon", "lime", "orange"
    };

    private readonly SpriteRenderer[] _liquidLayers = new SpriteRenderer[6];
    private Transform _visualRoot;
    private Transform _liquidRoot;
    private SpriteRenderer _glassBackRenderer;
    private SpriteRenderer _glassFrontRenderer;
    private SpriteRenderer _lidRenderer;
    private SpriteRenderer _selectionGlowRenderer;
    private SpriteMask _liquidMask;
    private ParticleSystem _sparkleParticles;

    public void Initialize(Transform bottleRoot)
    {
        _visualRoot = new GameObject("visual").transform;
        _visualRoot.SetParent(bottleRoot, false);

        _selectionGlowRenderer = CreateSprite("bottle_selected_glow", _visualRoot, 1.45f);
        _glassBackRenderer = CreateSprite("bottle_glass_back", _visualRoot, 1.45f);
        _glassFrontRenderer = CreateSprite("bottle_glass_front", _visualRoot, 1.45f);
        _lidRenderer = CreateSprite("lid_gold", _visualRoot, BottleView.BottleHeight);

        var maskObject = new GameObject("mask");
        maskObject.transform.SetParent(_visualRoot, false);
        maskObject.transform.localPosition = new Vector3(0, 1.45f, 0);
        _liquidMask = maskObject.AddComponent<SpriteMask>();
        _liquidMask.sprite = LoadSprite("bottle_mask");
        _liquidMask.isCustomRangeActive = true;

        _liquidRoot = new GameObject("liquid").transform;
        _liquidRoot.SetParent(bottleRoot, false);
        for (int i = 0; i < _liquidLayers.Length; i++)
        {
            var layerObject = new GameObject("layer" + i);
            layerObject.transform.SetParent(_liquidRoot, false);
            _liquidLayers[i] = layerObject.AddComponent<SpriteRenderer>();
            _liquidLayers[i].maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }

        _sparkleParticles = Effects.Sparkles(_liquidRoot);
    }

    private static SpriteRenderer CreateSprite(string spriteName, Transform parent, float localY)
    {
        var spriteObject = new GameObject(spriteName);
        spriteObject.transform.SetParent(parent, false);
        spriteObject.transform.localPosition = new Vector3(0, localY, 0);

        var renderer = spriteObject.AddComponent<SpriteRenderer>();
        renderer.sprite = LoadSprite(spriteName);
        return renderer;
    }

    private static Sprite LoadSprite(string spriteName)
    {
        string resourcePath = "Sprites/" + spriteName;
        if (!SpriteCache.TryGetValue(resourcePath, out Sprite sprite))
        {
            sprite = Resources.Load<Sprite>(resourcePath);
            SpriteCache[resourcePath] = sprite;
        }

        return sprite;
    }

    private static Sprite LoadLiquidSprite(int colorIndex, bool isHidden)
    {
        if (isHidden)
            return LoadSprite("Colors/liquid_hidden");

        if (colorIndex < 0 || colorIndex >= ColorSpriteNames.Length)
            return null;

        return LoadSprite("Colors/liquid_" + ColorSpriteNames[colorIndex]);
    }

    public void SetSortingOrder(int sortingOrder)
    {
        if (!_visualRoot)
            return;

        _selectionGlowRenderer.sortingOrder = sortingOrder - 1;
        _glassBackRenderer.sortingOrder = sortingOrder;
        _glassFrontRenderer.sortingOrder = sortingOrder + 2;
        _lidRenderer.sortingOrder = sortingOrder + 3;
        foreach (var layer in _liquidLayers)
            layer.sortingOrder = sortingOrder + 1;

        _liquidMask.backSortingOrder = sortingOrder;
        _liquidMask.frontSortingOrder = sortingOrder + 1;
        _sparkleParticles.GetComponent<ParticleSystemRenderer>().sortingOrder = sortingOrder + 2;
    }

    public void Render(
        Bottle bottleState,
        float removedLayerCount,
        int addedColorIndex,
        float addedLayerAmount,
        float selectionBlend,
        float lidBlend,
        float wobbleAmount,
        float hintHighlightEndTime,
        float bottleScale)
    {
        _visualRoot.localPosition = new Vector3(0, selectionBlend * .35f, 0);

        float glowAlpha = Mathf.Max(
            selectionBlend * .9f,
            Time.time < hintHighlightEndTime ? .5f + .4f * Mathf.Sin(Time.time * 8) : 0);
        _selectionGlowRenderer.color = new Color(1, .89f, .47f, glowAlpha);
        _lidRenderer.color = new Color(1, 1, 1, Mathf.Min(1, lidBlend * 1.5f));
        _lidRenderer.transform.localPosition = new Vector3(
            0,
            BottleView.BottleHeight + .07f + (1 - lidBlend) * .22f,
            0);

        float bottleTiltCosine = Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad);
        float currentLayerHeight = BottleView.LayerHeight
            * (.35f + .65f * Mathf.Abs(bottleTiltCosine));
        float lowestBottleY = Mathf.Min(
            _visualRoot.TransformPoint(new Vector3(-.5f, 0)).y,
            _visualRoot.TransformPoint(new Vector3(.5f, 0)).y);
        _liquidRoot.rotation = Quaternion.identity;
        _liquidRoot.position = new Vector3(
            _visualRoot.position.x,
            lowestBottleY + .015f * bottleScale,
            0);

        var visibleLayers = new List<(int colorIndex, bool isHidden, float visibleAmount)>();
        for (int i = 0; i < bottleState.layers.Count; i++)
        {
            visibleLayers.Add((bottleState.layers[i], bottleState.IsHidden(i), 1f));
        }

        float remainingRemovedLayerCount = removedLayerCount;
        for (int i = visibleLayers.Count - 1; i >= 0 && remainingRemovedLayerCount > 0; i--)
        {
            float removedAmount = Mathf.Min(1, remainingRemovedLayerCount);
            visibleLayers[i] = (
                visibleLayers[i].colorIndex,
                visibleLayers[i].isHidden,
                1 - removedAmount);
            remainingRemovedLayerCount -= removedAmount;
        }

        if (addedColorIndex >= 0 && addedLayerAmount > 0)
            visibleLayers.Add((addedColorIndex, false, addedLayerAmount));

        int lastVisibleLayerIndex = -1;
        for (int i = 0; i < visibleLayers.Count; i++)
        {
            if (visibleLayers[i].visibleAmount > .001f)
                lastVisibleLayerIndex = i;
        }

        float accumulatedLiquidHeight = 0;
        for (int i = 0; i < _liquidLayers.Length; i++)
        {
            var renderer = _liquidLayers[i];
            if (i >= visibleLayers.Count || visibleLayers[i].visibleAmount <= .001f)
            {
                renderer.enabled = false;
                continue;
            }

            var layer = visibleLayers[i];
            renderer.enabled = true;
            renderer.sprite = LoadLiquidSprite(layer.colorIndex, layer.isHidden);

            float renderedLayerHeight = currentLayerHeight * layer.visibleAmount;
            if (i == lastVisibleLayerIndex)
                renderedLayerHeight *= 1 + Mathf.Sin(Time.time * 7) * wobbleAmount * .03f;

            renderer.transform.localScale = new Vector3(1.2f, renderedLayerHeight / .44f, 1);
            renderer.transform.localPosition = new Vector3(
                0,
                accumulatedLiquidHeight + renderedLayerHeight / 2,
                0);
            accumulatedLiquidHeight += renderedLayerHeight;
        }

        _sparkleParticles.transform.localPosition = new Vector3(0, accumulatedLiquidHeight / 2, 0);
        var sparkleShape = _sparkleParticles.shape;
        sparkleShape.scale = new Vector3(.55f, Mathf.Max(.02f, accumulatedLiquidHeight - .1f), .01f);
        var sparkleEmission = _sparkleParticles.emission;
        sparkleEmission.enabled = accumulatedLiquidHeight > .2f && !bottleState.isDelivered;
    }
}
