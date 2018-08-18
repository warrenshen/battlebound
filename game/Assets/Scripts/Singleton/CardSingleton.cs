using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class CardSingleton : Singleton<CardSingleton>
{
    private GameObject cardPrefab;

    private Stack<HyperCard.Card> cardVisualPool;
    private static int CARD_POOL_SIZE = 30;

    [SerializeField]
    private Dictionary<string, GameObject> summonPool;

    [SerializeField]
    private Texture2D[] gems;
    public Texture2D[] Gems => gems;

    [SerializeField]
    private TMP_FontAsset[] fontAssets;
    public TMP_FontAsset[] FontAssets => fontAssets;


    private new void Awake()
    {
        base.Awake();

        this.summonPool = new Dictionary<string, GameObject>();
        this.cardVisualPool = new Stack<HyperCard.Card>();

        this.cardPrefab = Resources.Load("Prefabs/Card") as GameObject;

        Transform cardPoolRoot = new GameObject("Card Pool").transform;
        cardPoolRoot.transform.parent = this.transform;

        LanguageUtility.Instance(); //just to begin the loading process

        for (int i = 0; i < CARD_POOL_SIZE; i++)
        {
            GameObject created = Instantiate(GetCardPrefab(), transform.position, Quaternion.identity);
            created.transform.parent = cardPoolRoot;
            created.SetActive(false);
            HyperCard.Card cardVisual = created.GetComponent<HyperCard.Card>();
            cardVisualPool.Push(cardVisual);
        }
    }

    public void Start()
    {
        Transform summonPoolRoot = new GameObject("Summon Pool").transform;
        summonPoolRoot.transform.parent = this.transform;

        foreach (string creaturePrefabName in Card.CARD_NAMES_CREATURE)
        {
            GameObject summon = GameObject.Instantiate(
                ResourceSingleton.Instance.GetPrefabByName(creaturePrefabName),
                summonPoolRoot
            );
            summon.transform.parent = summonPoolRoot;
            AnimateIdle animateIdle = summon.AddComponent<AnimateIdle>();
            summon.SetActive(false);
            summonPool.Add(creaturePrefabName, summon);
        }
    }

    public HyperCard.Card TakeCardFromPool()
    {
        if (cardVisualPool.Count <= 0)
        {
            GameObject created = Instantiate(this.GetCardPrefab(), transform.position, Quaternion.identity);
            return created.GetComponent<HyperCard.Card>();
        }
        HyperCard.Card chosen = cardVisualPool.Pop();
        chosen.transform.localScale = BattleCardObject.CARD_VISUAL_SIZE;
        chosen.ResetParams();
        chosen.gameObject.SetActive(true);
        return chosen;
    }

    public void ReturnCardToPool(HyperCard.Card cardVisual)
    {
        cardVisual.transform.parent = this.transform;
        cardVisualPool.Push(cardVisual);
        cardVisual.gameObject.SetActive(false);
    }

    public GameObject GetSummonFromPool(string name)
    {
        return summonPool[name];
    }

    public GameObject GetCardPrefab()
    {
        return this.cardPrefab;
    }
}
