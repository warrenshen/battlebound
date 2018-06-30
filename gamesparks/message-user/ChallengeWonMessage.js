// ====================================================================================================
//
// Cloud Code for ChallengeWonMessage, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("ProcessRankModule");
require("CancelScheduledTimeEventsModule");
require("ChallengeGrantExperienceModule");

// This needs to be up here since it defines the `challengeId`, `playerId`, and `challengeStateData` variables.
require("ChallengeUserMessageModule");

cancelScheduledTimeEvents(challengeId, playerId);

const player = Spark.getPlayer();
player.removePrivateData("activeChallengeId");

if (Spark.getData().challenge.shortCode === "RankedChallenge") {
    var playerId1 = Spark.getData().challenge.challenger.id;
    var playerId2 = Spark.getData().challenge.challenged[0].id;

    var winnerPlayerId = Spark.getPlayer().getPlayerId();
    var loserPlayerId = winnerPlayerId != playerId1 ? playerId1 : playerId2;

    var newScores = processRank(winnerPlayerId, loserPlayerId);

    const newScore = newScores[winnerPlayerId];
    setLeaderboardsScore(newScore);
}

const experienceCards = grantExperienceByPlayerAndChallenge(playerId, challengeStateData);

const level = player.getPrivateData("level");
const levelPrevious = level;

const challengeEndState = {
    playerId: playerId,
    level: level,
    levelPrevious: levelPrevious,
    experienceCards: experienceCards,
};

Spark.setScriptData("challengeEndState", challengeEndState);
setScriptSuccess();
