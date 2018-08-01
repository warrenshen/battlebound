using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardWeapon : MonoBehaviour
{
    [SerializeField]
    private int attack;
    public int Attack => attack;

    private int durability;
    public int Durability => durability;

    private Player wielder;
    public Player Wielder => wielder;


    public void Initialize(Player player, WeaponCard card)
    {
        this.attack = card.Attack;
        this.durability = card.Durability;
        this.wielder = player;
    }

    public int AttackMade(Targetable target)
    {
        int damageDone = 0;

        target.TakeDamage(this.attack);
        if (!target.IsAvatar)
        {
            BoardCreature targetCreature = target as BoardCreature;
            damageDone = this.wielder.TakeDamage(targetCreature.GetAttack());
        }
        //to-do: updated weapon durability rendering
        this.durability -= 1;
        if (CheckBroken())
            Destroy(gameObject);

        return damageDone;
    }

    private bool CheckBroken()
    {
        return this.durability <= 0;
    }

}
