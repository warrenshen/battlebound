using UnityEngine;
using System.IO;
using System;

/// <summary>
/// GameSparks settings which are used with <see cref="GameSparksUnity"/> to 
/// connect to the GameSparks backend. 
/// </summary>
public class GameSparksSettings : ScriptableObject
{

    public const string gamesparksSettingsAssetName = "GameSparksSettings";
    public const string gamesparksSettingsPath = "GameSparks/Resources";
    public const string gamesparksSettingsAssetExtension = ".asset";
    private static readonly string liveServiceUrlBase = "wss://live-{0}.ws.gamesparks.net/ws/{1}/{0}";
    private static readonly string previewServiceUrlBase = "wss://preview-{0}.ws.gamesparks.net/ws/{1}/{0}";
    private static GameSparksSettings instance;

    public static void SetInstance(GameSparksSettings settings)
    {
        instance = settings;
    }

    public static GameSparksSettings Instance
    {
        get
        {
            if (ReferenceEquals(instance, null))
            {
                instance = Resources.Load(gamesparksSettingsAssetName) as GameSparksSettings;
                if (ReferenceEquals(instance, null))
                {
                    // If not found, autocreate the asset object.
                    instance = CreateInstance<GameSparksSettings>();
                }
            }
            return instance;
        }
    }

    [SerializeField]
    private string sdkVersion;
    [SerializeField]
    private string apiKey = "";
    [SerializeField]
    private string apiSecret = "";
    [SerializeField]
    private string apiKeyStaging = "";
    [SerializeField]
    private string apiSecretStaging = "";
    [SerializeField]
    private string credential = "device";
    [SerializeField]
    private bool previewBuild = true;
    [SerializeField]
    private bool debugBuild = false;

    public static bool PreviewBuild
    {
        get { return Instance.previewBuild; }
        set { Instance.previewBuild = value; }
    }

    public static string SdkVersion
    {
        get { return Instance.sdkVersion; }
        set { Instance.sdkVersion = value; }
    }

    public static string ApiSecret
    {
        get { return Instance.apiSecret; }
        set { Instance.apiSecret = value; }
    }

    public static string ApiKey
    {
        get { return Instance.apiKey; }
        set { Instance.apiKey = value; }
    }

    public static string ApiSecretStaging
    {
        get { return Instance.apiSecretStaging; }
        set { Instance.apiSecretStaging = value; }
    }

    public static string ApiKeyStaging
    {
        get { return Instance.apiKeyStaging; }
        set { Instance.apiKeyStaging = value; }
    }

    public static string Credential
    {
        get { return (Instance.credential == null || Instance.credential.Length == 0) ? "device" : Instance.credential; }
        set { Instance.credential = value; }
    }

    public static bool DebugBuild
    {
        get { return Instance.debugBuild; }
        set { Instance.debugBuild = value; }
    }

    public static string ServiceUrl
    {
        get
        {
            String urlAddition = ApiKeyFinal();

            if (ApiSecretFinal().Contains(":"))
            {
                urlAddition = ApiSecretFinal().Substring(0, ApiSecretFinal().IndexOf(":")) + "/" + urlAddition;
            }
            if (Instance.previewBuild)
            {
                return String.Format(previewServiceUrlBase, urlAddition, Instance.credential);
            }
            return String.Format(liveServiceUrlBase, urlAddition, Instance.credential);
        }
    }

    /*
     * Returns api key to use based on developer flag for development vs staging.
     */
    public static string ApiKeyFinal()
    {
        if (FlagHelper.IsServerStaging())
        {
            return Instance.apiKeyStaging;
        }
        else
        {
            return Instance.apiKey;
        }
    }

    /*
     * Returns api secret to use based on developer flag for development vs staging.
     */
    public static string ApiSecretFinal()
    {
        if (FlagHelper.IsServerStaging())
        {
            return Instance.apiSecretStaging;
        }
        else
        {
            return Instance.apiSecret;
        }
    }
}
