// ====================================================================================================
//
// Challenge event for when player uses a card in its hand. This can be to move
// a card from the player's hand onto the field or to use the spell power of a
// card from the player's hand (which discards the card afterwards).
//
// Card categories:
// - 0: minion (play on field)
// - 1: spell (use and discard)
// - 2: structure (play on field)
// - 3: weapon (use and discard)
//
// Security concerns:
// - Is the Card played valid (attributes are not changed)?
//   The Card is stored on server-side and cannot be tampered with.
// - Does the player own the Card played?
//   It could only be in the player's hand (stored server side) if so.
//
// ====================================================================================================
require("ChallengeEventPrefix");
require("CardAbilitiesModule");
require("AttackModule");

const CARD_TYPE_MINION = 0;
const CARD_TYPE_SPELL = 1;
const CARD_TYPE_STRUCTURE = 2;

const MOVE_TYPE_PLAY_MINION = "MOVE_TYPE_PLAY_MINION";
const MOVE_TYPE_PLAY_SPELL = "MOVE_TYPE_PLAY_SPELL";

const cardId = Spark.getData().cardId;
const attributes = Spark.getData().attributes;

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
    Spark.setScriptError("ERROR", "Invalid cardId parameter");
    Spark.exit();
}

const playedCard = playerHand[handIndex];

if (playedCard.manaCost > playerManaCurrent) {
    Spark.setScriptError("ERROR", "Card mana cost exceeds player's current mana.");
    Spark.exit();
} else {
    playerState.manaCurrent = playerState.manaCurrent - playedCard.cost;
    if (!Number.isInteger(playerState.manaCurrent)) {
        Spark.setScriptError("ERROR", "Player mana current is no longer an int.");
        Spark.exit();
    }
}

var move;

if (playedCard.category === CARD_TYPE_MINION) {
    // Index at which to play card.
    const fieldIndex = attributes.fieldIndex;

    // Ensure that index to play card at is valid.
    if (fieldIndex < 0 || fieldIndex > playerField.length + 1) {
        Spark.setScriptError("ERROR", "Invalid fieldIndex parameter");
        Spark.exit();
    }
    
    if (!Array.isArray(playedCard.abilities)) {
        playedCard.abilities = [];
    }
    if (playedCard.abilities.indexOf(CARD_ABILITY_CHARGE) >= 0) {
        playedCard.canAttack = 1;
    } else {
        playedCard.canAttack = 0;
    }
    if (playedCard.abilities.indexOf(CARD_ABILITY_SHIELD) >= 0) {
        playedCard.hasShield = 1;
    } else {
        playedCard.hasShield = 0;
    }
    
    const newField = playerField.slice(0, fieldIndex).concat([playedCard]).concat(playerField.slice(fieldIndex));
    playerState.field = newField;
    
    move = {
        playerId: playerId,
        type: MOVE_TYPE_PLAY_MINION,
        attributes: {
            cardId: cardId,
            fieldIndex: fieldIndex,
        },
    };
} else if (playedCard.category === CARD_TYPE_SPELL) {
    if (playedCard.name === "Blizzard") {
        // For now, only spell that exists is to damage all enemy cards by 1.
        opponentField.forEach(function(card) { damageCard(card, 2) });
    } else {
        Spark.setScriptError("ERROR", "Unrecognized spell card name.");
        Spark.exit();
    }
    
    const newOpponentField = opponentField.filter(function(card) { return card.health > 0 });
    opponentState.field = newOpponentField;
    
    move = {
        playerId: playerId,
        type: MOVE_TYPE_PLAY_SPELL,
        attributes: {
            cardId: cardId,
        },
    };
} else {
    if (error) {
        Spark.setScriptError("ERROR", "Unrecognized card category.");
        Spark.exit();
    }
}

// Remove played card from hand.
const newHand = playerHand.slice(0, handIndex).concat(playerHand.slice(handIndex + 1));
playerState.hand = newHand;
playerState.handSize = newHand.length;
    
challengeStateData.moves.push(move);
challengeStateData.lastMoves = [move];
challengeStateData.moveTakenThisTurn = 0;
    
require("PersistChallengeStateModule");
