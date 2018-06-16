using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Card {
    //public enum CardType: byte { Creature, Spell, Weapon, Structure };

    [SerializeField]
    protected string id;
    public string Id => id;

    [SerializeField]
    protected string name;
    public string Name => name;

    //[SerializeField]
    //protected CardType category;
    //public CardType Category => category;

    [SerializeField]
    protected int cost;
    public int Cost
    {
        get { return cost; }
        set { cost = value; }
    }

    [SerializeField]
    protected string image;
    public string Image => image;

    protected Player owner;
    public Player Owner => owner;

    public CardObject wrapper;
}

public class CreatureCard : Card {
    [SerializeField]
    protected int attack;
    public int Attack => attack;

    [SerializeField]
    protected int health;
    public int Health => health;

    public CreatureCard(string id, string name, int cost, string image, int attack, int health, Player owner = null) {
        this.id = id;
        this.name = name;
        //this.category = category;
        this.cost = cost;
        this.image = image;
        this.owner = owner;

        this.attack = attack;
        this.health = health;
    }
}

public class SpellCard : Card
{
    private bool targeted;      //affects single target or whole board?
    public bool Targeted => targeted;
    public SpellCard(string id, string name, int cost, string image, Player owner = null)
    {
        this.id = id;
        this.name = name;
        //this.category = category;
        this.cost = cost;
        this.image = image;
        this.owner = owner;
    }
}

public class WeaponCard : Card
{
    [SerializeField]
    protected int attack;
    public int Attack => attack;

    [SerializeField]
    protected int durability;
    public int Durability => durability;

    public WeaponCard(string id, string name, int cost, string image, int attack, int durability, Player owner = null)
    {
        this.id = id;
        this.name = name;
        //this.category = category;
        this.cost = cost;
        this.image = image;
        this.owner = owner;

        this.attack = attack;
        this.durability = durability;
    }
}

public class StructureCard : Card
{
    public StructureCard(string id, string name, int cost, string image, int attack, int durability, Player owner = null)
    {
        this.id = id;
        this.name = name;
        //this.category = category;
        this.cost = cost;
        this.image = image;
        this.owner = owner;
    }
}