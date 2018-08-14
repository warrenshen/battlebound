using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    private string displayName;
    public string DisplayName => displayName;

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

    private int turnCount;

    private Board.PlayingField field;
    public Board.PlayingField Field => Field;

    private PlayerAvatar avatar;
    public PlayerAvatar Avatar => avatar;

    private List<Card> mulliganCards;

    private List<Card> keptMulliganCards; // Only used by player (you).
    public List<Card> KeptMulliganCards => keptMulliganCards;

    private List<int> replaceMulliganIndices;
    private List<Card> replaceMulliganCards;

    public Player(string id, string name, string displayName)
    {
        this.id = id;
        this.name = name;
        this.displayName = displayName;

        this.hasTurn = false;
        this.deck = GetDeck();
        this.deckSize = this.deck.Size();

        this.mana = 20;
        this.maxMana = 20;

        this.cardCount = this.deckSize;
        this.mode = PLAYER_STATE_MODE_NORMAL;
        this.turnCount = 0;

        this.replaceMulliganIndices = new List<int>();
        this.replaceMulliganCards = new List<Card>();

        this.hand = new Hand(this);
        this.avatar = new PlayerAvatar(this);
    }

    public Player(PlayerState playerState, string name)
    {
        this.id = playerState.Id;
        this.name = name;
        this.displayName = playerState.DisplayName;

        this.hasTurn = playerState.HasTurn == 1;
        this.deckSize = playerState.DeckSize;

        this.mana = playerState.ManaCurrent;
        this.maxMana = playerState.ManaMax;

        this.cardCount = playerState.CardCount;
        this.mode = playerState.Mode;
        this.turnCount = playerState.TurnCount;

        this.replaceMulliganIndices = new List<int>();
        this.replaceMulliganCards = new List<Card>();

        this.avatar = new PlayerAvatar(this, playerState);
    }

    public void Initialize(PlayerState playerState)
    {
        List<Card> handCards = playerState.GetCardsHand();
        this.hand = new Hand(this);
        AddCardsOnResume(handCards);
    }

    public string GetCardId()
    {
        return this.avatar.GetCardId();
    }

    public int GetHealth()
    {
        return this.avatar.Health;
    }

    public int TakeDamage(int amount)
    {
        return this.avatar.TakeDamage(amount);
    }

    public int Heal(int amount)
    {
        return this.avatar.Heal(amount);
    }

    public void PlayCard(BattleCardObject battleCardObject)
    {
        this.mana -= battleCardObject.GetCost();
        RenderMana();

        this.hand.RemoveByCardId(battleCardObject.GetCardId());
        this.hand.RepositionCards();
    }

    private void RenderMana()
    {
        TextMeshPro manaText = GameObject.Find(name + " Mana").GetComponent<TextMeshPro>();
        manaText.text = String.Format("{0}/{1}", mana.ToString(), maxMana.ToString());
    }

    public void EndTurn()
    {
        this.hasTurn = false;
        this.turnCount += 1;
    }

    public void NewTurn()
    {
        this.hasTurn = true;

        if (this.turnCount > 0)
        {
            this.maxMana = Math.Min(maxMana + 10, 100);
            this.mana = maxMana;
        }

        RenderTurnStart();
    }

    public void RenderTurnStart()
    {
        RenderMana();

        Vector3 targetPosition;
        if (this.name == "Player")
        {
            targetPosition = BattleManager.Instance.PlayerHandTransform.position;
        }
        else
        {
            targetPosition = BattleManager.Instance.EnemyHandTransform.position;
        }

        LeanTween.cancel(BattleManager.Instance.LightObject);
        LeanTween
            .move(
                BattleManager.Instance.LightObject,
                new Vector3(
                    targetPosition.x,
                    targetPosition.y,
                    BattleManager.Instance.LightObject.transform.position.z
                ),
                0.4f
            )
            .setEaseOutQuart();
    }

    public void RenderGameStart()
    {
        RenderMana();
    }

    /*
     * Create and add a device challenge move for draw card.
     */
    public int DrawCardDevice()
    {
        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(this.id);

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

        return BattleState.Instance().AddDeviceMove(challengeMove);
    }

    public void DrawCardMock(bool isMulligan = false)
    {
        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(this.id);

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
            challengeMoveAttributes.SetCard(drawnCard.GetChallengeCard(this.id));
            challengeMove.SetMoveAttributes(challengeMoveAttributes);
        }

        BattleState.Instance().AddServerMove(challengeMove);
    }

    /*
     * Only meant to be used for local dev - do not use willy nilly!
     */
    private void DrawCardForce()
    {
        DrawCardDevice();
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

    public bool IsModeMulligan()
    {
        return this.mode == PLAYER_STATE_MODE_MULLIGAN || this.mode == PLAYER_STATE_MODE_MULLIGAN_WAITING;
    }

    public void BeginMulligan(List<Card> mulliganCards)
    {
        this.mode = PLAYER_STATE_MODE_MULLIGAN;

        this.mulliganCards = new List<Card>(mulliganCards);
        this.keptMulliganCards = new List<Card>(mulliganCards);

        for (int i = 0; i < this.keptMulliganCards.Count; ++i)
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
        if (!FlagHelper.IsServerEnabled())
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
            BattleManager.Instance.SetBoardCenterText("Waiting on opponent mulligan..");
        }
        else
        {
            Debug.LogError("Resume mulligan called on player not in mulligan mode.");
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

        List<int> deckCardIndices = new List<int>();
        for (int i = 0; i < this.mulliganCards.Count; i += 1)
        {
            Card mulliganCard = this.mulliganCards[i];
            if (!cardIds.Contains(mulliganCard.Id))
            {
                deckCardIndices.Add(i);
            }
        }

        if (FlagHelper.IsServerEnabled())
        {
            BattleSingleton.Instance.SendChallengePlayMulliganRequest(cardIds);
        }
        else
        {
            ChallengeMove challengeMove = new ChallengeMove();
            challengeMove.SetPlayerId(this.id);
            challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_PLAY_MULLIGAN);

            ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
            moveAttributes.SetDeckCardIndices(deckCardIndices);

            challengeMove.SetMoveAttributes(moveAttributes);
            BattleState.Instance().AddServerMove(challengeMove);

            if (deckCardIndices.Count <= 0)
            {
                Player opponent = BattleState.Instance().GetOpponentByPlayerId(this.id);
                opponent.PlayMulliganMock();
            }
            else
            {
                foreach (int index in deckCardIndices)
                {
                    DrawCardMock(true);
                }
            }
        }
    }

    /*
     * Call on opponent in local development mode to simulate mulligan.
     */
    public void PlayMulliganMock()
    {
        if (!FlagHelper.IsServerEnabled())
        {
            if (this.Mode == Player.PLAYER_STATE_MODE_MULLIGAN)
            {
                List<int> deckCardIndices = new List<int> { 0, 2 };

                ChallengeMove challengeMove = new ChallengeMove();
                challengeMove = new ChallengeMove();
                challengeMove.SetPlayerId(this.Id);
                challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_PLAY_MULLIGAN);

                ChallengeMove.ChallengeMoveAttributes moveAttributes = new ChallengeMove.ChallengeMoveAttributes();
                moveAttributes.SetDeckCardIndices(deckCardIndices);

                challengeMove.SetMoveAttributes(moveAttributes);
                BattleState.Instance().AddServerMove(challengeMove);

                foreach (int _ in deckCardIndices)
                {
                    DrawCardMock(true);
                }

                challengeMove = new ChallengeMove();
                challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_FINISH_MULLIGAN);
                BattleState.Instance().AddServerMove(challengeMove);
            }
        }
    }

    public void PlayMulliganByIndices(List<int> replacedCardIndices)
    {
        if (this.mode != PLAYER_STATE_MODE_MULLIGAN)
        {
            Debug.LogError("Player not in mulligan mode but received play mulligan.");
            return;
        }

        this.replaceMulliganIndices = replacedCardIndices;

        // It is very important that we iterate downwards,
        // since the ReplaceCardByMulligan call removes cards from hand by index.
        for (int i = this.mulliganCards.Count - 1; i >= 0; i -= 1)
        {
            if (this.replaceMulliganIndices.Contains(i))
            {
                Card removeCard = this.mulliganCards[i];
                ReplaceCardByMulligan(removeCard, i);

                ChallengeMove challengeMove = new ChallengeMove();
                challengeMove.SetPlayerId(this.id);
                challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_DRAW_CARD_MULLIGAN);
                BattleState.Instance().AddDeviceMove(challengeMove);
            }
        }

        this.mode = PLAYER_STATE_MODE_MULLIGAN_WAITING;
    }

    private void ReplaceCardByMulligan(Card card, int index)
    {
        this.hand.RemoveByIndex(index);
        ReturnCardToDeck(card);
        BattleManager.Instance.UseCard(card.wrapper as BattleCardObject);
    }

    public void AddDrawnCardMulligan(Card card)
    {
        this.replaceMulliganCards.Add(card);

        if (this.replaceMulliganCards.Count == this.replaceMulliganIndices.Count)
        {
            this.deckSize -= this.replaceMulliganCards.Count;

            int replaceCardIndex = 0;
            foreach (int replaceMulliganIndex in replaceMulliganIndices)
            {
                Card mulliganCard = this.mulliganCards[replaceCardIndex];
                if (replaceCardIndex == this.replaceMulliganCards.Count - 1)
                {
                    EffectManager.Instance.OnDrawMulliganStart();
                    BattleCardObject battleCardObject = AddMulliganCard(
                        this.replaceMulliganCards[replaceCardIndex],
                        replaceMulliganIndex,
                        new UnityAction(() => EffectManager.Instance.OnDrawMulliganFinish(this.id))
                    );
                }
                else
                {
                    BattleCardObject battleCardObject = AddMulliganCard(
                        this.replaceMulliganCards[replaceCardIndex],
                        replaceMulliganIndex
                    );
                }
                replaceCardIndex += 1;
            }

            Player opponent = BattleState.Instance().GetOpponentByPlayerId(this.id);
            opponent.PlayMulliganMock();
        }
    }

    public void FinishMulligan(bool isActivePlayer, bool shouldCallEffectManager = false)
    {
        if (this.mode != PLAYER_STATE_MODE_MULLIGAN_WAITING)
        {
            Debug.LogError("Finish mulligan called when player not in mulligan waiting mode");
            return;
        }

        this.keptMulliganCards = new List<Card>();
        this.replaceMulliganCards = new List<Card>();
        this.mode = PLAYER_STATE_MODE_NORMAL;

        if (shouldCallEffectManager)
        {
            this.hand.RepositionCards(
                new UnityAction(() => EffectManager.Instance.OnFinishMulliganFinish())
            );
        }
        else
        {
            this.hand.RepositionCards();
        }

        if (isActivePlayer)
        {
            this.hasTurn = true;
            RenderTurnStart();
        }
    }

    public BattleCardObject AddMulliganCard(Card card, int index, UnityAction onAnimationFinish = null)
    {
        BattleCardObject battleCardObject = BattleManager.Instance.InitializeBattleCardObject(
            this,
            card,
            GetHandTransform()
        );

        foreach (HyperCard.Card.CustomSpriteParam spriteParam in battleCardObject.visual.SpriteObjects)
        {
            spriteParam.IsAffectedByFilters = true;
        }

        this.hand.InsertCardObject(battleCardObject, index);

        BattleManager.Instance.AnimateDrawCardForMulligan(
            this,
            battleCardObject,
            index,
            this.mulliganCards.Count,
            onAnimationFinish
        );

        return battleCardObject;
    }

    public void AddDrawnCardHandFull(Card card)
    {
        BattleCardObject battleCardObject = BattleManager.Instance.InitializeBattleCardObject(
            this,
            card,
            GetHandTransform()
        );

        this.deckSize -= 1;
        BattleManager.Instance.AnimateDrawCard(this, battleCardObject)
            .setOnComplete(() =>
            {
                CardTween
                    .move(battleCardObject, battleCardObject.transform.position, CardTween.TWEEN_DURATION)
                    .setOnComplete(() =>
                    {
                        battleCardObject.Burn(() => RepositionCards(() => EffectManager.Instance.OnDrawCardFinish()));
                    });
            });
    }

    public BattleCardObject AddDrawnCard(Card card)
    {
        BattleCardObject battleCardObject = BattleManager.Instance.InitializeBattleCardObject(
            this,
            card,
            GetHandTransform()
        );

        this.deckSize -= 1;
        this.hand.AddCardObject(battleCardObject);

        BattleManager.Instance.AnimateDrawCard(this, battleCardObject);
        return battleCardObject;
    }

    public BattleCardObject AddCardOnResume(Card card)
    {
        BattleCardObject battleCardObject = BattleManager.Instance.InitializeBattleCardObject(
            this,
            card,
            GetHandTransform()
        );

        this.hand.AddCardObject(battleCardObject);
        return battleCardObject;
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
        if (!FlagHelper.IsServerEnabled())
        {
            this.deck.AddCard(card);
        }

        this.deckSize += 1;
    }

    public int GetOpponentHandIndex(int handIndex)
    {
        return this.hand.Size() - 1 - handIndex;
    }

    public int GetNewCardRank()
    {
        int cardRank = this.cardCount;
        this.cardCount += 1;
        return cardRank;
    }

    private Transform GetHandTransform()
    {
        if (this.name == "Player")
        {
            return BattleManager.Instance.PlayerHandTransform;
        }
        else
        {
            return BattleManager.Instance.EnemyHandTransform;
        }
    }

    public void RepositionCards(UnityAction onRepositionFinish = null)
    {
        this.hand.RepositionCards(onRepositionFinish);
    }

    public string GetNewCardId()
    {
        return string.Format("{0}-{1}", this.id, GetNewCardRank());
    }

    private Deck GetDeck()
    {
        string deckName = PlayerPrefs.GetString("selectedDeck", "DeckA");
        List<Card> cards = new List<Card>();

        cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_BLESSED_NEWBORN, 2));
        cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_FIREBUG_CATELYN, 2));
        cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_TEMPLE_GUARDIAN, 1));
        cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_WATERBORNE_RAZORBACK, 1));
        cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_BOMBSHELL_BOMBADIER, 1));
        cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_MARSHWATER_SQUEALER, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_TAJI_THE_FEARLESS, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_YOUNG_KYO, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_EMBERKITTY, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_FIRESTRIDED_TIGRESS, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_UNKINDLED_JUNIOR, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_FLAMEBELCHER, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_FIREBORN_MENACE, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_TEA_GREENLEAF, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_WAVE_CHARMER, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_POSEIDONS_HANDMAIDEN, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_NESSA_NATURES_CHAMPION, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_BUBBLE_SQUIRTER, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_SWIFT_SHELLBACK, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_SENTIENT_SEAKING, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_CRYSTAL_SNAPPER, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_BATTLECLAD_GASDON, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_REDHAIRED_PALADIN, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_FIRESWORN_GODBLADE, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_RITUAL_HATCHLING, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_HELLBRINGER, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_HOOFED_LUSH, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_DIONYSIAN_TOSSPOT, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_SEAHORSE_SQUIRE, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_TRIDENT_BATTLEMAGE, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_SNEERBLADE, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_TIMEWARP_KINGPIN, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_LUX, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_THUNDEROUS_DESPERADO, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_CEREBOAROUS, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_GUPPEA, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_RHYNOKARP, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_PRICKLEPILLAR, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_ADDERSPINE_WEEVIL, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_THIEF_OF_NIGHT, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_POWER_SIPHONER, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_LIL_RUSTY, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_INFERNO_902, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_CHAR_BOT_451, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_MEGAPUNK, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_DUSK_DWELLER, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_TALUSREAVER, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_KRUL_PHANTOM_SKULLCRUSHER, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_BLUE_GIPSY_V3, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_FROSTLAND_THRASHER_8, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_CYBER_SENTINEL, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_PAL_V1, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_ARC_KNIGHT, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_ARMORED_WARDEN, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_GRAVEYARD_GUARDIAN, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_TERRATANK, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_CULYSSA, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_ABYSSAL_EEL, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_FIRESMITH_APPRENTICE, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_FORGEMECH, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_LIGHTHUNTER, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_BRINGER_OF_DAWN, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_ABYSSAL_EEL, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_EMILIA_AIRHEART, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_PEARL_NYMPH, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_TARA_SWAN_PRINCESS, 1));
        //cards.Add(new CreatureCard(GetNewCardId(), Card.CARD_NAME_FROSTSPORE, 1));

        cards.Add(new SpellCard(GetNewCardId(), Card.CARD_NAME_UNSTABLE_POWER, 4));
        cards.Add(new SpellCard(GetNewCardId(), Card.CARD_NAME_TOUCH_OF_ZEUS, 1));
        cards.Add(new SpellCard(GetNewCardId(), Card.CARD_NAME_RAZE_TO_ASHES, 1));
        cards.Add(new SpellCard(GetNewCardId(), Card.CARD_NAME_DEEP_FREEZE, 1));
        cards.Add(new SpellCard(GetNewCardId(), Card.CARD_NAME_BRR_BRR_BLIZZARD, 1));
        cards.Add(new SpellCard(GetNewCardId(), Card.CARD_NAME_RIOT_UP, 1));
        cards.Add(new SpellCard(GetNewCardId(), Card.CARD_NAME_WIDESPREAD_FROSTBITE, 1));
        cards.Add(new SpellCard(GetNewCardId(), Card.CARD_NAME_GREEDY_FINGERS, 1));
        cards.Add(new SpellCard(GetNewCardId(), Card.CARD_NAME_SILENCE_OF_THE_LAMBS, 1));
        cards.Add(new SpellCard(GetNewCardId(), Card.CARD_NAME_MUDSLINGING, 1));
        cards.Add(new SpellCard(GetNewCardId(), Card.CARD_NAME_DEATH_NOTE, 1));
        cards.Add(new SpellCard(GetNewCardId(), Card.CARD_NAME_BOMBS_AWAY, 1));
        cards.Add(new SpellCard(GetNewCardId(), Card.CARD_NAME_GRAVE_DIGGING, 1));
        cards.Add(new SpellCard(GetNewCardId(), Card.CARD_NAME_THE_SEVEN, 1));
        cards.Add(new SpellCard(GetNewCardId(), Card.CARD_NAME_BESTOWED_VIGOR, 1));
        cards.Add(new SpellCard(GetNewCardId(), Card.CARD_NAME_BATTLE_ROYALE, 1));

        Deck chosen = new Deck(deckName, cards);
        return chosen;
    }

    public PlayerState GeneratePlayerState()
    {
        PlayerState playerState = new PlayerState();

        playerState.SetId(this.id);
        playerState.SetDisplayName(this.displayName);
        playerState.SetHasTurn(this.hasTurn ? 1 : 0);
        playerState.SetTurnCount(this.turnCount);
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
            Board.PlayingField playingField = Board.Instance().GetFieldByPlayerId(this.id);
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
}
