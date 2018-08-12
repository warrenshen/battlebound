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

    public enum ClassColor { Neutral, Fire, Earth, Water, Dark, Light };
    public ClassColor classColor;

    public static Color COLOR_FIRE = new Color(0.6087f, 0.118f, 0.078f);
    public static Color COLOR_EARTH = new Color(0.024f, 0.463f, 0.29f); //new Color(0.310f, 0.533f, 0.463f);
    public static Color COLOR_WATER = new Color(0.204f, 0.184f, 0.576f);
    public static Color COLOR_DARK = new Color(0.255f, 0.067f, 0.412f);
    public static Color COLOR_LIGHT = new Color(1, 0.7647f, 0.1686f);

    public static Dictionary<ClassColor, Color> CLASS_TO_COLOR = new Dictionary<ClassColor, Color>()
    {
        { ClassColor.Neutral, Color.white },
        { ClassColor.Fire,  CardTemplate.COLOR_FIRE},
        { ClassColor.Earth, CardTemplate.COLOR_EARTH },
        { ClassColor.Water, CardTemplate.COLOR_WATER },
        { ClassColor.Dark, CardTemplate.COLOR_DARK },
        { ClassColor.Light, CardTemplate.COLOR_LIGHT }
    };

    //weapon
    public int durability;
    //to-do: special attributes?

    //spell
    public bool targeted;
    //to-do: effect id?

    public CardTemplate()
    {
        //empty constructor
        this.frontScale = new Vector2(0.65f, 0.85f);
        this.frontOffset = new Vector2(0, 0);

        this.backScale = new Vector2(1, 1);
        this.backOffset = new Vector2(0, 0);

        this.rarity = Card.RarityType.Common;
        this.cardType = Card.CardType.Creature;
        this.abilities = new string[4];
        this.classColor = ClassColor.Neutral;
    }

    public static Color ColorFromClass(CardTemplate.ClassColor classColor)
    {
        if (!CLASS_TO_COLOR.ContainsKey(classColor))
            return Color.white;
        return CLASS_TO_COLOR[classColor];
    }
}