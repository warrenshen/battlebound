﻿using System.Collections;
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

    private bool hasTurn;
    public bool HasTurn => hasTurn;

    private int cardCount;
    public int CardCount => cardCount;

    private int mode;
    public int Mode => mode;

    private Board.PlayingField field;
    public Board.PlayingField Field => Field;

    private PlayerAvatar avatar;
    public PlayerAvatar Avatar => avatar;

    private List<Card> keptMulliganCards;
    public List<Card> KeptMulliganCards => keptMulliganCards;

    private List<Card> removedMulliganCards;
    public List<Card> RemovedMulliganCards => removedMulliganCards;

    private List<Card> replaceMulliganCards;

    public Player(string id, string name)
    {
        this.id = id;
        this.name = name;

        this.hasTurn = false;
        this.deck = GetDeck();
        this.deckSize = this.deck.Size();

        this.mana = 40;
        this.maxMana = 40;

        this.cardCount = 0;

        this.hand = new Hand(this);

        this.avatar = GameObject.Find(String.Format("{0} Avatar", this.name)).GetComponent<PlayerAvatar>();
        this.avatar.Initialize(this);

        this.replaceMulliganCards = new List<Card>();
    }

    public Player(PlayerState playerState, string name)
    {
        this.id = playerState.Id;
        this.name = name;

        this.hasTurn = playerState.HasTurn == 1;
        this.deckSize = playerState.DeckSize;

        this.mana = playerState.ManaCurrent;
        this.maxMana = playerState.ManaMax;

        this.cardCount = playerState.CardCount;

        this.avatar = GameObject.Find(String.Format("{0} Avatar", this.name)).GetComponent<PlayerAvatar>();
        this.avatar.Initialize(this, playerState);
        this.mode = playerState.Mode;

        this.replaceMulliganCards = new List<Card>();
    }

    public void Initialize(PlayerState playerState)
    {
        List<Card> handCards = playerState.GetCardsHand();
        this.hand = new Hand(this);
        AddCardsOnResume(handCards);
    }

    public void SetMode(int mode)
    {
        this.mode = mode;
    }

    public void PlayCard(BattleCardObject battleCardObject)
    {
        this.mana -= battleCardObject.GetCost();
        RenderMana();

        this.hand.RemoveByCardId(battleCardObject.Card.Id);
        this.hand.RepositionCards();
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

    public void EndTurn()
    {
        this.hasTurn = false;
        this.hand.RecedeCards();
    }

    public void NewTurn()
    {
        this.hasTurn = true;

        this.maxMana = Math.Min(maxMana + 10, 100);
        this.mana = maxMana;

        RenderTurnStart();
    }

    public void MulliganNewTurn()
    {
        this.hasTurn = true;
        RenderTurnStart();
    }

    public void RenderTurnStart()
    {
        RenderMana();

        //placeholder indicator
        Vector3 targetPosition = GameObject.Find(name + " Hand").transform.position;
        GameObject light = GameObject.Find("Point Light");
        LeanTween.move(light, new Vector3(targetPosition.x, targetPosition.y, light.transform.position.z), 0.4f).setEaseOutQuart();

        this.hand.RepositionCards();
    }

    public void RenderGameStart()
    {
        RenderMana();
        this.hand.RepositionCards();
    }

    /*
     * Create and add a device challenge move for draw card.
     */
    public int DrawCardDevice()
    {
        int rank = BattleManager.Instance.GetDeviceMoveRank();

        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(this.id);
        challengeMove.SetRank(rank);

        if (this.deckSize <= 0)
        {
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_DRAW_CARD_DECK_EMPTY);
        }
        else
        {
            if (this.hand.IsFull())
            {
                challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_DRAW_CARD_HAND_FULL);
            }
            else
            {
                challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_DRAW_CARD);
            }
        }

        BattleManager.Instance.AddDeviceMove(challengeMove);
        return rank;
    }

    private void DrawCardMock(bool isMulligan = false)
    {
        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(this.id);
        challengeMove.SetRank(BattleManager.Instance.GetServerMoveRank());

        if (this.deckSize <= 0)
        {
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_DRAW_CARD_DECK_EMPTY);
        }
        else
        {
            int randomIndex = UnityEngine.Random.Range(0, this.deck.Size());
            Card drawnCard = this.deck.Cards[randomIndex];
            this.deck.Cards.RemoveAt(randomIndex);

            if (isMulligan)
            {
                challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_DRAW_CARD_MULLIGAN);
            }
            else if (this.hand.IsFull())
            {
                challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_DRAW_CARD_HAND_FULL);
            }
            else
            {
                challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_DRAW_CARD);
            }

            ChallengeMove.ChallengeMoveAttributes challengeMoveAttributes = new ChallengeMove.ChallengeMoveAttributes();
            challengeMoveAttributes.SetCard(drawnCard.GetChallengeCard());
            challengeMove.SetMoveAttributes(challengeMoveAttributes);
        }

        BattleManager.Instance.ReceiveChallengeMove(challengeMove);
    }

    public void DrawCardsMock(int amount)
    {
        while (amount > 0)
        {
            DrawCardMock();
            amount--;
        }
    }

    /*
     * Only meant to be used for local dev - do not use willy nilly!
     */
    private void DrawCardForce()
    {
        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(this.id);
        challengeMove.SetRank(BattleManager.Instance.GetDeviceMoveRank());
        challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_DRAW_CARD);
        BattleManager.Instance.AddDeviceMove(challengeMove);

        DrawCardMock();
    }

    /*
     * Only meant to be used for local dev - do not use willy nilly!
     */
    public void DrawCardsForce(int amount)
    {
        while (amount > 0)
        {
            DrawCardForce();
            amount--;
        }
    }

    private Card PopCardFromDeck()
    {
        this.deckSize -= 1;

        int randomIndex = UnityEngine.Random.Range(0, this.deck.Size());
        Card drawnCard = this.deck.Cards[randomIndex];
        this.deck.Cards.RemoveAt(randomIndex);

        return drawnCard;
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

    public void BeginMulligan(List<Card> mulliganCards)
    {
        this.mode = PLAYER_STATE_MODE_MULLIGAN;

        this.keptMulliganCards = mulliganCards;
        this.removedMulliganCards = new List<Card>();

        for (int i = 0; i < this.keptMulliganCards.Count; i++)
        {
            BattleCardObject battleCardObject = AddMulliganCard(
                this.keptMulliganCards[i],
                i
            );
        }

        BattleManager.Instance.SetBoardCenterText("Choose cards to mulligan..");
    }

    /*
     * Function only for connected mode.
     */
    public void ResumeMulligan(List<Card> mulliganCards)
    {
        if (!DeveloperPanel.IsServerEnabled())
        {
            Debug.LogError("Resume mulligan called when not in connected mode.");
            return;
        }

        if (this.mode == PLAYER_STATE_MODE_MULLIGAN)
        {
            BeginMulligan(mulliganCards);
        }
        else if (this.mode == PLAYER_STATE_MODE_MULLIGAN_WAITING)
        {
            BattleManager.Instance.SetBoardCenterText("Waiting on opponent to mulligan..");
        }
        else
        {
            Debug.LogError("Resume mulligan called on player not in mulligan mode.");
        }
    }

    public bool IsModeMulligan()
    {
        return this.mode == PLAYER_STATE_MODE_MULLIGAN || this.mode == PLAYER_STATE_MODE_MULLIGAN_WAITING;
    }

    /*
     * If player is in mulligan waiting state, advance to normal state.
     * If player is in mulligan state, do nothing.
     * If player is in normal state, log an error.
     * 
     * This function should be called after this player's opponent plays its mulligan.
     */
    public void AdvanceMulliganState()
    {
        if (this.mode == PLAYER_STATE_MODE_MULLIGAN_WAITING)
        {
            this.mode = PLAYER_STATE_MODE_NORMAL;
        }
        else if (this.mode == PLAYER_STATE_MODE_NORMAL)
        {
            Debug.LogError("Advance mulligan state called when player in normal state.");
        }
    }

    public void PlayMulligan(int opponentMode)
    {
        if (this.mode != PLAYER_STATE_MODE_MULLIGAN)
        {
            Debug.LogError("Player not in mulligan mode but received play mulligan.");
            return;
        }
        else if (opponentMode == PLAYER_STATE_MODE_NORMAL)
        {
            Debug.LogError("Opponent in normal mode but received play mulligan.");
            return;
        }

        List<string> cardIds = new List<string>();
        foreach (Card keptCard in this.keptMulliganCards)
        {
            //card just needs to be repositioned to hand
            cardIds.Add(keptCard.Id);
        }

        if (DeveloperPanel.IsServerEnabled())
        {
            BattleSingleton.Instance.SendChallengePlayMulliganRequest(cardIds);
        }
        else
        {
            ChallengeMove challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(this.id);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_PLAY_MULLIGAN);
            challengeMove.SetRank(BattleManager.Instance.GetServerMoveRank());
            BattleManager.Instance.ReceiveChallengeMove(challengeMove);
        }

        foreach (Card removedCard in this.removedMulliganCards)
        {
            ReplaceCardByMulligan(removedCard);
        }

        this.mode = PLAYER_STATE_MODE_MULLIGAN_WAITING;
        BattleManager.Instance.SetBoardCenterText("Waiting on opponent to mulligan..");

        // If not in connected mode, automatically perform mulligan for opponent.
        if (!DeveloperPanel.IsServerEnabled())
        {
            string opponentId = Board.Instance.GetOpponentIdByPlayerId(this.id);

            ChallengeMove challengeMove = new ChallengeMove();
            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(opponentId);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_PLAY_MULLIGAN);
            challengeMove.SetRank(BattleManager.Instance.GetServerMoveRank());

            ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
            moveAttributes.SetDeckCardIndices(new List<int>());
            challengeMove.SetMoveAttributes(moveAttributes);

            BattleManager.Instance.ReceiveChallengeMove(challengeMove);

            challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(opponentId);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_FINISH_MULLIGAN);
            challengeMove.SetRank(BattleManager.Instance.GetServerMoveRank());

            BattleManager.Instance.ReceiveChallengeMove(challengeMove);
        }
    }

    /*
     * Function called on opponent player.
     */
    public void PlayMulliganByIndices(List<int> replacedCardIndices)
    {
        if (this.mode != PLAYER_STATE_MODE_MULLIGAN)
        {
            Debug.LogError("Player not in mulligan mode but received play mulligan.");
            return;
        }

        // It is very important that we iterate downwards,
        // since the ReplaceCardByMulligan call removes cards from hand by index.
        for (int i = this.keptMulliganCards.Count - 1; i >= 0; i -= 1)
        {
            if (replacedCardIndices.Contains(i))
            {
                Card removeCard = this.keptMulliganCards[i];
                this.keptMulliganCards.RemoveAt(i);

                ReplaceCardByMulligan(removeCard, i);
            }
        }

        this.mode = PLAYER_STATE_MODE_MULLIGAN_WAITING;
    }

    /*
     * Function called on player "you".
     */
    public void FinishMulligan()
    {
        if (this.mode != PLAYER_STATE_MODE_MULLIGAN_WAITING)
        {
            Debug.LogError("Finish mulligan called when player not in mulligan waiting mode");
            return;
        }

        this.keptMulliganCards = new List<Card>();
        this.removedMulliganCards = new List<Card>();

        foreach (Card card in this.replaceMulliganCards)
        {
            AddDrawnCard(card);
        }

        this.replaceMulliganCards = new List<Card>();

        this.mode = PLAYER_STATE_MODE_NORMAL;

        BattleManager.Instance.HideMulliganOverlay(this);

        RenderTurnStart();
    }

    private void ReplaceCardByMulligan(Card card)
    {
        this.hand.RemoveByCardId(card.Id);
        ReturnCardToDeck(card);

        if (!DeveloperPanel.IsServerEnabled())
        {
            DrawCardMock(true);
        }

        BattleManager.Instance.UseCard(this, card.wrapper as BattleCardObject); //to-do, plays incorrect sound for now, b/c use card plays "play card" sound, maybe decouple
    }

    private void ReplaceCardByMulligan(Card card, int index)
    {
        this.hand.RemoveByIndex(index);
        ReturnCardToDeck(card);

        if (!DeveloperPanel.IsServerEnabled())
        {
            DrawCardMock(true);
        }
        Debug.Log(this.deckSize);
        BattleManager.Instance.UseCard(this, card.wrapper as BattleCardObject); //to-do, plays incorrect sound for now, b/c use card plays "play card" sound, maybe decouple
    }

    public void AddDrawnCardMulligan(Card card)
    {
        this.replaceMulliganCards.Add(card);
    }

    public void AddDrawnCardHandFull(Card card)
    {
        GameObject created = new GameObject(card.Name);
        BattleCardObject createdBattleCard = created.AddComponent<BattleCardObject>();
        createdBattleCard.Initialize(this, card);

        created.transform.parent = GameObject.Find(
            this.name + " Hand"
        ).transform;

        this.deckSize -= 1;
        BattleManager.Instance.AnimateDrawCard(this, createdBattleCard)
            .setOnComplete(() =>
            {
                CardTween
                    .move(createdBattleCard, createdBattleCard.transform.position, CardTween.TWEEN_DURATION)
                    .setOnComplete(() =>
                    {
                        EffectManager.Instance.OnDrawCardFinish();
                        createdBattleCard.Burn();
                    });
            });
    }

    public BattleCardObject AddDrawnCard(Card card)
    {
        GameObject created = new GameObject(card.Name);
        BattleCardObject createdBattleCard = created.AddComponent<BattleCardObject>();
        createdBattleCard.Initialize(this, card);

        created.transform.parent = GameObject.Find(
            this.name + " Hand"
        ).transform;

        this.deckSize -= 1;
        this.hand.AddCardObject(createdBattleCard);

        BattleManager.Instance.AnimateDrawCard(this, createdBattleCard);
        return createdBattleCard;
    }

    public BattleCardObject AddMulliganCard(Card card, int index)
    {
        GameObject created = new GameObject(card.Name);
        BattleCardObject battleCardObject = created.AddComponent<BattleCardObject>();
        battleCardObject.Initialize(this, card);

        created.transform.parent = GameObject.Find(
            this.name + " Hand"
        ).transform;

        this.hand.AddCardObject(battleCardObject);

        BattleManager.Instance.AnimateDrawCardForMulligan(
            this,
            battleCardObject,
            index
        );

        return battleCardObject;
    }

    public BattleCardObject AddCardOnResume(Card card)
    {
        GameObject created = new GameObject(card.Name);
        BattleCardObject createdBattleCard = created.AddComponent<BattleCardObject>();
        createdBattleCard.Initialize(this, card);

        created.transform.parent = GameObject.Find(
            this.name + " Hand"
        ).transform;

        this.hand.AddCardObject(createdBattleCard);

        return createdBattleCard;
    }

    private void AddCardsOnResume(List<Card> cards)
    {
        foreach (Card card in cards)
        {
            AddCardOnResume(card);
        }

        this.hand.RepositionCards();
    }

    public void ReturnCardToDeck(Card card)
    {
        if (!DeveloperPanel.IsServerEnabled())
        {
            this.deck.AddCard(card);
        }

        this.deckSize += 1;
    }

    public int GetOpponentHandIndex(int handIndex)
    {
        return this.hand.Size() - 1 - handIndex;
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
        playerState.SetCardCount(this.cardCount);
        playerState.SetDeckSize(this.deckSize);
        playerState.SetMode(this.mode);

        List<ChallengeCard> handCards = new List<ChallengeCard>();
        if (this.mode != PLAYER_STATE_MODE_MULLIGAN)
        {
            for (int i = 0; i < this.hand.Size(); i += 1)
            {
                BattleCardObject battleCardObject = this.hand.GetCardObjectByIndex(i);
                ChallengeCard challengeCard = battleCardObject.GetChallengeCard();
                challengeCard.SetPlayerId(this.id);
                handCards.Add(challengeCard);
            }
        }
        playerState.SetHand(handCards);

        ChallengeCard[] fieldCards = new ChallengeCard[6];
        for (int i = 0; i < 6; i += 1)
        {
            Board.PlayingField playingField = Board.Instance.GetFieldByPlayerId(this.id);
            BoardCreature boardCreature = playingField.GetCreatureByIndex(i);

            if (boardCreature == null)
            {
                ChallengeCard challengeCard = new ChallengeCard();
                challengeCard.SetId("EMPTY");
                fieldCards.SetValue(challengeCard, i);
            }
            else
            {
                fieldCards.SetValue(boardCreature.GetChallengeCard(), i);
            }
        }
        playerState.SetField(fieldCards);

        return playerState;
    }

    private Deck GetDeck()
    {
        string deckName = PlayerPrefs.GetString("selectedDeck", "DeckA");
        //do manually for now
        //cards.Add(new CreatureCard("C2", "Cursed Imp", 1));

        List<Card> cards = new List<Card>();
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_BLESSED_NEWBORN, 2));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_TEMPLE_GUARDIAN, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_WATERBORNE_RAZORBACK, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_BOMBSHELL_BOMBADIER, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_FIREBUG_CATELYN, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_MARSHWATER_SQUEALER, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_TAJI_THE_FEARLESS, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_YOUNG_KYO, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_EMBERKITTY, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_FIRESTRIDED_TIGRESS, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_UNKINDLED_JUNIOR, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_FLAMEBELCHER, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_FIREBORN_MENACE, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_TEA_GREENLEAF, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_WAVE_CHARMER, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_POSEIDONS_HANDMAIDEN, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_NESSA_NATURES_CHAMPION, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_BUBBLE_SQUIRTER, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_SWIFT_SHELLBACK, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_SENTIENT_SEAKING, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_CRYSTAL_SNAPPER, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_BATTLECLAD_GASDON, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_REDHAIRED_PALADIN, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_FIRESWORN_GODBLADE, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_RITUAL_HATCHLING, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_HELLBRINGER, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_HOOFED_LUSH, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_DIONYSIAN_TOSSPOT, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_SEAHORSE_SQUIRE, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_TRIDENT_BATTLEMAGE, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_SNEERBLADE, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_TIMEWARP_KINGPIN, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_LUX, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_THUNDEROUS_DESPERADO, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_CEREBOAROUS, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_GUPPEA, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_RHYNOKARP, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_PRICKLEPILLAR, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_ADDERSPINE_WEEVIL, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_THIEF_OF_NIGHT, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_POWER_SIPHONER, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_LIL_RUSTY, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_INFERNO_902, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_CHAR_BOT_451, 1));
        cards.Add(new CreatureCard(this.name + GetNewCardRank(), Card.CARD_NAME_MEGAPUNK, 1));

        cards.Add(new SpellCard(this.name + GetNewCardRank(), Card.CARD_NAME_UNSTABLE_POWER, 4));
        cards.Add(new SpellCard(this.name + GetNewCardRank(), Card.CARD_NAME_TOUCH_OF_ZEUS, 1));
        cards.Add(new SpellCard(this.name + GetNewCardRank(), Card.CARD_NAME_RAZE_TO_ASHES, 1));
        cards.Add(new SpellCard(this.name + GetNewCardRank(), Card.CARD_NAME_DEEP_FREEZE, 1));
        cards.Add(new SpellCard(this.name + GetNewCardRank(), Card.CARD_NAME_BRR_BRR_BLIZZARD, 1));
        cards.Add(new SpellCard(this.name + GetNewCardRank(), Card.CARD_NAME_RIOT_UP, 1));
        cards.Add(new SpellCard(this.name + GetNewCardRank(), Card.CARD_NAME_WIDESPREAD_FROSTBITE, 1));
        cards.Add(new SpellCard(this.name + GetNewCardRank(), Card.CARD_NAME_GREEDY_FINGERS, 1));
        cards.Add(new SpellCard(this.name + GetNewCardRank(), Card.CARD_NAME_SILENCE_OF_THE_LAMBS, 1));
        cards.Add(new SpellCard(this.name + GetNewCardRank(), Card.CARD_NAME_MUDSLINGING, 1));
        cards.Add(new SpellCard(this.name + GetNewCardRank(), Card.CARD_NAME_DEATH_NOTICE, 1));
        cards.Add(new SpellCard(this.name + GetNewCardRank(), Card.CARD_NAME_SPRAY_N_PRAY, 1));
        cards.Add(new SpellCard(this.name + GetNewCardRank(), Card.CARD_NAME_GRAVE_DIGGING, 1));

        Deck chosen = new Deck(deckName, cards, DeckRaw.DeckClass.Hunter, owner: this);
        return chosen;
    }

    public int GetNewCardRank()
    {
        int cardRank = this.cardCount;
        this.cardCount += 1;
        return cardRank;
    }
}
