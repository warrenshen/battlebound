using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

[System.Serializable]
public class Player
{
    public const int PLAYER_STATE_MODE_NORMAL = 0;
    public const int PLAYER_STATE_MODE_MULLIGAN = 1;
    public const int PLAYER_STATE_MODE_MULLIGAN_WAITING = 2;

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

    private int mode;
    public int Mode => mode;

    private List<Card> keptMulliganCards;
    public List<Card> KeptMulliganCards => keptMulliganCards;
    private List<Card> removedMulliganCards;
    public List<Card> RemovedMulliganCards => removedMulliganCards;

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

        Board.Instance.RegisterPlayer(this);
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
        this.AddDrawnCards(handCards, true, animate: false);

        this.avatar = GameObject.Find(String.Format("{0} Avatar", this.name)).GetComponent<PlayerAvatar>();
        this.avatar.Initialize(this, playerState);

        Card[] fieldCards = playerState.GetCardsField();

        Board.Instance.RegisterPlayer(this, fieldCards);
    }

    public void SetMode(int mode)
    {
        this.mode = mode;
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

    public int TakeDamage(int amount)
    {
        return this.avatar.TakeDamage(amount);
    }

    public int Heal(int amount)
    {
        return this.avatar.Heal(amount);
    }

    private void SetHasTurn(bool newTurn)
    {
        this.hasTurn = newTurn;
    }

    public void EndTurn()
    {
        SetHasTurn(false);
        this.hand.RecedeCards();
        Board.Instance.OnPlayerEndTurn(this.id);
    }

    public void NewTurn()
    {
        this.hasTurn = true;

        this.maxMana = Math.Min(maxMana + 10, 100);
        this.mana = maxMana;

        //board resetting
        Board.Instance.OnPlayerStartTurn(this.id);
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

    private int DrawCard(bool animate = true)
    {
        if (this.deck.Cards.Count <= 0)
        {
            this.Hand.RepositionCards();  //keep, needed to correct recession
            return 1; //amount fatigue
        }

        Card drawn = this.deck.Cards[0];
        this.deck.Cards.RemoveAt(0);
        this.AddDrawnCard(drawn, animate: animate); // Handles decrementing this.deckSize.

        return 0;
    }

    public int DrawCards(int amount, bool animate = true)
    {
        int fatigue = 0;

        while (amount > 0)
        {
            fatigue += DrawCard(animate);
            amount--;
        }
        return fatigue;
    }

    private Card PopCardFromDeck()
    {
        Card card = this.deck.Cards[0];
        this.deck.Cards.RemoveAt(0);
        return card;
    }

    public List<Card> PopCardsFromDeck(int amount)
    {
        List<Card> cards = new List<Card>();

        while (amount > 0)
        {
            cards.Add(PopCardFromDeck());
            amount--;
        }

        return cards;
    }

    public void BeginMulligan(List<Card> mulliganCards, bool show)
    {
        this.mode = PLAYER_STATE_MODE_MULLIGAN;
        this.keptMulliganCards = mulliganCards;
        this.removedMulliganCards = new List<Card>();

        if (!show)
            return;

        for (int i = 0; i < mulliganCards.Count; i++)
        {
            CardObject cardObject = AddDrawnCard(mulliganCards[i], reposition: false);
            BattleManager.Instance.AnimateDrawCardForMulligan(this, cardObject, i); //covers for the omitted reposition
        }
    }

    public void PlayMulligan(int opponentState)
    {
        if (opponentState == PLAYER_STATE_MODE_MULLIGAN_WAITING)
        {
            this.mode = PLAYER_STATE_MODE_NORMAL;
        }
        else
        {
            this.mode = PLAYER_STATE_MODE_MULLIGAN_WAITING;
        }

        List<string> cardIds = new List<string>();
        foreach (Card keptCard in this.keptMulliganCards)
        {
            //card just needs to be repositioned to hand
            cardIds.Add(keptCard.Id);
        }
        foreach (Card removedCard in this.removedMulliganCards)
        {
            if (!InspectorControlPanel.Instance.DevelopmentMode)
            {
                ReturnCardToDeck(removedCard);
                this.DrawCard();
            }
        }

        if (InspectorControlPanel.Instance.DevelopmentMode)
        {
            BattleSingleton.Instance.SendChallengePlayMulliganRequest(cardIds);
        }

        EndMulligan();
    }

    public void EndMulligan()
    {
        this.keptMulliganCards = new List<Card>();
        this.removedMulliganCards = new List<Card>();
        BattleManager.Instance.HideMulliganOverlay(this);
        this.hand.RepositionCards();

        //to-do: need to set player state to normal when both are done w/ mulligan. @Warren
    }

    public void ShowMulligan(List<int> replacedCardIndices, Player opponent)
    {
        if (opponent.Mode != PLAYER_STATE_MODE_MULLIGAN)
        {
            Debug.LogError("Opponent not in mulligan mode but received show mulligan.");
            return;
        }
        else if (this.mode == PLAYER_STATE_MODE_MULLIGAN_WAITING)
        {
            opponent.SetMode(PLAYER_STATE_MODE_NORMAL);
            this.mode = PLAYER_STATE_MODE_NORMAL;
        }
        else if (this.mode == PLAYER_STATE_MODE_MULLIGAN)
        {
            opponent.SetMode(PLAYER_STATE_MODE_MULLIGAN_WAITING);
        }
        else
        {
            Debug.LogError("Player not in mulligan mode but received show mulligan.");
            return;
        }

        if (InspectorControlPanel.Instance.DevelopmentMode)
        {
            // Throw away cards.
        }
        else
        {
            // TODO: animate throw away cards.
            for (int i = 0; i < opponent.KeptMulliganCards.Count + opponent.RemovedMulliganCards.Count; i += 1)
            {
                if (replacedCardIndices.Contains(i))
                {
                    opponent.DrawCard(animate: true);
                }
                else
                {
                    //just reposition
                }
            }
            opponent.EndMulligan();
        }
    }

    /*
     * @param bool isInit - do not decrement deck size if true
     */
    public CardObject AddDrawnCard(Card card, bool isInit = false, bool animate = true, bool reposition = true)
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

        if (reposition)
        {
            if (animate)
            {
                BattleManager.Instance.AnimateDrawCard(this, cardObject);
            }
            else
            {
                hand.RepositionCards();
            }
        }
        return cardObject;
    }

    public void AddDrawnCards(List<Card> cards, bool isInit = false, bool animate = true)
    {
        foreach (Card card in cards)
        {
            AddDrawnCard(card, isInit, animate: animate);
        }
    }

    public void ReturnCardToDeck(Card card)
    {
        if (!InspectorControlPanel.Instance.DevelopmentMode)
        {
            this.deck.AddCard(card);
            this.Hand.RemoveByCardId(card.Id);
            BattleManager.Instance.UseCard(this, card.wrapper); //to-do, plays incorrect sound for now, b/c use card plays "play card" sound, maybe decouple
        }
        else
        {
            Debug.LogError("This method should not be used in connected mode.");
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
            Board.PlayingField playingField = Board.Instance.GetFieldByPlayerId(this.id);
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

    private Deck GetDeck()
    {
        string deckName = PlayerPrefs.GetString("selectedDeck", "DeckA");

        //do manually for now
        List<Card> cards = new List<Card>();
        cards.Add(new CreatureCard("C1", "Blessed Newborn", "Battlecry: Draw a card", "HS/Direhorn_Hatchling", 2, 20, 10, 10, new List<String>() { Card.CARD_ABILITY_BATTLE_CRY_DRAW_CARD }, "BlessedNewborn"));
        cards.Add(new CreatureCard("C2", "Aqua Piglet", "At the end of each turn, recover 10 health", "HS/Direhorn_Hatchling", 1, 20, 20, 30, new List<String>() { Card.CARD_ABILITY_END_TURN_HEAL_TEN }, "PigletAllColor"));
        cards.Add(new CreatureCard("C3", "Temple Guardian", "Charge; Shield", "HS/Direhorn_Hatchling", 1, 60, 40, 70, new List<String>() { Card.CARD_ABILITY_TAUNT, Card.CARD_ABILITY_SHIELD }, "TempleGuardian"));
        cards.Add(new CreatureCard("C4", "Firebug Catelyn", "", "HS/Direhorn_Hatchling", 1, 10, 10, 10, new List<String>() { }, "FirebugCatelyn"));
        cards.Add(new CreatureCard("C5", "Pyre Dancer", "", "HS/Direhorn_Hatchling", 1, 30, 30, 20, new List<String>() { Card.CARD_ABILITY_CHARGE }));
        //cards.Add(new WeaponCard("C4", "Fiery War Axe", "HS/Fiery_War_Axe", 1, 3, 3, 2));
        //cards.Add(new WeaponCard("C5", "Fiery War Axe", "HS/Fiery_War_Axe", 1, 3, 3, 2));
        cards.Add(new SpellCard("C6", "Unstable Power", "Give a creature +30/+0, it dies at start of next turn", "HS/Lightning_Bolt", 4, 30));
        cards.Add(new CreatureCard("C7", "Cursed Imp", "Lifesteal", "HS/Direhorn_Hatchling", 1, 20, 10, 40, new List<String>() { Card.CARD_ABILITY_LIFE_STEAL }));
        cards.Add(new CreatureCard("C8", "Aqua Squealer", "Charge; At the end of each turn, recover 20 health", "HS/Direhorn_Hatchling", 1, 20, 10, 40, new List<String>() { Card.CARD_ABILITY_CHARGE, Card.CARD_ABILITY_END_TURN_HEAL_TWENTY }));

        Deck chosen = new Deck(deckName, cards, Deck.DeckClass.Hunter, owner: this);
        return chosen;
    }
}
