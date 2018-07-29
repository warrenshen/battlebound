// ====================================================================================================
//
// Cloud Code for TestChallengeGrantExperience, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeGrantExperienceModule");

const playerId = Spark.getData().challengePlayerId;
const challengeStateString = Spark.getData().challengeStateString;
const bCards = Spark.getData().bCardsJson;
const decksData = Spark.getData().decksDataJson;

const challengeStateData = JSON.parse(challengeStateString);
const expCardIds = challengeStateData.expCardIdsByPlayerId[playerId];

const response = handleGrantExperience(expCardIds, decksData, bCards);
const newDecksData = response[0];
const newBCards = response[1];
const expCards = response[2];

Spark.setScriptData("newDecksData", newDecksData);
Spark.setScriptData("newBCards", newBCards);
Spark.setScriptData("expCards", expCards);

setScriptSuccess();
