using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardWeapon : MonoBehaviour {

    [SerializeField]
    private int attack;
    public int Attack => attack;

    private int durability;
    public int Durability => durability;

    private Player user;
    public Player User => user;


    public void Initialize(WeaponCard card) {
        this.attack = card.Attack;
        this.durability = card.Durability;
    }

    public void MakeAttack(BoardCreature creature) {
        creature.TakeDamage(this.attack);
        bool alive = user.TakeDamage(creature.Attack);  //true if alive, false if not

        this.durability -= 1;
        if (CheckBroken())
            Destroy(gameObject);
    }

    private bool CheckBroken() {
        return this.durability <= 0;
    }
	
}
