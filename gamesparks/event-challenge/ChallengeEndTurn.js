// ====================================================================================================
//
// Cloud Code for ChallengeEndTurn, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ChallengeEventPrefix");
require("DeckModule");

var challengeStateData = challengeStateDataItem.getData();
var currentChallengeState = challengeStateData.current;

var currentPlayerState = currentChallengeState[playerId];

// Increase player's max mana by 1 if it's not max'ed out.
if (currentPlayerState.manaMax < 10) {
    currentPlayerState.manaMax += 1;
}

// Reset opponent's current mana to its max mana.
var opponentId = challengeStateData.opponentIdByPlayerId[playerId];
var currentOpponentState = currentChallengeState[opponentId];
currentOpponentState.manaCurrent = currentOpponentState.manaMax;

// Draw a card for opponent to start its turn.
var currentOpponentDeck = currentOpponentState.deck;
var drawCardResponse = drawCard(currentOpponentDeck);
var drawnCard = drawCardResponse[0];
var newDeck = drawCardResponse[1];
var handSize = currentOpponentState.hand.push(drawCardResponse[0]);
currentOpponentState.handSize = handSize;
currentOpponentState.deck = newDeck;
currentOpponentState.deckSize = newDeck.length;
// TODO: somehow tell client which card was drawn.

var error = challengeStateDataItem.persistor().persist().error();
if (error) {
    Spark.setScriptError("ERROR", error);
    Spark.exit();
}

// //Get all the cards on the player's play field
// var cards = Object.keys(playField[pId]);
// //Use the allowAtk function on every card
// cards.forEach(allowAtk)

//Finish player turn
challenge.takeTurn(playerId);

function allowAtk(obj){
    //Set the canAtk value to true, to allow the card to attack next turn
    playField[pId][obj].canAtk = true;
}
