using Assets.KnifeGame.Scripts.Core.Knife;
using UnityEngine;

public class KnifeSpwaner : MonoBehaviour
{
    [SerializeField] private KnifeView _knifePrefab;
    [SerializeField] private Transform _knifeSpwanPoint;

    private void Start()
    {
        Spwan();
    }
    public KnifeView Spwan()
    {
        KnifeView knife = Instantiate(_knifePrefab, _knifeSpwanPoint.position, Quaternion.identity);

        if (knife != null)
        {
            return knife;

        }
        return null;
    }
}
