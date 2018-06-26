using UnityEngine;

[System.Serializable]
public class PlayCardAttributes
{
    [SerializeField]
	private int fieldIndex;
	public int FieldIndex => fieldIndex;
    
	public PlayCardAttributes(int fieldIndex)
	{
		this.fieldIndex = fieldIndex;
	}
}
