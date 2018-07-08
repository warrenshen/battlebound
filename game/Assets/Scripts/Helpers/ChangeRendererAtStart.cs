using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRendererAtStart : MonoBehaviour
{
    public bool value = false;

    // Use this for initialization
    void Awake()
    {
        this.GetComponent<Renderer>().enabled = value;
    }
}
