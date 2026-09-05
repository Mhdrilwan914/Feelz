using UnityEngine;

public readonly struct HitInfo
{
    public Vector2 ContactPoint { get; }

    public Transform HitTransform { get; }


    public HitInfo(Vector2 contactPoint, Transform hitTransform)
    {

        ContactPoint = contactPoint;
        HitTransform = hitTransform;

    }
}