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

const challengeStateData = JSON.parse(challengeStateString);
handlePlaySpellUntargeted(challengeStateData, playerId, cardId);

Spark.setScriptData("challengeStateData", challengeStateData);
setScriptSuccess();
