using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ObjectUI : MouseWatchable
{
    protected bool selected;
    protected Vector3 originalSize;
    protected float scalingFactor;

    // Use this for initialization
    protected void Initialize()
    {
        scalingFactor = 1.05f;
        originalSize = transform.localScale;
    }

    public override void EnterHover()
    {
        transform.localScale = originalSize * scalingFactor;
    }

    public override void ExitHover()
    {
        transform.localScale = originalSize;
    }

    public void Select()
    {
        selected = true;
    }

    public void Deselect()
    {
        selected = false;
    }
}
