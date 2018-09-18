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
    private const string PLAYER_PREF_AUTH_TOKEN_KEY = "gamesparks.authtoken";
    private const string PLAYER_PREF_HAS_LOGGED_IN = "PLAYER_PREF_HAS_LOGGED_IN";

    private bool isAvailable;
    public bool IsAvailable => isAvailable;

    private bool isAuthenticated;
    public bool IsAuthenticated => isAuthenticated;

    private bool isLatestVersion;
    public bool IsLatestVersion => isLatestVersion;

    private string playerId;
    public string PlayerId => playerId;

    private string displayName;
    public string DisplayName => displayName;

    private string address;
    public string Address => address;

    private int rankGlobal;
    public int RankGlobal => rankGlobal;

    private int rankElo;
    public int RankElo => rankElo;

    private BigInteger balance;
    private int level;

    private string activeDeck;
    public string ActiveDeck => activeDeck;

    private List<UnityAction> authCallbacks;

    private UnityAction loginRegisterCallback;
    private string loginRegisterErrorMessage;
    public string LoginRegisterErrorMessage => loginRegisterErrorMessage;

    private void Awake()
    {
        base.Awake();

        if (this.isDestroyed)
        {
            return;
        }

        this.isLatestVersion = false;

        LeanTween.init(800);

        this.isAvailable = false;
        this.isAuthenticated = false;
        ClearAuthenticatedCallbacks();

        GS.Instance.GameSparksAvailable = OnGameSparksAvailable;
        GS.Instance.GameSparksAuthenticated = OnGameSparksAuthenticated;
    }

    private void OnGameSparksAvailable(bool available)
    {
        if (available)
        {
            this.isAvailable = true;

            this.playerId = null;
            this.displayName = null;
            this.address = null;
            this.rankGlobal = -1;
            this.rankElo = -1;
            this.balance = new BigInteger("0");
            this.level = 0;

            if (!GS.Instance.Authenticated)
            {
                foreach (UnityAction callback in this.authCallbacks)
                {
                    callback.Invoke();
                }
            }
        }
        else
        {
            this.isAvailable = false;
            Debug.LogWarning("GS not available...");
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

    /*
     * Callback will be invoked when authentication state changes.
     */
    public void AddAuthenticationCallback(UnityAction callback)
    {
        if (this.isAuthenticated)
        {
            callback.Invoke();
        }
        this.authCallbacks.Add(callback);
    }

    public void ClearAuthenticatedCallbacks()
    {
        this.authCallbacks = new List<UnityAction>();
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

        if (scriptData.GetInt("rankGlobal") == null)
        {
            this.rankGlobal = 0;
        }
        else
        {
            this.rankGlobal = (int)scriptData.GetInt("rankGlobal");
        }
        if (scriptData.GetInt("rankElo") == null)
        {
            this.rankElo = 0;
        }
        else
        {
            this.rankElo = (int)scriptData.GetInt("rankElo");
        }

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

        string activeDeck = scriptData.GetString("activeDeck");
        if (activeDeck != null)
        {
            this.activeDeck = activeDeck;
        }

        this.isAuthenticated = true;
        foreach (UnityAction callback in this.authCallbacks)
        {
            callback.Invoke();
        }
    }

    private void OnGetPlayerDataError(LogEventResponse response)
    {
        Debug.Log("OnGetPlayerDataError");
        Debug.Log(response.Errors.ToString());
    }

    public void Login(string name, string password, UnityAction onAuthFinish)
    {
        this.loginRegisterErrorMessage = "";
        this.loginRegisterCallback = onAuthFinish;

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
        PlayerPrefs.SetInt(PLAYER_PREF_HAS_LOGGED_IN, 1);
        this.isAuthenticated = true;
        SendGetPlayerDataRequest();
        this.loginRegisterCallback.Invoke();
    }

    private void OnLoginError(AuthenticationResponse response)
    {
        GSData scriptData = response.Errors;
        if (scriptData.GetString("DETAILS") == "UNRECOGNISED")
        {
            this.loginRegisterErrorMessage = "Invalid login or password";
        }
        else
        {
            this.loginRegisterErrorMessage = "Something went wrong, please try again later";
        }
        this.loginRegisterCallback.Invoke();
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

    public void Register(
        string email,
        string username,
        string password,
        UnityAction onAuthFinish
    )
    {
        this.loginRegisterErrorMessage = "";
        this.loginRegisterCallback = onAuthFinish;

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
        this.isAuthenticated = true;
        SendGetPlayerDataRequest();
        this.loginRegisterCallback.Invoke();
    }

    private void OnRegisterError(RegistrationResponse response)
    {
        GSData scriptData = response.Errors;
        if (scriptData.GetString("USERNAME") == "TAKEN")
        {
            Debug.Log(scriptData.GetString("USERNAME"));
            Debug.Log(scriptData.GetString("errorMessage"));
            this.loginRegisterErrorMessage = "Email already taken.";
        }
        else
        {
            this.loginRegisterErrorMessage = "Something went wrong, please try again later";
        }
        this.loginRegisterCallback.Invoke();
    }

    public void SendUpdateDisplayNameRequest(string displayName, UnityAction onAuthFinish)
    {
        this.loginRegisterErrorMessage = "";
        this.loginRegisterCallback = onAuthFinish;

        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("UpdateDisplayName");
        request.SetEventAttribute("displayName", displayName);
        request.Send(OnUpdateDisplayNameSuccess, OnUpdateDisplayNameError);
    }

    private void OnUpdateDisplayNameSuccess(LogEventResponse response)
    {
        GSData scriptData = response.ScriptData;
        this.displayName = scriptData.GetString("displayName");
        this.loginRegisterCallback.Invoke();
    }

    private void OnUpdateDisplayNameError(LogEventResponse response)
    {
        GSData scriptData = response.Errors;
        this.loginRegisterErrorMessage = scriptData.GetString("errorMessage");
        this.loginRegisterCallback.Invoke();
    }

    public bool IsDisplayNameValid()
    {
        return this.displayName != null && this.displayName.Length > 0;
    }

    public void SetPlayerAddress(string address)
    {
        this.address = address;
    }
}
