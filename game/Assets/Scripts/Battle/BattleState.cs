using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BattleState
{
    private const int BATTLE_STATE_NORMAL_MODE = 0;
    private const int BATTLE_STATE_MULLIGAN_MODE = 1;

    private int mode;

    private int spawnCount;
    public int SpawnCount => spawnCount;

    private int deviceMoveCount;
    private int serverMoveCount;

    private List<ChallengeCard> deadCards;

    private List<ChallengeMove> serverMoveQueue;
    private List<ChallengeMove> deviceMoveQueue;

    private List<ChallengeMove> serverMoves;

    private ChallengeEndState challengeEndState;

    private Player you;
    public Player You => you;

    private Player opponent;
    public Player Opponent => opponent;

    private int turnIndex;
    private List<Player> players;
    public List<Player> Players => players;

    private Player activePlayer;
    public Player ActivePlayer => activePlayer;

    private Dictionary<string, Player> playerIdToPlayer;

    private static BattleState instance;

    public static BattleState Instance()
    {
        if (BattleSingleton.Instance.IsEnvironmentTest() && instance == null)
        {
            throw new Exception("BattleState instance not set.");
        }
        if (instance == null)
        {
            instance = new BattleState();
        }
        // Note we do not call EffectManager.Instance.ReadyUp here,
        // as we leave that to the BattleManager.
        return instance;
    }

    public static BattleState InstantiateWithState(
        PlayerState playerState,
        PlayerState opponentState
    )
    {
        instance = new BattleState(
            playerState,
            opponentState,
            0,
            0,
            new List<ChallengeCard>(),
            new List<ChallengeMove>()
        );
        instance.RegisterPlayersWithState(
            playerState,
            opponentState
        );
        EffectManager.Instance.ReadyUp();
        return instance;
    }

    public static BattleState InstantiateWithState(
        PlayerState playerState,
        PlayerState opponentState,
        int moveCount,
        int spawnCount,
        List<ChallengeCard> deadCards,
        List<ChallengeMove> serverMoves
    )
    {
        instance = new BattleState(
            playerState,
            opponentState,
            moveCount,
            spawnCount,
            deadCards,
            serverMoves
        );
        instance.RegisterPlayersWithState(
            playerState,
            opponentState
        );
        EffectManager.Instance.ReadyUp();
        return instance;
    }

    private BattleState()
    {
        this.serverMoveQueue = new List<ChallengeMove>();
        this.deviceMoveQueue = new List<ChallengeMove>();

        this.serverMoveCount = 0;
        this.deviceMoveCount = 0;
        this.spawnCount = 0;

        this.deadCards = new List<ChallengeCard>();
        this.serverMoves = new List<ChallengeMove>();

        this.playerIdToPlayer = new Dictionary<string, Player>();
        this.players = new List<Player>();

        this.spawnCount = 0;

        this.you = new Player("Player", "Player", "pl4y3r");
        this.opponent = new Player("Enemy", "Enemy", "3n3my");

        this.playerIdToPlayer[this.you.Id] = this.you;
        this.playerIdToPlayer[this.opponent.Id] = this.opponent;

        this.players.Add(this.you);
        this.players.Add(this.opponent);

        Board.Instance().RegisterPlayer(this.you);
        Board.Instance().RegisterPlayer(this.opponent);
        Board.Instance().RegisterPlayerOpponent(this.you.Id, this.opponent.Id);
        Board.Instance().RegisterPlayerOpponent(this.opponent.Id, this.you.Id);

        if (!FlagHelper.IsServerEnabled())
        {
            GameStart();
        }
    }

    private BattleState(
        PlayerState playerState,
        PlayerState opponentState,
        int moveCount,
        int spawnCount,
        List<ChallengeCard> deadCards,
        List<ChallengeMove> serverMoves
    )
    {
        this.serverMoveQueue = new List<ChallengeMove>();
        this.deviceMoveQueue = new List<ChallengeMove>();

        this.serverMoveCount = moveCount;
        this.deviceMoveCount = moveCount;
        this.spawnCount = spawnCount;

        this.deadCards = deadCards;
        this.serverMoves = serverMoves;

        this.playerIdToPlayer = new Dictionary<string, Player>();
        this.players = new List<Player>();

        this.you = new Player(playerState, "Player");
        this.opponent = new Player(opponentState, "Enemy");

        this.playerIdToPlayer[this.you.Id] = this.you;
        this.playerIdToPlayer[this.opponent.Id] = this.opponent;

        this.players.Add(this.you);
        this.players.Add(this.opponent);

        Board.Instance().RegisterPlayerOpponent(this.you.Id, this.opponent.Id);
        Board.Instance().RegisterPlayerOpponent(this.opponent.Id, this.you.Id);
    }

    private void RegisterPlayersWithState(
        PlayerState playerState,
        PlayerState opponentState
    )
    {
        this.you.Initialize(playerState);
        this.opponent.Initialize(opponentState);

        Board.Instance().RegisterPlayer(this.you, playerState.Field);
        Board.Instance().RegisterPlayer(this.opponent, opponentState.Field);
    }

    public void GameStart()
    {
        if (FlagHelper.IsServerEnabled())
        {
            this.turnIndex = this.players.FindIndex(player => player.HasTurn);
            this.activePlayer = this.players[turnIndex % players.Count];
        }
        else
        {
            this.turnIndex = UnityEngine.Random.Range(0, players.Count);
            this.activePlayer = players[turnIndex % players.Count];
        }

        Player inactivePlayer = GetPlayerById(
            Board.Instance().GetOpponentIdByPlayerId(this.activePlayer.Id)
        );

        if (BattleSingleton.Instance.IsEnvironmentTest())
        {
            return;
        }

        if (FlagHelper.IsServerEnabled())
        {
            if (this.you.IsModeMulligan())
            {
                this.you.ResumeMulligan(
                    BattleSingleton.Instance.GetMulliganCards(this.you.Id)
                );
                this.opponent.ResumeMulligan(
                    BattleSingleton.Instance.GetMulliganCards(this.opponent.Id)
                );
                this.mode = BATTLE_STATE_MULLIGAN_MODE;

                activePlayer.RenderGameStart();
                inactivePlayer.RenderGameStart();
            }
            else
            {
                EndMulligan(this.you);
                EndMulligan(this.opponent);

                activePlayer.RenderTurnStart();
                inactivePlayer.RenderGameStart();
            }
        }
        else
        {
            this.mode = BATTLE_STATE_MULLIGAN_MODE;

            if (FlagHelper.ShouldSkipMulligan())
            {
                EndMulligan(this.you);
                EndMulligan(this.opponent);

                this.activePlayer.DrawCardsForce(3);
                inactivePlayer.DrawCardsForce(4);
            }
            else
            {
                this.activePlayer.BeginMulligan(this.activePlayer.PopCardsFromDeck(3));
                inactivePlayer.BeginMulligan(inactivePlayer.PopCardsFromDeck(4));
            }
        }
    }

    public void EndMulligan(Player player)
    {
        BattleManager.Instance.HideMulliganOverlay(player);

        if (this.mode != BATTLE_STATE_NORMAL_MODE)
        {
            this.mode = BATTLE_STATE_NORMAL_MODE;
            this.activePlayer.MulliganNewTurn();
            EffectManager.Instance.OnStartTurn(this.activePlayer.Id);
        }
    }

    public Player GetPlayerById(string playerId)
    {
        if (!this.playerIdToPlayer.ContainsKey(playerId))
        {
            throw new Exception(
                string.Format(
                    "Invalid key of options {0}: {1}",
                    string.Join(",", this.playerIdToPlayer.Keys),
                    playerId
                )
            );
        }
        return this.playerIdToPlayer[playerId];
    }

    public bool IsNormalMode()
    {
        return this.mode == BATTLE_STATE_NORMAL_MODE;
    }

    public bool CanReceiveChallengeMove()
    {
        return true;
    }

    public int GetNewSpawnRank()
    {
        int spawnRank = this.spawnCount;
        this.spawnCount += 1;
        return spawnRank;
    }

    private int GetServerMoveRank()
    {
        int rank = this.serverMoveCount;
        this.serverMoveCount += 1;
        return rank;
    }

    private int GetDeviceMoveRank()
    {
        int rank = this.deviceMoveCount;
        this.deviceMoveCount += 1;
        return rank;
    }

    public int AddServerMove(ChallengeMove challengeMove)
    {
        int rank = GetServerMoveRank();
        challengeMove.SetRank(rank);

        if (FlagHelper.IsLogVerbose())
        {
            Debug.Log("[MOCK] Server move queue: " + challengeMove.Rank);
            Debug.Log(JsonUtility.ToJson(challengeMove));
        }

        this.serverMoveQueue.Add(challengeMove);
        return rank;
    }

    public int AddDeviceMove(ChallengeMove challengeMove)
    {
        int rank = GetDeviceMoveRank();
        challengeMove.SetRank(rank);

        if (FlagHelper.IsLogVerbose())
        {
            Debug.Log("Device move queue: " + challengeMove.Rank);
            Debug.Log(JsonUtility.ToJson(challengeMove));
        }
        this.deviceMoveQueue.Add(challengeMove);

        return rank;
    }

    public List<ChallengeMove> GetServerMoves()
    {
        return this.serverMoves;
    }

    public List<ChallengeCard> GetDeadCards()
    {
        return this.deadCards;
    }

    public List<ChallengeCard> GetDeadCardsByPlayerId(string playerId)
    {
        //foreach (ChallengeCard deadCard in this.deadCards)
        //{
        //    Debug.Log(JsonUtility.ToJson(deadCard));
        //}
        return new List<ChallengeCard>(
            this.deadCards.Where(deadCard => deadCard.PlayerId == playerId)
        );
    }

    public void AddDeadCard(ChallengeCard deadCard)
    {
        this.deadCards.Add(deadCard);
    }

    public PlayerState GetPlayerState()
    {
        return this.you.GeneratePlayerState();
    }

    public PlayerState GetOpponentState()
    {
        return this.opponent.GeneratePlayerState();
    }

    public void ComparePlayerStates()
    {
        PlayerState devicePlayerState = GetPlayerState();
        PlayerState deviceOpponentState = GetOpponentState();

        if (!BattleSingleton.Instance.IsEnvironmentTest() && FlagHelper.IsServerEnabled())
        {
            bool doesMatch = BattleSingleton.Instance.ComparePlayerStates(
                devicePlayerState,
                deviceOpponentState,
                this.deviceMoveCount
            );

            if (!doesMatch)
            {
                ReloadBattleOnError();
            }
        }
    }

    /*
 * @param List<int> deckCardIndices - indices of cards opponent chose to put back in deck
 */
    private void ReceiveMovePlayMulligan(string playerId, List<int> deckCardIndices)
    {
        Player player = GetPlayerById(playerId);
        player.PlayMulliganByIndices(deckCardIndices);
    }

    private void ReceiveMoveFinishMulligan()
    {
        this.you.FinishMulligan();
        this.opponent.FinishMulligan();
        ComparePlayerStates(); // Compare state at the end of mulligan.
    }

    private void ReceiveMoveEndTurn(string playerId)
    {
        if (this.activePlayer.Id != playerId)
        {
            Debug.LogError("Device active player does not match challenge move player.");
            return;
        }

        NextTurn();
    }

    private void NextTurn()
    {
        activePlayer.EndTurn();

        EffectManager.Instance.OnEndTurn(
            activePlayer.Id,
            new UnityAction(ActualNextTurn)
        );
    }

    private void ActualNextTurn()
    {
        turnIndex++;
        activePlayer = players[turnIndex % players.Count];

        //do some turn transition render
        //SetBoardCenterText(string.Format("{0} Turn", activePlayer.Name));
        //SetPassiveCursor();

        activePlayer.NewTurn();
        EffectManager.Instance.OnStartTurn(activePlayer.Id);
    }

    private void ReceiveMoveDrawCard(string playerId, Card card)
    {
        Player player = GetPlayerById(playerId);
        player.AddDrawnCard(card);
    }

    private void ReceiveMoveDrawCardMulligan(string playerId, Card card)
    {
        Player player = GetPlayerById(playerId);
        player.AddDrawnCardMulligan(card);
    }

    private void ReceiveMoveDrawCardHandFull(string playerId, Card card)
    {
        Player player = GetPlayerById(playerId);
        player.AddDrawnCardHandFull(card);
    }

    private void ReceiveMoveDrawCardDeckEmpty(string playerId)
    {
        Player player = GetPlayerById(playerId);
        // TODO: animate and remove debug.
        Debug.Log("Receive move draw card deck empty");
        EffectManager.Instance.OnDrawCardFinish();
    }

    private void ReceiveMovePlayMinion(
        string playerId,
        string cardId,
        ChallengeCard challengeCard,
        int handIndex,
        int fieldIndex
    )
    {
        if (this.activePlayer.Id != playerId)
        {
            Debug.LogError("Device active player does not match challenge move player.");
            return;
        }

        if (FlagHelper.IsServerEnabled())
        {
            BattleCardObject battleCardObject = opponent.Hand.GetCardObjectByIndex(handIndex);
            battleCardObject.Reinitialize(challengeCard);
            opponent.PlayCard(battleCardObject);
            BattleManager.Instance.EnemyPlayCardToBoardAnim(battleCardObject, fieldIndex);
        }
        else
        {
            BattleCardObject battleCardObject = opponent.Hand.GetCardObjectByCardId(cardId);
            if (battleCardObject == null)
            {
                Debug.LogError(string.Format("Server demanded card to play, but none of id {0} was found.", cardId));
                return;
            }
            opponent.PlayCard(battleCardObject);
            BattleManager.Instance.EnemyPlayCardToBoardAnim(battleCardObject, fieldIndex);
        }
    }

    private void ReceiveMovePlaySpellTargeted(
        string playerId,
        string cardId,
        ChallengeCard challengeCard,
        int handIndex,
        string fieldId,
        string targetId
    )
    {
        if (this.activePlayer.Id != playerId)
        {
            Debug.LogError("Device active player does not match challenge move player.");
            return;
        }

        BoardCreature targetedCreature = Board.Instance().GetCreatureByPlayerIdAndCardId(fieldId, targetId);

        if (FlagHelper.IsServerEnabled())
        {
            int opponentHandIndex = opponent.GetOpponentHandIndex(handIndex);
            BattleCardObject battleCardObject = opponent.Hand.GetCardObjectByIndex(opponentHandIndex);
            battleCardObject.Reinitialize(challengeCard);

            opponent.PlayCard(battleCardObject);
            BattleManager.Instance.EnemyPlaySpellTargetedAnim(battleCardObject, targetedCreature);
        }
        else
        {
            BattleCardObject battleCardObject = opponent.Hand.GetCardObjectByCardId(cardId);
            if (battleCardObject == null)
            {
                Debug.LogError(string.Format("Demanded card to play, but none of id {0} was found.", cardId));
                return;
            }

            opponent.PlayCard(battleCardObject);
            BattleManager.Instance.EnemyPlaySpellTargetedAnim(battleCardObject, targetedCreature);
        }
    }

    private void ReceiveMovePlaySpellUntargeted(
        string playerId,
        string cardId,
        ChallengeCard challengeCard,
        int handIndex
    )
    {
        if (this.activePlayer.Id != playerId)
        {
            Debug.LogError("Device active player does not match challenge move player.");
            return;
        }

        if (FlagHelper.IsServerEnabled())
        {
            int opponentHandIndex = opponent.GetOpponentHandIndex(handIndex);
            BattleCardObject battleCardObject = opponent.Hand.GetCardObjectByIndex(opponentHandIndex);
            battleCardObject.Reinitialize(challengeCard);

            opponent.PlayCard(battleCardObject);
            BattleManager.Instance.EnemyPlaySpellUntargetedAnim(battleCardObject);
        }
        else
        {
            BattleCardObject battleCardObject = opponent.Hand.GetCardObjectByCardId(cardId);
            if (battleCardObject == null)
            {
                Debug.LogError(String.Format("Demanded card to play, but none of id {0} was found.", cardId));
                return;
            }

            opponent.PlayCard(battleCardObject);
            BattleManager.Instance.EnemyPlaySpellUntargetedAnim(battleCardObject);
        }
    }

    private void ReceiveMoveCardAttack(
        string playerId,
        string cardId,
        string fieldId,
        string targetId
    )
    {
        if (this.activePlayer.Id != playerId)
        {
            Debug.LogError("Device active player does not match challenge move player.");
            return;
        }

        Targetable attackingTargetable = Board.Instance().GetTargetableByPlayerIdAndCardId(playerId, cardId);
        Targetable defendingTargetable = Board.Instance().GetTargetableByPlayerIdAndCardId(fieldId, targetId);

        EffectManager.Instance.OnCreatureAttack(
            attackingTargetable,
            defendingTargetable
        );
    }

    private void ReceiveMoveRandomTarget(
        string playerId,
        ChallengeCard challengeCard,
        string fieldId,
        string targetId
    )
    {
        EffectManager.Instance.OnRandomTarget(
            playerId,
            challengeCard,
            fieldId,
            targetId
        );
    }

    private void ReceiveMoveSummonCreature(
        string playerId,
        ChallengeCard challengeCard,
        string fieldId,
        int fieldIndex
    )
    {
        Card card = challengeCard.GetCard();
        if (card.GetType() == typeof(CreatureCard))
        {
            Player player = GetPlayerById(playerId);

            Board.Instance().CreateAndPlaceCreature(
                challengeCard,
                fieldIndex,
                false
            );

            if (FlagHelper.IsServerEnabled())
            {
                GetNewSpawnRank();
                player.GetNewCardRank();
            }
            EffectManager.Instance.OnSummonCreatureFinish();
        }
        else
        {
            Debug.LogError("Invalid card category for summon creature move.");
        }
    }

    private void ReceiveMoveSummonCreatureFieldFull(
        string playerId,
        ChallengeCard challengeCard,
        string fieldId
    )
    {
        Card card = challengeCard.GetCard();
        if (card.GetType() == typeof(CreatureCard))
        {
            Player player = GetPlayerById(playerId);

            //Board.Instance().CreateAndPlaceCreature(
            //    challengeCard,
            //    fieldIndex,
            //    false
            //);

            if (FlagHelper.IsServerEnabled())
            {
                GetNewSpawnRank();
                player.GetNewCardRank();
            }
            EffectManager.Instance.OnSummonCreatureFinish();
        }
        else
        {
            Debug.LogError("Invalid card category for summon creature field full move.");
        }
    }

    private void ReceiveMoveConvertCreature(
        string playerId,
        string fieldId,
        string targetId
    )
    {
        EffectManager.Instance.OnConvertCreature(playerId, fieldId, targetId);
    }

    public void ReceiveMoveSurrenderByChoice(string playerId)
    {
        if (!FlagHelper.IsServerEnabled())
        {
            MockChallengeEnd(playerId);
        }

        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(Board.Instance().GetOpponentIdByPlayerId(playerId));
        challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_CHALLENGE_OVER);
        challengeMove.SetRank(GetDeviceMoveRank());
        AddDeviceMove(challengeMove);
    }

    public void ReceiveMoveSurrenderByExpire(string playerId)
    {
        if (!FlagHelper.IsServerEnabled())
        {
            MockChallengeEnd(playerId);
        }

        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(Board.Instance().GetOpponentIdByPlayerId(playerId));
        challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_CHALLENGE_OVER);
        challengeMove.SetRank(GetDeviceMoveRank());
        AddDeviceMove(challengeMove);
    }

    //{
    //    "id": "C14",
    //  "level": 0,
    //  "levelPrevious": 0,
    //  "exp": 2,
    //  "expMax": 10,
    //  "expPrevious": 1,
    //  "category": 0,
    //  "attack": 60,
    //  "health": 60,
    //  "cost": 70,
    //  "name": "Fireborn Menace",
    //  "description": "Battlecry: Deal 20 damage to any minion in front",
    //  "abilities": [
    //    16
    //  ]
    //}

    public void SetChallengeEndState(ChallengeEndState challengeEndState)
    {
        this.challengeEndState = challengeEndState;
    }

    public void ReceiveMoveChallengeOver(string winnerId)
    {
        if (this.challengeEndState == null)
        {
            Debug.LogError("Function should not be called unless challenge end state is set.");
            return;
        }

        GameObject.Destroy(GameObject.Find("BattleSingleton"));

        List<ExperienceCard> experienceCards = this.challengeEndState.ExperienceCards;
        if (this.you.Id == winnerId)
        {
            BattleManager.Instance.ShowBattleEndFX(experienceCards, true);
        }
        else
        {
            BattleManager.Instance.ShowBattleEndFX(experienceCards, false);
        }
    }

    public void ReceiveChallengeMove(ChallengeMove challengeMove)
    {
        // If device's log of server moves contains the new server move
        // already, skip this move. This can happen because the device adds
        // its own mock server moves to the log for optimistic rendering.
        foreach (ChallengeMove serverMove in GetServerMoves())
        {
            if (
                serverMove.PlayerId == this.you.Id &&
                serverMove.PlayerId == challengeMove.PlayerId &&
                serverMove.Category == challengeMove.Category &&
                serverMove.Rank == challengeMove.Rank
            )
            {
                if (FlagHelper.IsLogVerbose())
                {
                    Debug.Log("[SKIPPED] Server move queue: " + challengeMove.Rank);
                    Debug.Log(JsonUtility.ToJson(challengeMove));
                    return;
                }
            }
        }

        if (FlagHelper.IsLogVerbose())
        {
            Debug.Log("Server move queue: " + challengeMove.Rank);
            Debug.Log(JsonUtility.ToJson(challengeMove));
        }
        this.serverMoveQueue.Add(challengeMove);
        this.serverMoveCount += 1;
    }

    // Challenge moves that cannot be predicted by device.
    private static List<string> OPPONENT_SERVER_CHALLENGE_MOVES = new List<string>
    {
        ChallengeMove.MOVE_CATEGORY_PLAY_MULLIGAN,
        ChallengeMove.MOVE_CATEGORY_END_TURN,
        ChallengeMove.MOVE_CATEGORY_PLAY_MINION,
        ChallengeMove.MOVE_CATEGORY_CARD_ATTACK,
        ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_TARGETED,
        ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_UNTARGETED,
        ChallengeMove.MOVE_CATEGORY_SURRENDER_BY_CHOICE,
        ChallengeMove.MOVE_CATEGORY_SURRENDER_BY_EXPIRE,
    };

    // Challenge moves to skip since device has already performed them.
    private static List<string> PLAYER_SKIP_CHALLENGE_MOVES = new List<string>
    {
        ChallengeMove.MOVE_CATEGORY_PLAY_MINION,
        ChallengeMove.MOVE_CATEGORY_CARD_ATTACK,
        ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_TARGETED,
        ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_UNTARGETED,
    };

    /*
     * @return int - rank of move processed, -1 if no move processed
     */
    public int ProcessMoveQueue()
    {
        if (this.serverMoveQueue.Count <= 0)
        {
            return -1;
        }

        ChallengeMove serverMove = this.serverMoveQueue[0];
        this.serverMoves.Add(serverMove);

        if (
            serverMove.PlayerId == this.opponent.Id &&
            OPPONENT_SERVER_CHALLENGE_MOVES.Contains(serverMove.Category)
        )
        {
            Debug.Log("Device move queue: " + this.deviceMoveCount);
            this.deviceMoveCount += 1;
        }
        else if (
            serverMove.PlayerId == this.you.Id &&
            (
                serverMove.Category == ChallengeMove.MOVE_CATEGORY_PLAY_MULLIGAN ||
                serverMove.Category == ChallengeMove.MOVE_CATEGORY_END_TURN ||
                serverMove.Category == ChallengeMove.MOVE_CATEGORY_SURRENDER_BY_EXPIRE
            )
        )
        {
            Debug.Log("Device move queue: " + this.deviceMoveCount);
            this.deviceMoveCount += 1;
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_FINISH_MULLIGAN)
        {
            Debug.Log("Device move queue: " + this.deviceMoveCount);
            this.deviceMoveCount += 1;
        }
        else if (this.deviceMoveQueue.Count <= 0)
        {
            return -1;
        }
        else
        {
            ChallengeMove deviceMove = this.deviceMoveQueue[0];
            if (
                deviceMove.PlayerId != serverMove.PlayerId ||
                deviceMove.Category != serverMove.Category ||
                deviceMove.Rank != serverMove.Rank
            )
            {
                Debug.LogError("Device move does not match server move.");
                Debug.LogWarning(string.Format("PlayerId: {0} vs {1}.", deviceMove.PlayerId, serverMove.PlayerId));
                Debug.LogWarning(string.Format("Category: {0} vs {1}.", deviceMove.Category, serverMove.Category));
                Debug.LogWarning(string.Format("Rank: {0} vs {1}.", deviceMove.Rank, serverMove.Rank));

                this.deviceMoveQueue.RemoveAt(0);
                this.serverMoveQueue.RemoveAt(0);

                if (FlagHelper.IsServerEnabled())
                {
                    ReloadBattleOnError();
                }

                return -1;
            }

            this.deviceMoveQueue.RemoveAt(0);
        }

        this.serverMoveQueue.RemoveAt(0);

        if (
            serverMove.PlayerId == this.you.Id &&
            PLAYER_SKIP_CHALLENGE_MOVES.Contains(serverMove.Category)
        )
        {
            return -1;
        }

        if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_PLAY_MULLIGAN)
        {
            ReceiveMovePlayMulligan(
                serverMove.PlayerId,
                serverMove.Attributes.DeckCardIndices
            );
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_FINISH_MULLIGAN)
        {
            ReceiveMoveFinishMulligan();
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_END_TURN)
        {
            ReceiveMoveEndTurn(
                serverMove.PlayerId
            );
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_DRAW_CARD)
        {
            Card card = serverMove.Attributes.Card.GetCard();

            ReceiveMoveDrawCard(
                serverMove.PlayerId,
                card
            );
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_DRAW_CARD_MULLIGAN)
        {
            Card card = serverMove.Attributes.Card.GetCard();

            ReceiveMoveDrawCardMulligan(
                serverMove.PlayerId,
                card
            );
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_DRAW_CARD_HAND_FULL)
        {
            Card card = serverMove.Attributes.Card.GetCard();

            ReceiveMoveDrawCardHandFull(
                serverMove.PlayerId,
                card
            );
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_DRAW_CARD_DECK_EMPTY)
        {
            ReceiveMoveDrawCardDeckEmpty(
                serverMove.PlayerId
            );
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_DRAW_CARD_DECK_EMPTY)
        {
            Debug.LogError("Not supported.");
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_PLAY_MINION)
        {
            Card card = serverMove.Attributes.Card.GetCard();

            if (card.GetType() == typeof(CreatureCard))
            {
                ReceiveMovePlayMinion(
                    serverMove.PlayerId,
                    serverMove.Attributes.CardId,
                    serverMove.Attributes.Card,
                    serverMove.Attributes.HandIndex,
                    serverMove.Attributes.FieldIndex
                );
            }
            else
            {
                Debug.LogError("Invalid card category for play minion move");
            }
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_UNTARGETED)
        {
            Card card = serverMove.Attributes.Card.GetCard();

            if (card.GetType() == typeof(SpellCard))
            {
                ReceiveMovePlaySpellUntargeted(
                    serverMove.PlayerId,
                    serverMove.Attributes.CardId,
                    serverMove.Attributes.Card,
                    serverMove.Attributes.HandIndex
                );
            }
            else
            {
                Debug.LogError("Invalid card category for play spell general move");
            }
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_TARGETED)
        {
            Card card = serverMove.Attributes.Card.GetCard();

            if (card.GetType() == typeof(SpellCard))
            {
                ReceiveMovePlaySpellTargeted(
                    serverMove.PlayerId,
                    serverMove.Attributes.CardId,
                    serverMove.Attributes.Card,
                    serverMove.Attributes.HandIndex,
                    serverMove.Attributes.FieldId,
                    serverMove.Attributes.TargetId
                );
            }
            else
            {
                Debug.LogError("Invalid card category for play spell targeted move");
            }
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_CARD_ATTACK)
        {
            ReceiveMoveCardAttack(
                serverMove.PlayerId,
                serverMove.Attributes.CardId,
                serverMove.Attributes.FieldId,
                serverMove.Attributes.TargetId
            );
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_RANDOM_TARGET)
        {
            ReceiveMoveRandomTarget(
                serverMove.PlayerId,
                serverMove.Attributes.Card,
                serverMove.Attributes.FieldId,
                serverMove.Attributes.TargetId
            );
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE)
        {
            ReceiveMoveSummonCreature(
                serverMove.PlayerId,
                serverMove.Attributes.Card,
                serverMove.Attributes.FieldId,
                serverMove.Attributes.FieldIndex
            );
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_SUMMON_CREATURE_FIELD_FULL)
        {
            ReceiveMoveSummonCreatureFieldFull(
                serverMove.PlayerId,
                serverMove.Attributes.Card,
                serverMove.Attributes.FieldId
            );
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_SURRENDER_BY_CHOICE)
        {
            ReceiveMoveSurrenderByChoice(serverMove.PlayerId);
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_SURRENDER_BY_EXPIRE)
        {
            ReceiveMoveSurrenderByExpire(serverMove.PlayerId);
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_CONVERT_CREATURE)
        {
            ReceiveMoveConvertCreature(
                serverMove.PlayerId,
                serverMove.Attributes.FieldId,
                serverMove.Attributes.TargetId
            );
        }
        else if (serverMove.Category == ChallengeMove.MOVE_CATEGORY_CHALLENGE_OVER)
        {
            ReceiveMoveChallengeOver(serverMove.PlayerId);
        }
        else
        {
            Debug.LogError(string.Format("Unsupported challenge move category: {0}", serverMove.Category));
        }

        return serverMove.Rank;
    }

    private void ReloadBattleOnError()
    {
        if (FlagHelper.IsServerEnabled())
        {
            // TODO: show some prompt to user in this case?
            Application.LoadLevel("Battle");
        }
        else
        {
            Debug.LogError("Reload battle function called when not in server mode.");
        }
    }

    public void MockChallengeEnd(string loserId)
    {
        ChallengeEndState challengeEndState = new ChallengeEndState(
            this.you.Id,
            2,
            3
        );

        List<ExperienceCard> experienceCards = new List<ExperienceCard>();

        foreach (ChallengeMove serverMove in GetServerMoves())
        {
            if (
                serverMove.PlayerId == this.you.Id &&
                serverMove.Category == ChallengeMove.MOVE_CATEGORY_PLAY_MINION
            )
            {
                ChallengeCard challengeCard = serverMove.Attributes.Card;
                ExperienceCard experienceCard = new ExperienceCard(
                    challengeCard.Id,
                    challengeCard.Name,
                    2,
                    2,
                    8,
                    7,
                    10
                );

                experienceCards.Add(experienceCard);
            }
        }

        challengeEndState.SetExperienceCards(experienceCards);

        if (this.you.Id == loserId)
        {
            SetChallengeEndState(challengeEndState);
        }
        else
        {
            SetChallengeEndState(challengeEndState);
        }

        ChallengeMove challengeMove = new ChallengeMove();
        challengeMove.SetPlayerId(Board.Instance().GetOpponentIdByPlayerId(loserId));
        challengeMove.SetCategory(ChallengeMove.MOVE_CATEGORY_CHALLENGE_OVER);
        AddServerMove(challengeMove);
    }
}
