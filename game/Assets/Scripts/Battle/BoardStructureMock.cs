using UnityEngine;
using UnityEngine.Events;

public class BoardStructureMock : IBoardStructureObject
{
    private BoardStructure boardStructure;

    public void Initialize(BoardStructure boardStructure)
    {
        this.boardStructure = boardStructure;
    }

    public void SummonWithCallback(UnityAction onSummonFinish)
    {
        onSummonFinish.Invoke();
    }

    public void TakeDamage(int amount)
    { }

    public void Heal(int amount)
    { }

    public void Die()
    { }

    public void Redraw()
    { }
}
