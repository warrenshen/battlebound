using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Board
{
    [SerializeField]
    private Dictionary<string, PlayingField> playerIdToFields;

    [SerializeField]
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
        this.playerIdToFields = new Dictionary<string, PlayingField>();
        this.playerIdToAvatar = new Dictionary<string, PlayerAvatar>();
    }

    public void CreateAndPlaceCreature(CardObject cardObject, Vector3 position, int index)
    {
        GameObject boardCreatureGO = new GameObject(cardObject.Card.Name);
        boardCreatureGO.transform.position = position;
        BoardCreature boardCreature = boardCreatureGO.AddComponent<BoardCreature>();
        boardCreature.Initialize(cardObject);

        PlaceCreature(boardCreature, index);
    }

    public void PlaceCreature(BoardCreature boardCreature, int index)
    {
        PlayingField playingField = this.playerIdToFields[boardCreature.Owner.Id];
        playingField.Place(boardCreature, index);
    }

    public void RemoveCreature(BoardCreature creature)
    {
        PlayingField selected = this.playerIdToFields[creature.Owner.Id];
        selected.Remove(creature);
    }

    public void RegisterPlayer(Player player)
    {
        PlayingField playingField = new PlayingField(player);
        this.playerIdToFields[player.Id] = playingField;
        this.playerIdToAvatar[player.Id] = player.Avatar;
    }

    public void RegisterPlayer(Player player, Card[] fieldCards)
    {
        PlayingField playingField = new PlayingField(player, fieldCards);
        this.playerIdToFields[player.Id] = playingField;
        this.playerIdToAvatar[player.Id] = player.Avatar;
    }

    public PlayingField GetFieldByPlayerId(string playerId)
    {
        return this.playerIdToFields[playerId];
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

    public void RecoverCreaturesByPlayerId(string playerId)
    {
        GetFieldByPlayerId(playerId).RecoverCreatures();
    }

    [System.Serializable]
    public class PlayingField
    {
        [SerializeField]
        protected BoardCreature[] creatures;
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

        public void SpawnCards(Player player, Card[] cards)
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

                Transform boardPlace = GameObject.Find(String.Format("{0} {1}", player.Name, i)).transform;
                Board.Instance().CreateAndPlaceCreature(
                    cardObject,
                    boardPlace.position,
                    i
                );
            }
        }

        public BoardCreature GetCreatureByIndex(int index)
        {
            return this.creatures[index];
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

        public void RecoverCreatures()
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
