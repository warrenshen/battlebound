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

    private List<Card> replaceMulliganCards;

    public Player(string id, string name)
    {
        this.id = id;
        this.name = name;

        this.hasTurn = false;
        this.deck = GetDeck();
        this.deckSize = this.deck.Size();

        this.mana = 30;
        this.maxMana = 30;

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

        this.avatar = GameObject.Find(String.Format("{0} Avatar", this.name)).GetComponent<PlayerAvatar>();
        this.avatar.Initialize(this, playerState);
        this.mode = playerState.Mode;

        this.replaceMulliganCards = new List<Card>();
    }

    public void Initialize(PlayerState playerState)
    {
        List<Card> handCards = playerState.GetCardsHand();
        this.hand = new Hand(this);
        this.AddDrawnCards(handCards, true, animate: false);
    }

    public void SetMode(int mode)
    {
        this.mode = mode;
    }

    public void PlayCard(BattleCardObject battleCardObject)
    {
        this.mana -= battleCardObject.Card.GetCost();
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
            BattleCardObject battleCardObject = AddDrawnCard(
                this.keptMulliganCards[i],
                isInit: true,
                reposition: false //doesn't use standard RepositionCards()
            );
            BattleManager.Instance.AnimateDrawCardForMulligan(
                this,
                battleCardObject,
                i
            ); //special animation to replace the omitted reposition
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

    /*
     * @param bool isInit - do not decrement deck size if true
     */
    public BattleCardObject AddDrawnCard(Card card, bool isInit = false, bool animate = true, bool reposition = true)
    {
        GameObject created = new GameObject(card.Name);
        BattleCardObject createdBattleCard = created.AddComponent<BattleCardObject>();
        createdBattleCard.Initialize(this, card);

        created.transform.parent = GameObject.Find(
            this.name + " Hand"
        ).transform;

        if (!isInit)
        {
            this.deckSize -= 1;
        }

        this.hand.AddCardObject(createdBattleCard);

        if (reposition)
        {
            if (animate)
            {
                BattleManager.Instance.AnimateDrawCard(this, createdBattleCard);
            }
            else
            {
                this.hand.RepositionCards();
            }
        }
        return createdBattleCard;
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
        playerState.SetDeckSize(this.deckSize);
        playerState.SetMode(this.mode);

        List<PlayerState.ChallengeCard> handCards = new List<PlayerState.ChallengeCard>();
        if (this.mode != PLAYER_STATE_MODE_MULLIGAN)
        {
            for (int i = 0; i < this.hand.Size(); i += 1)
            {
                Card card = this.hand.GetCardObjectByIndex(i).Card;
                PlayerState.ChallengeCard challengeCard = card.GetChallengeCard();
                handCards.Add(challengeCard);
            }
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
                challengeCard.SetCategory((int)Card.CardType.Creature);
                challengeCard.SetName(boardCreature.CreatureCard.Name);
                //challengeCard.SetDescription(boardCreature.CreatureCard.Description);
                challengeCard.SetLevel(boardCreature.CreatureCard.Level);
                challengeCard.SetCost(boardCreature.CreatureCard.GetCost());
                challengeCard.SetCostStart(boardCreature.CreatureCard.GetCost());
                challengeCard.SetHealth(boardCreature.Health);
                challengeCard.SetHealthStart(boardCreature.CreatureCard.GetHealth());
                challengeCard.SetHealthMax(boardCreature.MaxHealth);
                challengeCard.SetAttack(boardCreature.Attack);
                challengeCard.SetAttackStart(boardCreature.CreatureCard.GetAttack());
                challengeCard.SetCanAttack(boardCreature.CanAttack);
                challengeCard.SetIsFrozen(boardCreature.IsFrozen);
                challengeCard.SetIsSilenced(boardCreature.IsSilenced ? 1 : 0);
                challengeCard.SetSpawnRank(boardCreature.SpawnRank);
                challengeCard.SetAbilities(boardCreature.Abilities);
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
        cards.Add(new CreatureCard("C0", "Blessed Newborn", 2));
        cards.Add(new CreatureCard("C1", "Temple Guardian", 1));
        //cards.Add(new CreatureCard("C2", "Cursed Imp", 1));
        cards.Add(new CreatureCard("C3", "Waterborne Razorback", 1));
        cards.Add(new SpellCard("C4", "Unstable Power", 4));
        cards.Add(new CreatureCard("C5", "Bombshell Bombadier", 1));
        cards.Add(new CreatureCard("C6", "Firebug Catelyn", 1));
        cards.Add(new CreatureCard("C7", "Marshwater Squealer", 1));
        cards.Add(new CreatureCard("C8", "Taji the Fearless", 1));
        cards.Add(new CreatureCard("C9", "Young Kyo", 1));
        cards.Add(new CreatureCard("C10", "Emberkitty", 1));
        cards.Add(new CreatureCard("C11", "Firestrided Tigress", 1));
        cards.Add(new CreatureCard("C12", "Unkindled Junior", 1));
        cards.Add(new CreatureCard("C13", "Flamebelcher", 1));
        cards.Add(new CreatureCard("C14", "Fireborn Menace", 1));
        cards.Add(new CreatureCard("C15", "Te'a Greenleaf", 1));
        cards.Add(new CreatureCard("C16", "Wave Charmer", 1));
        cards.Add(new CreatureCard("C17", "Poseidon's Handmaiden", 1));
        cards.Add(new CreatureCard("C18", "Nessa, Nature's Champion", 1));
        cards.Add(new SpellCard("C20", "Touch of Zeus", 1));
        cards.Add(new SpellCard("C21", "Raze to Ashes", 1));
        cards.Add(new SpellCard("C22", "Deep Freeze", 1));
        cards.Add(new SpellCard("C23", "Brr Brr Blizzard", 1));
        cards.Add(new SpellCard("C24", "Riot Up", 1));
        cards.Add(new SpellCard("C25", "Widespread Frostbite", 1));
        cards.Add(new SpellCard("C26", "Greedy Fingers", 1));
        cards.Add(new SpellCard("C27", "Silence of the Lambs", 1));
        cards.Add(new SpellCard("C28", "Mudslinging", 1));
        cards.Add(new SpellCard("C29", "Death Notice", 1));
        //cards.Add(new SpellCard("C30", "Spray n' Pray", 1));
        //cards.Add(new SpellCard("C31", "Battle Royale", 1));
        //cards.Add(new SpellCard("C32", "Grave-digging", 1));

        Deck chosen = new Deck(deckName, cards, DeckRaw.DeckClass.Hunter, owner: this);
        return chosen;
    }
}
