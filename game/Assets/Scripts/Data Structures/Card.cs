using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

//removed abstract so that card is serialized and attributes are visible from inspector
[System.Serializable]
public class Card
{
    public enum RarityType { Common, Rare, Epic, Legendary }

    public const string CARD_ABILITY_CHARGE = "CARD_ABILITY_CHARGE";
    public const string CARD_ABILITY_TAUNT = "CARD_ABILITY_TAUNT";
    public const string CARD_ABILITY_SHIELD = "CARD_ABILITY_SHIELD";
    // TODO
    public const string CARD_ABILITY_BOOST_FRIENDLY_ATTACK_BY_TEN = "CARD_ABILITY_BOOST_FRIENDLY_ATTACK_BY_ONE";
    public const string CARD_ABILITY_BATTLE_CRY_DRAW_CARD = "CARD_ABILITY_BATTLE_CRY_DRAW_CARD";
    public const string CARD_ABILITY_LIFE_STEAL = "CARD_ABILITY_LIFE_STEAL";
    public const string CARD_ABILITY_DEATH_RATTLE_DRAW_CARD = "CARD_ABILITY_DEATH_RATTLE_DRAW_CARD";
    public const string CARD_ABILITY_END_TURN_HEAL_TEN = "CARD_ABILITY_END_TURN_HEAL_TEN";
    public const string CARD_ABILITY_END_TURN_HEAL_TWENTY = "CARD_ABILITY_END_TURN_HEAL_TWENTY";
    public const string CARD_ABILITY_END_TURN_DRAW_CARD = "CARD_ABILITY_END_TURN_DRAW_CARD";
    public const string CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_TWENTY = "CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_TWENTY";
    // TODO
    public const string CARD_ABILITY_EACH_KILL_DRAW_CARD = "CARD_ABILITY_EACH_KILL_DRAW_CARD";

    public const string BUFF_CATEGORY_UNSTABLE_POWER = "BUFF_CATEGORY_UNSTABLE_POWER";

    public static readonly string[] VALID_BUFFS = {
        BUFF_CATEGORY_UNSTABLE_POWER,
    };

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

    protected string primaryEffectName;

    public CardObject wrapper;
    //Rarity, Description, FrontImage, BackImage all moved into CardObject, obtain via codex loading cached to BattleManager
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

    //protected string summonPrefabPath;  obtain from codex loading

    public CreatureCard(
        string id,
        string name,
        int level,
        int cost,
        int attack,
        int health,
        List<string> abilities
    )
    {
        this.id = id;
        this.name = name;
        this.level = level;
        this.cost = cost;
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
        int attack,
        int durability
    )
    {
        this.id = id;
        this.name = name;
        this.level = level;
        this.cost = cost;

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
        int attack,
        int durability
    )
    {
        this.id = id;
        this.name = name;
        this.level = level;
        this.cost = cost;
    }
}

[System.Serializable]
public class SpellCard : Card
{
    public const string SPELL_NAME_LIGHTNING_BOLT = "Lightning Bolt";
    public const string SPELL_NAME_UNSTABLE_POWER = "Unstable Power";

    public static readonly List<string> VALID_SPELLS = new List<string>
    {
        SPELL_NAME_LIGHTNING_BOLT,
        SPELL_NAME_UNSTABLE_POWER,
    };

    public static readonly List<string> TARGETED_SPELL_NAMES = new List<string>
    {
        SPELL_NAME_LIGHTNING_BOLT,
        SPELL_NAME_UNSTABLE_POWER,
    };
    private static Dictionary<string, string> spellToMethod;

    private bool targeted;      //affects single target or whole board?
    public bool Targeted => targeted;

    public SpellCard(
        string id,
        string name,
        int level,
        int cost
    )
    {
        if (!VALID_SPELLS.Contains(name))
        {
            Debug.LogError("Invalid spell name.");
        }

        this.id = id;
        this.name = name;
        this.level = level;
        this.cost = cost;

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
        spellToMethod[SPELL_NAME_LIGHTNING_BOLT] = "LightningBolt";
        spellToMethod[SPELL_NAME_UNSTABLE_POWER] = "UnstablePower";
    }

    public void Activate(BoardCreature creature)
    {
        string methodName = spellToMethod[this.name];
        MethodInfo method = typeof(SpellCard).GetMethod(methodName);
        method.Invoke(this, new object[] { creature });
    }

    public void LightningBolt(BoardCreature target)
    {
        SoundManager.Instance.PlaySound("ShockSFX", target.transform.position);
        target.TakeDamage(3);
        //play effect
        FXPoolManager.Instance.PlayEffect("LightningBoltVFX", target.transform.position);
    }

    public void UnstablePower(BoardCreature target)
    {
        target.AddAttack(30);
        target.AddBuff(Card.BUFF_CATEGORY_UNSTABLE_POWER);
    }
}
