using UnityEngine.Events;

public interface IBoardStructureObject
{
    void Initialize(BoardStructure boardStructure);

    void SummonWithCallback(UnityAction onSummonFinish);

    void TakeDamage(int amount);

    void Heal(int amount);

    void Die();

    void Redraw();
}
