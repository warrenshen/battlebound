using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Board
{
    public static readonly Dictionary<int, int> FIELD_INDEX_TO_FIELD_BACK_INDEX = new Dictionary<int, int>
    {
        { 0, 6 },
        { 1, 6 },
        { 2, 7 },
        { 3, 7 },
        { 4, 8 },
        { 5, 8 },
    };

    public static readonly Dictionary<int, List<int>> FIELD_BACK_INDEX_TO_FIELD_INDICES = new Dictionary<int, List<int>>
    {
        { 6, new List<int>{ 0, 1 } },
        { 7, new List<int>{ 2, 3 } },
        { 8, new List<int>{ 4, 5 } },
    };

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
        this.playerIdToField = new Dictionary<string, PlayingField>();
        this.playerIdToAvatar = new Dictionary<string, PlayerAvatar>();
    }

    public void RegisterPlayer(Player player)
    {
        PlayingField playingField = new PlayingField(player);
        this.playerIdToField[player.Id] = playingField;
        this.playerIdToAvatar[player.Id] = player.Avatar;
    }

    public void RegisterPlayer(
        Player player,
        ChallengeCard[] fieldCards,
        ChallengeCard[] fieldBackCards
    )
    {
        PlayingField playingField = new PlayingField(player);
        this.playerIdToField[player.Id] = playingField;
        this.playerIdToAvatar[player.Id] = player.Avatar;
        playingField.SpawnCardsFromChallengeState(fieldCards, fieldBackCards);
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

    public void CreateAndPlaceStructure(
        ChallengeCard challengeCard,
        int fieldIndex,
        bool shouldWarcry,
        bool isResume = false
    )
    {
        List<BoardStructure> aliveStructures =
            GetAliveStructuresByPlayerId(challengeCard.PlayerId);
        foreach (BoardStructure aliveStructure in aliveStructures)
        {
            if (aliveStructure.GetCardId() == challengeCard.Id)
            {
                Debug.LogError(
                    string.Format(
                        "Cannot place creature with card ID of existing structure: {0}",
                        challengeCard.Id
                    )
                );
                return;
            }
        }

        PlayingField playingField = this.playerIdToField[challengeCard.PlayerId];
        BoardStructure boardStructure = new BoardStructure(
            challengeCard,
            fieldIndex,
            isResume
        );
        playingField.Place(boardStructure, fieldIndex);
        boardStructure.SummonWithCallback(new UnityAction(() =>
        {
            if (shouldWarcry)
            {
                EffectManager.Instance.OnStructurePlay(
                    boardStructure.GetPlayerId(),
                    boardStructure.GetCardId()
                );
            }
        }));
    }

    public List<BoardCreature> GetCreaturesByStructure(BoardStructure boardStructure)
    {
        string playerId = boardStructure.GetPlayerId();
        int fieldBackIndex = boardStructure.FieldIndex;

        List<int> fieldIndices = FIELD_BACK_INDEX_TO_FIELD_INDICES[fieldBackIndex];

        List<BoardCreature> boardCreatures = new List<BoardCreature>();

        foreach (int fieldIndex in fieldIndices)
        {
            BoardCreature boardCreature = GetCreatureByPlayerIdAndIndex(playerId, fieldIndex);
            if (boardCreature != null)
            {
                boardCreatures.Add(boardCreature);
            }
        }

        return boardCreatures;
    }

    public BoardStructure GetStructureByCreature(BoardCreature boardCreature)
    {
        int fieldIndex = boardCreature.FieldIndex;
        int fieldBackIndex = FIELD_INDEX_TO_FIELD_BACK_INDEX[fieldIndex];
        PlayingField playingField = GetFieldByPlayerId(boardCreature.GetPlayerId());
        return playingField.GetStructureByIndex(fieldBackIndex);
    }

    public void RemoveCreature(BoardCreature boardCreature)
    {
        PlayingField playingField = this.playerIdToField[boardCreature.GetPlayerId()];
        playingField.Remove(boardCreature);
    }

    public void RemoveStructure(BoardStructure boardStructure)
    {
        PlayingField playingField = this.playerIdToField[boardStructure.GetPlayerId()];
        playingField.Remove(boardStructure);
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

    public List<BoardStructure> GetAliveStructuresByPlayerId(string playerId)
    {
        PlayingField playingField = GetFieldByPlayerId(playerId);
        return playingField.GetAliveStructures();
    }

    public List<BoardStructure> GetExposedStructuresByPlayerId(string playerId)
    {
        List<BoardStructure> boardStructures = new List<BoardStructure>();
        PlayingField playingField = GetFieldByPlayerId(playerId);

        foreach (int fieldBackIndex in new List<int> { 6, 7, 8 })
        {
            BoardStructure boardStructure = playingField.GetStructureByIndex(fieldBackIndex);
            if (boardStructure == null)
            {
                continue;
            }
            List<BoardCreature> boardCreatures = GetCreaturesByStructure(boardStructure);
            if (boardCreatures.Count <= 0)
            {
                boardStructures.Add(boardStructure);
            }
        }
        return boardStructures;
    }

    public List<BoardCreature> GetAliveCreaturesByPlayerIdExceptCardId(string playerId, string cardId)
    {
        return new List<BoardCreature>(
            GetAliveCreaturesByPlayerId(playerId).Where(
                boardCreature => boardCreature.GetCardId() != cardId
            )
        );
    }

    public List<BoardCreature> GetOpponentAliveCreaturesByPlayerId(string playerId)
    {
        string opponentId = BattleState.Instance().GetOpponentIdByPlayerId(playerId);
        return GetAliveCreaturesByPlayerId(opponentId);
    }

    public List<BoardCreature> GetOpponentFrozenCreaturesByPlayerId(string playerId)
    {
        return new List<BoardCreature>(
            GetOpponentAliveCreaturesByPlayerId(playerId)
                .Where(boardCreature => boardCreature.IsFrozen > 0)
        );
    }

    public BoardCreature GetCreatureByPlayerIdAndCardId(string playerId, string cardId)
    {
        PlayingField playingField = GetFieldByPlayerId(playerId);
        return playingField.GetCreatureByCardId(cardId);
    }

    public List<BoardCreature> GetCreaturesByPlayerIdsAndCardIds(
        List<string> playerIds,
        List<string> cardIds
    )
    {
        if (playerIds.Count != cardIds.Count)
        {
            Debug.LogError("Invalid arguments.");
            return new List<BoardCreature>();
        }
        else
        {
            List<BoardCreature> boardCreatures = new List<BoardCreature>();
            for (int i = 0; i < playerIds.Count; i += 1)
            {
                boardCreatures.Add(
                    GetCreatureByPlayerIdAndCardId(
                        playerIds[i],
                        cardIds[i]
                    )
                );
            }
            return boardCreatures;
        }
    }

    public BoardCreature GetCreatureByPlayerIdAndIndex(string playerId, int index)
    {
        PlayingField playingField = GetFieldByPlayerId(playerId);
        return playingField.GetCreatureByIndex(index);
    }

    public BoardStructure GetStructureByPlayerIdAndCardId(string playerId, string cardId)
    {
        PlayingField playingField = GetFieldByPlayerId(playerId);
        return playingField.GetStructureByCardId(cardId);
    }

    public BoardStructure GetStructureByPlayerIdAndIndex(string playerId, int fieldBackIndex)
    {
        PlayingField playingField = GetFieldByPlayerId(playerId);
        return playingField.GetStructureByIndex(fieldBackIndex);
    }

    public int GetInFrontIndexByPlayerIdAndCardId(string playerId, string cardId)
    {
        PlayingField playingField = GetFieldByPlayerId(playerId);
        int playerIndex = playingField.GetIndexByCardId(cardId);
        return 5 - playerIndex;
    }

    public BoardCreature GetInFrontCreatureByPlayerIdAndCardId(string playerId, string cardId)
    {
        int opponentIndex = GetInFrontIndexByPlayerIdAndCardId(playerId, cardId);
        return GetCreatureByPlayerIdAndIndex(
            BattleState.Instance().GetOpponentIdByPlayerId(playerId),
            opponentIndex
        );
    }

    public Transform GetInFrontBoardPlaceByPlayerIdAndCardId(string playerId, string cardId)
    {
        // Hack for unit tests - simply return null.
        if (BattleSingleton.Instance.IsEnvironmentTest())
        {
            return null;
        }

        int opponentIndex = GetInFrontIndexByPlayerIdAndCardId(playerId, cardId);
        return GetBoardPlaceByPlayerIdAndIndex(
            BattleState.Instance().GetOpponentIdByPlayerId(playerId),
            opponentIndex
        );
    }

    public int GetIndexByPlayerIdAndCardId(string playerId, string cardId)
    {
        PlayingField playingField = GetFieldByPlayerId(playerId);
        return playingField.GetIndexByCardId(cardId);
    }

    public int GetClosestAvailableIndexByPlayerId(string playerId, int index)
    {
        PlayingField playingField = GetFieldByPlayerId(playerId);
        List<int> offsets = new List<int> { 0, 1, -1, 2, -2, 3, -3, 4, -4, 5, -5 };

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
            Targetable targetable = field.GetCreatureByCardId(cardId);
            if (targetable != null)
            {
                return targetable;
            }
            else
            {
                return field.GetStructureByCardId(cardId);
            }
        }
    }

    public BoardCreature GetOpponentRandomCreature(string playerId)
    {
        List<BoardCreature> opponentCreatures = GetOpponentAliveCreaturesByPlayerId(playerId);
        if (opponentCreatures.Count <= 0)
        {
            return null;
        }

        if (BattleSingleton.Instance.IsEnvironmentTest())
        {
            return opponentCreatures[0];
        }
        else
        {
            int randomIndex = UnityEngine.Random.Range(0, opponentCreatures.Count);
            return opponentCreatures[randomIndex];
        }
    }

    public Targetable GetOpponentRandomTargetable(string playerId)
    {
        string opponentId = BattleState.Instance().GetOpponentIdByPlayerId(playerId);

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
        return playingField.GetAvailableFieldIndex();
    }

    public Vector3 GetFieldCenterByPlayerId(string playerId)
    {
        PlayingField playingField = GetFieldByPlayerId(playerId);
        Vector3 midpoint = (playingField.GetBoardPlaceByIndex(2).position + playingField.GetBoardPlaceByIndex(3).position) / 2.0f;
        return midpoint;
    }

    //[System.Serializable]
    public class PlayingField
    {
        [SerializeField]
        private BoardCreature[] field;

        [SerializeField]
        private BoardStructure[] fieldBack;

        [SerializeField]
        private Dictionary<int, Transform> indexToBoardPlace;

        public PlayingField(Player player)
        {
            this.field = new BoardCreature[6];
            this.fieldBack = new BoardStructure[3];
            this.indexToBoardPlace = new Dictionary<int, Transform>();

            CacheBoardPlaces(player);
        }

        private void CacheBoardPlaces(Player player)
        {
            if (!BattleSingleton.Instance.IsEnvironmentTest())
            {
                int activeChildCount = 0;
                GameObject boardParent = GameObject.Find(String.Format("{0} Board", player.Name));
                foreach (Transform child in boardParent.transform)
                {
                    if (child.gameObject.activeSelf)
                        activeChildCount++;
                }

                for (int i = 0; i < activeChildCount; i += 1)
                {
                    Transform boardPlace = GameObject.Find(
                        String.Format("{0} {1}", player.Name, i)
                    ).transform;
                    this.indexToBoardPlace[i] = boardPlace;
                }
            }
        }

        public void SpawnCardsFromChallengeState(
            ChallengeCard[] fieldCards,
            ChallengeCard[] fieldBackCards
        )
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

            for (int i = 0; i < 3; i += 1)
            {
                ChallengeCard challengeCard = fieldBackCards[i];

                if (challengeCard.Id == "EMPTY")
                {
                    continue;
                }

                Board.Instance().CreateAndPlaceStructure(challengeCard, i + 6, false, true);
            }
        }

        public bool Place(BoardCreature boardCreature, int index)
        {
            if (index < 0 || index > 5)
            {
                Debug.LogError("Invalid index for board creature.");
                return false;
            }
            if (this.field[index] != null)
            {
                Debug.LogError("Attempting to place unit where one exists.");
                return false;
            }
            else
            {
                this.field[index] = boardCreature;
                return true;
            }
        }

        public bool Place(BoardStructure boardStructure, int index)
        {
            if (index < 6 || index > 8)
            {
                Debug.LogError("Invalid index for board structure.");
                return false;
            }
            if (this.fieldBack[index - 6] != null)
            {
                Debug.LogError("Attempting to place unit where one exists.");
                return false;
            }
            else
            {
                this.fieldBack[index - 6] = boardStructure;
                return true;
            }
        }

        public Transform GetBoardPlaceByIndex(int index)
        {
            return this.indexToBoardPlace[index];
        }

        public bool IsPlaceEmpty(int index)
        {
            if (index < 6)
            {
                return this.field[index] == null;
            }
            else
            {
                return this.fieldBack[index - 6] == null;
            }
        }

        public BoardCreature GetCreatureByIndex(int index)
        {
            if (index < 0 || index > 5)
            {
                return null;
            }
            else
            {
                return this.field[index];
            }
        }

        public BoardStructure GetStructureByIndex(int fieldBackIndex)
        {
            if (fieldBackIndex < 6 || fieldBackIndex > 8)
            {
                Debug.Log(string.Format("Invalid field back index: {0}", fieldBackIndex));
                return null;
            }
            else
            {
                return this.fieldBack[fieldBackIndex - 6];
            }
        }

        public void Remove(BoardCreature boardCreature)
        {
            string cardId = boardCreature.GetCardId();

            int removeIndex = Array.FindIndex(
                this.field,
                element => element != null && element.GetCardId() == cardId
            );

            if (removeIndex < 0)
            {
                Debug.LogError(string.Format("Failed to remove card from hand with card ID: {0}", cardId));
                return;
            }

            this.field[removeIndex] = null;
        }

        public void Remove(BoardStructure boardStructure)
        {
            string cardId = boardStructure.GetCardId();

            int removeIndex = Array.FindIndex(
                this.fieldBack,
                element => element != null && element.GetCardId() == cardId
            );

            if (removeIndex < 0)
            {
                Debug.LogError(string.Format("Failed to remove card from hand with card ID: {0}", cardId));
                return;
            }

            this.fieldBack[removeIndex] = null;
        }

        public BoardCreature[] GetCreatures()
        {
            return this.field;
        }

        public int GetAvailableFieldIndex()
        {
            for (int i = 0; i < this.field.Length; i += 1)
            {
                BoardCreature boardCreature = GetCreatureByIndex(i);
                if (boardCreature == null)
                {
                    return i;
                }
            }

            return -1;
        }

        /*
         * @return List<BoardCreature> - list of non-null board creatures with positive health
         */
        public List<BoardCreature> GetAliveCreatures()
        {
            return new List<BoardCreature>(
                this.field.Where(
                    boardCreature => boardCreature != null &&
                    boardCreature.Health > 0
                )
            );
        }

        public List<BoardStructure> GetAliveStructures()
        {
            return new List<BoardStructure>(
                this.fieldBack.Where(
                    BoardStructure => BoardStructure != null &&
                    BoardStructure.Health > 0
                )
            );
        }

        public int GetIndexByCardId(string cardId)
        {
            return Array.FindIndex(
                this.field,
                creature => creature != null && creature.GetCardId() == cardId
            );
        }

        public BoardCreature GetCreatureByCardId(string cardId)
        {
            return Array.Find(
                this.field,
                boardCreature => boardCreature != null && boardCreature.GetCardId() == cardId
            );
        }

        public BoardStructure GetStructureByCardId(string cardId)
        {
            return Array.Find(
                this.fieldBack,
                boardStructure => boardStructure != null && boardStructure.GetCardId() == cardId
            );
        }
    }
}
