using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagHelper : MonoBehaviour
{
    public const string FLAG_USE_SERVER = "FLAG_USE_SERVER";
    public const string FLAG_LOG_VERBOSE = "FLAG_LOG_VERBOSE";
    public const string FLAG_SKIP_MULLIGAN = "FLAG_SKIP_MULLIGAN";

    public static bool GetFlag(string flag)
    {
        int value = PlayerPrefs.GetInt(flag);
        return value == 1;
    }

    public static bool IsServerEnabled()
    {
        return GetFlag(FLAG_USE_SERVER);
    }

    public static bool IsLogVerbose()
    {
        return GetFlag(FLAG_LOG_VERBOSE);
    }

    public static bool ShouldSkipMulligan()
    {
        return GetFlag(FLAG_SKIP_MULLIGAN);
    }
}
