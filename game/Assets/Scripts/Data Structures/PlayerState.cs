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
	private int handSize;
	public int HandSize => handSize;

	[SerializeField]
    private List<Card> hand;
    public List<Card> Hand => hand;

	[SerializeField]
	private int deckSize;
	public int DeckSize => deckSize;

	[SerializeField]
    private BoardCreature[] field;
    public BoardCreature[] Field => field;
    
	public void SetHasTurn(int hasTurn)
	{
		this.hasTurn = hasTurn;
	}

    public void SetManaCurrent(int manaCurrent)
	{
		this.manaCurrent = manaCurrent;
	}

    public void SetManaMax(int manaMax)
	{
		this.manaCurrent = manaMax;
	}

    public void SetHealth(int health)
	{
		this.health = health;
	}

    public void SetArmor(int armor)
	{
		this.armor = armor;
	}

    public void SetHandSize(int handSize)
	{
		this.handSize = handSize;
	}

    public void SetDeckSize(int deckSize)
	{
		this.deckSize = deckSize;
	}

    public bool Equals(PlayerState other)
	{
		// TODO: add in hand and field.
        return this.hasTurn == other.HasTurn &&
               this.manaCurrent == other.ManaCurrent &&
               this.manaMax == other.ManaMax &&
               this.health == other.Health &&
               this.armor == other.Armor &&
               this.handSize == other.HandSize &&
               this.deckSize == other.DeckSize;
    }
}
