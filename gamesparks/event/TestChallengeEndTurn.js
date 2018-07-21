// ====================================================================================================
//
// Cloud Code for TestChallengeEndTurn, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeMovesModule");
require("ChallengeEndTurnModule");

const challengeStateString = Spark.getData().challengeStateString;
const playerId = Spark.getData().challengePlayerId;
// const isExpired = Spark.getData().isExpired;

const challengeStateData = JSON.parse(challengeStateString);
handleChallengeEndTurn(challengeStateData, playerId);

Spark.setScriptData("challengeStateData", challengeStateData);
setScriptSuccess();
