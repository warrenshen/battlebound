using System.Collections;
using System.Collections.Generic;
using System;
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

    [SerializeField]
    private Dictionary<string, Dictionary<int, Transform>> playerIdToIndexToBoardPlace;

    public static Board Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        this.playerIdToOpponentId = new Dictionary<string, string>();
        this.playerIdToField = new Dictionary<string, PlayingField>();
        this.playerIdToAvatar = new Dictionary<string, PlayerAvatar>();
        this.playerIdToIndexToBoardPlace = new Dictionary<string, Dictionary<int, Transform>>();
    }

    public void RegisterPlayer(Player player)
    {
        CacheBoardPlaces(player);

        PlayingField playingField = new PlayingField(player);
        this.playerIdToField[player.Id] = playingField;
        this.playerIdToAvatar[player.Id] = player.Avatar;
    }

    public void RegisterPlayer(Player player, Card[] fieldCards, int[] spawnRanks)
    {
        CacheBoardPlaces(player);

        PlayingField playingField = new PlayingField(player, fieldCards, spawnRanks);
        this.playerIdToField[player.Id] = playingField;
        this.playerIdToAvatar[player.Id] = player.Avatar;
    }

    private void CacheBoardPlaces(Player player)
    {
        this.playerIdToIndexToBoardPlace[player.Id] = new Dictionary<int, Transform>();

        for (int i = 0; i < 6; i += 1)
        {
            Transform boardPlace = GameObject.Find(
                String.Format("{0} {1}", player.Name, i)
            ).transform;
            this.playerIdToIndexToBoardPlace[player.Id][i] = boardPlace;
        }
    }

    public void RegisterPlayerOpponent(string playerId, string opponentId)
    {
        this.playerIdToOpponentId[playerId] = opponentId;
    }

    public bool IsBoardPlaceOpen(string playerId, int index)
    {
        PlayingField playingField = this.playerIdToField[playerId];
        return playingField.IsPlaceEmpty(index);
    }

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

        Transform boardPlace = this.playerIdToIndexToBoardPlace[battleCardObject.Owner.Id][index];

        FXPoolManager.Instance.PlayEffect("SpawnVFX", boardPlace.position + new Vector3(0f, 0f, -0.1f));

        yield return new WaitForSeconds(0.2f);

        GameObject boardCreatureObject = new GameObject(battleCardObject.Card.Name);
        boardCreatureObject.transform.position = boardPlace.position;
        BoardCreature boardCreature = boardCreatureObject.AddComponent<BoardCreature>();
        boardCreature.Initialize(battleCardObject, spawnRank);

        Destroy(battleCardObject.gameObject);

        PlayingField playingField = this.playerIdToField[boardCreature.Owner.Id];
        playingField.Place(boardCreature, index);

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
        int opponentIndex = 6 - playerIndex;
        Debug.Log(playerIndex);
        Debug.Log(opponentIndex);
        return GetCreatureByPlayerIdAndIndex(this.playerIdToOpponentId[playerId], opponentIndex);
    }

    public Targetable GetTargetableByPlayerIdAndCardId(string playerId, string cardId)
    {
        if (cardId == PlayerAvatar.TARGET_ID_FACE)
        {
            Debug.Log(playerId);
            return this.playerIdToAvatar[playerId];
        }
        else
        {
            PlayingField field = GetFieldByPlayerId(playerId);
            return field.GetCreatureByCardId(cardId);
        }
    }

    public void OnPlayerStartTurn(string playerId)
    {
        GetFieldByPlayerId(playerId).RunCreatureStartTurns();
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
        }

        public PlayingField(Player player, Card[] cards, int[] spawnRanks)
        {
            this.playerId = player.Id;
            this.creatures = new BoardCreature[6];

            SpawnCards(player, cards, spawnRanks);
        }

        private void SpawnCards(Player player, Card[] cards, int[] spawnRanks)
        {
            for (int i = 0; i < 6; i += 1)
            {
                Card card = cards[i];
                int spawnRank = spawnRanks[i];

                if (card.Id == "EMPTY")
                {
                    continue;
                }

                GameObject created = new GameObject(card.Name);
                BattleCardObject battleCardObject = created.AddComponent<BattleCardObject>();
                battleCardObject.Initialize(player, card);

                Board.Instance.CreateAndPlaceCreature(
                    battleCardObject,
                    i,
                    spawnRank
                );
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

        public bool IsPlaceEmpty(int index)
        {
            return this.creatures[index] == null;
        }

        public BoardCreature GetCreatureByIndex(int index)
        {
            return this.creatures[index];
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

        public void RunCreatureStartTurns()
        {
            foreach (BoardCreature creature in creatures)
            {
                if (creature == null)
                {
                    continue;
                }

                creature.OnStartTurn();
            }
        }
    }
}
