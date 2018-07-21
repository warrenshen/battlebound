using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

public class SparkSingleton : Singleton<SparkSingleton>
{
    public string username = "warren";
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

        ClearAuthenticatedCallbacks();
        this.isAuthenticated = false;
        GS.Instance.GameSparksAuthenticated = OnGameSparksAuthenticated;
    }

    private void OnGameSparksAuthenticated(string result)
    {
        if (GS.Instance.Authenticated)
        {
            Debug.Log("Authenticated!");
            GetPlayerData();
        }
        else
        {
            Debug.Log("Not authenticated...");
        }
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
            Debug.Log("callback");
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
        request.Send(OnLoginSuccess, OnLoginError);
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

    private void Register(string username, string password)
    {
        RegistrationRequest request = new RegistrationRequest();
        request.SetUserName(username);
        request.SetDisplayName(username);
        request.SetPassword(password);
        request.Send(OnRegistrationSuccess, OnRegistrationError);
    }

    private void OnRegistrationSuccess(RegistrationResponse response)
    {

    }

    private void OnRegistrationError(RegistrationResponse response)
    {
        Debug.Log("OnRegistrationError");
        Debug.Log(response.Errors.ToString());
    }
}
