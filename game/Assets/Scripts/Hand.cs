using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand {
    private List<Card> cards;
    private List<CardObject> cardObjects;

    public Hand(Deck deck, int size) {
        cards = new List<Card>();
        Draw(deck, size);
        Visualize();
    }

    public Hand(List<Card> cards) {
        this.cards = cards;
        Visualize();
    }

    public void Visualize() {
        CreateCards();
        RepositionCards();
    }

    public int Draw(Deck deck) {
        if (deck.Cards.Count < 1)
            return 1; //amount overdraw
        
        Card drawn = deck.Cards[0];
        deck.Cards.RemoveAt(0);
        cards.Add(drawn);

        return 0;
    }

    public int Draw(Deck deck, int amount) {
        int overdraw = 0;

        while(amount > 0) {
            overdraw += Draw(deck);
            amount--;
        }
        return overdraw;
    }

    public void Discard(int count) {
        
    }

    private void CreateCards()
    {
        cardObjects = new List<CardObject>();
        foreach (Card card in cards)
        {
            GameObject created = new GameObject(card.Name);
            //created.transform.parent = collectionObject.transform;
            CardObject wrapper = created.AddComponent<CardObject>();
            wrapper.InitializeCard(card);
            cardObjects.Add(wrapper);
        }
    }

    private void RepositionCards() {
        int index = 0;
        foreach(CardObject elem in cardObjects) {
            elem.transform.localPosition = new Vector3(index * 2f, 0f, 0f);
            ++index;
        }
    }
}
