using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class Board : MonoBehaviour
{
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

        this.playerIdToField = new Dictionary<string, PlayingField>();
        this.playerIdToAvatar = new Dictionary<string, PlayerAvatar>();
        this.playerIdToIndexToBoardPlace = new Dictionary<string, Dictionary<int, Transform>>();
    }

    public void RegisterPlayer(Player player)
    {
        PlayingField playingField = new PlayingField(player);
        this.playerIdToField[player.Id] = playingField;
        this.playerIdToAvatar[player.Id] = player.Avatar;
        this.playerIdToIndexToBoardPlace[player.Id] = new Dictionary<int, Transform>();

        for (int i = 0; i < 6; i += 1)
        {
            Transform boardPlace = GameObject.Find(
                String.Format("{0} {1}", player.Name, i)
            ).transform;
            this.playerIdToIndexToBoardPlace[player.Id][i] = boardPlace;
        }
    }

    public void CreateAndPlaceCreature(CardObject cardObject, int index)
    {
        StartCoroutine(
            "CreateAndPlaceCreatureHelper",
            new object[2] { cardObject, index }
        );
    }

    private IEnumerator CreateAndPlaceCreatureHelper(object[] args)
    {
        CardObject cardObject = args[0] as CardObject;
        int index = (int)args[1];

        Transform boardPlace = this.playerIdToIndexToBoardPlace[cardObject.Owner.Id][index];

        FXPoolManager.Instance.PlayEffect("SpawnVFX", boardPlace.position + new Vector3(0f, 0f, -0.1f));

        yield return new WaitForSeconds(0.2f);

        GameObject boardCreatureGO = new GameObject(cardObject.Card.Name);
        boardCreatureGO.transform.position = boardPlace.position;
        BoardCreature boardCreature = boardCreatureGO.AddComponent<BoardCreature>();
        boardCreature.Initialize(cardObject);

        Destroy(cardObject.gameObject);

        boardCreature.OnPlay();

        PlayingField playingField = this.playerIdToField[boardCreature.Owner.Id];
        playingField.Place(boardCreature, index);
    }

    public void RemoveCreature(BoardCreature creature)
    {
        PlayingField selected = this.playerIdToField[creature.Owner.Id];
        selected.Remove(creature);
    }

    public void RegisterPlayer(Player player, Card[] fieldCards)
    {
        PlayingField playingField = new PlayingField(player, fieldCards);
        this.playerIdToField[player.Id] = playingField;
        this.playerIdToAvatar[player.Id] = player.Avatar;
    }

    public PlayingField GetFieldByPlayerId(string playerId)
    {
        return this.playerIdToField[playerId];
    }

    public BoardCreature GetCreatureByPlayerIdAndCardId(string playerId, string cardId)
    {
        PlayingField field = GetFieldByPlayerId(playerId);
        return field.GetCreatureByCardId(cardId);
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

    public void OnPlayerStartTurn(string playerId)
    {
        GetFieldByPlayerId(playerId).RunCreatureStartTurns();
    }

    public void OnPlayerEndTurn(string playerId)
    {
        GetFieldByPlayerId(playerId).RunCreatureEndTurns();
    }

    [System.Serializable]
    public class PlayingField
    {
        [SerializeField]
        private BoardCreature[] creatures;

        [SerializeField]
        private Dictionary<int, Transform> indexToBoardPlace;
        //List<Artifact> artifacts;

        private string playerId;

        public PlayingField(Player player)
        {
            this.playerId = player.Id;
            this.creatures = new BoardCreature[6];
        }

        public PlayingField(Player player, Card[] cards)
        {
            this.playerId = player.Id;
            this.creatures = new BoardCreature[6];

            SpawnCards(player, cards);
        }

        private void SpawnCards(Player player, Card[] cards)
        {
            for (int i = 0; i < 6; i += 1)
            {
                Card card = cards[i];

                if (card.Id == "EMPTY")
                {
                    continue;
                }

                GameObject cardObjectGO = new GameObject(card.Name);
                CardObject cardObject = cardObjectGO.AddComponent<CardObject>();
                cardObject.InitializeCard(player, card);

                Board.Instance.CreateAndPlaceCreature(cardObject, i);
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

        public BoardCreature GetCreatureByIndex(int index)
        {
            return this.creatures[index];
        }

        public void Remove(BoardCreature creature)
        {
            for (int i = 0; i < creatures.Length; i++)
            {
                if (creatures[i] == creature)
                    creatures[i] = null;
            }
        }

        public BoardCreature[] GetCreatures()
        {
            return creatures;
        }

        public BoardCreature GetCreatureByCardId(string cardId)
        {
            return Array.Find(
                this.creatures,
                creature => creature != null && creature.CreatureCard.Id == cardId
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

        public void RunCreatureEndTurns()
        {
            foreach (BoardCreature creature in creatures)
            {
                if (creature == null)
                {
                    continue;
                }

                creature.OnEndTurn();
            }
        }
    }
}
