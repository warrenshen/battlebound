using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSingleton : Singleton<ResourceSingleton>
{
    private GameObject cardPrefab;

    private Dictionary<string, GameObject> creatureNameToPrefab;
    private Dictionary<string, GameObject> effectNameToPrefab;

    private Dictionary<string, CardTemplate> cardNametoTemplate;

    private Dictionary<string, Texture2D> imageNameToTexture;

    private Stack<HyperCard.Card> cardVisualPool;
    private static int CARD_POOL_SIZE = 90;

    private void Awake()
    {
        base.Awake();

        this.cardPrefab = Resources.Load("Prefabs/Card") as GameObject;
        this.cardVisualPool = new Stack<HyperCard.Card>();
        for (int i = 0; i < CARD_POOL_SIZE; i++)
        {
            GameObject created = Instantiate(this.cardPrefab, transform.position, Quaternion.identity);
            created.transform.parent = this.transform;
            created.SetActive(false);
            HyperCard.Card cardVisual = created.GetComponent<HyperCard.Card>();
            cardVisualPool.Push(cardVisual);
        }

        this.creatureNameToPrefab = new Dictionary<string, GameObject>();
        this.effectNameToPrefab = new Dictionary<string, GameObject>();
        this.cardNametoTemplate = new Dictionary<string, CardTemplate>();

        TextAsset codexText = (TextAsset)Resources.Load("codex", typeof(TextAsset));
        string codexString = codexText.text;
        List<string> codexJsons = new List<string>(codexString.Split('\n'));

        foreach (string codexJson in codexJsons)
        {
            if (codexJson.Length > 0)
            {
                CardTemplate cardTemplate = JsonUtility.FromJson<CardTemplate>(codexJson);
                cardNametoTemplate.Add(cardTemplate.name, cardTemplate);
            }
        }

        this.imageNameToTexture = new Dictionary<string, Texture2D>();

        foreach (string cardName in this.cardNametoTemplate.Keys)
        {
            CardTemplate cardTemplate = this.cardNametoTemplate[cardName];
            string frontImage = cardTemplate.frontImage;
            string backImage = cardTemplate.backImage;
            string effectName = cardTemplate.effectPrefab;
            this.imageNameToTexture[frontImage] = Resources.Load(frontImage) as Texture2D;
            this.imageNameToTexture[backImage] = Resources.Load(backImage) as Texture2D;
            this.effectNameToPrefab[effectName] = Resources.Load(effectName) as GameObject;
        }

        foreach (string creatureName in Card.CARD_NAMES_CREATURE)
        {
            CreatureCard creatureCard = new CreatureCard("", creatureName, 0);
            string summonPrefabPath = creatureCard.GetSummonPrefab();
            GameObject prefab = Resources.Load(summonPrefabPath) as GameObject;
            this.creatureNameToPrefab[creatureName] = prefab;
        }
    }

    public HyperCard.Card TakeCardFromPool()
    {
        if (cardVisualPool.Count <= 0)
        {
            GameObject created = Instantiate(this.cardPrefab, transform.position, Quaternion.identity);
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

    public GameObject GetCreaturePrefabByName(string creatureName)
    {
        if (!this.creatureNameToPrefab.ContainsKey(creatureName))
        {
            Debug.LogError(string.Format("Creature name {0} does not exist in resource cache.", creatureName));
            return null;
        }

        return this.creatureNameToPrefab[creatureName];
    }

    public GameObject GetEffectPrefabByName(string effectName)
    {
        if (!this.effectNameToPrefab.ContainsKey(effectName))
        {
            Debug.LogError(string.Format("Effect name {0} does not exist in resource cache.", effectName));
            return null;
        }

        return this.effectNameToPrefab[effectName];
    }

    public CardTemplate GetCardTemplateByName(string cardName)
    {
        if (!this.cardNametoTemplate.ContainsKey(cardName))
        {
            Debug.LogError(string.Format("Card name {0} does not exist in resource cache.", cardName));
            return null;
        }

        return this.cardNametoTemplate[cardName];
    }

    public Texture2D GetImageTextureByName(string imageName)
    {
        if (imageName == null)
        {
            return null;
        }

        if (!this.imageNameToTexture.ContainsKey(imageName))
        {
            Debug.LogError(string.Format("Image name {0} does not exist in resource cache.", imageName));
            return null;
        }

        return this.imageNameToTexture[imageName];
    }

    public GameObject GetCardPrefab()
    {
        return this.cardPrefab;
    }
}
