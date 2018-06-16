using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXPoolManager : MonoBehaviour {
    public Dictionary<string, int> effectIndices;

    public static FXPoolManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

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
