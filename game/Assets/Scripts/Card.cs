using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card {
    public enum CardType: byte { Creature, Spell, Weapon, Structure };

    [SerializeField]
    private string id;
    public string Id => id;

    //read-only
    [SerializeField]
    private string name;
    public string Name => name;

    [SerializeField]
    private CardType category;
    public CardType Category => category;

    [SerializeField]
    private int cost;
    public int Cost
    {
        get { return cost; }
        set { cost = value; }
    }

    [SerializeField]
    private int attack;
    public int Attack => attack;

    [SerializeField]
    private int health;
    public int Health => health;

    [SerializeField]
    private string image;
    public string Image => image;


    public Card(string id, string name, CardType category, int cost, int attack, int health, string image) {
        this.id = id;
        this.name = name;
        this.category = category;
        this.attack = attack;
        this.health = health;
        this.cost = cost;
        this.image = image;
    }
}
