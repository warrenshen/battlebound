using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

[System.Serializable]
public class Player {
    private string name;
    private Deck deck;

    [SerializeField]
    private Hand hand;
    public Hand Hand => hand;

    private int mana;
    public int Mana => mana;
    private int maxMana;
    public int MaxMana => maxMana;

    private Board.PlayingField field;

    public Player(string name)
    {
        this.name = name;
        deck = GetDeck();
        hand = new Hand(deck, 7, this.name);
        mana = 10;
        maxMana = 10;
    }

    public void PlayCard(CardObject cardObject) {
        hand.Remove(cardObject);
        mana -= cardObject.card.Cost;
        RenderMana();
    }

    private void RenderMana() {
        TextMeshPro manaText = GameObject.Find(name + " Mana").GetComponent<TextMeshPro>();
        manaText.text = String.Format("{0}/{1}", mana.ToString(), maxMana.ToString());
    }

    public void SetPlayingField(Board.PlayingField field) {
        this.field = field;
    }

    private Deck GetDeck()
    {
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
