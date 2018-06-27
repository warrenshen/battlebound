// ====================================================================================================
//
// Cloud Code for ChallengeLostMessage, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ProcessRankModule");
require("CancelScheduledTimeEventsModule");
require("ChallengeGrantExperienceModule");

// This needs to be up here since it defines the `challengeId`, `playerId`, and `challengeStateData` variables.
require("ChallengeUserMessageModule");

cancelScheduledTimeEvents(challengeId, playerId);

player.removePrivateData("activeChallengeId");

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

const resultCards = grantExperienceByPlayerAndChallenge(playerId, challengeStateData);
Spark.setScriptData("resultCards", resultCards);
