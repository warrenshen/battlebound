using UnityEngine;
using UnityEngine.Events;

public class BoardCreatureMock : IBoardCreatureObject
{
    private BoardCreature boardCreature;

    public void Initialize(BoardCreature boardCreature)
    {
        this.boardCreature = boardCreature;
    }

    public void SummonWithCallback(UnityAction onSummonFinish)
    {
        onSummonFinish.Invoke();
    }

    public void FightAnimationWithCallback(TargetableObject other, UnityAction onFightFinish)
    {
        onFightFinish.Invoke();
    }

    public void TakeDamage(int amount)
    { }

    public void Heal(int amount)
    { }

    public void RenderDeathwish()
    { }

    public void RenderWarcry()
    { }

    public void Die()
    { }

    public bool HasAbility(string ability)
    {
        return this.boardCreature.HasAbility(ability);
    }

    public void Redraw()
    { }
}
