using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Board : MonoBehaviour
{
    [SerializeField]
    private Dictionary<string, string> playerIdToOpponentId;

    [SerializeField]
    private Dictionary<string, PlayingField> playerIdToField;

    [SerializeField]
    private Dictionary<string, PlayerAvatar> playerIdToAvatar;

    public static Board Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

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

    public void RegisterPlayer(Player player, PlayerState.ChallengeCard[] fieldCards)
    {
        PlayingField playingField = new PlayingField(player, fieldCards);
        this.playerIdToField[player.Id] = playingField;
        this.playerIdToAvatar[player.Id] = player.Avatar;
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
        return BattleManager.Instance.GetPlayerById(GetOpponentIdByPlayerId(playerId));
    }

    public bool IsBoardPlaceOpen(string playerId, int index)
    {
        PlayingField playingField = this.playerIdToField[playerId];
        return playingField.IsPlaceEmpty(index);
    }

    /*
     * Do NOT use to spawn creatures when resuming games.
     */
    public void CreateAndPlaceCreature(
        BattleCardObject battleCardObject,
        int index,
        int spawnRank
    )
    {
        StartCoroutine(
            "CreateAndPlaceCreatureHelper",
            new object[3] { battleCardObject, index, spawnRank }
        );
    }

    private IEnumerator CreateAndPlaceCreatureHelper(object[] args)
    {
        BattleCardObject battleCardObject = args[0] as BattleCardObject;
        int index = (int)args[1];
        int spawnRank = (int)args[2];

        PlayingField playingField = this.playerIdToField[battleCardObject.Owner.Id];
        Transform boardPlace = playingField.GetBoardPlaceByIndex(index);

        GameObject boardCreatureObject = new GameObject(battleCardObject.Card.Name);
        boardCreatureObject.transform.position = boardPlace.position;
        BoardCreature boardCreature = boardCreatureObject.AddComponent<BoardCreature>();

        playingField.Place(boardCreature, index);
        boardCreature.Initialize(battleCardObject.Card as CreatureCard, spawnRank);

        FXPoolManager.Instance.PlayEffect("SpawnVFX", boardPlace.position + new Vector3(0f, 0f, -0.1f));
        yield return new WaitForSeconds(0.2f);

        boardCreature.Summon(battleCardObject);

        Destroy(battleCardObject.gameObject);

        EffectManager.Instance.OnCreaturePlay(
            boardCreature.Owner.Id,
            boardCreature.GetCardId()
        );
    }

    public void RemoveCreatureByPlayerIdAndCardId(string playerId, string cardId)
    {
        BoardCreature boardCreature = GetCreatureByPlayerIdAndCardId(playerId, cardId);
        RemoveCreature(boardCreature);
    }

    private void RemoveCreature(BoardCreature creature)
    {
        PlayingField selected = this.playerIdToField[creature.Owner.Id];
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
        PlayingField playingField = GetFieldByPlayerId(playerId);
        int playerIndex = playingField.GetIndexByCardId(cardId);
        int opponentIndex = 5 - playerIndex;
        return GetBoardPlaceByPlayerIdAndIndex(this.playerIdToOpponentId[playerId], opponentIndex);
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

    public Targetable GetOpponentRandomTargetable(string playerId)
    {
        string opponentId = this.playerIdToOpponentId[playerId];

        List<Targetable> opponentTargetables = new List<Targetable>();

        List<BoardCreature> opponentCreatures = GetAliveCreaturesByPlayerId(opponentId);
        opponentTargetables.AddRange(opponentCreatures);
        opponentTargetables.Add(BattleManager.Instance.GetPlayerById(opponentId).Avatar);

        int randomIndex = UnityEngine.Random.Range(0, opponentTargetables.Count);
        return opponentTargetables[randomIndex];
    }

    [System.Serializable]
    public class PlayingField
    {
        [SerializeField]
        private BoardCreature[] creatures;

        [SerializeField]
        private Dictionary<int, Transform> indexToBoardPlace;

        private string playerId;

        public PlayingField(Player player)
        {
            this.playerId = player.Id;
            this.creatures = new BoardCreature[6];
            this.indexToBoardPlace = new Dictionary<int, Transform>();

            CacheBoardPlaces(player);
        }

        public PlayingField(Player player, PlayerState.ChallengeCard[] fieldCards)
        {
            this.playerId = player.Id;
            this.creatures = new BoardCreature[6];
            this.indexToBoardPlace = new Dictionary<int, Transform>();

            CacheBoardPlaces(player);
            SpawnCardsFromChallengeState(player, fieldCards);
        }

        private void CacheBoardPlaces(Player player)
        {
            for (int i = 0; i < 6; i += 1)
            {
                Transform boardPlace = GameObject.Find(
                    String.Format("{0} {1}", player.Name, i)
                ).transform;
                this.indexToBoardPlace[i] = boardPlace;
            }
        }

        private void SpawnCardsFromChallengeState(Player player, PlayerState.ChallengeCard[] fieldCards)
        {
            for (int i = 0; i < 6; i += 1)
            {
                PlayerState.ChallengeCard challengeCard = fieldCards[i];

                if (challengeCard.Id == "EMPTY")
                {
                    continue;
                }

                SpawnCreatureFromChallengeCard(
                    player,
                    challengeCard,
                    i
                );
            }
        }

        private void SpawnCreatureFromChallengeCard(
            Player player,
            PlayerState.ChallengeCard challengeCard,
            int index
        )
        {
            Transform boardPlace = GetBoardPlaceByIndex(index);

            CreatureCard creatureCard = challengeCard.GetCard(false) as CreatureCard;

            GameObject created = new GameObject(creatureCard.Name);
            BattleCardObject battleCardObject = created.AddComponent<BattleCardObject>();
            battleCardObject.Initialize(player, creatureCard);

            GameObject boardCreatureObject = new GameObject(battleCardObject.Card.Name);
            boardCreatureObject.transform.position = boardPlace.position;
            BoardCreature boardCreature = boardCreatureObject.AddComponent<BoardCreature>();
            boardCreature.InitializeFromChallengeCard(battleCardObject, challengeCard);

            Destroy(battleCardObject.gameObject);

            Place(boardCreature, index);
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
            creature.Die();
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
