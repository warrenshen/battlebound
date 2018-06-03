using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

public class SparkSingleton : Singleton<SparkSingleton> {
    private void Awake()
    {
        base.Awake();
        Login();
    }

    private void Login()
    {
        AuthenticationRequest request = new AuthenticationRequest();
        request.SetUserName("warren");
        request.SetPassword("password");
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
        Login();
    }

    private void OnRegistrationError(RegistrationResponse response)
    {
        //errorMessageText.text = response.Errors.JSON.ToString();
    }
}
