using UnityEngine;

[System.Serializable]
public class ChallengeMove
{
	[SerializeField]
	private string playerId;
	public string PlayerId => playerId;
    
	[SerializeField]
	private string category;
	public string Category => category;

	[SerializeField]
	private Attributes moveAttributes;
	public Attributes MoveAttributes => moveAttributes;
    
	[System.Serializable]
	public class Attributes
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

	public void SetMoveAttributes(Attributes moveAttributes)
	{
		this.moveAttributes = moveAttributes;	
	}
}
