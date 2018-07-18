using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//removed abstract so that card is serialized and attributes are visible from inspector
[System.Serializable]
public abstract class Card
{
    public const string CARD_NAME_FIREBUG_CATELYN = "Firebug Catelyn";
    public const string CARD_NAME_MARSHWATER_SQUEALER = "Marshwater Squealer";
    public const string CARD_NAME_WATERBORNE_RAZORBACK = "Waterborne Razorback";
    public const string CARD_NAME_BLESSED_NEWBORN = "Blessed Newborn";
    public const string CARD_NAME_YOUNG_KYO = "Young Kyo";
    public const string CARD_NAME_WAVE_CHARMER = "Wave Charmer";
    public const string CARD_NAME_POSEIDONS_HANDMAIDEN = "Poseidon's Handmaiden";
    public const string CARD_NAME_EMBERKITTY = "Emberkitty";
    public const string CARD_NAME_FIRESTRIDED_TIGRESS = "Firestrided Tigress";
    public const string CARD_NAME_TEMPLE_GUARDIAN = "Temple Guardian";
    public const string CARD_NAME_BOMBSHELL_BOMBADIER = "Bombshell Bombadier";
    public const string CARD_NAME_TAJI_THE_FEARLESS = "Taji the Fearless";
    public const string CARD_NAME_UNKINDLED_JUNIOR = "Unkindled Junior";
    public const string CARD_NAME_FLAMEBELCHER = "Flamebelcher";
    public const string CARD_NAME_FIREBORN_MENACE = "Fireborn Menace";
    public const string CARD_NAME_TEA_GREENLEAF = "Te'a Greenleaf";
    public const string CARD_NAME_NESSA_NATURES_CHAMPION = "Nessa, Nature's Champion";

    public static readonly List<string> CREATURE_CARD_NAMES = new List<string>
    {
        CARD_NAME_FIREBUG_CATELYN,
        CARD_NAME_MARSHWATER_SQUEALER,
        CARD_NAME_WATERBORNE_RAZORBACK,
        CARD_NAME_BLESSED_NEWBORN,
        CARD_NAME_YOUNG_KYO,
        CARD_NAME_WAVE_CHARMER,
        CARD_NAME_POSEIDONS_HANDMAIDEN,
        CARD_NAME_EMBERKITTY,
        CARD_NAME_FIRESTRIDED_TIGRESS,
        CARD_NAME_TEMPLE_GUARDIAN,
        CARD_NAME_BOMBSHELL_BOMBADIER,
        CARD_NAME_TAJI_THE_FEARLESS,
        CARD_NAME_UNKINDLED_JUNIOR,
        CARD_NAME_FLAMEBELCHER,
        CARD_NAME_FIREBORN_MENACE,
        CARD_NAME_TEA_GREENLEAF,
        CARD_NAME_NESSA_NATURES_CHAMPION,
    };

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

    public static readonly Dictionary<int, string> ABILITY_CODE_TO_STRING = new Dictionary<int, string>
    {
        { 0, CARD_ABILITY_CHARGE },
        { 1, CARD_ABILITY_TAUNT },
        { 2, CARD_ABILITY_SHIELD },
        { 3, CARD_ABILITY_BOOST_FRIENDLY_ATTACK_BY_TEN },
        { 4, CARD_ABILITY_BATTLE_CRY_DRAW_CARD },
        { 5, CARD_ABILITY_LIFE_STEAL },
        { 6, CARD_ABILITY_DEATH_RATTLE_DRAW_CARD },
        { 7, CARD_ABILITY_END_TURN_HEAL_TEN },
        { 8, CARD_ABILITY_END_TURN_HEAL_TWENTY },
        { 9, CARD_ABILITY_END_TURN_DRAW_CARD },
        { 10, CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY },
        { 11, CARD_ABILITY_EACH_KILL_DRAW_CARD },
        { 12, CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY },
        { 13, CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN },
        { 14, CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY },
        { 15, CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN },
        { 16, CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY },
        { 17, CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY },
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

    public Texture2D GetFrontImageTexture()
    {
        return ResourceSingleton.Instance.GetImageTextureByName(GetFrontImage());
    }

    public Texture2D GetBackImageTexture()
    {
        return ResourceSingleton.Instance.GetImageTextureByName(GetBackImage());
    }

    protected void LoadCodex()
    {
        if (this.name == null)
        {
            this.cardTemplate = new CardTemplate();
            return;
        }

        CardTemplate template = ResourceSingleton.Instance.GetCardTemplateByName(this.name);
        if (template == null)
        {
            this.cardTemplate = new CardTemplate();
        }
        else
        {
            this.cardTemplate = template;
        }
    }

    public static List<string> GetAbilityStringsByCodes(List<string> abilityCodes)
    {
        List<string> abilityStrings = new List<string>();
        foreach (string abilityCode in abilityCodes)
        {
            if (VALID_ABILITIES.Contains(abilityCode))
            {
                abilityStrings.Add(abilityCode);
            }
            else
            {
                int abilityInt = Int32.Parse(abilityCode);
                abilityStrings.Add(ABILITY_CODE_TO_STRING[abilityInt]);
            }
        }
        return abilityStrings;
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
        challengeCard.SetAbilities(this.GetAbilities());

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
    public const string SPELL_NAME_BRR_BRR_BLIZZARD = "Brr Brr Blizzard";
    public const string SPELL_NAME_RAZE_TO_ASHES = "Raze to Ashes";


    public static readonly List<string> VALID_SPELLS = new List<string>
    {
        SPELL_NAME_LIGHTNING_BOLT,
        SPELL_NAME_UNSTABLE_POWER,
        SPELL_NAME_FREEZE,
        SPELL_NAME_DEEP_FREEZE,
        SPELL_NAME_RIOT_UP,
        SPELL_NAME_BRR_BRR_BLIZZARD,
        SPELL_NAME_RAZE_TO_ASHES,
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
        SPELL_NAME_BRR_BRR_BLIZZARD,
        SPELL_NAME_RAZE_TO_ASHES,
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
            Debug.LogError(string.Format("Invalid spell name: {0}.", name));
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
