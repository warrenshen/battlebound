// ====================================================================================================
//
// Cloud Code for TestChallengePlaySpellUntargeted, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengePlaySpellUntargetedModule");

const challengeStateString = Spark.getData().challengeStateString;
const playerId = Spark.getData().challengePlayerId;
const cardId = Spark.getData().cardId;

// Turn on test mode.
GLOBAL_ENVIRONMENT = 1;

const challengeStateData = JSON.parse(challengeStateString);
handleChallengePlaySpellUntargeted(challengeStateData, playerId, cardId);

Spark.setScriptData("challengeStateData", challengeStateData);
setScriptSuccess();
