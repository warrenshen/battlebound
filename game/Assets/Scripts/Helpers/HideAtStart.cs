using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideAtStart : MonoBehaviour
{
    public bool hide = true;

    // Use this for initialization
    void Awake()
    {
        if (hide)
        {
            gameObject.SetActive(false);
        }
    }
}
