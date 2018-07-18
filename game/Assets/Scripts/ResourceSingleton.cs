using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSingleton : Singleton<ResourceSingleton>
{
    private Dictionary<string, GameObject> creatureNameToPrefab;

    private Dictionary<string, CardTemplate> cardNametoTemplate;

    private void Awake()
    {
        base.Awake();

        this.creatureNameToPrefab = new Dictionary<string, GameObject>();

        string codexPath = Application.dataPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "codex.txt";
        this.cardNametoTemplate = CodexHelper.ParseFile(codexPath);

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
            Debug.LogError("Creature name does not exist in resource cache.");
            return null;
        }

        return this.creatureNameToPrefab[creatureName];
    }

    public CardTemplate GetCardTemplateByName(string cardName)
    {
        if (!this.cardNametoTemplate.ContainsKey(cardName))
        {
            Debug.LogError(string.Format("Creature name {0} does not exist in resource cache.", cardName));
            return null;
        }

        return this.cardNametoTemplate[cardName];
    }
}
