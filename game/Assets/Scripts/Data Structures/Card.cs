using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

//removed abstract so that card is serialized and attributes are visible from inspector
[System.Serializable]
public class Card
{
    public static int CARD_CATEGORY_MINION = 0;
    public static int CARD_CATEGORY_SPELL = 1;
    public static int CARD_CATEGORY_STRUCTURE = 2;

    [SerializeField]
    protected string id;
    public string Id => id;

    [SerializeField]
    protected string name;
    public string Name => name;

    [SerializeField]
    protected int level;
    public int Level => level;

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

    public CardObject wrapper;
}

[System.Serializable]
public class CreatureCard : Card
{
    [SerializeField]
    protected int attack;
    public int Attack => attack;

    [SerializeField]
    protected int health;
    public int Health => health;

    private List<string> abilities;
    public List<string> Abilities => abilities;

    public CreatureCard(
        string id,
        string name,
        int level,
        int cost,
        string image,
        int attack,
        int health,
        List<string> abilities
    )
    {
        this.id = id;
        this.name = name;
        this.level = level;
        this.cost = cost;
        this.image = image;
        this.attack = attack;
        this.health = health;
        this.abilities = abilities;
    }
}

[System.Serializable]
public class WeaponCard : Card
{
    [SerializeField]
    protected int attack;
    public int Attack => attack;

    [SerializeField]
    protected int durability;
    public int Durability => durability;

    public WeaponCard(
        string id,
        string name,
        int level,
        int cost,
        string image,
        int attack,
        int durability
    )
    {
        this.id = id;
        this.name = name;
        this.level = level;
        this.cost = cost;
        this.image = image;

        this.attack = attack;
        this.durability = durability;
    }
}

[System.Serializable]
public class StructureCard : Card
{
    public StructureCard(
        string id,
        string name,
        int level,
        int cost,
        string image,
        int attack,
        int durability
    )
    {
        this.id = id;
        this.name = name;
        this.level = level;
        this.cost = cost;
        this.image = image;
    }
}

[System.Serializable]
public class SpellCard : Card
{
    public static readonly string NAME_LIGHTNING_BOLT = "Lightning Bolt";

    public static readonly List<string> TARGETED_SPELL_NAMES = new List<string>
    {
        NAME_LIGHTNING_BOLT,
    };
    private static Dictionary<string, string> spellToMethod;

    private bool targeted;      //affects single target or whole board?
    public bool Targeted => targeted;

    public SpellCard(
        string id,
        string name,
        int level,
        int cost,
        string image
    )
    {
        this.id = id;
        this.name = name;
        this.level = level;
        this.cost = cost;
        this.image = image;

        if (TARGETED_SPELL_NAMES.Contains(this.name))
        {
            this.targeted = true;
        }
        else
        {
            this.targeted = false;
        }

        if (spellToMethod == null)
        {
            InitSpellDict();
        }
    }

    private void InitSpellDict()
    {
        spellToMethod = new Dictionary<string, string>();
        spellToMethod["l_bolt"] = "LightningBolt";
    }

    public void Activate(BoardCreature creature, string shortName)
    {
        string methodName = spellToMethod[shortName];
        MethodInfo method = typeof(SpellCard).GetMethod(methodName);
        method.Invoke(this, new object[] { creature });
    }

    public void LightningBolt(BoardCreature target)
    {
        SoundManager.Instance.PlaySound("Shot", target.transform.position);
        target.TakeDamage(3);
        //play effect
        FXPoolManager.Instance.PlayEffect("LightningBolt", target.transform.position);
    }
}