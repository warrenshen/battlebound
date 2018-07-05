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
const MOVE_CATEGORY_SURRENDER_BY_CHOICE = "MOVE_CATEGORY_SURRENDER_BY_CHOICE";
const MOVE_CATEGORY_SURRENDER_BY_EXPIRE = "MOVE_CATEGORY_SURRENDER_BY_EXPIRE";
const MOVE_CATEGORY_END_TURN = "MOVE_CATEGORY_END_TURN";
const MOVE_CATEGORY_DRAW_CARD = "MOVE_CATEGORY_DRAW_CARD";
const MOVE_CATEGORY_DRAW_CARD_FAILURE = "MOVE_CATEGORY_DRAW_CARD_FAILURE";
const MOVE_CATEGORY_PLAY_MINION = "MOVE_CATEGORY_PLAY_MINION";
const MOVE_CATEGORY_PLAY_SPELL_TARGETED = "MOVE_CATEGORY_PLAY_SPELL_TARGETED";
const MOVE_CATEGORY_PLAY_SPELL_UNTARGETED = "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED";
const MOVE_CATEGORY_CARD_ATTACK = "MOVE_CATEGORY_CARD_ATTACK";
