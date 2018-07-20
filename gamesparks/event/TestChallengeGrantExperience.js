// ====================================================================================================
//
// Cloud Code for TestChallengeGrantExperience, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ChallengeGrantExperienceModule");

const API = Spark.getGameDataService();

const playerId = Spark.getData().pId;
const challengeId = Spark.getData().challengeId;
const challenge = Spark.getChallenge(challengeId);

const challengeStateDataItem = API.getItem("ChallengeState", challengeId).document();

if (challengeStateDataItem === null) {
    Spark.setScriptError("ERROR", "ChallengeState does not exist.");
    Spark.exit();
}

const challengeStateData = challengeStateDataItem.getData();

const resultCards = grantExperienceByPlayerAndChallenge(playerId, challengeStateData);

Spark.setScriptData("resultCards", resultCards);
Spark.setScriptData("responseCode", 200);
