// ====================================================================================================
//
// Cloud Code for ChallengeEndTurn, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ChallengeEventPrefix");
require("DeckModule");
require("CancelScheduledTimeEventsModule");

// Note that the call below must be before the `challenge.consumeTurn()` call.
cancelScheduledTimeEvents(challengeId, playerId, challengeStateData);

const MOVE_TYPE_END_TURN = "MOVE_TYPE_END_TURN";

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

if (opponentDeck.length > 0) {
    const drawCardResponse = drawCard(opponentDeck);
    const drawnCard = drawCardResponse[0];
    const newDeck = drawCardResponse[1];
    const handSize = opponentState.hand.push(drawCardResponse[0]);

    opponentState.handSize = handSize;
    opponentState.deck = newDeck;
    opponentState.deckSize = newDeck.length;
    // TODO: somehow tell client/player whose turn is next which card was drawn.
}

// Set all cards on opponent's field to be able to attack.
opponentState.field.map(function(card) {
    card.canAttack = 1;
})

// Set `hasTurn` attributes in ChallengeState.
playerState.hasTurn = 0;
opponentState.hasTurn = 1;

const move = {
    playerId: playerId,
    type: MOVE_TYPE_END_TURN,
};
challengeStateData.moves.push(move);
challengeStateData.lastMoves = [move];

challengeStateData.turnCountByPlayerId[playerId] += 1;

require("PersistChallengeStateModule");

// Finish player turn (without sending a ChallengeTurnTaken message)
// since this will already be sent because this is a challenge event.
challenge.consumeTurn(playerId);

const scheduler = Spark.getScheduler();
const opponentRunningKey = "TROM" + ":" + challengeId + ":" + challengeStateData.turnCountByPlayerId[opponentId];
const opponentExpiredKey = "TLEM" + ":" + challengeId + ":" + challengeStateData.turnCountByPlayerId[opponentId];
const data = {
    challengeId: challengeId,
    opponentId: opponentId,
    hasTurnPlayerId: opponentId,
};
var success;
success = scheduler.inSeconds("TimeRunningOutModule", 30, data, opponentRunningKey);
success = scheduler.inSeconds("TimeLimitExpiredModule", 40, data, opponentExpiredKey);
