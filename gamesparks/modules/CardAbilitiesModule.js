// ====================================================================================================
//
// Cloud Code for AbilitiesModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
const CARD_ABILITY_CHARGE = 0;
const CARD_ABILITY_TAUNT = 1;
const CARD_ABILITY_SHIELD = 2;
const CARD_ABILITY_BOOST_FRIENDLY_ATTACK_BY_ONE = 3;
const CARD_ABILITY_BATTLE_CRY_DRAW_CARD = 4;
const CARD_ABILITY_LIFE_STEAL = 5;
const CARD_ABILITY_DEATH_RATTLE_DRAW_CARD = 6;
const CARD_ABILITY_END_TURN_HEAL_TEN = 7;
const CARD_ABILITY_END_TURN_HEAL_TWENTY = 8;
const CARD_ABILITY_END_TURN_DRAW_CARD = 9;
const CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_TWENTY = 10;
const CARD_ABILITY_EACH_KILL_DRAW_CARD = 11;

const VALID_ABILITIES = [
    CARD_ABILITY_CHARGE,
    CARD_ABILITY_TAUNT,
    CARD_ABILITY_SHIELD,
    CARD_ABILITY_BOOST_FRIENDLY_ATTACK_BY_ONE,
    CARD_ABILITY_BATTLE_CRY_DRAW_CARD,
    CARD_ABILITY_LIFE_STEAL,
    CARD_ABILITY_DEATH_RATTLE_DRAW_CARD,
    CARD_ABILITY_END_TURN_HEAL_TEN,
    CARD_ABILITY_END_TURN_HEAL_TWENTY,
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
CARD_ABILITY_INT_TO_STRING[CARD_ABILITY_BOOST_FRIENDLY_ATTACK_BY_ONE] = "BOOST_FRIENDLY_ATTACK_BY_ONE";
