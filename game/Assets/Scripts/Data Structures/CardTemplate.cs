using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTemplate
{
    public string name;
    public Card.CardType cardType;
    public string description; //to-do: need to add to view
    public int cost;
    public Card.RarityType rarity;
    public string frontImage;            //doesn't need field
    public string backImage;      //doesn't need field

    public Vector2 frontScale;
    public Vector2 frontOffset;

    public Vector2 backScale;
    public Vector2 backOffset;

    //creature
    public int attack;
    public int health;
    public string summonPrefab;
    public string effectPrefab; //shared between creature and spell cards
    public string[] abilities;

    //weapon
    public int durability;
    //to-do: special attributes?

    //spell
    public bool targeted;
    //to-do: effect id?

    public CardTemplate()
    {
        //empty constructor
        this.frontScale = new Vector2(1, 1);
        this.frontOffset = new Vector2(0, 0);

        this.backScale = new Vector2(1, 1.66F);
        this.backOffset = new Vector2(0, -0.66F);

        this.rarity = Card.RarityType.Common;
        this.cardType = Card.CardType.Creature;
        this.abilities = new string[4];
    }
}