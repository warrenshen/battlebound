using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collection : MonoBehaviour {
    public List<Card> collection;

    private Camera cam;

    public void Awake()
    {
        //jank json test, for generating list of json objects
        List<Card> cards = new List<Card>();
        cards.Add(new Card("Direhorn Hatchling", Card.Type.Creature, 5, "Direhorn_Hatchling(55524)"));
        cards.Add(new Card("Fiery War Axe", Card.Type.Weapon, 3, "Fiery_War_Axe(632)_Gold"));
        cards.Add(new Card("Armorsmith", Card.Type.Creature, 2, "Armorsmith(644)"));
        cards.Add(new Card("Greater Mithril Spellstone", Card.Type.Spell, 7, "Greater_Mithril_Spellstone(76874)"));
        cards.Add(new Card("Grommash Hellscream", Card.Type.Creature, 8, "Grommash_Hellscream(643)"));
        cards.Add(new Card("Slam", Card.Type.Weapon, 2, "Slam(215)"));
        //hunter cards
        cards.Add(new Card("To My Side", Card.Type.Spell, 6, "To_My_Side!(76970)"));
        cards.Add(new Card("Play Dead", Card.Type.Spell, 6, "Play_Dead(62891)"));
        cards.Add(new Card("Crackling Razormaw", Card.Type.Creature, 2, "Crackling_Razormaw(55500)"));
        cards.Add(new Card("Huffer", Card.Type.Creature, 3, "Huffer(369)"));
        cards.Add(new Card("Eaglehorn Bow", Card.Type.Weapon, 3, "Eaglehorn_Bow(363)"));


        //ping server for collection json
        string json = JsonList.ToJson(cards);
        ParseCards(json);
    }

    // Use this for initialization
    void Start () {
        cam = Camera.main;

        GameObject root = new GameObject("Collection");
        CreateCards(root);
	}

    void ParseCards(string json) {
        Debug.Log(json);
        collection = JsonList.FromJson<Card>(json);
    }

    void CreateCards(GameObject root) {
        int index = 0;
        int rowSize = 4;

        Vector3 topLeft = cam.ViewportToWorldPoint(new Vector3(0.115f, 0.81f, transform.position.z));
        Vector3 horizontalOffset = new Vector3(2.7f, 0f, 0f);
        Vector3 verticalOffset = new Vector3(0f, -3.8f, 0f);

        foreach (Card card in collection)
        {
            GameObject created = new GameObject(card.name);
            created.transform.parent = root.transform;
            CardObject cWrapper = created.AddComponent<CardObject>();
            cWrapper.InitializeCard(card);

            created.transform.position = topLeft + index % rowSize * horizontalOffset + index / rowSize * verticalOffset;
            index++;
        }
    }
}
