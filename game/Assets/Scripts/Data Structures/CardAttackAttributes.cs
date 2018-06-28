using UnityEngine;

[System.Serializable]
public class CardAttackAttributes
{
    [SerializeField]
    private string fieldId;
    public string FieldId => fieldId;

    [SerializeField]
    private string targetId;
    public string TargetId => targetId;

    public CardAttackAttributes(string fieldId, string targetId)
    {
        this.fieldId = fieldId;
        this.targetId = targetId;
    }
}
