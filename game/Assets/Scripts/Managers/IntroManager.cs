﻿using UnityEngine;

public class IntroManager : MonoBehaviour
{
    private void Awake()
    {
        SparkSingleton.Instance.AddAuthenticationCallback(Callback);
    }

    private void Callback()
    {
        if (SparkSingleton.Instance.IsAuthenticated)
        {
            Application.LoadLevel("Menu");
        }
        else
        {
            Application.LoadLevel("Login");
        }
    }
}
