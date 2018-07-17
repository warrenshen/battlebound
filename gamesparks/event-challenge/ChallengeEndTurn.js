// ====================================================================================================
//
// Cloud Code for ChallengeEndTurn, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeEventPrefix");
require("ChallengeEndTurnModule");

// This is secure because the only thing a malicious actor
// can do is send end turn's on time expired for themselves -
// which would only be detrimental to themselves.
const isExpired = Spark.getData().isExpired;

// Note that the call below must be before the `challenge.consumeTurn()` call.
cancelScheduledTimeEvents(challengeId, playerId);

// Used to determine whether to send
// time expiring messages for opponent.
var isChallengeOver = false;

// If this is an end turn request from time expired and no move
// has been taken this turn, increment player's expired streak.
if (isExpired && challengeStateData.moveTakenThisTurn === 0) {
    challengeStateData.expiredStreakByPlayerId[playerId] += 1;
    
    // If expired streak is greater than 2, auto-surrender player.
    if (challengeStateData.expiredStreakByPlayerId[playerId] > 2) {
        move = {
            playerId: playerId,
            category: MOVE_CATEGORY_SURRENDER_BY_EXPIRE,
        };
        addChallengeMove(challengeStateData, move);

        // TODO: log challenge winner in ChallengeState.
        const opponent = Spark.loadPlayer(opponentId);
        challenge.winChallenge(opponent);
        isChallengeOver = true;
    }
} else {
    challengeStateData.expiredStreakByPlayerId[playerId] = 0;
}

if (!isChallengeOver) {
    handleChallengeEndTurnEvent(challengeStateData, playerId);
    
    // Finish player turn (without sending a ChallengeTurnTaken message)
    // since this will already be sent because this is a challenge event.
    challenge.consumeTurn(playerId);
    
    // We only send these time expiring messages if challenge is not over.
    const scheduler = Spark.getScheduler();
    const opponentRunningKey = "TROM" + ":" + challengeId + ":" + opponentId;
    const opponentExpiredKey = "TLEM" + ":" + challengeId + ":" + opponentId;
    const data = {
        challengeId: challengeId,
        opponentId: opponentId,
        hasTurnPlayerId: opponentId,
    };
    // var success;
    // success = scheduler.inSeconds("TimeRunningOutModule", 60, data, opponentRunningKey);
    // success = scheduler.inSeconds("TimeLimitExpiredModule", 75, data, opponentExpiredKey);
}

require("PersistChallengeStateModule");

setScriptSuccess();
