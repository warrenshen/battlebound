using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXPool : MonoBehaviour {
    public Dictionary<string, int> effectIndices;

	// Use this for initialization
	void Start () {
        effectIndices = new Dictionary<string, int>();
        foreach(Transform effect in transform) {
            effectIndices.Add(effect.name, 0);
        }
	}
	
    public void PlayEffect(string name, Vector3 pos) {
        GameObject parent = GameObject.Find(name);
        Transform chosen = parent.transform.GetChild(effectIndices[name]);
        chosen.position = pos;
        chosen.gameObject.SetActive(true);
        effectIndices[name] = (effectIndices[name] + 1) % parent.transform.childCount;
    }
}
