using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXPoolManager : MonoBehaviour {
    public Dictionary<string, int> effectIndices;
    public Dictionary<string, string> shortToFull;

    public static FXPoolManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        this.effectIndices = new Dictionary<string, int>();
        this.shortToFull = new Dictionary<string, string>();
        //init conversion
        this.shortToFull["shielded"] = "DivineShield";
        this.shortToFull["taunt"] = "Taunt";
    }

	// Use this for initialization
	void Start () {
        foreach(Transform effect in transform) {
            effectIndices.Add(effect.name, 0);
        }
	}

    private bool IsShortname(string initial) {
        if (shortToFull.ContainsKey(initial))
            return true;
        else return false;
    }

    public bool HasEffect(string name) {
        if (IsShortname(name))
            name = shortToFull[name];
        GameObject container = GameObject.Find(name);
        return container != null;
    }

    private Transform GetEffect(string name) {
        if (IsShortname(name))
            name = shortToFull[name];
        GameObject container = GameObject.Find(name);
        if (container == null)
            Debug.LogWarning(string.Format("Effect {0} not found!", name));
        
        Transform chosen = container.transform.GetChild(effectIndices[name]);
        effectIndices[name] = (effectIndices[name] + 1) % container.transform.childCount;
        return chosen;
    }
	
    public void PlayEffect(string name, Vector3 pos) {
        Transform chosen = GetEffect(name);
        chosen.position = pos;
        chosen.gameObject.SetActive(true);
    }

    public Transform AssignEffect(string name, Transform parent) {
        Transform chosen = GetEffect(name);
        chosen.parent = parent;
        chosen.localPosition = Vector3.zero;
        chosen.gameObject.SetActive(true);
        return chosen;
    }

    public void UnassignEffect(string name, GameObject effect, Transform parent) {
        effect.SetActive(false);
        effect.transform.parent = GetEffect(name);
        effect.transform.localPosition = Vector3.zero;
    }
}
