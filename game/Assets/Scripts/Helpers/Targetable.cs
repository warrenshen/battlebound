using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]  //serialization won't work because abstract
public abstract class Targetable : MonoBehaviour
{
    protected bool isAvatar = false;
    public bool IsAvatar => isAvatar;

    public abstract string GetCardId();
    public abstract string GetPlayerId();

    protected Player owner;
    public Player Owner => owner;

    [SerializeField]
    protected int canAttack;
    public int CanAttack => canAttack;
    protected int maxAttacks;

    public abstract void Fight(Targetable other);
    public abstract bool TakeDamage(int amount);

    public void RecoverAttack()
    {
        this.canAttack = this.maxAttacks;
    }
}
