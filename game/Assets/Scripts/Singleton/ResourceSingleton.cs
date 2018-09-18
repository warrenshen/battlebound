using System.Collections;
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

        StartCoroutine("LoadResourcesAsync");

#if !UNITY_EDITOR
        StartCoroutine("LoadPrefabsAsync");
#endif
        //foreach (string creatureName in Card.CARD_NAMES_CREATURE)
        //{
        //    CreatureCard creatureCard = new CreatureCard("", creatureName, 0);
        //    string summonPrefabPath = creatureCard.GetSummonPrefab();
        //    GameObject prefab = Resources.Load(summonPrefabPath) as GameObject;
        //    this.nameToPrefab[creatureName] = prefab;
        //}

        //foreach (string structureName in Card.CARD_NAMES_STRUCTURES)
        //{
        //    StructureCard structureCard = new StructureCard("", structureName, 0);
        //    string summonPrefabPath = structureCard.GetSummonPrefab();
        //    GameObject prefab = Resources.Load(summonPrefabPath) as GameObject;
        //    this.nameToPrefab[structureName] = prefab;
        //}
    }

    private IEnumerator LoadResourcesAsync()
    {
        Application.backgroundLoadingPriority = ThreadPriority.Low;

        foreach (string cardName in this.cardNametoTemplate.Keys)
        {
            CardTemplate cardTemplate = this.cardNametoTemplate[cardName];
            string effectName = cardTemplate.effectPrefab;
            string frontImage = cardTemplate.frontImage;
            string backImage = cardTemplate.backImage;

            ResourceRequest resourceRequest;

            if (string.IsNullOrEmpty(effectName))
            {
                if (FlagHelper.IsLogVerbose())
                {
                    //Debug.LogWarning(string.Format("Effect name for card {0} does not exist.", cardName));
                }
            }
            else
            {
                resourceRequest = Resources.LoadAsync(effectName);
                yield return resourceRequest;
                this.effectNameToPrefab[effectName] = (GameObject)resourceRequest.asset;
            }

            if (string.IsNullOrEmpty(frontImage))
            {
                if (FlagHelper.IsLogVerbose())
                {
                    Debug.LogWarning(string.Format("Front image for card {0} does not exist.", cardName));
                }
            }
            else
            {
                resourceRequest = Resources.LoadAsync(frontImage);
                yield return resourceRequest;
                Texture2D frontTexture = (Texture2D)resourceRequest.asset;
                this.imageNameToTexture[frontImage] = frontTexture;
                CreateSprite(frontImage, frontTexture);
            }

            if (string.IsNullOrEmpty(backImage))
            {
                if (FlagHelper.IsLogVerbose())
                {
                    Debug.LogWarning(string.Format("Back image for card {0} does not exist.", cardName));
                }
            }
            else
            {
                resourceRequest = Resources.LoadAsync(backImage);
                yield return resourceRequest;
                Texture2D backTexture = (Texture2D)resourceRequest.asset;
                this.imageNameToTexture[backImage] = backTexture;
                CreateSprite(backImage, backTexture);
            }

            yield return new WaitForSeconds(0.1f);
        }

        yield return null;
    }

    private void CreateSprite(string imageName, Texture2D imageTexture)
    {
        if (imageTexture != null)
        {
            this.imageNameToSprite[imageName] = Sprite.Create(
                imageTexture,
                new Rect(0.0f, 0.0f, imageTexture.width, imageTexture.height),
                new Vector2(0.5f, 0.5f),
                100.0f,
                0,
                SpriteMeshType.FullRect
            );
        }
        else
        {
            this.imageNameToSprite[imageName] = null;
        }
    }

    private IEnumerator LoadPrefabsAsync()
    {
        Application.backgroundLoadingPriority = ThreadPriority.Low;

        foreach (string creatureName in Card.CARD_NAMES_CREATURE)
        {
            CreatureCard creatureCard = new CreatureCard("", creatureName, 0);
            string summonPrefabPath = creatureCard.GetSummonPrefab();

            if (string.IsNullOrEmpty(summonPrefabPath))
            {
                Debug.LogWarning("Summon prefab path does not exist.");
                continue;
            }

            ResourceRequest resourceRequest = Resources.LoadAsync(summonPrefabPath);
            yield return resourceRequest;
            this.nameToPrefab[creatureName] = (GameObject)resourceRequest.asset;

            yield return new WaitForSeconds(0.1f);
        }

        yield return null;
    }

    public GameObject GetPrefabByName(string creatureName)
    {
        creatureName = creatureName.Replace(",", "~"); //to-do: find a less hacky way to do this.., or just update server
        return GetPrefabByNameLazy(creatureName);
    }

    private GameObject GetPrefabByNameLazy(string creatureName)
    {
        if (!this.nameToPrefab.ContainsKey(creatureName))
        {
            CreatureCard creatureCard = new CreatureCard("", creatureName, 0);
            string summonPrefabPath = creatureCard.GetSummonPrefab();

            GameObject summonPrefab = (GameObject)Resources.Load(summonPrefabPath);
            this.nameToPrefab[creatureName] = summonPrefab;
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
        cardName = cardName.Replace(",", "~"); //to-do: find a less hacky way to do this.., or just update server
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
        return GetImageTextureByNameLazy(imageName);
    }

    private Texture2D GetImageTextureByNameLazy(string imageName)
    {
        if (!this.imageNameToTexture.ContainsKey(imageName))
        {
            Texture2D imageTexture = Resources.Load(imageName) as Texture2D;
            this.imageNameToTexture[imageName] = imageTexture;
        }
        return this.imageNameToTexture[imageName];
    }

    public Sprite GetSpriteByName(string imageName)
    {
        if (imageName == null)
        {
            return null;
        }
        return GetSpriteByNameLazy(imageName);
    }

    private Sprite GetSpriteByNameLazy(string imageName)
    {
        if (!this.imageNameToSprite.ContainsKey(imageName))
        {
            Texture2D imageTexture = GetImageTextureByName(imageName);
            CreateSprite(imageName, imageTexture);
        }
        return this.imageNameToSprite[imageName];
    }
}
