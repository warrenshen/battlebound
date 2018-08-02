using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChallengeMove
{
    public const string MOVE_CATEGORY_PLAY_MULLIGAN = "MOVE_CATEGORY_PLAY_MULLIGAN";
    public const string MOVE_CATEGORY_FINISH_MULLIGAN = "MOVE_CATEGORY_FINISH_MULLIGAN";
    public const string MOVE_CATEGORY_END_TURN = "MOVE_CATEGORY_END_TURN";
    public const string MOVE_CATEGORY_DRAW_CARD = "MOVE_CATEGORY_DRAW_CARD";
    public const string MOVE_CATEGORY_DRAW_CARD_MULLIGAN = "MOVE_CATEGORY_DRAW_CARD_MULLIGAN";
    public const string MOVE_CATEGORY_DRAW_CARD_HAND_FULL = "MOVE_CATEGORY_DRAW_CARD_HAND_FULL";
    public const string MOVE_CATEGORY_DRAW_CARD_DECK_EMPTY = "MOVE_CATEGORY_DRAW_CARD_DECK_EMPTY";
    public const string MOVE_CATEGORY_PLAY_MINION = "MOVE_CATEGORY_PLAY_MINION";
    public const string MOVE_CATEGORY_PLAY_SPELL_TARGETED = "MOVE_CATEGORY_PLAY_SPELL_TARGETED";
    public const string MOVE_CATEGORY_PLAY_SPELL_UNTARGETED = "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED";
    public const string MOVE_CATEGORY_CARD_ATTACK = "MOVE_CATEGORY_CARD_ATTACK";
    public const string MOVE_CATEGORY_RANDOM_TARGET = "MOVE_CATEGORY_RANDOM_TARGET";
    public const string MOVE_CATEGORY_SUMMON_CREATURE = "MOVE_CATEGORY_SUMMON_CREATURE";
    public const string MOVE_CATEGORY_SUMMON_CREATURE_FIELD_FULL = "MOVE_CATEGORY_SUMMON_CREATURE_FIELD_FULL";
    public const string MOVE_CATEGORY_SUMMON_CREATURE_NO_CREATURE = "MOVE_CATEGORY_SUMMON_CREATURE_NO_CREATURE";
    public const string MOVE_CATEGORY_CONVERT_CREATURE = "MOVE_CATEGORY_CONVERT_CREATURE";
    public const string MOVE_CATEGORY_SURRENDER_BY_CHOICE = "MOVE_CATEGORY_SURRENDER_BY_CHOICE";
    public const string MOVE_CATEGORY_SURRENDER_BY_EXPIRE = "MOVE_CATEGORY_SURRENDER_BY_EXPIRE";
    public const string MOVE_CATEGORY_CHALLENGE_OVER = "MOVE_CATEGORY_CHALLENGE_OVER";

    [SerializeField]
    private string playerId;
    public string PlayerId => playerId;

    [SerializeField]
    private string category;
    public string Category => category;

    [SerializeField]
    private int rank;
    public int Rank => rank;

    [SerializeField]
    private ChallengeMoveAttributes attributes;
    public ChallengeMoveAttributes Attributes => attributes;

    public void SetPlayerId(string playerId)
    {
        this.playerId = playerId;
    }

    public void SetCategory(string category)
    {
        this.category = category;
    }

    public void SetMoveAttributes(ChallengeMoveAttributes attributes)
    {
        this.attributes = attributes;
    }

    public void SetRank(int rank)
    {
        this.rank = rank;
    }

    [System.Serializable]
    public class ChallengeMoveAttributes
    {
        [SerializeField]
        private string cardId;
        public string CardId => cardId;

        [SerializeField]
        private string fieldId;
        public string FieldId => fieldId;

        [SerializeField]
        private string targetId;
        public string TargetId => targetId;

        [SerializeField]
        private int handIndex;
        public int HandIndex => handIndex;

        [SerializeField]
        private int fieldIndex;
        public int FieldIndex => fieldIndex;

        [SerializeField]
        private List<int> deckCardIndices;
        public List<int> DeckCardIndices => deckCardIndices;

        [SerializeField]
        private ChallengeCard card;
        public ChallengeCard Card => card;

        public void SetCardId(string cardId)
        {
            this.cardId = cardId;
        }

        public void SetFieldId(string fieldId)
        {
            this.fieldId = fieldId;
        }

        public void SetTargetId(string targetId)
        {
            this.targetId = targetId;
        }

        public void SetHandIndex(int handIndex)
        {
            this.handIndex = handIndex;
        }

        public void SetFieldIndex(int fieldIndex)
        {
            this.fieldIndex = fieldIndex;
        }

        public void SetDeckCardIndices(List<int> deckCardIndices)
        {
            this.deckCardIndices = deckCardIndices;
        }

        public void SetCard(ChallengeCard card)
        {
            this.card = card;
        }
    }
}
