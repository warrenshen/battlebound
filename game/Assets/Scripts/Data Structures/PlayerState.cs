using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerState
{
	[SerializeField]
	private int hasTurn;
	public int HasTurn => hasTurn;

	[SerializeField]
	private int manaCurrent;
	public int ManaCurrent => manaCurrent;

	[SerializeField]
	private int manaMax;
	public int ManaMax => manaMax;

	[SerializeField]
	private int health;
	public int Health => health;

	[SerializeField]
	private int armor;
	public int Armor => armor;

	[SerializeField]
	private List<Card> field;
	public List<Card> Field => field;

	[SerializeField]
	private int handSize;
	public int HandSize => handSize;

	[SerializeField]
	private int deckSize;
	public int DeckSize => deckSize;

	[SerializeField]
	private List<Card> hand;
	public List<Card> Hand => hand;
}
