using System.Collections.Generic;
using UnityEngine;


public class ClayPlatePoolManager : MonoBehaviour
{
    [Header("Prefab Reference")]
    [SerializeField] private ClayPlateMovement _clayPlatePrefab;
    [SerializeField] private string _adderessablekey = "ClayPlatePrefab";

    [Header("Pool Configuration")]
    [SerializeField] private int _initialPoolSize = 10;
    [SerializeField] private int _maxPoolSize = 20;

    private readonly Queue<ClayPlateMovement> _availablePlates = new();



    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < _initialPoolSize; i++)
        {
            ClayPlateMovement plate = CreateNewPlate();

            plate.gameObject.SetActive(false);

            _availablePlates.Enqueue(plate);
        }
    }

    private ClayPlateMovement CreateNewPlate()
    {
        ClayPlateMovement plate = Instantiate(_clayPlatePrefab, this.transform);
        plate.Initialize(this);
        if (plate == null)
        {
            DevLog.Log("failed to  create place the prefab is missing the clayplate component ");
            return null;
        }
       
        return plate;
    }


    public ClayPlateMovement GetPlate()
    {
        ClayPlateMovement clayPlate = null;

        if (_availablePlates.Count > 0)
        {
            clayPlate = _availablePlates.Dequeue();
        }

        if (clayPlate == null && _availablePlates.Count < _maxPoolSize)
        {
            clayPlate = CreateNewPlate();
            _maxPoolSize ++;
        }

        if (clayPlate == null)
        {
            DevLog.Log("No plates available in the pool and max pool size reached.");
            return null;
        }


        clayPlate.gameObject.SetActive(true);

        return clayPlate;

    }

    public void ReturnPlate(ClayPlateMovement plate)
    {
        if (plate == null)
        {
            DevLog.LogError("Attempted to return a null plate to the pool.");
            return;
        }
       

        plate.gameObject.SetActive(false);
       
        

        if(_availablePlates.Count >= _maxPoolSize)
        {
            Destroy(plate.gameObject);
        }
        else
        {
            _availablePlates.Enqueue(plate);
        }
    }

  

 
}

