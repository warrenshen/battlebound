// ====================================================================================================
//
// Cloud Code for ChallengeEndTurn, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ChallengeEventPrefix");
require("DeckModule");

const challengeState = challengeStateData.current;
const playerState = challengeState[playerId];

// Increase player's max mana by 1 if it's not max'ed out.
if (playerState.manaMax < 10) {
    playerState.manaMax += 1;
}

// Reset opponent's current mana to its max mana.
const opponentState = challengeState[opponentId];
opponentState.manaCurrent = opponentState.manaMax;


// Draw a card for opponent to start its turn.
const opponentDeck = opponentState.deck;
const drawCardResponse = drawCard(opponentDeck);
const drawnCard = drawCardResponse[0];
const newDeck = drawCardResponse[1];
const handSize = opponentState.hand.push(drawCardResponse[0]);

opponentState.handSize = handSize;
opponentState.deck = newDeck;
opponentState.deckSize = newDeck.length;
// TODO: somehow tell client/player whose turn is next which card was drawn.

// Set all cards on opponent's field to be able to attack.
opponentState.field.map(function(card) {
    card.canAttack = true;
})

// Set `hasTurn` attributes in ChallengeState.
playerState.hasTurn = false;
opponentState.hasTurn = true;

const error = challengeStateDataItem.persistor().persist().error();
if (error) {
    Spark.setScriptError("ERROR", error);
    Spark.exit();
}

// Finish player turn
challenge.takeTurn(playerId);
