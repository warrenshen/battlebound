// ====================================================================================================
//
// Cloud Code for ChallengePlaySpellTargeted, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeEventPrefix");
require("CardAbilitiesModule");
require("AttackModule");
require("ChallengeMovesModule");

const cardId = Spark.getData().cardId;
const attributesJson = Spark.getData().attributesJson;
const attributesString = Spark.getData().attributesString;

const challengeState = challengeStateData.current;

const playerState = challengeState[playerId];
if (playerState.mode !== PLAYER_STATE_MODE_NORMAL) {
    setScriptError("Player state is not in normal mode.");
}

const playerManaCurrent = playerState.manaCurrent;
const playerHand = playerState.hand;
const playerField = playerState.field;

const opponentState = challengeState[opponentId];
const opponentField = opponentState.field;

// Find index of card played in hand.
const handIndex = playerHand.findIndex(function(card) { return card.id === cardId });
if (handIndex < 0) {
    setScriptError("Invalid cardId parameter");
}

const playedCard = playerHand[handIndex];

if (playedCard.cost > playerManaCurrent) {
    setScriptError("Card mana cost exceeds player's current mana.");
} else {
    playerState.manaCurrent -= playedCard.cost;
    if (!Number.isInteger(playerState.manaCurrent)) {
        setScriptError("Player mana current is no longer an int.");
    }
}

var attributes;

if (attributesJson.targetId) {
    attributes = attributesJson;
} else {
    attributes = JSON.parse(attributesString);
    if (!attributes.targetId) {
        setScriptError("Invalid attributesJson or attributesString parameter");
    }
}

if (playedCard.category !== CARD_CATEGORY_SPELL) {
    setScriptError("Invalid card category - must be spell category.");
}

// Reset `lastMoves` attribute in ChallengeState.
challengeStateData.lastMoves = [];
challengeStateData.moveTakenThisTurn = 1;

var move = {
    playerId: playerId,
    category: MOVE_CATEGORY_PLAY_SPELL_TARGETED,
    attributes: {
        card: playedCard,
        cardId: cardId,
        handIndex: handIndex,
    },
};
challengeStateData.moves.push(move);
challengeStateData.lastMoves = [move];

const targetId = attributes.targetId;
const fieldId = attributes.fieldId;

if (playedCard.name === "Lightning Bolt") {
    // if (fieldId !== playerId && fieldId !== opponentId) {
    if (fieldId !== opponentId) {
        setScriptError("Invalid fieldId parameter.");
    }
    
    // Find index of defending card in player field.
    defendingIndex = opponentField.findIndex(function(card) { return card.id === targetId });
    if (defendingIndex < 0) {
        setScriptError("Invalid targetId parameter - card does not exist.");
    }
    
    defendingCard = opponentField[defendingIndex];
    damageCard(defendingCard, 1);
} else if (playedCard.name === "Unstable Power") {
    if (fieldId !== playerId) {
        setScriptError("Invalid fieldId parameter.");
    }
    // Give a creature +30, it dies at start of next turn.
    card.attack += 30;
    card.buffs.push({
        category: BUFF_CATEGORY_UNSTABLE_POWER,
        granterId: playedCard.id,
        attack: 30,
        abilities: [],
    });
} else {
    setScriptError("Unrecognized spell card name.");
}

const filterDeadResponse = filterDeadCardsFromFields(playerField, opponentField);
playerState.field = filterDeadResponse[0];
opponentState.field = filterDeadResponse[1];

// Remove played card from hand.
const newHand = playerHand.slice(0, handIndex).concat(playerHand.slice(handIndex + 1));
playerState.hand = newHand;

require("PersistChallengeStateModule");

setScriptSuccess();
