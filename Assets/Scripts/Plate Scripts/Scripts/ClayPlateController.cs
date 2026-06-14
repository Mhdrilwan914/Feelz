using UnityEngine;

public class ClayPlateController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ClayPlatePoolManager _clayPool;
    [SerializeField] private DifficultyManager _difficulty;
    [SerializeField] private float _padding = .2f;
    private float _spawnTimer;

    private Camera _cam;
    private float _minX;
    private float _maxX;
    private float _halfWidth;
    private float _halfHeight;

    public float SpawnY { get; private set; }

    private void Awake()
    {
       
        _cam = Camera.main;

        if (_cam == null)
        {
            Debug.LogError("Main Camera not found!");
            return;
        }
       
        CalculateBounds();
    }

    private void CalculateBounds()
    {
        _halfHeight = _cam.orthographicSize;
        _halfWidth = _halfHeight * _cam.aspect;

        DevLog.Log($"game _halfHeight: {_halfHeight}, _halfWidth: {_halfWidth}");

        Vector3 camPos = _cam.transform.position;

       
        float horizontalPadding = _halfWidth * _padding;

        _minX = camPos.x - _halfWidth + horizontalPadding;
        _maxX = camPos.x + _halfWidth - horizontalPadding;

        DevLog.Log($"Spawn Range X → min: {_minX}, max: {_maxX}");

      
        float spawnOffset = 1f;
        SpawnY = camPos.y - _halfHeight - spawnOffset;
    }

    private void Update()
    {
        if (_clayPool == null || _difficulty == null) return;

        _difficulty.UpdateTime(Time.deltaTime);

        _spawnTimer += Time.deltaTime;

       
        while (_spawnTimer >= _difficulty.GetSpawnInterval())
        {
            _spawnTimer -= _difficulty.GetSpawnInterval();
            ReleasePlateOnAir();
        }
    }

    private void ReleasePlateOnAir()
    {
        ClayPlateMovement plate = _clayPool.GetPlate();
        if (plate == null) return;

        float padding = 0.3f;
        float randomX = Random.Range(_minX + padding, _maxX - padding);

        plate.transform.position = new Vector3(randomX, SpawnY, 0f);

        plate.LaunchPlate();
    }
}