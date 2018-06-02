// ====================================================================================================
//
// Cloud Code for ChallengeLostMessage, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

require("ProcessRank");

if (Spark.getData().challenge.shortCode === "RankedChallenge") {
    var playerId1 = Spark.getData().challenge.challenger.id;
    var playerId2 = Spark.getData().challenge.challenged[0].id;

    var loserPlayerId = Spark.getPlayer().getPlayerId();
    var winnerPlayerId = loserPlayerId != playerId1 ? playerId1 : playerId2;

    var newScores = processRank(winnerPlayerId, loserPlayerId);
    
    var currentPlayerId = Spark.getPlayer().getPlayerId();
    const newScore = newScores[currentPlayerId];
    setLeaderboardsScore(newScore);
}

require("ChallengeUserMessageSuffix");

require("CancelScheduledTimeEventsModule");
cancelScheduledTimeEvents(challengeId, playerId, challengeStateData);
