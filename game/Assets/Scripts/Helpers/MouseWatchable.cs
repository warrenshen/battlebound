using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class MouseWatchable : MonoBehaviour
{
    [SerializeField]
    public bool noInteraction;

    public virtual void MouseDown()
    {

    }
    public virtual void MouseUp()
    {

    }
    public virtual void EnterHover()
    {

    }
    public virtual void ExitHover()
    {

    }
}
