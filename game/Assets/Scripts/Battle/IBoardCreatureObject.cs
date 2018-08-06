using UnityEngine.Events;

public interface IBoardCreatureObject
{
    void Initialize(BoardCreature boardCreature);

    void SummonWithCallback(UnityAction onSummonFinish);

    void FightAnimationWithCallback(TargetableObject other, UnityAction onFightFinish);

    void TakeDamage(int amount);

    void Heal(int amount);

    void RenderDeathwish();

    void Die();

    bool HasAbility(string ability);

    void Redraw();
}
