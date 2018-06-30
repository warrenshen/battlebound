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

[System.Serializable]
public class PlaySpellTargetedAttributes
{
    [SerializeField]
    private string fieldId;
    public string FieldId => fieldId;

    [SerializeField]
    private string targetId;
    public string TargetId => targetId;

    public PlaySpellTargetedAttributes(string fieldId, string targetId)
    {
        this.fieldId = fieldId;
        this.targetId = targetId;
    }
}

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
