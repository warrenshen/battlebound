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

    private void Awake()
    {
        base.Awake();

        this.cardPrefab = Resources.Load("Prefabs/Card") as GameObject;

        this.creatureNameToPrefab = new Dictionary<string, GameObject>();

        string codexPath = Application.dataPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "codex.txt";
        this.cardNametoTemplate = CodexHelper.ParseFile(codexPath);

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

        foreach (string creatureName in Card.CREATURE_CARD_NAMES)
        {
            CreatureCard creatureCard = new CreatureCard("", creatureName, 0);
            string summonPrefabPath = creatureCard.GetSummonPrefab();
            GameObject prefab = Resources.Load(summonPrefabPath) as GameObject;
            this.creatureNameToPrefab[creatureName] = prefab;
        }
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
