// ====================================================================================================
//
// Cloud Code for ChallengeEventPrefix, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
var API = Spark.getGameDataService();

var playerId = Spark.getPlayer().getPlayerId();
var challengeId = Spark.getData().challengeInstanceId;
var challenge = Spark.getChallenge(challengeId);

var challengeStateGetResult = API.getItem("ChallengeState", challengeId);
var challengeStateDataItem = challengeStateGetResult.document();

if (challengeStateDataItem === null) {
    Spark.setScriptError("ERROR", "Challenge state does not exist.");
    Spark.exit();
}
