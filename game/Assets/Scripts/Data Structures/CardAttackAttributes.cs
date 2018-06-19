using UnityEngine;

[System.Serializable]
public class CardAttackAttributes
{
    [SerializeField]
    private int fieldId;
	public int FieldId => fieldId;

	[SerializeField]
	private string targetId;
	public string TargetId => targetId;
}
