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
    public const string CARD_NAME_BUBBLE_SQUIRTER = "Bubble Squirter";
    public const string CARD_NAME_SWIFT_SHELLBACK = "Swift Shellback";
    public const string CARD_NAME_SENTIENT_SEAKING = "Sentient Seaking";
    public const string CARD_NAME_CRYSTAL_SNAPPER = "Crystal Snapper";
    public const string CARD_NAME_BATTLECLAD_GASDON = "Battleclad Gasdon";
    public const string CARD_NAME_REDHAIRED_PALADIN = "Redhaired Paladin";
    public const string CARD_NAME_FIRESWORN_GODBLADE = "Firesworn Godblade";
    public const string CARD_NAME_RITUAL_HATCHLING = "Ritual Hatchling";
    public const string CARD_NAME_HELLBRINGER = "Hellbringer";
    public const string CARD_NAME_HOOFED_LUSH = "Hoofed Lush";
    public const string CARD_NAME_DIONYSIAN_TOSSPOT = "Dionysian Tosspot";
    public const string CARD_NAME_SEAHORSE_SQUIRE = "Seahorse Squire";
    public const string CARD_NAME_TRIDENT_BATTLEMAGE = "Trident Battlemage";
    public const string CARD_NAME_SNEERBLADE = "Sneerblade";
    public const string CARD_NAME_TIMEWARP_KINGPIN = "Timewarp Kingpin";
    public const string CARD_NAME_LUX = "Lux";
    public const string CARD_NAME_THUNDEROUS_DESPERADO = "Thunderous Desperado";
    public const string CARD_NAME_CEREBOAROUS = "Cereboarus";
    public const string CARD_NAME_GUPPEA = "Guppea";
    public const string CARD_NAME_RHYNOKARP = "Rhynokarp";
    public const string CARD_NAME_PRICKLEPILLAR = "Pricklepillar";
    public const string CARD_NAME_ADDERSPINE_WEEVIL = "Adderspine Weevil";
    public const string CARD_NAME_THIEF_OF_NIGHT = "Thief of Night";
    public const string CARD_NAME_POWER_SIPHONER = "POWER SIPH#NER";
    public const string CARD_NAME_LIL_RUSTY = "Lil' Rusty";
    public const string CARD_NAME_INFERNO_902 = "INFERNO-902";
    public const string CARD_NAME_CHAR_BOT_451 = "CHAR-BOT-451";
    public const string CARD_NAME_MEGAPUNK = "MegaPUNK";

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
        CARD_NAME_BUBBLE_SQUIRTER,
        CARD_NAME_SWIFT_SHELLBACK,
        CARD_NAME_SENTIENT_SEAKING,
        CARD_NAME_CRYSTAL_SNAPPER,
        CARD_NAME_BATTLECLAD_GASDON,
        CARD_NAME_REDHAIRED_PALADIN,
        CARD_NAME_FIRESWORN_GODBLADE,
        CARD_NAME_RITUAL_HATCHLING,
        CARD_NAME_HELLBRINGER,
        CARD_NAME_HOOFED_LUSH,
        CARD_NAME_DIONYSIAN_TOSSPOT,
        CARD_NAME_SEAHORSE_SQUIRE,
        CARD_NAME_TRIDENT_BATTLEMAGE,
        CARD_NAME_SNEERBLADE,
        CARD_NAME_TIMEWARP_KINGPIN,
        CARD_NAME_LUX,
        CARD_NAME_THUNDEROUS_DESPERADO,
        CARD_NAME_CEREBOAROUS,
        CARD_NAME_GUPPEA,
        CARD_NAME_RHYNOKARP,
        CARD_NAME_PRICKLEPILLAR,
        CARD_NAME_ADDERSPINE_WEEVIL,
        CARD_NAME_THIEF_OF_NIGHT,
        CARD_NAME_POWER_SIPHONER,
        CARD_NAME_LIL_RUSTY,
        CARD_NAME_INFERNO_902,
        CARD_NAME_CHAR_BOT_451,
        CARD_NAME_MEGAPUNK,
    };

    public enum CardType { Creature, Spell, Weapon, Structure };
    public enum RarityType { Common, Uncommon, Rare, Epic, Legendary, Cosmic };

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
    public const string CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN = "CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN";
    public const string CARD_ABILITY_EACH_KILL_DRAW_CARD = "CARD_ABILITY_EACH_KILL_DRAW_CARD";
    public const string CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY = "CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY";
    public const string CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN = "CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN";
    public const string CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY = "CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY";
    public const string CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN = "CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN";
    public const string CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY = "CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY";
    public const string CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY = "CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY";
    public const string CARD_ABILITY_BATTLE_CRY_SILENCE_IN_FRONT = "CARD_ABILITY_BATTLE_CRY_SILENCE_IN_FRONT";
    public const string CARD_ABILITY_BATTLE_CRY_HEAL_FRIENDLY_MAX = "CARD_ABILITY_BATTLE_CRY_HEAL_FRIENDLY_MAX";
    public const string CARD_ABILITY_BATTLE_CRY_TAUNT_ADJACENT_FRIENDLY = "CARD_ABILITY_BATTLE_CRY_TAUNT_ADJACENT_FRIENDLY";
    public const string CARD_ABILITY_LETHAL = "CARD_ABILITY_LETHAL";
    public const string CARD_ABILITY_ATTACK_DAMAGE_ADJACENT_BY_TEN = "CARD_ABILITY_ATTACK_DAMAGE_ADJACENT_BY_TEN";
    public const string CARD_ABILITY_ATTACK_DAMAGE_ADJACENT_BY_ATTACK = "CARD_ABILITY_ATTACK_DAMAGE_ADJACENT_BY_ATTACK";
    public const string CARD_ABILITY_BATTLE_CRY_DAMAGE_PLAYER_FACE_BY_TWENTY = "CARD_ABILITY_BATTLE_CRY_DAMAGE_PLAYER_FACE_BY_TWENTY";
    public const string CARD_ABILITY_END_TURN_BOTH_PLAYERS_DRAW_CARD = "CARD_ABILITY_END_TURN_BOTH_PLAYERS_DRAW_CARD";
    public const string CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_TWENTY = "CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_TWENTY";
    public const string CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_FOURTY = "CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_FOURTY";
    public const string CARD_ABILITY_BATTLE_CRY_HEAL_ALL_CREATURES_BY_FOURTY = "CARD_ABILITY_BATTLE_CRY_HEAL_ALL_CREATURES_BY_FOURTY";
    public const string CARD_ABILITY_BATTLE_CRY_REVIVE_HIGHEST_COST_CREATURE = "CARD_ABILITY_BATTLE_CRY_REVIVE_HIGHEST_COST_CREATURE";
    public const string CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_OPPONENT_CREATURES_BY_TWENTY = "CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_OPPONENT_CREATURES_BY_TWENTY";
    public const string CARD_ABILITY_BATTLE_CRY_SILENCE_ALL_OPPONENT_CREATURES = "CARD_ABILITY_BATTLE_CRY_SILENCE_ALL_OPPONENT_CREATURES";
    public const string CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_CREATURES_BY_THIRTY = "CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_CREATURES_BY_THIRTY";
    public const string CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY = "CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY";

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
        CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN,
        CARD_ABILITY_EACH_KILL_DRAW_CARD,
        CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY,
        CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN,
        CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY,
        CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN,
        CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY,
        CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY,
        CARD_ABILITY_BATTLE_CRY_SILENCE_IN_FRONT,
        CARD_ABILITY_BATTLE_CRY_HEAL_FRIENDLY_MAX,
        CARD_ABILITY_BATTLE_CRY_TAUNT_ADJACENT_FRIENDLY,
        CARD_ABILITY_LETHAL,
        CARD_ABILITY_ATTACK_DAMAGE_ADJACENT_BY_TEN,
        CARD_ABILITY_ATTACK_DAMAGE_ADJACENT_BY_ATTACK,
        CARD_ABILITY_BATTLE_CRY_DAMAGE_PLAYER_FACE_BY_TWENTY,
        CARD_ABILITY_END_TURN_BOTH_PLAYERS_DRAW_CARD,
        CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_TWENTY,
        CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_FOURTY,
        CARD_ABILITY_BATTLE_CRY_HEAL_ALL_CREATURES_BY_FOURTY,
        CARD_ABILITY_BATTLE_CRY_REVIVE_HIGHEST_COST_CREATURE,
        CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_OPPONENT_CREATURES_BY_TWENTY,
        CARD_ABILITY_BATTLE_CRY_SILENCE_ALL_OPPONENT_CREATURES,
        CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_CREATURES_BY_THIRTY,
        CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY,
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
        { 10, CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN },
        { 11, CARD_ABILITY_EACH_KILL_DRAW_CARD },
        { 12, CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY },
        { 13, CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN },
        { 14, CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY },
        { 15, CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN },
        { 16, CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY },
        { 17, CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY },
        { 18, CARD_ABILITY_BATTLE_CRY_SILENCE_IN_FRONT },
        { 19, CARD_ABILITY_BATTLE_CRY_HEAL_FRIENDLY_MAX },
        { 20, CARD_ABILITY_BATTLE_CRY_TAUNT_ADJACENT_FRIENDLY },
        { 21, CARD_ABILITY_LETHAL },
        { 22, CARD_ABILITY_ATTACK_DAMAGE_ADJACENT_BY_TEN },
        { 23, CARD_ABILITY_ATTACK_DAMAGE_ADJACENT_BY_ATTACK },
        { 24, CARD_ABILITY_BATTLE_CRY_DAMAGE_PLAYER_FACE_BY_TWENTY },
        { 25, CARD_ABILITY_END_TURN_BOTH_PLAYERS_DRAW_CARD },
        { 26, CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_TWENTY },
        { 27, CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_FOURTY },
        { 28, CARD_ABILITY_BATTLE_CRY_HEAL_ALL_CREATURES_BY_FOURTY },
        { 29, CARD_ABILITY_BATTLE_CRY_REVIVE_HIGHEST_COST_CREATURE },
        { 30, CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_OPPONENT_CREATURES_BY_TWENTY },
        { 31, CARD_ABILITY_BATTLE_CRY_SILENCE_ALL_OPPONENT_CREATURES },
        { 32, CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_CREATURES_BY_THIRTY },
        { 33, CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY },
    };

    public const string BUFF_CATEGORY_UNSTABLE_POWER = "BUFF_CATEGORY_UNSTABLE_POWER";

    public static readonly string[] VALID_BUFFS = {
        BUFF_CATEGORY_UNSTABLE_POWER,
    };

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

    public abstract ChallengeCard GetChallengeCard();

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

    public RarityType GetRarity()
    {
        return this.cardTemplate.rarity;
    }

    public CardTemplate.ClassColor GetClassColor()
    {
        return this.cardTemplate.classColor;
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
    //protected int level;
    //public int Level => level;

    private int attack;
    private int health;
    private List<string> abilities;

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

        this.attack = attack;
        this.health = health;
        this.abilities = abilities;

        LoadCodex();
    }

    public int GetAttack()
    {
        if (this.attack > 0)
        {
            return this.attack;
        }
        else
        {
            return this.cardTemplate.attack;
        }
    }

    public int GetHealth()
    {
        if (this.health > 0)
        {
            return this.health;
        }
        else
        {
            return this.cardTemplate.health;
        }
    }

    public List<string> GetAbilities()
    {
        return new List<string>(this.cardTemplate.abilities);
    }

    public string GetSummonPrefab()
    {
        return this.cardTemplate.summonPrefab;
    }

    public string GetEffectPrefab()
    {
        return this.cardTemplate.effectPrefab;
    }

    public override ChallengeCard GetChallengeCard()
    {
        ChallengeCard challengeCard = new ChallengeCard();
        challengeCard.SetId(this.id);
        challengeCard.SetCategory((int)Card.CardType.Creature);
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

    public static new CreatureCard GetFromJson(string json)
    {
        CreatureCard creatureCard = JsonUtility.FromJson<CreatureCard>(json);
        creatureCard.LoadCodex();
        return creatureCard;
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

    public override ChallengeCard GetChallengeCard()
    {
        ChallengeCard challengeCard = new ChallengeCard();
        challengeCard.SetId(this.id);
        challengeCard.SetCategory((int)Card.CardType.Weapon);
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

    public static new WeaponCard GetFromJson(string json)
    {
        WeaponCard weaponCard = JsonUtility.FromJson<WeaponCard>(json);
        weaponCard.LoadCodex();
        return weaponCard;
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

    public override ChallengeCard GetChallengeCard()
    {
        ChallengeCard challengeCard = new ChallengeCard();
        challengeCard.SetId(this.id);
        challengeCard.SetCategory((int)Card.CardType.Structure);
        challengeCard.SetName(this.name);
        challengeCard.SetLevel(this.level);
        challengeCard.SetCost(this.GetCost());
        challengeCard.SetCostStart(this.GetCost());

        return challengeCard;
    }

    public static new StructureCard GetFromJson(string json)
    {
        StructureCard structureCard = JsonUtility.FromJson<StructureCard>(json);
        structureCard.LoadCodex();
        return structureCard;
    }
}

[System.Serializable]
public class SpellCard : Card
{
    public const string SPELL_NAME_TOUCH_OF_ZEUS = "Touch of Zeus";
    public const string SPELL_NAME_UNSTABLE_POWER = "Unstable Power";
    public const string SPELL_NAME_DEEP_FREEZE = "Deep Freeze";
    public const string SPELL_NAME_WIDESPREAD_FROSTBITE = "Widespread Frostbite";
    public const string SPELL_NAME_DEATH_NOTICE = "Death Notice";

    public const string SPELL_NAME_RIOT_UP = "Riot Up";
    public const string SPELL_NAME_BRR_BRR_BLIZZARD = "Brr Brr Blizzard";
    public const string SPELL_NAME_RAZE_TO_ASHES = "Raze to Ashes";
    public const string SPELL_NAME_GREEDY_FINGERS = "Greedy Fingers";
    public const string SPELL_NAME_SILENCE_OF_THE_LAMBS = "Silence of the Lambs";
    public const string SPELL_NAME_MUDSLINGING = "Mudslinging";
    public const string SPELL_NAME_SPRAY_N_PRAY = "Spray n' Pray";
    public const string SPELL_NAME_GRAVE_DIGGING = "Grave-digging";

    public static readonly List<string> VALID_SPELLS = new List<string>
    {
        SPELL_NAME_TOUCH_OF_ZEUS,
        SPELL_NAME_UNSTABLE_POWER,
        SPELL_NAME_DEEP_FREEZE,
        SPELL_NAME_WIDESPREAD_FROSTBITE,
        SPELL_NAME_DEATH_NOTICE,

        SPELL_NAME_RIOT_UP,
        SPELL_NAME_BRR_BRR_BLIZZARD,
        SPELL_NAME_RAZE_TO_ASHES,
        SPELL_NAME_GREEDY_FINGERS,
        SPELL_NAME_SILENCE_OF_THE_LAMBS,
        SPELL_NAME_MUDSLINGING,
        SPELL_NAME_SPRAY_N_PRAY,
        SPELL_NAME_GRAVE_DIGGING,
    };

    public static readonly List<string> TARGETED_SPELL_NAMES = new List<string>
    {
        SPELL_NAME_TOUCH_OF_ZEUS,
        SPELL_NAME_UNSTABLE_POWER,
        SPELL_NAME_DEEP_FREEZE,
        SPELL_NAME_WIDESPREAD_FROSTBITE,
        SPELL_NAME_DEATH_NOTICE,
    };

    public static readonly List<string> UNTARGETED_SPELL_NAMES = new List<string>
    {
        SPELL_NAME_RIOT_UP,
        SPELL_NAME_BRR_BRR_BLIZZARD,
        SPELL_NAME_RAZE_TO_ASHES,
        SPELL_NAME_GREEDY_FINGERS,
        SPELL_NAME_SILENCE_OF_THE_LAMBS,
        SPELL_NAME_MUDSLINGING,
        SPELL_NAME_SPRAY_N_PRAY,
        SPELL_NAME_GRAVE_DIGGING,
    };

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

    public override ChallengeCard GetChallengeCard()
    {
        ChallengeCard challengeCard = new ChallengeCard();
        challengeCard.SetId(this.id);
        challengeCard.SetCategory((int)Card.CardType.Spell);
        challengeCard.SetName(this.name);
        challengeCard.SetLevel(this.level);
        challengeCard.SetCost(this.GetCost());
        challengeCard.SetCostStart(this.GetCost());

        return challengeCard;
    }

    public static new SpellCard GetFromJson(string json)
    {
        SpellCard spellCard = JsonUtility.FromJson<SpellCard>(json);
        spellCard.LoadCodex();
        return spellCard;
    }
}
