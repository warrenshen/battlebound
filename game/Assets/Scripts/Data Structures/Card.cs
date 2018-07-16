using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

//removed abstract so that card is serialized and attributes are visible from inspector
[System.Serializable]
public abstract class Card
{
    public enum RarityType { Common, Uncommon, Rare, Epic, Legendary, Cosmic }

    public const string CARD_EMPTY_ABILITY = "EMPTY";
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
    public const string CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY = "CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY";
    public const string CARD_ABILITY_EACH_KILL_DRAW_CARD = "CARD_ABILITY_EACH_KILL_DRAW_CARD";
    public const string CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY = "CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY";
    public const string CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN = "CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN";
    public const string CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY = "CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY";
    public const string CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN = "CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN";
    public const string CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY = "CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY";
    public const string CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY = "CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY";

    public static readonly string[] VALID_ABILITIES = {
        CARD_EMPTY_ABILITY,
        CARD_ABILITY_CHARGE,
        CARD_ABILITY_TAUNT,
        CARD_ABILITY_SHIELD,
        CARD_ABILITY_BOOST_FRIENDLY_ATTACK_BY_TEN,
        CARD_ABILITY_BATTLE_CRY_DRAW_CARD,
        CARD_ABILITY_LIFE_STEAL,
        CARD_ABILITY_DEATH_RATTLE_DRAW_CARD,
        CARD_ABILITY_END_TURN_HEAL_TEN,
        CARD_ABILITY_END_TURN_HEAL_TWENTY,
        CARD_ABILITY_END_TURN_DRAW_CARD,
        CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY,
        CARD_ABILITY_EACH_KILL_DRAW_CARD,
        CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY,
        CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN,
        CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY,
        CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN,
        CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY,
        CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY,
    };

    public const string BUFF_CATEGORY_UNSTABLE_POWER = "BUFF_CATEGORY_UNSTABLE_POWER";

    public static readonly string[] VALID_BUFFS = {
        BUFF_CATEGORY_UNSTABLE_POWER,
    };

    public static int CARD_CATEGORY_MINION = 0;
    public static int CARD_CATEGORY_SPELL = 1;
    public static int CARD_CATEGORY_STRUCTURE = 2;
    public static int CARD_CATEGORY_WEAPON = 3;

    [SerializeField]
    protected string id;
    public string Id => id;

    [SerializeField]
    protected string name;
    public string Name => name;

    [SerializeField]
    protected int level;
    public int Level => level;

    protected string primaryEffectName;

    protected CardTemplate cardTemplate;

    public CardObject wrapper;
    //Rarity, Description, FrontImage, BackImage all moved into CardObject, obtain via codex loading cached to BattleManager

    public abstract PlayerState.ChallengeCard GetChallengeCard();

    public string GetName()
    {
        return this.cardTemplate.name;
    }

    public string GetDescription()
    {
        return this.cardTemplate.description;
    }

    public int GetCost()
    {
        return this.cardTemplate.cost;
    }

    public string GetFrontImage()
    {
        return this.cardTemplate.frontImage;
    }

    public string GetBackImage()
    {
        return this.cardTemplate.backImage;
    }

    public Vector2 GetFrontScale()
    {
        return this.cardTemplate.frontScale;
    }

    public Vector2 GetFrontOffset()
    {
        return this.cardTemplate.frontOffset;
    }

    public Vector2 GetBackScale()
    {
        return this.cardTemplate.backScale;
    }
    public Vector2 GetBackOffset()
    {
        return this.cardTemplate.backOffset;
    }

    protected void LoadCodex()
    {
        if (this.name == null)
        {
            this.cardTemplate = new CardTemplate();
        }
        else if (!BattleManager.Instance.CardNameToTemplate.ContainsKey(this.name))
        {
            Debug.LogError(string.Format("Card {0} does not exist in codex.", this.Name));
            this.cardTemplate = new CardTemplate();
        }
        else
        {
            this.cardTemplate = BattleManager.Instance.CardNameToTemplate[this.name];
        }
    }
}

[System.Serializable]
public class CreatureCard : Card
{
    public CreatureCard(
        string id,
        string name,
        int level
    )
    {
        this.id = id;
        this.name = name;
        this.level = level;

        LoadCodex();
    }

    public int GetAttack()
    {
        return this.cardTemplate.attack;
    }

    public int GetHealth()
    {
        return this.cardTemplate.health;
    }

    public List<string> GetAbilities()
    {
        return new List<string>(this.cardTemplate.abilities);
    }

    public string GetSummonPrefab()
    {
        return this.cardTemplate.summonPrefab;
    }

    public override PlayerState.ChallengeCard GetChallengeCard()
    {
        PlayerState.ChallengeCard challengeCard = new PlayerState.ChallengeCard();
        challengeCard.SetId(this.id);
        challengeCard.SetCategory(CARD_CATEGORY_MINION);
        challengeCard.SetName(this.name);
        challengeCard.SetLevel(this.level);
        challengeCard.SetCost(this.GetCost());
        challengeCard.SetCostStart(this.GetCost());
        challengeCard.SetHealth(this.GetHealth());
        challengeCard.SetHealthStart(this.GetHealth());
        challengeCard.SetHealthMax(this.GetHealth());
        challengeCard.SetAttack(this.GetAttack());
        challengeCard.SetAttackStart(this.GetAttack());

        return challengeCard;
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

        LoadCodex();
    }

    public override PlayerState.ChallengeCard GetChallengeCard()
    {
        PlayerState.ChallengeCard challengeCard = new PlayerState.ChallengeCard();
        challengeCard.SetId(this.id);
        challengeCard.SetCategory(CARD_CATEGORY_WEAPON);
        challengeCard.SetName(this.name);
        challengeCard.SetLevel(this.level);
        challengeCard.SetCost(this.GetCost());
        challengeCard.SetCostStart(this.GetCost());
        challengeCard.SetHealth(this.durability);
        challengeCard.SetHealthStart(this.durability);
        challengeCard.SetHealthMax(this.durability);
        challengeCard.SetAttack(this.attack);
        challengeCard.SetAttackStart(this.attack);

        return challengeCard;
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

        LoadCodex();
    }

    public override PlayerState.ChallengeCard GetChallengeCard()
    {
        PlayerState.ChallengeCard challengeCard = new PlayerState.ChallengeCard();
        challengeCard.SetId(this.id);
        challengeCard.SetCategory(CARD_CATEGORY_STRUCTURE);
        challengeCard.SetName(this.name);
        challengeCard.SetLevel(this.level);
        challengeCard.SetCost(this.GetCost());
        challengeCard.SetCostStart(this.GetCost());

        return challengeCard;
    }
}

[System.Serializable]
public class SpellCard : Card
{
    public const string SPELL_NAME_LIGHTNING_BOLT = "Touch of Zeus";
    public const string SPELL_NAME_UNSTABLE_POWER = "Unstable Power";
    public const string SPELL_NAME_FREEZE = "Freeze";
    public const string SPELL_NAME_DEEP_FREEZE = "Deep Freeze";

    public const string SPELL_NAME_RIOT_UP = "Riot Up";
    public const string SPELL_NAME_BLIZZARD = "Blizzard";


    public static readonly List<string> VALID_SPELLS = new List<string>
    {
        SPELL_NAME_LIGHTNING_BOLT,
        SPELL_NAME_UNSTABLE_POWER,
        SPELL_NAME_FREEZE,
        SPELL_NAME_DEEP_FREEZE,
        SPELL_NAME_RIOT_UP,
        SPELL_NAME_BLIZZARD,
    };

    public static readonly List<string> TARGETED_SPELL_NAMES = new List<string>
    {
        SPELL_NAME_LIGHTNING_BOLT,
        SPELL_NAME_UNSTABLE_POWER,
        SPELL_NAME_FREEZE,
        SPELL_NAME_DEEP_FREEZE,
    };

    public static readonly List<string> UNTARGETED_SPELL_NAMES = new List<string>
    {
        SPELL_NAME_RIOT_UP,
        SPELL_NAME_BLIZZARD,
    };

    private static Dictionary<string, string> spellToMethod;

    private bool targeted;      //affects single target or whole board?
    public bool Targeted => targeted;

    public SpellCard(
        string id,
        string name,
        int level
    )
    {
        if (!VALID_SPELLS.Contains(name))
        {
            Debug.LogError("Invalid spell name.");
        }

        this.id = id;
        this.name = name;
        this.level = level;

        if (TARGETED_SPELL_NAMES.Contains(this.name))
        {
            this.targeted = true;
        }
        else
        {
            this.targeted = false;
        }

        LoadCodex();
    }

    public override PlayerState.ChallengeCard GetChallengeCard()
    {
        PlayerState.ChallengeCard challengeCard = new PlayerState.ChallengeCard();
        challengeCard.SetId(this.id);
        challengeCard.SetCategory(CARD_CATEGORY_SPELL);
        challengeCard.SetName(this.name);
        challengeCard.SetLevel(this.level);
        challengeCard.SetCost(this.GetCost());
        challengeCard.SetCostStart(this.GetCost());

        return challengeCard;
    }
}
