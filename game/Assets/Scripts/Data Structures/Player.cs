using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

[System.Serializable]
public class Player
{
    private string name;
    public string Name => name;

    private Deck deck;
    public Deck Deck => deck;

    private Hand hand;
    public Hand Hand => hand;

    private int mana;
    public int Mana => mana;

    private int maxMana;
    public int MaxMana => maxMana;

    private Board.PlayingField field;
    public Board.PlayingField Field => Field;

	private bool hasTurn;
	public bool HasTurn => hasTurn;

    private int health;
    public int Health => health;

    private int armor;
    public int Armor => armor;

    public Player(string name)
    {
		this.name = name;
        this.hasTurn = false;
        this.health = 30;
        this.deck = GetDeck();
        this.hand = new Hand(deck, 7, this.name);
        this.mana = 10;
        this.maxMana = 10;

		Debug.Log(GeneratePlayerState());
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
        cards.Add(new CreatureCard("C1", "Direhorn Hatchling", 5, "Direhorn_Hatchling", 3, 6, owner: this, abilities : new List<String>(){"shielded"}));
        cards.Add(new WeaponCard("C2", "Fiery War Axe", 3, "Fiery_War_Axe", 3, 2, owner: this));
        cards.Add(new SpellCard("C6", "Lightning Bolt", 1, "Lightning_Bolt", targeted: true, owner: this));
        cards.Add(new WeaponCard("C4", "Fiery War Axe", 3, "Fiery_War_Axe", 3, 2, owner: this));
        cards.Add(new CreatureCard("C1", "Direhorn Hatchling", 5, "Direhorn_Hatchling", 3, 6, owner: this, abilities: new List<String>() { "taunt" }));
        cards.Add(new WeaponCard("C2", "Fiery War Axe", 3, "Fiery_War_Axe", 3, 2, owner: this));
        cards.Add(new SpellCard("C6", "Lightning Bolt", 1, "Lightning_Bolt", targeted: true, owner: this));
        cards.Add(new SpellCard("C6", "Lightning Bolt", 1, "Lightning_Bolt", targeted: true, owner: this));
        cards.Add(new SpellCard("C6", "Lightning Bolt", 1, "Lightning_Bolt", targeted: true, owner: this));
        cards.Add(new SpellCard("C6", "Lightning Bolt", 1, "Lightning_Bolt", targeted: true, owner: this));

        Deck chosen = new Deck(deckName, cards, Deck.DeckClass.Hunter, owner: this);
        return chosen;
    }

	public void SetHasTurn(bool newTurn)
	{
		this.hasTurn = newTurn;
	}

    public void NewTurn() {
        //board resetting
        field.RecoverCreatures();

        maxMana = Math.Min(maxMana + 1, 10);
        mana = maxMana;
        hand.Draw(deck, 1);
        this.hasTurn = true;
        RenderMana();

        //placeholder indicator
        Vector3 targetPosition = GameObject.Find(name + " Hand").transform.position;
        GameObject light = GameObject.Find("Point Light");
        LeanTween.move(light, new Vector3(targetPosition.x, targetPosition.y, light.transform.position.z), 0.4f).setEaseOutQuart();
    }

	public PlayerState GeneratePlayerState()
	{
		PlayerState playerState = new PlayerState();

		playerState.SetHasTurn(this.hasTurn ? 1 : 0);
		playerState.SetManaCurrent(this.mana);
		playerState.SetManaMax(this.maxMana);
		playerState.SetHealth(this.health);
		playerState.SetArmor(this.Armor);
		playerState.SetHandSize(this.Hand.Size());
		playerState.SetDeckSize(this.Deck.Size());

		return playerState;
	}

    public bool TakeDamage(int amount) {
        this.health -= amount;
        return this.health > 0;
    }
}
