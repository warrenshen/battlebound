using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

public class SparkSingleton : Singleton<SparkSingleton> {
    public string username = "warren";
    public string password = "password";

    private void Awake()
    {
        base.Awake();
        Login(username, password);
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
        Debug.LogError(response.Errors.JSON.ToString());
    }

    //private void Register()
    //{
    //    RegistrationRequest request = new RegistrationRequest();
    //    request.SetUserName(userNameInput.text);
    //    request.SetDisplayName(userNameInput.text);
    //    request.SetPassword(passwordInput.text);
    //    request.Send(OnRegistrationSuccess, OnRegistrationError);
    //}

    private void OnRegistrationSuccess(RegistrationResponse response)
    {
        
    }

    private void OnRegistrationError(RegistrationResponse response)
    {
        //errorMessageText.text = response.Errors.JSON.ToString();
    }
}
