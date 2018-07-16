using UnityEngine;
using UnityEngine.Events;

[System.Serializable]  //serialization won't work because abstract
public abstract class Targetable : MonoBehaviour
{
    protected bool isAvatar = false;
    public bool IsAvatar => isAvatar;

    protected Player owner;
    public Player Owner => owner;

    [SerializeField]
    protected int canAttack;
    public int CanAttack => canAttack;
    protected int maxAttacks;

    public abstract string GetCardId();
    public abstract string GetPlayerId();
    public abstract bool CanAttackNow();

    public abstract int Fight(Targetable other);
    public abstract int TakeDamage(int amount);
    public abstract int Heal(int amount);

    public abstract void OnStartTurn();
    //public abstract void OnEndTurn();
}
