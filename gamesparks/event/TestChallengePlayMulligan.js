// ====================================================================================================
//
// Cloud Code for TestChallengePlayMulligan, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengePlayMulliganModule");

const challengeStateString = Spark.getData().challengeStateString;
const playerId = Spark.getData().challengePlayerId;
const cardIds = Spark.getData().cardIds;

const challengeStateData = JSON.parse(challengeStateString);
handleChallengePlayMulligan(challengeStateData, playerId, cardIds);

Spark.setScriptData("challengeStateData", challengeStateData);
setScriptSuccess();
