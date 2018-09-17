// ====================================================================================================
//
// Cloud Code for TestChallengeSurrender, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeSurrenderModule");

const challengeStateString = Spark.getData().challengeStateString;
const playerId = Spark.getData().challengePlayerId;

// Turn on test mode.
GLOBAL_ENVIRONMENT = 1;

const challengeStateData = JSON.parse(challengeStateString);
handleChallengeSurrender(challengeStateData, playerId);

Spark.setScriptData("challengeStateData", challengeStateData);
setScriptSuccess();
