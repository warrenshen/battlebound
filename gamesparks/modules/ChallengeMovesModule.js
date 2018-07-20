// ====================================================================================================
//
// Cloud Code for ChallengeMovesModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
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
const MOVE_CATEGORY_DRAW_CARD_FAILURE = "MOVE_CATEGORY_DRAW_CARD_FAILURE";
const MOVE_CATEGORY_PLAY_MINION = "MOVE_CATEGORY_PLAY_MINION";
const MOVE_CATEGORY_PLAY_SPELL_TARGETED = "MOVE_CATEGORY_PLAY_SPELL_TARGETED";
const MOVE_CATEGORY_PLAY_SPELL_UNTARGETED = "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED";
const MOVE_CATEGORY_CARD_ATTACK = "MOVE_CATEGORY_CARD_ATTACK";
const MOVE_CATEGORY_DEATH_RATTLE_ATTACK_RANDOM_TARGET = "MOVE_CATEGORY_DEATH_RATTLE_ATTACK_RANDOM_TARGET";

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
const CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY = 10;
const CARD_ABILITY_EACH_KILL_DRAW_CARD = 11;
const CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY = 12;
const CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN = 13;
const CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY = 14;
const CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN = 15;
const CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY = 16;
const CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY = 17;

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
    CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY,
    CARD_ABILITY_EACH_KILL_DRAW_CARD,
    CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY,
    CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN,
    CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY,
    CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN,
    CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY,
];
    
// Abilities that can be granted to other cards.
// If the grantor card dies, the granted abilities are lost.
const VALID_GRANTABLE_ABILITIES = [
    CARD_ABILITY_CHARGE,
    CARD_ABILITY_TAUNT,
    CARD_ABILITY_LIFE_STEAL,
];

const CARD_ABILITY_INT_TO_STRING = {};
CARD_ABILITY_INT_TO_STRING[CARD_ABILITY_CHARGE] = "CHARGE";
CARD_ABILITY_INT_TO_STRING[CARD_ABILITY_TAUNT] = "TAUNT";
CARD_ABILITY_INT_TO_STRING[CARD_ABILITY_SHIELD] = "SHIELD";
CARD_ABILITY_INT_TO_STRING[CARD_ABILITY_BOOST_FRIENDLY_ATTACK_BY_TEN] = "CARD_ABILITY_BOOST_FRIENDLY_ATTACK_BY_TEN";
CARD_ABILITY_INT_TO_STRING[CARD_ABILITY_BATTLE_CRY_DRAW_CARD] = "CARD_ABILITY_BATTLE_CRY_DRAW_CARD";
CARD_ABILITY_INT_TO_STRING[CARD_ABILITY_LIFE_STEAL] = "CARD_ABILITY_LIFE_STEAL";
CARD_ABILITY_INT_TO_STRING[CARD_ABILITY_DEATH_RATTLE_DRAW_CARD] = "CARD_ABILITY_DEATH_RATTLE_DRAW_CARD";
CARD_ABILITY_INT_TO_STRING[CARD_ABILITY_END_TURN_HEAL_TEN] = "CARD_ABILITY_END_TURN_HEAL_TEN";
CARD_ABILITY_INT_TO_STRING[CARD_ABILITY_END_TURN_HEAL_TWENTY] = "CARD_ABILITY_END_TURN_HEAL_TWENTY";
CARD_ABILITY_INT_TO_STRING[CARD_ABILITY_END_TURN_DRAW_CARD] = "CARD_ABILITY_END_TURN_DRAW_CARD";
CARD_ABILITY_INT_TO_STRING[CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY] = "CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY";
CARD_ABILITY_INT_TO_STRING[CARD_ABILITY_EACH_KILL_DRAW_CARD] = "CARD_ABILITY_EACH_KILL_DRAW_CARD";

const BUFF_CATEGORY_INCREMENT_ATTACK = 1000;
const BUFF_CATEGORY_UNSTABLE_POWER = 1001;

const TARGET_ID_FACE = "TARGET_ID_FACE";
