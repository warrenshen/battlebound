using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSingleton : Singleton<ResourceSingleton>
{
    private Dictionary<string, GameObject> nameToPrefab;

    private Dictionary<string, GameObject> effectNameToPrefab;

    private Dictionary<string, CardTemplate> cardNametoTemplate;

    private Dictionary<string, Texture2D> imageNameToTexture;

    private Dictionary<string, Sprite> imageNameToSprite;

    private new void Awake()
    {
        base.Awake();

        this.nameToPrefab = new Dictionary<string, GameObject>();
        this.effectNameToPrefab = new Dictionary<string, GameObject>();
        this.cardNametoTemplate = new Dictionary<string, CardTemplate>();

        TextAsset codexText = (TextAsset)Resources.Load("codex", typeof(TextAsset));
        string codexString = codexText.text;
        List<string> codexJsons = new List<string>(codexString.Split('\n'));

        //LanguageUtility.Instance(); //just to begin the loading process

        foreach (string codexJson in codexJsons)
        {
            if (codexJson.Length > 0)
            {
                CardTemplate cardTemplate = JsonUtility.FromJson<CardTemplate>(codexJson);
                cardNametoTemplate.Add(cardTemplate.name, cardTemplate);
            }
        }

        this.imageNameToTexture = new Dictionary<string, Texture2D>();
        this.imageNameToSprite = new Dictionary<string, Sprite>();

        foreach (string cardName in this.cardNametoTemplate.Keys)
        {
            CardTemplate cardTemplate = this.cardNametoTemplate[cardName];
            string frontImage = cardTemplate.frontImage;
            string backImage = cardTemplate.backImage;
            string effectName = cardTemplate.effectPrefab;

            Texture2D frontTexture = Resources.Load(frontImage) as Texture2D;
            Texture2D backTexture = Resources.Load(backImage) as Texture2D;
            this.imageNameToTexture[frontImage] = frontTexture;
            this.imageNameToTexture[backImage] = backTexture;
            this.effectNameToPrefab[effectName] = Resources.Load(effectName) as GameObject;

            if (frontTexture != null)
            {
                this.imageNameToSprite[frontImage] = Sprite.Create(
                    frontTexture,
                    new Rect(0.0f, 0.0f, frontTexture.width, frontTexture.height),
                    new Vector2(0.5f, 0.5f),
                    100.0f
                );
            }
            else
            {
                this.imageNameToSprite[frontImage] = null;
            }
            if (backTexture != null)
            {
                this.imageNameToSprite[backImage] = Sprite.Create(
                    backTexture,
                    new Rect(0.0f, 0.0f, backTexture.width, backTexture.height),
                    new Vector2(0.5f, 0.5f),
                    100.0f
                );
            }
            else
            {
                this.imageNameToSprite[backImage] = null;
            }
        }


        foreach (string creatureName in Card.CARD_NAMES_CREATURE)
        {
            CreatureCard creatureCard = new CreatureCard("", creatureName, 0);
            string summonPrefabPath = creatureCard.GetSummonPrefab();
            GameObject prefab = Resources.Load(summonPrefabPath) as GameObject;
            this.nameToPrefab[creatureName] = prefab;
        }

        foreach (string structureName in Card.CARD_NAMES_STRUCTURES)
        {
            StructureCard structureCard = new StructureCard("", structureName, 0);
            string summonPrefabPath = structureCard.GetSummonPrefab();
            GameObject prefab = Resources.Load(summonPrefabPath) as GameObject;
            this.nameToPrefab[structureName] = prefab;
        }
    }

    public GameObject GetPrefabByName(string creatureName)
    {
        if (!this.nameToPrefab.ContainsKey(creatureName))
        {
            Debug.LogError(string.Format("Creature name {0} does not exist in resource cache.", creatureName));
            return null;
        }
        return this.nameToPrefab[creatureName];
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

    public Sprite GetSpriteByName(string imageName)
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

        return this.imageNameToSprite[imageName];
    }
}
