using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Hand {
    [SerializeField]
    private List<Card> cards;

    private string name;

    public Hand(Deck deck, int size, string name="Player")
	{
        this.cards = new List<Card>();
        this.name = name;
        Draw(deck, size);
    }

	public Hand(string name, List<Card> cards)
	{
		this.name = name;
		this.cards = cards;
		CreateCardObjects(this.cards);
		RepositionCards();
	}

    private int Draw(Deck deck) {
        if (deck.Cards.Count < 1)
            return 1; //amount fatigue
        
        Card drawn = deck.Cards[0];
        deck.Cards.RemoveAt(0);
        cards.Add(drawn);
        CreateCardObjects(drawn);
        return 0;
    }

    public Card GetCardById(string cardId)
	{
        foreach (Card card in this.cards)
		{
			if (card.Id == cardId)
			{
				return card;
			}
        }
        return null;
    }

    public int Draw(Deck deck, int amount) {
        int fatigue = 0;

        while(amount > 0) {
            fatigue += Draw(deck);
            amount--;
        }
        RepositionCards();
        return fatigue;
    }

    public int AddDrawnCard(Card drawnCard)
	{
		this.cards.Add(drawnCard);
		CreateCardObjects(drawnCard);
		RepositionCards();
		return 1;
	}

    public void Discard(int count) {
        
    }

    public void RemoveAndReposition(CardObject cardObject) {
        //cardObjects.Remove(cardObject);
        cards.Remove(cardObject.card);
        RepositionCards();
    }

    private void CreateCardObjects(Card card) {
        GameObject created = new GameObject(card.Name);
        CardObject wrapper = created.AddComponent<CardObject>();
        wrapper.InitializeCard(card);
        created.transform.parent = GameObject.Find(name + " Hand").transform;
    }

    private void CreateCardObjects(List<Card> toCreate)
    {
        foreach (Card card in toCreate)
        {
            CreateCardObjects(card);
        }
    }

    public void RepositionCards()
	{
        int size = cards.Count;
        //if no cards, return
        if (size <= 0)
            return;

        float cardWidth = cards[0].wrapper.transform.GetComponent<BoxCollider>().size.x + 0.24f - 0.15f * size;
        float rotation_x = 70f;

        for (int k = 0; k < size; k++)
        {
            int pos = -((size - 1) / 2) + k;
            float vOffset = -0.15f * Mathf.Abs(pos) + Random.Range(-0.05f, 0.05f);
            cards[k].wrapper.Renderer.sortingOrder = 10 + k;

            Vector3 adjustedPos = new Vector3(pos * cardWidth/1.11f, 0.2f * pos, vOffset);
            float tweenTime = 0.1f;
            LeanTween.moveLocal(cards[k].wrapper.gameObject, adjustedPos, tweenTime);
            LeanTween.rotateLocal(cards[k].wrapper.gameObject, new Vector3(rotation_x, pos * 4f, 0), tweenTime);
        }
    }

	public Card GetCardByIndex(int index)
	{
		return this.cards.ElementAt(index);
	}

    public int Size() {
        return cards.Count;
    }
}
