using System.Collections.Generic;
using System.Linq;
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
	private List<ChallengeCard> hand;
	public List<ChallengeCard> Hand => hand;

	[SerializeField]
	private int deckSize;
	public int DeckSize => deckSize;

	[SerializeField]
	private ChallengeCard[] field;
	public ChallengeCard[] Field => field;
    
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

    public void SetHand(List<ChallengeCard> hand)
    {
        this.hand = Hand;
    }

    public void SetDeckSize(int deckSize)
	{
		this.deckSize = deckSize;
	}
    
	public void SetField(ChallengeCard[] field)
	{
		this.field = field;
	}

    public bool Equals(PlayerState other)
	{
		if (this.hand.Count != other.Hand.Count)
		{
			return false;
		}
		else if (this.field.Length != other.Field.Length)
		{
			return false;
		}

		bool areHandsEqual = true;
		bool areFieldsEqual = true;

		for (int i = 0; i < this.hand.Count; i += 1)
		{
			areHandsEqual &= !this.hand.ElementAt(i).Equals(other.Hand.ElementAt(i));
		}
		for (int i = 0; i < this.field.Length; i += 1)
		{
			areFieldsEqual &= !this.hand.ElementAt(i).Equals(other.Hand.ElementAt(i));
		}

		if (!areHandsEqual || !areFieldsEqual)
		{
			return false;
		}

        return this.hasTurn == other.HasTurn &&
               this.manaCurrent == other.ManaCurrent &&
               this.manaMax == other.ManaMax &&
               this.health == other.Health &&
               this.armor == other.Armor &&
               this.handSize == other.HandSize &&
               this.deckSize == other.DeckSize;
    }

	[System.Serializable]
    public class ChallengeCard
	{
		[SerializeField]
		private string id;
		public string Id => id;

		[SerializeField]
		private int category;
		public int Category => category;

		[SerializeField]
		private string name;
		public string Name => name;

		[SerializeField]
		private string description;
		public string Description => description;

		[SerializeField]
		private int level;
		public int Level => level;

		[SerializeField]
		private int manaCost;
		public int ManaCost => manaCost;

		[SerializeField]
		private int health;
		public int Health => health;

		[SerializeField]
		private int healthStart;
		public int HealthStart => healthStart;

		[SerializeField]
		private int attack;
		public int Attack => attack;

		[SerializeField]
		private int attackStart;
		public int AttackStart => attackStart;

		[SerializeField]
		private int canAttack;
		public int CanAttack => canAttack;

		[SerializeField]
		private int hasShield;
		public int HasShield => hasShield;

		[SerializeField]
		private List<string> abilities;
		public List<string> Abilities => abilities;

        // TODO
		//[SerializeField]
		//private List<Buff> buffs;
		//public List<Buff> Buffs => buffs;

        public void SetId(string id)
		{
			this.id = id;
		}

		public void SetCategory(int category)
		{
			this.category = category;
		}

        public void SetName(string name)
		{
			this.name = name;
		}

        public void SetDescription(string description)
		{
			this.description = description;
		}

        public void SetLevel(int level)
		{
			this.level = level;
		}

        public void SetManaCost(int manaCost)
		{
			this.manaCost = manaCost;
		}

        public void SetHealth(int health)
		{
			this.health = health;
		}

		public void SetHealthStart(int healthStart)
		{
			this.healthStart = healthStart;
		}

        public void SetAttack(int attack)
		{
			this.attack = attack;
		}

        public void SetAttackStart(int attackStart)
		{
			this.attackStart = attackStart;
		}

        public void SetCanAttack(int canAttack)
		{
			this.canAttack = canAttack;
		}

		public void SetHasShield(int hasShield)
		{
			this.hasShield = hasShield;
		}

		public void SetAbilities(List<string> abilities)
		{
			this.abilities = abilities;
		}

		public bool Equals(ChallengeCard other)
		{
			if (this.id == "EMPTY" && other.Id == "EMPTY")
			{
				return true;	
			}

			return (
				this.id == other.Id &&
				this.category == other.Category &&
				this.name == other.Name &&
				this.name == other.Description &&
				this.level == other.Level &&
				this.manaCost == other.ManaCost &&
				this.health == other.Health &&
				this.healthStart == other.HealthStart &&
				this.attack == other.Attack &&
				this.attackStart == other.AttackStart &&
				this.canAttack == other.CanAttack &&
				this.hasShield == other.HasShield &&
				this.abilities.SequenceEqual(other.Abilities)
			);
		}
	}
}
