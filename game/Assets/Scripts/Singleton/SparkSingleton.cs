using System;
using System.Collections.Generic;
using NBitcoin.BouncyCastle.Math;
using UnityEngine;
using UnityEngine.Events;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

public class SparkSingleton : Singleton<SparkSingleton>
{
    private static string PLAYER_PREF_AUTH_TOKEN_KEY = "gamesparks.authtoken";

    [SerializeField]
    private string username = "";

    [SerializeField]
    private string password = "";

    private bool isAuthenticated;
    public bool IsAuthenticated => isAuthenticated;

    private bool isLatestVersion;
    public bool IsLatestVersion => isLatestVersion;

    private string playerId;
    public string PlayerId => playerId;

    private string displayName;
    public string DisplayName => displayName;

    private string address;
    private BigInteger balance;
    private int level;

    private List<UnityAction> authenticatedCallbacks;

    private void Awake()
    {
        base.Awake();

        if (this.isDestroyed)
        {
            return;
        }

        this.isLatestVersion = false;

        LeanTween.init(800);

#if UNITY_EDITOR
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            LoginDevelopment(username, password);
        }
#endif

        ClearAuthenticatedCallbacks();
        this.isAuthenticated = false;

        GS.Instance.GameSparksAvailable = OnGameSparksAvailable;
        GS.Instance.GameSparksAuthenticated = OnGameSparksAuthenticated;
    }

    private void OnGameSparksAvailable(bool available)
    {
        if (available)
        {
            this.playerId = null;
            this.displayName = null;
            this.address = null;
            this.balance = new BigInteger("0");
            this.level = 0;
        }
        else
        {
            Debug.LogWarning("not available...");
        }
    }

    private void OnGameSparksAuthenticated(string playerId)
    {
        if (GS.Instance.Authenticated)
        {
            SendGetPlayerDataRequest();
        }
        else
        {
            Debug.LogWarning("Not authenticated...");
        }
    }

    public void AddAuthenticatedCallback(UnityAction callback)
    {
        if (this.isAuthenticated)
        {
            callback();
        }
        else
        {
            this.authenticatedCallbacks.Add(callback);
        }
    }

    public void ClearAuthenticatedCallbacks()
    {
        this.authenticatedCallbacks = new List<UnityAction>();
    }

    private void SendGetGameVersionRequest()
    {

        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("GetGameVersion");
        request.Send(OnGetGameVersionSuccess, OnGetGameVersionError);
    }

    private void OnGetGameVersionSuccess(LogEventResponse response)
    {
        GSData scriptData = response.ScriptData;
        string version = scriptData.GetString("version");

        try
        {
            float versionFloat = float.Parse(version);
            if (Application.version == version)
            {
                this.isLatestVersion = true;
            }
        }
        catch (FormatException)
        {
            Debug.LogError("Invalid version from server");
        }

        if (FlagHelper.IsLogVerbose())
        {
            if (this.isLatestVersion)
            {
                Debug.Log("Game version is latest.");
            }
            else
            {
                Debug.Log("Game version is NOT latest.");
            }
        }
    }

    private void OnGetGameVersionError(LogEventResponse response)
    {
        Debug.LogError("GetGameVersion request error.");
    }

    private void SendGetPlayerDataRequest()
    {
        // Redundant but just to be sure variable is false.
        this.isLatestVersion = false;

        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("GetPlayerData");
        request.Send(OnGetPlayerDataSuccess, OnGetPlayerDataError);
    }

    private void OnGetPlayerDataSuccess(LogEventResponse response)
    {
        GSData scriptData = response.ScriptData;

        string latestVersion = scriptData.GetString("latestVersion");
        try
        {
            float versionFloat = float.Parse(latestVersion);
            if (Application.version == latestVersion)
            {
                this.isLatestVersion = true;
            }
        }
        catch (FormatException)
        {
            this.isLatestVersion = false;
            Debug.LogError("Invalid version from server");
        }

        if (FlagHelper.IsLogVerbose())
        {
            if (this.isLatestVersion)
            {
                Debug.Log("Game version is latest.");
            }
            else
            {
                Debug.Log("Game version is NOT latest.");
            }
        }

        this.playerId = scriptData.GetString("playerId");
        this.displayName = scriptData.GetString("displayName");
        this.address = scriptData.GetString("address");

        string balanceString = scriptData.GetString("balance");
        if (balanceString == null)
        {
            this.balance = new BigInteger("0");
        }
        else
        {
            this.balance = new BigInteger(balanceString);
        }

        if (scriptData.GetInt("level") == null)
        {
            this.level = 0;
        }
        else
        {
            this.level = (int)scriptData.GetInt("level");
        }

        this.isAuthenticated = true;
        foreach (UnityAction callback in this.authenticatedCallbacks)
        {
            callback();
        }
    }

    private void OnGetPlayerDataError(LogEventResponse response)
    {
        Debug.Log("OnGetPlayerDataError");
        Debug.Log(response.Errors.ToString());
    }

    public void Login(string name, string password)
    {
        AuthenticationRequest request = new AuthenticationRequest();
        request.SetUserName(name);
        request.SetPassword(password);
        request.Send(
            OnLoginSuccess,
            OnLoginError
        );
    }

    private void OnLoginSuccess(AuthenticationResponse response)
    {
        SendGetPlayerDataRequest();
        LoginRegisterPanel.Instance.OnLoginSuccess(response);
    }

    private void OnLoginError(AuthenticationResponse response)
    {
        LoginRegisterPanel.Instance.OnLoginError(response);
    }

    public void LoginDevelopment(string name, string password)
    {
        AuthenticationRequest request = new AuthenticationRequest();
        request.SetUserName(name);
        request.SetPassword(password);
        request.Send(
            OnLoginDevelopmentSuccess,
            OnLoginDevelopmentError
        );
    }

    private void OnLoginDevelopmentSuccess(AuthenticationResponse response)
    {
        //LoadingManager.Instance.LoadNextScene();
        Debug.Log("Logged in.");

    }

    private void OnLoginDevelopmentError(AuthenticationResponse response)
    {
        Debug.Log("OnLoginError");
        Debug.Log(response.Errors.ToString());
    }

    public void Logout()
    {
        ClearAuthenticatedCallbacks();

        this.isAuthenticated = false;
        this.playerId = null;
        this.address = null;
        this.balance = new BigInteger("0");
        this.level = 0;

        DeckStore.Instance().Logout();
        GS.GSPlatform.AuthToken = null;
        PlayerPrefs.SetString(PLAYER_PREF_AUTH_TOKEN_KEY, null);
        GS.Reset();

        Application.LoadLevel("Login");
    }

    public void Register(string email, string username, string password)
    {
        RegistrationRequest request = new RegistrationRequest();
        request.SetUserName(email);
        request.SetDisplayName(username);
        request.SetPassword(password);
        request.Send(
            OnRegisterSuccess,
            OnRegisterError
        );
    }

    private void OnRegisterSuccess(RegistrationResponse response)
    {
        SendGetPlayerDataRequest();
        LoginRegisterPanel.Instance.OnRegisterSuccess(response);
    }

    private void OnRegisterError(RegistrationResponse response)
    {
        LoginRegisterPanel.Instance.OnRegisterError(response);
    }
}
