// ====================================================================================================
//
// Cloud Code for CancelScheduledTimeEventsModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
function cancelScheduledTimeEvents(challengeId, playerId, challengeStateData) {
    const scheduler = Spark.getScheduler();
    // TROM = TimeRunningOutModule.
    // TLEM = TimeLimitExpiredModule.
    const runningKey = "TROM" + ":" + challengeId + ":" + challengeStateData.turnCountByPlayerId[playerId];
    const expiredKey = "TLEM" + ":" + challengeId + ":" + challengeStateData.turnCountByPlayerId[playerId];
    scheduler.cancel(runningKey);
    scheduler.cancel(expiredKey);
}
