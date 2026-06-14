using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
 
    [Header("Spawn Settings")]
    public float startSpawnInterval = 2f;
    public float minSpawnInterval = 0.6f;

    [Header("Force Settings")]
    public float baseForce = 15f;
    public float maxForce = 22f;

    private float _gameTime;

    public void UpdateTime(float deltaTime)
    {
        _gameTime += deltaTime;
    }

    public float GetSpawnInterval()
    {
        float t = Mathf.Clamp01(_gameTime / 60f); // 60 sec to max difficulty
        return Mathf.Lerp(startSpawnInterval, minSpawnInterval, t);
    }

    public float GetLaunchForce()
    {
        float t = Mathf.Clamp01(_gameTime / 60f);
        return Mathf.Lerp(baseForce, maxForce, t);
    }

    public float GetHorizontalVariance()
    {
        float t = Mathf.Clamp01(_gameTime / 60f);
        return Mathf.Lerp(0.3f, 0.6f, t);
    }
}

