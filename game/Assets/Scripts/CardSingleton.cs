using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSingleton : Singleton<CardSingleton>
{
    private GameObject cardPrefab;

    private Stack<HyperCard.Card> cardVisualPool;
    private static int CARD_POOL_SIZE = 90;

    private Dictionary<string, GameObject> summonPool;

    private new void Awake()
    {
        base.Awake();

        this.cardPrefab = Resources.Load("Prefabs/Card") as GameObject;

        this.cardVisualPool = new Stack<HyperCard.Card>();
        Transform cardPoolRoot = new GameObject("Card Pool").transform;
        cardPoolRoot.transform.parent = this.transform;

        for (int i = 0; i < CARD_POOL_SIZE; i++)
        {
            GameObject created = Instantiate(this.GetCardPrefab(), transform.position, Quaternion.identity);
            created.transform.parent = cardPoolRoot;
            created.SetActive(false);
            HyperCard.Card cardVisual = created.GetComponent<HyperCard.Card>();
            cardVisualPool.Push(cardVisual);
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

    public GameObject GetCardPrefab()
    {
        return this.cardPrefab;
    }
}
