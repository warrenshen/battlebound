using UnityEngine;

[System.Serializable]
public class ChallengeMove
{
	public static string CATEGORY_END_TURN = "MOVE_CATEGORY_END_TURN";
	public static string CATEGORY_PLAY_CARD = "MOVE_CATEGORY_PLAY_CARD";
	public static string CATEGORY_CARD_ATTACK = "MOVE_CATEGORY_CARD_ATTACK";

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
		private int fieldIndex;
		public int FieldIndex => fieldIndex;

        public void SetCardId(string cardId)
		{
			this.cardId = cardId;
		}

        public void SetFieldId(string fieldId)
		{
			this.fieldId = fieldId;
		}

        public void SetTargetid(string targetId)
		{
			this.targetId = targetId;
		}

        public void SetFieldIndex(int fieldIndex)
		{
			this.fieldIndex = fieldIndex;
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
