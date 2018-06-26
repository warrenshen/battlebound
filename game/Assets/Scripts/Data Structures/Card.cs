using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public abstract class Card
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
		int cost,
		string image,
		int attack,
		int health,
		List<string> abilities,
		Player owner = null
	)
	{
        this.id = id;
        this.name = name;
        this.cost = cost;
        this.image = image;
        this.attack = attack;
        this.health = health;
        this.abilities = abilities;

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

public class SpellCard : Card
{
    private static Dictionary<string, string> spellToMethod;

    private bool targeted;      //affects single target or whole board?
    public bool Targeted => targeted;

    public SpellCard(string id, string name, int cost, string image, bool targeted = false, Player owner = null)
    {
        this.id = id;
        this.name = name;
        //this.category = category;
        this.cost = cost;
        this.image = image;
        this.targeted = targeted;
        this.owner = owner;

        if(spellToMethod == null) {
            InitSpellDict();
        }
    }

    private void InitSpellDict() {
        spellToMethod = new Dictionary<string, string>();
        spellToMethod["l_bolt"] = "LightningBolt";
    }

    public void Activate(BoardCreature creature, string shortName) {
        string methodName = spellToMethod[shortName];
        MethodInfo method = typeof(SpellCard).GetMethod(methodName);
        method.Invoke(this, new object[]{ creature });
    }

    public void LightningBolt(BoardCreature target) {
        SoundManager.Instance.PlaySound("Shot", target.transform.position);
        target.TakeDamage(3);
        //play effect
        FXPoolManager.Instance.PlayEffect("LightningBolt", target.transform.position);
    }
}