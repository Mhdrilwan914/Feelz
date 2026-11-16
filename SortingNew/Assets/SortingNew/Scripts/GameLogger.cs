using UnityEngine;

public static class GameLogger
{


    // Enable logs only in the Unity Editor or Development Builds.
    private static bool IsLoggingEnabled => Debug.isDebugBuild;

    public static void Log(object message)
    {
        if (!IsLoggingEnabled)
            return;

        Debug.Log($"[GAME] {message}");
    }

    public static void Warning(object message)
    {
        if (!IsLoggingEnabled)
            return;

        Debug.LogWarning($"[GAME WARNING] {message}");
    }

    public static void Error(object message)
    {
        if (!IsLoggingEnabled)
            return;

        Debug.LogError($"[GAME ERROR] {message}");
    }
}



