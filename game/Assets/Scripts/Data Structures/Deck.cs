using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Deck {
	private string name;
	public string Name => name;

    [SerializeField]
	private List<Card> cards;
	public List<Card> Cards => cards;

	public enum DeckClass : byte { Warrior, Hunter }; //TODO: add more
	private DeckClass hero;
	public DeckClass Hero => Hero;

    public Deck(string name, List<Card> cards, DeckClass hero, Player owner = null) {
		this.name = name;
		this.cards = cards;
		this.hero = hero;

        //todo: SHUFFLE
	}

    public void SetCards(List<Card> cards) {
        this.cards = cards;
    }

    public void AddCard(Card card) {
        this.cards.Add(card);
    }

    public void RemoveCard(Card card) {
        //Debug.Log(this.cards.Remove(card));
        this.cards.Remove(card);
    }

    public override string ToString()
    {
        string output = "";
        foreach(Card card in cards) {
            output += card.Name + ", ";
        }
        return output.Substring(0, output.Length - 1);
    }
}
