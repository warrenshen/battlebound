using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTemplate
{
    public string name;
    public CardRaw.CardType cardType;
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

    //weapon
    public int durability;
    //to-do: special attributes?

    //spell
    public bool targeted;
    //to-do: effect id?

    public CardTemplate()
    {
        //empty constructor
    }
}
