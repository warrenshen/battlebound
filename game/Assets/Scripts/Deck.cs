using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Deck {
	private string name;
	public string Name => name;

	private List<Card> cards;
	public List<Card> Cards => cards;

	public enum DeckClass : byte { Warrior, Hunter }; //TODO: add more
	private DeckClass hero;
	public DeckClass Hero => Hero;

	public Deck(string name, List<Card> cards, DeckClass hero) {
		this.name = name;
		this.cards = cards;
		this.hero = hero;
	}
}
