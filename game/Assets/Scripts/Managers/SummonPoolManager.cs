using System;
using System.Collections.Generic;
using UnityEngine;

public class SummonPoolManager : MonoBehaviour
{
    public static SummonPoolManager Instance { get; private set; }
    private Dictionary<string, GameObject> summonPool;

    private void Awake()
    {
        Instance = this;
        this.summonPool = new Dictionary<string, GameObject>();
    }

    private void Start()
    {
        Transform summonPoolRoot = new GameObject("Summon Pool").transform;
        summonPoolRoot.transform.parent = this.transform;

        foreach (string creaturePrefabName in Card.CARD_NAMES_CREATURE)
        {
            if (ResourceSingleton.Instance.GetPrefabByName(creaturePrefabName) == null)
            {
                Debug.LogError(string.Format("No prefab for creature: ", creaturePrefabName));
                continue;
            }

            GameObject summon = GameObject.Instantiate(
                ResourceSingleton.Instance.GetPrefabByName(creaturePrefabName),
                summonPoolRoot
            );
            summon.transform.parent = summonPoolRoot;
            AnimateIdle animateIdle = summon.AddComponent<AnimateIdle>();
            summon.SetActive(false);
            this.summonPool.Add(creaturePrefabName, summon);
        }
    }

    public GameObject GetSummonFromPool(string name)
    {
        if (this.summonPool.ContainsKey(name))
        {
            return this.summonPool[name];
        }
        else
        {
            Debug.LogError(String.Format("Could not summon {0} from summon pool. {1} pooled.", name, summonPool.Count));
            return null;
        }
    }
}
