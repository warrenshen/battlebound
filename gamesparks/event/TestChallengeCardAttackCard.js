// ====================================================================================================
//
// Cloud Code for TestChallengeCardAttackCard, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeCardAttackCardModule");

const challengeStateString = Spark.getData().challengeStateString;
const playerId = Spark.getData().challengePlayerId;
const cardId = Spark.getData().cardId;
const attributesJson = Spark.getData().attributesJson;

const challengeStateData = JSON.parse(challengeStateString);
handleChallengeCardAttackCard(challengeStateData, playerId, cardId, attributesJson);

Spark.setScriptData("challengeStateData", challengeStateData);
setScriptSuccess();
