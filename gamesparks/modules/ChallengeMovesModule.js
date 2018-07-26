// ====================================================================================================
//
// Cloud Code for ChallengeMovesModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
const TIME_LIMIT_CATEGORY_NORMAL = "TIME_LIMIT_CATEGORY_NORMAL";
const TIME_LIMIT_CATEGORY_MULLIGAN = "TIME_LIMIT_CATEGORY_MULLIGAN";

const PLAYER_STATE_MODE_NORMAL = 0;
const PLAYER_STATE_MODE_MULLIGAN = 1;
const PLAYER_STATE_MODE_MULLIGAN_WAITING = 2;

const CARD_CATEGORY_MINION = 0;
const CARD_CATEGORY_SPELL = 1;
const CARD_CATEGORY_STRUCTURE = 2;

const MOVE_CATEGORY_PLAY_MULLIGAN = "MOVE_CATEGORY_PLAY_MULLIGAN";
const MOVE_CATEGORY_FINISH_MULLIGAN = "MOVE_CATEGORY_FINISH_MULLIGAN";
const MOVE_CATEGORY_SURRENDER_BY_CHOICE = "MOVE_CATEGORY_SURRENDER_BY_CHOICE";
const MOVE_CATEGORY_SURRENDER_BY_EXPIRE = "MOVE_CATEGORY_SURRENDER_BY_EXPIRE";
const MOVE_CATEGORY_END_TURN = "MOVE_CATEGORY_END_TURN";
const MOVE_CATEGORY_DRAW_CARD = "MOVE_CATEGORY_DRAW_CARD";
const MOVE_CATEGORY_DRAW_CARD_MULLIGAN = "MOVE_CATEGORY_DRAW_CARD_MULLIGAN";
const MOVE_CATEGORY_DRAW_CARD_HAND_FULL = "MOVE_CATEGORY_DRAW_CARD_HAND_FULL";
const MOVE_CATEGORY_DRAW_CARD_DECK_EMPTY = "MOVE_CATEGORY_DRAW_CARD_DECK_EMPTY";
const MOVE_CATEGORY_PLAY_MINION = "MOVE_CATEGORY_PLAY_MINION";
const MOVE_CATEGORY_PLAY_SPELL_TARGETED = "MOVE_CATEGORY_PLAY_SPELL_TARGETED";
const MOVE_CATEGORY_PLAY_SPELL_UNTARGETED = "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED";
const MOVE_CATEGORY_CARD_ATTACK = "MOVE_CATEGORY_CARD_ATTACK";
const MOVE_CATEGORY_RANDOM_TARGET = "MOVE_CATEGORY_RANDOM_TARGET";
const MOVE_CATEGORY_SUMMON_CREATURE = "MOVE_CATEGORY_SUMMON_CREATURE";
const MOVE_CATEGORY_SUMMON_CREATURE_FIELD_FULL = "MOVE_CATEGORY_SUMMON_CREATURE_FIELD_FULL";
const MOVE_CATEGORY_SUMMON_CREATURE_NO_CREATURE = "MOVE_CATEGORY_SUMMON_CREATURE_NO_CREATURE"; // For revive dead card but not dead cards.

const CARD_ABILITY_CHARGE = 0;
const CARD_ABILITY_TAUNT = 1;
const CARD_ABILITY_SHIELD = 2;
const CARD_ABILITY_BOOST_FRIENDLY_ATTACK_BY_TEN = 3;
const CARD_ABILITY_BATTLE_CRY_DRAW_CARD = 4;
const CARD_ABILITY_LIFE_STEAL = 5;
const CARD_ABILITY_DEATH_RATTLE_DRAW_CARD = 6;
const CARD_ABILITY_END_TURN_HEAL_TEN = 7;
const CARD_ABILITY_END_TURN_HEAL_TWENTY = 8;
const CARD_ABILITY_END_TURN_DRAW_CARD = 9;
const CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN = 10;
const CARD_ABILITY_EACH_KILL_DRAW_CARD = 11;
const CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY = 12;
const CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN = 13;
const CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY = 14;
const CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN = 15;
const CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY = 16;
const CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY = 17;
const CARD_ABILITY_BATTLE_CRY_SILENCE_IN_FRONT = 18;
const CARD_ABILITY_BATTLE_CRY_HEAL_FRIENDLY_MAX = 19;
const CARD_ABILITY_BATTLE_CRY_TAUNT_ADJACENT_FRIENDLY = 20;
const CARD_ABILITY_LETHAL = 21;
const CARD_ABILITY_ATTACK_DAMAGE_ADJACENT_BY_TEN = 22;
const CARD_ABILITY_ATTACK_DAMAGE_ADJACENT_BY_ATTACK = 23;
const CARD_ABILITY_BATTLE_CRY_DAMAGE_PLAYER_FACE_BY_TWENTY = 24;
const CARD_ABILITY_END_TURN_BOTH_PLAYERS_DRAW_CARD = 25;
const CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_TWENTY = 26;
const CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_FOURTY = 27;
const CARD_ABILITY_BATTLE_CRY_HEAL_ALL_CREATURES_BY_FOURTY = 28;
const CARD_ABILITY_BATTLE_CRY_REVIVE_HIGHEST_COST_CREATURE = 29; //
const CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_OPPONENT_CREATURES_BY_TWENTY = 30;
const CARD_ABILITY_BATTLE_CRY_SILENCE_ALL_OPPONENT_CREATURES = 31;
const CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_CREATURES_BY_THIRTY = 32;

const VALID_ABILITIES = [
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
];
    
// Abilities that can be granted to other cards.
// If the grantor card dies, the granted abilities are lost.
const VALID_GRANTABLE_ABILITIES = [
    CARD_ABILITY_CHARGE,
    CARD_ABILITY_TAUNT,
    CARD_ABILITY_LIFE_STEAL,
];

const BUFF_CATEGORY_INCREMENT_ATTACK = 1000;
const BUFF_CATEGORY_UNSTABLE_POWER = 1001;

const TARGET_ID_FACE = "TARGET_ID_FACE";
