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
// ====================================================================================================
require("ChallengeEventPrefix");

const TARGET_ID_FACE = "TARGET_ID_FACE";
const MOVE_TYPE_CARD_ATTACK = "MOVE_TYPE_CARD_ATTACK";

const cardId = Spark.getData().cardId;
const attributes = Spark.getData().attributes;
// const fieldId = attributes.fieldId;
const targetId = attributes.targetId;

const challengeState = challengeStateData.current;

const playerState = challengeState[playerId];
const playerField = playerState.field;

const opponentState = challengeState[opponentId];
const opponentField = opponentState.field;

// Find index of attacking card in player field.
const attackingIndex = playerField.findIndex(function(card) { return card.id === cardId });
if (attackingIndex < 0) {
    Spark.setScriptError("ERROR", "Invalid cardId parameter");
    Spark.exit();
}

const attackingCard = playerField[attackingIndex];
// Check if card can actually attack - return error if not and
// set it to not be able to attack anymore after this if so.
if (attackingCard.canAttack === 0) {
    Spark.setScriptError("ERROR", "Card cannot attack anymore.");
    Spark.exit();
} else {
    attackingCard.canAttack = 0;
}

if (targetId === TARGET_ID_FACE) {
    opponentState.health -= attackingCard.attack;
    if (opponentState.health <= 0) {
        opponentState.health = 0;
        challenge.winChallenge(Spark.getPlayer());
    }
} else {
    // Find index of attacking card in opponent field.
    const defendingIndex = opponentField.findIndex(function(card) { return card.id === cardId });
    if (defendingIndex < 0) {
        Spark.setScriptError("ERROR", "Invalid targetId parameter");
        Spark.exit();
    }
    
    var defendingCard = opponentField[defendingIndex];
    
    attackingCard.health -= defendingCard.attack;
    defendingCard.health -= attackingCard.attack;
    
    if (attackingCard.health <= 0) {
        const newPlayerField = playerField.slice(0, attackingIndex).concat(playerField.slice(attackingIndex + 1));
        playerState.field = newPlayerField;
    }
    if (defendingCard.health <= 0) {
        const newOpponentField = opponentField.slice(0, defendingIndex).concat(opponentField.slice(defendingIndex + 1));
        opponentState.field = newOpponentField;
    }
}

const move = {
    playerId: playerId,
    type: MOVE_TYPE_CARD_ATTACK,
    attributes: {
        cardId: cardId,
        targetId: targetId,
    },
};
challengeStateData.moves.push(move);
challengeStateData.lastMoves = [move];

require("PersistChallengeStateModule");
