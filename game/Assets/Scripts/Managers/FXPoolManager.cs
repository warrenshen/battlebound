using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXPoolManager : MonoBehaviour
{
    public Dictionary<string, int> effectIndices;
    public Dictionary<string, string> shortToFull;

    public static FXPoolManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        this.effectIndices = new Dictionary<string, int>();
        this.shortToFull = new Dictionary<string, string>();
        //init conversion
        this.shortToFull[Card.CARD_ABILITY_SHIELD] = "DivineShieldVFX";
        this.shortToFull[Card.CARD_ABILITY_TAUNT] = "TauntVFX";

        foreach (Transform effect in transform)
        {
            effectIndices.Add(effect.name, 0);
        }
    }

    // Use this for initialization
    void Start()
    {
        foreach (Transform effect in transform)
        {
            Debug.Log(effect.name);
            effectIndices.Add(effect.name, 0);
        }
    }

    private string ResolveName(string initial)
    {
        if (shortToFull.ContainsKey(initial))
            return shortToFull[initial];
        else return initial;
    }

    public bool HasEffect(string effect)
    {
        return GetEffectPool(effect) != null;
    }

    public GameObject GetEffectPool(string effect)
    {
        effect = ResolveName(effect);
        GameObject pool = GameObject.Find(effect);
        return pool;
    }

    private Transform GetEffect(string effect)
    {
        effect = ResolveName(effect);
        GameObject pool = GetEffectPool(effect);
        if (pool == null)
        {
            Debug.LogError(string.Format("Effect {0} not found!", effect));
        }

        Transform chosen = pool.transform.GetChild(effectIndices[effect]);
        effectIndices[effect] = (effectIndices[effect] + 1) % pool.transform.childCount;
        return chosen;
    }

    public void PlayEffect(string effect, Vector3 pos)
    {
        Transform chosen = GetEffect(effect);
        chosen.position = pos;
        chosen.gameObject.SetActive(true);
    }

    public Transform AssignEffect(string effect, Transform parent)
    {
        Transform chosen = GetEffect(effect);
        chosen.parent = parent;
        chosen.localPosition = Vector3.zero;
        chosen.gameObject.SetActive(true);
        return chosen;
    }

    public void UnassignEffect(string effect, GameObject effectObject, Transform parent)
    {
        effectObject.SetActive(false);
        effectObject.transform.parent = GetEffect(effect);
        effectObject.transform.localPosition = Vector3.zero;
    }
}
