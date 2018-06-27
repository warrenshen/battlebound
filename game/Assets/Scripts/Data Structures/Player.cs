using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

[System.Serializable]
public class Player
{
    private string id;
    public string Id => id;

    private string name;
    public string Name => name;

    private Deck deck;
    public Deck Deck => deck;

    private int deckSize;
    public int DeckSize => deckSize;

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

    public PlayerAvatar avatar;


    public Player(string id, string name)
    {
        this.id = id;
        this.name = name;

        this.hasTurn = false;
        this.health = 30;
        this.deck = GetDeck();
        this.hand = new Hand(this.name);
        this.mana = 10;
        this.maxMana = 10;
    }

    public Player(PlayerState playerState, string name)
    {
        this.id = playerState.Id;
        this.name = name;

        this.hasTurn = playerState.HasTurn == 1 ? true : false;
        this.health = playerState.Health;
        this.armor = playerState.Armor;
        this.deckSize = playerState.DeckSize;
        this.mana = playerState.ManaCurrent;
        this.maxMana = playerState.ManaMax;

        List<Card> cards = playerState.GetCardsFromChallengeCards(this);
        this.hand = new Hand(this.name, cards);
    }

    public void PlayCard(CardObject cardObject)
    {
        hand.RemoveAndReposition(cardObject);
        mana -= cardObject.Card.Cost;
        RenderMana();
    }

    private void RenderMana()
    {
        TextMeshPro manaText = GameObject.Find(name + " Mana").GetComponent<TextMeshPro>();
        manaText.text = String.Format("{0}/{1}", mana.ToString(), maxMana.ToString());
    }

    public void SetPlayingField(Board.PlayingField field)
    {
        this.field = field;
    }

    private Deck GetDeck()
    {
        string deckName = PlayerPrefs.GetString("selectedDeck", "DeckA");

        //do manually for now
        List<Card> cards = new List<Card>();
        cards.Add(new CreatureCard("C1", "Direhorn Hatchling", 5, "Direhorn_Hatchling", 3, 6, new List<String>() { "shielded" }, owner: this));
        cards.Add(new WeaponCard("C2", "Fiery War Axe", 3, "Fiery_War_Axe", 3, 2, owner: this));
        cards.Add(new SpellCard("C6", "Lightning Bolt", 1, "Lightning_Bolt", targeted: true, owner: this));
        cards.Add(new WeaponCard("C4", "Fiery War Axe", 3, "Fiery_War_Axe", 3, 2, owner: this));
        cards.Add(new CreatureCard("C1", "Direhorn Hatchling", 5, "Direhorn_Hatchling", 3, 6, new List<String>() { "taunt" }, owner: this));
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

    public void NewTurn()
    {
        this.hasTurn = true;

        this.maxMana = Math.Min(maxMana + 1, 10);
        this.mana = maxMana;

        //board resetting
        field.RecoverCreatures();

        if (!InspectorControlPanel.Instance.DevelopmentMode)
        {
            this.DrawCard();
        }

        this.RenderTurnStart();
    }

    public void RenderTurnStart()
    {
        RenderMana();

        //placeholder indicator
        Vector3 targetPosition = GameObject.Find(name + " Hand").transform.position;
        GameObject light = GameObject.Find("Point Light");
        LeanTween.move(light, new Vector3(targetPosition.x, targetPosition.y, light.transform.position.z), 0.4f).setEaseOutQuart();
    }

    private int DrawCard()
    {
        if (this.deck.Cards.Count <= 0)
        {
            return 1; //amount fatigue
        }

        Card drawn = this.deck.Cards[0];
        this.deck.Cards.RemoveAt(0);
        this.AddDrawnCard(drawn); // Handles decrementing this.deckSize.

        return 0;
    }

    public int DrawCards(int amount)
    {
        int fatigue = 0;

        while (amount > 0)
        {
            fatigue += DrawCard();
            amount--;
        }

        return fatigue;
    }

    public PlayerState GeneratePlayerState()
    {
        PlayerState playerState = new PlayerState();

        playerState.SetId(this.Id);
        playerState.SetHasTurn(this.hasTurn ? 1 : 0);
        playerState.SetManaCurrent(this.mana);
        playerState.SetManaMax(this.maxMana);
        playerState.SetHealth(this.health);
        playerState.SetArmor(this.Armor);
        playerState.SetDeckSize(this.deckSize);

        List<PlayerState.ChallengeCard> handCards = new List<PlayerState.ChallengeCard>();
        for (int i = 0; i < this.hand.Size(); i += 1)
        {
            Card card = this.hand.GetCardByIndex(i);
            PlayerState.ChallengeCard challengeCard = new PlayerState.ChallengeCard();

            challengeCard.SetId(card.Id);
            challengeCard.SetName(card.Name);
            //challengeCard.SetDescription(boardCreature.Card.Description);
            //challengeCard.SetLevel(boardCreature.Card.Level);
            challengeCard.SetManaCost(card.Cost);

            if (card.GetType() == typeof(CreatureCard))
            {
                challengeCard.SetCategory(Card.CARD_CATEGORY_MINION);
                challengeCard.SetHealth(((CreatureCard)card).Health);
                challengeCard.SetAttack(((CreatureCard)card).Attack);
            }
            else if (card.GetType() == typeof(SpellCard))
            {
                challengeCard.SetCategory(Card.CARD_CATEGORY_SPELL);
            }
            // abilities

            handCards.Add(challengeCard);
        }
        playerState.SetHand(handCards);

        PlayerState.ChallengeCard[] fieldCards = new PlayerState.ChallengeCard[6];
        for (int i = 0; i < 6; i += 1)
        {
            BoardCreature boardCreature = this.field.GetCreatureByIndex(i);
            PlayerState.ChallengeCard challengeCard = new PlayerState.ChallengeCard();

            if (boardCreature == null)
            {
                challengeCard.SetId("EMPTY");
            }
            else
            {
                challengeCard.SetId(boardCreature.Card.Id);
                challengeCard.SetCategory(Card.CARD_CATEGORY_MINION);
                challengeCard.SetName(boardCreature.Card.Name);
                //challengeCard.SetDescription(boardCreature.Card.Description);
                //challengeCard.SetLevel(boardCreature.Card.Level);
                challengeCard.SetManaCost(boardCreature.Card.Cost);
                challengeCard.SetHealth(boardCreature.Health);
                challengeCard.SetHealthStart(boardCreature.Card.Health);
                challengeCard.SetAttack(boardCreature.Attack);
                challengeCard.SetAttackStart(boardCreature.Card.Attack);
                challengeCard.SetCanAttack(boardCreature.CanAttack);
                //challengeCard.SetHasShield(boardCreature.HasShield);
                // abilities
            }

            fieldCards.SetValue(challengeCard, i);
        }
        playerState.SetField(fieldCards);

        return playerState;
    }

    public bool TakeDamage(int amount)
    {
        this.health -= amount;
        return this.health > 0;
    }

    public int GetOpponentHandIndex(int handIndex)
    {
        return this.Hand.Size() - 1 - handIndex;
    }

    public void AddDrawnCard(Card card)
    {
        this.deckSize -= 1;
        this.Hand.AddDrawnCard(card);
    }
}
