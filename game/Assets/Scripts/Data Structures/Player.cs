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

    private PlayerAvatar avatar;
    public PlayerAvatar Avatar => avatar;

    public Player(string id, string name)
    {
        this.id = id;
        this.name = name;

        this.hasTurn = false;
        this.deck = GetDeck();
        this.mana = 30;
        this.maxMana = 30;

        this.hand = new Hand(this);

        this.avatar = GameObject.Find(String.Format("{0} Avatar", this.name)).GetComponent<PlayerAvatar>();
        this.avatar.Initialize(this);

        Board.Instance().RegisterPlayer(this);
    }

    public Player(PlayerState playerState, string name)
    {
        this.id = playerState.Id;
        this.name = name;

        this.hasTurn = playerState.HasTurn == 1;
        this.deckSize = playerState.DeckSize;
        this.mana = playerState.ManaCurrent;
        this.maxMana = playerState.ManaMax;

        List<Card> handCards = playerState.GetCardsHand();
        this.hand = new Hand(this);
        this.AddDrawnCards(handCards, true);

        this.avatar = GameObject.Find(String.Format("{0} Avatar", this.name)).GetComponent<PlayerAvatar>();
        this.avatar.Initialize(this, playerState);

        Card[] fieldCards = playerState.GetCardsField();

        Board.Instance().RegisterPlayer(this, fieldCards);
    }

    public void PlayCard(CardObject cardObject)
    {
        this.mana -= cardObject.Card.Cost;
        RenderMana();
        this.hand.RemoveByCardId(cardObject.Card.Id);
    }

    private void RenderMana()
    {
        TextMeshPro manaText = GameObject.Find(name + " Mana").GetComponent<TextMeshPro>();
        manaText.text = String.Format("{0}/{1}", mana.ToString(), maxMana.ToString());
    }

    private Deck GetDeck()
    {
        string deckName = PlayerPrefs.GetString("selectedDeck", "DeckA");

        //do manually for now
        List<Card> cards = new List<Card>();
        cards.Add(new CreatureCard("C1", "Direhorn Hatchling", 2, 5, "HS/Direhorn_Hatchling", 3, 6, new List<String>() { Card.CARD_ABILITY_SHIELD }));
        cards.Add(new CreatureCard("C2", "Direhorn Hatchling", 1, 5, "HS/Direhorn_Hatchling", 3, 6, new List<String>() { Card.CARD_ABILITY_TAUNT }));
        cards.Add(new CreatureCard("C3", "Direhorn Hatchling", 1, 5, "HS/Direhorn_Hatchling", 3, 6, new List<String>() { Card.CARD_ABILITY_CHARGE }));
        cards.Add(new WeaponCard("C4", "Fiery War Axe", 1, 3, "HS/Fiery_War_Axe", 3, 2));
        cards.Add(new WeaponCard("C5", "Fiery War Axe", 1, 3, "HS/Fiery_War_Axe", 3, 2));
        cards.Add(new SpellCard("C6", "Lightning Bolt", 4, 1, "HS/Lightning_Bolt"));
        cards.Add(new SpellCard("C7", "Lightning Bolt", 5, 1, "HS/Lightning_Bolt"));
        cards.Add(new SpellCard("C8", "Lightning Bolt", 6, 1, "HS/Lightning_Bolt"));
        cards.Add(new SpellCard("C9", "Lightning Bolt", 1, 1, "HS/Lightning_Bolt"));
        cards.Add(new WeaponCard("C10", "Fiery War Axe", 1, 3, "HS/Fiery_War_Axe", 3, 2));

        Deck chosen = new Deck(deckName, cards, Deck.DeckClass.Hunter, owner: this);
        return chosen;
    }

    public int TakeDamage(int amount)
    {
        return this.avatar.TakeDamage(amount);
    }

    public int Heal(int amount)
    {
        return this.avatar.Heal(amount);
    }

    public void SetHasTurn(bool newTurn)
    {
        this.hasTurn = newTurn;
    }

    public void NewTurn()
    {
        this.hasTurn = true;

        this.maxMana = Math.Min(maxMana + 10, 100);
        this.mana = maxMana;

        //board resetting
        Board.Instance().RecoverCreaturesByPlayerId(this.Id);
        this.avatar.OnStartTurn();

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
            this.Hand.RepositionCards();  //needed to correct recession
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

    /*
     * @param bool isInit - do not decrement deck size if true
     */
    public void AddDrawnCard(Card card, bool isInit = false)
    {
        GameObject created = new GameObject(card.Name);
        CardObject cardObject = created.AddComponent<CardObject>();
        cardObject.InitializeCard(this, card);
        created.transform.parent = GameObject.Find(
            this.name + " Hand"
        ).transform;

        if (!isInit)
        {
            this.deckSize -= 1;
        }
        this.Hand.AddCardObject(cardObject);
    }

    public void AddDrawnCards(List<Card> cards, bool isInit = false)
    {
        foreach (Card card in cards)
        {
            AddDrawnCard(card, isInit);
        }
    }

    public int GetOpponentHandIndex(int handIndex)
    {
        return this.Hand.Size() - 1 - handIndex;
    }

    public PlayerState GeneratePlayerState()
    {
        PlayerState playerState = new PlayerState();

        playerState.SetId(this.id);
        playerState.SetHasTurn(this.hasTurn ? 1 : 0);
        playerState.SetManaCurrent(this.mana);
        playerState.SetManaMax(this.maxMana);
        playerState.SetHealth(this.avatar.Health);
        playerState.SetHealthMax(this.avatar.MaxHealth);
        playerState.SetArmor(this.avatar.Armor);
        playerState.SetDeckSize(this.deckSize);

        List<PlayerState.ChallengeCard> handCards = new List<PlayerState.ChallengeCard>();
        for (int i = 0; i < this.hand.Size(); i += 1)
        {
            Card card = this.hand.GetCardObjectByIndex(i).Card;
            PlayerState.ChallengeCard challengeCard = new PlayerState.ChallengeCard();

            challengeCard.SetId(card.Id);
            challengeCard.SetName(card.Name);
            //challengeCard.SetDescription(boardCreature.Card.Description);
            //challengeCard.SetLevel(boardCreature.Card.Level);
            challengeCard.SetCost(card.Cost);
            challengeCard.SetCostStart(card.Cost);

            if (card.GetType() == typeof(CreatureCard))
            {
                CreatureCard creatureCard = (CreatureCard)card;
                challengeCard.SetCategory(Card.CARD_CATEGORY_MINION);
                challengeCard.SetHealth(creatureCard.Health);
                challengeCard.SetHealthStart(creatureCard.Health);
                challengeCard.SetHealthMax(creatureCard.Health);
                challengeCard.SetAttack(creatureCard.Attack);
                challengeCard.SetAttackStart(creatureCard.Attack);
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
            Board.PlayingField playingField = Board.Instance().GetFieldByPlayerId(this.id);
            BoardCreature boardCreature = playingField.GetCreatureByIndex(i);
            PlayerState.ChallengeCard challengeCard = new PlayerState.ChallengeCard();

            if (boardCreature == null)
            {
                challengeCard.SetId("EMPTY");
            }
            else
            {
                challengeCard.SetId(boardCreature.CreatureCard.Id);
                challengeCard.SetCategory(Card.CARD_CATEGORY_MINION);
                challengeCard.SetName(boardCreature.CreatureCard.Name);
                //challengeCard.SetDescription(boardCreature.Card.Description);
                //challengeCard.SetLevel(boardCreature.Card.Level);
                challengeCard.SetCost(boardCreature.CreatureCard.Cost);
                challengeCard.SetCostStart(boardCreature.CreatureCard.Cost);
                challengeCard.SetHealth(boardCreature.Health);
                challengeCard.SetHealthStart(boardCreature.CreatureCard.Health);
                challengeCard.SetHealthMax(boardCreature.MaxHealth);
                challengeCard.SetAttack(boardCreature.Attack);
                challengeCard.SetAttackStart(boardCreature.CreatureCard.Attack);
                challengeCard.SetCanAttack(boardCreature.CanAttack);
                //challengeCard.SetHasShield(boardCreature.HasShield);
                // abilities
            }

            fieldCards.SetValue(challengeCard, i);
        }
        playerState.SetField(fieldCards);

        return playerState;
    }
}
