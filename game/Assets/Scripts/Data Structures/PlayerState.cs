using System.Collections.Generic;
using UnityEngine;
using System;

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
    private BoardCreature[] field;
    public BoardCreature[] Field => field;

	[SerializeField]
	private int handSize;
	public int HandSize => handSize;

	[SerializeField]
	private int deckSize;
	public int DeckSize => deckSize;

	[SerializeField]
	private List<Card> hand;
	public List<Card> Hand => hand;


    public PlayerState(Player player) {
        this.Sync(player);
    }

    public void Sync(Player player) {
        this.hasTurn = Convert.ToInt32(player.hasTurn);
        this.manaCurrent = player.Mana;
        this.manaMax = player.MaxMana;
        this.health = player.Health;
        this.armor = player.Armor;
        this.field = player.Field.GetCreatures();
        this.handSize = player.Hand.Size();
        this.deckSize = player.Deck.Size();
    }

    public bool Equals(PlayerState other) {
        return this.hasTurn == other.HasTurn &&
               this.manaCurrent == other.ManaCurrent &&
               this.manaMax == other.ManaMax &&
               this.health == other.Health &&
               this.armor == other.Armor &&
               this.field == other.Field &&
               this.handSize == other.HandSize &&
               this.deckSize == other.DeckSize;
    }
}
