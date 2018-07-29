using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

public class SparkSingleton : Singleton<SparkSingleton>
{
    public string username = "nick";
    public string password = "password";

    private bool isAuthenticated;
    public bool IsAuthenticated;

    private string playerId;
    private string address;
    private long balance;
    private int level;

    private List<UnityAction> authenticatedCallbacks;

    private void Awake()
    {
        base.Awake();

        if (this.isDestroyed)
        {
            return;
        }

        LeanTween.init(800);

        //LoginDevelopment(username, password);

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
            this.address = null;
            this.balance = 0;
            this.level = 0;

            //if (!GS.Instance.Authenticated)
            //{
            //    Login(this.username, this.password);
            //}
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
            GetPlayerData();
        }
        else
        {
            Debug.LogWarning("Not authenticated...");
        }
    }

    public string GetPlayerId()
    {
        return this.playerId;
    }

    public void AddAuthenticatedCallback(UnityAction callback)
    {
        this.authenticatedCallbacks.Add(callback);
    }

    public void ClearAuthenticatedCallbacks()
    {
        this.authenticatedCallbacks = new List<UnityAction>();
    }

    private void GetPlayerData()
    {
        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("GetPlayerData");
        request.Send(OnGetPlayerDataSuccess, OnGetPlayerDataError);
    }

    private void OnGetPlayerDataSuccess(LogEventResponse response)
    {
        GSData scriptData = response.ScriptData;
        this.playerId = scriptData.GetString("playerId");
        this.address = scriptData.GetString("address");

        if (scriptData.GetLong("balance") == null)
        {
            this.balance = 0;
        }
        else
        {
            this.balance = (long)scriptData.GetLong("balance");
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
            LoginRegisterPanel.Instance.OnLoginSuccess,
            LoginRegisterPanel.Instance.OnLoginError
        );
    }

    public void LoginDevelopment(string name, string password)
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
        //LoadingManager.Instance.LoadNextScene();
        Debug.Log("Logged in.");

    }

    private void OnLoginError(AuthenticationResponse response)
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
        this.balance = 0;
        this.level = 0;

        GS.GSPlatform.AuthToken = null;
        GS.Reset();
    }

    public void Register(string email, string username, string password)
    {
        RegistrationRequest request = new RegistrationRequest();
        request.SetUserName(email);
        request.SetDisplayName(username);
        request.SetPassword(password);
        request.Send(
            LoginRegisterPanel.Instance.OnRegisterSuccess,
            LoginRegisterPanel.Instance.OnRegisterError
        );
    }
}
