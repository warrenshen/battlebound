// ====================================================================================================
//
// Cloud Code for TestChallengePlayCard, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengePlayCardModule");

const challengeStateString = Spark.getData().challengeStateString;
const playerId = Spark.getData().challengePlayerId;
const cardId = Spark.getData().cardId;
const attributesJson = Spark.getData().attributesJson;

// Turn on test mode.
GLOBAL_ENVIRONMENT = 1;

const challengeStateData = JSON.parse(challengeStateString);
challengeStateData.lastMoves = [];
handleChallengePlayCard(challengeStateData, playerId, cardId, attributesJson);

Spark.setScriptData("challengeStateData", challengeStateData);
setScriptSuccess();
