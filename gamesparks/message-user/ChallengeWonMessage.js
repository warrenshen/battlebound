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
cancelMulliganTimeEvents(challengeId);

const player = Spark.getPlayer();
player.removePrivateData("activeChallengeId");

const shortCode = Spark.getData().challenge.shortCode;

if (shortCode === "RankedChallenge") {
    var playerId1 = Spark.getData().challenge.challenger.id;
    var playerId2 = Spark.getData().challenge.challenged[0].id;

    var winnerPlayerId = Spark.getPlayer().getPlayerId();
    var loserPlayerId = winnerPlayerId != playerId1 ? playerId1 : playerId2;

    var newScores = processRank(winnerPlayerId, loserPlayerId);

    const newScore = newScores[winnerPlayerId];
    setLeaderboardsScore(newScore);
}

// Win-loss info begin.
const matchType = SHORT_CODE_TO_MATCH_TYPE[shortCode];
if (matchType == null) {
    setScriptError("Invalid short code: " + shortCode);
}

var infoByMatchType = player.getPrivateData("infoByMatchType");
if (!infoByMatchType) {
    infoByMatchType = {};
}

if (!infoByMatchType[matchType]) {
    infoByMatchType[matchType] = {
        winStreak: 0,
        wins: 0,
        losses: 0,
    };
}

const info = infoByMatchType[matchType];
info.wins += 1;
info.winStreak += 1;

player.setPrivateData("infoByMatchType", infoByMatchType);
// --

const experienceCards = grantExperienceByPlayerAndChallenge(playerId, challengeId);

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
