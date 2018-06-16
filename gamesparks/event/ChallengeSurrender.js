// ====================================================================================================
//
// Cloud Code for ChallengeSurrender, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
const API = Spark.getGameDataService();

const player = Spark.getPlayer();
const playerId = player.getPlayerId();
const challengeId = Spark.getData().challengeId;
const challenge = Spark.getChallenge(challengeId);

if (challenge.getRunState() != "RUNNING") {
    Spark.setScriptError("ERROR", "Challenge is not running.");
    Spark.exit();
}

const challengeStateDataItem = API.getItem("ChallengeState", challengeId).document();

if (challengeStateDataItem === null) {
    Spark.setScriptError("ERROR", "ChallengeState does not exist.");
    Spark.exit();
}

const challengeStateData = challengeStateDataItem.getData();
const opponentId = challengeStateData.opponentIdByPlayerId[playerId];

// TODO: log challenge winner in ChallengeState.

const opponent = Spark.loadPlayer(opponentId);
challenge.winChallenge(opponent);
