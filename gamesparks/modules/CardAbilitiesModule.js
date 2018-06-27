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

const VALID_ABILITIES = [
    CARD_ABILITY_CHARGE,
    CARD_ABILITY_TAUNT,
    CARD_ABILITY_SHIELD,
    CARD_ABILITY_BOOST_FRIENDLY_ATTACK_BY_ONE,
];

const VALID_GRANTABLE_ABILITIES = [
    CARD_ABILITY_CHARGE,
    CARD_ABILITY_TAUNT,
];

const CARD_ABILITY_INT_TO_STRING = {};
CARD_ABILITY_INT_TO_STRING[CARD_ABILITY_CHARGE] = "CHARGE";
CARD_ABILITY_INT_TO_STRING[CARD_ABILITY_TAUNT] = "TAUNT";
CARD_ABILITY_INT_TO_STRING[CARD_ABILITY_SHIELD] = "SHIELD";
CARD_ABILITY_INT_TO_STRING[CARD_ABILITY_BOOST_FRIENDLY_ATTACK_BY_ONE] = "BOOST_FRIENDLY_ATTACK_BY_ONE";
