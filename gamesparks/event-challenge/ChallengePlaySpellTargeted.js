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

const targetId = attributes.targetId;
const fieldId = attributes.fieldId;

// if (playedCard.name === "Blizzard") {
//     // For now, only spell that exists is to damage all enemy cards by 1.
//     opponentField.forEach(function(card) { damageCard(card, 2) });
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
} else {
    setScriptError("Unrecognized spell card name.");
}

const deadCards = opponentField.filter(function(card) { return card.health <= 0 });
const deadCardIds = deadCards.map(function(card) { return card.id });

const newFields = filterDeadCardsFromFields(playerField, opponentField);
playerState.field = newFields[0];
opponentState.field = newFields[1];

move = {
    playerId: playerId,
    category: MOVE_CATEGORY_PLAY_SPELL_TARGETED,
    attributes: {
        card: playedCard,
        cardId: cardId,
        handIndex: handIndex,
    },
};

// Remove played card from hand.
const newHand = playerHand.slice(0, handIndex).concat(playerHand.slice(handIndex + 1));
playerState.hand = newHand;
    
challengeStateData.moves.push(move);
challengeStateData.lastMoves = [move];
challengeStateData.moveTakenThisTurn = 1;
    
require("PersistChallengeStateModule");

setScriptSuccess();
