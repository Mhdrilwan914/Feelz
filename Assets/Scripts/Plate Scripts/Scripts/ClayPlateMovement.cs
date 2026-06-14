using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class ClayPlateMovement : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D _rigidbody;
    private ClayPlatePoolManager _poolManager;

    [Header("Launch Settings")]
    [SerializeField] private Vector2 _launchForceRange = new (-1f, 1f);
    [SerializeField] private Vector2 _launchVerticalRange = new (17f, 18f);
    [SerializeField] private Vector2 _launchTorqueRange = new (-50f, 50f);

    [Header("Boundaries")]
    [SerializeField] private float _returnToPoolThreshold = -10f;

    private bool _isInitialized = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        if (_rigidbody == null)
        {
            Debug.LogError($"ClayPlateMovement on {gameObject.name} is missing Rigidbody2D component!");
        }
    }

    private void FixedUpdate()
    {
        if (!_isInitialized || _poolManager == null) return;

        if (transform.position.y < _returnToPoolThreshold)
        {
            Debug.Log($"Plate {gameObject.name} fell below threshold, returning to pool.");
            ReturnToPool();
        }
    }

    public void Initialize(ClayPlatePoolManager poolManager)
    {
        if (poolManager == null)
        {
            Debug.LogError($"Cannot initialize {gameObject.name}: Pool manager is null");
            return;
        }

        _poolManager = poolManager;
        _isInitialized = true;
    }

    public void LaunchPlate()
    {
        if (!_isInitialized)
        {
            Debug.LogError($"Cannot launch {gameObject.name}: Plate not initialized");
            return;
        }

        if (_rigidbody == null)
        {
            Debug.LogError($"Cannot launch {gameObject.name}: Rigidbody2D is missing");
            return;
        }

       
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;

        // Generate random launch force
        float horizontal = Random.Range(_launchForceRange.x, _launchForceRange.y);
        float vertical = Random.Range(_launchVerticalRange.x, _launchVerticalRange.y);

        Vector2 force = new Vector2(horizontal, vertical);
        _rigidbody.AddForce(force, ForceMode2D.Impulse);

     
        float torque = Random.Range(_launchTorqueRange.x, _launchTorqueRange.y);
        _rigidbody.AddTorque(torque, ForceMode2D.Impulse);
    }

    public void ReturnToPool()
    {
        if (!_isInitialized || _poolManager == null)
        {
            Debug.LogWarning($"Cannot return {gameObject.name} to pool - not properly initialized");
            Destroy(gameObject);
            return;
        }

        // Reset velocity only, NOT position
        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
        }

        _poolManager.ReturnPlate(this);
    }

    private void OnDestroy()
    {
        _poolManager = null;
        _isInitialized = false;
    }

  
}