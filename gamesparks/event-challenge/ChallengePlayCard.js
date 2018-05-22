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

var challengeStateData = challengeStateDataItem.getData();
var currentChallengeState = challengeStateData.current;

var currentPlayerState = currentChallengeState[playerId];
var currentPlayerHand = currentPlayerState.hand;
var currentPlayerManaCurrent = currentPlayerState.manaCurrent;
var currentPlayerField = currentPlayerState.field;

// Find index of card played in hand.
var cardIndex = currentPlayerHand.findIndex(function(card) { return card.id === cardId });
if (cardIndex < 0) {
    Spark.setScriptError("ERROR", "Invalid cardId parameter");
    Spark.exit();
}

if (fieldIndex < 0 || fieldIndex > currentPlayerField.length + 1) {
    Spark.setScriptError("ERROR", "Invalid fieldIndex parameter");
    Spark.exit();
}

var playedCard = currentPlayerHand[cardIndex];
var newHand = currentPlayerHand.slice(0, cardIndex).concat(currentPlayerHand.slice(cardIndex + 1));
var newField = currentPlayerField.slice(0, fieldIndex).concat([playedCard]).concat(currentPlayerField.slice(fieldIndex));

if (playedCard.manaCost > currentPlayerManaCurrent) {
    Spark.setScriptError("ERROR", "Card mana cost exceeds player's current mana.");
    Spark.exit();
}

currentPlayerState.manaCurrent = currentPlayerState.manaCurrent - playedCard.manaCost;
currentPlayerState.hand = newHand;
currentPlayerState.handSize = newHand.length;
currentPlayerState.field = newField;

var error = challengeStateDataItem.persistor().persist().error();
if (error) {
    Spark.setScriptError("ERROR", error);
    Spark.exit();
}
