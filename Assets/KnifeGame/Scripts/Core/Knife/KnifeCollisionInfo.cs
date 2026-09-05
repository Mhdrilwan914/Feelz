using UnityEngine;

namespace Assets.KnifeGame.Scripts.Core.Knife
{
    public class KnifeCollisionInfo
    {
        public GameObject HitObject { get; }

        public Vector2 ContactPoint { get; }

        public Vector2 Normal { get; }


        public KnifeCollisionInfo(GameObject hitObject, Vector2 contactPoint, Vector2 normal)
        {
            HitObject = hitObject;
            ContactPoint = contactPoint;
            Normal = normal;
        }

    }
}