// ====================================================================================================
//
// Cloud Code for ChallengePlayCard, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
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
 
var cardId = Spark.getData().cardId;
var attributes = Spark.getData().attributes;
// Index at which to play card.
var fieldIndex = attributes.fieldIndex;

var challengeState = challengeStateData.current;

var playerState = challengeState[playerId];
var playerManaCurrent = playerState.manaCurrent;
var playerHand = playerState.hand;
var playerField = playerState.field;

// Find index of card played in hand.
var handIndex = playerHand.findIndex(function(card) { return card.id === cardId });
if (handIndex < 0) {
    Spark.setScriptError("ERROR", "Invalid cardId parameter");
    Spark.exit();
}

// Ensure that index to play card at is valid.
if (fieldIndex < 0 || fieldIndex > playerField.length + 1) {
    Spark.setScriptError("ERROR", "Invalid fieldIndex parameter");
    Spark.exit();
}

var playedCard = playerHand[handIndex];
playedCard.canAttack = false;

var newHand = playerHand.slice(0, handIndex).concat(playerHand.slice(handIndex + 1));
var newField = playerField.slice(0, fieldIndex).concat([playedCard]).concat(playerField.slice(fieldIndex));

if (playedCard.manaCost > playerManaCurrent) {
    Spark.setScriptError("ERROR", "Card mana cost exceeds player's current mana.");
    Spark.exit();
}

playerState.manaCurrent = playerState.manaCurrent - playedCard.manaCost;
playerState.hand = newHand;
playerState.handSize = newHand.length;
playerState.field = newField;

var error = challengeStateDataItem.persistor().persist().error();
if (error) {
    Spark.setScriptError("ERROR", error);
    Spark.exit();
}
