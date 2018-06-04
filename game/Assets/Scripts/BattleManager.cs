using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {
    private Deck playerDeck;
    private Hand playerHand;

	// Use this for initialization
	void Awake () {
        playerDeck = GetDeck();
        playerHand = new Hand(playerDeck, 4);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private Deck GetDeck() {
        string deckName = PlayerPrefs.GetString("selectedDeck", "DeckA");

        //do manually for now
        List<Card> cards = new List<Card>();
        cards.Add(new Card("C1", "Direhorn Hatchling", Card.CardType.Creature, 5, 3, 6, "Direhorn_Hatchling"));
        cards.Add(new Card("C2", "Fiery War Axe", Card.CardType.Weapon, 3, 3, 2, "Fiery_War_Axe"));
        cards.Add(new Card("C3", "Crushing Walls", Card.CardType.Spell, 7, 3, 2, "Crushing_Walls"));
        cards.Add(new Card("C4", "Fiery War Axe", Card.CardType.Weapon, 3, 3, 2, "Fiery_War_Axe"));
        cards.Add(new Card("C1", "Direhorn Hatchling", Card.CardType.Creature, 5, 3, 6, "Direhorn_Hatchling"));
        cards.Add(new Card("C2", "Fiery War Axe", Card.CardType.Weapon, 3, 3, 2, "Fiery_War_Axe"));
        cards.Add(new Card("C3", "Crushing Walls", Card.CardType.Spell, 7, 3, 2, "Crushing_Walls"));


        Deck chosen = new Deck(deckName, cards, Deck.DeckClass.Hunter);
        return chosen;
    }
}
