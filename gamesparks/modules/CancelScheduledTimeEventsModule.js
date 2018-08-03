// ====================================================================================================
//
// Cloud Code for CancelScheduledTimeEventsModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
function startTurnTimeEvents(challengeId, playerId) {
    const scheduler = Spark.getScheduler();
    const runningKey = "TROM" + ":" + challengeId + ":" + playerId;
    const expiredKey = "TLEM" + ":" + challengeId + ":" + playerId;
    const data = {
        category: TIME_LIMIT_CATEGORY_NORMAL,
        challengeId: challengeId,
        opponentId: playerId,
        hasTurnPlayerId: playerId,
    };
    
    var success;
    success = scheduler.inSeconds("TimeRunningOutModule", 45, data, runningKey);
    success = scheduler.inSeconds("TimeLimitExpiredModule", 60, data, expiredKey);
}

function cancelScheduledTimeEvents(challengeId, playerId) {
    const scheduler = Spark.getScheduler();
    // TROM = TimeRunningOutModule.
    // TLEM = TimeLimitExpiredModule.
    const runningKey = "TROM" + ":" + challengeId + ":" + playerId;
    const expiredKey = "TLEM" + ":" + challengeId + ":" + playerId;
    scheduler.cancel(runningKey);
    scheduler.cancel(expiredKey);
}

function startMulliganTimeEvents(challengeId, playerIds) {
    const scheduler = Spark.getScheduler();
    const runningKey = "TROM" + ":" + challengeId + ":" + "MULLIGAN";
    const expiredKey = "TLEM" + ":" + challengeId + ":" + "MULLIGAN";
    const data = {
        category: TIME_LIMIT_CATEGORY_MULLIGAN,
        challengeId: challengeId,
        playerIds: playerIds,
    };
    var success;
    success = scheduler.inSeconds("TimeRunningOutModule", 45, data, runningKey);
    success = scheduler.inSeconds("TimeLimitExpiredModule", 60, data, expiredKey);
}

function cancelMulliganTimeEvents(challengeId) {
    const scheduler = Spark.getScheduler();
    // TROM = TimeRunningOutModule.
    // TLEM = TimeLimitExpiredModule.
    const runningKey = "TROM" + ":" + challengeId + ":" + "MULLIGAN";
    const expiredKey = "TLEM" + ":" + challengeId + ":" + "MULLIGAN";
    scheduler.cancel(runningKey);
    scheduler.cancel(expiredKey);
}
