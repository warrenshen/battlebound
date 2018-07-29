// ====================================================================================================
//
// Cloud Code for ChallengeCardModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
const CARD_COLOR_NEUTRAL = 0;
const CARD_COLOR_FIRE = 1;
const CARD_COLOR_EARTH = 2;
const CARD_COLOR_WATER = 3;
const CARD_COLOR_DARK = 4;
const CARD_COLOR_LIGHT = 5;

const CARD_NAME_FIREBUG_CATELYN = "Firebug Catelyn";
const CARD_NAME_MARSHWATER_SQUEALER = "Marshwater Squealer";
const CARD_NAME_WATERBORNE_RAZORBACK = "Waterborne Razorback";
const CARD_NAME_BLESSED_NEWBORN = "Blessed Newborn";
const CARD_NAME_YOUNG_KYO = "Young Kyo";
const CARD_NAME_WAVE_CHARMER = "Wave Charmer";
const CARD_NAME_POSEIDONS_HANDMAIDEN = "Poseidon's Handmaiden";
const CARD_NAME_EMBERKITTY = "Emberkitty";
const CARD_NAME_FIRESTRIDED_TIGRESS = "Firestrided Tigress";
const CARD_NAME_TEMPLE_GUARDIAN = "Temple Guardian";
const CARD_NAME_BOMBSHELL_BOMBADIER = "Bombshell Bombadier";
const CARD_NAME_TAJI_THE_FEARLESS = "Taji the Fearless";
const CARD_NAME_UNKINDLED_JUNIOR = "Unkindled Junior";
const CARD_NAME_FLAMEBELCHER = "Flamebelcher";
const CARD_NAME_FIREBORN_MENACE = "Fireborn Menace";
const CARD_NAME_TEA_GREENLEAF = "Te'a Greenleaf";
const CARD_NAME_NESSA_NATURES_CHAMPION = "Nessa, Nature's Champion";
const CARD_NAME_BUBBLE_SQUIRTER = "Bubble Squirter";
const CARD_NAME_SWIFT_SHELLBACK = "Swift Shellback";
const CARD_NAME_SENTIENT_SEAKING = "Sentient Seaking";
const CARD_NAME_CRYSTAL_SNAPPER = "Crystal Snapper";
const CARD_NAME_BATTLECLAD_GASDON = "Battleclad Gasdon";
const CARD_NAME_REDHAIRED_PALADIN = "Redhaired Paladin";
const CARD_NAME_FIRESWORN_GODBLADE = "Firesworn Godblade";
const CARD_NAME_RITUAL_HATCHLING = "Ritual Hatchling";
const CARD_NAME_HELLBRINGER = "Hellbringer";
const CARD_NAME_HOOFED_LUSH = "Hoofed Lush";
const CARD_NAME_DIONYSIAN_TOSSPOT = "Dionysian Tosspot";
const CARD_NAME_SEAHORSE_SQUIRE = "Seahorse Squire";
const CARD_NAME_TRIDENT_BATTLEMAGE = "Trident Battlemage";
const CARD_NAME_SNEERBLADE = "Sneerblade";
const CARD_NAME_TIMEWARP_KINGPIN = "Timewarp Kingpin";
const CARD_NAME_LUX = "Lux";
const CARD_NAME_THUNDEROUS_DESPERADO = "Thunderous Desperado";
const CARD_NAME_CEREBOAROUS = "Cereboarus";
const CARD_NAME_GUPPEA = "Guppea";
const CARD_NAME_RHYNOKARP = "Rhynokarp";
const CARD_NAME_PRICKLEPILLAR = "Pricklepillar";
const CARD_NAME_ADDERSPINE_WEEVIL = "Adderspine Weevil";
const CARD_NAME_THIEF_OF_NIGHT = "Thief of Night";
const CARD_NAME_POWER_SIPHONER = "POWER SIPH#NER";
const CARD_NAME_LIL_RUSTY = "Lil' Rusty";
const CARD_NAME_INFERNO_902 = "INFERNO-902";
const CARD_NAME_CHAR_BOT_451 = "CHAR-BOT-451";
const CARD_NAME_MEGAPUNK = "MegaPUNK";

const CARD_ABILITY_TO_DESCRIPTION = {};
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_CHARGE] = "Charge";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_TAUNT] = "Taunt";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_SHIELD] = "Shield";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BOOST_FRIENDLY_ATTACK_BY_TEN] = "Boost";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_DRAW_CARD] = "Warcry: Draw a card";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_LIFE_STEAL] = "Lifesteal";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_DEATH_RATTLE_DRAW_CARD] = "Deathwish: Draw a card";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_END_TURN_HEAL_TEN] = "End turn: Heal 10 hp";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_END_TURN_HEAL_TWENTY] = "End turn: Heal 20 hp";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_END_TURN_DRAW_CARD] = "End turn: Draw a card";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN] = "Deathwish: Deal 10 dmg to enemy";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_EACH_KILL_DRAW_CARD] = "Each kill: Draw a card";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY] = "Deathwish: Deal 20 dmg to three random targetables";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN] = "End turn: Deal 10 dmg to creature in front";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY] = "End turn: Deal 20 dmg to creature in front";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN] = "Warcry: Deal 10 dmg to creature in front";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY] = "Warcry: Deal 20 dmg to creature in front";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY] = "On take damage: Deal 30 dmg to yourself";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_SILENCE_IN_FRONT] = "Warcry: Silence creature in front";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_HEAL_FRIENDLY_MAX] = "Warcry: Heal friendly creatures to full health";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_TAUNT_ADJACENT_FRIENDLY] = "Warcry: Grant Taunt to adjacent creatures";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_LETHAL] = "Lethal";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_ATTACK_DAMAGE_ADJACENT_BY_TEN] = "On attack: Deal 10 dmg to adjacent creatures";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_ATTACK_DAMAGE_ADJACENT_BY_ATTACK] = "On attack: Deal 20 dmg to adjacent creatures";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_DAMAGE_PLAYER_FACE_BY_TWENTY] = "Warcry: Deal 20 dmg to yourself";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_END_TURN_BOTH_PLAYERS_DRAW_CARD] = "End turn [BOTH]: Draw a card";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_TWENTY] = "Warcry: Heal 20 hp to adjacent creatures";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_FOURTY] = "Warcry: Heal 40 hp to adjacent creatures";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_HEAL_ALL_CREATURES_BY_FOURTY] = "Warcry: Heal 40 hp to all creatures";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_REVIVE_HIGHEST_COST_CREATURE] = "Warcry: Revive your most recent highest cost dead creature";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_OPPONENT_CREATURES_BY_TWENTY] = "Deathwish: Deal 20 dmg to all enemy creatures";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_SILENCE_ALL_OPPONENT_CREATURES] = "Warcry: Silence all enemy creatures";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_CREATURES_BY_THIRTY] = "Deathwish: Deal 30 dmg to all creatures";

const CARD_NAME_TO_LEVEL_TO_STATS = {};

CARD_NAME_TO_LEVEL_TO_ABILITIES[CARD_NAME_FIREBUG_CATELYN] = {
    1: {
        cost: 20,
        attack: 20,
        health: 10,
        abilities: [CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN],
    },
    2: {
        cost: 20,
        attack: 24,
        health: 10,
        abilities: [CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN],
    },
    3: {
        cost: 20,
        attack: 24,
        health: 14,
        abilities: [CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN],
    },
    4: {
        cost: 20,
        attack: 28,
        health: 14,
        abilities: [CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY],
    },
    5: {
        cost: 20,
        attack: 28,
        health: 18,
        abilities: [CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY],
    },
    6: {
        cost: 20,
        attack: 30,
        health: 30,
        abilities: [CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY],
    },
};

CARD_NAME_TO_LEVEL_TO_ABILITIES[CARD_NAME_MARSHWATER_SQUEALER] = {
    1: {
        cost: 20,
        attack: 20,
        health: 30,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
    2: {
        cost: 20,
        attack: 20,
        health: 34,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
    3: {
        cost: 20,
        attack: 24,
        health: 34,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
    4: {
        cost: 20,
        attack: 24,
        health: 38,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
    5: {
        cost: 20,
        attack: 28,
        health: 38,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
    6: {
        cost: 20,
        attack: 30,
        health: 40,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
};

CARD_NAME_TO_LEVEL_TO_ABILITIES[CARD_NAME_MARSHWATER_SQUEALER] = {
    1: {
        cost: 20,
        attack: 20,
        health: 30,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
    2: {
        cost: 20,
        attack: 20,
        health: 34,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
    3: {
        cost: 20,
        attack: 24,
        health: 34,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
    4: {
        cost: 20,
        attack: 24,
        health: 38,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
    5: {
        cost: 20,
        attack: 28,
        health: 38,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
    6: {
        cost: 20,
        attack: 30,
        health: 40,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
};

CARD_NAME_TO_LEVEL_TO_ABILITIES[CARD_NAME_WATERBORNE_RAZORBACK] = {
    1: {
        cost: 40,
        attack: 30,
        health: 60,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TWENTY],
    },
    2: {
        cost: 40,
        attack: 30,
        health: 60,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TWENTY],
    },
    3: {
        cost: 40,
        attack: 30,
        health: 60,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TWENTY],
    },
    4: {
        cost: 40,
        attack: 30,
        health: 60,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TWENTY],
    },
    5: {
        cost: 40,
        attack: 30,
        health: 60,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TWENTY],
    },
    6: {
        cost: 40,
        attack: 30,
        health: 60,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TWENTY],
    },
};

CARD_NAME_TO_LEVEL_TO_ABILITIES[CARD_NAME_BLESSED_NEWBORN] = {
    1: {
        cost: 20,
        attack: 10,
        health: 10,
        abilities: [CARD_ABILITY_BATTLE_CRY_DRAW_CARD],
    },
    2: {
        cost: 20,
        attack: 12,
        health: 12,
        abilities: [CARD_ABILITY_BATTLE_CRY_DRAW_CARD],
    },
    3: {
        cost: 20,
        attack: 14,
        health: 14,
        abilities: [CARD_ABILITY_BATTLE_CRY_DRAW_CARD],
    },
    4: {
        cost: 20,
        attack: 16,
        health: 16,
        abilities: [CARD_ABILITY_BATTLE_CRY_DRAW_CARD],
    },
    5: {
        cost: 20,
        attack: 18,
        health: 18,
        abilities: [CARD_ABILITY_BATTLE_CRY_DRAW_CARD],
    },
    6: {
        cost: 20,
        attack: 20,
        health: 20,
        abilities: [CARD_ABILITY_BATTLE_CRY_DRAW_CARD],
    },
};
