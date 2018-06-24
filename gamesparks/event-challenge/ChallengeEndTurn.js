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
require("ChallengeMovesModule");

// This is secure because the only thing a malicious actor
// can do is send end turn's on time expired for themselves -
// which would only be detrimental to themselves.
const isExpired = Spark.getData().isExpired;

// Note that the call below must be before the `challenge.consumeTurn()` call.
cancelScheduledTimeEvents(challengeId, playerId);

const challengeState = challengeStateData.current;

// PLAYER STATE UPDATES //
const playerState = challengeState[playerId];

// Increase player's max mana by 1 if it's not max'ed out.
if (playerState.manaMax < 10) {
    playerState.manaMax += 1;
}

// Set `hasTurn` attributes in ChallengeState.
playerState.hasTurn = 0;

// Reset `lastMoves` attribute in ChallengeState.
challengeStateData.lastMoves = [];

var move = {
    playerId: playerId,
    category: MOVE_CATEGORY_END_TURN,
};
challengeStateData.moves.push(move);
challengeStateData.lastMoves.push(move);

challengeStateData.turnCountByPlayerId[playerId] += 1;
// If this is a end turn request from time expired and no move
// has been taken this turn, increment player's expired streak.
if (isExpired && challengeStateData.moveTakenThisTurn === 0) {
    challengeStateData.expiredStreakByPlayerId[playerId] += 1;
    
    // If expired streak is greater than 2, auto-surrender player.
    if (challengeStateData.expiredStreakByPlayerId[playerId] > 2) {
        move = {
            playerId: playerId,
            category: MOVE_CATEGORY_SURRENDER_BY_EXPIRE,
        };
        challengeStateData.moves.push(move);
        challengeStateData.lastMoves.push(move);

        // TODO: log challenge winner in ChallengeState.
        const opponent = Spark.loadPlayer(opponentId);
        challenge.winChallenge(opponent);
    }
} else {
    challengeStateData.expiredStreakByPlayerId[playerId] = 0;
}

// OPPONENT STATE UPDATES //
const opponentState = challengeState[opponentId];

opponentState.manaCurrent = opponentState.manaMax;
opponentState.hasTurn = 1;

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
    
    move = {
        playerId: opponentId,
        category: MOVE_CATEGORY_DRAW_CARD,
        attributes: {
            card: drawnCard,
        },
    };
    challengeStateData.moves.push(move);
    challengeStateData.lastMoves.push(move);
}

// Set all cards on opponent's field to be able to attack.
opponentState.field.forEach(function(card) {
    if (card.id !== "EMPTY") {
        // TODO: maybe should not set to 1 for all cards.
        card.canAttack = 1;
    }
})

// Player with next turn should start with no move taken.
challengeStateData.moveTakenThisTurn = 0;

require("PersistChallengeStateModule");

// Finish player turn (without sending a ChallengeTurnTaken message)
// since this will already be sent because this is a challenge event.
challenge.consumeTurn(playerId);

const scheduler = Spark.getScheduler();
const opponentRunningKey = "TROM" + ":" + challengeId + ":" + opponentId;
const opponentExpiredKey = "TLEM" + ":" + challengeId + ":" + opponentId;
const data = {
    challengeId: challengeId,
    opponentId: opponentId,
    hasTurnPlayerId: opponentId,
};
var success;
success = scheduler.inSeconds("TimeRunningOutModule", 60, data, opponentRunningKey);
success = scheduler.inSeconds("TimeLimitExpiredModule", 75, data, opponentExpiredKey);
