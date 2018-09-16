using UnityEngine;

public class FlagHelper : MonoBehaviour
{
    public const string FLAG_USE_SERVER = "FLAG_USE_SERVER";
    public const string FLAG_USE_STAGING = "FLAG_USE_STAGING";
    public const string FLAG_LOG_VERBOSE = "FLAG_LOG_VERBOSE";
    public const string FLAG_SKIP_MULLIGAN = "FLAG_SKIP_MULLIGAN";

    public static bool GetFlag(string flag)
    {
        int value = PlayerPrefs.GetInt(flag);
        return value == 1;
    }

    public static bool IsServerEnabled()
    {
#if UNITY_EDITOR
        return GetFlag(FLAG_USE_SERVER);
#else
        return true;
#endif
    }

    public static bool IsServerStaging()
    {
#if UNITY_EDITOR
        return GetFlag(FLAG_USE_STAGING);
#else
        return true;
#endif
    }

    public static bool IsLogVerbose()
    {
#if UNITY_EDITOR
        return GetFlag(FLAG_LOG_VERBOSE);
#else
        return Debug.isDebugBuild;
#endif
    }

    public static bool ShouldSkipMulligan()
    {
#if UNITY_EDITOR
        return GetFlag(FLAG_SKIP_MULLIGAN);
#else
        return false;
#endif
    }
}
