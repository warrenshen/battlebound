// ====================================================================================================
//
// Challenge event for when player uses a card on its field to attack a card on opponent's field
// or the opponent's "face" directly. Without any special effects, in the card vs card situation
// the attacking card takes X damage and the defending card takes Y damage - where X is the
// defending card's attack and Y is the attacking card's attack. Any card whose health drop to
// less than equal to zero is taken off the field. Without any special effects, in the card vs
// face situation the opponent's health is decreased by the attacking card's attack and the
// attacking card is unaffected.
//
// Attributes:
// - targetId: card ID of card to attack or TARGET_ID_FACE
// - fieldId: 0 => player's field (friendly fire), 1 => opponent's field
//
// ====================================================================================================
require("ChallengeEventPrefix");
require("CardAbilitiesModule");
require("AttackModule");

const TARGET_ID_FACE = "TARGET_ID_FACE";
const MOVE_TYPE_CARD_ATTACK = "MOVE_TYPE_CARD_ATTACK";

const cardId = Spark.getData().cardId;
const attributes = Spark.getData().attributes;
const fieldId = attributes.fieldId;
const targetId = attributes.targetId;

const challengeState = challengeStateData.current;

const playerState = challengeState[playerId];
const playerField = playerState.field;

const opponentState = challengeState[opponentId];
const opponentField = opponentState.field;

if (fieldId !== 0 && fieldId !== 1) {
    Spark.setScriptError("ERROR", "Invalid fieldId parameter.");
    Spark.exit();
}

// Find index of attacking card in player field.
const attackingIndex = playerField.findIndex(function(card) { return card.id === cardId });
if (attackingIndex < 0) {
    Spark.setScriptError("ERROR", "Invalid cardId parameter.");
    Spark.exit();
}

const attackingCard = playerField[attackingIndex];
// Check if card can actually attack - return error if not and update if so.
if (attackingCard.canAttack <= 0) {
    Spark.setScriptError("ERROR", "Card cannot attack anymore.");
    Spark.exit();
} else {
    attackingCard.canAttack -= 1;
}

var defendingIndex;
var defendingCard;
var newPlayerField;
var newOpponentField;

// Friendly fire.
if (fieldId === 0) {
    if (targetId === TARGET_ID_FACE) {
        Spark.setScriptError("ERROR", "Invalid fieldId parameter - cannot attack self.");
        Spark.exit();
    } else {
        // Find index of defending card in player field.
        defendingIndex = playerField.findIndex(function(card) { return card.id === targetId });
        if (defendingIndex < 0) {
            Spark.setScriptError("ERROR", "Invalid targetId parameter - card does not exist.");
            Spark.exit();
        } else if (attackingIndex === defendingIndex) {
            Spark.setScriptError("ERROR", "Invalid targetId parameter - cannot attack self.");
            Spark.exit();
        }
        
        defendingCard = playerField[defendingIndex];
        
        attackingCard.health -= defendingCard.attack;
        defendingCard.health -= attackingCard.attack;
        
        newPlayerField = playerField.filter(function(card) { return card.health > 0 });
        playerState.field = newPlayerField;
    }
} else {
    const tauntCards = opponentField.filter(function(card) { return card.abilities && card.abilities.indexOf(CARD_ABILITY_TAUNT) >= 0 });
    const tauntIds = tauntCards.map(function(card) { return card.id });
    
    // `targetId` must be of a taunt card if any exist.
    if (tauntIds.length > 0 && tauntIds.indexOf(targetId) < 0) {
        Spark.setScriptError("ERROR", "Invalid targetId parameter - taunt cards exist.");
        Spark.exit();
    }
        
    if (targetId === TARGET_ID_FACE) {
        opponentState.health -= attackingCard.attack;
        if (opponentState.health <= 0) {
            opponentState.health = 0;
            challenge.winChallenge(Spark.getPlayer());
        }
    } else {
        // Find index of defending card in opponent field.
        defendingIndex = opponentField.findIndex(function(card) { return card.id === targetId });
        if (defendingIndex < 0) {
            Spark.setScriptError("ERROR", "Invalid targetId parameter - card does not exist.");
            Spark.exit();
        }
        
        defendingCard = opponentField[defendingIndex];
        
        damageCard(defendingCard, attackingCard.attack);
        damageCard(attackingCard, defendingCard.attack);
        
        newPlayerField = playerField.filter(function(card) { return card.health > 0 });
        playerState.field = newPlayerField;
        newOpponentField = opponentField.filter(function(card) { return card.health > 0 });
        opponentState.field = newOpponentField;
    }
}

const move = {
    playerId: playerId,
    type: MOVE_TYPE_CARD_ATTACK,
    attributes: {
        cardId: cardId,
        fieldId: fieldId,
        targetId: targetId,
    },
};
challengeStateData.moves.push(move);
challengeStateData.lastMoves = [move];

require("PersistChallengeStateModule");
