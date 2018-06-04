using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectUI : MonoBehaviour {
	protected bool selected;
	protected Vector3 originalSize;
    protected float scalingFactor;

	// Use this for initialization
	protected void Initialize () {
        scalingFactor = 1.03f;
		originalSize = transform.localScale;
	}
	
	protected void OnMouseEnter()
    {
        transform.localScale = originalSize * scalingFactor;
    }

	protected void OnMouseExit()
    {
        transform.localScale = originalSize;
    }

    public void Select() {
        selected = true;
    }

    public void Deselect() {
        selected = false;
    }
}
