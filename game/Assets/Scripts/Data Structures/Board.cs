using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Board
{
    private Dictionary<string, string> playerIdToOpponentId;

    private Dictionary<string, PlayingField> playerIdToField;

    private Dictionary<string, PlayerAvatar> playerIdToAvatar;

    private static Board instance;

    public static Board Instance()
    {
        if (instance == null)
        {
            instance = new Board();
        }
        return instance;
    }

    public Board()
    {
        this.playerIdToOpponentId = new Dictionary<string, string>();
        this.playerIdToField = new Dictionary<string, PlayingField>();
        this.playerIdToAvatar = new Dictionary<string, PlayerAvatar>();
    }

    public void RegisterPlayer(Player player)
    {
        PlayingField playingField = new PlayingField(player);
        this.playerIdToField[player.Id] = playingField;
        this.playerIdToAvatar[player.Id] = player.Avatar;
    }

    public void RegisterPlayer(Player player, ChallengeCard[] fieldCards)
    {
        PlayingField playingField = new PlayingField(player);
        this.playerIdToField[player.Id] = playingField;
        this.playerIdToAvatar[player.Id] = player.Avatar;
        playingField.SpawnCardsFromChallengeState(fieldCards);
    }

    public void RegisterPlayerOpponent(string playerId, string opponentId)
    {
        this.playerIdToOpponentId[playerId] = opponentId;
    }

    public string GetOpponentIdByPlayerId(string playerId)
    {
        return this.playerIdToOpponentId[playerId];
    }

    public Player GetOpponentByPlayerId(string playerId)
    {
        BattleState battleState = BattleState.Instance();
        if (battleState == null)
        {
            throw new Exception("BattleState does not exist.");
        }
        return battleState.GetPlayerById(GetOpponentIdByPlayerId(playerId));
    }

    public bool IsBoardPlaceOpen(string playerId, int index)
    {
        PlayingField playingField = this.playerIdToField[playerId];
        return playingField.IsPlaceEmpty(index);
    }

    public void CreateAndPlaceCreature(
        ChallengeCard challengeCard,
        int fieldIndex,
        bool shouldWarcry,
        bool isResume = false
    )
    {
        List<BoardCreature> aliveCreatures =
            GetAliveCreaturesByPlayerId(challengeCard.PlayerId);
        foreach (BoardCreature aliveCreature in aliveCreatures)
        {
            if (aliveCreature.GetCardId() == challengeCard.Id)
            {
                Debug.LogError(
                    string.Format(
                        "Cannot place creature with card ID of existing creature: {0}",
                        challengeCard.Id
                    )
                );
                return;
            }
        }

        PlayingField playingField = this.playerIdToField[challengeCard.PlayerId];
        BoardCreature boardCreature = new BoardCreature(
            challengeCard,
            fieldIndex,
            isResume
        );
        playingField.Place(boardCreature, fieldIndex);
        boardCreature.SummonWithCallback(new UnityAction(() =>
        {
            if (shouldWarcry)
            {
                EffectManager.Instance.OnCreaturePlay(
                    boardCreature.GetPlayerId(),
                    boardCreature.GetCardId()
                );
            }
        }));
    }

    public void RemoveCreatureByPlayerIdAndCardId(string playerId, string cardId)
    {
        BoardCreature boardCreature = GetCreatureByPlayerIdAndCardId(playerId, cardId);
        RemoveCreature(boardCreature);
    }

    private void RemoveCreature(BoardCreature creature)
    {
        PlayingField selected = this.playerIdToField[creature.GetPlayerId()];
        selected.Remove(creature);
    }

    public PlayingField GetFieldByPlayerId(string playerId)
    {
        return this.playerIdToField[playerId];
    }

    public Transform GetBoardPlaceByPlayerIdAndIndex(string playerId, int index)
    {
        return GetFieldByPlayerId(playerId).GetBoardPlaceByIndex(index);
    }

    public List<BoardCreature> GetAliveCreaturesByPlayerId(string playerId)
    {
        PlayingField playingField = GetFieldByPlayerId(playerId);
        return playingField.GetAliveCreatures();
    }

    public List<BoardCreature> GetOpponentAliveCreaturesByPlayerId(string playerId)
    {
        string opponentId = this.playerIdToOpponentId[playerId];
        return GetAliveCreaturesByPlayerId(opponentId);
    }

    public BoardCreature GetCreatureByPlayerIdAndCardId(string playerId, string cardId)
    {
        PlayingField playingField = GetFieldByPlayerId(playerId);
        return playingField.GetCreatureByCardId(cardId);
    }

    public BoardCreature GetCreatureByPlayerIdAndIndex(string playerId, int index)
    {
        PlayingField playingField = GetFieldByPlayerId(playerId);
        return playingField.GetCreatureByIndex(index);
    }

    public BoardCreature GetInFrontCreatureByPlayerIdAndCardId(string playerId, string cardId)
    {
        PlayingField playingField = GetFieldByPlayerId(playerId);
        int playerIndex = playingField.GetIndexByCardId(cardId);
        int opponentIndex = 5 - playerIndex;
        return GetCreatureByPlayerIdAndIndex(this.playerIdToOpponentId[playerId], opponentIndex);
    }

    public Transform GetInFrontBoardPlaceByPlayerIdAndCardId(string playerId, string cardId)
    {
        // Hack for unit tests - simply return null.
        if (BattleSingleton.Instance.IsEnvironmentTest())
        {
            return null;
        }

        PlayingField playingField = GetFieldByPlayerId(playerId);
        int playerIndex = playingField.GetIndexByCardId(cardId);
        int opponentIndex = 5 - playerIndex;
        return GetBoardPlaceByPlayerIdAndIndex(this.playerIdToOpponentId[playerId], opponentIndex);
    }

    public int GetIndexByPlayerIdAndCardId(string playerId, string cardId)
    {
        PlayingField playingField = GetFieldByPlayerId(playerId);
        return playingField.GetIndexByCardId(cardId);
    }

    public int GetClosestAvailableIndexByPlayerId(string playerId, int index)
    {
        PlayingField playingField = GetFieldByPlayerId(playerId);
        List<int> offsets = new List<int> { 1, -1, 2, -2, 3, -3, 4, -4, 5, -5 };

        foreach (int offset in offsets)
        {
            int currentIndex = index + offset;
            if (currentIndex < 0 || currentIndex > 5)
            {
                continue;
            }
            if (playingField.GetCreatureByIndex(currentIndex) == null)
            {
                return currentIndex;
            }
        }

        return -1;
    }

    /*
     * Returns list of three board creatures to the left, right, and in front
     * of creature with given player and card IDs. List can be length 0 - 3.
     */
    public List<BoardCreature> GetAdjacentCreaturesByPlayerIdAndCardId(string playerId, string cardId)
    {
        PlayingField playingField = GetFieldByPlayerId(playerId);
        int cardIndex = playingField.GetIndexByCardId(cardId);

        List<BoardCreature> adjacentCreatures = new List<BoardCreature>
        {
            playingField.GetCreatureByIndex(cardIndex - 1),
            playingField.GetCreatureByIndex(cardIndex + 1),
        };

        return new List<BoardCreature>(
            adjacentCreatures.Where(boardCreature => boardCreature != null)
        );
    }

    public Targetable GetTargetableByPlayerIdAndCardId(string playerId, string cardId)
    {
        if (cardId == PlayerAvatar.TARGET_ID_FACE)
        {
            return this.playerIdToAvatar[playerId];
        }
        else
        {
            PlayingField field = GetFieldByPlayerId(playerId);
            return field.GetCreatureByCardId(cardId);
        }
    }

    public BoardCreature GetOpponentRandomCreature(string playerId)
    {
        string opponentId = this.playerIdToOpponentId[playerId];

        List<BoardCreature> opponentCreatures = GetAliveCreaturesByPlayerId(opponentId);

        if (opponentCreatures.Count <= 0)
        {
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, opponentCreatures.Count);
        return opponentCreatures[randomIndex];
    }

    public Targetable GetOpponentRandomTargetable(string playerId)
    {
        string opponentId = this.playerIdToOpponentId[playerId];

        List<Targetable> opponentTargetables = new List<Targetable>();

        List<BoardCreature> opponentCreatures = GetAliveCreaturesByPlayerId(opponentId);
        opponentTargetables.AddRange(opponentCreatures);
        opponentTargetables.Add(BattleState.Instance().GetPlayerById(opponentId).Avatar);

        int randomIndex = UnityEngine.Random.Range(0, opponentTargetables.Count);
        return opponentTargetables[randomIndex];
    }

    public int GetAvailableFieldIndexByPlayerId(string playerId)
    {
        PlayingField playingField = GetFieldByPlayerId(playerId);
        BoardCreature[] boardCreatures = playingField.GetCreatures();

        for (int i = 0; i < boardCreatures.Length; i += 1)
        {
            BoardCreature boardCreature = playingField.GetCreatureByIndex(i);
            if (boardCreature == null)
            {
                return i;
            }
        }

        return -1;
    }

    [System.Serializable]
    public class PlayingField
    {
        [SerializeField]
        private BoardCreature[] creatures;

        [SerializeField]
        private Dictionary<int, Transform> indexToBoardPlace;

        public PlayingField(Player player)
        {
            this.creatures = new BoardCreature[6];
            this.indexToBoardPlace = new Dictionary<int, Transform>();

            CacheBoardPlaces(player);
        }

        private void CacheBoardPlaces(Player player)
        {
            if (!BattleSingleton.Instance.IsEnvironmentTest())
            {
                for (int i = 0; i < 6; i += 1)
                {
                    Transform boardPlace = GameObject.Find(
                        String.Format("{0} {1}", player.Name, i)
                    ).transform;
                    this.indexToBoardPlace[i] = boardPlace;
                }
            }
        }

        public void SpawnCardsFromChallengeState(ChallengeCard[] fieldCards)
        {
            for (int i = 0; i < 6; i += 1)
            {
                ChallengeCard challengeCard = fieldCards[i];

                if (challengeCard.Id == "EMPTY")
                {
                    continue;
                }

                Board.Instance().CreateAndPlaceCreature(challengeCard, i, false, true);
            }
        }

        public bool Place(BoardCreature creature, int index)
        {
            if (creatures[index] != null)
            {
                Debug.LogError("Attempting to place unit where one exists.");
                return false;
            }
            else
            {
                creatures[index] = creature;
                return true;
            }
        }

        public Transform GetBoardPlaceByIndex(int index)
        {
            return this.indexToBoardPlace[index];
        }

        public bool IsPlaceEmpty(int index)
        {
            return this.creatures[index] == null;
        }

        public BoardCreature GetCreatureByIndex(int index)
        {
            if (index < 0 || index > 5)
            {
                return null;
            }
            else
            {
                return this.creatures[index];
            }
        }

        public void Remove(BoardCreature creature)
        {
            for (int i = 0; i < creatures.Length; i++)
            {
                if (creatures[i] == creature)
                {
                    creatures[i] = null;
                }
            }
        }

        public BoardCreature[] GetCreatures()
        {
            return creatures;
        }

        /*
         * @return List<BoardCreature> - list of non-null board creatures with positive health
         */
        public List<BoardCreature> GetAliveCreatures()
        {
            List<BoardCreature> boardCreatures = new List<BoardCreature>();
            for (int i = 0; i < this.creatures.Length; i++)
            {
                BoardCreature boardCreature = this.creatures[i];
                if (boardCreature != null && boardCreature.Health > 0)
                {
                    boardCreatures.Add(creatures[i]);
                }
            }
            return boardCreatures;
        }

        public int GetIndexByCardId(string cardId)
        {
            return Array.FindIndex(
                this.creatures,
                creature => creature != null && creature.GetCardId() == cardId
            );
        }

        public BoardCreature GetCreatureByCardId(string cardId)
        {
            return Array.Find(
                this.creatures,
                creature => creature != null && creature.GetCardId() == cardId
            );
        }
    }
}
