// ====================================================================================================
//
// Challenge event for when player uses a card in its hand.
//
// ====================================================================================================
require("ChallengeEventPrefix");

/**
 * Security concerns:
 * - Is the Card played valid (attributes are not changed)?
 *   The Card is stored on server-side and cannot be tampered with.
 * - Does the player own the Card played?
 *   It could only be in the player's hand (stored server side) if so.
 **/
 
const CARD_TYPE_MINION = "CARD_TYPE_MINION";
const MOVE_TYPE_PLAY_MINION = "MOVE_TYPE_PLAY_MINION";

const cardId = Spark.getData().cardId;
const attributes = Spark.getData().attributes;

const challengeState = challengeStateData.current;

const playerState = challengeState[playerId];
const playerManaCurrent = playerState.manaCurrent;
const playerHand = playerState.hand;
const playerField = playerState.field;

// Find index of card played in hand.
const handIndex = playerHand.findIndex(function(card) { return card.id === cardId });
if (handIndex < 0) {
    Spark.setScriptError("ERROR", "Invalid cardId parameter");
    Spark.exit();
}

const playedCard = playerHand[handIndex];

if (playedCard.type === CARD_TYPE_MINION) {
    // Index at which to play card.
    const fieldIndex = attributes.fieldIndex;

    // Ensure that index to play card at is valid.
    if (fieldIndex < 0 || fieldIndex > playerField.length + 1) {
        Spark.setScriptError("ERROR", "Invalid fieldIndex parameter");
        Spark.exit();
    }
    
    playedCard.canAttack = false;
    
    const newHand = playerHand.slice(0, handIndex).concat(playerHand.slice(handIndex + 1));
    const newField = playerField.slice(0, fieldIndex).concat([playedCard]).concat(playerField.slice(fieldIndex));
    
    if (playedCard.manaCost > playerManaCurrent) {
        Spark.setScriptError("ERROR", "Card mana cost exceeds player's current mana.");
        Spark.exit();
    }
    
    playerState.manaCurrent = playerState.manaCurrent - playedCard.manaCost;
    playerState.hand = newHand;
    playerState.handSize = newHand.length;
    playerState.field = newField;
    
    const move = {
        playerId: playerId,
        type: MOVE_TYPE_PLAY_MINION,
        attributes: {
            cardId: cardId,
            fieldIndex: fieldIndex,
        },
    };
    challengeStateData.moves.push(move);
    challengeStateData.lastMoves = [move];
} else {
    if (error) {
        Spark.setScriptError("ERROR", "Unrecognized card type.");
        Spark.exit();
    }
}

require("PersistChallengeStateModule");
