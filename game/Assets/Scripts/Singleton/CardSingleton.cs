using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class CardSingleton : Singleton<CardSingleton>
{
    private GameObject cardPrefab;

    private Stack<HyperCard.Card> cardVisualPool;
    private static int CARD_POOL_SIZE = 10;

    [SerializeField]
    private GameObject creatureCardObjectPrefab;
    public GameObject CreatureCardObjectPrefab => creatureCardObjectPrefab;
    [SerializeField]
    private GameObject spellCardObjectPrefab;
    public GameObject SpellCardObjectPrefab => spellCardObjectPrefab;
    [SerializeField]
    private GameObject structureCardObjectPrefab;
    public GameObject StructureCardObjectPrefab => structureCardObjectPrefab;
    [SerializeField]
    private GameObject weaponCardObjectPrefab;
    public GameObject WeaponCardObjectPrefab => weaponCardObjectPrefab;

    [SerializeField]
    private Texture2D[] gems;
    public Texture2D[] Gems => gems;

    [SerializeField]
    private TMP_FontAsset[] fontAssets;
    public TMP_FontAsset[] FontAssets => fontAssets;


    private new void Awake()
    {
        base.Awake();

        this.cardVisualPool = new Stack<HyperCard.Card>();
        this.cardPrefab = Resources.Load("Prefabs/Creature Card") as GameObject;

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

    public HyperCard.Card TakeCardFromPool()
    {
        HyperCard.Card chosen;

        if (cardVisualPool.Count <= 0)
        {
            GameObject created = Instantiate(this.GetCardPrefab(), transform.position, Quaternion.identity);
            chosen = created.GetComponent<HyperCard.Card>();
        }
        else
        {
            chosen = cardVisualPool.Pop();
        }

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

    public GameObject GetCardPrefab()
    {
        return this.cardPrefab;
    }
}
