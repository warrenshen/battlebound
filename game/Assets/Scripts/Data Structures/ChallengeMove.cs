using UnityEngine;

[System.Serializable]
public class ChallengeMove
{
	public static string MOVE_CATEGORY_SURRENDER_BY_CHOICE = "MOVE_CATEGORY_SURRENDER_BY_CHOICE";
	public static string MOVE_CATEGORY_SURRENDER_BY_EXPIRE = "MOVE_CATEGORY_SURRENDER_BY_EXPIRE";
	public static string MOVE_CATEGORY_END_TURN = "MOVE_CATEGORY_END_TURN";
	public static string MOVE_CATEGORY_DRAW_CARD = "MOVE_CATEGORY_DRAW_CARD";
	public static string MOVE_CATEGORY_PLAY_MINION = "MOVE_CATEGORY_PLAY_MINION";
	public static string MOVE_CATEGORY_PLAY_SPELL_GENERAL = "MOVE_CATEGORY_PLAY_SPELL_GENERAL";
	public static string MOVE_CATEGORY_PLAY_SPELL_TARGETED = "MOVE_CATEGORY_PLAY_SPELL_TARGETED";
	public static string MOVE_CATEGORY_CARD_ATTACK = "MOVE_CATEGORY_CARD_ATTACK";

	[SerializeField]
	private string playerId;
	public string PlayerId => playerId;

	[SerializeField]
	private string category;
	public string Category => category;

	[SerializeField]
	private ChallengeMoveAttributes attributes;
	public ChallengeMoveAttributes Attributes => attributes;

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
		private PlayerState.ChallengeCard card;
		public PlayerState.ChallengeCard Card => card;

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

		public void SetCard(PlayerState.ChallengeCard card)
		{
			this.card = card;
		}
	}

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
}
