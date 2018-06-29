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

    public void AttackMade(Targetable target)
    {
        target.TakeDamage(this.attack);
        if (!target.IsAvatar)
        {
            BoardCreature targetCreature = target as BoardCreature;
            bool alive = this.wielder.Avatar.TakeDamage(targetCreature.Attack);  //true if alive, false if not
        }
        //to-do: updated weapon durability rendering
        this.durability -= 1;
        if (CheckBroken())
            Destroy(gameObject);
    }

    private bool CheckBroken()
    {
        return this.durability <= 0;
    }

}
