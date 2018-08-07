using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance => instance;

    protected bool isDestroyed;

    protected void Awake()
    {
        if (instance != null && this != instance)
        {
            this.isDestroyed = true;
            Destroy(this.gameObject);
        }
        else
        {
            this.isDestroyed = false;
            instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
